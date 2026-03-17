interface AuthCardProps {
  title: string;
  children: React.ReactNode;
}

export default function AuthCard({ title, children }: AuthCardProps) {
  return (
    <div className="auth-page">
      <div className="card auth-card">
        <div className="auth-brand">
          <div className="auth-brand-icon">✚</div>
          <span className="auth-brand-name">ClinicCare</span>
        </div>
        <h1>{title}</h1>
        {children}
      </div>
    </div>
  );
}
