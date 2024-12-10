using CrypticWizard.RandomWordGenerator;
using System.Diagnostics;
using static CrypticWizard.RandomWordGenerator.WordGenerator;
internal class Program
{
    static void Main(string[] args)
    {
        Game game = new();
        game.Launch();
    }
}
enum Difficulty
{
    EASY = 0,
    MEDIUM = 1,
    HARD = 2
}
internal struct Settings
{
    internal double scoreMultiplier;
    internal double timeLimit;
    public Settings(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.EASY:
                scoreMultiplier = 1;
                timeLimit = 10;
                break;
            case Difficulty.MEDIUM:
                scoreMultiplier = 2;
                timeLimit = 9;
                break;
            case Difficulty.HARD:
                scoreMultiplier = 3;
                timeLimit = 8;
                break;
        }
    }
}
internal class Game
{
    //Using cryptic wizards external library that creates random words
    //Can be found here: https://github.com/cryptic-wizard/random-word-generator
    WordGenerator wordgenerator = new WordGenerator();
    Stopwatch stopWatch = new Stopwatch();
    Dictionary<string, int> leaderboards = new();
    Difficulty difficulty;
    int totalPoints;
    string word = "";
    double timeLimit;

    private int AskDifficulty()
    {
        string[] options = { "Easy", "Medium", "Hard" };
        int currentIndex = 0;
        ConsoleKey keyPressed;
        do
        {
            Console.Clear(); //Clear the console window
            Console.WriteLine("Select Difficulty (Use Arrow Keys and Enter):");

            // Display options with a cursor
            for (int i = 0; i < options.Length; i++)
            {
                if (i == currentIndex)
                {
                    Console.WriteLine($"> {options[i]}"); // Highlight the selected option with >
                }
                else
                {
                    Console.WriteLine($"  {options[i]}"); // Normal display of option
                }
            }

            keyPressed = Console.ReadKey(true).Key;

            // Update currentIndex based on key press
            if (keyPressed == ConsoleKey.UpArrow)
            {
                currentIndex = (currentIndex - 1 + options.Length) % options.Length;
            }
            else if (keyPressed == ConsoleKey.DownArrow)
            {
                currentIndex = (currentIndex + 1) % options.Length;
            }
        }
        //Repeat until user presses enter
        while (keyPressed != ConsoleKey.Enter);
        return currentIndex;
    }

    private void PrintUI(string modifiedWord, ConsoleColor color)
    {
        Console.Clear(); //Clearing console
        Console.ResetColor(); //Resetting Color
        Console.WriteLine(difficulty);
        Console.WriteLine("Your sentence is: " + word);
        Console.Write("Type here: ");
        Console.ForegroundColor = color; //Changing the color to red if wrong input was given
        Console.Write(modifiedWord + "\n");
        Console.SetCursorPosition(11, 2); //Setting the cursor where the word starts
    }

    private void MakeEasyWord()
    {
        word = wordgenerator.GetWord(); // Getting a random word from the library
    }
    private void MakeMediumWord()
    {
        string word1 = "";
        string word2 = "";

        word1 = wordgenerator.GetWord();
        word2 = wordgenerator.GetWord();
        word = word1 + " " + word2;

    }
    private void MakeHardWord()
    {
        string word1 = "";
        string word2 = "";
        string word3 = "";
        string word4 = "";
        word1 = wordgenerator.GetWord(PartOfSpeech.noun);
        word2 = wordgenerator.GetWord(PartOfSpeech.verb) + "s";
        word3 = wordgenerator.GetWord(PartOfSpeech.adj);
        word4 = wordgenerator.GetWord(PartOfSpeech.noun);
        word = word1 + " " + word2 + " " + word3 + " " + word4;
    }

