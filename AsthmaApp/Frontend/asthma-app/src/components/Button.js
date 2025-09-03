import React from "react";
import "../css_files/Button.css";

function Button({ text, onClick, className = "", type = "button", disabled = false }) {
  return (
    <button
      type={type}
      onClick={onClick}
      className={`custom-button ${className}`}
      disabled={disabled}
    >
      {text}
    </button>
  );
}

export default Button;
