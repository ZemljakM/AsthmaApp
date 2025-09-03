import React, { useState } from "react";
import "../css_files/Sidebar.css";
import { LogOut } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { FaUsers, FaUserPlus, FaUserMd, FaClipboardList, FaFileMedical } from "react-icons/fa";

function Sidebar({ user }) {
  const navigate = useNavigate();
  const [isOpen, setIsOpen] = useState(false);

  const navItems = {
    Patient: [
      { label: "Records", icon: FaClipboardList, path: "/records" },
      { label: "Add Record", icon: FaFileMedical, path: "/add-record" },
    ],
    Doctor: [
      { label: "Patients", icon: FaUsers, path: "/patients" },
      { label: "Add Patient", icon: FaUserPlus, path: "/add-patient" },
      { label: "Records", icon: FaClipboardList, path: "/records" },
      { label: "Add Record", icon: FaFileMedical, path: "/add-record" },
    ],
    Admin: [
      { label: "Users", icon: FaUsers, path: "/all-users" },
      { label: "Doctor Requests", icon: FaUserMd, path: "/doctor-requests" },
    ],
  };

  const handleLogout = () => {
    localStorage.removeItem("token");
    setIsOpen(false);
    navigate("/login");
  };

  const handleNavigate = (path) => {
    setIsOpen(false);
    navigate(path);
  };

  const items = navItems[user?.role] || [];
  const initial = (user?.displayName?.charAt(0) || "").toUpperCase();

  return (
    <>
      <button
        className="hamburger-btn"
        aria-label="Open menu"
        aria-expanded={isOpen}
        onClick={() => setIsOpen(true)}
      >
        ☰
      </button>

      {isOpen && <div className="sidebar-overlay" onClick={() => setIsOpen(false)} />}

      <div className={`sidebar ${isOpen ? "open" : ""}`}>
        <div className="sidebar__logo" onClick={() => handleNavigate("/")}>
          <img src="/logo.png" alt="Logo" className="sidebar__logo-image" />
        </div>

        <nav className="sidebar__nav">
          {items.map((item) => (
            <div
              key={item.label}
              className="sidebar__nav-item"
              onClick={() => handleNavigate(item.path)}
            >
              <item.icon className="sidebar__icon" />
              <span className="sidebar__label">{item.label}</span>
            </div>
          ))}
        </nav>

        <div className="sidebar__profile">
          <div className="sidebar__avatar" onClick={() => handleNavigate("/profile")}>
            {initial}
          </div>
          <div className="sidebar__userinfo" onClick={() => handleNavigate("/profile")}>
            <div className="sidebar__name">{user?.displayName || ""}</div>
            <div className="sidebar__role">{user?.role || ""}</div>
          </div>
          <button className="sidebar__logout" onClick={handleLogout} aria-label="Log out">
            <span className="logout-icon"><LogOut size={18} /></span>
          </button>
        </div>
      </div>
    </>
  );
}

export default Sidebar;
