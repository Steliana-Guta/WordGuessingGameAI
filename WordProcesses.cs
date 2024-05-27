using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WordGuessingGameAI
{
    internal class WordProcesses
    {
        /// <summary>
        /// String List containing the words from the dictionary file 
        /// </summary>
        public static List<string> wordList;

        /// <summary>
        /// Creates object for initialising random integer values 
        /// </summary>
        private static Random random = null;

        /// <summary>
        /// Stores the last min max generated word 
        /// </summary>
        public static string minMaxWord = "";

        /// <summary>
        /// Method checks if word does not contain guessed letters in order to seelect similar words
        /// And check if the hidden word had a lot of guessed letters
        /// </summary>
        /// <param name="word"></param> //Iterating each possibility of a similar word for the word family
        /// <returns></returns> //Returns TRUE or FALSE if the word contains or not guessed letters
        public static bool RunWordSimilarityRules(string word)
        {
            bool output = true;

            //Check if word contains letters that have already been guessed (from the tried letters list)
            foreach (var c in Display.triedLetters)
            {
                output &= !word.Contains(c);
            }

            //Checks that the word has the same number of correct letters as the previous hidden word
            foreach (var keyValuePair in Display.correctHiddenLetters)
            {
                output &= word.Count(x => x == keyValuePair.Key) == keyValuePair.Value;
            }
            return output;
        }

        /// <summary>
        /// Retrieve all possible word families 
        /// </summary>
        /// <param name="word"></param> // Hidden word
        /// <param name="mask"></param> //Masked word
        /// <returns></returns> //Word familied in decending order 
        public static IEnumerable<IEnumerable<string>> GetWordFamilies(string word, string mask)
        {
            //Use words as word family if word family is uninitalized
            var words = Display.wordFamily;
            if (words == null)
                words = WordProcesses.wordList;

            List<IEnumerable<string>> output = new List<IEnumerable<string>>();
            //Get letters that have not been guessed
            List<char> notTriedLetter = new List<char>();
            for (int i = 0; i < word.Length; i++)
            {
                if (mask[i] == '-')
                    notTriedLetter.Add(word[i]);
            }

            //Get all word families
            Parallel.ForEach(notTriedLetter, unrevealedLetter =>
            {
                lock (output)
                {
                    string newMask = "";
                    for (int i = 0; i < mask.Length; i++)
                    {
                        if (word[i] == unrevealedLetter)
                            newMask += unrevealedLetter.ToString();
                        else
                            newMask += mask[i].ToString();
                    }
                    output.Add(GetSimilarFamilyWords(newMask, word));
                }
            });
            //returing them in descending order so that the largest is first to be picked up when method ends
            return output.OrderByDescending(x => x.Count());
        }

        /// <summary>
        /// Retrieve word family using the min max algorithm
        /// </summary>
        /// <param name="word"></param> //Hidden word
        /// <param name="mask"></param> //Masked word
        /// <returns></returns>
        public static IEnumerable<string> GetMinMaxFamily(string word, string mask)
        {
            //Use words as word family if word family is uninitalized
            var words = Display.wordFamily;
            if (words == null)
                words = WordProcesses.wordList;

            //Return word family based on Knuth's minmax algorithm
            var wordFamilies = GetWordFamilies(word, mask);
            if (wordFamilies.Count() == 0) return null;

            //Get word family with the largest count, will retrieve first as they were sorted in descending order
            var largestWordFamily = wordFamilies.First();
            if (largestWordFamily.Count() == 0)
                return null;

            //Get value from largestWordFamily that will reduce wordFamily size the most
            int lowest = -1;
            minMaxWord = "";
            Parallel.ForEach(largestWordFamily, wrd =>
            {
                lock (minMaxWord)
                {
                    //Get new masked word
                    var newWordMask = new string(wrd.Select(x => Display.triedLetters.Contains(x) ? x : '-').ToArray());

                    //Get how many words would be similar
                    int len = GetSimilarFamilyWords(mask, wrd).Count();

                    if (lowest == -1 || len < lowest)
                    {
                        lowest = len;
                        minMaxWord = wrd;
                    }
                }
            });
            return largestWordFamily;
        }

        /// <summary>
        /// Method gets the words of the same word family, where the correctly guessed letters are the same 
        /// </summary>
        /// <param name="maskedWord"></param> //Masked word
        /// <param name="secretWord"></param> //Hidden word
        /// <returns></returns> //Returns words in the same family with the hidden word
        public static IEnumerable<string> GetSimilarFamilyWords(string maskedWord, string secretWord)
        {
            //Use words as word family if word family is uninitalized
            var words = Display.wordFamily;
            if (words == null)
            {
                words = WordProcesses.wordList;
            }
            //Get all the words that match the masked word
            return words.Where(word => word.Length == maskedWord.Length && word != secretWord && CheckLengthMatch(word, maskedWord) && RunWordSimilarityRules(word));
        }

        /// <summary>
        /// This method check if the masked word matced the length of the hidden word
        /// </summary>
        /// <param name="word"></param> //Hidden word
        /// <param name="mask"></param> //Masked word
        /// <returns></returns> //Returns TRUE if the lengths match
        /// <exception cref="Exception"></exception>
        private static bool CheckLengthMatch(string word, string mask)
        {
            //Throw error if lenghts are not the same 
            if (word.Length != mask.Length) throw new Exception("Word and Mask are not the same length");

            bool matchMask = true;
            for (int i = 0; i < mask.Length; i++)
            {
                if (mask[i] != '-') matchMask &= mask[i] == word[i];
            }
            return matchMask;
        }

        /// <summary>
        /// Searches and retrieves a word of specified length from the word list
        /// </summary>
        /// <param name="wordLength"></param> //Random generated integer between 4 and 12
        /// <returns></returns> //Out of the created list of possible words of this lenght retrieves a random choosed word and returns it
        public static string GetRandomWordOfLength(int wordLength)
        {
            if (random == null) random = new Random();
            //Get list of possible words of this lenght
            var words = wordList.Where(x => x.Length == wordLength).ToList();
            //Return a random word out of words
            return words[random.Next(0, words.Count())];
        }

        /// <summary>
        /// This method reads the dictionary file and adds all words into the word list
        /// </summary>
        public static void LoadDictionary()
        {
            //Get all words from dictionary into list
            wordList = File.ReadAllLines("dictionary.txt").ToList();
        }
    }
}