    private void GetWord()
    {
        switch (difficulty)
        {
            case Difficulty.EASY:
                MakeEasyWord();
                break;
            case Difficulty.MEDIUM:
                MakeMediumWord();
                break;
            case Difficulty.HARD:
                MakeHardWord();
                break;
        }
    }
    private int UpdateWord()
    {
        int inputTimes = 0; //Times user has inputted
        int extraTime = 0; //extraTime
        while (inputTimes < word.Length)//Repeat as many times as the word is long
        {
            char userInput = Console.ReadKey().KeyChar;

            if (userInput == word[inputTimes]) //Correct input
            {
                inputTimes++;
                string modifiedWord = word.Substring(inputTimes);
                PrintUI(modifiedWord, ConsoleColor.Gray);
            }
            else //Wrong input
            {
                extraTime++; //1 second penalty for writing a wrong character
                string modifiedWord = word.Substring(inputTimes);
                PrintUI(modifiedWord, ConsoleColor.Red);
            }
        }
        return extraTime;
    }
    private bool CalculateResults(double timeTaken, Settings settings)
    {
        if (timeTaken > settings.timeLimit)
        {
            Console.WriteLine("You lost");
            return false;
        }
        else
        {
            Console.WriteLine($"You earned {100 * settings.scoreMultiplier} points");
            totalPoints += 100 * (int)settings.scoreMultiplier;
            return true;
        }
    }
    private void PlayGame(Settings settings)
    {
        bool alive = true;
        totalPoints = 0;
        while (alive)
        {
            GetWord(); //Getting a word based on difficulty
            PrintUI(word, ConsoleColor.Gray); //Printing the interface
            stopWatch.Start();
            double extraTime = UpdateWord();
            stopWatch.Stop();
            double time = stopWatch.ElapsedMilliseconds;
            string userResults = extraTime == 0 ? $"\nYour time was: {Math.Round((time / 1000), 2)}s" : $"\nYour time was: {Math.Round((time / 1000), 2)}s + {extraTime}s penalty for incorrect input \n= {Math.Round((time / 1000 + extraTime), 2)}s";
            Console.WriteLine(userResults);
            alive = CalculateResults(time / 1000 + extraTime, settings);
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            stopWatch.Reset();
        }
    }
    public void Launch()
    {
        CreateLeaderboards();

        while (true)
        {
            difficulty = (Difficulty)AskDifficulty();
            PlayGame(new Settings(difficulty));
            Console.WriteLine("\nYou got: " + totalPoints + " points");
            CheckLeaderboards(CreatePlayerAccount());
            Console.WriteLine("Do you want to play again (y/n)");
            var playAgain = Console.ReadKey();
            if (playAgain.KeyChar != 'y')
            {
                break;
            }
        }
    }
    private string CreatePlayerAccount()
    {
        while (true)
        {
            Console.Write("Enter your PlayerTag: ");
            string playerName = Console.ReadLine();
            if (string.IsNullOrEmpty(playerName))
            {
                Console.WriteLine("Please enter a valid name.");
            }
            else
            {
                return playerName;
            }
        }

    }
    private void CreateLeaderboards()
    {
        leaderboards.Add("Testiteppo", 200);
        leaderboards.Add("Jussi", 150);
        leaderboards.Add("Matias", 400);
        leaderboards.Add("JK", 500);
        leaderboards.Add("Kunkku", 700);
    }
    private void OrderLeaderboards()
    {
        leaderboards = leaderboards.OrderByDescending(x => x.Value).ToDictionary();
    }
    private void ShowLeaderboards()
    {
        int index = 1;
        foreach (var player in leaderboards)
        {
            Console.WriteLine($"Name: {index}. Name: {player.Key} Score: {player.Value}");
            index++;

            if (index > 5)
            {
                break;
            }
        }
    }
    private void CheckLeaderboards(string playerName)
    {
        foreach (var player in leaderboards)
        {
            if (totalPoints > player.Value)
            {
                leaderboards.Add(playerName, totalPoints);
                Console.WriteLine("Congratulations you get to leaderboards!");
                break;
            }

        }
        OrderLeaderboards();
        ShowLeaderboards();
    }
}