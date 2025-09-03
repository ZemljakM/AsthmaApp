import React from "react";
import "../css_files/FormControls.css";

function Toggle({ label, name, checked, onChange }) {
  return (
    <div className="profile-field">
      <span className="label">{label}:</span>
      <label className="switch">
        <input
          type="checkbox"
          name={name}
          checked={checked}
          onChange={onChange}
        />
        <span className="slider round"></span>
      </label>
    </div>
  );
}

export default Toggle;
