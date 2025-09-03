import React, { useEffect, useState } from "react";
import axios from "axios";
import { useParams } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import Button from "./Button";
import "../css_files/Record.css";
import Message from "./Message";

function Record() {
  const { recordId } = useParams();
  const [record, setRecord] = useState(null);
  const [form, setForm] = useState({});
  const [isDoctor, setIsDoctor] = useState(false);
  const [message, setMessage] = useState({ text: "", type: "" });
  const [isUserRecord, setIsUserRecord] = useState();

  useEffect(() => {
    const token = localStorage.getItem("token");

    if (token) {
      const decoded = jwtDecode(token);
      setIsDoctor(decoded?.role === "Doctor");

      axios
        .get(`https://localhost:7138/api/Record/${recordId}`, {
          headers: { Authorization: `Bearer ${token}` },
        })
        .then((res) => {
          setRecord(res.data);
          setForm({
            ...res.data,
            diagnosis: res.data.diagnosis || "",
            recommendation: res.data.recommendation || "",
            fev1: res.data.feV1,
            fvc: res.data.fvc,
          });
          setIsUserRecord(res.data.userId !== decoded.nameid);
        })
        .catch((err) => console.error("Failed to fetch record", err));
    }
  }, [recordId]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const handleSave = async () => {
    const token = localStorage.getItem("token");
    try {
      await axios.put(
        `https://localhost:7138/api/Record`,
        {
          id: record.id,
          fev1: parseFloat(form.fev1) || null,
          fvc: parseFloat(form.fvc) || null,
        },
        { 
          headers: { Authorization: `Bearer ${token}` } 
        }
      );
      setMessage({ text: "FEV1/FVC updated successfully.", type: "success" });
    } catch (err) {
      console.error("Failed to update", err);
      setMessage({ text: "Failed to update.", type: "error" });
    }
  };

  const handleApprove = async () => {
    const token = localStorage.getItem("token");
    try {
      await axios.put(
        `https://localhost:7138/api/Record`,
        { id: record.id, recommendation: form.recommendation },
        { 
          headers: { Authorization: `Bearer ${token}` } 
        }
      );
      setForm((prev) => ({ ...prev, isApproved: true }));
      setMessage({ text: "Record approved.", type: "success" });
    } catch (err) {
      console.error("Failed to approve", err);
      setMessage({ text: "Approval failed.", type: "error" });
    }
  };

  if (!record) {
    return (
      <div className="record-page loading">
        <div className="spinner"></div>
      </div>
    );
  }

  const yn = (v) => (
    <span className={`pill ${v ? "pill-yes" : "pill-no"}`}>{v ? "Yes" : "No"}</span>
  );

  return (
    <div className="record-page">
      <h2>Record Details</h2>

      <div className="record-fields">
        <Field label="BMI" value={form.bmi} />
        <Field label="Smoking" value={yn(form.smoking)} raw />
        <Field label="Family History of Asthma" value={yn(form.familyHistoryOfAsthma)} raw />
        <Field label="History of Allergies" value={yn(form.historyOfAllergies)} raw />
        <Field label="Eczema" value={yn(form.eczema)} raw />
        <Field label="Hay Fever" value={yn(form.hayFever)} raw />
        <Field label="Gastroesophageal Reflux" value={yn(form.gastroesophagealReflux)} raw />
        <Field label="Pet Allergy" value={yn(form.petAllergy)} raw />
        <Field label="Wheezing" value={yn(form.wheezing)} raw />
        <Field label="Shortness of Breath" value={yn(form.shortnessOfBreath)} raw />
        <Field label="Chest Tightness" value={yn(form.chestTightness)} raw />
        <Field label="Coughing" value={yn(form.coughing)} raw />
        <Field label="Nighttime Symptoms" value={yn(form.nighttimeSymptoms)} raw />
        <Field label="Exercise Induced" value={yn(form.exerciseInduced)} raw />
        <Field label="Diet Quality" value={form.dietQuality} />
        <Field label="Sleep Quality" value={form.sleepQuality} />
        <Field label="Physical Activity" value={form.physicalActivity} />
        <Field label="Dust Exposure" value={form.dustExposure} />
        <Field label="Pollen Exposure" value={form.pollenExposure} />
        <Field label="Pollution Exposure" value={form.pollutionExposure} />
        <Field
          label="Location"
          value={
            form.location ? `${form.location?.city ?? "-"}, ${form.location?.country ?? "-"}` : "-"
          }
        />
        <Field label="Is Approved" value={yn(form.isApproved)} raw />
        <Field label="Date Created" value={new Date(form.dateCreated).toLocaleDateString()} />
      </div>

      {((isDoctor && isUserRecord) || form.isApproved) && (
        <div className="record-field full-width inline">
          <span className="kv">
            <span className="key">Prediction:</span>
            <span className="val">{form.diagnosis || "-"}</span>
          </span>
        </div>
      )}

      {((isDoctor && isUserRecord) || form.isApproved) && (
        <div className="record-field full-width">
          <label className="key block">Recommendation:</label>
          {isDoctor && !form.isApproved ? (
            <textarea
              name="recommendation"
              value={form.recommendation}
              onChange={handleChange}
              rows={4}
              className="rec-textarea"
            />
          ) : (
            <span className="val block">{form.recommendation || "-"}</span>
          )}
        </div>
      )}

      {(isDoctor && isUserRecord) && !form.isApproved && (
        <div className="record-actions left">
          <Button text="Approve" onClick={handleApprove} />
        </div>
      )}

      <div className="record-fields fev-section">
        <h3>FEV1 / FVC</h3>
        {isDoctor ? (
          <>
            <EditableField
              label="FEV1"
              name="fev1"
              value={form.fev1 !== null && form.fev1 !== undefined ? form.fev1 : ""}
              onChange={handleChange}
            />
            <EditableField
              label="FVC"
              name="fvc"
              value={form.fvc !== null && form.fvc !== undefined ? form.fvc : ""}
              onChange={handleChange}
            />
          </>
        ) : (
          <>
            <Field label="FEV1" value={form.fev1 != null && form.fev1 !== 0 ? form.fev1 : "-"} />
            <Field label="FVC" value={form.fvc != null && form.fvc !== 0 ? form.fvc : "-"} />
          </>
        )}
      </div>

      {(isDoctor && isUserRecord) && (
        <>
          <div className="record-actions left">
            <Button text="Save" onClick={handleSave} />
          </div>
          {message.text && (
            <Message text={message.text} type={message.type} />
          )}
        </>
      )}
    </div>
  );
}

const Field = ({ label, value, raw = false }) => (
  <div className="record-field inline">
    <span className="kv">
      <span className="key">{label}:</span>
      {raw ? <span className="val">{value}</span> : <span className="val">{value ?? "-"}</span>}
    </span>
  </div>
);

const EditableField = ({ label, name, value, onChange }) => (
  <div className="record-field inline">
    <span className="kv">
      <label className="key" htmlFor={name}>{label}:</label>
      <input
        id={name}
        type="number"
        name={name}
        value={value}
        onChange={onChange}
        step="0.01"
        placeholder="Enter value"
        className="num-input"
      />
    </span>
  </div>
);

export default Record;
