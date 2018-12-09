using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Series_Filename_Filter
{
    class Program
    {
        private static string mainDirectory = "";
        private static string delimiter;
        private static List<string> removals = new List<string>();
        private static List<KeyValuePair<string, string>> replacements = new List<KeyValuePair<string, string>>();

        private static bool running = true;
        private static bool changesConfirmed = false;
        private static string softwareName;
        private static Version version;
        private static string menuStatus = "Waiting for input...";

        static void Main(string[] args)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            version = assembly.GetName().Version;
            softwareName = assembly.GetName().Name;
            Console.Title = softwareName + ": Version " + version;

            while (true)
            {
                DisplayLogo();
                DisplayLicense();
                Console.WriteLine("\nPress ENTER key to continue...");
                Console.ReadLine();
                break;
            }

            while (running)
            {
                Console.Clear();
                DisplayLogo();
                DisplayMenu();
                var input = Console.ReadLine();
                if (int.TryParse(input, out int menuOption))
                {
                    switch (menuOption)
                    {
                        case 1:
                            CollectParameters(menuOption);
                            menuStatus = "Waiting for input...";
                            break;
                        case 2:
                            CollectParameters(menuOption);
                            menuStatus = "Waiting for input...";
                            break;
                        case 3:
                            DisplayHelp();
                            menuStatus = "Waiting for input...";
                            break;
                        case 4:
                            running = false;
                            break;
                        default:
                            menuStatus = "Error: Invalid menu option selected! Try again.";
                            break;
                    }
                }
                else
                {
                    menuStatus = "Error: Input error! Please enter a menu option.";
                }
            }
        }

        private static void DisplayLogo()
        {
            WriteInColor("  ___          _          ___ _ _     " +
                "\n / __| ___ _ _(_)___ ___ | __(_) |___ " +
                "\n \\__ \\/ -_) '_| / -_|_-< | _|| | / -_)" +
                "\n |___/\\___|_| |_\\___/__/ |_| |_|_\\___|" +
                "\n   | _ \\___ _ _  __ _ _ __  ___ _ _   " +
                "\n   |   / -_) ' \\/ _` | '  \\/ -_) '_|  " +
                "\n   |_|_\\___|_||_\\__,_|_|_|_\\___|_|    " +
                "\n\n--------------------------------------",
                ConsoleColor.Cyan,
                true);
        }

        private static void DisplayLicense()
        {
            WriteInColor(softwareName + " Copyright (C) 2018 Lonewolf Software" +
                "\nThis program is free software: you can redistribute it and/or modify" +
                "\nit under the terms of the GNU General Public License as published by" +
                "\nthe Free Software Foundation, either version 3 of the License, or" +
                "\n(at your option) any later version." +
                "\n" +
                "\nThis program is distributed in the hope that it will be useful," +
                "\nbut WITHOUT ANY WARRANTY; without even the implied warranty of" +
                "\nMERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the" +
                "\nGNU General Public License for more details." +
                "\n" +
                "\nYou should have received a copy of the GNU General Public License" +
                "\nalong with this program.  If not, see <https://www.gnu.org/licenses/>.",
                ConsoleColor.Cyan,
                true);
        }

        private static void DisplayHelp()
        {
            Console.Clear();
            DisplayLogo();
            Console.WriteLine("This program was made as a tool for mass renaming of TV Series file names." +
                "\nAs of version " + version + ", this program can remove and/or replace unwanted" +
                "\nwords. Episode file names must include episode information in one of the" +
                "\nfollowing non-case sensitive patterns:");

            WriteInColor("Single episodes: s01e01" +
                "\nMulti episodes: s01e01e02",
                ConsoleColor.Cyan,
                true);

            WriteInColor("\nSeries Main Directory", ConsoleColor.Cyan, false);
            Console.WriteLine(" refers to the directory name containing all season folders.");
            WriteInColor("\nDelimiter", ConsoleColor.Cyan, false);
            Console.WriteLine(" refers to the character separating words within a filename. " +
                "\nTypically ' ' (space) or '.' (period).");

            Console.WriteLine("\nPress ENTER key to go back...");
            Console.ReadLine();
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("[1] Simple Rename: Removals Only");
            Console.WriteLine("[2] Complex Rename: Removals and Replacements");
            Console.WriteLine("[3] Help");
            Console.WriteLine("[4] Exit");
            Console.Write("\nStatus: ");
            WriteInColor(menuStatus, menuStatus.ToLower().Contains("error") ? ConsoleColor.Red : ConsoleColor.Cyan, false);
            Console.Write("\n> ");
        }

        private static void DisplayResults(int filesChanged, int foldersChecked)
        {
            Console.Clear();
            DisplayLogo();
            WriteInColor("Renaming Successful\n", ConsoleColor.Cyan, true);
            Console.Write("Folders Checked: ");
            WriteInColor(foldersChecked.ToString(), ConsoleColor.Cyan, true);
            Console.Write("Files Changed: ");
            WriteInColor(filesChanged.ToString(), ConsoleColor.Cyan, true);
            Console.WriteLine("\nPress ENTER key to return to menu...");
            Console.ReadLine();
        }

        private static void CollectParameters(int option)
        {
            bool confirmedParameters = false;
            while (!confirmedParameters)
            {
                Console.Clear();
                ResetDefaults();
                DisplayLogo();

                //Collect Parameters
                while (!Directory.Exists(mainDirectory))
                {
                    if (!string.IsNullOrWhiteSpace(mainDirectory) && !Directory.Exists(mainDirectory))
                    {
                        WriteInColor("Error: Invalid directory! Try again.\n\n", ConsoleColor.Red, false);
                    }
                    WriteInColor("Enter series main directory:", ConsoleColor.Cyan, true);
                    mainDirectory = Console.ReadLine();
                }

                WriteInColor("\nEnter file-name word delimiter:", ConsoleColor.Cyan, true);
                delimiter = Console.ReadLine();

                WriteInColor("\nEnter words to be removed. Spaces are retained. Separate with ',':", ConsoleColor.Cyan, true);
                removals = Console.ReadLine().Split(',').ToList();
                removals = RemoveEmptyElements(removals);

                if (option == 2)
                {
                    bool replacementsInvalid = true, replacementsError = false;
                    while (replacementsInvalid)
                    {
                        replacements.Clear();
                        if (replacementsError) WriteInColor("Error: Can not replace the same word more than once. Try again.", ConsoleColor.Red, false);

                        WriteInColor("\nEnter desired word replacements. Spaces are retained." +
                            "\nSeparate old-new word pairs with ','. Separate multiple replacemenent pairs with ';'." +
                            "\nE.g.: ST,Star Trek;TNG,The Next Generation;", ConsoleColor.Cyan, true);
                        var replacementList = Console.ReadLine().Split(';').ToList();
                        foreach (string element in RemoveEmptyElements(replacementList))
                        {
                            var elements = element.Split(',');
                            if (replacements.Count > 0 && replacements.Any(pair => pair.Key.Equals(elements[0])))
                            {
                                replacementsError = true;
                                replacementsInvalid = true;
                                break;
                            }
                            else
                            {
                                replacements.Add(new KeyValuePair<string, string>(elements[0], elements[1]));
                                replacementsError = false;
                                replacementsInvalid = false;
                            }
                        }
                    }
                }

                //Display Confirmation of Parameters
                Console.Clear();
                DisplayLogo();
                Console.WriteLine("Please confirm parameters:\n");
                Console.Write("Main Directory: ");
                WriteInColor(mainDirectory, ConsoleColor.Cyan, true);
                Console.Write("Delimiter: ");
                WriteInColor("'" + delimiter.ToString() + "'", ConsoleColor.Cyan, true);
                Console.Write("Removals: ");
                WriteInColor(PrintList(removals), ConsoleColor.Cyan, true);
                if (replacements.Count > 0)
                {
                    Console.Write("Replacements: ");
                    string replacementString = "";
                    foreach (KeyValuePair<string, string> pair in replacements)
                    {
                        replacementString += $"'{pair.Key}' => '{pair.Value}'; ";
                    }
                    WriteInColor(replacementString, ConsoleColor.Cyan, true);
                }

                //Confirm Parameters
                bool confirmationInvalid = true, confirmationError = false;
                while (confirmationInvalid)
                {
                    if (confirmationError) WriteInColor("Error: Invalid input! Enter 'y' or 'n'.", ConsoleColor.Red, false);

                    Console.WriteLine("\nUse these parameters? [y/n]:");
                    var input = Console.ReadLine();
                    if (char.TryParse(input, out char result))
                    {
                        confirmationError = result == 'y' ? false : (result == 'n' ? false : true);
                        confirmedParameters = confirmationError == true ? false : (result == 'y' ? true : false);
                        confirmationInvalid = confirmationError == true ? true : false;
                    }
                    else
                    {
                        confirmationError = true;
                    }
                }
            }

            Filter();
        }

        private static bool ConfirmChanges(string oldFileName, string newFileName)
        {
            bool confirmationChoice = false;

            Console.Clear();
            DisplayLogo();
            Console.WriteLine("Files will be changed as shown below. Please confirm changes.\n");
            Console.Write("Old: ");
            WriteInColor(oldFileName, ConsoleColor.Red, true);
            Console.Write("New: ");
            WriteInColor(newFileName, ConsoleColor.Green, true);

            bool confirmationInvalid = true, confirmationError = false;
            while (confirmationInvalid)
            {
                if (confirmationError) WriteInColor("Error: Invalid input! Enter 'y' or 'n'.", ConsoleColor.Red, false);

                Console.WriteLine("\nContinue with these changes? [y/n]:");
                var input = Console.ReadLine();
                if (char.TryParse(input, out char result))
                {
                    confirmationError = result == 'y' ? false : (result == 'n' ? false : true);
                    confirmationChoice = confirmationError == true ? false : (result == 'y' ? true : false);
                    changesConfirmed = confirmationChoice;
                    confirmationInvalid = confirmationError == true ? true : false;
                }
                else
                {
                    confirmationError = true;
                }
            }

            return confirmationChoice;
        }

        private static void Filter()
        {
            var seasons = Directory.GetDirectories(mainDirectory);
            var foldersChecked = seasons.Count();
            var filesChanged = 0;

            foreach (string season in seasons)
            {
                var files = Directory.GetFiles(season);

                foreach (string file in files)
                {
                    var fileDirectory = file.Substring(0, file.Length - file.Split('\\').Last().Length);
                    var fullFileName = file.Split('\\').Last();
                    var extension = "." + fullFileName.Split('.').Last();
                    var oldFileName = fullFileName.Substring(0, fullFileName.Length - extension.Length);

                    var fileNameWords = oldFileName.Split(delimiter.ToCharArray()).ToArray();
                    var newFileName = "";

                    for (int i = 0; i < fileNameWords.Length; i++)
                    {
                        var word = fileNameWords[i];

                        if (!removals.Contains(word) && !string.IsNullOrWhiteSpace(word))
                        {
                            if (replacements.Count > 0)
                            {
                                foreach (KeyValuePair<string, string> pair in replacements)
                                {
                                    if (word.Equals(pair.Key))
                                    {
                                        word = pair.Value;

                                    }
                                }
                            }

                            if (word.Contains("-"))
                            {
                                word = word.Replace('-', ' ');
                            }

                            if (i != 0)
                            {
                                newFileName += " ";
                            }

                            if (Regex.Match(word, @"^.*S\d\dE\d\d", RegexOptions.IgnoreCase).Success)
                            {
                                if (word.Length > 6)
                                {
                                    word = word.Substring(0, 6) + "-" + word.Substring(6, 3);
                                }

                                if (i == fileNameWords.Length - 1)
                                {
                                    word = "- " + word.ToLower();
                                }
                                else
                                {
                                    word = "- " + word.ToLower() + " -";
                                }
                            }

                            newFileName += word;
                        }
                    }

                    if (!changesConfirmed)
                    {
                        var confirmed = ConfirmChanges(fullFileName, newFileName + extension);
                        if (!confirmed)
                        {
                            return;
                        }
                    }

                    var newFile = fileDirectory + newFileName + extension;
                    File.Move(file, newFile);
                    filesChanged++;
                }
            }

            DisplayResults(filesChanged, foldersChecked);
        }

        #region Non-core methods
        public static void WriteInColor(string text, ConsoleColor color, bool writeLine)
        {
            if (writeLine)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ResetColor();
            }

            if (!writeLine)
            {
                Console.ForegroundColor = color;
                Console.Write(text);
                Console.ResetColor();
            }
        }

        public static string PrintList(List<string> list)
        {
            string outputString = "";
            for (int i = 0; i < list.Count; i++)
            {
                outputString += "'" + list[i] + "'";
                if (i != list.Count - 1)
                {
                    outputString += ",";
                }
            }

            return outputString;
        }

        public static List<string> RemoveEmptyElements(List<string> list)
        {
            return new List<string>(list.Where(element => !string.IsNullOrWhiteSpace(element)).Distinct().ToList());
        }

        public static void ResetDefaults()
        {
            mainDirectory = "";
            delimiter = "";
            removals.Clear();
            replacements.Clear();
        }
        #endregion
    }
}
