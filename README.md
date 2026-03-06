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

## Tech Stack@

**Backend**

- ASP.NET Core (.NET 10)
- Entity Framework Core + SQL Server

**Frontend**

- React (learning opportunity for myself)

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

### In Progress

- AutoMapper

### Planned

- Unit Tests
- AutoMapper
- Email notifications
- Viber notifications
- React frontend
- React Native mobile app
- All my TODO comments resolved (I try to have clean code so I will leave no unresolved comments in the end)
- And some other cool stuff - stay tuned

---

## API Endpoints

_API endpoint documentation coming soon._

---
