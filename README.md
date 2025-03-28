# TestWorkForModsen

Инструкиция по запуску проекта в докере: 
1. Убедитесь, что у вас установлен Docker.
2. В консоли разработчика выполните docker-compose build --no-cache и затем docker-compose up
3. После запуска композиции перейдите на страницу http://localhost:8080 (Или https://localhost:8081). Если все сделано верно, должна открыться страница с дальнейшими инструкциями
   
=======
Бэкэнд веб-пиложение для обработки http запросов и выдачи ответов

Структура бд: 

Таблица Account хранит в себе приватные данные об аккаунте пользователя, такие как пароль, почта и роль. Привязывается по средствам один к одному к таблице User

Таблица User хранит в себе публичные данные, такие как имя, фамилия, день рождения. Тут же дублируется почта для денормализации и более удобного вытягивания данных

Таблица Event хранит в себе информацию о событиях, например название, время проведения, максимальное количество участников.

Таблицы Event и User связаны связью многие ко многим через таблицу ConnectorEventUser, которая хранит айди ивента, айди юзера и время записи юзера на ивент

Таблица RefreshToken привязана к таблице Account и хранит в себе рефреш токен

Использованная база данных - postgresql 

На стадии сборки в файле program стоит метод-инициализатор бд, который заполняет каждую таблицу, - кроме рефреш токен, - сотней тестовых записей. После первого создания тестовых данных метод больше не будет генерировать новые данные

Основная документация по api генерируется свагером и находится по адрессу https://localhost:8081/swagger/index.html. 

Пример вызова api - http://localhost:8080/api/AccountApi/id/1 (Или https://localhost:8081/api/AccountApi/id/1) - вернет json аккаунта под id 1, но только если мы авторизовались
https://localhost:8081/api/accountapi/paged?pageNumber=1&pageSize=25 - вернет пагинацию данных первых 25 записей. Для проверки валидации можно запросить отрицательное количество страниц или записей
