# image-folder-to-grid
Generates a single image Grid using the images in the input Folder.

# Third Party Licenses
While making this project I used 2 NuGet Packages:
* System.Drawing.Common by Microsoft, which is under the
  [MIT License](https://licenses.nuget.org/MIT).
* SixLabors.ImageSharp by Six Labors and contributors, under the
  [GNU Affero General Public License v3](https://www.gnu.org/licenses/agpl-3.0).

# Conception
The idea for this project came from a friend that is developing an Online RPG
called [AetherStory](https://www.aetherstory.com/) . They mentioned it while
talking with their chat during a stream, and I thought it'd be fun to make.

# Purpose
The idea was that you'd have a folder full of images, and you'd want to spread
these images in a single file in grid form. This is very useful when making
games.

# Solution
The solution is as simple as the idea. It was made in a console app so it
could be easily automated, it'd take AT LEAST a program, an input folder, and
an output folder. It'd scan the folder for images, resize each one, and
position them using a simple formula that puts them one by one horizontally,
and wraps back to the first cell on the next row when a row is complete.

# Parameters
There are 2 different programs that can be used, and they share some parameters
but not all of them, here are two examples and their descriptions:

# Net Core program:
```
.\ImageFolderToGrid.exe program="netcore" inputpath="drive:\path\to\input" outputpath="drive:\path\to\output" width=480 cellwidth=32 cellheight=32 interpolationmode=7
```
* Program: specifies which of the two programs to use to process the images, in this case netcore.
* Input Path: path to the folder with all the images in it.
* Output Path: which folder the output image containing the grid will be saved to.
* Width: width of the grid in the output file.
* Cell Width: width of each cell in the grid. It is possible that the width is not multiple of the cell width, in which case there will be empty space at the end of the row.
* Cell Height: the height of each cell. This is also used to calculate the height of the resulting image.
* Interpolation Mode: the interpolation mode method to expand or shrink. 
```
public enum InterpolationMode {
	Invalid = -1,
	Default = 0,
	Low = 1,
	High = 2,
	Bilinear = 3,
	Bicubic = 4,
	NearestNeighbor = 5,
	HighQualityBilinear = 6,
	HighQualityBicubic = 7
}
```

# ImageSharp program:
```
.\ImageFolderToGrid.exe program="imagesharp" inputpath="drive:\path\to\input" outputpath="drive:\path\to\output" program="netcore" width=480 cellwidth=32 cellheight=32 sigma=4.5 radius=2 resamplerid=1
```
* Program: specifies which of the two programs to use to process the images, in this case imagesharp.
* Input Path: path to the folder with all the images in it.
* Output Path: which folder the output image containing the grid will be saved to.
* Width: width of the grid in the output file.
* Cell Width: width of each cell in the grid. It is possible that the width is not multiple of the cell width, in which case there will be empty space at the end of the row.
* Cell Height: the height of each cell. This is also used to calculate the height of the resulting image.
* Sigma: this is one of the two values used by the Gaussian Sharpening Processor. Default in the source is 9.0, but for my purposes default is 4.5
* Radius: the other value used by Gaussian Sharpening Processor. Default in the source is 3, but for my purposes default is 2
* ResamplerId: There are 14 different resamplers that are used when expanding and shrinking the images. The default one is Bicubic.
