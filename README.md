# Market-API

## ASP .NET API para manipulação de produtos com autenticação para usuários

### Para rodar o projeto
É necessária uma instância local do banco de dados mySql na porta 3306.
Clone o repositorio em sua máquina local e abra a solução "Usuarios-Produtos-API.sln" no diretório "/Market-API" na IDE do Visual Studio 2019.
Adcione à solução um item chamado "Arquivo de configuração do aplicativo" do Visual C#.
Isso gera um arquivo appsettings.json. Reescreva-o com o seguinte código de configuração do banco de dados:
~~~json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost; port=3306; Database=marketdb; user=root; password=<sua_senha>; Persist Security Info=False;"
  }
}
~~~
Lembre-se de inserir a senha correta no campo "password".
Você também pode personalizar a configuração acima para que se adeque a seu sistema.

Compile o projeto com o IIS Express.

No console do Gerenciador de Pacotes do Visual Studio, digite
```Add-Migration market```
```update-database```

Está pronto! basta compilar o projeto com o IIS Express e a API está funcionando!

### Documentação e teste

Para fins de documentação e teste da API, a aplicação conta com o Swagger, uma interface que mostra informações de todas as requisições possíveis com instruções de como executá-las da forma correta, além de possibilitar estas execuções.
Para acessar a documentaçãom basta abrir o Google Chrome Web Browser na url especificada pelo console do Visual Studio, no qual a API está ativa.

### Como consumir
No primeiro acesso à API, há apenas uma requisição autorizada:
```Get /api/Usuarios/authenticate```.

Informe no corpo da requisição o objeto JSON com as informações de usuário padrão:
~~~json
{ 
  Identificacao: "admin-123",
  Nome: "admin",
  Email: "admin@example.com",
  Senha: "admin123"
}
~~~

A respota do servidor será um token de autenticação JWT Bearer.
Agora, sempre que for fazer uma requisição, insira o campo authorization em seu header com o valor 'Bearer \[espaço\] \[seu-token\]' e você será considerado autorizado.
O token de autenticação expira em uma hora, garantindo a segurança do usuário.

### Por falar em segurança
  
A aplicação possui um sistema de encriptação de senha.
Uma vez enviada ao servidor, é extremamente difícil descobrí-la.
Sua string é encriptada para base 64 várias vezes em sequencia, sempre aumentando de largura, e então tem uma parte cortada, o que dificulta sua decodificação.
Sempre que uma senha é inserida na API, ela é encriptada exatamente da mesma forma.
Ao comparar a senha inserida com as senhas encriptadas presentes no banco de dados, se efetua sua validação.
Além disso, não é difícil pensar em maneiras de aprimorar ainda mais a segurança desse sistema de encriptação a custo de performance da aplicação, codificando-a e cortando-a mais e mais vezes.

### Caminho

Escrevi muito sobre o resultado da aplicação, mas não do meu caminho até chegar nele. Minha maior dor de cabeça foi com a configuração do token JWT Bearer, e meu maior problema em relação a ele foi também o mais inesperado: inverti a ordem dos comandos ```App.useAuthentication();``` e ```App.useAuthorization();``` nas configurações do projeto. Por incrível que pareca, isso impediu o funcionamento do programa e levei horas pra perceber esse "probleminha", depois de reescrever quase o programa todo. Achei divertido configurar meu próprio método de validação de campo quando defini que o valor de um produto não poderia ser negativo. Também gostei de buscar formas de implementar uma identificação para cada usuário e cada produto, usando uma string de 9 caracteres e uma de 36, respectivamente. A identificação de apenas 9 letras foi pensada para que o usuário a memorizasse mais facilmente. Outro problema satisfatório de se resolver foi o da encriptação convincente da senha, que já citei no tópico anterior. Ele me levou a descobrir a existência das FPE(Format Preserving Encryption). Constuir os controladores não foi uma tarefa difícil graças ao Entity Framework, mas tive o desafio de levar a senha do 
