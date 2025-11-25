## 🏦 AcademyIO – Plataforma de Educação Online com DevOps

Bem-vindo à nova era do **AcademyIO**, um projeto evoluído no **MBA DevXpert Full Stack .NET** com foco em arquitetura moderna, DevOps, CI/CD e orquestração de contêineres.

O AcademyIO é uma plataforma de ensino digital distribuída, agora totalmente containerizada com Docker, orquestrada com Kubernetes e com um pipeline de integração e entrega contínua (CI/CD) automatizado com GitHub Actions.

---

### 🚀 Sobre o Projeto

Construído com uma abordagem baseada em microsserviços e princípios de **Domain-Driven Design (DDD)**, o AcademyIO foi refatorado para operar em um ecossistema DevOps completo, garantindo escalabilidade, resiliência e automação.

- 🐳 **Containerização Completa:** Todos os microsserviços, o banco de dados e o message bus rodam em contêineres Docker.
- ⚙️ **Orquestração com Kubernetes:** Manifestos declarativos para deploy, serviços, configurações e segredos.
- 🔄 **CI/CD com GitHub Actions:** Build, testes, lint e publicação de imagens no Docker Hub automatizados.
- 👁️ **Observabilidade:** Health checks configurados para monitoramento de liveness e readiness no Kubernetes.
- 💪 **Resiliência:** Políticas de Retry e Circuit Breaker implementadas com Polly nas chamadas entre serviços.

---

### 🛠️ Tecnologias Utilizadas

**Back-End:**

- C# 12 e .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- Microsserviços com comunicação via REST e Mensageria (RabbitMQ)
- Polly (para resiliência)

**DevOps e Infraestrutura:**

- Docker (multi-stage builds)
- Docker Compose
- Kubernetes (Kind, Minikube, ou qualquer cluster)
- GitHub Actions (CI/CD)
- SQL Server e RabbitMQ (rodando em contêineres)

---

### ▶️ Como Executar o Projeto

Esta seção descreve como executar o ambiente completo localmente usando Docker Compose e como fazer o deploy em um cluster Kubernetes.

#### 📌 Pré-requisitos

