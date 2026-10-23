# Lexia web app

React 19 + TypeScript + Vite + Tailwind CSS, with TanStack Query for server state and React Router.

```bash
npm install
npm run dev        # http://localhost:5173, talks to the gateway at VITE_API_BASE_URL
npm run lint && npm run typecheck && npm test && npm run build
```

Configuration lives in `.env.development` (see `.env.example`). `VITE_AUTH_MODE=dev` signs in as a local author; `oidc` uses Keycloak.

Screens never call `fetch` directly: all requests go through `src/services/api`, whose types mirror the .NET DTOs. Change both sides together.
