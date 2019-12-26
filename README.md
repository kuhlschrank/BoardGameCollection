# BoardGameCollection

## Web Application
- cd BoardGameCollection/BoardGameCollection.Web
- dotnet publish
- docker build . -t bgc-web:1.0.0
- docker run -d --name bgc-web --restart always -p 80:80 -e ConnectionStrings:CollectionContext='myConnection' bgc-web:1.0.0

## Crawler Application
- cd BoardGameCollection/BoardGameCollection.Crawler
- dotnet publish
- docker build . -t bgc-crawler:1.0.0
- docker run -d --name bgc-crawler --restart always -e ConnectionStrings:CollectionContext='myConnection' bgc-crawler:1.0.0
