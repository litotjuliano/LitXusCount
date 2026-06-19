## ADDED Requirements

### Requirement: Per-action permissions on the Company Info endpoint
The Company Info endpoint SHALL require `Settings.CompanyInfo.View` for reads and `Settings.CompanyInfo.Edit` for the edit action, instead of bare `[Authorize]`. There is no `Settings.CompanyInfo.Create`/`Delete` permission, consistent with Company Info having no create or delete operation.

#### Scenario: Missing view permission is rejected
- **WHEN** a user without `Settings.CompanyInfo.View` requests Company Info
- **THEN** the response status is 403

#### Scenario: Missing edit permission is rejected
- **WHEN** a user with `Settings.CompanyInfo.View` but not `Settings.CompanyInfo.Edit` attempts to edit Company Info
- **THEN** the response status is 403
