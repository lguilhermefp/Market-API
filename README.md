![Alt text](screenshot-api.png?raw=true "Title")

# Market-API

## API ASP .NET para manipulação de produtos com autenticação para usuários
#### Parte de Processo Seletivo da WLS Soluções

### Para rodar o projeto

_As recomendações aqui presentes foram pensadas para o __Windows__._

É necessária uma instância local do banco de dados __mySql__ na porta 3306.

Use o comando ```git clone``` no __Windows Powershell__ no diretório desejado para obter o repositorio em sua máquina local e abra a solução _Usuarios-Produtos-API.sln_ presente no diretório _/Market-API_ na IDE do Visual Studio 2019.

Adcione à solução um item chamado _Arquivo de configuração do aplicativo_ do __Visual C#__.
Isso gera um arquivo _appsettings.json_. Reescreva-o com o seguinte código de configuração do banco de dados __mySql__:
~~~json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost; port=3306; Database=marketdb; user=root; password=<sua_senha>; Persist Security Info=False;"
  }
}
~~~
Lembre-se de inserir a senha correta no campo _password_.
Você também pode personalizar a configuração acima para que se adeque ao seu sistema.

Antes de proseguir, instale todas as dependências descritas no arquivo _Usuarios-Produtos-API.csproj_

Compile o projeto com o __IIS Express__.

No __console do Gerenciador de Pacotes__ do __Visual Studio__, digite
```Add-Migration market```
e
```update-database```

_Está pronto._ Basta compilar o projeto com o __IIS Express__ novamente e a API estará funcionando com persistência de dados no database _marketdb_ da instância do banco de dados __mySql__

### Documentação e teste

Para fins de documentação e teste da API, a aplicação conta com o __Swagger__, uma interface que mostra informações de todas as requisições possíveis com instruções de como executá-las da forma correta, além de possibilitar estas execuções.
Para acessar a documentação da API basta abrir o __Chrome Web Browser__ na porta especificada para consumo da API pelo console do __Visual Studio__ no url _https://localhost:PORT/swagger_.

### Como consumir

No primeiro acesso à API, há apenas uma requisição autorizada:
```Post /api/Usuarios/authenticate```.

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
Sua string é encriptada para base 64 várias vezes em sequência, sempre aumentando de largura, e então tem uma parte cortada, o que dificulta sua decodificação.
Sempre que uma senha é inserida na API, ela é encriptada exatamente da mesma forma.
Ao comparar a senha inserida com as senhas encriptadas presentes no banco de dados, efetua-se sua validação durante o uso.
Além disso, não é difícil pensar em maneiras de aprimorar ainda mais a segurança desse sistema de encriptação a custo de performance da aplicação, codificando-a e cortando-a mais e mais vezes.

### Caminho

Escrevi muito sobre o resultado da aplicação, mas não do meu caminho até chegar nele.

Achei divertido configurar meu próprio método de validação de campo quando defini que o valor de um produto não poderia ser negativo.
Também gostei de buscar formas de implementar uma identificação para cada usuário e cada produto, usando uma string de 9 caracteres e uma de 36, respectivamente.
A identificação de apenas 9 letras foi pensada para que o usuário a memorizasse mais facilmente.

Minha maior dor de cabeça foi com a configuração do token JWT Bearer, e meu maior problema em relação a ele foi também o mais inesperado: inverti a ordem dos comandos ```App.useAuthentication();``` e ```App.useAuthorization();``` nas configurações do projeto.

Por incrível que pareça, isso impediu o funcionamento do programa e levei horas pra perceber esse "probleminha", depois de reescrever quase o programa todo.

Constuir os controladores não foi uma tarefa difícil graças ao Entity Framework, mas tive o desafio de descobrir como levar informações do usuário do banco de dados até o função de autenticação do Bearer, o que não seria tarefa tão difícil não fosse eu ter que resolver paralelamente a confusão com os comandos de configuração que acabei de citar.

Outro problema satisfatório de se resolver foi o da encriptação convincente da senha, que já citei no tópico anterior.
Ele me levou a descobrir a existência das FPE(Format Preserving Encryption), cuja existência me surpreendeu muito.

Para garantir que eu era capaz de fazer o programa funcionar completamente como eu desejava, fiz aplicações teste antes, uma vez que estou mais habituado com APIs criadas com __Javascript__, até garantir segurança com todas as funcionalidades que o desafio propunha.

Gosto de __C#__ por ser uma linguagem muito bem otimizada e muito completa, superando inclusive Java, linguagem de POO que usei muito. Gostei muito da forma prática de documentar a aplicação com __Swagger__, que é extremamente fácil de configurar.




