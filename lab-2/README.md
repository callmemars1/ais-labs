Запуск для теста на локальной машине.

## Запуск сервера

Собираем докер-образ командой:
```shell
docker build -t tcpserver:latest ./server
```

Запускаем контейнер из собранного образа командой:
```shell
docker run -d \
  --name=tcpsrv \
  -p 5000:5000 \
  -v ./logs:/app/logs \
  tcpserver:latest python server.py -p 5000 -t 100 -lp /app/logs
```

Протестировать сервер можно утилитой netcat, отправив запрос из командной строки.
```shell
echo "This is a test" | nc localhost 5000
```
Далее проверяем файл с логами и видим что все работает.

## Client

Собираем докер-образ командой:
```shell
docker build -t tcpclient:latest ./client
```

Запускаем контейнер из собранного образа командой:
```shell
docker run --rm \
  -v ./data/test-1.txt:/app/file.txt \
  tcpclient:latest python client.py -f /app/file.txt -a host.docker.internal -p 5000
```