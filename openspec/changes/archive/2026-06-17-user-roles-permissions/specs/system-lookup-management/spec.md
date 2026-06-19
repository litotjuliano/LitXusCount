## ADDED Requirements

### Requirement: Per-entity permissions on lookup endpoints
Each of the 8 lookup entities' endpoints SHALL require a permission specific to that entity and action (`Settings.{Entity}.View` for reads, `Settings.{Entity}.Create`/`Edit`/`Delete` for writes) instead of bare `[Authorize]`. Holding a permission for one entity (e.g. `Settings.Currency.Delete`) SHALL NOT grant the equivalent action on a different entity (e.g. `Settings.Category.Delete`).

#### Scenario: Missing permission is rejected
- **WHEN** a user without `Settings.Currency.Delete` attempts to delete a currency
- **THEN** the response status is 403

#### Scenario: Holding the permission succeeds
- **WHEN** a user with `Settings.Currency.Delete` attempts to delete a currency
- **THEN** the request succeeds

#### Scenario: Permissions do not cross entities
- **WHEN** a user has `Settings.Currency.Delete` but not `Settings.Category.Delete`, and attempts to delete a category
- **THEN** the response status is 403
