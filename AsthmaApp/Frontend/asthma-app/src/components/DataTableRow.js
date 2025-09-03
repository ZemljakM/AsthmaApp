import React from "react";
import Button from "./Button";

function DataTableRow({
  item,
  columns,
  onView,
  onDelete,
  isRecord,
  addPatient,
  onAddPatient,
  added,
  onApprove,
}) {
  const renderValue = (columnKey, value) => {
    if (columnKey === "isApproved") {
      return value ? "✔" : "X";
    }

    if (columnKey === "isActive") {
      return value === null || value === undefined ? "-" : value ? "Yes" : "No";
    }

    if (columnKey === "fev1" || columnKey === "fvc") {
      return value == null || value === 0 ? "-" : value;
    }

    if (columnKey === "dateCreated" && value) {
      return new Date(value).toLocaleDateString();
    }

    return value ?? "-";
  };

  return (
    <tr>
      {columns.map((col) => (
        <td key={col.key}>{renderValue(col.key, item[col.key])}</td>
      ))}
      <td>
        {addPatient ? (
          <Button
            text={added ? "Patient Added" : "Add Patient"}
            onClick={() => onAddPatient(item.id)}
            className="add-patient-btn"
            disabled={added}
          />
        ) : (
          <>
            <Button
              text="View"
              onClick={() => onView(item.id)}
              className="view-btn"
            />
            {!isRecord && item.isActive && onApprove && !item.isApproved && (
              <Button
                text="Approve"
                onClick={() => onApprove(item.id)}
                className="approve-btn"
              />
            )}
            {!isRecord && item.isActive && (
              <Button
                text="Delete"
                onClick={() => onDelete(item.id)}
                className="delete-btn"
              />
            )}
          </>
        )}
      </td>
    </tr>
  );
}

export default DataTableRow;
