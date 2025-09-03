import React, { useEffect } from "react";
import RecordsTable from "./RecordsTable";
import { jwtDecode } from "jwt-decode";

function RecordsPage() {
  const token = localStorage.getItem("token");
  const decoded = jwtDecode(token);
  const idToFetch = decoded?.nameid;

  useEffect(() => {
    if (!token) return;
  }, []);

  return (
    <div className="all-records">
      <h2>Your Health Records</h2>
      <RecordsTable userId={idToFetch} />
    </div>
  );
}

export default RecordsPage;
