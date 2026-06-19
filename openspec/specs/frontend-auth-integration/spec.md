## Purpose

Defines the integration between the admin dashboard shell and the backend's Identity/JWT auth: the real login flow, token storage and attachment, and route protection for unauthenticated users.

## Requirements

### Requirement: Real login through the dashboard UI
The dashboard's `SignInPage` SHALL submit credentials to the real backend JWT login endpoint and handle success/failure responses, replacing any mock or static authentication the WowDash template ships with.

#### Scenario: Successful login navigates to the dashboard
- **WHEN** a user submits valid credentials on the sign-in page
- **THEN** the frontend receives a JWT, stores it, and navigates to the authenticated dashboard shell

#### Scenario: Failed login shows an error
- **WHEN** a user submits invalid credentials on the sign-in page
- **THEN** the frontend displays an error message and does not navigate away from the sign-in page

### Requirement: Token attached to authenticated requests
Once logged in, the frontend SHALL attach the stored JWT as an `Authorization: Bearer` header on subsequent API requests via the shared API client.

#### Scenario: Authenticated API call succeeds
- **WHEN** a logged-in user's frontend makes a request to a protected API endpoint (e.g. a "who am I" or `/health` check requiring auth)
- **THEN** the request includes the bearer token and receives a successful response

### Requirement: Route protection for unauthenticated users
The frontend SHALL redirect unauthenticated users attempting to access any page other than the sign-in page back to the sign-in page.

#### Scenario: Unauthenticated access redirects to sign-in
- **WHEN** a user with no valid stored token navigates directly to a protected dashboard route
- **THEN** they are redirected to `SignInPage` instead of seeing the protected content
