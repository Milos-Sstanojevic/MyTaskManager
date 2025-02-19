import './App.css';
import HomePage from "./HomePage/HomePage"
import { Route, Routes, useLocation } from 'react-router-dom';
import React from 'react'
import MainPage from './MainPage/MainPage';
import Navbar from "./MainPage/Navbar/Navbar";

function App() {
  const location = useLocation();
  const isHomePage = location.pathname === '/';
  
  return (
    <div className="app">
      {!isHomePage && <Navbar />}
      <section className="content">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/MainPage" element={<MainPage/>}/>
        </Routes>
      </section>
    </div>
  );
}

export default App;
