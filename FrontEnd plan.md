# BookIt - Frontend Plan (React)

## Tech Stack

- **React** (Vite)
- **React Router** - navigacija
- **Axios** - HTTP pozivi
- **Tailwind CSS** - stilizacija

---

## Struktura foldera

```
src/
├── api/          → axios pozivi (po resursu: appointmentApi.js, tenantApi.js...)
├── components/   → reusable komponente (Button, Card, Navbar...)
├── pages/        → stranice (LoginPage, BookingPage...)
├── context/      → AuthContext (čuvanje tokena i user info)
└── hooks/        → custom hookovi (useAuth, useTenant...)
```

---

## Faza 1: Setup i osnove (~4-8h)

**Što trebaš naučiti/postaviti:**
- Što je Vite i kako kreirati React projekt (`npm create vite@latest`)
- Struktura React projekta - što je `src/`, `public/`, `main.jsx`, `App.jsx`
- Što su komponente, props, state (`useState`)
- Kako CSS radi u Reactu
- Setup Tailwind CSS
- Setup Axios

**Resursi:**
- react.dev (službena dokumentacija - interaktivna, odlična za početnike)
- Tailwind CSS dokumentacija
- Axios dokumentacija

---

## Faza 2: Routing i autentifikacija (~6-10h)

**Što trebaš naučiti:**
- **React Router** (`react-router-dom`) - navigacija između stranica
- `useEffect` hook - kako dohvaćati podatke kad se komponenta učita
- Kako spremiti JWT token (localStorage)
- Zaštićene rute - redirect na login ako nisi ulogiran
- `AuthContext` - globalno čuvanje korisničkih podataka

**Stranice:**
- `/login`
- `/register`

---

## Faza 3: Javne stranice (~4-6h)

**Što trebaš naučiti:**
- `useParams` - kako dohvatiti `:slug` iz URL-a
- Uvjetno renderiranje (`if loading... / if error... / else prikaži podatke`)

**Stranice:**
- `/` - landing page (jednostavna, linkovi na login/register)
- `/:slug` - javni profil tenanta (naziv, opis, kontakt, lista usluga)

---

## Faza 4: Booking flow (~10-16h)

Najkompleksniji dio. Multi-step forma:

1. Odabir usluge
2. Odabir datuma
3. Odabir termina (poziv na AvailabilityController)
4. Potvrda i slanje

**Što trebaš naučiti:**
- `useState` za praćenje koraka wizarda
- Kako prikazati kalendar (biblioteka: **react-datepicker**)
- Kako slati POST request s Axiosom

**Stranice:**
- `/:slug/booking`

---

## Faza 5: Klijent dashboard (~4-6h)

**Stranice:**
- `/dashboard` - moji appointmenti (lista s statusima)
- Otkazivanje appointmenta

---

## Faza 6: Tenant dashboard (~8-12h)

**Stranice:**
- `/dashboard/tenant` - pending appointmenti (potvrdi/odbij)
- `/dashboard/tenant/services` - pregled servisa, dodavanje, brisanje
- `/dashboard/tenant/working-hours` - postavljanje radnog vremena

---

## Procjena vremena

| Faza | Opis | Procjena |
|------|------|----------|
| 1 | Setup i osnove | 4-8h |
| 2 | Routing i autentifikacija | 6-10h |
| 3 | Javne stranice | 4-6h |
| 4 | Booking flow | 10-16h |
| 5 | Klijent dashboard | 4-6h |
| 6 | Tenant dashboard | 8-12h |
| **Ukupno** | | **~36-58h** |

Realno: **3 do 6 tjedana**, ovisno o tempu rada.

Prve dvije faze su sporije jer se uči novi React mindset (drugačiji od C#). Nakon toga tempo se ubrzava.
