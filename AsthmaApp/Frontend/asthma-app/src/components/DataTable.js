import React from "react";
import DataTableRow from "./DataTableRow";
import "../css_files/UsersTable.css";

function DataTable({
  data,
  onView,
  onDelete,
  isRecord = false,
  addPatient = false,
  onAddPatient = null,
  added = false, 
  onApprove = null,
}) {

  const userColumns = [
    { name: "OIB", key: "oib" },
    { name: "First Name", key: "firstName" },
    { name: "Last Name", key: "lastName" },
    { name: "Active", key: "isActive" },
  ];

  const recordColumns = [
    { name: "Date Created", key: "dateCreated" },
    { name: "Approved", key: "isApproved" },
    { name: "FVC", key: "fvc" },
    { name: "FEV1", key: "feV1" },
  ];

  const columns = isRecord ? recordColumns : userColumns;

  return (
    <table className="users-table">
      <thead>
        <tr>
          {columns.map((col) => (<th key={col.key}>{col.name}</th>))}
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        {data.map((item) => (
          <DataTableRow
            key={item.id}
            item={item}
            columns={columns}
            onView={onView}
            onDelete={onDelete}
            isRecord={isRecord}
            addPatient={addPatient}
            onAddPatient={onAddPatient}
            added={added}
            onApprove={onApprove}
          />
        ))}
      </tbody>
    </table>
  );
}

export default DataTable;
