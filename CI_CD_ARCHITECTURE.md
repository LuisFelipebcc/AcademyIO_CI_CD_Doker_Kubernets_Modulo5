# GitHub Actions CI/CD Architecture

## Overview
Restructured CI/CD pipeline with a **reusable workflow template** to eliminate code duplication across 5 microservices.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                      .github/workflows/                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────┐  ┌──────────────────┐  ┌────────────┐   │
│  │  auth-ci.yml     │  │ courses-ci.yml   │  │ ...        │   │
│  │ (Service-specific)   (18 lines)     (5 workflows)        │   │
│  └────────┬─────────┘  └────────┬────────┘  └────┬───────┘   │
│           │                     │                 │            │
│           │                     │                 │            │
│           └─────────────────────┼─────────────────┘            │
│                                 │                              │
│                                 ▼                              │
│                    ┌────────────────────────┐                  │
│                    │  ci-template.yml       │                  │
│                    │  (Reusable workflow)   │                  │
│                    │  - workflow_call       │                  │
│                    │  - Build               │                  │
│                    │  - Test + Coverage     │                  │
│                    │  - Docker Push         │                  │
│                    │  (106 lines)           │                  │
│                    └────────────────────────┘                  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Key Changes

### Before (❌ Duplicated)
```yaml
# 5 separate files with IDENTICAL jobs:
# auth-ci.yml, courses-ci.yml, students-ci.yml, payments-ci.yml, bff-ci.yml
# ~100 lines each = 500+ lines of duplicated code
```

### After (✅ DRY)
```yaml
# 5 lightweight service workflows (~18 lines each)
# 1 reusable template (~106 lines)
# Total: ~200 lines vs 500+ lines (60% reduction)
```

## Service Workflows (Lightweight)

Each service now follows this pattern:

```yaml
name: Auth API CI

on:
  push:
    branches: ["main"]
    paths:
      - "src/services/AcademyIO.Auth.API/**"
      - "src/building-blocks/**"
      - ".github/workflows/auth-ci.yml"
  pull_request:
    branches: ["main"]
    paths:
      - "src/services/AcademyIO.Auth.API/**"
      - "src/building-blocks/**"
      - ".github/workflows/auth-ci.yml"

jobs:
  ci:
    uses: ./.github/workflows/ci-template.yml
    with:
      project_name: "src/services/AcademyIO.Auth.API/"
      dockerfile_path: "./src/services/AcademyIO.Auth.API/Dockerfile"
      docker_image_name: "academyio-auth-api"
    secrets: inherit
```

### Service Workflows Files
- ✅ [auth-ci.yml](.github/workflows/auth-ci.yml)
- ✅ [courses-ci.yml](.github/workflows/courses-ci.yml)
- ✅ [students-ci.yml](.github/workflows/students-ci.yml)
- ✅ [payments-ci.yml](.github/workflows/payments-ci.yml)
- ✅ [bff-ci.yml](.github/workflows/bff-ci.yml)

## Reusable Template (`ci-template.yml`)

The **single source of truth** for CI/CD logic:

### Inputs
| Parameter | Purpose |
|-----------|---------|
| `project_name` | Project path (e.g., `src/services/AcademyIO.Courses.API/`) |
| `dockerfile_path` | Dockerfile location |
| `docker_image_name` | Docker Hub image name |

### Jobs

#### 1. **Build Job** (Runs on every push/PR)
- ✅ Restore dependencies
- ✅ Build in Release mode
- ✅ Format check (Lint)
- ✅ Run unit tests with coverage
- ✅ Upload coverage artifacts
- ✅ Validate 80% coverage threshold per project

#### 2. **Docker Build & Push Job** (Only on main branch)
- ✅ Requires build job to succeed
- ✅ Authenticates with Docker Hub (via secrets)
- ✅ Builds Docker image
- ✅ Pushes with tags: `{version}` and `latest`

## Coverage Validation

Each service must maintain **≥80% code coverage**:

