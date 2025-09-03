import React, { useEffect, useState } from "react";
import axios from "axios";
import { jwtDecode } from "jwt-decode";
import "../css_files/Profile.css";
import { useParams } from "react-router-dom";
import Button from "./Button";
import TextInput from "./TextInput";
import Dropdown from "./Dropdown";
import RecordsTable from "./RecordsTable";

function Profile() {
  const { userId } = useParams();
  const [user, setUser] = useState();
  const [ethnicities, setEthnicities] = useState([]);
  const [educationLevels, setEducationLevels] = useState([]);
  const [editing, setEditing] = useState(false);
  const [form, setForm] = useState({});
  const [loggedInUserId, setLoggedInUserId] = useState(null);
  const [loggedInUserRole, setLoggedInUserRole] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");

    if (token) {
      try {
        const decoded = jwtDecode(token);
        const loggedId = decoded?.nameid;
        const role = decoded?.role;
        setLoggedInUserId(loggedId);
        setLoggedInUserRole(role);

        const idToFetch = userId || loggedId;

        const userPromise = axios.get(`https://localhost:7138/api/User/${idToFetch}`, {
          headers: { Authorization: `Bearer ${token}` },
        });

        const ethnicityPromise = !userId 
        ? axios.get(`https://localhost:7138/api/Ethnicity`, {
            headers: { Authorization: `Bearer ${token}` },
          })
        : Promise.resolve({ data: [] });

        const educationPromise = !userId
        ? axios.get(`https://localhost:7138/api/EducationLevel`, {
            headers: { Authorization: `Bearer ${token}` },
          })
        : Promise.resolve({ data: [] });

        axios
          .all([userPromise, ethnicityPromise, educationPromise])
          .then(([userRes, ethRes, eduRes]) => {
            setUser(userRes.data);
            setForm({
              firstName: userRes.data.firstName || "",
              lastName: userRes.data.lastName || "",
              dateOfBirth: userRes.data.dateOfBirth?.split("T")[0] || null,
              ethnicityId: userRes.data.ethnicityId || "",
              educationLevelId: userRes.data.educationLevelId || "",
              gender: userRes.data.gender || "",
            });
            setEthnicities(ethRes.data);
            setEducationLevels(eduRes.data);
          });
      } catch (error) {
        console.error("Invalid token.");
        localStorage.removeItem("token");
      }
    }
  }, [userId]);

  const isOwnProfile = userId === undefined || userId === loggedInUserId;
  const isDoctorLoggedIn = userId && userId !== loggedInUserId && loggedInUserRole === "Doctor";

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({...prev, [name]: value === "" ? null : value}));
  };

  const handleSave = async () => {
    const token = localStorage.getItem("token");
    const updatedUser = {
        id: user.id,
        firstName: form.firstName,
        lastName: form.lastName,
        dateOfBirth: form.dateOfBirth,
        ethnicityId: form.ethnicityId,
        educationLevelId: form.educationLevelId,
        gender: form.gender
    };

    try {
      await axios.put(`https://localhost:7138/api/User`, 
        updatedUser,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );
      setEditing(false);
      window.location.reload()
    } catch (err) {
      console.error("Update failed", err);
    }
  };

  if (!user) {
    return (
      <div className="profile-page loading">
        <div className="spinner"></div>
      </div>
    );
  }

  return (
    <div className="profile-page">
      <h2>{isOwnProfile ? "My Profile" : "User Profile"}</h2>
      <div className="profile-fields">
        {editing ? (
          <TextInput label="First Name" name="firstName" value={form.firstName} onChange={handleChange} />
        ) : (
          <Field label="First Name" value={user.firstName} />
        )}
        {editing ? (
          <TextInput label="Last Name" name="lastName" value={form.lastName} onChange={handleChange} />
        ) : (
          <Field label="Last Name" value={user.lastName} />
        )}
        <Field label="OIB" value={user.oib} />
        <Field
          label="Date of Birth"
          value={
            editing
              ? form.dateOfBirth || ""
              : user.dateOfBirth
              ? new Date(user.dateOfBirth).toLocaleDateString()
              : ""
          }
          editable={editing}
          name="dateOfBirth"
          onChange={handleChange}
          type="date"
        />
        <Field label="Email" value={user.email} />
        {!user.gender && editing ? (
            <Dropdown
              label="Gender"
              name="gender"
              value={form.gender}
              onChange={handleChange}
              options={["Male", "Female"]}
            />
            ) : (
            <Field label="Gender" value={user.gender} />
        )}
        {!user.ethnicityId && editing ? (
            <Dropdown
              label="Ethnicity"
              name="ethnicityId"
              value={form.ethnicityId}
              onChange={handleChange}
              options={ethnicities}
            />
            ) : (
            <Field label="Ethnicity" value={user.ethnicity?.name ?? ""} />
        )}
        {editing ? (
          <Dropdown
            label="Education Level"
            name="educationLevelId"
            value={form.educationLevelId}
            onChange={handleChange}
            options={educationLevels}
          />
        ) : (
          <Field label="Education Level" value={user.educationLevel?.name ?? ""} />
        )}
        {!isDoctorLoggedIn ? (
          <Field
            label="Doctor"
            value={`${user.doctor.firstName ?? ""} ${user.doctor.lastName ?? ""}`.trim()}
          />
        ) : null
        } 
        
      </div>

      {isOwnProfile && (
        <div className="profile-actions">
          {editing ? (
            <>
              <Button text="Save" onClick={handleSave} />
              <Button text="Cancel" onClick={() => setEditing(false)} className="cancel-button" />
            </>
          ) : (
            <Button text="Edit Profile" onClick={() => setEditing(true)} />
          )}
        </div>
      )}

      {isDoctorLoggedIn && (
        <div className="profile-records">
          <h3>Patient Records</h3>
          <RecordsTable userId={userId} />
        </div>
      )}
      
    </div>
  );
}

const Field = ({ label, value, editable, name, onChange, type = "text" }) => {
  return (
    <div className="profile-field">
      <span className="label">{label}:</span>
      {editable ? (
        <input type={type} name={name} value={value} onChange={onChange} />
      ) : (
        <span className="value">{value ?? ""}</span>
      )}
    </div>
  );
};

export default Profile;
