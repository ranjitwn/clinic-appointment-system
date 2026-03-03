import Navbar from "../layout/Header";
import Footer from "../layout/Footer";

export default function Layout({ children }: { children: React.ReactNode }) {
  return (
    <div className="app-container">
      <Navbar />
      <main className="main-content">{children}</main>
      <Footer />
    </div>
  );
}
