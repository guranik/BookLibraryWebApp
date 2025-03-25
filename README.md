Для запуска проекта рекомендуется выполнить этот набор команд в менеджере пакетов VS:

1. Клонировать репозиторий
git clone https://github.com/guranik/BookLibraryWebApp.git

2. Установить Docker Desktop, выбрать поддержку Windows контейнеров, нажав правой кнопкой мыши по docker в системном трее и
выбрав "Switch to windows containers"
в случае возникновения ошибок во время процесса включения поддержки открыть PowerShell и выполнить там команду:
Enable-WindowsOptionalFeature -Online -FeatureName $("Microsoft-Hyper-V", "Containers") -All

3. Выполнить команду
docker-compose up --build

4. В браузере API и клиент будут доступны соответственно по ссылкам:
http://localhost:5000
http://localhost:5001
