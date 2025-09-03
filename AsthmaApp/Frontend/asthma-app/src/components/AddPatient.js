import React, { useState, useEffect } from "react";
import axios from "axios";
import DataTable from "./DataTable"; 
import "../css_files/AddPatient.css";
import { jwtDecode } from "jwt-decode";
import Message from "./Message";
import Button from "./Button";

function AddPatient() {
  const [oib, setOib] = useState("");
  const [patient, setPatient] = useState(null);
  const [loggedInDoctorId, setLoggedInDoctorId] = useState(null); 
  const [token, setToken] = useState(null);
  const [message, setMessage] = useState("");
  const [patientAdded, setPatientAdded] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem("token");
    setToken(token);
    if (token) {
      const decodedToken = jwtDecode(token);
      setLoggedInDoctorId(decodedToken.nameid);
    }
  }, []);

  const handleOibChange = (e) => {
    setOib(e.target.value);
  };

  const handleSearchPatient = async () => {
    setPatient(null);
    setMessage("");
    setPatientAdded(false);

    try {
      const response = await axios.get(`https://localhost:7138/api/User/patient/${oib}`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      if (response.data.id === loggedInDoctorId) {
        setMessage("You can't add yourself as a patient.");
        return;
      }

      if (response.data) {
        if (response.data.doctorId !== null){
          setMessage("This patient is already assigned to another doctor and can't be added again.");
        }
        else {
          setPatient(response.data);
        }
      } else {
        setMessage("No patient found with entered OIB.");
      }
    } catch (error) {
      setMessage("Something went wrong. Please try again.");
    }
  };

  const handleAddPatient = async () => {
    try {
      const token = localStorage.getItem("token");

      const updatedPatientData = {
        id: patient.id,  
        doctorId: loggedInDoctorId, 
      };


      await axios.put(`https://localhost:7138/api/User`,
        updatedPatientData,
        { 
          headers: { Authorization: `Bearer ${token}` } 
        }
      );

      setPatientAdded(true);
    } catch (error) {
      console.error("Error adding patient", error);
    }
  };

  return (
    <>
      <h2>Search and Add Patient</h2>

      <div>
        <input
          type="text"
          placeholder="Enter OIB"
          value={oib}
          onChange={handleOibChange}
        />
        <Button
          text="Search"
          onClick={handleSearchPatient}
          className="custom-button" 
        />
      </div>

      {message && <Message text={message} type="info" />}

      {patient && (
        <div>
          <DataTable
            data={[patient]}
            showActions={false}
            addPatient={true}
            onAddPatient={handleAddPatient}
            added={patientAdded}
          />
        </div>
      )}
    </>
  );
}

export default AddPatient;
