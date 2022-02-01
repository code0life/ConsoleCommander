using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace ConsoleCommanderMain
{
    /// <summary>
    /// Основнйо класс программы
    /// </summary>
    class ConsoleCommanderMain
    {
        static void Main(string[] args)
        {
            int sizeTree = GetHeightTreeSetting();
            int sizeSubDirectory = GetVisibleSubDirectorySetting();
            string lastPath = GetLastPath();

            Start(sizeTree, sizeSubDirectory, lastPath);
        }
        static void Start(int sizeTree, int sizeSubDirectory, string lastPath)
        {
            if (sizeTree != 0 && sizeTree <= 25)
            {
                Brain b = new Brain(sizeTree, sizeSubDirectory, lastPath);
                //b.Display.Show();
            }
        }
        /// <summary>
        /// Получаем значение максимальной длины дерева файлов из настроек проекта.
        /// </summary>
        /// <returns></returns>
        static int GetHeightTreeSetting()
        {
            try
            {
                return Properties.Settings.Default.MaxHeightTree;
            }
            catch
            {
                Console.WriteLine("ERROR. FIELD MaxHeightTree NOT FOUND or FILE SETTINGS NOT FOUND");
                return 0;
            }
        }
        /// <summary>
        /// Получаем значение максимальной отображаемой длины субкаталога n уровня.
        /// </summary>
        /// <returns></returns>
        static int GetVisibleSubDirectorySetting()
        {
            try
            {
                return Properties.Settings.Default.MaxVisibleSubDirectiry;
            }
            catch
            {
                Console.WriteLine("ERROR. FIELD MaxVisibleSubDirectiry NOT FOUND or FILE SETTINGS NOT FOUND");
                return 0;
            }
        }
        /// <summary>
        /// Сохраняем путь в файл
        /// </summary>
        /// <param name="path">Путь</param>
        public static void SavePath(string path)
        {
            string curPath = Path.Combine(Directory.GetCurrentDirectory(), "save.txt");

            // This text is added only once to the file.
            if (File.Exists(curPath))
            {
                // Create a file to write to.
                File.WriteAllText(curPath, path);
            }
            else
            {
                File.Create(curPath);
            }

        }
        /// <summary>
        /// Загружаем последний путь из файла, если он есть
        /// </summary>
        /// <returns></returns>
        static string GetLastPath()
        {
            string curPath = Path.Combine(Directory.GetCurrentDirectory(), "save.txt");
            string str = "";
            if (File.Exists(curPath))
            {
                str = File.ReadAllText(curPath);
            }
            else
            {
                File.Create(curPath);
            }

            return str;
        }
    }
}
