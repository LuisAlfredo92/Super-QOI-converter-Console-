using Core;

namespace Super_QOI_converter__Console_
{
    /// <summary>
    /// Main class of the console version. It contains the Main function
    /// </summary>
    internal class Program : IOptionsConfirmation
    {
        private static bool? _copyFileInfo, _deleteSources, _ignoreColors, _overwrite, _readDirectories;
        private static readonly List<string> Paths = new();

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">Arguments that the program could use, they can be
        /// files paths, directories, options or nothing</param>
        private static void Main(string[] args)
        {
            _copyFileInfo = _deleteSources = _ignoreColors = _overwrite = _readDirectories = null;

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            switch (args.Length)
            {
                case 0: // Will start the program and receive paths and options internally
                    ReceivePaths();
                    break;

                case 1:
                    switch (args[0]) // printing help or error commands
                    {
                        case "-h":
                        case "--help":
                            Console.Write(Messages.Welcome_to);
                            ChangeConsoleColor(ConsoleColor.Cyan);
                            Console.Write($@" {Messages.Program_name}");
                            ChangeConsoleColor(ConsoleColor.White);
                            Console.Write($@" {Messages.Specify_console_version} ");
                            ChangeConsoleColor(ConsoleColor.Cyan);
                            Console.WriteLine(Messages.Made_by + @": LuisAlfredo92 (https://github.com/LuisAlfredo92)");
                            ChangeConsoleColor(ConsoleColor.White);
                            Console.WriteLine($@"{Messages.Repo_official_link_message}: https://github.com/LuisAlfredo92/Super-QOI-converter"
                                            + Environment.NewLine);
                            Console.WriteLine(Messages.With_help_of);
                            Console.WriteLine(@"- QoiSharp (https://github.com/NUlliiON/QoiSharp) " + Messages.Made_by
                                + @": NUlliiON (https://github.com/NUlliiON) " + Messages.Under_a_MIT_license);
                            Console.WriteLine(@"- StbImageSharp (https://github.com/StbSharp/StbImageSharp) " + Messages.Made_by
                                + @": StbSharp (https://github.com/StbSharp)" + Environment.NewLine);

                            ChangeConsoleColor(ConsoleColor.Cyan);
                            Console.WriteLine(Messages.Not_console_necessary + Environment.NewLine);

                            ChangeConsoleColor(ConsoleColor.White);
                            Console.WriteLine(Messages.Commands_title + Environment.NewLine);
                            Console.WriteLine(Messages.Copy_attributes_and_dates_option + Environment.NewLine);
                            Console.WriteLine(Messages.Not_copy_attributes_and_dates_option + Environment.NewLine);
                            Console.WriteLine(Messages.Delete_source_option + Environment.NewLine);
                            Console.WriteLine(Messages.Not_delete_source_option + Environment.NewLine);
                            Console.WriteLine(Messages.Help_option + Environment.NewLine);
                            Console.WriteLine(Messages.Ignore_colors_option + Environment.NewLine);
                            Console.WriteLine(Messages.Overwrite_option + Environment.NewLine);
                            Console.WriteLine(Messages.Do_not_overwrite_option + Environment.NewLine);
                            Console.WriteLine(Messages.Version_option + Environment.NewLine + Environment.NewLine);

                            Console.WriteLine(Messages.Here_are_some_examples + Environment.NewLine);
                            Console.WriteLine(Messages.Examples);

                            Environment.Exit(0);
                            break;

                        case "-v":
                        case "--version":
                            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                            Console.WriteLine(@"Super QOI Converter - " + fvi.FileVersion);
                            Environment.Exit(0);
                            break;

                        case "-c":
                        case "-d":
                        case "-nc":
                        case "-nd":
                            ReceivePaths();
                            break;

                        case "-i":
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(Messages.You_must_add_paths + Environment.NewLine);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write($@"{Messages.Type} ");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write($@"""{Messages.Program_name}"" -h ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($@"{Messages.More_info_how_to_execute}{Environment.NewLine}");

                            Environment.Exit(0);
                            break;

                        default:
                            if (!Uri.IsWellFormedUriString(args[0], UriKind.Absolute))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(Messages.Invalid_option_or_path + Environment.NewLine);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write($@"{Messages.Type} ");
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write($@"""{Messages.Program_name}"" -h ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine($@"{Messages.More_info_how_to_execute}{Environment.NewLine}");

                                Environment.Exit(0);
                            }
                            break;
                    }
                    break;

                default: // Will start the program with the paths received
                    // If the user writes contradictory options, the program will close
                    if ((args.Contains("-c") && args.Contains("-nc")) ||
                        (args.Contains("-d") && args.Contains("-nd")) ||
                        (args.Contains("-o") && args.Contains("-no")))
                    {
                        ChangeConsoleColor(ConsoleColor.Red);
                        Console.WriteLine(Messages.Contradictory_options_error + Environment.NewLine);
                        ChangeConsoleColor(ConsoleColor.White);
                        Environment.Exit(0);
                    }

                    // Reading options
                    if (args.Contains("-c"))
                        _copyFileInfo = true;
                    else if (args.Contains("-nc"))
                        _copyFileInfo = false;

                    if (args.Contains("-d"))
                        _deleteSources = true;
                    else if (args.Contains("-nd"))
                        _deleteSources = false;

                    if (args.Contains("-o"))
                        _overwrite = true;
                    else if (args.Contains("-no"))
                        _overwrite = false;

                    // If the user writes only options without paths, the program will ask for paths
                    if (args.All(element => new List<string> { "-c", "-nc", "-d", "-nd", "-o", "-no" }.Contains(element)))
                        ReceivePaths();
                    else
                        Paths.AddRange(args.Where(element => !new List<string> { "-c", "-nc", "-d", "-nd", "-o", "-no" }.Contains(element)));

                    break;
            }

            //TODO: Ask user for number of concurrent conversions

            while (Paths.Count != 0)
            {
                Converter.ConvertToQoi(new Program(), Paths.First());
                Paths.Remove(Paths.First());
            }

            ChangeConsoleColor(ConsoleColor.White);
            Console.Clear();
            Console.WriteLine(Messages.Thank_you);
        }

