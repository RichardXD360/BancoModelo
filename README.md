**Objetivo do Projeto**

Criar um modelo simples de sistema bancário onde seja possível:
Cadastrar usuários
Validar dados
Executar transações simuladas (ex.: depósitos, saques, transferências)
Todas as operações retornam status codes HTTP corretos, permitindo fácil integração com outros sistemas e ilustrando boas práticas de APIs.

**O projeto segue um padrão de camadas para facilitar manutenção e escalabilidade:**

BancoModelo/
│
├─ Application      → Camada de Interfaces
├─ Data.Domain      → Serviços para manipulação de dados
├─ Service          → Camada de regras de negócio e orquestração
├─ Shared           → Classes compartilhadas/utilitários
└─ BancoModelo.sln  → Solução principal

**Tecnologias Utilizadas**
C# .NET (API RESTful)
SQL Server / SQLite (persistência e manipulação de dados)
Git/GitHub para versionamento


**Endpoints de Exemplo**
Método	Endpoint	Descrição	Status Codes
POST	/api/CriarUsuario	Cria um novo usuário	201 Created, 400 Bad Request
POST	/api/VerificarUsuario	Retorna dados de um usuário	200 OK, 404 Not Found
POST	/api/EfetuarTransacao	Realiza uma transação	200 OK, 400 Bad Request, 500 Internal Server Error

**Projeto desenvolvido para estudo e demonstração de habilidades.
Licença livre para uso pessoal ou acadêmico.**
