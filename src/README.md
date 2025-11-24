# Plataforma Educacional com Pipeline CI/CD, Docker e Kubernetes

Este projeto consiste em uma plataforma educacional distribuída, construída com .NET, Docker e orquestrada com Kubernetes. O objetivo é fornecer um ecossistema DevOps completo com automação de build, testes, integração e entrega contínua.

## Arquitetura

A plataforma é composta pelos seguintes microsserviços:
-   **Auth API**: Autenticação e gerenciamento de usuários.
-   **Courses API**: Gerenciamento de cursos e aulas.
-   **Students API**: Gerenciamento de matrículas e progresso dos alunos.
-   **Payments API**: Processamento de pagamentos.
-   **BFF (Backend for Frontend)**: Um gateway que agrega as chamadas para as outras APIs, simplificando o consumo pelo front-end.

## Executando Localmente com Docker Compose

Para executar toda a plataforma em seu ambiente local, você precisará ter o Docker e o Docker Compose instalados.

1.  **Clone o repositório:**
    ```sh
    git clone <URL_DO_REPOSITORIO>
    cd <DIRETORIO_DO_PROJETO>/src
    ```

2.  **Execute o Docker Compose:**
    A partir da pasta `src` (que contém o arquivo `docker-compose.yml`), execute o seguinte comando para construir as imagens e iniciar todos os contêineres:
    ```sh
    docker-compose up -d --build
    ```

3.  **Acessando os serviços:**
    Após a conclusão do comando, os serviços estarão disponíveis nos seguintes endereços:
    -   BFF: `http://localhost:8084`
    -   Payments API: `http://localhost:8080`
    -   Auth API: `http://localhost:8081`
    -   Courses API: `http://localhost:8082`
    -   Students API: `http://localhost:8083`
    -   SQL Server: `localhost:1433`

4.  **Parando o ambiente:**
    Para parar todos os contêineres, execute:
    ```sh
    docker-compose down
    ```

## Deploy no Kubernetes

Para fazer o deploy em um cluster Kubernetes (como Minikube, Kind ou um provedor de nuvem), você precisará ter o `kubectl` configurado para acessar seu cluster.

1.  **Imagens Docker:**
    Certifique-se de que as imagens Docker de cada serviço foram publicadas em um registro de contêineres, como o Docker Hub. Os pipelines de CI/CD neste projeto estão configurados para publicar as imagens automaticamente.

    Você precisará atualizar os arquivos `*-manifest.yml` em cada serviço para usar o seu nome de usuário do Docker Hub no lugar do placeholder `SEU_USUARIO_DOCKERHUB`.

2.  **Aplicando os Manifestos:**
    Para cada serviço, aplique os manifestos YAML que definem os `Deployments`, `Services`, `ConfigMaps` e `Secrets`.

    Exemplo para a API de Cursos:
    ```sh
    kubectl apply -f ./services/AcademyIO.Courses.API/courses-api-manifest.yml
    ```

    Você deve aplicar os manifestos para todos os serviços:
    -   `./services/AcademyIO.Auth.API/auth-api-manifest.yml`
    -   `./services/AcademyIO.Courses.API/courses-api-manifest.yml`
    -   `./services/AcademyIO.Students.API/students-api-manifest.yml`
    -   `./services/AcademyIO.Payments.API/payments-api-manifest.yml`
    -   `./api-gateways/AcademyIO.Bff/bff-manifest.yml`

    **Observação:** A ordem de aplicação geralmente não importa, pois o Kubernetes gerencia as dependências. No entanto, é uma boa prática aplicar primeiro as configurações e segredos, se eles forem independentes.

3.  **Verificando o Deploy:**
    Para verificar se os pods estão rodando, execute:
    ```sh
    kubectl get pods
    ```

    Para verificar os serviços e obter o IP externo (no caso do BFF, que está como `LoadBalancer`), execute:
    ```sh
    kubectl get services
    ```

## Pipeline CI/CD

Este projeto utiliza GitHub Actions para automação de CI/CD. Existem workflows separados para cada serviço no diretório `.github/workflows`. Esses pipelines são acionados em cada push ou pull request para a branch `main`, executando as seguintes etapas:
-   Restauração de dependências
-   Build do projeto
-   Execução de testes
-   Build e push da imagem Docker para o Docker Hub (requer a configuração dos segredos `DOCKERHUB_USERNAME` e `DOCKERHUB_TOKEN` no repositório).