        /// <summary>
        /// Function to select the Yes, Yes to all, No, No to all options
        /// </summary>
        /// <param name="message">The message that will be showed on the console.
        /// It's recommended it is a question</param>
        /// <param name="configuration">The reference to the variable that will be
        /// overwritten if the user selects a "to all" option</param>
        /// <returns>A bool that indicates the selected option</returns>
        public static bool SelectOption(string message, ref bool? configuration)
        {
            var keyPressed = ConsoleKey.Enter;
            byte selectedOption = 1;

            do
            {
                switch (keyPressed)
                {
                    case ConsoleKey.UpArrow:
                        if (selectedOption > 1)
                            selectedOption--;
                        else
                            selectedOption = 4;
                        break;

                    case ConsoleKey.DownArrow:
                        if (selectedOption < 4)
                            selectedOption++;
                        else
                            selectedOption = 1;
                        break;

                    case ConsoleKey.NumPad1:
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.NumPad3:
                    case ConsoleKey.NumPad4:
                        selectedOption = (byte)((int)keyPressed - 96);
                        break;

                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                    case ConsoleKey.D4:
                        selectedOption = (byte)((int)keyPressed - 48);
                        break;
                }

                Console.Clear();
                ChangeConsoleColor(ConsoleColor.Yellow);
                Console.WriteLine(message);
                ChangeConsoleColor(ConsoleColor.White);
                Console.WriteLine(Messages.Use_arrows_or_numbers_and_Enter);

                var options = new[] { Messages.Yes, Messages.Yes_to_all, Messages.No, Messages.No_to_all };
                for (byte i = 1; i <= 4; i++)
                {
                    options[i - 1] = (selectedOption == i ? "> " : "") + $"{i}. " + options[i - 1];
                    ChangeConsoleColor(selectedOption == i ? ConsoleColor.Cyan : ConsoleColor.Gray);
                    Console.WriteLine(options[i - 1]);
                }

                keyPressed = Console.ReadKey().Key;
            } while (keyPressed != ConsoleKey.Enter);

            configuration = selectedOption switch
            {
                2 => true,
                4 => false,
                _ => configuration
            };

            return selectedOption is 1 or 2;
        }

        /// <summary>
        /// Changes the console color depending on the -i option.
        /// </summary>
        /// <param name="color">The color that the text will have</param>
        private static void ChangeConsoleColor(ConsoleColor color)
        {
            if (_ignoreColors != true)
                Console.ForegroundColor = color;
        }

