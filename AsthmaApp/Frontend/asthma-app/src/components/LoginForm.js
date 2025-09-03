import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate, Link } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';
import '../css_files/LoginStyle.css';
import { FaEye, FaEyeSlash } from "react-icons/fa";

function LoginForm() {
  const [credentials, setCredentials] = useState({});
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      try {
        const decoded = jwtDecode(token);
        const currentTime = Date.now() / 1000;
        if (decoded.exp > currentTime) {
          navigate("/"); 
        } else {
          localStorage.removeItem("token");
        }
      } catch {
        localStorage.removeItem("token");
      }
    }
  }, []);

  const handleChange = (e) => {
    setCredentials({...credentials, [e.target.name]: e.target.value});
  };

  const toggleShowPassword = () => {
    setShowPassword(prev => !prev);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const response = await axios.post('https://localhost:7138/api/User/login', credentials);

      const token = response.data.token;
      localStorage.setItem('token', token);

      navigate('/');

    } catch (err) {
      console.error(err);
      setError('Invalid email or password.');
    }
  };

  return (
    <div className="bg-img">
      <div className="content">
        <header>Log In</header>
        <form onSubmit={handleSubmit}>
          <div className="field">
            <span className="fa fa-user"></span>
            <input type="email" name="email" placeholder="Email" value={credentials.email || ''} onChange={handleChange} required/>
          </div>
          <div className="field space">
            <span className="fa fa-lock"></span>
            <input type={showPassword ? "text" : "password"} name="password" className="pass-key" placeholder="Password" value={credentials.password || ''} onChange={handleChange} required/>
            <span className="show" onClick={toggleShowPassword}>
              {showPassword ? <FaEye /> : <FaEyeSlash />}
            </span>
          </div>

          {error && <p style={{ color: 'red', marginTop: '10px' }}>{error}</p>}

          <div className="field type--disabled">
            <input type="submit" value="LOGIN" />
          </div>
        </form>

        <div className="signup">
          Don't have account? <Link className='link' to={"/register"}>Sign Up</Link>
        </div>
      </div>
    </div>
  );
}

export default LoginForm;
