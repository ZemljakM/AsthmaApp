import React from "react";
import "../css_files/Message.css";

function Message({ text, type = "info", inline = false }) {
  const baseClass = inline ? "message-inline" : "message";
  return <div className={`${baseClass} message-${type}`}>{text}</div>;
}


export default Message;
