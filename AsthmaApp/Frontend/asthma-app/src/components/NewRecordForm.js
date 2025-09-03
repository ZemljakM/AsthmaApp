import React, { useState } from "react";
import "../css_files/NewRecordForm.css";
import { useNavigate } from "react-router-dom";
import TextInput from "./TextInput";
import Toggle from "./Toggle";
import Slider from "./Slider";
import axios from "axios";
import Button from "./Button";

function NewRecordForm() {
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({
    weight: 70.0,
    height: 1.80,
    smoking: false,
    familyHistoryOfAsthma: false,
    historyOfAllergies: false,
    eczema: false,
    hayFever: false,
    gastroesophagealReflux: false,
    petAllergy: false,
    wheezing: false,
    shortnessOfBreath: false,
    chestTightness: false,
    coughing: false,
    nighttimeSymptoms: false,
    exerciseInduced: false,
    city: "",
    country: "",
    dietQuality: 5,
    sleepQuality: 7,
    physicalActivity: 5,
    dustExposure: 5
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    const requiredFields = ["weight", "height", "city", "country"];
    const isEmpty = requiredFields.some((field) => !form[field]);

    if (isEmpty) {
      setError("All fields must be filled.");
      return;
    }

    const token = localStorage.getItem("token");
    if (!token) {
      setError("Authentication token missing.");
      return;
    }

    setLoading(true);

    try {
      await axios.get(`https://localhost:7138/api/User/check-profile`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      const response = await axios.post(`https://localhost:7138/api/Record`,
        form,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      if (response.status === 200 || response.status === 201) {
        setSuccess("Record submitted successfully!");
        setTimeout(() => navigate("/records"), 1500);
      }
    } catch (err) {
      if (err.response?.status === 400) {
        const msg =
          err.response.data?.includes("Profile not completed.")
            ? "Your profile must be completed first."
            : "Something went wrong. Check all values and try again.";
        setError(msg);
      } else {
        setError("Unexpected error occurred.");
      }
    } finally {
      setLoading(false); 
    }
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value
    }));
  };

  const handleSlider = (name, value) => {
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  return (
    <div className="new-record">
      <h2>New Health Record</h2>
      <form>
        <TextInput label="Weight (kg)" name="weight" type="number" value={form.weight} step="1" onChange={handleChange} />
        <TextInput label="Height (m)" name="height" type="number" value={form.height} step="0.01" onChange={handleChange} />
        <TextInput label="City" name="city" value={form.city} onChange={handleChange} />
        <TextInput label="Country" name="country" value={form.country} onChange={handleChange} />

        <h4>Symptoms and History</h4>
        {[
          "smoking",
          "familyHistoryOfAsthma",
          "historyOfAllergies",
          "eczema",
          "hayFever",
          "gastroesophagealReflux",
          "petAllergy",
          "wheezing",
          "shortnessOfBreath",
          "chestTightness",
          "coughing",
          "nighttimeSymptoms",
          "exerciseInduced"
        ].map((key) => (
          <Toggle
            key={key}
            label={key.replace(/([A-Z])/g, " $1").replace(/^./, str => str.toUpperCase())}
            name={key}
            checked={form[key]}
            onChange={handleChange}
          />
        ))}

        <h4>Lifestyle</h4>
        <Slider
          label="Diet Quality"
          name="dietQuality"
          value={form.dietQuality}
          onChange={handleSlider}
        />

        <Slider
          label="Sleep Quality"
          name="sleepQuality"
          value={form.sleepQuality}
          min={4}
          max={10}
          step={0.1}
          onChange={handleSlider}
        />

        <Slider
          label="Physical Activity"
          name="physicalActivity"
          value={form.physicalActivity}
          onChange={handleSlider}
        />

        <Slider
          label="Dust Exposure"
          name="dustExposure"
          value={form.dustExposure}
          onChange={handleSlider}
        />
        {error && <div className="error-message">{error}</div>}
        {success && <div className="success-message">{success}</div>}
        <div style={{ marginTop: "20px" }}>
          <Button
            text={loading ? "Submitting..." : "Submit Record"}
            onClick={handleSubmit}
            className="submit-button"
            disabled={loading} 
          />
        </div>
      </form>
    </div>
  );
}

export default NewRecordForm;