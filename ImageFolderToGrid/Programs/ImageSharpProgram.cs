using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;

namespace ImageFolderToGrid.Programs {
    class ImageSharpProgram : IProgram {

        //.\ImageFolderToGrid.exe program="imagesharp" inputpath="drive:\path\to\input" outputpath="drive:\path\to\output" program="netcore" width=480 cellwidth=32 cellheight=32 sigma=4.5 radius=2 resamplerid=1

        /// <summary>
        /// Using the supplied arguments, will load all the images in a folder, create a new image and put all the images in the input folder in a grid manner, then save the new image to the output folder with the output name.
        /// Arguments (the ones marked with * are optional): 
        /// inputpath=drive:\path\to\input
        /// outputpath=drive:\path\to\output
        /// *outputfile=OutputGrid
        /// *width=520
        /// *cellwidth=26
        /// *cellheight=26
        /// *sigma=4.5
        /// *radius=2
        /// *resamplerid=1
        /// See <see cref="System.Drawing.Drawing2D.InterpolationMode"/> for more information on interpolation mode.
        /// </summary>
        /// <param name="arguments">Preprocessed arguments from the command line.</param>
        public void Main(Dictionary<string, string> arguments) {
            ImageSharpProgramConfiguration config = new ImageSharpProgramConfiguration();

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
                images.Add(Image.Load(path));
            }

            Image result = MergeImageInGrid(images, config.OutputWidth, config.CellWidth, config.CellHeight, config.Sigma, config.Radius, config.ResamplerId);

            if (result == null) {
                Console.WriteLine("Something went wrong merging the images.");
                return;
            }

            result.Save(config.OutputFilePath);

            result.Dispose();
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
        /// <param name="sigma">Sigma for the Gaussian Sharpen Processor. Original value was 9 but default for this program is 4.5.</param>
        /// <param name="radius">Radius for the Gaussian Sharpen Processor. Original value was 3 but default for this program is 2.</param>
        /// <param name="resamplerId">Arbitrary Id to map to a Resampler. Default is 1. <see cref="ImageSharpProgram.GetResampler(int)">See GetResampler(int).</see> </param>
        /// <returns>A single image with the Grid as described before.</returns>
        private Image MergeImageInGrid(List<Image> images, int imageWidth, int cellWidth, int cellHeight, float sigma, int radius, int resamplerId) {
            int cols = imageWidth / cellWidth;

            //Integer division of doing "ceiling"
            int rows = (images.Count + cols - 1) / cols;

            int gridHeight = rows * cellHeight;

            Image result = new Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(imageWidth, gridHeight);

            SixLabors.ImageSharp.Processing.Processors.Transforms.IResampler resampler = GetResampler(resamplerId);

            for (int i = 0; i < images.Count; ++i) {

                images[i].Mutate(i => i
                    .Resize(cellWidth, cellHeight, resampler)
                    .ApplyProcessor(new SixLabors.ImageSharp.Processing.Processors.Convolution.GaussianSharpenProcessor(sigma, radius)));

                //x = (i % cols) * config.CellWidth, this makes it wrap on the row
                //y =  (i / cols) * config.CellHeight, this makes it go down a row every time a full row is drawn
                result.Mutate(o => o.DrawImage(images[i], new Point((i % cols) * cellWidth, (i / cols) * cellHeight), 1f));
            }

            return result;
        }

        /// <summary>
        /// Helper function to get the Resamplers using an arbitrary Id between 1 and 14 inclusive instead.
        /// </summary>
        /// <param name="id">Arbitrary Id between 1 and 14 inclusive, mapped to one of the Resamplers.</param>
        /// <returns>The corresponding Resampler if the Id matches, Bicubic otherwise.</returns>
        private SixLabors.ImageSharp.Processing.Processors.Transforms.IResampler GetResampler(int id) {
            SixLabors.ImageSharp.Processing.Processors.Transforms.IResampler resampler = KnownResamplers.Bicubic;

            switch (id) {
                case 1:
                    resampler = KnownResamplers.Bicubic;
                    break;
                case 2:
                    resampler = KnownResamplers.Box;
                    break;
                case 3:
                    resampler = KnownResamplers.CatmullRom;
                    break;
                case 4:
                    resampler = KnownResamplers.Hermite;
                    break;
                case 5:
                    resampler = KnownResamplers.Lanczos2;
                    break;
                case 6:
                    resampler = KnownResamplers.Lanczos5;
                    break;
                case 7:
                    resampler = KnownResamplers.Lanczos8;
                    break;
                case 8:
                    resampler = KnownResamplers.MitchellNetravali;
                    break;
                case 9:
                    resampler = KnownResamplers.NearestNeighbor;
                    break;
                case 10:
                    resampler = KnownResamplers.Robidoux;
                    break;
                case 11:
                    resampler = KnownResamplers.RobidouxSharp;
                    break;
                case 12:
                    resampler = KnownResamplers.Spline;
                    break;
                case 13:
                    resampler = KnownResamplers.Triangle;
                    break;
                case 14:
                    resampler = KnownResamplers.Welch;
                    break;
            }

            return resampler;
        }

        /// <summary>
        /// All of the configurations and a helper to load them from the arguments.
        /// </summary>
        private class ImageSharpProgramConfiguration : Configuration.ProgramConfiguration {
            public float Sigma { get; set; }
            public int Radius { get; set; }
            public int ResamplerId { get; set; }

            /// <summary>
            /// Sets the known default values.
            /// </summary>
            public ImageSharpProgramConfiguration() : base() {
                Sigma = 4.5f;
                Radius = 2;
                ResamplerId = 1;
            }

            /// <summary>
            /// Fills the fields using the supplied argument dictionary.
            /// Does additional validation.
            /// </summary>
            /// <param name="arguments">The fields and their values in a dictionary.</param>
            public new void LoadFrom(Dictionary<string, string> arguments) {
                base.LoadFrom(arguments);

                if (arguments.ContainsKey(ImageSharpArgumentConfiguration.SigmaArg)) {
                    if (float.TryParse(arguments[ImageSharpArgumentConfiguration.SigmaArg], out float temp)) {
                        Sigma = temp;
                    }
                }

                if (arguments.ContainsKey(ImageSharpArgumentConfiguration.RadiusArg)) {
                    if (int.TryParse(arguments[ImageSharpArgumentConfiguration.RadiusArg], out int temp)) {
                        Radius = temp;
                    }
                }

                if (arguments.ContainsKey(ImageSharpArgumentConfiguration.ResamplerIdArg)) {
                    if (int.TryParse(arguments[ImageSharpArgumentConfiguration.ResamplerIdArg], out int temp)) {
                        ResamplerId = temp;
                    }
                }

            }
        }

        /// <summary>
        /// Constants with the name of the arguments in lowercase
        /// </summary>
        private class ImageSharpArgumentConfiguration : Configuration.ArgumentConfiguration {
            public static string SigmaArg = "sigma";
            public static string RadiusArg = "radius";
            public static string ResamplerIdArg = "resamplerid";
        }
    }
}
