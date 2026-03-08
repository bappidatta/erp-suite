import { useAuth } from "@shared/auth/auth-context";

export function DashboardPage() {
  const { auth, logout } = useAuth();

  return (
    <main className="dashboard-shell">
      <header className="dashboard-header">
        <div>
          <h1>ERP Dashboard (MVP)</h1>
          <p>Welcome, {auth?.user.fullName} ({auth?.user.email})</p>
        </div>
        <button onClick={logout}>Logout</button>
      </header>

      <section className="dashboard-grid">
        <article>
          <h2>Admin</h2>
          <p>User and organization setup is now ready for Sprint 0.2.</p>
        </article>
        <article>
          <h2>Finance</h2>
          <p>GL/AP/AR modules are scaffolded and pending feature implementation.</p>
        </article>
        <article>
          <h2>Procurement</h2>
          <p>Requisition and PO workflows will be added in upcoming sprints.</p>
        </article>
      </section>
    </main>
  );
}
