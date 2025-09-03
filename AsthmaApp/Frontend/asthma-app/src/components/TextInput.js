import React from "react";
import "../css_files/FormControls.css";

function TextInput({ label, name, value, onChange, type = "text", placeholder = "" }) {
  return (
    <div className="profile-field">
      <span className="label">{label}:</span>
      <input
        type={type}
        name={name}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        className="inline-input"
      />
    </div>
  );
}

export default TextInput;
