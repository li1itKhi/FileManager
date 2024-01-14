using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_KhidiryanPOB2212  //C:\Users\Лилит\Desktop
{
    internal class Program
    {
        public static int WINDOW_WIDTH = 120; //120
        public static int WINDOW_HEIGHT = 40; //40

        public static string tree = "";

        public static string currentDir = "C:\\Users\\Лилит\\Desktop";
        public static string rootDir = "C:\\Users\\Лилит\\Desktop";
        
        public static char accept; // хранит значение пользовательского выбора Y/N
        static void Main(string[] args)
        {
            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);

            DrawConsole(0, 0, WINDOW_WIDTH, 16);

            DrawConsole(0, 18, WINDOW_WIDTH, 8);

            UpdateConsole();
        }

        public static void UpdateMessageBox(string message) //бокс ошибок
        {
            DrawConsole(0, 18, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 19);
            Console.Write(message);
        }

        public static void UpdateConsole()
        {
            DrawInputCommandField();
            CommandInputProcess();
        }

        public static void CommandInputProcess()
        {
            StringBuilder command = new StringBuilder();

            int left = 0; int top = 0;
            GetCurrentCursorPosition(ref top, ref left);

            char pressedKey;

            do
            {
                int currentLeft = 0; int currentTop = 0;
                GetCurrentCursorPosition(ref currentTop, ref currentLeft);

                pressedKey = Console.ReadKey().KeyChar;

                if ((ConsoleKey)pressedKey == ConsoleKey.Backspace && Console.CursorLeft < left)
                {
                    Console.Write('>');
                }
                else if ((ConsoleKey)pressedKey == ConsoleKey.Backspace)
                {
                    command.Remove(command.Length - 1, 1);

                    Console.Write(' ');
                    Console.SetCursorPosition(currentLeft - 1, currentTop);
                }
                else if (currentLeft >= WINDOW_WIDTH - 2)
                {
                    Console.SetCursorPosition(currentLeft, currentTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(currentLeft, currentTop);
                }
                else if ((ConsoleKey)pressedKey != ConsoleKey.Enter)
                {
                    command.Append(pressedKey);
                }
            }
            while ((ConsoleKey)pressedKey != ConsoleKey.Enter);

            CommandParser(command.ToString());
        }

        public static void CommandParser(string command) //поле команд
        {
            string[] commandPoint = command.Split(' ');
            
            try
            {
                switch (commandPoint[0])
                {
                    case "cd": //перемещение между директориями
                        if (commandPoint.Length < 2)
                        {
                            currentDir = rootDir;
                        }
                        else if (commandPoint[1] == "..") //на уровень выше
                        {
                            string[] folders = currentDir.Split('\\');
                            currentDir = "";

                            for (int i = 0; i < folders.Length - 1; i++)
                            {
                                if (i < folders.Length - 2)
                                {
                                    currentDir += folders[i] + '\\';
                                }
                                else
                                {
                                    currentDir += folders[i];
                                }
                            }
                        }
                        break;

                    case "tree": //вывод дерева
                        if (commandPoint.Length < 3)
                        {
                            DrawTree(int.Parse(commandPoint[1]), currentDir);
                        }
                        else
                        {
                            DrawTree(int.Parse(commandPoint[1]), commandPoint[2]);
                        }
                        break;

                    case "mkdir": //создание папки
                        bool isRelativePhathExist = Directory.Exists(currentDir += '\\' + commandPoint[1]);
                        bool isAbsoletPhathExist = Directory.Exists(commandPoint[1]);

                        //  проверка на абсолютный путь        инвертирование значения
                        if (commandPoint[1].Contains('\\') && !isAbsoletPhathExist) //если папк нет то создается новая
                        {
                            Directory.CreateDirectory(commandPoint[1]);

                        }
                        // если папка существует, значение инвертируется в false
                        else if (commandPoint[1].Contains('\\') && !isRelativePhathExist)
                        {
                            Directory.CreateDirectory(currentDir += '\\' + commandPoint[1]);
                        }

                        else if (Directory.Exists(commandPoint[1]) || isRelativePhathExist)
                        {
                            UpdateMessageBox("Ошибка папки или папка существует");
                        }
                        else
                        {
                            UpdateMessageBox("Ошибка");
                        }
                        break;

                    case "touch": //создание файлов
                        bool isRelativePhathExist_f = File.Exists(currentDir += '\\' + commandPoint[1]);
                        bool isAbsoletPhathExist_f = File.Exists(commandPoint[1]);

                        if (!commandPoint[1].Contains('.'))
                        {
                            UpdateMessageBox("Укажите расширение файла");
                        }
                        //  проверка на абсолютный путь        инвертирование значения
                        else if (commandPoint[1].Contains('\\') && !isAbsoletPhathExist_f) //если папк нет то создается новая
                        {
                            File.Create(commandPoint[1]);

                        }
                        // если папка существует, значение инвертируется в false
                        else if (commandPoint[1].Contains('\\') && !isRelativePhathExist_f)
                        {
                            File.Create(currentDir += '\\' + commandPoint[1]);
                        }

                        else if (File.Exists(commandPoint[1]) || isRelativePhathExist_f)
                        {
                            UpdateMessageBox("ERROR! ошибка файла или файл существует");
                        }
                        break;

                    case "cp": //копирование

                        if (File.Exists(commandPoint[1]))
                        {
                            File.Copy(commandPoint[1], commandPoint[2], false);
                        }
                        else
                        {
                            UpdateMessageBox("Такого файла не существует!");
                        }

                        break;

                    case "rm -r": //удаление файлов

                        if (File.Exists(commandPoint[1]))
                        {
                            Console.Write($"Вы уверены, что хотите удалить файл '{commandPoint[1]}'? (Y/N): ");

                            accept = char.Parse(Console.ReadLine());

                            if (accept == 'Y' || accept == 'y')
                            {
                                File.Delete(commandPoint[1]);

                                UpdateMessageBox("Файл успешно удален.");
                            }
                            else if (accept == 'N' || accept == 'n')
                            {
                                UpdateMessageBox("Операция удаления файла отменена.");
                            }
                        }
                        else
                        {
                            UpdateMessageBox("Такого файла не существует!");
                        }

                        break;

                    case "rm": //удаление директории

                        if (Directory.Exists(commandPoint[1]))
                        {
                            Console.Write($"Вы уверены, что хотите удалить директорию '{commandPoint[1]}'? (Y/N): ");

                            accept = char.Parse(Console.ReadLine());

                            if (accept == 'Y' || accept == 'y')
                            {
                                Directory.Delete(commandPoint[1]);

                                UpdateMessageBox("Директория удалена.");
                            }
                            else if (accept == 'N' || accept == 'n')
                            {
                                UpdateMessageBox("Операция удаления директории отменена.");
                            }
                        }
                        else
                        {
                            UpdateMessageBox("Такой директории не существует!");
                        }

                        break;

                    case "mv": //перемещение

                        if (File.Exists(commandPoint[1]))
                        {
                            Console.Write($"Вы уверены, что хотите переместить файл '{commandPoint[1]}' в '{commandPoint[2]}'? (Y/N): ");
                            
                            accept = char.Parse(Console.ReadLine());

                            if (accept == 'Y' || accept == 'y')
                            {
                                File.Move(commandPoint[1], commandPoint[2]);

                                UpdateMessageBox("Файл успешно перемещен.");
                            }
                            else if (accept == 'N' || accept == 'n')
                            {
                                UpdateMessageBox("Операция перемещения отменена.");
                            }
                        }
                        else
                        {
                            UpdateMessageBox("Такого файла не существует!");
                        }

                        break;

                    case "show": //демонстрация файлов
                        
                            if (commandPoint[1].Contains("\\"))
                            {
                                ShowingFiles(commandPoint[1]);
                            }
                            else if (commandPoint[1].Contains("."))
                            {
                                ShowingFiles(currentDir + "\\" + commandPoint[1]);
                            }
                            else
                            {
                                UpdateMessageBox("Неверные имя файла или путь к нему!");
                            }
                        
                        break;

                    case "help": //помощь
                        {
                            Help();
                        }
                        break;

                    case "write": //запись совершенных операциий
                        {
                            if (File.Exists(currentDir + "\\" + commandPoint[2]))
                            {
                                List<string> historyText = new List<string>();

                                historyText.AddRange(File.ReadLines(currentDir + "\\" + commandPoint[2]));

                                historyText.Add(commandPoint[1]);

                                File.WriteAllLines(currentDir + "\\" + commandPoint[2], historyText);

                                UpdateMessageBox("Данные записаны!");
                            }
                            else
                            {
                                UpdateMessageBox("Такого файла не существует!");
                            }
                        }
                        break;
                }
            }
            catch (Exception exMess)
            {
                UpdateMessageBox(exMess.Message);
            }

            UpdateConsole();
        }

        public static void DrawInputCommandField() //отрисовка поля для ввода команды
        {
            DrawConsole(0, 28, WINDOW_WIDTH, 1);

            int top = 0; int left = 0;
            GetCurrentCursorPosition(ref top, ref left);

            Console.SetCursorPosition(1, top - 1);

            Console.Write($"{currentDir}>");
        }

        public static void GetCurrentCursorPosition(ref int top, ref int left) //отслеживание положения курсора
        {
            top = Console.CursorTop;
            left = Console.CursorLeft;
        }

        public static void DrawConsole(int chordX, int chorZ, int width, int height) //(большое поле) отрисовка выполненных команд.
        {
            Console.SetCursorPosition(chordX, chorZ);

            Console.Write('╔');
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write('═');
            }
            Console.Write('╗');

            for (int row = 0; row < height; row++)
            {
                Console.Write('║');
                for (int column = 0; column < width - 2; column++)
                {
                    Console.Write(' ');
                }
                Console.Write('║');
            }
            Console.Write('╚');

            for (int i = 0; i < width - 2; i++)
            {
                Console.Write('═');
            }
            Console.Write('╝');
        }

        public static void GetTree(DirectoryInfo directory, string indent, bool lastDirectory) //отрисовка дерева
        {
            tree += indent;
            if (lastDirectory)
            {
                tree += "└──";
                indent += "   ";
            }
            else
            {
                tree += "├──";
                indent += "│  ";
            }

            tree += directory.Name + '\n';

            DirectoryInfo[] subDirectories = directory.GetDirectories();

            for (int i = 0; i < subDirectories.Length; i++)
            {
                GetTree(subDirectories[i], indent, i == subDirectories.Length - 1);
            }
        }

        public static void DrawTree(int page, string defaultPath) //проход по страницам
        {
            GetTree(new DirectoryInfo(defaultPath), "", true);

            DrawConsole(0, 0, WINDOW_WIDTH, 16);

            string[] line = tree.Split('\n');

            int linesAtPage = 16;

            int pagesQuantity = (int)Math.Ceiling(line.Length / (double)linesAtPage);

            string[,] pages = new string[pagesQuantity, linesAtPage];

            if (line.Length >= linesAtPage)
            {
                for (int i = 0; i < pages.GetLength(0); i++)
                {
                    int counter = 0;
                    for (int j = linesAtPage * i; j < linesAtPage * (i + 1); j++)
                    {
                        if (line[j] == "")
                        {
                            break;
                        }
                        pages[i, counter] = line[j];
                        counter++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < pages.GetLength(0); i++)
                {
                    int counter = 0;
                    for (int j = linesAtPage * i; j < line.Length; j++)
                    {
                        pages[i, counter] = line[j];
                        counter++;
                    }
                }
            }

            for (int i = 0; i < pages.GetLength(1); i++)
            {
                Console.SetCursorPosition(1, i + 1);

                Console.WriteLine(pages[page - 1, i]);
            }

            string currentPage = $"╡Страница: [ {page} / {pages.GetLength(0)} ]╞";
            Console.SetCursorPosition(WINDOW_WIDTH / 2 - currentPage.Length / 2, 17);
            Console.Write(currentPage);

            tree = "";
        }

        public static void ShowingFiles(string path)
        {
            try
            {
                int maxLines = WINDOW_HEIGHT - 32; // Максимальное количество строк для вывода

                int strConsole = 0;

                var lines = File.ReadLines(path, Encoding.UTF8).Take(maxLines);

                foreach (string line in lines)
                {
                    if (line.Length > WINDOW_WIDTH - 2)
                    {
                        string truncatedLine = line.Substring(0, WINDOW_WIDTH - 2);

                        Console.SetCursorPosition(2, 19 + strConsole);

                        Console.WriteLine(truncatedLine);
                    }
                    else
                    {
                        Console.SetCursorPosition(2, 19 + strConsole);

                        Console.WriteLine(line);
                    }

                    strConsole++;
                }
            }
            catch (Exception ex)
            {
                UpdateMessageBox("Ошибка при чтении файла: " + ex.Message);
            }
        }

        public static void Help()
        {
            string helpComands = "Доступные команды:\n" +
                              "cd [путь] - изменить текущую директорию\n" +
                              "cd .. - перейти на уровень выше.\n" +
                              "tree [глубина] [путь] - отобразить дерево файлов и папок\n" +
                              "mkdir [путь] - создать новую директорию\n" +
                              "touch [путь] - создать новый файл\n" +
                              "cp [существующий путь] [новый путь] - скопировать файл\n" +
                              "rm -r[путь] - удалить файл\n" +
                              "rm [путь] - удалить директорию\n" +
                              "mv [старый путь] [новый путь] - переместить файл или директорию\n" +
                              "show [путь к файлу] - отобразить содержимое файла\n" +
                              "write [текст] [путь к файлу] - записать текст в файл\n" +
                              "help - отобразить эту справку\n";

            string[] helpLines = helpComands.Split('\n');

            int maxLines = WINDOW_HEIGHT - 32; // Максимальное количество строк для вывода

            int totalPages = (int)Math.Ceiling((double)helpLines.Length / maxLines);

            int page = 1;

            while (true)
            {
                DrawConsole(0, 18, WINDOW_WIDTH, 8); // Очищаем второе окно

                int startLine = (page - 1) * maxLines;

                int endLine = Math.Min(startLine + maxLines, helpLines.Length);

                for (int i = startLine; i < endLine; i++)
                {
                    Console.SetCursorPosition(1, i - startLine + 19); // Выводим второе окно

                    Console.WriteLine(helpLines[i]);
                }

                string pageInfo = $"╡Страница: {page} / {totalPages} (Для прокрутки используйте клавиши вверх и вниз, для завершения нажмите клавишу ESC)╞";
                
                Console.SetCursorPosition(WINDOW_WIDTH / 2 - pageInfo.Length / 2, 17);
                
                Console.Write(pageInfo);

                ConsoleKeyInfo keyInfo = Console.ReadKey();
               
                if (keyInfo.Key == ConsoleKey.DownArrow && page < totalPages)
                {
                    page++;
                }
                else if (keyInfo.Key == ConsoleKey.UpArrow && page > 1)
                {
                    page--;
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }

            UpdateConsole();
        }

    }
}
