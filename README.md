# PayFlow

PayFlow é um gateway de pagamentos leve, flexível e extensível, desenvolvido em .NET 9, com integração em múltiplos provedores de pagamento e conteinerizado com Docker.

---

## 🏗️ Arquitetura e Componentes

- **ASP.NET Core Web API**: Camada de apresentação e endpoints REST.
- **Entity Framework Core**: ORM para persistência de dados no SQL Server.
- **Design Pattern Factory**: Seleção dinâmica do provedor de pagamento conforme regras de negócio.
- **Injeção de Dependência (DI)**: Utiliza lifetimes `Scoped` e `Transient` para controle de ciclo de vida dos serviços.
- **Docker & Docker Compose**: Conteinerização da aplicação e do banco de dados SQL Server.
- **Swagger**: Documentação e testes interativos da API.

---

## 📦 Estrutura de Pastas
payflow/ │ ├── core/ │   ├── data/ │   │   └── PayFlowDbContext.cs │   ├── interfaces/ │   │   └── IPaymentProvider.cs │   ├── models/ │   │   └── (Modelos de domínio) │   └── services/ │       └── PaymentService.cs │ ├── controllers/ │   ├── PaymentsController.cs │   ├── FastPayController.cs │   └── SecurePayController.cs │ ├── migrations/ │ ├── Dockerfile ├── docker-compose.yml ├── entrypoint.sh ├── appsettings.json └── README.md


---

## 🧩 Componentes Principais

- **Controllers**:  
  - `PaymentsController`: Endpoint principal para processar pagamentos.
  - `FastPayController` e `SecurePayController`: Mocks dos provedores para testes locais.

- **Providers**:  
  - `FastPayProvider` e `SecurePayProvider`: Implementam a interface `IPaymentProvider` e encapsulam a lógica de integração com cada provedor.

- **Factory**:  
  - `PaymentProviderFactory`: Seleciona automaticamente o provedor de acordo com o valor da transação.

- **Service**:  
  - `PaymentService`: Orquestra o fluxo de pagamento, incluindo fallback entre provedores.

- **DbContext**:  
  - `PayFlowDbContext`: Gerencia o acesso ao banco de dados e aplica as migrations.

---

## 🏛️ Design Patterns Utilizados

- **Factory Pattern**:  
  Permite alternar entre diferentes provedores de pagamento sem alterar a lógica principal da aplicação. O `PaymentProviderFactory` recebe todos os provedores registrados e seleciona o correto conforme a regra de negócio.

- **Dependency Injection (DI)**:  
  - **Transient**: Cada requisição para um provider (`FastPayProvider`, `SecurePayProvider`) cria uma nova instância. Isso é útil para serviços stateless e leves.
  - **Scoped**: O `PaymentProviderFactory` e o `PaymentService` são criados uma vez por requisição HTTP, garantindo consistência durante o ciclo de vida da requisição.

---

## 🔄 Ciclo de Vida dos Serviços (Scoped vs Transient)

- **Transient**:  
  - Uma nova instância é criada toda vez que o serviço é solicitado.
  - Usado para provedores de pagamento (`IPaymentProvider`), pois não mantêm estado entre requisições.

- **Scoped**:  
  - Uma instância é criada por requisição HTTP.
  - Usado para `PaymentProviderFactory` e `PaymentService`, garantindo que a mesma instância seja usada durante todo o processamento de uma requisição.

---

## 🚀 Como Executar a Aplicação

### Pré-requisitos
- Docker e Docker Compose instalados
- Git instalado

### Passos

1. **Clone o repositório**
   git clone <url-do-repositorio> cd <nome-da-pasta>

2. Abra o Terminal do Visual Studio ou VS Code, e acessa diretório raiz:
   cd payflow
   
3. **Compile os containers, suba e informe -d para deixar o terminal aberta para os próximos comandos**
   docker-compose up --build -d

4; Execute o comando abaixo para instalar o .Net Entity Framework no container (necessário para executar o comando na sequência gerar o banco e estrutura através da migrations.
   docker run --rm --network payflow_default -v ${PWD}:/app -w /app mcr.microsoft.com/dotnet/sdk:9.0 sh -c 'dotnet tool install --global dotnet-ef && 

5. Execute o comando para gerar o banco de dados SQL Server no outro container através do code first (pode executdo junto com a linha de cima)
     export PATH="$PATH:/root/.dotnet/tools" && dotnet ef database update --startup-project . --project .'

     > No Windows PowerShell, use `${PWD}`.  
    > No Linux/Mac, use `$(pwd)`.

6. **Acesse a API**
- Swagger UI: [http://localhost:8080/swagger](http://localhost:8080/swagger)

5. **Testes**
- Utilize o Swagger para testar os endpoints `/payments`, `/fastpay/payments` e `/securepay/payments`.

---

## ⚙️ Configurações Importantes

- **String de conexão, caso deseja alterar**:  
Altere em `appsettings.json` e no `docker-compose.yml` para garantir acesso ao SQL Server do container.

- **Endpoints dos provedores**:  
Configurados em `appsettings.json` na seção `ProviderUrls`.  
No Docker, utilize o nome do serviço (`payflow`) e a porta correta.

---

## 🛠️ Comandos Úteis

- **Parar containers e remover volumes (reset total):**

- docker-compose down -v

- 
- **Rebuild dos containers:**

- docker-compose up --build -d

- 
---

## 📝 Observações

- O banco de dados é inicializado e migrado automaticamente via comando externo.
- O sistema está pronto para ser estendido com novos provedores, bastando implementar a interface `IPaymentProvider` e registrar no DI.
- O uso de Docker garante portabilidade e facilidade de setup para novos desenvolvedores.

---

## 📄 Exemplos de Payload

**POST /payments**
{ "amount": 120.50, "currency": "BRL" }


**POST /fastpay/payments**
{ "transaction_amount": 90.00, "currency": "BRL", "payer": { "email": "cliente@teste.com" }, "installments": 1, "description": "Compra via FastPay" }


**POST /securepay/payments**
{ "amount_cents": 12050, "currency_code": "BRL", "client_reference": "ORD-20251022" }


---

## 👨‍💻 Desenvolvedor

Delio Darwin - Dev Fullstack


