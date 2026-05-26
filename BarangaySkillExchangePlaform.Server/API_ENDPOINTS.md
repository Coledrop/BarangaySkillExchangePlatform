# Barangay Skill Exchange API Endpoints

Base URL in development:

- HTTP: `http://localhost:5067`
- HTTPS: `https://localhost:7045`

All endpoints are routed under `/api`. Request and response bodies use JSON.

## Notes

- Authentication is implemented with ASP.NET Core Identity cookies. Authorization is not currently enforced by the users, skill offers, or exchanges controllers.
- The API uses ASP.NET Core model binding. No explicit validation attributes are defined in the current DTOs.
- `createdAt` and `completedAt` timestamps are stored as UTC values.
- `ServiceRequest` models and DTOs exist, but no service request controller is currently exposed.

## Common Error Shape

Most controller errors return an object with a `message` field:

```json
{
  "message": "User not found."
}
```

## Authentication

Routes are defined in `Controllers/AuthController.cs` with base route `/api`.

Authentication uses ASP.NET Core Identity cookie sign-in. Successful `login` and `register` responses set the auth cookie. `logout` clears it.

### POST `/api/register`

Creates a new user account with a password, signs the user in, and returns a sanitized user payload. New users are saved with `status` set to `Active`.

Request body:

```json
{
  "fullName": "Juan Dela Cruz",
  "email": "juan@example.com",
  "password": "secret123",
  "contactNumber": "09171234567",
  "address": "Barangay Sample",
  "purok": "Purok 1",
  "role": "Resident"
}
```

Success response:

- `201 Created`
- Body:

```json
{
  "message": "Registration successful.",
  "user": {
    "id": 1,
    "fullName": "Juan Dela Cruz",
    "email": "juan@example.com",
    "contactNumber": "09171234567",
    "address": "Barangay Sample",
    "role": "Resident",
    "status": "Active",
    "createdAt": "2026-05-25T00:00:00Z"
  }
}
```

Error responses:

- `400 Bad Request` when Identity password or user validation fails.
- `409 Conflict` when the email is already registered.

### POST `/api/login`

Signs in an existing non-suspended user with email and password.

Request body:

```json
{
  "email": "juan@example.com",
  "password": "secret123",
  "rememberMe": false
}
```

Success response:

- `200 OK`
- Body:

```json
{
  "message": "Login successful.",
  "user": {
    "id": 1,
    "fullName": "Juan Dela Cruz",
    "email": "juan@example.com",
    "contactNumber": "09171234567",
    "address": "Barangay Sample",
    "role": "Resident",
    "status": "Active",
    "createdAt": "2026-05-25T00:00:00Z"
  }
}
```

Error responses:

- `401 Unauthorized` when the email or password is invalid.
- `401 Unauthorized` when the user status is `Suspended`.

### POST `/api/logout`

Signs out the current cookie-authenticated user.

Request body: none.

Success response:

- `200 OK`

```json
{
  "message": "Logout successful."
}
```

## Users

Routes are defined in `Controllers/UsersController.cs` with base route `/api/users`.

### GET `/api/users`

Returns all users ordered by newest `createdAt` first.

Query parameters: none.

Success response:

- `200 OK`
- Body: array of `User`

### GET `/api/users/{id}`

Returns a single user by integer ID.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | User ID. |

Success response:

- `200 OK`
- Body: `User`

Error responses:

- `404 Not Found` when the user does not exist.

### POST `/api/users`

Creates a user. New users are saved with `status` set to `Active`.

Request body:

```json
{
  "fullName": "Juan Dela Cruz",
  "email": "juan@example.com",
  "contactNumber": "09171234567",
  "address": "Barangay Sample",
  "purok": "Purok 1",
  "role": "Resident"
}
```

Fields:

| Name | Type | Required | Default | Notes |
| --- | --- | --- | --- | --- |
| `fullName` | string | Yes | `""` | Saved to `FullName`. |
| `email` | string | Yes | `""` | Must be unique. |
| `contactNumber` | string | Yes | `""` | Saved to `ContactNumber`. |
| `address` | string | Yes | `""` | Saved to `Address`. |
| `purok` | string | No | `""` | Present in DTO but not currently saved to the `User` model. |
| `role` | string | No | `"Resident"` | Saved to `Role`. |

Success response:

- `201 Created`
- Body: created `User`
- `Location` points to `/api/users/{id}`

Error responses:

- `409 Conflict` when the email is already registered.

### PUT `/api/users/{id}`

Updates an existing user's profile fields and status.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | User ID. |

Request body:

```json
{
  "fullName": "Juan Dela Cruz",
  "contactNumber": "09171234567",
  "address": "Barangay Sample",
  "purok": "Purok 1",
  "status": "Active"
}
```

Fields:

