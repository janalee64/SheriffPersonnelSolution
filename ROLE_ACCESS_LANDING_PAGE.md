# Role Access & Home Landing Page Design Proposals

## Problem Summary
When a user logs in with only the `Administrator` role (or any role other than `Personnel`), they encounter:
1. An initial **"404 - Page Not Found"** error (caused by server-side HTTP 403 Forbidden on root `/` which gets re-executed to `/not-found`).
2. Clicking **"Go To Home"** leads to **"Access Denied"** (caused by client-side Blazor `AuthorizeRouteView` blocking `/` because `Home.razor` requires `Roles = "Personnel"`).

## Root Cause
- The root route `@page "/"` is currently bound directly to `SheriffPersonnel.Web/Components/Pages/Home.razor`, decorated with `@attribute [Authorize(Roles = "Personnel")]`.
- Non-Personnel users hitting `/` are blocked before rendering on initial request (403 re-executed to `/not-found`), and blocked client-side on SPA navigation (`AccessDenied.razor`).

## Proposed Options

### Option A: Generic Home Dashboard with Role Cards (Recommended)
- Move Employee Search to `@page "/employees"` (requiring `Roles = "Personnel"`).
- Make `/` an authenticated generic Home Dashboard (`@attribute [Authorize]`).
- Display a welcome header and interactive cards/tiles for available modules based on the user's assigned roles:
  - **User Admin & Code Tables**: Visible to `Administrator`.
  - **Employee Search**: Visible to `Personnel`.
  - **Inventory Admin**: Visible to `Inventory`.
  - **Multiple Equipment**: Visible to `Equipment`.
  - **Multiple Trainings**: Visible to `Training`.
  - **Range Statistics**: Visible to `Range`.
  - **Enter Range Scores**: Visible to all authenticated users.

### Option B: Smart Dynamic Landing Page Redirect
- Keep `/` open to all authenticated users (`@attribute [Authorize]`).
- On initialization, inspect the user's roles and automatically redirect to their primary authorized module:
  1. `Personnel` -> `/employees`
  2. `Administrator` -> `/admin/users`
  3. `Inventory` -> `/inventoryadmin`
  4. `Equipment` -> `/multipleequipment`
  5. `Training` -> `/multipletrainings`
  6. `Range` -> `/rangestatistics`
  7. Fallback -> `/enterrangescores`

