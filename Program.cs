using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileManager
{
    internal class Program
    {
       public static void Main(string[] args)
        {
            //Если приложение было испоьзовано ранее, оператор открывает последний активный каталог
            if(Properties.Settings.Default.Start!="new")
            {
                PrintDirAndFiles(Properties.Settings.Default.Start,Properties.Settings.Default.NumberOfPage, Properties.Settings.Default.NumberOfFilePage);
            }
            //Создает папку и в ней файл для записи всех отловленных ошибок
            if (!Directory.Exists(@"D:\\FileManager\\errors"))
            {
                Directory.CreateDirectory(@"D:\\FileManager\\errors");
                File.Create(@"D:\\FileManager\\errors\\all_exceptions.txt");
            }
            while (true)
            {
                string commandLine = Console.ReadLine();
                if (commandLine =="quit")
                {
                    return;
                }
                string[] commands = CommandParser(commandLine);
                string[] currentCommand = SpaceException(commands);
                SwitchCommand(currentCommand);
            }
           
        }
        //Метод считает вес каталога в байтах
        public static long FolderSize(string path,ref long size)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        FileInfo file = new FileInfo(files[i]);
                        size = size + file.Length;
                    }
                }
                string[] dirs = Directory.GetDirectories(path);
                if (dirs != null)
                {
                    for (int j = 0; j < dirs.Length; j++)
                    {
                        FolderSize(dirs[j],ref size);
                    }
                }
            }
            return size;
        }
        //Метод отображения основной информации о каталоге/файле
        public static void ShowInfo(string path)
        {
            if (Directory.Exists(path))
            {
                //Переменная, которая используется для подсчета веса каталога
                long count = 0;
                try
                {
                    DirectoryInfo directory = new DirectoryInfo(path);
                    Console.WriteLine($"Имя каталога: {directory.Name}");
                    Console.WriteLine($"Атрибут каталога: {directory.Attributes}");
                    Console.WriteLine($"Время создания каталога: {directory.CreationTime}");
                    Console.WriteLine($"Размер каталога: {FolderSize(path, ref count)} B");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    try
                    {
                        FileInfo file = new FileInfo(path);
                        Console.WriteLine($"Имя файла: {file.Name}");
                        Console.WriteLine($"Атрибут файла: {file.Attributes}");
                        Console.WriteLine($"Время создания файла: {file.CreationTime}");
                        Console.WriteLine($"Размер файла: {file.Length} B");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
                    }
                }
                else
                {
                    Console.WriteLine("Указанный каталог/файл не найден.");
                }
            }
        }
        //Метод удаляет файл или каталог с помощью рекурсии
        public static string DeleteDirsAndFiles(string path)
        {
            string fin = "Файл Удален";
            if(File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch(Exception ex)
                {
                    fin = ex.Message;
                    File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
                }
            }
            else
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        string [] files =Directory.GetFiles(path);
                        if(files != null)
                        {
                            for (int i=0; i<files.Length; i++)
                            {
                                File.Delete(files[i]);
                            }
                        }
                        string [] dirs = Directory.GetDirectories(path);
                        if(dirs != null)
                        {
                            for (int i =0; i < dirs.Length; i++)
                            {
                                DeleteDirsAndFiles(dirs[i]);
                            }
                        }
                        Directory.Delete(path);
                    }
                    catch (Exception ex)
                    {
                        fin = ex.Message;
                        File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
                    }
                    fin = "Каталог удален";
                }
                else
                {
                    fin = "Указанный каталог/файл не найден.";
                }
            }
            return fin;
        }
        //Метод копирует каталог/файл в указанное место
        public static string CopyDirsAndFiles (string oldPath, string newPath)
        {
            //Осуществялет копирование папок рекурсивно
            if (Directory.Exists(oldPath))
            {
                string finall = "Копирование каталога выполнено успешно.";
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(oldPath);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    Directory.CreateDirectory($"{newPath}/{dir.Name}");
                    string[] files = Directory.GetFiles(oldPath);
                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            FileInfo file = new FileInfo(files[i]);
                            File.Copy(file.FullName, $"{newPath}\\{dir.Name}\\{file.Name}", true);
                        }
                    }
                    string[] dirs = Directory.GetDirectories(oldPath);
                    {
                        if (dirs != null)
                            for (int j = 0; j < dirs.Length; j++)
                            {
                                CopyDirsAndFiles(dirs[j], $"{newPath}/{dir.Name}");
                            }
                    }
                }
                catch (Exception ex)
                {
                    finall = "Ошибка.";
                    Console.WriteLine(finall);
                    Console.WriteLine(ex.Message);
                    File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
                }
                return finall;
            }
            //Осуществляет копирование файла, также отлавливает разного вида ошибки
            else
            {
                string fin= "Файл успешно скопирован."; 
                try
                {
                    File.Copy(oldPath, newPath, true);
                }
                catch (FileNotFoundException ex)
                {
                    fin ="Файл не найден.";
                    File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
                }
                catch (DirectoryNotFoundException ex)
                {
                    fin ="Путь указан неверно.";
                    File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
                }
                catch (IOException ex)
                {
                    fin ="В качестве файла назначения указан каталог, а не файл.";
                    File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
                }
                return fin;
            }
        }
        /* Данный метод отрабатывает в тех случаях, когда в пути есть каталог с пробелом в названии (такой, как  c:\Новая папка), 
         * т.к. парсер разделяет команды пробелами, он с этой задачей не справляется */
        public static  string[] SpaceException (string [] list)
        {
            string[] spaceList = new string[list.Length];
            spaceList[0] = list[0];
            string path = "";
            if (list[0] == "cd"|| list[0]=="dl" || list[0] == "info")
            {
                if (list.Length > 3)
                {
                    for (int i = 1; i < list.Length; i++)
                    {
                        if(list[list.Length-1] ==" ")
                        {
                            break;
                        }
                        if (i == 1)
                        {
                            path = list[i];
                        }
                        else
                        {
                            path = path + " " + list[i];
                        }
                    }
                    spaceList[1] = path;
                }
                else
                {
                    spaceList = list;
                }
            }
            if (list[0] =="ls")
            {
                if(list.Length > 5)
                {
                    for (int i = 1; i < list.Length-2; i++)
                    {
                        if (i == 1)
                        {
                            path = list[i];
                        }
                        else
                        {
                            path = path + " " + list[i];
                        }
                    }
                    spaceList[1] = path;
                    spaceList[2] = list[list.Length-2];
                    spaceList[3] = list[list.Length - 1];
                }
                else
                {
                    spaceList = list;
                }
            }
            if (list[0] == "cp")
            {
                if (list.Length > 3)
                {
                    for (int i = 2; i < list.Length; i++)
                    {
                        if (list[list.Length - 1] == " ")
                        {
                            break;
                        }
                        if (i == 2)
                        {
                            path = list[i];
                        }
                        else
                        {
                            path = path + " " + list[i];
                        }
                    }
                    spaceList[1] = list[1];
                    spaceList[2] = path;
                }
                else
                {
                    spaceList = list;
                }
            }
            
            return spaceList;
        }
        //Метод, который выполняет ту или иную команду, которую написал пользователь
        public static void SwitchCommand(string []listOfCommand)
        {
            string[] currentCommand = listOfCommand;
            try
            {
                switch (currentCommand[0].ToLower())
                {
                    case "cd":
                        PrintDirAndFiles(currentCommand[1].Trim(), 0, 0);
                        //Сохранение текущего активного каталога, чтобы в случае закрытия и открытия приложеняия снова консоль выводила последний каталог
                        Properties.Settings.Default.Start = currentCommand[1].Trim();
                        Properties.Settings.Default.NumberOfPage = 0;
                        Properties.Settings.Default.NumberOfFilePage = 0;
                        Properties.Settings.Default.Save();
                        break;
                    case "ls":
                        string oldPath = Properties.Settings.Default.Start;
                        int numberOfPage = 0;
                        int numberOfFilePage = 0;
                        if (currentCommand.Length < 5)
                        {
                            Console.WriteLine("Не указан номер страницы.");
                            break;
                        }
                        if (currentCommand[2] == "-p")
                        {
                            //Конструкция позволяет при перходе на другую страницу каталогов оставаться на выбранной странице файлов и наоборот
                            if(oldPath == currentCommand[1].Trim())
                            {
                                numberOfFilePage = Properties.Settings.Default.NumberOfFilePage;
                            }
                            PrintDirAndFiles(currentCommand[1].Trim(), Convert.ToInt32(currentCommand[3]) - 1, numberOfFilePage);
                            Properties.Settings.Default.Start = currentCommand[1].Trim();
                            Properties.Settings.Default.NumberOfPage = Convert.ToInt32(currentCommand[3])-1;
                            Properties.Settings.Default.NumberOfFilePage = numberOfFilePage;
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            if (currentCommand[2] == "-fp")
                            {
                                if (oldPath == currentCommand[1].Trim())
                                {
                                    numberOfPage = Properties.Settings.Default.NumberOfPage;
                                }
                                PrintDirAndFiles(currentCommand[1].Trim(), numberOfPage, Convert.ToInt32(currentCommand[3]) - 1);
                                Properties.Settings.Default.Start = currentCommand[1].Trim();
                                Properties.Settings.Default.NumberOfPage = numberOfPage;
                                Properties.Settings.Default.NumberOfFilePage = Convert.ToInt32(currentCommand[3])-1;
                                Properties.Settings.Default.Save();
                            }
                            else
                            {
                                Console.WriteLine("Введена некорректная команда. Для вывода списка доступных команд напишите help.");
                            }
                        }
                        break;
                    case "ps":
                        int size = Convert.ToInt32(currentCommand[1]);
                        if (size < 5)
                        {
                            Console.WriteLine("Минимальнальное количество выводимых элементов должно быть не меньше 5.");
                        }
                        else
                        {
                            Properties.Settings.Default.PageSize = size;
                            Properties.Settings.Default.Save();
                        }
                        break;
                    case "width":
                        int width = Convert.ToInt32(currentCommand[1]);
                        if (width < 70)
                        {
                            Console.WriteLine("Минимальнальная ширина окна должна быть больше 70.");
                        }
                        else
                        {
                            Properties.Settings.Default.WidthOfConsole = width;
                            Properties.Settings.Default.Save();
                        }
                        break;
                    case "cp":
                        Console.WriteLine(CopyDirsAndFiles(currentCommand[1].Trim(), currentCommand[2].Trim()));
                        break;
                    case "dl":
                        Console.WriteLine(DeleteDirsAndFiles(currentCommand[1].Trim()));
                        break;
                    case "info":
                        ShowInfo(currentCommand[1].Trim());
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "help":
                        Help();
                        break;
                    default:
                        Console.WriteLine("Введена некорректная команда. Для вывода списка доступных команд напишите help.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Введена некорректная команда.Для вывода списка доступных команд напишите help.");
                File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
            }
        }
        //Парсер, который принимает на вход строку, написанную пользователем и разделяет ее для дальнейшей работы программы
        public static string[] CommandParser (string command)
        {
            /*данный цикл создает массив для будущих расперсеных команд и предотвращает случаи, 
             * когда из-за большого кол-ва пробелов будет выделятся много места под массив */
            int count=1;
            for(int i = 0;i < command.Length;i++)
            {
                if(command[i]==' ')
                {
                    if(i == command.Length - 1 || command[i+1] != ' ')
                    {
                        count++;
                    }
                }
            }
            string[] listOfCommand = new string[count+1];
            string com="";
            int size = 0;
            int copyIndex = 0;
            //Основной цикл, который парсит команды
            for(int a=0; a<command.Length;a++)
            {
                //Отдельная проверка для копирования файлов, так как в команде используется специфичный символ
                if (listOfCommand[copyIndex] == "cp")
                {
                    if (command[a]!='>')
                    {
                        com = com + command[a];
                    }
                    else
                    {
                        listOfCommand[size] = com;
                        copyIndex ++;
                        size++;
                        com = "";
                    }
                }
                else
                {
                    if (command[a] != ' ')
                    {
                        com = com + command[a];
                    }
                    if (a == command.Length - 1)
                    {
                        listOfCommand[size] = com;
                        break;
                    }
                    if (command[a] == ' ' || a == command.Length - 1)
                    {
                        if (a == 0 || command[a - 1] == ' ')
                        {
                            continue;
                        }
                        listOfCommand[size] = com;
                        size++;
                        com = "";
                    }
                }
            }
            return listOfCommand;
        }
        //Метод вывода каталогов и файлов 2-х уровней с пейджингом
        public static void PrintDirAndFiles(string path, int numberOfPage, int numberOfFilePage)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                int width = Properties.Settings.Default.WidthOfConsole; // Ширина отрисовки конслои
                int len = path.Length;
                int count = 0; //Счетчик для случая, если выводимое кол-во каталогов будет меньше значения PageSize, консоль дорисовует рамки до мин. значения 
                int fileCount = 0; //Аналогично для файлов
                int page = numberOfPage;
                int maxPage = dirs.Length / Properties.Settings.Default.PageSize;
                if (dirs.Length % Properties.Settings.Default.PageSize != 0)
                {
                    maxPage++;
                }
                int filePage = numberOfFilePage;
                int maxFilePage = files.Length / Properties.Settings.Default.FilePageSize;
                if (files.Length % Properties.Settings.Default.FilePageSize != 0)
                {
                    maxFilePage++;
                }
                if (page >= maxPage || filePage >= maxFilePage)
                {
                    if (page != 0 || filePage != 0)
                    {
                        Console.WriteLine("Введенная страница не найдена.");
                        return;
                    }
                }
                if (path.Length > width - 20)
                {
                    Console.WriteLine("Длина указанного пути слишком велика, чтобы отобразить в консоли. Для корректного отображения увеличьте параметр width.");
                    return;
                }
                Console.Clear();
                int minElem = page * Properties.Settings.Default.PageSize;
                int maxElem = minElem + Properties.Settings.Default.PageSize;
                Console.Write('┌'); // Конструкция для рисования "шапки" консоли
                for (int i = 0; i < width; i++)
                {
                    Console.Write('─');
                }
                Console.Write("┐\n");
                Console.Write('│');
                Console.Write($"{path}");
                //Цикл для отрисовки боковых границ
                for (int i = len; i <= width; i++)
                {
                    if (i != width)
                    {
                        Console.Write(' ');
                    }
                    else
                    {
                        Console.Write("│\n");
                    }

                }
                /* В случае, если в названии пути последним символом указывается не '\',
                 *а просто символ, то данная операция позволит отбражать название каталогов и дирикторий без '\'в начале */
                if (len > 4)
                {
                    if (path[len - 1] != '\u005C')
                    {
                        len++;
                    }
                }
                //Основной цикл, который выводит два уровня дерева
                for (int i = minElem; i < maxElem; i++)
                {
                    if (dirs.Length <= i)
                    {
                        if (i == 0)
                        {
                            string empty = "│По указанному пути нет каталогов";
                            Console.Write(empty);
                            for (int l = empty.Length - 1; l <= width; l++)
                            {
                                if (l != width)
                                {
                                    Console.Write(' ');
                                }
                                else
                                {
                                    Console.Write("│\n");
                                }
                            }
                            maxPage = 1;
                        }
                        break;
                    }
                    Console.Write('│');
                    //Позвоялет отрисовывать первый уровень дерева не сначала, а с того места, на котором закончился путь
                    for (int c = 0; c < len - 2; c++)
                    {
                        Console.Write(' ');
                    }
                    //Конструкция ниже исключает ситуацию, когда название каталога слишком велико и может вылезти за рамки консоли
                    if (dirs[i].Length >= width)
                    {
                        Console.Write($"|-{dirs[i].Substring(len, Math.Abs(width - len - 3))}...│\n");
                        continue;
                    }
                    else
                    {
                        Console.Write($"|-{dirs[i].Substring(len)}");
                    }
                    count++;
                    for (int e = dirs[i].Length; e <= width; e++)
                    {
                        if (e != width)
                        {
                            Console.Write(' ');
                        }
                        else
                        {
                            Console.Write("│\n");
                        }
                    }
                    //С помощью этого логического оператора осуществлен показ каталогов второго уровня
                    try
                    {
                        if (Directory.GetDirectories(dirs[i]) != null)
                        {
                            string[] dirsLvl2 = Directory.GetDirectories(dirs[i]);
                            for (int a = 0; a < dirsLvl2.Length; a++)
                            {
                                Console.Write('│');
                                for (int x = 0; x < dirs[i].Length - 1; x++)
                                {
                                    //Данная конструкция рисует "ветви" дерева
                                    if (x == len - 2)
                                    {
                                        Console.Write('|');
                                    }
                                    else
                                    {
                                        Console.Write(' ');
                                    }
                                }
                                int errorPage2 = 0;
                                //Такая же конструкция для предотврашения вылезания названия за пределы, только для второго уровня
                                if (dirsLvl2[a].Length >= width)
                                {
                                    Console.Write($"|-{dirsLvl2[a].Substring(dirs[i].Length + 1, Math.Abs(width - dirs[i].Length - 4))}...");
                                    errorPage2 = width;
                                }
                                else
                                {
                                    Console.Write($"|-{dirsLvl2[a].Substring(dirs[i].Length + 1)}");
                                    errorPage2 = dirsLvl2[a].Length;
                                }
                                count++;
                                for (int e = errorPage2; e <= width; e++)
                                {
                                    if (e != width)
                                    {
                                        Console.Write(' ');
                                    }
                                    else
                                    {
                                        Console.Write("│\n");
                                    }
                                }
                            }
                        }
                    }
                    catch(UnauthorizedAccessException ex)
                    {
                        File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
                        continue;
                    }
                }
                //Данная конструкция рисует нижние рамки для области каталогов
                while (true)
                {
                    if (count < Properties.Settings.Default.PageSize)
                    {
                        Console.Write("│");
                        for (int w = 0; w <= width; w++)
                        {
                            if (w != width)
                            {
                                Console.Write(' ');
                            }
                            else
                            {
                                Console.Write("│\n");
                            }
                        }
                        count++;
                    }
                    else
                    {
                        Console.Write('├');
                        for (int i = 0; i < width; i++)
                        {
                            if (i == width - 20)
                            {
                                string pageinfo = $"Стр. {page + 1} из {maxPage}";
                                Console.Write(pageinfo);
                                i = i + pageinfo.Length;
                            }
                            Console.Write('─');
                        }
                        Console.Write("┤\n");
                        break;
                    }
                }
                int minEl = filePage * Properties.Settings.Default.FilePageSize;
                int maxEl = minEl + Properties.Settings.Default.FilePageSize;
                //Цикл, который выводит список файлов из указанного пути постранично
                for (int j = minEl; j < maxEl; j++)
                {
                    if (files.Length <= j)
                    {
                        if (j == 0)
                        {
                            string fileEmpty = "│По указанному пути нет файлов";
                            Console.Write(fileEmpty);
                            for (int t = fileEmpty.Length - 1; t <= width; t++)
                            {
                                if (t != width)
                                {
                                    Console.Write(' ');
                                }
                                else
                                {
                                    Console.Write("│\n");
                                }
                            }
                            maxFilePage = 1;
                        }
                        break;
                    }
                    Console.Write('│');
                    FileInfo fileInf = new FileInfo(files[j]);
                    /* Блок кода ниже выводит имя файла без расширения, после чего попускает p символов, 
                     * чтобы вне зависимости от длины имени, другие две колонки были ровными, 
                     * точно такой же способ применен и для написания расширения файла и его размера */
                    string fileName;
                    if (fileInf.Name.Length > 30)
                    {
                        fileName = $"{fileInf.Name.Substring(0, 27)}...";
                        Console.Write(fileName);
                    }
                    else
                    {
                        fileName = fileInf.Name.Substring(0, fileInf.Name.Length - fileInf.Extension.Length);
                        Console.Write(fileName);
                    }
                    fileCount++;
                    for (int p = fileName.Length; p <= 30; p++)
                    {
                        Console.Write(' ');
                        if (p == 30)
                        {
                            Console.Write(fileInf.Extension);
                        }
                    }
                    for (int m = fileInf.Extension.Length + 30; m <= 50; m++)
                    {
                        Console.Write(' ');
                        if (m == 50)
                        {
                            Console.Write($"{fileInf.Length} B");
                        }
                    }
                    for (int r = fileInf.Length.ToString().Length + 52; r <= width - 2; r++)
                    {
                        if (r != width - 2)
                        {
                            Console.Write(' ');
                        }
                        else
                        {
                            Console.Write("│\n");
                        }
                    }
                }
                //Данная конструкция рисует нижние рамки всего дерева
                while (true)
                {
                    if (fileCount < Properties.Settings.Default.FilePageSize)
                    {
                        Console.Write("│");
                        for (int w = 0; w <= width; w++)
                        {
                            if (w != width)
                            {
                                Console.Write(' ');
                            }
                            else
                            {
                                Console.Write("│\n");
                            }
                        }
                        fileCount++;
                    }
                    else
                    {
                        Console.Write('└');
                        for (int i = 0; i < width; i++)
                        {
                            if (i == width - 20)
                            {
                                string filePageInfo = $"Стр. {filePage + 1} из {maxFilePage}";
                                Console.Write(filePageInfo);
                                i = i + filePageInfo.Length;
                            }
                            Console.Write('─');
                        }
                        Console.Write("┘\n");
                        break;
                    }
                }
            }
            catch(DirectoryNotFoundException ex)
            {
                Console.WriteLine("Указанного каталога не существует.");
                File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
            }
            catch(UnauthorizedAccessException ex)
            {
                Console.WriteLine("Отказано в доступе к данному каталогу.");
                File.AppendAllText(@"D:\\FileManager\\errors\\all_exceptions.txt", $"{DateTime.Now} {ex.Message}\n");
            }
        }
        //Выводит список команд, доступных пользователю
        public static void Help()
        {
            string[] list = {
                @"""cd C:\Windows"" ─ Открытие директории по указонному пути и вывод каталогов и файлов этой директории постранично.",
                @"""ls D:\Documents -p(-fp) 2"" ─ Переход на заданную страницу каталогов (файлов) по указанному пути.",
                @"""ps 10"" ─ Изменение количества выводимых каталогов на одной странице.",
                @"""width 100"" ─ Изменение ширины окон, в котором выводятся каталоги и файлы.",
                @"""cp d:\Source.txt>c:\Target.txt"" ─ Копирование файла.",
                @"""cp d:\Source>c:\Target"" ─ Копирование каталога.",
                @"""dl d:\Source"" ─ Удаление каталога.",
                @"""dl d:\Source.txt"" ─ Удаление файла.",
                @"""info d:\Source"" ─ Предоставляет информацию о каталоге.",
                @"""info d:\Source.txt"" ─ Предоставляет информацию о файле.",
                @"""clear"" ─ Очищает консоль."
            };
            for(int i=0; i<list.Length; i++)
            {
                Console.WriteLine(list[i]);
            }    
        }
    }
}