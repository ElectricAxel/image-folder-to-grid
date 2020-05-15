using System;
using System.Collections.Generic;
using System.Text;

namespace ImageFolderToGrid {
    class Program {

        const string programArg = "program";
        const string netCoreProgram = "netcore";
        const string imageSharpProgram = "imagesharp";

        public static void Main(string[] args) {

            Dictionary<string, string> arguments = Programs.Utilities.ProcessArguments(args);

            if (arguments == null) {
                return;
            }

            if (!arguments.ContainsKey(programArg)) {
                Console.WriteLine($"Must specify the argument {programArg}.");
                return;
            }

            Programs.IProgram imageProgram;

            switch (arguments[programArg].ToLower()) {
                case netCoreProgram:
                    imageProgram = new Programs.NetCoreProgram();
                    break;
                case imageSharpProgram:
                    imageProgram = new Programs.ImageSharpProgram();
                    break;
                default:
                    imageProgram = null;
                    break;
            }

            if (imageProgram == null) {
                Console.WriteLine("Invalid program!");
                return;
            }

            imageProgram.Main(arguments);
        }
    }
}