```bash
# The template runs this Python script:
python3 - "$file" "${{ inputs.project_name }}" <<'PY'
  # Parses coverage.cobertura.xml
  # Calculates line coverage for specific project
  # Exits with code 1 if below 80% threshold
PY
```

### Current Status
| Service | Coverage | Status |
|---------|----------|--------|
| AcademyIO.Auth.API | N/A | ⏳ Needs tests |
| AcademyIO.Courses.API | 7.07% | ⚠️ Below threshold |
| AcademyIO.Students.API | N/A | ⏳ Needs tests |
| AcademyIO.Payments.API | N/A | ⏳ Needs tests |
| AcademyIO.Bff | N/A | ⏳ Needs tests |

## Benefits

| Aspect | Before | After |
|--------|--------|-------|
| **Lines of Code** | 500+ | ~200 |
| **Duplication** | ❌ Severe (5x) | ✅ None |
| **Maintainability** | ❌ Change 5 files | ✅ Change 1 file |
| **Service Setup** | ❌ Complex | ✅ 8 lines |
| **Consistency** | ❌ Easy to drift | ✅ Enforced |

## How to Add a New Service

1. Create new service workflow file: `.github/workflows/myservice-ci.yml`
2. Add 8-line template call:
   ```yaml
   jobs:
     ci:
       uses: ./.github/workflows/ci-template.yml
       with:
         project_name: "src/services/AcademyIO.MyService.API/"
         dockerfile_path: "./src/services/AcademyIO.MyService.API/Dockerfile"
         docker_image_name: "academyio-myservice-api"
       secrets: inherit
   ```
3. Done! ✅

## How to Update CI/CD Logic

**Old Way** (❌ 5 files to update):
```
Modify auth-ci.yml
Modify courses-ci.yml
Modify students-ci.yml
Modify payments-ci.yml
Modify bff-ci.yml
```

**New Way** (✅ 1 file to update):
```
Modify .github/workflows/ci-template.yml
↓ All 5 services automatically use new logic
```

## Trigger Paths

Each service workflow only triggers on changes to:
- Its service directory: `src/services/AcademyIO.Auth.API/**`
- Shared building blocks: `src/building-blocks/**`
- Its own workflow file: `.github/workflows/auth-ci.yml`

This prevents unnecessary builds for unrelated changes.

## Secrets Required

For Docker push functionality:
- `DOCKERHUB_USERNAME`
- `DOCKERHUB_TOKEN`

Configure in GitHub repository Settings → Secrets & Variables → Repository Secrets

## Testing Locally

```bash
# Run all tests with coverage
dotnet test --collect:"XPlat Code Coverage" --settings tests/coverage.runsettings AcademyIO.sln

# Check coverage for specific project
# (Python script in ci-template.yml does this)
```

## Files Modified

| File | Changes |
|------|---------|
| [.github/workflows/ci-template.yml](.github/workflows/ci-template.yml) | ✅ Created (new reusable template) |
| [.github/workflows/auth-ci.yml](.github/workflows/auth-ci.yml) | ✅ Refactored (18 lines, calls template) |
| [.github/workflows/courses-ci.yml](.github/workflows/courses-ci.yml) | ✅ Refactored (18 lines, calls template) |
| [.github/workflows/students-ci.yml](.github/workflows/students-ci.yml) | ✅ Refactored (18 lines, calls template) |
| [.github/workflows/payments-ci.yml](.github/workflows/payments-ci.yml) | ✅ Refactored (18 lines, calls template) |
| [.github/workflows/bff-ci.yml](.github/workflows/bff-ci.yml) | ✅ Refactored (18 lines, calls template) |

## Next Steps

1. **Test in GitHub**: Make a commit to trigger workflows
   ```bash
   git add .
   git commit -m "refactor: consolidate CI/CD with reusable workflow template"
   git push origin main
   ```

2. **Monitor**: Check GitHub Actions tab to verify workflows execute correctly

3. **Improve Coverage**: Add more tests to reach 80% threshold per service

4. **Iterate**: Any future CI/CD changes only require updating `ci-template.yml`
