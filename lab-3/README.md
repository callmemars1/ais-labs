# Лабораторная работа № 3 - Контейнеризация приложения.

Было разработано простое приложение на питоне, которое отвечает на HTTP-запросы. Приложение содержит файл index.html с простым содержимым и файл Python, который обрабатывает HTTP-запросы.
Приложение обрабатывает два вида запросов:
* GET `/` - возвращает содержимое файла index.html с данными, которые были записаны на сервер.
* POST `/post` - сохраняет данные из тела запроса в память сервера.

Для запуска приложения необходимо собрать образ `simple-server`:
```bash
docker build -t simple-server .
```

Затем необходимо запустить контейнер из ранее собранного образа `simple-server`:
```bash
docker run -p 5000:5000 simple-server
```

Таким образом наше приложение будет доступно по адресу http://localhost:5000.