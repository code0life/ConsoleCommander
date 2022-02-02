# ConsoleCommander #

https://img.shields.io/badge/style-for--the--badge-green?logo=appveyor&style=for-the-badge

### Общее описание ###
Консольный файловый менеджер начального уровня, который покрывает минимальный набор функционала по работе с файлами. Делался для курса ***Введение в C#***

В настройках Settings.setting заданы дефолтные значения:
MaxHeightTree - максимальная высота отображаемого дерева на 1 странице
MaxVisibleSubDirectiry - максимальная длина отображамемого 2 уронвня дерева

При первом открытии приложения, юудет создан файл в корне каталога программы save.txt, где будет храниться последний путь в котором мы работали в программе. 
При первом заходе мы попадём в папку пользователя.

Решение состоит из нескольких файлов:
ConsoleCommanderMain - осуществляет запуск приложения, считывания параметров для отобраения дерева из конфига и путь, с которым в последний раз работали в программе. А также его сохранение в файл.
FormatText - содержит в основном некотоыре функции для работы с текстом, такие как центрование или раскрашивание.
ClassLibrary - содержит классы, отвечающие за отрисовку консольного коммандера и его перерисовку. Также имеются классы для хранения некоторых загруженных ранее переменных.
CommandParser - содержит функции, для обработки комманд, введённых в консоль. Список поддерживаемых комманд представлен чуть ниже. 

Команды:
cd <Название_каталога> - команда для изменения рабочего каталога. Принимает как локальные, так и глобальные пути.
bk - команда для перехода в родительский каталог рабочего каталога. (Переход назад)
ls p<Номер_страницы> - команда для изменения страницы в отображаемом каталоге.
cp <A> <B> - команда для копирования каталога/файла A, в каталог/файл B. Принимает как локальные, так и глобальные пути.
rm <Файл/Каталог> - команда для удаления каталога/файла. Принимает как локальные, так и глобальные пути.
info <Файл/Каталог/Команда> - команда для вывода информации о файле/каталоге/комманде. Если параметр не указан выведет информацию о текущем рабочем каталоге. Принимает как локальные, так и глобальные пути.
exit - команда для выхода из программы
  

![image](https://user-images.githubusercontent.com/34949894/152061953-782943c6-891e-44f5-83f4-bccace859497.png)