- **Docker Desktop:** Essencial para rodar os contêineres e o ambiente Docker Compose.
- **kubectl:** Necessário para interagir com um cluster Kubernetes.
- **Um cluster Kubernetes (opcional):** Se desejar fazer o deploy, pode usar [Kind](https://kind.sigs.k8s.io/) ou [Minikube](https://minikube.sigs.k8s.io/docs/start/) para um cluster local.

---

### 🐳 Ambiente de Desenvolvimento com Docker Compose

Esta é a forma recomendada para executar todo o ecossistema de microsserviços em sua máquina local.

1.  **(Opcional, mas recomendado) Crie um arquivo `.env` na pasta `src`**:
    Para evitar senhas hard-coded, crie um arquivo chamado `.env` dentro da pasta `src` e adicione a seguinte variável. O `docker-compose.yml` está configurado para usá-la.

    ```env
    DB_PASSWORD=YourStrong!Passw0rd
    ```

2.  **Navegue até a pasta `src`**:

    ```bash
    cd src
    ```

3.  **Suba os contêineres**:
    Use o comando abaixo para construir as imagens Docker de cada microsserviço e iniciar todos os contêineres em segundo plano.
    ```bash
    docker compose up --build -d
    ```
    _Nota: Dependendo da sua instalação, talvez seja necessário usar `docker-compose` (com hífen)._

Após a execução, os seguintes serviços estarão disponíveis:

- **BFF (Gateway):** `http://localhost:8084`
- **Auth API:** `http://localhost:8081`
- **Courses API:** `http://localhost:8082`
- **Students API:** `http://localhost:8083`
- **Payments API:** `http://localhost:8080`
- **RabbitMQ Management:** `http://localhost:15672` (usuário: `guest`, senha: `guest`)
- **SQL Server:** Acessível na porta `1433` para gerenciamento externo.

---

### ⚙️ Deploy em um Cluster Kubernetes

Os manifestos YAML para a infraestrutura e para cada serviço estão localizados nas pastas `src/k8s` e nas respectivas pastas de projeto.

#### Passo 1: Crie os Segredos no Cluster

Segredos como senhas de banco de dados e chaves JWT não devem ser versionados. Crie-os diretamente no seu cluster Kubernetes. **Substitua os valores de exemplo (`<..._PASSWORD_...>`, etc.) pelos seus segredos reais.**

```bash
# Segredo para o SQL Server
kubectl create secret generic sql-server-secret --from-literal=SA_PASSWORD='<YOUR_DB_PASSWORD>'

# Segredo para o RabbitMQ
kubectl create secret generic rabbitmq-secret --from-literal=RABBITMQ_USER='guest' --from-literal=RABBITMQ_PASS='guest'

# Segredo para a Auth API
kubectl create secret generic auth-api-secret \
  --from-literal=ConnectionStrings__DefaultConnection='Server=sqlserver-service;Database=AuthDb;User Id=sa;Password=<YOUR_DB_PASSWORD>;TrustServerCertificate=true' \
  --from-literal=Jwt__Key='<YOUR_JWT_KEY_AUTH>'

# Segredo para a Courses API
kubectl create secret generic courses-api-secret \
  --from-literal=ConnectionStrings__DefaultConnection='Server=sqlserver-service;Database=CoursesDb;User Id=sa;Password=<YOUR_DB_PASSWORD>;TrustServerCertificate=true'

# Segredo para a Students API
kubectl create secret generic students-api-secret \
  --from-literal=ConnectionStrings__DefaultConnection='Server=sqlserver-service;Database=StudentsDb;User Id=sa;Password=<YOUR_DB_PASSWORD>;TrustServerCertificate=true'

# Segredo para a Payments API
kubectl create secret generic payments-api-secret \
  --from-literal=ConnectionStrings__DefaultConnection='Server=sqlserver-service;Database=PaymentsDb;User Id=sa;Password=<YOUR_DB_PASSWORD>;TrustServerCertificate=true' \
  --from-literal=Payments__ProviderApiKey='<YOUR_PAYMENT_PROVIDER_API_KEY>'

# Segredo para o BFF
kubectl create secret generic bff-secret \
  --from-literal=Jwt__Key='<YOUR_JWT_KEY_BFF>'
```

#### Passo 2: Faça o Deploy da Infraestrutura (SQL Server e RabbitMQ)

Aplique os manifestos para os serviços de infraestrutura.

```bash
# Navegue até a pasta src
cd src

# Aplique os manifestos de infra
kubectl apply -f ./k8s/sqlserver.yml
kubectl apply -f ./k8s/rabbitmq.yml
```

#### Passo 3: Faça o Deploy dos Serviços da Aplicação

Antes de aplicar os manifestos, você precisa **substituir o placeholder do nome de usuário do Docker Hub** nos arquivos `*-manifest.yml` de cada serviço.

A imagem está no formato `{{DOCKERHUB_USERNAME}}/<nome-da-imagem>:latest`. **Substitua `{{DOCKERHUB_USERNAME}}` pelo seu usuário do Docker Hub** ou configure o secret `DOCKERHUB_USERNAME` e use `${{ secrets.DOCKERHUB_USERNAME }}` nos workflows para etiquetar as imagens automaticamente.

Depois de substituir, aplique os manifestos para cada serviço:

```bash
# Estando na pasta src
kubectl apply -f ./services/AcademyIO.Auth.API/auth-api-manifest.yml
kubectl apply -f ./services/AcademyIO.Courses.API/courses-api-manifest.yml
kubectl apply -f ./services/AcademyIO.Students.API/students-api-manifest.yml
kubectl apply -f ./services/AcademyIO.Payments.API/payments-api-manifest.yml
kubectl apply -f ./api-gateways/AcademyIO.Bff/bff-manifest.yml
```

Para verificar o status, use `kubectl get pods` e `kubectl get services`. O BFF estará acessível através do IP externo do seu `LoadBalancer`.

---

### 🔄 Pipeline de CI/CD com GitHub Actions

Este repositório está configurado com pipelines de CI/CD em `.github/workflows`.

- **Como funciona:** Para cada microsserviço, um workflow é acionado em todo `push` ou `pull_request` para a branch `main`.
- **Etapas:** O pipeline executa `build`, `lint` (verificação de formato), e `testes`.
- **Publicação:** Se as etapas anteriores passarem em um push para a `main`, uma imagem Docker é construída e publicada no Docker Hub.

Para que a publicação funcione, você deve configurar os seguintes segredos no seu repositório GitHub (`Settings > Secrets and variables > Actions`):

- `DOCKERHUB_USERNAME`: Seu nome de usuário do Docker Hub.
- `DOCKERHUB_TOKEN`: Um token de acesso do Docker Hub com permissões de escrita.
