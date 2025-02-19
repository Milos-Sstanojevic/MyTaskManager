import React from 'react'
import './Navbar.css'

const Navbar = () => {

    const handleLogOut = () => {
    // Remove token from local storage
    localStorage.removeItem('token');
    // Refresh the window
    window.location.reload();
  };

    return (
        <nav className="nav">
            <img className="logo-img" src="https://img.freepik.com/free-vector/bird-colorful-logo-gradient-vector_343694-1365.jpg"></img>
      <ul>
          Sparkle Nest
        <div className='other-links'>
          <p onClick={handleLogOut}>Log out</p>
        </div>
      </ul>
    </nav>
    )
}

export default Navbar;