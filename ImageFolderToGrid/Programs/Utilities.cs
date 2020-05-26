using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImageFolderToGrid.Programs {
    public static class Utilities {

        public static Dictionary<string, string> ProcessArguments(string[] args) {
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            foreach (string arg in args) {
                string[] parts = arg.Split("=");
                if (parts.Length != 2) {
                    Console.WriteLine($"Invalid argument {arg}.");
                    return null;
                }

                parts[0] = parts[0].Trim().ToLower();

                if (arguments.ContainsKey(parts[0])) {
                    Console.WriteLine($"Repeated argument {parts[0]}.");
                    return null;
                }

                arguments[parts[0]] = parts[1].Trim();
            }

            return arguments;
        }

        public static List<string> LoadImages(string folderPaths) {
            List<string> output = new List<string>();

            foreach (string folderPath in folderPaths.Split(';', StringSplitOptions.RemoveEmptyEntries)) {
                if (!Directory.Exists(folderPath)) {
                    return null;
                }

                string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
                foreach (string file in files) {
                    if (File.Exists(file)) {
                        string ext = Path.GetExtension(file);
                        if (ext.Equals(".bmp", StringComparison.InvariantCultureIgnoreCase) || ext.Equals(".png", StringComparison.InvariantCultureIgnoreCase) || ext.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) || ext.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase)) { //TODO: add more formats
                            output.Add(file);
                        }
                    }
                }
            }

            return output;
        }

    }
}
