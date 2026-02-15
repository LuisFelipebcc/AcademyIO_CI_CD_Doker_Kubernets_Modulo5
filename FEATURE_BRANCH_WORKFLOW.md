# Fluxo de CI/CD para Feature Branches

## ✅ Problema Resolvido

**Antes**: Actions não disparavam em branches de feature (apenas em `main`)  
**Agora**: ✅ Actions disparam em qualquer branch + PR automático criado

---

## 🔄 Novo Fluxo de Desenvolvimento

### Passo 1: Criar branch de feature
```bash
git checkout -b feature/sua-feature
```

### Passo 2: Fazer commit
```bash
git add .
git commit -m "add: sua mudança"
git push origin feature/sua-feature
```

### Passo 3: Automático! 🤖
- ✅ **CI Actions disparam** (Build, Test, Coverage)
- ✅ **PR criado automaticamente** no GitHub
- ✅ **Link da ação**: GitHub Actions → Tab "Actions"

### Passo 4: Revisar e Merge
- Verifique os resultados no PR
- Quando tudo passar ✅, faça merge para `main`
- Actions disparam novamente e push para Docker Hub

---

## 📋 Triggers Configurados

### Build, Test & Coverage (CI)
```yaml
# Dispara em: feature/*, fix/*, hotfix/*, main, etc
on.push.branches: ["**"]
```

### PR Automático (Auto-PR)
```yaml
# Dispara em: feature/*, fix/*, hotfix/*
on.push.branches: ["feature/**", "fix/**", "hotfix/**"]
```

### Merge para Docker (CD)
```yaml
# Só dispara em: main
on.push.branches: ["main"]
```

---

## 📊 Comparação: Antes vs Depois

| Cenário | Antes ❌ | Depois ✅ |
|---------|---------|---------|
| Push em `feature/minha-feature` | Nada acontecia | CI roda + PR criado automaticamente |
| PR para `main` | Manual (você criava) | Automático (workflow cria) |
| Merge para `main` | CI roda | CI + Docker push |

---

## 🎯 Exemplos de Uso

### Caso 1: Desenvolvimento de Feature
```bash
# Você faz:
git checkout -b feature/add-tests
# ... edita arquivos ...
git add .
git commit -m "add: unit tests for Auth API"
git push

# GitHub Actions faz:
1. ✅ Cria PR automaticamente
2. ✅ Roda CI (build, test, coverage)
3. ✅ Mostra resultado no PR
```

### Caso 2: Correção de Bug
```bash
git checkout -b fix/auth-validation
# ... corrige bug ...
git push

# GitHub Actions faz:
1. ✅ Cria PR automaticamente
2. ✅ Roda CI
3. ✅ Você revisa e faz merge
```

### Caso 3: Merge para Main
```bash
# Você faz (via GitHub UI ou CLI):
git checkout main
git merge feature/add-tests
git push

# GitHub Actions faz:
1. ✅ Roda todos os testes
2. ✅ Valida cobertura 80%
3. ✅ Build e push Docker image
4. ✅ Tag: seu-docker-image:latest
```

---

## 🔐 Proteções de Branch

### Triggers por Branch

#### `main` Branch
- ✅ CI roda (Build + Test + Coverage)
- ✅ Docker push (se tudo passar)
- ✅ Cobertura validada (80%)
- ⚠️ **Requer PR para merge**

#### Feature Branches (`feature/*`, `fix/*`, `hotfix/*`)
- ✅ CI roda (Build + Test + Coverage)
- ✅ PR criado automaticamente
- ✅ Sem Docker push (segurança)
- ✅ Sem mudança em produção

---

## 📝 Arquivo: `.github/workflows/auto-pr.yml`

```yaml
name: Create Pull Request

on:
  push:
    branches:
      - "feature/**"
      - "fix/**"
      - "hotfix/**"

permissions:
  pull-requests: write
  contents: read

jobs:
  create-pr:
    runs-on: ubuntu-latest
    steps:
      - Checkout
      - Create PR (apenas se não existir)
```

**Lógica**:
1. Detecta push em `feature/*`, `fix/*`, ou `hotfix/*`
2. Verifica se PR já existe
3. Se não existir → cria PR automaticamente
4. Base: `main` | Head: sua branch

---

## 🚀 Próximo Push seu em `feature/gitactions`

Quando você fazer o próximo push, acontecerá:

```
git push origin feature/gitactions
          ↓
GitHub detects push
          ↓
Triggers: auto-pr.yml + courses-ci.yml + auth-ci.yml + ...
          ↓
1. Create PR (se não existir)
2. Run CI for each service
          ↓
Ver no GitHub Actions → seu workflow com status ✅ ou ❌
```

---

## 📖 Referência Rápida

| Comando | Resultado |
|---------|-----------|
| `git push origin feature/xyz` | PR criado + CI roda |
| `git push origin fix/xyz` | PR criado + CI roda |
| `git push origin hotfix/xyz` | PR criado + CI roda |
| `git push origin main` | CI + Docker push |

---

## ❓ Troubleshooting

### "PR não foi criado"
- Verifique se você está em `feature/*`, `fix/*`, ou `hotfix/*`
- Veja a ação `auto-pr.yml` na aba Actions

### "CI não rodou"
- Confirme que você mudou arquivos em:
  - `src/services/AcademyIO.*/` 
  - `src/building-blocks/`
  - `.github/workflows/`
- Caso contrário, CI não dispara (otimização de paths)

### "PR diz que está aguardando checks"
- Aguarde 2-5 minutos
- Vá em Actions → veja o progresso
- Quando todos passarem, aparecerá ✅ "All checks passed"

---

**Resumo**: Agora é tudo automático! Push → PR criado → CI roda → Merge quando passar ✅
