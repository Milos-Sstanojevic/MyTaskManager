import React from 'react';
import './Navbar.css';

const Navbar = () => {
  const token=localStorage.getItem('token');

    const handleLogOut = () => {
    localStorage.removeItem('token');
    window.location.href = "http://localhost:3000";
  };

  const handleAboutUsClicked = () => {
    document.getElementById("about-us")?.scrollIntoView({ behavior: "smooth" });
    console.log("About us Scroll");
  }

  const handleContactClicked = () => {
    document.getElementById("contact")?.scrollIntoView({ behavior: "smooth" });
    //kod za skrol
    console.log("Contact Scroll");
  }
  
  const handleLogIn = () => {
    document.getElementById("login")?.scrollIntoView({ behavior: "smooth" });
    //ovde ide kod za log in skrol
    console.log("login Scroll")
  }


    return (
        <nav className="navbar">
          <div className="logo-container">
            <img className="logo-img" src="https://img.freepik.com/free-vector/bird-colorful-logo-gradient-vector_343694-1365.jpg" alt="Logo"/>
            <span className="brand-name">SparkleNest</span>
          </div>
          <div className='nav-links'>
            {token===null ? (
                <>
                <p className="nav-item" onClick={handleAboutUsClicked}>About Us</p>
                <p className="nav-item" onClick={handleContactClicked}>Contact</p>
                <p className="nav-item" onClick={handleLogIn}>Log In</p>
                </>
              ) : (
                <p className="nav-item" onClick={handleLogOut}>Log Out</p>
              )
            }

          </div>
        </nav>
    );
}

export default Navbar;