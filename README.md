Для запуска проекта рекомендуется выполнить этот набор команд в менеджере пакетов VS:

1. Клонировать репозиторий
git clone https://github.com/guranic/BookLibraryWebApp.git

3. Создать файл docker-compose.yml в корневой папке решения:
version: '3.8'

services:
  booklibraryapi:
    build:
      context: ./BookLibraryAPI
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    container_name: booklibraryapi

  booklibraryclient:
    build:
      context: ./BookLibraryClient
      dockerfile: Dockerfile
    ports:
      - "5001:80"
    container_name: booklibraryclient

3. Установить Docker Desktop, выбрать поддержку Windows контейнеров, нажав правой кнопкой мыши по docker в системном трее и
выбрав "Switch to windows containers"
в случае возникновения ошибок во время процесса включения поддержки открыть PowerShell и выполнить там команду:
Enable-WindowsOptionalFeature -Online -FeatureName $("Microsoft-Hyper-V", "Containers") -All

4. Выполнить команду
docker-compose up --build

5. В браузере API и клиент будут доступны соответственно по ссылкам:
http://localhost:5000
http://localhost:5001
