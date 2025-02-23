import './App.css';
import HomePage from "./HomePage/HomePage"
import { Route, Routes, useLocation } from 'react-router-dom';
import React from 'react'
import MainPage from './MainPage/MainPage';
import Navbar from "./MainPage/Navbar/Navbar";
import MainPageTabs from './MainPage/MainPageTabs/MainPageTabs';
import BadRoutePage from './BadRoutePage/BadRoutePage';

function App() {
  const location = useLocation();
  const isHomePage = location.pathname === '/';
  const token = localStorage.getItem('token');


  return (
    <div className="app">
      {!isHomePage && token && <Navbar />}
      <section className="content">
        <Routes>
          <Route path="/" element={<HomePage />} />
          {token && (
            <Route path="/MainPage" element={<MainPageTabs />} />
          )}
        </Routes>
      </section>
    </div>
  );
}

export default App;
