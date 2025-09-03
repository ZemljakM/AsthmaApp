import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate, Link } from 'react-router-dom';
import '../css_files/LoginStyle.css';
import Dropdown from './Dropdown';
import Message from './Message';

function RegisterForm() {
  const [credentials, setCredentials] = useState({});
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [info, setInfo] = useState('');
  const navigate = useNavigate();

  const roles = ["Patient", "Doctor"];

  const handleChange = (e) => {
    setCredentials({ ...credentials, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setInfo('');

    if (credentials.password !== credentials.confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    if (!/^\d{11}$/.test(credentials.oib)) {
      setError('OIB must be exactly 11 digits');
      return;
    }

    if (!credentials.roleName) {
      setError('Please select a role');
      return;
    }

    try {
      const response = await axios.post('https://localhost:7138/api/User/register', credentials);
      const token = response.data.token;

      if (token) {
        localStorage.setItem('token', token);
        navigate('/');
      } else {
        setCredentials({});
        setInfo('Registration successful. Your account requires admin approval before login.');
      }
    } catch (err) {
      console.error(err);
      setError('Registration failed. Try again.');
    }
  };

  return (
    <div className="bg-img">
      <div className="content wide">
        <header>Sign Up</header>
        <form onSubmit={handleSubmit}>
          <div className="field-row">
            <div className="field">
              <input
                type="text"
                name="firstName"
                placeholder="First Name"
                value={credentials.firstName || ''}
                onChange={handleChange}
                required
              />
            </div>
            <div className="field">
              <input
                type="text"
                name="lastName"
                placeholder="Last Name"
                value={credentials.lastName || ''}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <div className="field-row">
            <div className="field">
              <input
                type="email"
                name="email"
                placeholder="Email"
                value={credentials.email || ''}
                onChange={handleChange}
                required
              />
            </div>
            <div className="field">
              <input
                type="text"
                name="oib"
                placeholder="OIB"
                value={credentials.oib || ''}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <div className="field-row">
            <div className="field">
              <input
                type={showPassword ? 'text' : 'password'}
                name="password"
                className="pass-key"
                placeholder="Password"
                value={credentials.password || ''}
                onChange={handleChange}
                required
              />
            </div>
            <div className="field">
              <input
                type={showPassword ? 'text' : 'password'}
                name="confirmPassword"
                className="pass-key"
                placeholder="Confirm Password"
                value={credentials.confirmPassword || ''}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <Dropdown
            name="roleName"
            value={credentials.roleName || ''}
            onChange={handleChange}
            options={roles}
            placeholder='Select Role'
            className="auth-dropdown"
            selectClassName="auth-select"
          />

          {error && <Message text={error} type="error" inline />}
          {info && <Message text={info} type="info" inline />}

          <div className="field type--disabled">
            <input type="submit" value="SIGN UP" />
          </div>
        </form>

        <div className="signup">
          Already have an account? <Link className='link' to="/login">Log In</Link>
        </div>
      </div>
    </div>
  );
}

export default RegisterForm;
