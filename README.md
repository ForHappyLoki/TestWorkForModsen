# TestWorkForModsen
Бэкэнд веб-пиложение для обработки http запросов и выдачи ответов

Структура бд: 

Таблица Account хранит в себе приватные данные об аккаунте пользователя, такие как пароль, почта и роль. Привязывается по средствам один к одному к таблице User

Таблица User хранит в себе публичные данные, такие как имя, фамилия, день рождения. Тут же дублируется почта для денормализации и более удобного вытягивания данных

Таблица Event хранит в себе информацию о событиях, например название, время проведения, максимальное количество участников.

Таблицы Event и User связаны связью многие ко многим через таблицу ConnectorEventUser, которая хранит айди ивента, айди юзера и время записи юзера на ивент

Таблица RefreshToken привязана к таблице Account и хранит в себе рефреш токен

Использованная база данных - Microsoft SQL Server. Из этого вытекает потребносить создавать контейнер на платформе windows, поскольку линукс этой субдшкой не поддерживается

На стадии сборки в файле program стоит метод-инициализатор бд, который заполняет каждую таблицу, - кроме рефреш токен, - сотней тестовых записей. После первого создания тестовых данных метод больше не будет генерировать новые данные

Основная документация по api генерируется свагером и находится по адрессу https://localhost:xxxx/swagger/index.html. Единственная проблема - локалхост это адресс не постоянный, в продакшене следует поменять его на постоянный адресс хостинга, или сделать порты статическими в самом контейнере

Пример вызова api - https://localhost:xxxx/EventApi/id/1 - вернет json события под id 1
