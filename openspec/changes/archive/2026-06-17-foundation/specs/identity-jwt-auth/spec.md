## ADDED Requirements

### Requirement: ASP.NET Core Identity user model
The system SHALL use ASP.NET Core Identity for user storage, with an `ApplicationUser` entity extending `IdentityUser`, persisted via `ApplicationDbContext` in `LitXusCount.Infrastructure`.

#### Scenario: A user can be created
- **WHEN** a new user is registered with a valid username/email and password
- **THEN** an `ApplicationUser` row is persisted and the password is stored hashed (never in plaintext)

### Requirement: JWT bearer authentication, no cookie/Razor login
The API SHALL authenticate requests using JWT bearer tokens only. No cookie-based or server-rendered login page SHALL be implemented — the API issues tokens that the separate SPA frontend stores and attaches to requests.

#### Scenario: Valid credentials return a JWT
- **WHEN** a POST request is made to the login endpoint with a valid username/email and password
- **THEN** the response contains a signed JWT access token and a refresh token

#### Scenario: Invalid credentials are rejected
- **WHEN** a POST request is made to the login endpoint with an incorrect password
- **THEN** the response status is 401 and no token is issued

#### Scenario: Protected endpoint requires a valid token
- **WHEN** a request is made to an endpoint marked `[Authorize]` without a valid `Authorization: Bearer` header
- **THEN** the response status is 401

#### Scenario: Protected endpoint accepts a valid token
- **WHEN** a request is made to an endpoint marked `[Authorize]` with a valid, unexpired JWT in the `Authorization: Bearer` header
- **THEN** the request succeeds and the endpoint can resolve the calling user's identity

### Requirement: Refresh token support
The system SHALL support exchanging a valid refresh token for a new access token, without requiring the user to re-enter credentials, modeled on InventoryMSNV's refresh-token table pattern.

#### Scenario: Refresh token issues a new access token
- **WHEN** a request is made to the refresh endpoint with a valid, unexpired refresh token
- **THEN** a new JWT access token is returned

### Requirement: Password and lockout policy
The system SHALL enforce a configurable password policy and account lockout after repeated failed login attempts, consistent with standard ASP.NET Core Identity options.

#### Scenario: Account locks out after repeated failures
- **WHEN** a user fails to log in with the wrong password more times than the configured maximum attempts
- **THEN** subsequent login attempts are rejected until the lockout period elapses, even with the correct password
