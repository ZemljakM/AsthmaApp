import Sidebar from "./Sidebar"; 
import React, { useEffect } from "react";
import { useNavigate, Outlet } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import "../css_files/Dashboard.css";

const Dashboard = ({ user }) => {
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) return;

    const decoded = jwtDecode(token);
    const role = decoded?.role;

    if (role === "Admin") {
      navigate("/all-users");
    } else if (role === "Doctor") {
      navigate("/patients");
    } else if (role === "Patient") {
      navigate("/records");
    }
  }, [navigate]);

  return (
    <div className="dashboard-layout">
      <Sidebar user={user} />
      <div className="dashboard-content">
        <Outlet />
      </div>
    </div>
  );
};

export default Dashboard;
