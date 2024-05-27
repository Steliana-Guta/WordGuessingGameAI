using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace WordGuessingGameAI
{
    internal class Display
    {
        /// <summary>
        /// This is the secret word the user has to guess, 
        /// this will keep changing as the user attempts guesses
        /// </summary>
        private static string hiddenWord;

        /// <summary>
        /// This will hold the number of guesses the user has made so far
        /// </summary>
        private static int guessesTried;

        /// <summary>
        /// This will hold the maximum number of guesses the user is allowed 
        /// calculated as the double fo the word length
        /// </summary>
        private static int guessesTotal;

        /// <summary>
        /// This will keep the count of guesses left for the user to try 
        /// </summary>
        private static int guessesLeft => guessesTotal - guessesTried;

        /// <summary>
        /// Creates object for initialising random integer values 
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// This is used for the display how many words are in the word family as they play
        /// IF THE displayWordListLength HAS BEEN MARKED AS TRUE
        /// </summary>
        static int posCount = 1;

        /// <summary>
        /// Char list collection for the tried letters by user 
        /// </summary>
        public static List<char> triedLetters = new List<char>();

        /// <summary>
        /// Disctionary containing the count of all correct guesses
        /// </summary>
        public static Dictionary<char, int> correctHiddenLetters = new Dictionary<char, int>();

        /// <summary>
        /// Ths will contain the 'masked' word containing any guessed letters such as '--e--s'
        /// </summary>
        private static string maskedWord;

        /// <summary>
        /// List of word family, words similar to the 'hiddenWord' that it could be swapped with
        /// </summary>
        public static List<string> wordFamily;

        /// <summary>
        /// Method that returns the masked word checking for tried letters 
        /// </summary>
        /// <returns></returns> Returns masked word
        private static string GetMaskedForm()
            => new string(hiddenWord.Select(x => triedLetters.Contains(x) ? x : '-').ToArray());

        /// <summary>
        /// Presends game, picks a hidden word, displays masked word, display number of guesses allowed 
        /// Once no more hashes present in masked word, calls AskForRetry method
        /// </summary>
        public static void StartGame()
        {
            Console.Clear();
            //Greet the player with and introduce the game
            Console.WriteLine("Let's play a game...\n");
            Console.WriteLine("I, the PC will select a word.");

            //Get a random word of ranged length from the dictionary
            GetWordFromDictionary();
            //Get the number of guesses allowed in the game
            guessesTotal = (hiddenWord.Length) * 2;

            //Give user a overview of the choices they made
            Console.Clear();
            Console.WriteLine($"You have to guess a word of length {hiddenWord.Length}");
            Console.WriteLine($"You have {guessesTotal} guesses. Good Luck!");
            Console.WriteLine("Press any key to continue...");
            Console.Title = "Guess the word";

            Console.ReadKey();

            //Clear list containing tried letters
            triedLetters.Clear();
            correctHiddenLetters.Clear();

            //Ask the user for letters untill they run out of tries
            for (guessesTried = 0; guessesTried < guessesTotal && maskedWord.Contains("-"); guessesTried++)
            {
                maskedWord = GetMaskedForm();
                SetDisplay();
            }
            //If there are no more hashed letters user wins
            if (!maskedWord.Contains("-"))
            {
                Console.WriteLine("\n\nYou are correct!!");
                Console.WriteLine($"\n\nThe word was {hiddenWord}.");
                //Time variable to pause screen for user to see above write line
                var stopwatch = Stopwatch.StartNew();
                //Pause for 3 seconds
                Thread.Sleep(3000);
                //Stop the pause once 3 sencods pass
                stopwatch.Stop();
            }
            else
            {
                Console.WriteLine("\n\n\nYou lost! Let me get ready for another one!");
                Console.WriteLine($"\n\nThe word was {hiddenWord}.");
                //Time variable to pause screen for user to see above write line
                var stopwatch = Stopwatch.StartNew();
                //Pause for 3 seconds
                Thread.Sleep(3000);
                //Stop the pause once 3 sencods pass
                stopwatch.Stop();
            }
            
            
        }

        /// <summary>
        /// This will be the rules used to change the hidden word and will be keeping count od word family count if displayed
        /// </summary>
        private static void RunWordFamilyRules()
        {
            //
            var newWordFamily = WordProcesses.GetMinMaxFamily(hiddenWord, maskedWord);

            if (newWordFamily == null) return;
            if (newWordFamily.Count() != 0)
            {
                hiddenWord = WordProcesses.minMaxWord;
                wordFamily = newWordFamily.ToList();
                posCount = wordFamily.Count;
            }
        }

        /// <summary>
        /// Retrieves hidden word from dictionary, if requested will show the word family count 
        /// Recursive method that makes sure the input is valid and critiria is met
        /// </summary>
        private static void GetWordFromDictionary()
        {
            int wordLength = random.Next(4, 13);

            //Select a word of that random in range length
            try
            {
                //Select random secret word and generate maskedWord
                hiddenWord = WordProcesses.GetRandomWordOfLength(wordLength);
                maskedWord = GetMaskedForm();

                //Get word family so min max algorithm has fewer words to deal with
                wordFamily = WordProcesses.GetSimilarFamilyWords(maskedWord, hiddenWord).ToList();
                posCount = wordFamily.Count;
            }
            catch (Exception)
            {
                //If no word of randomly selected length has been successfuly selected from the dictionary
                Console.WriteLine("\nI do not know a word of that length!");
                Console.WriteLine("Please let me try again.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                //Retry method hoping this time it will find a word in range
                GetWordFromDictionary();
            }
        }

        /// <summary>
        /// Print the game display with all options and variables, checkes user input and validates guesses adding to the triedWods list 
        /// </summary>
        private static void SetDisplay()
        {
            if (!maskedWord.Contains("-")) return;

            Console.Clear();
            Console.WriteLine("\n\n          " + maskedWord + "\n\n");
            //Ordering the tried letters alphabetically for display purposes 
            triedLetters = triedLetters.OrderBy(x => x).ToList();
            //Prompt user with the letters available, letters tried and tries left
            Console.WriteLine("Possible Guesses: a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z.");
            Console.WriteLine($"You have Guessed: {string.Join(", ", triedLetters.Distinct())}");
            Console.WriteLine($"You have {guessesLeft} tries left.");

            //Empty variable for input
            char charInput = '/';

            //Ask user for letter if input was not a letter
            while (!char.IsLetter(Char.ToLower(charInput)))
            {
                Console.WriteLine("\nPlease input a letter of the alphabet");
                Console.Write("Guess a letter in the word: ");
                charInput = Console.ReadKey().KeyChar;
                Console.Write("   .... Please wait while I think about this."); //on first try sometimes it takes long to process,
                                                                                //this will stay on screen only when program is slow
            }
            //If the letter tried was already tried prompt user and pause for 3 seconds
            if (triedLetters.Contains(Char.ToLower(charInput)))
            {
                Console.WriteLine("\nYou have already guessed this!");
                //Not taking a try out for already tried letters
                guessesTried--;
                //Time variable for allowing short pause for user to see above write line
                var stopwatch = Stopwatch.StartNew();
                //Pause for 3 seconds
                Thread.Sleep(3000);
                //Continuing program once 3 sencods pass
                stopwatch.Stop();
            }
            //If guess is part of the hidden word
            if (hiddenWord.Contains(Char.ToLower(charInput)))
            {
                //Check if this is the last try and there are 2 available words left, use first
                if (wordFamily != null && guessesLeft == 1 && wordFamily.Count == 2)
                {
                    hiddenWord = wordFamily.Where(x => x != hiddenWord).First();
                }
                else
                {
                    //Cheat if character is guessed correctly
                    RunWordFamilyRules();
                }
                try
                {
                    correctHiddenLetters.Add(Char.ToLower(charInput), hiddenWord.Count(x => x == Char.ToLower(charInput)));
                    //Add letter to tried letters list
                    triedLetters.Add(Char.ToLower(charInput));
                    //Update number of word family total
                    posCount = wordFamily.Count() + 1;
                }
                //This will unfortunately run into exceptions quite often if a used letter is tried again
                //Or if multiple letters are typed simultaniously 
                catch (Exception)
                {
                    Console.WriteLine($"\nInvalid entry. Press ENTER to try again.");
                    Console.ReadLine();
                    //Add try back, otherwise it will be taken in account as valid guess
                    guessesTried++;
                    //Re-display game from last iteration
                    SetDisplay();
                }
            }
            //If guess is not part of the hidden word, then add tried letter to list
            else
            {
                triedLetters.Add(Char.ToLower(charInput));
            }
        }
    }
}
