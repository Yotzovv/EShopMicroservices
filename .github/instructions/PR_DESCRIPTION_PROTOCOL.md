# PR_DESCRIPTION_PROTOCOL.md

## Pull Request Description Writing Protocol

When asked to "Write description for the pull request", use the following protocol and template, adapted for this solution:

### Step 1: Identify All Changed Files
- **Command**: `git log [base_branch]..HEAD --name-only --pretty=format: | Where-Object { $_ -ne "" } | Sort-Object | Get-Unique`
- **Purpose**: Get complete list of files modified in the current branch
- **Note**: Replace [base_branch] with the merge base (e.g., `4a9666ec`)

### Step 2: Analyze Each File's Current State (NOT from commits)
- **Read Current Files**: Use VS Code workspace context, not git diff/show commands
- **Core Business Logic**: Examine `src/Services/` files for new methods, modified logic, added logging
- **Unit Tests**: Count and identify new test methods in `tests/**/UnitTests/`
- **Integration Tests**: Count and identify new test methods in `tests/**/IntegrationTests/`
- **Test Infrastructure**: Check for new utility methods, test context enhancements

### Step 3: Structure the Description

#### Format Template (Ready to Copy):
```markdown
## [TICKET-ID] [Ticket Title]

### Summary
Brief description of what the PR implements (1-2 sentences).

### Changes

#### Core Business Logic
**`[FileName].cs`**
- **`[MethodName]`**: [Description of what was added/modified]
- **`[AnotherMethodName]`**: [Description of what was added/modified]

#### Unit Tests (+X new tests)
**`[TestFileName].cs`**
- `[ShortTestName_Scenario_Result]`: [Brief description]
- `[AnotherShortTestName_Scenario_Result]`: [Brief description]

#### Integration Tests (+X new tests)
**`[IntegrationTestFileName].cs`**
- `[ShortTestName_Scenario_Result]`: [Brief description]

#### Test Infrastructure
**`[UtilityFileName].cs`**
- **`[MethodName]`** (NEW METHOD): [Brief description]

Implementation satisfies [TICKET-ID] requirements by [brief summary of how requirements are met].
```

### Step 4: Naming and Formatting Rules

- Use `src/Services/` for all microservice code references.
- Use backticks around file and method names.
- Use concise, single-line bullet points for each change.
- Truncate test names to scenario and expected result.
- Use **+X new tests** to show test count additions.

### Step 5: Content Requirements
- Always mention if operations continue to succeed (non-breaking changes).
- Specify team prefixes, log levels, and contexts for logging changes.
- Count unit tests and integration tests separately.
- Highlight new utility methods and test enhancements.