        /// <summary>
        /// Receives the paths of the files or directories
        /// if the user didn't write it on arguments when executing
        /// </summary>
        private static void ReceivePaths()
        {
            string tempPath;

            ChangeConsoleColor(ConsoleColor.Red);
            Console.WriteLine(Messages.You_did_not_add_any_path);
            ChangeConsoleColor(ConsoleColor.White);
            Console.WriteLine(Messages.Write_your_paths_now);

            do
            {
                tempPath = Console.ReadLine() ?? string.Empty;
                // If the path is invalid or different to Exit string
                bool validPath = File.Exists(tempPath) || Directory.Exists(tempPath),
                    isExitString = string.Equals(tempPath, Messages.Exit, StringComparison.OrdinalIgnoreCase);

                if (!validPath && !isExitString)
                {
                    ChangeConsoleColor(ConsoleColor.Red);
                    Console.WriteLine(Messages.Reading_invalid_path_message);
                    ChangeConsoleColor(ConsoleColor.White);
                }
                else if (!isExitString && !File.GetAttributes(tempPath).HasFlag(FileAttributes.Directory)
                         && !(tempPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                            || tempPath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                            || tempPath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                            || tempPath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)))
                {
                    ChangeConsoleColor(ConsoleColor.Red);
                    Console.WriteLine(Messages.Not_image_or_folder);
                    ChangeConsoleColor(ConsoleColor.White);
                }
                else if (Paths.Contains(tempPath))
                {
                    ChangeConsoleColor(ConsoleColor.Red);
                    Console.WriteLine(Messages.Already_entered);
                    ChangeConsoleColor(ConsoleColor.White);
                }
                else
                {
                    // If is different to Exit string it will add it to paths
                    if (!string.Equals(tempPath, Messages.Exit, StringComparison.OrdinalIgnoreCase))
                    {
                        Paths.Add(tempPath);
                        continue;
                    }

                    // If the user types Exit string but didn't add any path
                    if (Paths.Any()) continue;
                    Console.WriteLine(Messages.Add_at_least_one_path);
                    tempPath = string.Empty;
                }
            } while (!string.Equals(tempPath, Messages.Exit, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Function from IConfirmOptions that will confirm or ask if the user wants to
        /// copy attributes and dates from the original files
        /// </summary>
        /// <param name="originalFile">Path to the original file.
        /// It can be null since it isn't necessary. In this case it's only to
        /// specify the file we're talking about</param>
        /// <returns>If the respective variable has a value, that value is returned.
        /// If the variable is null, it'll ask to the user what they want to do</returns>
        public bool ConfirmCopy(string originalFile = "")
        {
            if (_copyFileInfo.HasValue) return _copyFileInfo.Value;

            var msg = string.Concat(originalFile, "\n", Messages.Do_you_want_to_copy_file_info);
            return SelectOption(msg, ref _copyFileInfo);
        }

        /// <summary>
        /// Function from IConfirmOptions that will confirm or ask if the user wants to
        /// delete the original files
        /// </summary>
        /// <param name="originalFile">Path to the original file.
        /// It can be null since it isn't necessary. In this case it's only to
        /// specify the file we're talking about</param>
        /// <returns>If the respective variable has a value, that value is returned.
        /// If the variable is null, it'll ask to the user what they want to do</returns>
        public bool ConfirmDeletion(string originalFile = "")
        {
            if (_deleteSources.HasValue) return _deleteSources.Value;

            ChangeConsoleColor(ConsoleColor.Red);
            Console.WriteLine(Messages.This_is_going_to_delete_permanently);
            ChangeConsoleColor(ConsoleColor.White);
            var msg = string.Concat(originalFile, "\n", Messages.Do_you_want_to_delete_original_file);
            return SelectOption(msg, ref _deleteSources);
        }

        /// <summary>
        /// Function from IConfirmOptions that will confirm or ask if the user wants to
        /// overwrite existing files
        /// </summary>
        /// <param name="existingFile">Path to the existing file.
        /// It can't be null because we must show the already existing file so the user
        /// can modify it externally</param>
        /// <returns>If the respective variable has a value, that value is returned.
        /// If the variable is null, it'll ask to the user what they want to do</returns>
        public bool ConfirmOverwrite(ref string existingFile)
        {
            if (_overwrite.HasValue) return _overwrite.Value;

            var msg = string.Concat(existingFile, "\n", Messages.File_already_exists);
            return SelectOption(msg, ref _overwrite);
        }

        /// <summary>
        /// Function from IConfirmOptions to handle if the program recognizes a directory path.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        public void ManageDirectory(string directoryPath)
        {
            var msg = string.Format(Messages.It_looks_like_a_directory, directoryPath);
            if ((_readDirectories.HasValue && _readDirectories.Value)
                || SelectOption(msg, ref _readDirectories))
            {
                //TODO: Add option to do it recursive?
                Paths.AddRange(
                    Directory.GetFiles(directoryPath).Where(element =>
                        element.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                        || element.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                        || element.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                        || element.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                    );
            }
        }
    }
}