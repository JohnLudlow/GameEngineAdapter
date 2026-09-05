# AGENTS.md — GitHub Copilot Harness Configuration

This file configures agents and skills for the GitHub Copilot CLI harness in the GameEngineAdapter repository.

For each agent or skill that supports configuration, create a YAML block below with the agent's key and your harness-specific overrides. Configuration merges with defaults and CONTRIBUTING.md settings according to jl-config precedence (AGENTS.md > CONTRIBUTING.md > defaults).

## Agent config

```yaml
# Planning and feature development workflow
jl_planner:
  plan_destination: github_issue
  file_storage_location: docs/plans/

jl_feature_planner:
  plan_destination: github_issue
  file_storage_location: docs/plans/

# Implementation workflow (TDD preferred for C# library code)
jl_tdd_implementer:
  implementation_style: red-green-refactor

jl_feature_implementer:
  code_style: csharp

# Testing and quality
jl_feature_tester:
  test_framework: xunit

jl_tester:
  test_framework: xunit

# Documentation
jl_feature_documenter:
  doc_location: docs/guides/

jl_documenter:
  doc_location: docs/guides/

# Prototype and proof-of-concept work
jl_prototype:
  plan_destination: inline_message
```

## Notes

- **Configuration applied**: This file configures jl-* planning, implementation, testing, and documentation skills for the GitHub Copilot CLI harness.
- **Precedence**: AGENTS.md settings override CONTRIBUTING.md or agent defaults. Omitted settings fall back to lower-precedence sources.
- **Team-wide defaults**: See CONTRIBUTING.md for shared repository configuration (if defined).
- **Portability**: Another contributor can use this same AGENTS.md and inherit the same agent behavior.

For detailed configuration guidance, see:

- `CONTRIBUTING.md` — team-wide agent configuration
- Individual skill documentation for schema, defaults, and allowed values
- [jl-config](https://github.com/johnludlow/agents/tree/main/.agents/skills/jl-config) — portable configuration mechanism
