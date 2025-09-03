import { Navigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';
import React from "react";

const ProtectedRoute = ({ children }) => {
  const token = localStorage.getItem('token');

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  try {
    const decoded = jwtDecode(token);
    const currentTime = Date.now() / 1000;

    if (decoded.exp < currentTime) {
      localStorage.removeItem('token');
      return <Navigate to="/login" replace />;
    }

    const user = {
      id: decoded.nameid,
      email: decoded.email,
      role: decoded.role, 
      displayName: decoded.unique_name
    };

    return React.cloneElement(children, {
      user,
      role: user.role,
    });
  } catch {
    localStorage.removeItem('token');
    return <Navigate to="/login" replace />;
  }
};

export default ProtectedRoute;
