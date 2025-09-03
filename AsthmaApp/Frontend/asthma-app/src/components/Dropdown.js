import React from "react";
import "../css_files/FormControls.css";

function Dropdown({ label, name, value, onChange, options, placeholder = "Select...", className = "", selectClassName = "" }) {
  return (
    <div className={className || "profile-field"}>
      {label && <span className="label">{label}:</span>}
      <select
        name={name}
        value={value}
        onChange={onChange}
        className={selectClassName || "inline-input"}
      >
        <option value="">{placeholder}</option>
        {options.map((opt) => (
          <option key={opt.id ?? opt} value={opt.id ?? opt}>
            {opt.name ?? opt}
          </option>
        ))}
      </select>
    </div>
  );
}

export default Dropdown;
