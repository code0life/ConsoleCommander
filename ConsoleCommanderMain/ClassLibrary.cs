using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace ClassLibrary
{
    /// <summary>
    /// Класс для хранения всех данных
    /// </summary>
    public class State
    {
        private string userName;
        private string path;
        private int page;
        private List<string> history;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }
        public int Page
        {
            get { return page; }
            set { page = value; }
        }
        public State(string loadPath)
        {
            userName = Environment.UserName;
            if (!string.IsNullOrEmpty(loadPath) && Directory.Exists(loadPath))
            {
                Path = loadPath;
            }
            else
            {
                Path = $"C:\\Users\\{userName}";
            }
            page = 1;
            history = new List<string>();
        }
        public void SavePath(string patch)
        {
            Path = patch;
            ConsoleCommanderMain.ConsoleCommanderMain.SavePath(patch);
        }

        /// <summary>
        /// Добавление записи в историю комманд
        /// </summary>
        /// <param name="line">Строка для добавление в историю комманд</param>
        public void AddToHistory(string line)
        {
            history.Add(line);
        }

    }

    /// <summary>
    /// Класс по отображению всех имеющихся элементов
    /// </summary>
    public class Display
    {
        Brain brain;

        Title title;
        Body body;
        ConsoleLine cl;

        public Display(Brain b)
        {
            brain = b;
            title = new Title();
            body = new Body(b);
            cl = new ConsoleLine(b);
        }
        public Body Body
        {
            get { return body; }
        }
        /// <summary>
        /// Перерисовывает все элементы на экране
        /// </summary>
        public void Show()
        {
            while (true)
            {
                Console.Clear();
                title.Show();
                body.Show();
                brain.Info.Show();
                cl.Show();
            }
        }
    }
    /// <summary>
    /// Класс по отображению дерева котологов
    /// </summary>
    public class Body
    {
        private Brain brain;
        private int patchHeightBlock;
        private int pageHeightBlock;
        private int curPage;
        private int maxPages;

        public int PageHeightBlock
        {
            get { return pageHeightBlock; }
        }
        public int PatchHeightBlock
        {
            get { return patchHeightBlock; }
        }
        public int CurPage
        {
            get { return curPage; }
            set { curPage = value; }
        }
        public int MaxPages
        {
            get { return maxPages; }
            set { maxPages = value; }
        }
        public Body(Brain b)
        {
            patchHeightBlock = 1;
            pageHeightBlock = 1;
            curPage = 1;
            maxPages = 0;
            brain = b;
        }
        /// <summary>
        /// Показывает элемент body, в который входит вывод текущего пути, "пейджинга" и дерева каталогов.
        /// </summary>
        public void Show()
        {
            string patch = brain.State.Path;
            int titleHeightBlock = brain.Title.HeightBlock;
            FormatText.ShowInCenter(patch, titleHeightBlock);
            Console.SetCursorPosition(0, titleHeightBlock + PageHeightBlock+PatchHeightBlock);
            List<string> findLines = GetListFiles(patch);
            DrawTree(list: findLines, curPage);
            Console.SetCursorPosition(0, titleHeightBlock + PatchHeightBlock);
            DrawPages(findLines, curPage);
        }
        /// <summary>
        /// Выводит список каталогов
        /// </summary>
        /// <param name="list">Список из элементов каталога</param>
        /// <param name="curPage">Номер выводимой страницы из каталога</param>
        public void DrawTree(List<string> list, int curPage)
        {
            int maxTreeString = brain.MaxTreeString;

            Console.WriteLine("...");
            int startLine = maxTreeString * (curPage - 1);
            int endLine = startLine + maxTreeString;
            int i = 0;
            foreach (string fname in list)
            {
                if (startLine <= i && i < endLine)
                {
                    Console.WriteLine(fname);
                }
                i++;
            }
        }
        /// <summary>
        /// Рисует список страниц дерева каталога.
        /// </summary>
        /// <param name="lines">Список из элементов в каталоге</param>
        /// <param name="curPage">Номер выводимой страницы из каталога</param>
        /// <param name="limitPages">Ограничиваем кол-во видимых страниц</param>
        public void DrawPages(List<string> lines, int curPage, int limitPages = 50)
        {
            int maxTreeString = brain.MaxTreeString;
            if (lines.Count > maxTreeString)
            {
                float tmp_pages = lines.Count / maxTreeString;
                int count = tmp_pages == 0 ? (int)tmp_pages : (int)tmp_pages + 1;
                MaxPages = count;
                Console.Write("[");
                for (int i = 0; i < count; i++)
                {
                    if (limitPages > i)
                    {
                        int id = i + 1;
                        if (id == curPage)
                        {
                            FormatText.Color($" {id} ", backColor: ConsoleColor.DarkCyan);
                        }
                        else
                        {
                            Console.Write($" {id} ");
                        }
                    }

                }
                Console.Write("]");
            }
            else
            {
                MaxPages = 0;
            }
        }
        /// <summary>
        /// Получаем список файлов и папок в каталоге
        /// </summary>
        /// <param name="patch">Путь до каталога</param>
        /// <param name="cur_nesting">Текущий уровень глубины каталога</param>
        /// <param name="max_nesting">Максимальный уровень глубины каталога</param>
        /// <param name="parent_files">Информация о кол-ве файлов и папок у родителя</param>
        /// <returns></returns>
        public List<string> GetListFiles(string patch, int cur_nesting = 1, int max_nesting = 2, int parent_files = -1)
        {
            int maxSizeSubDirectory = brain.MaxSizeSubDirectory;
            List<string> ls = new List<string>();
            try
            {
                List<string> listFolders = new List<string>();
                List<string> listFiles = new List<string>();
                try
                {
                    listFiles.AddRange(Directory.GetFiles(patch, "*.*", SearchOption.TopDirectoryOnly));
                    listFolders.AddRange(Directory.GetDirectories(patch));
                }
                catch (UnauthorizedAccessException) { } //Недостаточно доступа к скрытым и системным файлам


                int count_folder = 0;
                int limit_count = 0;
                bool need_show_limit = true;

                foreach (string folder in listFolders)
                {
                    string new_patch = $"{patch}\\{Short(folder)}";

                    if (cur_nesting < max_nesting)
                    {
                        limit_count = 0;
                        need_show_limit = true;
                        if (count_folder != listFolders.Count - 1)
                        {
                            ls.Add($" ├─{Short(folder)}");
                        }
                        else
                        {
                            if(listFiles.Count != 0)
                            {
                                ls.Add($" ├─{Short(folder)}");
                            }
                            else
                            {
                                ls.Add($" └─{Short(folder)}");
                            }
                        }
                        int nesting = cur_nesting + 1;
                        int new_parent_files = listFolders.Count + listFiles.Count;
                        ls.AddRange(GetListFiles(folder, nesting, max_nesting, new_parent_files));
                    }
                    else
                    {
                        if (limit_count < maxSizeSubDirectory)
                        {
                            if (count_folder != listFolders.Count - 1)
                            {
                                ls.Add($" │  ├─{Short(folder)}");
                            }
                            else
                            {
                                if (listFolders.Count + listFiles.Count > maxSizeSubDirectory)
                                {
                                    if (parent_files > 0 && parent_files == 1)
                                    {
                                        ls.Add($"    ├─{Short(folder)}");
                                    }
                                    else
                                    {
                                        ls.Add($" │  ├─{Short(folder)}");
                                    }
                                }
                                else
                                {
                                    if (parent_files > 0 && parent_files == 1)
                                    {
                                        ls.Add($"   └─{Short(folder)}");
                                    }
                                    else
                                    {
                                        ls.Add($" │  └─{Short(folder)}");
                                    }
                                }
                            }
                            limit_count++;
                        }
                        if (need_show_limit && limit_count == maxSizeSubDirectory && (listFolders.Count + listFiles.Count > maxSizeSubDirectory))
                        {
                            ls.Add($" │  └─ ... (other {listFolders.Count + listFiles.Count - maxSizeSubDirectory} objects)");
                            need_show_limit = false;
                        }
                    }
                    count_folder++;
                }

                int count_file = 0;
                foreach (string filename in listFiles)
                {
                    if (cur_nesting < max_nesting)
                    {
                        limit_count = 0;
                        need_show_limit = true;
                        if (count_file != listFiles.Count-1)
                        {
                            ls.Add($" ├─{Short(filename)}");
                        }
                        else
                        {
                            ls.Add($" └─{Short(filename)}");
                        }
                    }
                    else
                    {
                        if (limit_count < maxSizeSubDirectory)
                        {
                            if (count_file != listFiles.Count - 1)
                            {
                                if (parent_files > 0 && parent_files > 1)
                                {
                                    ls.Add($" │  ├─{Short(filename)}");
                                }
                                else
                                {
                                    ls.Add($"    ├─{Short(filename)}");
                                }
                            }
                            else
                            {
                                if (parent_files > 0 && parent_files > 1)
                                {
                                    ls.Add($" │  └─{Short(filename)}");
                                }
                                else
                                {
                                    ls.Add($"    └─{Short(filename)}");
                                }
                              }
                            limit_count++;
                        }
                        if (need_show_limit && limit_count == maxSizeSubDirectory && (listFolders.Count + listFiles.Count > maxSizeSubDirectory))
                        {
                            if (parent_files > 0 && parent_files > 1)
                            {
                                ls.Add($" │  └─ ... (other {listFolders.Count + listFiles.Count - maxSizeSubDirectory} objects)");
                            }
                            else
                            {
                                ls.Add($"    └─ ... (other {listFolders.Count + listFiles.Count - maxSizeSubDirectory} objects)");
                            }
                            
                            need_show_limit = false;
                        }
                    }
                    count_file++;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return ls;
        }
        /// <summary>
        /// Получаем имя каталога или файла из выбранного пути
        /// </summary>
        /// <param name="patch">Путь до каталога или файла</param>
        /// <returns>Возвращаем имя файла или катлога</returns>
        public string Short(string patch)
        {
            return Path.GetFileName(patch);
        }
    }
    /// <summary>
    /// Класс по формированию заголовка
    /// </summary>
    public class Title
    {
        string name = "ConsoleCommander";
        string version = "1.0.1";
        int heightBlock = 1;

        public string Name
        {
            get { return name; }
        }
        public int HeightBlock
        {
            get { return heightBlock; }
        }
        public string Version
        {
            get { return version; }
        }
        /// <summary>
        /// Выводим имя и версию коммандера
        /// </summary>
        public void Show()
        {
            FormatText.ShowInCenter($"{Name} [Version {Version}]");
        }
    }
    /// <summary>
    /// Класс отображаемого блока с информацией
    /// </summary>
    public class Info
    {
        Brain brain;
        string fileName;
        string dirName;
        string error;
        string trues;
        string com;
        int heightBlock = 1;

        public Info(Brain b)
        {
            brain = b;
        }
        public Info(/*string _fileName*/)
        {
            //fileName = _fileName;
        }

        public int HeightBlock
        {
            get { return heightBlock; }
        }
        /// <summary>
        /// Если есть информация или ошибка, то выводим её
        /// </summary>
        public void Show()
        {
            if (!String.IsNullOrEmpty(error))
            {
                FormatText.ShowInDown($"Error! {error}",
                    addPosY: -1, isError: true);
            }
            else if (!String.IsNullOrEmpty(trues))
            {
                FormatText.ShowInDown($"Done! {trues}",
                    addPosY: -1, isTrues: true);
            }
            else if (!String.IsNullOrEmpty(com))
            {
                FormatText.ShowInDown($"{com}",
                    addPosY: -2, isInfoCommand: true);
            }
            else if (!string.IsNullOrEmpty(fileName))
            {
                FileInfo f = new FileInfo(fileName);
                //string attributes = File.GetAttributes(fileName).ToString();
                string attributes = GetAllAttributes(fileName);
                string access = GetAllAccess(fileName);
                FormatText.ShowInDown($"Name: {Path.GetFileNameWithoutExtension(fileName)}, Extension: {f.Extension}, Length: {f.Length} bytes, Full Path: {f.FullName.ToLowerInvariant()}, Type: File, " +
                    $"\nCreation Time: {f.CreationTime}, Last Access Time: {f.LastAccessTime}, Last Write Time: {f.LastWriteTime}" +
                    $" \n{(!string.IsNullOrEmpty(attributes) ? "Attributes: " + attributes : "")}{(!string.IsNullOrEmpty(access) ? " Access: " + access : "")}",
                    addPosY: -2, isInfoCommand: true);
            }
            else if (!string.IsNullOrEmpty(dirName))
            {
                DirectoryInfo dir = new DirectoryInfo(dirName);
                //string attributes = GetAllAttributes(dirName);
                string attributes = GetAllAttributes(dirName);
                string access = GetAllAccess(dirName);
                FormatText.ShowInDown($"Name: {dir.Name}, Full Path: {dir.FullName.ToLowerInvariant()}, Type: Directory, Creation Time: {dir.CreationTime}," +
                    $"\nLast Access Time: {dir.LastAccessTime}, Last Write Time: {dir.LastWriteTime}" +
                    $"\n{(!string.IsNullOrEmpty(attributes) ? "Attributes: " + attributes : "")}{(!string.IsNullOrEmpty(access) ? " Access: " + access : "")}",
                    addPosY: -2, isInfoCommand: true);
            }
            Clear();
        }
        /// <summary>
        /// Добавляем ошибку в переменную
        /// </summary>
        /// <param name="e">Текст ошибки</param>
        public void AddError(string e)
        {
            error = e;
        }
        public void AddTrue(string t)
        {
            trues = t;
        }
        public void CommandInfo(string t)
        {
            com = t;
        }
        public void FileInfo(string t)
        {
            fileName = t;
        }
        public void DirInfo(string t)
        {
            dirName = t;
        }
        /// <summary>
        /// Очищаем переменную с ошибкой
        /// </summary>
        public void Clear()
        {
            trues = "";
            error = "";
            com = "";
            fileName = "";
            dirName = "";
        }

        /// <summary>
        /// Берем строку из атрибутов файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns></returns>
        public static string GetAllAttributes(string path)
        {
            string str = string.Empty;

            try
            {
                if (File.Exists(path))
                {
                    try
                    {
                        FileAttributes tFSI = File.GetAttributes(path);

                        StringBuilder sb = new StringBuilder();
                        Dictionary<FileAttributes, String> dictionary = new Dictionary<FileAttributes, string> {
                { FileAttributes.Archive,"A" },{FileAttributes.Directory,"D" },{FileAttributes.Hidden,"H"},
                { FileAttributes.NotContentIndexed,"I"},{ FileAttributes.ReparsePoint,"L"},{ FileAttributes.ReadOnly,"R"},
                { FileAttributes.System,"S"}};

                        foreach (var atr in dictionary)
                        {
                            if (tFSI.HasFlag(atr.Key))
                            {
                                sb.Append(atr.Value);
                            }
                            else
                            {
                                sb.Append("-");
                            }
                        }

                        str = sb.ToString();
                    }
                    catch (Exception)
                    {
                        str = "-------";
                    }
                }
                else if (Directory.Exists(path))
                {
                    try
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(path);
                        FileAttributes folderAttributes = dirInfo.Attributes;

                        StringBuilder sb = new StringBuilder();
                        Dictionary<FileAttributes, String> dictionary = new Dictionary<FileAttributes, string> {
                { FileAttributes.Archive,"A" },{FileAttributes.Directory,"D" },{FileAttributes.Hidden,"H"},
                { FileAttributes.NotContentIndexed,"I"},{ FileAttributes.ReparsePoint,"L"},{ FileAttributes.ReadOnly,"R"},
                { FileAttributes.System,"S"}};

                        foreach (var atr in dictionary)
                        {
                            if (folderAttributes.HasFlag(atr.Key))
                            {
                                sb.Append(atr.Value);
                            }
                            else
                            {
                                sb.Append("-");
                            }
                        }

                        str = sb.ToString();
                    }
                    catch (Exception)
                    {
                        str = "-------";
                    }
                }
            }
            catch (Exception)
            {
                return str;
            }
            return str;
        }

        /// <summary>
        /// Проверка на доступы
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <param name="fsr">Правило</param>
        /// <returns></returns>
        public static bool Check(string path, FileSystemRights fsr = FileSystemRights.FullControl)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectorySecurity acl = dir.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(SecurityIdentifier));
            for (int x = 0; x < rules.Count; x++)
            {
                FileSystemAccessRule currentRule = (FileSystemAccessRule)rules[x];
                AccessControlType accessType = currentRule.AccessControlType;
                if (accessType == AccessControlType.Deny && (currentRule.FileSystemRights & fsr) == fsr)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Получаем права доступа
        /// </summary>
        /// <param name="path">Путь</param>
        /// <returns></returns>
        public static string GetAllAccess(string path)
        {
            string permissionShort = string.Empty;
            if (Check(path, FileSystemRights.ReadData))
            {
                permissionShort += "R";
            }
            else
            {
                permissionShort += "-";
            }
            if (Check(path, FileSystemRights.WriteData))
            {
                permissionShort += "W";
            }
            else
            {
                permissionShort += "-";
            }
            if (Check(path, FileSystemRights.ExecuteFile))
            {
                permissionShort += "X";
            }
            else
            {
                permissionShort += "-";
            }

            return permissionShort;
        }

    }

    /// <summary>
    /// Класс по формированию строки для ввода
    /// </summary>
    public class ConsoleLine
    {
        Brain brain;
        string postfix = ">";

        public string Postfix
        {
            get { return postfix; }
        }
        public ConsoleLine(Brain b)
        {
            brain = b;
        }
        /// <summary>
        /// Выводим строку комманд
        /// </summary>
        public void Show()
        {
            FormatText.ShowInDown($"\n{brain.State.Path}{Postfix}");
            string input = Console.ReadLine();
            CommandParser.Parser(brain, input);
        }

    }
    /// <summary>
    /// Основной класс для работы приложения
    /// </summary>
    public class Brain
    {
        private State s;
        private Display display;
        private Title title;
        private Info info;

        private int maxTreeString;
        private int maxSizeSubDirectory;
        public int MaxTreeString
        {
            get { return maxTreeString; }
            set { maxTreeString = value; }
        }
        public int MaxSizeSubDirectory
        {
            get { return maxSizeSubDirectory; }
            set { maxSizeSubDirectory = value; }
        }
        /// <summary>
        /// Конструктор где храним текущие данные и заданные параметры
        /// </summary>
        /// <param name="maxSize">Максимальная длина списка дерева файлов на 1 страницу</param>
        /// <param name="sizeSubDirectory">Максимальная длина второго уровня дерева файлов</param>
        /// <param name="lastPath">Последний путь в приложении</param>
        public Brain(int maxSize, int sizeSubDirectory, string lastPath)
        {
            s = new State(lastPath);
            display = new Display(this);
            title = new Title();
            info = new Info();
            MaxTreeString = maxSize;
            MaxSizeSubDirectory = sizeSubDirectory;

            display.Show();
        }

        public Display Display
        {
            get { return display; }
        }
        public State State
        {
            get { return s; }
        }
        public Title Title
        {
            get { return title; }
        }
        public Info Info
        {
            get { return info; }
        }
    }
}
