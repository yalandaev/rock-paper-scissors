# rock-paper-scissors
Игра "Камень-ножницы-бумага", реализованная как веб-приложение ASP.NET MVC. Игроки могут регистрироваться, создавать игровые комнаты, общаться в чате, и, собственно играть в "Камень-ножницы-бумага". Игра происходит в режиме реального времени, все действия игроков совершаются без обновления страницы (при помощи SignalR). 
Была реализована в ходе выполнения тестового задания при прохождении собеседования в довольно сжатые сроки, основной упор делал на функциональность. Дизайн сделал как смог с использованеим Twitter Bootstrap (я не дизайнер).

Краткий экскурс по функциональности:

При входе мы можем ввести логин\пароль (Player_1 123, Player_2 123 и т.д.):
![alt tag](https://github.com/yalandaev/rock-paper-scissors/blob/master/Images/1.png)

Либо зарегистрироваться
![alt tag](https://github.com/yalandaev/rock-paper-scissors/blob/master/Images/2.png)
Данные пользователей хранятся в «фейковом» репозитории, в оперативной памяти. При перезапуске приложения будут удалены.

Главное окно лобби разделено не несколько частей  - чат, список игроков и список игр.
![alt tag](https://github.com/yalandaev/rock-paper-scissors/blob/master/Images/3.png)
![alt tag](https://github.com/yalandaev/rock-paper-scissors/blob/master/Images/4.png)

Список игр динамически обновляется (сами игры, и колонки «Игроки» и «Состояние»)
![alt tag](https://github.com/yalandaev/rock-paper-scissors/blob/master/Images/5.png)

Создать игру:
![alt tag](https://github.com/yalandaev/rock-paper-scissors/blob/master/Images/6.png)

Другой игрок может войти в игру кликнув на неё мышью в списке игр.
![alt tag](https://github.com/yalandaev/rock-paper-scissors/blob/master/Images/7.png)

Окно «Боя»: 
![alt tag](https://github.com/yalandaev/rock-paper-scissors/blob/master/Images/8.png)

Ведётся лог игры:
![alt tag](https://github.com/yalandaev/rock-paper-scissors/blob/master/Images/9.png)

И вот тут тоже:
![alt tag](https://github.com/yalandaev/rock-paper-scissors/blob/master/Images/10.png)
