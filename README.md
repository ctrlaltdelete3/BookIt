# BookIt App

This app is for booking all kinds of appointments (appointments with hairdressers, beauticians, personal trainers, etc.). Main goal for the app is for it to make appointment management accessible to anyone, regardless of their technical background, and help small service providers run their business more efficiently.

_**Note:** I started working on this app to sharpen up my .NET skills and learn new technologies along the way. The app is being built in iterations — the first version focuses on core booking functionality, while more advanced features are planned for later_

---

## Table of Contents

- [About](#about)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Current Status](#current-status)
- [API Endpoints](#api-endpoints)

---

## About

BookIt is a REST API for an appointment booking platform. Service providers (such as hairdressers, beauticians, personal trainers, etc.) can register their business, define services they provide with custom time slots and working hours, and manage incoming booking requests. Clients can browse available slots and send appointment requests, which the provider can confirm or reject.

The idea behind this came from real-world situations - many small service providers still manage appointments manually (phone calls, messages) and keep track of their schedules by hand. This is often the case because they lack a strong technical background, or existing solutions feel too complex. The goal of this app is to digitize all of that with as few clicks as possible — simple, fast, and easy for everyone.

---

## Tech Stack

**Backend**

- ASP.NET Core (.NET 10)
- Entity Framework Core + SQL Server

**Frontend**

- Angular (learning opportunity for myself)

---

## Architecture

```
BookIt.Api         → Controllers, Middleware, Filters
BookIt.Application → DTOs, Interfaces, Validators
BookIt.Services    → Business Logic
BookIt.DAL         → Repositories, DbContext, Migrations
BookIt.Domain      → Entities
```

---

## Current Status

### Done

- User authentication
- Tenant management
- Service management
- Service time slots
- Working hours configuration
- Appointment booking
- Availability calculation
- Global exception handling middleware
- Input validation (FluentValidation) - DTOs
- Unit tests (service layer)
- Refresh token authentication (short-lived access token + HttpOnly cookie refresh token)
- Angular frontend ([bookit-frontend](../bookit-frontend))
- Appointment double-booking prevention (DB-level unique constraint on tenant/service/date/time + friendly error message on conflict)

### Planned

- Dynamic slot computation — calculate available appointment slots directly from working hours minus existing bookings, instead of relying on manually configured fixed time slots per service
- AutoMapper (replace current manual DTO mapping)
- Email notifications
- Viber notifications
- Ionic mobile app (maybe, in future plans)
- All my TODO comments resolved (I try to have clean code so I will leave no unresolved comments in the end)
- And some other cool stuff - stay tuned

---

## API Endpoints

_API endpoint documentation coming soon._

---

## Known Limitations

These are known gaps, tracked for follow-up — not oversights.

- **FluentValidation validators have no dedicated tests** — existing tests call services directly, bypassing the `ValidationFilter` pipeline where validators actually run
- **Tenant owners can't view or reactivate their own soft-deleted services** — deleting a service hides it from the owner too, with no way to see or restore it
- **No cross-field validation on working hours** — a day marked as a working day isn't required to have start/end/pause times filled in
- **No redirect back to the originally requested page** after login or a failed silent token refresh — user always lands on the default page instead
- **`User`/`Tenant` shared fields (`Id`, `CreatedAt`, contact info) aren't factored into a common base class** — planned as a pure C#/OOP cleanup, no schema change
- **`xunit` v2 → v3 migration pending** (low priority, current version still receives security patches)

---
