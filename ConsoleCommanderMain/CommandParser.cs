using ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Класс для работы со строкой
/// </summary>
public static class CommandParser
{
    /// <summary>
    /// Разбираем введённую строку.
    /// </summary>
    /// <param name="b">Объект класса, хранящий текущие данные</param>
    /// <param name="input">Передаём введённую строку</param>
    public static void Parser(Brain b, string input)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
        {
            b.Info.AddError("An empty string entered.");
        }
        else
        {
            int res;
            bool isInt = Int32.TryParse(input, out res);
            if (isInt)
            {
                b.Info.AddError("The command cannot start with a number.");
            }
            else
            {

                string cmd = GetCommand(input);
                if (input == cmd)
                {
                    if (!IsNotParametersCommand(input))
                    {
                        if (IsTryCommand(cmd))
                        {
                            b.Info.AddError($"Icorrect format command: {cmd}");
                        }
                        else
                        {
                            b.Info.AddError($"Unknown command: {cmd}.");
                        }
                    }
                    else
                    {
                        Action(b, input);
                    }
                }
                else
                {
                    if (IsTryCommand(cmd))
                    {
                        Action(b, input);
                    }
                    else
                    {
                        b.Info.AddError($"Unknown command: {cmd}.");
                    }
                }

            }
        }
    }
    /// <summary>
    /// Достаём из строки команду.
    /// </summary>
    /// <param name="input">Текст строки</param>
    /// <returns>Команда из строки</returns>
    public static string GetCommand(string input)
    {
        if (IsNotParametersCommand(input))
        {
            return input;
        }
        int end = input.IndexOf(" ", 0);
        if (end != -1)
        {
            return input.Substring(0, end).Trim();
        }
        return input;
    }
    /// <summary>
    /// Убираем команду из строки.
    /// </summary>
    /// <param name="input">Строка</param>
    /// <returns></returns>
    public static string GetWithoutCommand(string input)
    {
        int end = input.IndexOf(" ") + 1;
        if(end == 0)
        {
            return "";
        }
        string options = input.Substring(end);
        return options;
    }
    /// <summary>
    /// Определяем, содержит ли строка полный путь до файла.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsFullPath(string input)
    {
        if (input.Contains(":"))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Если команда содержится в списке реализуемых команд.
    /// </summary>
    /// <param name="input">Команда</param>
    /// <returns></returns>
    public static bool IsTryCommand(string input)
    {
        List<string> listCommand = GetListCommand();
        foreach (string command in listCommand)
        {
            if (command == input)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Проверка не является ли команда, командой без параметров
    /// </summary>
    /// <param name="input">Текст</param>
    /// <returns></returns>
    public static bool IsNotParametersCommand(string input)
    {
        List<string> listCommand = GetListCommand(isShort: true);
        foreach (string command in listCommand)
        {
            if (command == input)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Определяем что делать с введённой командой.
    /// </summary>
    /// <param name="b">Объект класса, хранящий текущие данные</param>
    /// <param name="input">Строка</param>
    public static void Action(Brain b, string input)
    {
        string cmd = GetCommand(input);
        string options = GetWithoutCommand(input);

        b.Display.Body.CurPage = 1;

        switch (cmd)
        {
            case "cd":
                SetPath(b, options);
                break;
            case "bk":
                ParentDirectory(b);
                break;
            case "ls":
                SetPage(b, options);
                break;
            case "cp":
                Copy(b, options);
                break;
            case "rm":
                Delete(b, options);
                break;
            case "info":
                Information(b, options);
                break;
            case "exit":
                Environment.Exit(0);
                break;
        }

    }
    /// <summary>
    /// Устанавливаем новый путь.
    /// </summary>
    /// <param name="b">Объект класса, хранящий текущие данные</param>
    /// <param name="input">Параметры команды</param>
    public static void SetPath(Brain b, string input)
    {
        string path = b.State.Path;
        string curPath = Path.Combine(path, input);
        if (IsFullPath(input))
        {
            curPath = input;
        }
        if (File.Exists(curPath))
        {
            b.Info.AddError($"'{curPath}' is a file. Can't navigate to file.");
        }
        else if (Directory.Exists(curPath))
        {
            //b.State.Path = curPath.ToLowerInvariant(); //Потому что я не знаю как послать свой кривонаписанный путь, и у директории потом достать реальный путь
            //(со всеми большими и маленькими буквами), а не мой кривой, котоырй я могу в консоле прописать
            b.State.SavePath(curPath.ToLowerInvariant());
        }
        else
        {
            b.Info.AddError($"'{curPath}' does not exist.");
        }
    }
    /// <summary>
    /// Переходим в родительский каталог
    /// </summary>
    /// <param name="b">Объект класса, хранящий текущие данные</param>
    public static void ParentDirectory(Brain b)
    {
        string path = b.State.Path;
        string new_path = Path.GetDirectoryName(path);
        if (new_path == null)
        {
            b.Info.AddError($"You cannot change to the parent directory. You are in the root: {path}");
        }
        else
        {
            b.State.SavePath(new_path);
        }
    }
    /// <summary>
    /// Меняем отображаемую страницу в отображаемом дереве файлов
    /// </summary>
    /// <param name="b">Объект класса, хранящий текущие данные</param>
    /// <param name="input">Параметры команды</param>
    public static void SetPage(Brain b, string input)
    {
        int end = input.IndexOf("p") + 1;
        string options = input.Substring(end);
        string only_num = options.Trim();

        int white_space = only_num.IndexOf(" ", 0);
        if (white_space != -1)
        {
            int new_end = only_num.IndexOf(" ", 0) + 1;
            only_num = only_num.Substring(0, new_end);
        }

        if (int.TryParse(only_num, out int index))
        {
            int maxPages = b.Display.Body.MaxPages;
            if (maxPages > 0)
            {
                if (index != 0 && index <= maxPages)
                {
                    b.Display.Body.CurPage = index;
                }
                else
                {
                    b.Info.AddError($"There are no {index} pages in this catalog. Must be selected from a range [1..{maxPages}]");
                }
            }
            else
            {
                b.Info.AddError($"There are no pages in this directory");
            }
        }
        else
        {
            b.Info.AddError($"'{only_num}' is not a number");
        }
    }
    /// <summary>
    /// Копируем директорию\файл
    /// </summary>
    /// <param name="b">Объект класса, хранящий текущие данные</param>
    /// <param name="input">Параметры команды</param>
    public static void Copy(Brain b, string input)
    {
        string path = b.State.Path;
        List<int> ws = new List<int>();
        List<string> paths = new List<string>();

        input = input.Trim();

        int index = input.IndexOf(" ");
        while (index != -1)
        {
            ws.Add(index);
            index = input.IndexOf(" ", index + 1);
        }

        string stringA = "";
        string stringB = "";

        foreach (int id in ws)
        {
            paths.Clear();
            stringA = input.Substring(0, id);
            stringB = input.Substring(id + 1);
            string curPathA = Path.Combine(path, stringA);
            if (IsFullPath(stringA))
            {
                curPathA = stringA;
            }
            string curPathB = Path.Combine(path, stringB);
            if (IsFullPath(stringB))
            {
                curPathB = stringB;
            }
            if (File.Exists(curPathA) || Directory.Exists(curPathA))
            {
                paths.Add(curPathA);
                paths.Add(curPathB);
                break;
            }
        }

        if (paths.Count >= 2)
        {
            if (File.Exists(paths[0]))
            {
                try
                {
                    File.Copy(paths[0], paths[1]);
                    b.Info.AddTrue($"Copy {paths[0]} to {paths[1]}");
                }
                catch (Exception)
                {
                    b.Info.AddError($"Invalid command parameters 'cp' <{(paths.Count == 1 ? paths[0] : "INCORRECT")}> <{(paths.Count == 2 ? paths[1] : "INCORRECT")}>");
                    throw;
                }
            }
            else if (Directory.Exists(paths[0]))
            {
                try
                {
                    //Создать идентичную структуру папок
                    foreach (string dirPath in Directory.GetDirectories(paths[0], "*",
                        SearchOption.AllDirectories))
                    {
                        Directory.CreateDirectory(dirPath.Replace(paths[0], paths[1]));
                    }

                    //Копировать все файлы и перезаписать файлы с идентичным именем
                    foreach (string newPath in Directory.GetFiles(paths[0], "*.*",
                        SearchOption.AllDirectories))
                    {
                        File.Copy(newPath, newPath.Replace(paths[0], paths[1]), true);
                        b.Info.AddTrue($"Copy {paths[0]} to {paths[1]}");
                    }
                }
                catch (Exception)
                {
                    b.Info.AddError($"Invalid command parameters 'cp' <{(paths.Count == 1 ? paths[0] : "INCORRECT")}> <{(paths.Count == 2 ? paths[1] : "INCORRECT")}>");
                    throw;
                }
            }
            else
            {
                b.Info.AddError($"Invalid command parameters 'cp' <{(paths.Count == 1 ? paths[0] : "INCORRECT")}> <{(paths.Count == 2 ? paths[1] : "INCORRECT")}>");
            }
        }
        else
        {
            if (paths.Count == 0)
            {
                string tmpA = Path.Combine(path, input);
                if (IsFullPath(input))
                {
                    tmpA = input;
                }
                b.Info.AddError($"Invalid command parameters 'cp'");
            }
            else
            {
                b.Info.AddError($"Invalid command parameters 'cp' <{(paths.Count == 1 ? paths[0] : "INCORRECT")}> <{(paths.Count == 2 ? paths[1] : "INCORRECT")}>");
            }
        }
    }

    /// <summary>
    /// Удаляем директорию/файл
    /// </summary>
    /// <param name="b">Объект класса, хранящий текущие данные</param>
    /// <param name="input">Параметры команды</param>
    public static void Delete(Brain b, string input)
    {
        string path = b.State.Path;

        input = input.Trim();

        string curPath = Path.Combine(path, input);
        if (IsFullPath(input))
        {
            curPath = input;
        }

        try
        {
            if (File.Exists(curPath))
            {
                File.Delete(curPath);
                b.Info.AddTrue($"Delete file {curPath}");
            }
            else if (Directory.Exists(curPath))
            {
                Directory.Delete(curPath, true);
                b.Info.AddTrue($"Delete directory {curPath}");
            }
        }
        catch (Exception)
        {
            b.Info.AddError($"Invalid command parameters 'rm' <INCORRECT>");
        }
    }

    /// <summary>
    /// Получаем информацию о файле/директории/команде
    /// </summary>
    /// <param name="b">Объект класса, хранящий текущие данные</param>
    /// <param name="input">Параметры команды</param>
    public static void Information(Brain b, string input)
    {
        string path = b.State.Path;

        input = input.Trim();

        if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
        {
            b.Info.DirInfo(path);
        }
        else
        {
            if (IsTryCommand(input))
            {
                b.Info.CommandInfo(GetInfOCommands(input));
            }
            else
            {
                string curPath = Path.Combine(path, input);
                if (IsFullPath(input))
                {
                    curPath = input;
                }

                if (File.Exists(curPath))
                {
                    b.Info.FileInfo(curPath);
                }else if (Directory.Exists(curPath))
                {
                    b.Info.DirInfo(curPath);
                }
                else
                {
                    b.Info.AddError($"Invalid command parameters 'info' <file/directory/command>");
                }
            }
        }

    }

    /// <summary>
    /// Получаем список возможных комманд
    /// </summary>
    /// <param name="isShort">Истина, если нужен список команд без парамтеров</param>
    /// <returns></returns>
    public static List<string> GetListCommand(bool isShort = false)
    {
        if (isShort)
        {
            return new List<string> { "bk", "info", "exit" };
        }

        return new List<string> { "cd", "bk", "ls", "cp", "rm", "info", "exit" };
    }
    /// <summary>
    /// Получаем описание команды
    /// </summary>
    /// <param name="com">Команда</param>
    /// <returns></returns>
    public static string GetInfOCommands(string com)
    {
        string info = "ERROR! Description not set.";
        switch (com)
        {
            case "cd":
                info = "cd <directory_name> - is a command to change the working directory. Accepts both local and global paths.";
                break;
            case "bk":
                info = "bk - is a command to change to the parent directory of the working directory. (Jump back).";
                break;
            case "ls":
                info = "ls p<page_number> - is a command to change the page in the displayed directory.";
                break;
            case "cp":
                info = "cp <A> <B> - command to copy directory/file (A) to directory/file (B). Accepts both local and global paths.";
                break;
            case "rm":
                info = "rm <file/directory> - command to remove a directory/file. Accepts both local and global paths.";
                break;
            case "info":
                info = "info <file/directory/command> - command to display information about a file/directory/command. If the parameter is not specified, it will display information about the current working directory. Accepts both local and global paths.";
                break;
            case "exit":
                info = "exit - command to exit the program.";
                break;
        }

        return info;
    }
}