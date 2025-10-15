using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;

// Silence warnings that annoy me
#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8604

namespace EtheriumModPackager
{
    class Program
    {
        static void Main(string[] args)
        {
            string directoryPath;
            string mainDllNameArg = null;

            // Handle command-line arguments or ask user for path
            if (args.Length > 0)
            {
                directoryPath = args[0];
                if (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine($"Directory '{directoryPath}' does not exist.");
                    return;
                }

                if (args.Length > 1)
                    mainDllNameArg = args[1]; // optional main DLL filename
            }
            else
            {
                // Prompt user for directory
                Console.Write("Enter path to the mod directory: ");
                directoryPath = Console.ReadLine();
                while (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine("Directory does not exist. Try again.");
                    directoryPath = Console.ReadLine();
                }
            }

            //Fix a bug where paths such as "./bin/net35/" would cause mods to not have version numbers in the file name
            directoryPath = Path.GetFullPath(directoryPath);

            // Find all DLLs in the directory
            string[] dllFiles = Directory.GetFiles(directoryPath, "*.dll", SearchOption.TopDirectoryOnly);
            if (dllFiles.Length == 0)
            {
                Console.WriteLine("No .dll file found in directory. Cannot create .etheriummod file.");
                return;
            }

            string dllFile;

            // If optional DLL argument provided, use it directly
            if (!string.IsNullOrEmpty(mainDllNameArg))
            {
                dllFile = Path.Combine(directoryPath, mainDllNameArg);
                if (!File.Exists(dllFile))
                {
                    Console.WriteLine($"Specified DLL '{mainDllNameArg}' does not exist in the directory.");
                    return;
                }
            }
            else
            {
                // Find what the main DLL file is
                if (dllFiles.Length == 1)
                {
                    dllFile = dllFiles[0];
                }
                else
                {
                    // Try to find DLL matching the folder name
                    string folderName = Path.GetFileName(directoryPath);
                    dllFile = dllFiles.FirstOrDefault(f =>
                        Path.GetFileNameWithoutExtension(f).Equals(folderName, StringComparison.OrdinalIgnoreCase));

                    // If no match, prompt the user
                    if (dllFile == null)
                    {
                        Console.WriteLine("Multiple DLLs found, but none match the folder name. Please select the main DLL:");
                        for (int i = 0; i < dllFiles.Length; i++)
                        {
                            Console.WriteLine($"{i + 1}: {Path.GetFileName(dllFiles[i])}");
                        }

                        int choice = 0;
                        while (choice < 1 || choice > dllFiles.Length)
                        {
                            Console.Write("Enter the number of the DLL to use: ");
                            string input = Console.ReadLine();
                            int.TryParse(input, out choice);
                        }

                        dllFile = dllFiles[choice - 1];
                    }
                }
            }

            string dllName = Path.GetFileNameWithoutExtension(dllFile);

            // Attempt to read version, omit if unavailable
            string versionString = "";
            try
            {
                Assembly assembly = Assembly.LoadFile(dllFile);
                Version ver = assembly.GetName().Version;
                if (ver != null)
                    versionString = $"_{ver.Major}_{ver.Minor}_{ver.Build}";
            }
            catch
            {
                // silently ignore if version cannot be read
            }

            string modFileName = $"{dllName}{versionString}.etheriummod";
            string modFilePath = Path.Combine(directoryPath, modFileName);

            Console.WriteLine($"Creating mod file: {modFilePath}");

            // 6. Compress directory contents into .etheriummod
            using (FileStream fsOut = File.Create(modFilePath))
            using (ZipOutputStream zipStream = new ZipOutputStream(fsOut))
            {
                zipStream.SetLevel(9); // maximum compression
                CompressDirectory(directoryPath, zipStream, directoryPath, modFilePath);
            }

            Console.WriteLine("Mod file created successfully!");
        }

        private static void CompressDirectory(string dirPath, ZipOutputStream zipStream, string basePath, string modFilePath)
        {
            foreach (string filePath in Directory.GetFiles(dirPath))
            {
                // Skip the mod file itself
                if (Path.GetFullPath(filePath) == Path.GetFullPath(modFilePath))
                    continue;

                string entryName = Path.GetRelativePath(basePath, filePath).Replace("\\", "/");
                ZipEntry entry = new ZipEntry(entryName)
                {
                    DateTime = DateTime.Now
                };
                zipStream.PutNextEntry(entry);

                using (FileStream fs = File.OpenRead(filePath))
                {
                    fs.CopyTo(zipStream);
                }

                zipStream.CloseEntry();
            }


            foreach (string subDir in Directory.GetDirectories(dirPath))
            {
                CompressDirectory(subDir, zipStream, basePath, modFilePath);
            }
        }
    }
}

#pragma warning disable CS8604
#pragma warning disable CS8602
#pragma warning restore CS8600