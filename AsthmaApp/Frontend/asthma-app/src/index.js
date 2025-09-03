import React from 'react';
import ReactDOM from 'react-dom/client';
import './css_files/index.css';
import reportWebVitals from './reportWebVitals';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import LoginForm from './components/LoginForm.js'; 
import RegisterForm from './components/RegisterForm.js';
import Dashboard from './components/Dashboard.js';
import Profile from './components/Profile.js';
import ProtectedRoute from './components/ProtectedRoute.js';
import AllUsers from "./components/AllUsers.js";
import NewRecordForm from './components/NewRecordForm.js';
import RecordsPage from './components/RecordsPage.js';
import Record from './components/Record.js';
import AddPatient from './components/AddPatient.js';
import DoctorRequests from './components/DoctorRequests.js';

const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginForm />,
  },
  {
    path: "/register",
    element: <RegisterForm />,
  },
  {
    path: "/",
    element: (
      <ProtectedRoute><Dashboard /></ProtectedRoute>
    ),
    children: [
      {
        path: "profile",
        element: <Profile />
      },
      {
        path: "/all-users",
        element: <AllUsers />
      },
      {
        path: "/user/:userId",
        element: <Profile />
      },
      {
        path: "/add-record",
        element: <NewRecordForm />
      },
      {
        path: "/records",
        element: <RecordsPage />
      },
      {
        path: "/record/:recordId",
        element: <Record />
      },
      {
        path: "add-patient",
        element: <AddPatient />
      },
      {
        path: "/patients",
        element: <AllUsers />
      },
      { 
        path: "/doctor-requests", 
        element: <DoctorRequests />
      },
    ]
  },
]);


const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
