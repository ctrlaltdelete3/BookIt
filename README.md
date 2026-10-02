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

BookIt is a REST API for an appointment booking platform. Service providers (such as hairdressers, beauticians, personal trainers, etc.) can register their business, define services they provide (with duration and break time) and their working hours, and manage incoming booking requests. Clients can browse available slots and send appointment requests, which the provider can confirm or reject.

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
- Working hours configuration (incl. cross-field validation — working day requires start/end time, optional pause must fit inside working hours)
- Appointment booking
- Dynamic availability calculation — free slots computed from working hours minus pause and existing bookings, using service duration + break time (no manually configured time slots)
- Global exception handling middleware
- Input validation (FluentValidation) - DTOs
- Unit tests (service layer + working hours validator)
- Refresh token authentication (short-lived access token + HttpOnly cookie refresh token)
- Angular frontend ([bookit-frontend](https://github.com/ctrlaltdelete3/bookit-frontend))
- Appointment double-booking prevention (DB-level unique constraint on tenant/service/date/time + friendly error message on conflict)

### Planned

- Weekly availability endpoint + weekly calendar view in the frontend
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

- **Most FluentValidation validators have no dedicated tests** — only `WorkingHourDtoValidator` is covered; validators for appointments, services, tenants and login/register are not (service tests call services directly, bypassing the `ValidationFilter` pipeline where validators actually run)
- **Tenant owners can't view or reactivate their own soft-deleted services** — deleting a service hides it from the owner too, with no way to see or restore it
- **Duplicate email on registration returns 400 instead of 409 Conflict**
- **Error messages are mixed English/Croatian** — most backend messages are in English while the frontend UI is in Croatian
- **No redirect back to the originally requested page** after login or a failed silent token refresh — user always lands on the default page instead
- **`User`/`Tenant` shared fields (`Id`, `CreatedAt`, contact info) aren't factored into a common base class** — planned as a pure C#/OOP cleanup, no schema change
- **`xunit` v2 → v3 migration pending** (low priority, current version still receives security patches)

---
