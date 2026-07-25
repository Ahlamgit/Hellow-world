# AGENTS.md

## Cursor Cloud specific instructions

This repository is a minimal GitHub exploration starter (`Hellow-world`). It contains only a `README.md` and has no application code, dependencies, build system, or automated tests.

### Repository contents

| Path | Purpose |
|------|---------|
| `README.md` | Project description ("This rep is for exploring github.") |
| `.git/` | Git metadata; remote is `https://github.com/Ahlamgit/Hellow-world` |

### Services

There are no runnable services (no web server, API, database, or CLI). End-to-end product testing is not applicable until application code is added.

### Development workflow

The intended workflow is Git/GitHub exploration:

```bash
git status
git fetch origin
git pull origin main
```

### Lint / test / build

None configured. No `package.json`, `Makefile`, CI workflows, or git hooks (only default `.sample` hooks).

### Adding an application later

When code is added to this repo, update this section with:

- Dependency install command (for the VM update script)
- How to start required services
- Lint and test commands
- Environment variables or secrets needed
