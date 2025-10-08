# EtheriumModPackager

**EtheriumModPackager** is a simple utility for creating `.etheriummod` files from a directory of files, including mod DLLs and assets, for use with mods made for Etherium. It supports automatic version detection, drag-and-drop, and command-line usage.

## Features

- Create `.etheriummod` files from a folder of mod files.
- Automatically detect the main DLL and read its version to append to the mod filename.
- Include all files and subdirectories in the mod package.
- Optionally specify the main DLL via a command-line argument.
- Works via drag-and-drop, command-line arguments, or interactive prompts.

## Requirements

Windows 10/11 or later.

## Usage
### Drag-and-Drop

1. Drag a folder containing your mod files (DLL and assets) onto EtheriumModPackager.exe.
2. The .etheriummod file will be created in the same folder as the mod directory.

### Command-Line
```EtheriumModPackager.exe "C:\Path\To\ModDirectory" "MainDLLName.dll"```

1. First argument: path to the mod directory.
2. Optional second argument: specify the main DLL filename to bypass auto-detection.

### Interactive Prompt

1. If no arguments are provided, the program will prompt for the directory path.
2. If multiple DLLs exist and none match the folder name, you will be prompted to select the main DLL.

## Mod File Naming

The generated .etheriummod file is named using the main DLL filename and its version:

```MainDLL_1_0_0.etheriummod```


If the version cannot be read, the version suffix is omitted e.g:

```MainDLL.etheriummod```

## File Structure

All files and subdirectories within the selected mod directory are included in the `.etheriummod`.
Files are stored at the root of the archive, not nested under the directory name.

#### Example:

```
ModDirectory/
  ├─ MyMod.dll
  ├─ Assets/
      ├─ texture.png
```


Resulting `MyMod.etheriummod`:

```
MyMod.dll
Assets/texture.png
```

## Build Instructions

1. Clone or download the repository.
2. Open the solution in Visual Studio 2022 or later.
3. Build the project in Release mode.
4. Optionally, publish a single EXE using the Publish feature or via command line:

```dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true /p:SelfContained=true```

The resulting EXE can be distributed as a standalone tool.

## Notes

The program uses ICSharpCode.SharpZipLib for compression.
The tool ensures that `.etheriummod` files are portable and easy to distribute.

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE](../blob/main/LICENSE) for details.
