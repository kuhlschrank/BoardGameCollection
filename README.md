# BoardGameCollection

## Web Application
cd BoardGameCollection/BoardGameCollection.Web
dotnet publish
docker build . -t bgc-web:1.0.0
docker run -d --name bgc-web --restart always -e ConnectionStrings:CollectionContext='<connectionString>' bgc-web:1.0.0

## Crawler Application
cd BoardGameCollection/BoardGameCollection.Crawler
dotnet publish
docker build . -t bgc-crawler:1.0.0
docker run -d --name bgc-crawler --restart always -e ConnectionStrings:CollectionContext='<connectionString>' bgc-crawler:1.0.0
