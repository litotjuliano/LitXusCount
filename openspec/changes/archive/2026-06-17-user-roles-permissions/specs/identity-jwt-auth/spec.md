## ADDED Requirements

### Requirement: JWT carries effective permission claims
In addition to role-name claims, the JWT access token SHALL include a `"permission"` claim for every permission granted by any role the user belongs to (the union across all their roles, de-duplicated).

#### Scenario: Permission claims reflect the user's roles
- **WHEN** a user who belongs to a role with permissions `Users.View` and `Roles.View` logs in
- **THEN** the issued JWT contains `"permission"` claims for both `Users.View` and `Roles.View`

#### Scenario: Permissions from multiple roles are merged
- **WHEN** a user belongs to two roles whose permission sets only partially overlap
- **THEN** the issued JWT's permission claims are the union of both roles' permissions, with no duplicates
