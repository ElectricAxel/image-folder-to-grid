using SixLabors.ImageSharp;
using System;
using System.IO;

namespace ImageDeleter {
    class Program {
        static void Main(string[] args) {
            Image<SixLabors.ImageSharp.PixelFormats.Rgba32> temp;

            if (!Directory.Exists(args[0])) {
                return;
            }

            string[] files = Directory.GetFiles(args[0]);
            foreach (string file in files) {
                if (File.Exists(file)) {
                    string ext = Path.GetExtension(file);
                    if (ext.Equals(".bmp", StringComparison.InvariantCultureIgnoreCase) || ext.Equals(".png", StringComparison.InvariantCultureIgnoreCase) || ext.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) || ext.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase)) { //TODO: add more formats

                        temp = Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(file);

                        int w = temp.Width - 1, h = temp.Height - 1;
                        if (temp[0, 0].A == 0
                            || temp[w, 0].A == 0
                            || temp[0, h].A == 0
                            || temp[w, h].A == 0) {

                            File.Delete(file);

                        }
                    }
                }
            }
        }
    }
}
