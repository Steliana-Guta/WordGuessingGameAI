using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WordGuessingGameAI
{
    class Program
    {
        /// <summary>
        /// Main function, runs the Main Menu function until 
        /// </summary>  
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            bool runMainMenu = true;
            MainMenu(runMainMenu);
            while (runMainMenu)
            {
                runMainMenu = MainMenu(runMainMenu);
            }
        }

        /// <summary>
        /// Method will load the dictionary and present a menu with 3 options
        /// One for an EASY game, one for a HARD game, and one to exit
        /// </summary>
        /// 
        public static bool MainMenu( bool run)
        {
            //Prompt the user with a game loading message, this will likely be not seen
            //But might due to the largy dictionary file being loaded
            Console.Clear();
            Console.WriteLine("Game loading...");
            WordProcesses.LoadDictionary();
            Console.Clear();
            //Create variable to hold user selection
            char choice = '\0';
            try
            {
                while (choice != '1' && choice != '2' && choice != '3')
                {
                    // display menu
                    Console.Clear();
                    Console.WriteLine("Please select a game option: ");
                    Console.WriteLine("(1) EASY");
                    Console.WriteLine("(2) HARD - not working/not attempted");
                    Console.WriteLine("(3) Exit Program");
                    choice = Console.ReadKey().KeyChar;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"\nInvalid entry. Press ENTER to try again.");
                Console.ReadLine();
                throw;
            }

            //Route to menu selection
            switch (choice)
            {
                case '1':
                    Display.StartGame();
                    break;
                case '2':
                    Console.WriteLine("\nSorry this option is not ready yet. Please select another option once I reload this page.");
                    //Time variable to pause screen for user to see above write line
                    var stopwatch = Stopwatch.StartNew();
                    //Pause for 3 seconds
                    Thread.Sleep(3000);
                    //Stop the pause once 3 sencods pass
                    stopwatch.Stop();
                    //To not get stuck in loop call method again
                    MainMenu(true);
                    break;
                case '3':
                    Environment.Exit(0); 
                    return false;
                    break;
                default:
                    Console.WriteLine($"{choice} isn't a valid selection. Starting over.");
                    break;
            }
            return run;
        }
    }
}
