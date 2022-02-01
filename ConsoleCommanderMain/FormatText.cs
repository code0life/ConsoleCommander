using System;

/// <summary>
/// Класс для работы с текстом
/// </summary>
public static class FormatText
{
    /// <summary>
    /// Сбрасываем позицию курсора
    /// </summary>
    public static void ResetCursor()
    {
        Console.SetCursorPosition(0, 0);
    }
    /// <summary>
    /// Показываем текст в центре
    /// </summary>
    /// <param name="text">Текст</param>
    /// <param name="centerY">Сдвиг текста по оси Y</param>
    public static void ShowInCenter(string text, int centerY = 0)
    {
        // ResetCursor();
        int centerX = (Console.WindowWidth / 2) - (text.Length / 2);
        Console.SetCursorPosition(centerX, centerY);
        Console.Write(text);
    }
    /// <summary>
    /// Показываем текст в снизу
    /// </summary>
    /// <param name="text">Текст</param>
    /// <param name="posX">Сдвиг текста по оси X</param>
    /// <param name="addPosY">Сдвиг текста по оси X</param>
    /// <param name="isError">Значение, показывающее является ли выводимое сообщение ошибкой.</param>
    /// <param name="isTrues">Значение, показывающее является ли выводимое сообщение утверждением.</param>
    /// <param name="isInfoCommand">Значение, показывающее является ли выводимое сообщение инормацией о команде.</param>
    public static void ShowInDown(string text, int posX = 0, int addPosY = 0, bool isError = false, bool isTrues = false, bool isInfoCommand = false)
    {
        int posY = Console.WindowHeight - 2 + addPosY;
        Console.SetCursorPosition(posX, posY);
        if(!isError && !isTrues && !isInfoCommand)
        {
            Console.Write(text);
        }
        else if (isError)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.ResetColor();
        }
        else if (isTrues)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.ResetColor();
        }
        else if (isInfoCommand)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.ResetColor();
        }
    }
    /// <summary>
    /// Показываем текст вверху
    /// </summary>
    /// <param name="text">Текст</param>
    /// <param name="posX">Сдвиг текста по оси X</param>
    /// <param name="posY">Сдвиг текста по оси Y</param>
    public static void ShowInTop(string text, int posX = 0, int posY = 0)
    {
        posY = Console.WindowHeight - 2;
        Console.SetCursorPosition(posX, posY);
        Console.Write(text);
    }
    /// <summary>
    /// Меняем цвет выводимого текста и его фона
    /// </summary>
    /// <param name="text">Текст</param>
    /// <param name="backColor">Цвет фона</param>
    /// <param name="textColor">Цвет текста</param>
    public static void Color(string text, ConsoleColor backColor = ConsoleColor.White, ConsoleColor textColor = ConsoleColor.Black)
    {
        Console.BackgroundColor = backColor;
        Console.ForegroundColor = textColor;
        Console.Write(text);
        Console.ResetColor();
    }
}
