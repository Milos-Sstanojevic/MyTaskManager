import React from "react";
import {Navigate,Outlet} from 'react-router-dom'

const BadRoutePage = () => {
    const token = localStorage.getItem('token');

    if (!token)
        window.location.href = "http://localhost:3000";
    
    return <Outlet/>
}

export default BadRoutePage;