| Name | Type | Required | Default | Notes |
| --- | --- | --- | --- | --- |
| `fullName` | string | Yes | `""` | Updates `FullName`. |
| `contactNumber` | string | Yes | `""` | Updates `ContactNumber`. |
| `address` | string | Yes | `""` | Updates `Address`. |
| `purok` | string | No | `""` | Present in DTO but not currently saved to the `User` model. |
| `status` | string | No | `"Active"` | Updates `Status`; no controller-level allowed list is enforced. |

Success response:

- `200 OK`
- Body: updated `User`

Error responses:

- `404 Not Found` when the user does not exist.

### DELETE `/api/users/{id}`

Deletes a user by ID.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | User ID. |

Success response:

- `200 OK`

```json
{
  "message": "User deleted successfully."
}
```

Error responses:

- `404 Not Found` when the user does not exist.

## Skill Offers

Routes are defined in `Controllers/SkillOffersController.cs` with base route `/api/skill-offers`.

### GET `/api/skill-offers`

Returns skill offers ordered by newest `createdAt` first. Each result includes its related `user`.

Query parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `search` | string | No | Case-insensitive partial match against `title` or `description`. |
| `category` | string | No | Case-insensitive exact match against `category`. |

Example:

```http
GET /api/skill-offers?search=tutor&category=Education
```

Success response:

- `200 OK`
- Body: array of `SkillOffer`

### GET `/api/skill-offers/{id}`

Returns a single skill offer by integer ID. The response includes its related `user`.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | Skill offer ID. |

Success response:

- `200 OK`
- Body: `SkillOffer`

Error responses:

- `404 Not Found` when the skill offer does not exist.

### POST `/api/skill-offers`

Creates a skill offer for an existing active user. New skill offers are saved with `status` set to `Active`.

Request body:

```json
{
  "userId": 1,
  "title": "Math tutoring",
  "description": "I can help with basic algebra.",
  "category": "Education",
  "availability": "Weekends",
  "locationPreference": "Barangay hall"
}
```

Fields:

| Name | Type | Required | Default |
| --- | --- | --- | --- |
| `userId` | integer | Yes | `0` |
| `title` | string | Yes | `""` |
| `description` | string | Yes | `""` |
| `category` | string | Yes | `""` |
| `availability` | string | Yes | `""` |
| `locationPreference` | string | Yes | `""` |

Success response:

- `201 Created`
- Body: created `SkillOffer`
- `Location` points to `/api/skill-offers/{id}`

Error responses:

- `400 Bad Request` when the user does not exist.
- `400 Bad Request` when the user status is `Suspended`.

### PUT `/api/skill-offers/{id}`

Updates an existing skill offer.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | Skill offer ID. |

Request body:

```json
{
  "title": "Math tutoring",
  "description": "I can help with basic algebra.",
  "category": "Education",
  "availability": "Weekends",
  "locationPreference": "Barangay hall",
  "status": "Active"
}
```

Fields:

| Name | Type | Required | Default | Notes |
| --- | --- | --- | --- | --- |
| `title` | string | Yes | `""` | Updates `Title`. |
| `description` | string | Yes | `""` | Updates `Description`. |
| `category` | string | Yes | `""` | Updates `Category`. |
| `availability` | string | Yes | `""` | Updates `Availability`. |
| `locationPreference` | string | Yes | `""` | Updates `LocationPreference`. |
| `status` | string | No | `"Active"` | No controller-level allowed list is enforced. |

Success response:

- `200 OK`
- Body: updated `SkillOffer`

Error responses:

- `404 Not Found` when the skill offer does not exist.

### DELETE `/api/skill-offers/{id}`

Deletes a skill offer by ID.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | Skill offer ID. |

Success response:

- `200 OK`

```json
{
  "message": "Skill offer deleted successfully."
}
```

Error responses:

- `404 Not Found` when the skill offer does not exist.

## Exchanges

Routes are defined in `Controllers/ExchangesController.cs` with base route `/api/exchanges`.

### GET `/api/exchanges`

Returns all exchanges ordered by newest `createdAt` first. Each result includes related `requester`, `provider`, `skillOffer`, and `serviceRequest` when available.

Success response:

- `200 OK`
- Body: array of `Exchange`

### GET `/api/exchanges/{id}`

Returns a single exchange by integer ID. The response includes related `requester`, `provider`, `skillOffer`, and `serviceRequest` when available.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | Exchange ID. |

Success response:

- `200 OK`
- Body: `Exchange`

Error responses:

- `404 Not Found` when the exchange does not exist.

### GET `/api/exchanges/user/{userId}`

Returns all exchanges where the user is either the requester or provider, ordered by newest `createdAt` first.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `userId` | integer | Yes | Requester or provider user ID. |

Success response:

- `200 OK`
- Body: array of `Exchange`

### POST `/api/exchanges`

Creates a new exchange between two different non-suspended users. New exchanges are saved with `status` set to `Pending`.

Request body:

```json
{
  "requesterId": 1,
  "providerId": 2,
  "skillOfferId": 5,
  "serviceRequestId": null,
  "message": "Can we schedule this on Saturday?",
  "proposedSchedule": "Saturday 2 PM",
  "location": "Barangay hall"
}
```

Fields:

| Name | Type | Required | Default | Notes |
| --- | --- | --- | --- | --- |
| `requesterId` | integer | Yes | `0` | Must not equal `providerId`. |
| `providerId` | integer | Yes | `0` | Must not equal `requesterId`. |
| `skillOfferId` | integer or null | No | `null` | No controller-level existence check is currently performed. |
| `serviceRequestId` | integer or null | No | `null` | No controller-level existence check is currently performed. |
| `message` | string | Yes | `""` | Saved to `Message`. |
| `proposedSchedule` | string | Yes | `""` | Saved to `ProposedSchedule`. |
| `location` | string | Yes | `""` | Saved to `Location`. |

Success response:

- `201 Created`
- Body: created `Exchange`
- `Location` points to `/api/exchanges/{id}`

Error responses:

- `400 Bad Request` when requester and provider are the same user.
- `400 Bad Request` when requester or provider does not exist.
- `400 Bad Request` when requester or provider has status `Suspended`.

### PUT `/api/exchanges/{id}/status`

Updates an exchange status. If the new status is `Completed`, `completedAt` is set to the current UTC timestamp.

Allowed statuses:

- `Pending`
- `Accepted`
- `Rejected`
- `Completed`
- `Cancelled`

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | Exchange ID. |

Request body:

```json
{
  "status": "Accepted"
}
```

Success response:

- `200 OK`
- Body: updated `Exchange`

Error responses:

- `400 Bad Request` when `status` is not one of the allowed statuses.
- `404 Not Found` when the exchange does not exist.

### PUT `/api/exchanges/{id}/accept`

Sets an exchange status to `Accepted`.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | Exchange ID. |

Success response:

- `200 OK`

```json
{
  "message": "Exchange accepted successfully.",
  "exchange": {}
}
```

Error responses:

- `404 Not Found` when the exchange does not exist.

### PUT `/api/exchanges/{id}/reject`

Sets an exchange status to `Rejected`.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | Exchange ID. |

Success response:

- `200 OK`

```json
{
  "message": "Exchange rejected successfully.",
  "exchange": {}
}
```

Error responses:

- `404 Not Found` when the exchange does not exist.

### PUT `/api/exchanges/{id}/complete`

Sets an exchange status to `Completed` and sets `completedAt` to the current UTC timestamp.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | Exchange ID. |

Success response:

- `200 OK`

```json
{
  "message": "Exchange completed successfully.",
  "exchange": {}
}
```

Error responses:

- `404 Not Found` when the exchange does not exist.

### DELETE `/api/exchanges/{id}`

Deletes an exchange by ID.

Path parameters:

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | integer | Yes | Exchange ID. |

Success response:

- `200 OK`

```json
{
  "message": "Exchange deleted successfully."
}
```

Error responses:

- `404 Not Found` when the exchange does not exist.

## Response Models

### User

```json
{
  "id": 1,
  "fullName": "Juan Dela Cruz",
  "email": "juan@example.com",
  "contactNumber": "09171234567",
  "address": "Barangay Sample",
  "role": "Resident",
  "status": "Active",
  "createdAt": "2026-05-25T00:00:00Z",
  "skillOffers": [],
  "serviceRequests": []
}
```

`User` inherits from `IdentityUser<int>`, so responses may also include ASP.NET Identity fields such as `userName`, `normalizedUserName`, `normalizedEmail`, `emailConfirmed`, `phoneNumber`, `lockoutEnabled`, and related properties.

### SkillOffer

```json
{
  "id": 5,
  "userId": 1,
  "user": {},
  "title": "Math tutoring",
  "description": "I can help with basic algebra.",
  "category": "Education",
  "availability": "Weekends",
  "locationPreference": "Barangay hall",
  "status": "Active",
  "createdAt": "2026-05-25T00:00:00Z"
}
```

### ServiceRequest

```json
{
  "id": 7,
  "userId": 1,
  "user": {},
  "title": "Need plumbing help",
  "description": "Kitchen sink repair.",
  "category": "Home repair",
  "exchangeOffer": "Computer troubleshooting",
  "preferredSchedule": "Weekday evening",
  "location": "Barangay Sample",
  "status": "Open",
  "createdAt": "2026-05-25T00:00:00Z"
}
```

### Exchange

```json
{
  "id": 10,
  "requesterId": 1,
  "requester": {},
  "providerId": 2,
  "provider": {},
  "skillOfferId": 5,
  "skillOffer": {},
  "serviceRequestId": null,
  "serviceRequest": null,
  "message": "Can we schedule this on Saturday?",
  "proposedSchedule": "Saturday 2 PM",
  "location": "Barangay hall",
  "status": "Pending",
  "createdAt": "2026-05-25T00:00:00Z",
  "completedAt": null
}
```
