using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите диск:");

            DriveInfo[] drives = DriveInfo.GetDrives();
            int selectedDiskIndex = 0;

            while (true)
            {
                Console.Clear();

                for (int i = 0; i < drives.Length; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write($"{(i == selectedDiskIndex ? "-> " : "   ")}{drives[i].Name} - {FormatBytes(drives[i].AvailableFreeSpace)} свободно из {FormatBytes(drives[i].TotalSize)}");
                }

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    return; // Выход из программы
                }
                else if (key.Key == ConsoleKey.UpArrow && selectedDiskIndex > 0)
                {
                    selectedDiskIndex--;
                }
                else if (key.Key == ConsoleKey.DownArrow && selectedDiskIndex < drives.Length - 1)
                {
                    selectedDiskIndex++;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    DisplayDriveInfo(drives[selectedDiskIndex].RootDirectory.FullName);
                    break;
                }
            }
        }
    }

    static void DisplayDriveInfo(string path)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Текущий путь: {path}");

            // Ваш код для отображения информации о диске
            // Например, вы можете использовать код из предыдущего ответа для DisplayDriveInfo

            Console.WriteLine("Меню:");
            Console.WriteLine("-> 1. Просмотреть содержимое");
            Console.WriteLine("   2. Вернуться к выбору диска");

            ConsoleKeyInfo menuChoice = Console.ReadKey(true);

            if (menuChoice.Key == ConsoleKey.Escape)
            {
                return; // Возврат к выбору диска
            }
            else if (menuChoice.Key == ConsoleKey.D1)
            {
                path = DisplayFolderContents(path);
            }
            else if (menuChoice.Key == ConsoleKey.D2)
            {
                break; // Возврат к выбору диска
            }
        }
    }

    static string DisplayFolderContents(string path)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Содержимое папки {path}:");

            string[] directories = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            for (int i = 0; i < directories.Length; i++)
            {
                Console.WriteLine($"{(i == 0 ? "-> " : "   ")}{Path.GetFileName(directories[i])} (Папка)");
            }

            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{(i == 0 ? "-> " : "   ")}{Path.GetFileName(files[i])} (Файл)");
            }

            Console.WriteLine("Меню:");
            Console.WriteLine("-> 0. Вернуться назад");

            ConsoleKeyInfo menuChoice = Console.ReadKey(true);

            if (menuChoice.Key == ConsoleKey.Escape)
            {
                return path; // Возврат к предыдущему меню
            }
            else if (menuChoice.Key == ConsoleKey.D0)
            {
                string parentPath = Path.GetDirectoryName(path);
                if (parentPath != null)
                {
                    path = parentPath;
                }
            }
            else if (menuChoice.Key == ConsoleKey.Enter)
            {
                Console.WriteLine("\nВведите имя папки или файла для перехода:");
                string input = Console.ReadLine();

                string selectedPath = directories.FirstOrDefault(d => Path.GetFileName(d).Equals(input, StringComparison.OrdinalIgnoreCase));

                if (selectedPath == null)
                {
                    selectedPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals(input, StringComparison.OrdinalIgnoreCase));
                }

                if (selectedPath != null)
                {
                    if (Directory.Exists(selectedPath))
                    {
                        path = selectedPath;
                    }
                    else
                    {
                        OpenFileInDefaultProgram(selectedPath);
                    }
                }
                else
                {
                    Console.WriteLine("Некорректный выбор. Попробуйте еще раз.");
                    Console.ReadKey();
                }
            }
        }

        return path;
    }

    static void OpenFileInDefaultProgram(string filePath)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не удалось открыть файл. Ошибка: {ex.Message}");
            Console.ReadKey();
        }
    }

    static string FormatBytes(long bytes)
    {
        const int scale = 1024;
        string[] orders = { "TB", "GB", "MB", "KB", "Bytes" };
        long max = (long)Math.Pow(scale, orders.Length - 1);

        foreach (string order in orders)
        {
            if (bytes > max)
                return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

            max /= scale;
        }

        return "0 Bytes";
    }
}