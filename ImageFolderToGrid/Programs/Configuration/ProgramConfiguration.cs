using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImageFolderToGrid.Programs.Configuration {

    /// <summary>
    /// All of the configurations and a helper to load them from the arguments.
    /// </summary>
    public class ProgramConfiguration {
        public bool IsValid { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string OutputFileName { get; set; }
        public string OutputFilePath {
            get {
                return Path.Combine(OutputPath, OutputFileName);
            }
        }
        public int OutputWidth { get; set; }
        public int CellWidth { get; set; }
        public int CellHeight { get; set; }

        /// <summary>
        /// Sets the known default values.
        /// </summary>
        public ProgramConfiguration() {
            IsValid = false;
            OutputFileName = "OutputGrid";
            OutputWidth = 520;
            CellWidth = 26;
            CellHeight = 26;
        }

        /// <summary>
        /// Fills the fields using the supplied argument dictionary.
        /// Does additional validation.
        /// </summary>
        /// <param name="arguments">The fields and their values in a dictionary.</param>
        public void LoadFrom(Dictionary<string, string> arguments) {
            IsValid = false;

            if (!arguments.ContainsKey(ArgumentConfiguration.InputPathArg)) {
                Console.WriteLine($"Must contain {ArgumentConfiguration.InputPathArg} argument.");
                return;
            }

            if (!arguments.ContainsKey(ArgumentConfiguration.OutputPathArg)) {
                Console.WriteLine($"Must contain {ArgumentConfiguration.OutputPathArg} argument.");
                return;
            }

            InputPath = arguments[ArgumentConfiguration.InputPathArg];
            OutputPath = arguments[ArgumentConfiguration.OutputPathArg];

            if (!Directory.Exists(InputPath)) {
                Console.WriteLine("Invalid input directory!");
                return;
            }

            if (!Directory.Exists(OutputPath)) {
                Console.WriteLine("Invalid output directory!");
                return;
            }

            if (arguments.ContainsKey(ArgumentConfiguration.OutputFileArg)) {
                OutputFileName = Path.GetFileNameWithoutExtension(arguments[ArgumentConfiguration.OutputFileArg]);
            }

            string suffix = ".png";
            string outputFilePath;

            //don't want an infinite loop
            for (int i = 1; i <= 21; ++i) {
                outputFilePath = Path.Combine(OutputPath, OutputFileName + suffix);

                if (!File.Exists(outputFilePath)) {
                    OutputFileName += suffix;
                    break;
                }

                suffix = $" ({i}).png";
            }

            if (arguments.ContainsKey(ArgumentConfiguration.WidthArg)) {
                if (int.TryParse(arguments[ArgumentConfiguration.WidthArg], out int temp)) {
                    OutputWidth = temp;
                }
            }

            if (arguments.ContainsKey(ArgumentConfiguration.CellWidthArg)) {
                if (int.TryParse(arguments[ArgumentConfiguration.CellWidthArg], out int temp)) {
                    CellWidth = temp;
                }
            }

            if (arguments.ContainsKey(ArgumentConfiguration.CellHeightArg)) {
                if (int.TryParse(arguments[ArgumentConfiguration.CellHeightArg], out int temp)) {
                    CellHeight = temp;
                }
            }

            IsValid = true;
        }
    }
}
