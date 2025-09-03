import React, { useEffect, useState } from "react";
import axios from "axios";
import DataTable from "./DataTable";
import Pagination from "./Pagination";
import { useNavigate } from "react-router-dom";
import Message from "./Message";

function RecordsTable({ userId }) {
  const [records, setRecords] = useState([]);
  const [currentPage, setCurrentPage] = useState(0);
  const [pageSize] = useState(6);
  const [pageCount, setPageCount] = useState(0);
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) return;

    axios
      .all([
        axios.get(`https://localhost:7138/api/Record`, {
          headers: { Authorization: `Bearer ${token}` },
          params: {
            userId: userId,
            pageNumber: currentPage + 1,
            pageSize: pageSize
          },
        }),
        axios.get(`https://localhost:7138/api/Record/count-records`, {
          headers: { Authorization: `Bearer ${token}` },
          params: { userId: userId },
        }),
      ])
      .then(([recordsRes, countRes]) => {
        setRecords(recordsRes.data);
        const total = countRes.data;
        setPageCount(Math.ceil(total / pageSize));
      })
      .catch((err) => console.error("Failed to fetch records", err));
  }, [currentPage, userId]);

  const handlePageClick = (data) => {
    setCurrentPage(data.selected);
  };

  const handleView = (id) => {
    navigate(`/record/${id}`);
  };

  const formatValue = (value) => {
    return value === null || value === 0 ? "-" : value;
  };

  return (
    <div>
      {records.length > 0 ? (
        <>
          <DataTable
            data={records.map(r => ({
              ...r,
              feV1: formatValue(r.feV1),
              fvc: formatValue(r.fvc)
            }))}
            onView={handleView}
            isRecord={true}
            columns={[
              { name: "Date Created", key: "dateCreated" },
              { name: "Approved", key: "isApproved" },
              { name: "FEV1", key: "feV1" },
              { name: "FVC", key: "fvc" },
            ]}
          />
          <Pagination pageCount={pageCount} onPageChange={handlePageClick} />
        </>
      ) : (
        <Message text="No records found." type="info" />
      )}
      
    </div>
  );
}

export default RecordsTable;
