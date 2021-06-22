# Market-API

ASP .NET API para manipulação de produtos com autenticação para usuários

Para rodar o projeto, é necessária uma instância local do banco de dados mySql. É necessário na solução "api-asp-net-4" um arquivo appsettings.json contendo as configurações do banco da seguinte forma:
{ "ConnectionStrings": "server=localhost; port=3306; database=productsdb; user=root; password=; Persist Security Info=False; }
No Visual Studio 2019, use os comandos no console do Gerenciador de Pacotes
dotnet restore dotnet build --no-restore Install-Package Microsoft.EntityFrameworkCore.Tools Add-Migration usuariosprodutos update-database
está pronto! basta compilar o projeto.
