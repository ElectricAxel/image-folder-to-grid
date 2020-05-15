using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageFolderToGrid.Programs {
    public class NetCoreProgram : IProgram {

        //.\ImageFolderToGrid.exe program="netcore" inputpath="drive:\path\to\input" outputpath="drive:\path\to\output" program="netcore" width=480 cellwidth=32 cellheight=32 interpolationmode=7

        /// <summary>
        /// Using the supplied arguments, will load all the images in a folder, create a new image and put all the images in the input folder in a grid manner, then save the new image to the output folder with the output name.
        /// Arguments (the ones marked with * are optional): 
        /// inputpath=drive:\path\to\input
        /// outputpath=drive:\path\to\output
        /// *outputfile=OutputGrid
        /// *width=520
        /// *cellwidth=26
        /// *cellheight=26
        /// *interpolationmode=7
        /// See <see cref="System.Drawing.Drawing2D.InterpolationMode"/> for more information on interpolation mode.
        /// </summary>
        /// <param name="arguments">Preprocessed arguments from the command line.</param>
        public void Main(Dictionary<string, string> arguments) {

            NetCoreProgramConfiguration config = new NetCoreProgramConfiguration();

            config.LoadFrom(arguments);

            if (!config.IsValid) {
                return;
            }

            List<string> imagePaths = Utilities.LoadImages(config.InputPath);

            if (imagePaths == null) {
                Console.WriteLine("Something went wrong loading the images.");
                return;
            }

            if (imagePaths.Count == 0) {
                Console.WriteLine($"No images of the supported format found in the input folder {config.InputPath}");
                return;
            }

            //These images are guaranteed to exist and have the image extension, but the file could be corrupted...

            List<Image> images = new List<Image>();

            foreach (string path in imagePaths) {
                Image temp = Image.FromFile(path);
                images.Add(new Bitmap(temp, config.CellWidth, config.CellHeight)); //Test 1
            }

            Image result = MergeImageInGrid(images, config.OutputWidth, config.CellWidth, config.CellHeight, config.InterpolationMode);

            if (result == null) {
                Console.WriteLine("Something went wrong merging the images.");
                return;
            }

            // This was to avoid an error overwriting the file, but that's not an option anymore.
            //foreach (Image image in images) {
            //    image.Dispose();
            //}

            result.Save(config.OutputFilePath, System.Drawing.Imaging.ImageFormat.Png);

        }

        /// <summary>
        /// Creates a new image and lays the list of images in the cells of a grid.
        /// Fills the grid is filled left to right, top to bottom.
        /// Images will be resized to the cell size.
        /// Final image will be as high as necessary depending on the amount of images.
        /// </summary>
        /// <param name="images"></param>
        /// <param name="imageWidth">Width of grid, with no padding.</param>
        /// <param name="cellWidth">Width of a cell, with no padding.</param>
        /// <param name="cellHeight">Height of a cell, with no padding.</param>
        /// <param name="interpolationMode">Quality of the interpolation used for resizing the images.</param>
        /// <returns>A single image with the Grid as described before.</returns>
        private Image MergeImageInGrid(List<Image> images, int imageWidth, int cellWidth, int cellHeight, System.Drawing.Drawing2D.InterpolationMode interpolationMode) {
            int cols = imageWidth / cellWidth;

            //Integer division of doing "ceiling"
            int rows = (images.Count + cols - 1) / cols;

            int gridHeight = rows * cellHeight;

            Image grid = new Bitmap(imageWidth, gridHeight);

            using (Graphics graphics = Graphics.FromImage(grid)) {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = interpolationMode;
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                for (int i = 0; i < images.Count; ++i) {
                    //x = (i % cols) * cellWidth, this makes it wrap on the row
                    //y =  (i / cols) * cellHeight, this makes it go down a row every time a full row is drawn
                    graphics.DrawImage(images[i], (i % cols) * cellWidth, (i / cols) * cellHeight);
                }
            }

            return grid;
        }

        /// <summary>
        /// All of the configurations and a helper to load them from the arguments.
        /// </summary>
        private class NetCoreProgramConfiguration : Configuration.ProgramConfiguration {
            private int _interpolationMode;
            public System.Drawing.Drawing2D.InterpolationMode InterpolationMode {
                get {
                    return (System.Drawing.Drawing2D.InterpolationMode)_interpolationMode;
                }
                set {
                    _interpolationMode = (int)value;
                }
            }

            /// <summary>
            /// Sets the known default values.
            /// </summary>
            public NetCoreProgramConfiguration() : base() {

                //System.Drawing.Drawing2D.InterpolationMode interpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                _interpolationMode = (int)System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            }

            /// <summary>
            /// Fills the fields using the supplied argument dictionary.
            /// Does additional validation.
            /// </summary>
            /// <param name="arguments">The fields and their values in a dictionary.</param>
            public new void LoadFrom(Dictionary<string, string> arguments) {
                base.LoadFrom(arguments);

                if (arguments.ContainsKey(NetCoreArgumentConfiguration.InterpolationModeArg)) {
                    if (int.TryParse(arguments[NetCoreArgumentConfiguration.InterpolationModeArg], out int temp) &&
                        Enum.IsDefined(typeof(System.Drawing.Drawing2D.InterpolationMode), temp)) {
                        _interpolationMode = temp;
                    }
                }
            }
        }

        /// <summary>
        /// Constants with the name of the arguments in lowercase
        /// </summary>
        private class NetCoreArgumentConfiguration : Configuration.ArgumentConfiguration {
            public static string InterpolationModeArg = "interpolationmode";
        }
    }
}
