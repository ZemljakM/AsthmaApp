import React from "react";
import "../css_files/FormControls.css";

function Slider({ label, name, value, onChange, min = 0, max = 10, step = 0.1 }) {
  return (
    <div className="profile-field slider-container">
      <span className="label">{label}:</span>
      <div className="slider-column">
        <input
          type="range"
          name={name}
          min={min}
          max={max}
          step={step}
          value={value}
          onChange={(e) => onChange(name, e.target.value)}
          className="custom-slider"
        />
        <div className="slider-labels">
          <span>Poor</span>
          <span>Moderate</span>
          <span>Excellent</span>
        </div>
        <div className="slider-value">Current: {parseFloat(value).toFixed(1)}</div>
      </div>
    </div>
  );
}

export default Slider;
