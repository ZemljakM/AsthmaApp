import React, { useEffect, useState } from "react";
import axios from "axios";
import DataTable from "./DataTable";
import Pagination from "./Pagination";
import { useNavigate } from "react-router-dom";
import Message from "./Message";

function DoctorRequests() {
  const [doctors, setDoctors] = useState([]);
  const [currentPage, setCurrentPage] = useState(0);
  const [pageSize] = useState(6);
  const [pageCount, setPageCount] = useState(0);
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) return;

    axios
      .all([
        axios.get(`https://localhost:7138/api/User`, {
          headers: { Authorization: `Bearer ${token}` },
          params: { 
            isApproved: false,
            pageNumber: currentPage + 1 
          },
        }),
        axios.get(`https://localhost:7138/api/User/count-users`, {
          headers: { Authorization: `Bearer ${token}` },
          params: { isApproved: false },
        }),
      ])
      .then(([doctorRes, countRes]) => {
        setDoctors(doctorRes.data);
        setPageCount(Math.ceil(countRes.data / pageSize));
      })
      .catch((err) => console.error("Failed to fetch doctor requests", err));
  }, [currentPage]);

  const handlePageClick = (data) => {
    setCurrentPage(data.selected);
  };

    const handleApprove = async (userId) => {
        const token = localStorage.getItem("token");

        const updatedUser = {
            id: userId,
            isApproved: true, 
        };

        try {
            await axios.put(
            `https://localhost:7138/api/User`,
            updatedUser,
            {
                headers: { Authorization: `Bearer ${token}` },
            }
            );
            setDoctors(prev => prev.filter(u => u.id !== userId));
        } catch (err) {
            console.error("Failed to approve user:", err);
            alert("Approval failed.");
        }
        };

  const handleView = (id) => {
    navigate(`/user/${id}`);
  };

  const handleDelete = async (userId) => {
  const token = localStorage.getItem("token");

  try {
    await axios.delete(`https://localhost:7138/api/User/${userId}`, {
      headers: { Authorization: `Bearer ${token}` },
    });

    setDoctors((prev) => prev.filter((user) => user.id !== userId));
  } catch (err) {
    console.error("Failed to delete user:", err);
    alert("Delete failed.");
  }
};


  return (
    <div className="doctor-requests">
      <h2>New Doctor Requests</h2>
      {doctors.length > 0 ? (
        <>
          <DataTable
            data={doctors}
            onView={handleView}
            isRecord={false}
            onApprove={handleApprove}
            onDelete={handleDelete}
          />
          <Pagination pageCount={pageCount} onPageChange={handlePageClick} />
        </>
      ) : (
        <Message text="No new doctor requests found." type="info" />
      )}
    </div>
  );
}

export default DoctorRequests;
