# AcademyIO.Payments.API — DevOps Quick Start

This folder contains the Payments API and the files required for the minimum DevOps delivery (24/11).

Quick Start (requirements: Docker, Docker Compose)

1. Build and run with Docker Compose

```powershell
cd c:\Users\Public\Repositorio\Mudulo5\src
docker compose up --build
```

The compose file starts a local SQL Server and the Payments API (mapped to `http://localhost:8080`).

2. Swagger and Health

- Swagger: `http://localhost:8080/swagger` (se a API expõe Swagger em Development)
- Liveness: `http://localhost:8080/health/live`
- Readiness: `http://localhost:8080/health/ready`

3. Build image locally

```powershell
cd services\AcademyIO.Payments.API
docker build -t <seu-usuario>/academyio-payments-api:latest -f Dockerfile .
docker run --rm -e ASPNETCORE_URLS=http://+:8080 -p 8080:8080 <seu-usuario>/academyio-payments-api:latest
```

4. Apply Kubernetes manifest (requires kubectl + cluster)

```powershell
kubectl apply -f .\services\AcademyIO.Payments.API\payments-api-manifest.yml
kubectl rollout status deployment/payments-api-deployment
kubectl port-forward svc/payments-api-service 8080:80
```

Notes

- Replace `<seu-usuario>` with your Docker Hub username if you push images.
- Update connection strings and secrets before applying to a real cluster.
