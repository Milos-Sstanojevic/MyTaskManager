import React, { useState } from "react";
import "./MainPage.css"; // Import the CSS file

export default function MainPage() {

  const existingToken = localStorage.getItem('token');
  if (!existingToken) {
    window.location.href = '/';
    return null; 
  }

    return (
        <div className="tabs-cards-div">
            <div className="tabs-div">

            </div>
            <div className="cards-div">

            </div>
        </div>
    );
}
