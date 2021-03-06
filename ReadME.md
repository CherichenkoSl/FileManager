Данное приложение представляет собой консольный файловый менеджер начального уровня. В приложении реализована функция открытия последнего активного каталога, если программа запускалась раньше у пользователя. Возможности файлового менеджера: 

1.Просмотр каталогов и файлов по указанному пользователем пути;

2.Возможность копирования файлов и каталогов;

3.Возможность удаления файлов и каталогов(удаление и копирование каталогов реализовано с помощью рекурсии);

4.Просмотр информации о файле или каталоге.

Основная структура файлового менеджера:

![image](https://user-images.githubusercontent.com/98385677/161363952-c59eed3e-1c84-4509-8611-3620e42d8569.png)

1.Область, где выводится дерево каталогов 2-х уровней, находящихся в директории, которую указал пользователь;

2.Область, где выводится список файлов и основная информация о них, находящихся в этой же директории. На нижних границах 1 и 2 области указано сколько всего есть страниц каталогов и файлов;

3.Область, где выводится основная информация о файле\каталоге после указания соответствующей команды;

4.Область командной строки, куда пользователь вводит новые команды.

Список всех доступных команд:

![image](https://user-images.githubusercontent.com/98385677/161363958-2bd5b852-6ed5-49e0-a3c3-fcb7f0cd2f89.png)

Также список команд можно вывести на консоль, написав команду help. После выполнения каждой команды на экран выводится информация об успешном выполнении.

При возникновении нестандартных ситуаций приложение обрабатывает всевозможные ошибки и не дает “упасть”. При ошибках, сообщение ошибки выводится на экран пользователю и дополнительно записывается в файл “D:\\FileManager\\errors\\all\_exceptions.txt ” вместе со временем, в момент которого она случилась. В файловом менеджере предусмотрена система пейджинга. В настройках приложения пользователь может выбрать количество выводимых каталогов 1-го уровня(через команду ps), в то время как количество выводимых файлов на страницу задано приложением и пользователю этот параметр изменить нельзя.

![image](https://user-images.githubusercontent.com/98385677/161363962-0121e450-6bb0-4cc2-ab8b-a244d50131c3.png)

Также пользователь может изменять свойство WidthOfConsole, отвечающее за ширину рамок отрисовываемого приложения. Это введено для того, если пользователь указывает слишком длинное название директории, она смогла вывестись на экран корректно.

![image](https://user-images.githubusercontent.com/98385677/161363974-c401abda-a134-4269-a4ee-219e034a8471.png)

![image](https://user-images.githubusercontent.com/98385677/161363980-d16cf820-8ba2-4bfd-93f6-ef55008dc5a5.png)

Последние три свойства служат для сохранения последнего активного каталога при закрытии приложения.

**Алгоритм работы приложения:** при первом открытии приложения пользователь видит перед собой пустую консоль (при повторных запусках сначала открывается последний активных каталог). При вводе команды приложение считывает это как одну строку и отправляет сначала в парсер, где эта строка разбивается на массив строк, которые потом участвуют в выполнении тех или иных методов. После парсера массив строк отправляется в метод SpaceException, который обрабатывает ситуации, связанные с пробелами. Например: пользователь указал путь “C:\Program Files”. Парсер просто разделяет строку по пробелам. Так как в пути есть пробел, то парсер создаст отдельно строки “C:\Program” и “Files”. SpaceException не допускает этого. После уже отформатированный массив строк отправляется в метод SwithCommand, в котором в зависимости от ключевых слов вызывается тот или иной метод на отрисовку, удаления, копирования или показа информации о файле/каталоге. При разработке данного приложения было смоделировано множество нестандартных ситуаций. Все полученные результаты улучшили “стойкость” приложение перед ошибками.

Для улучшения читабельности в коде приложения добавлено множество комментариев. 
