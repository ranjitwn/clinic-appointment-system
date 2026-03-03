interface AuthCardProps {
  title: string;
  children: React.ReactNode;
}

export default function AuthCard({ title, children }: AuthCardProps) {
  return (
    <main>
      <div className="card auth-card">
        <h1>{title}</h1>
        {children}
      </div>
    </main>
  );
}
