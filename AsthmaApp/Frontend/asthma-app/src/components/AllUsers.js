import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import Pagination from "./Pagination";
import DataTable from "./DataTable";
import { jwtDecode } from "jwt-decode";
import Modal from "./Modal";
import Message from "./Message";

function AllUsers() {
  const [users, setUsers] = useState([]);
  const [currentPage, setCurrentPage] = useState(0);
  const [pageSize] = useState(6);
  const [pageCount, setPageCount] = useState(0);
  const [role, setRole] = useState(null);
  const [loggedInUserId, setLoggedInUserId] = useState(null);
  const navigate = useNavigate();
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedId, setSelectedId] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) return;

    const decoded = jwtDecode(token);
    setRole(decoded?.role); 
    setLoggedInUserId(decoded?.nameid); 
  }, []);

  useEffect(() => {
    if (role && loggedInUserId !== null) {
      if (role === "Admin") {
        getUsers();
      } else {
        getPatients();
      }
    }
  }, [role, loggedInUserId, currentPage]);

  async function getUsers(){
    const token = localStorage.getItem("token");
    axios
      .all([
        axios.get(`https://localhost:7138/api/User`, {
          headers: { Authorization: `Bearer ${token}` },
          params: { 
            isApproved: true,
            pageNumber: currentPage + 1,
            pageSize
          },
        }),
        axios.get(`https://localhost:7138/api/User/count-users`, {
          headers: { Authorization: `Bearer ${token}` },
          params: { isApproved: true },
        }),
      ])
      .then(([userRes, countRes]) => {
        setUsers(userRes.data);
        const total = countRes.data;
        setPageCount(Math.ceil(total / pageSize));
      })
      .catch((err) => console.error("Failed to fetch users", err));
  }

  async function getPatients(){
    const token = localStorage.getItem("token");
    axios
      .all([
        axios.get(`https://localhost:7138/api/User/patients/${loggedInUserId}`, {
          headers: { Authorization: `Bearer ${token}` },
          params: { pageNumber: currentPage + 1, pageSize },
        }),
        axios.get(`https://localhost:7138/api/User/count-users`, {
          headers: { Authorization: `Bearer ${token}` },
          params: { doctorId: loggedInUserId },
        }),
      ])
      .then(([userRes, countRes]) => {
        setUsers(userRes.data);
        const total = countRes.data;
        setPageCount(Math.ceil(total / pageSize));
      })
      .catch((err) => console.error("Failed to fetch patients", err));
  }

  const handlePageClick = (data) => {
    setCurrentPage(data.selected);
  };

  const handleModal = (id) => {
    setSelectedId(id);
    setModalOpen(true);
  }

  const handleDelete = async () => {
    const token = localStorage.getItem("token");

    try {
      if (role === "Doctor") {
        await axios.put(`https://localhost:7138/api/User`,
          {
            id: selectedId,
            doctorId: null,
            isDoctorEdited: true,
          },
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        setUsers((prev) => prev.filter((user) => user.id !== selectedId));
      } else if (role === "Admin") {
        await axios.put(`https://localhost:7138/api/User/${selectedId}`,
          {},
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        setUsers((prev) =>
          prev.map((user) =>
            user.id === selectedId ? { ...user, isActive: false } : user
          )
        );
      }
    } catch (err) {
      console.error("Failed to update user", err);
    } finally {
      setModalOpen(false);
      setSelectedId(null);
    }
  };

  const handleView = (id) => {
    navigate(`/user/${id}`);
  };

  const modalTitle = role === "Doctor" ? "Remove Patient" : "Deactivate User";
  const modalMessage =
    role === "Doctor"
      ? "Are you sure you want to remove this patient from your list?"
      : "Are you sure you want to deactivate this user?";

  return (
    <div className="all-users">
      {role === "Admin" ? <h2>All Users</h2> : <h2>All Patients</h2>}
      {users.length > 0 ? (
        <>
          <DataTable
            data={users}
            onView={handleView}
            onDelete={handleModal}
            isRecord={false}
          />
          <Pagination pageCount={pageCount} onPageChange={handlePageClick} />
        </>
      ) : (
        <Message
          text={
            role === "Admin"
              ? "No users found."
              : "No patients found."
          }
          type="info"
        />
      )}

      <Modal 
        isOpen={modalOpen}
        title={modalTitle}
        message={modalMessage}
        onConfirm={handleDelete}
        onCancel={() => setModalOpen(false)}
      />
    </div>
  );
}

export default AllUsers;
