import { useEffect, useState } from "react";
import "./HomePage.css";
import { NavLink, useNavigate } from "react-router-dom";
import SignUpContainer from "./LoginComponents/SignupContainer";
import SignInContainer from "./LoginComponents/SignInContainer";
import OverlayContainer from "./LoginComponents/OverlayContainer";
import Overlay from "./LoginComponents/Overlay";
import LeftOverlayPanel from "./LoginComponents/LeftOverlayPanel";
import RightOverlayPanel from "./LoginComponents/RightOverlayPanel";


const HomePage = () => {
    
const [signIn, toggle] = useState(true);
  const [username, setUsername] = useState(null);
  const [password, setPassword] = useState();
  const [repeatPassword, setRepeatPassword] = useState(null);
  const [email, setEmail] = useState();
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState(null);
  const navigate = useNavigate();
  const [notVerified, setNotVerified] = useState(true);
  const [popup, setPopup] = useState(false);
  const [errorForgMail,setErrorForgMail]=useState("");
  const [forgotEmail,setForgotEmail]=useState("");

  const guestOnClick = () => {
    localStorage.setItem("Guest", true);
  };
  if (localStorage.getItem("token")) {
    return (
      <div className="prijavljenPage">
        <div className="prijavljen">
          <h1>Korisnik je vec prijavljen</h1>
          <br />
          <NavLink to="/" className="link-pocetna">
            Pocetna stranica!
          </NavLink>
        </div>
      </div>
    );
    }
    
  const resetInputs = () => {
    toggle(true);
    setEmail(null);
    setPassword("");
    setUsername("");
    setRepeatPassword("");
    setErrorMessage("");
  };
  const resetInputsK = () => {
    toggle(false);
    setEmail("");
    setPassword("");
    setUsername("");
    setRepeatPassword("");
    setErrorMessage("");
  };

  const handleSignupSubmit = (e) => {
    e.preventDefault();
    const profil = { username, email, password, repeatPassword };
    setLoading(true);
    fetch(
      `http://localhost:5164/Login/SignUp/${
        profil.username
      }/${encodeURIComponent(profil.email)}/${profil.password}/${
        profil.repeatPassword
      }`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
      }
    )
      .then((res) => {
        setLoading(false);
        if (!res.ok) {
          res.text().then((text) => {
            console.log(text);
            setErrorMessage(text);
          });
        } else {
          console.log(res);
          navigate("/");
          setErrorMessage("");
          window.location.reload();
        }
      })
      .catch((err) => {
        setLoading(false);
        console.log(err);
      });
  };

  const forgotPasswordPopup = () => {
    setPopup(true);
  };

  const handleLoginSubmit = async (e) => {
    e.preventDefault();
    const profil = { email, password };
    setLoading(true);
    try {
      await fetch(
        `http://localhost:5164/Login/LoginUser/${profil.email}/${profil.password}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        }
      ).then(async (res) => {
        setLoading(false);
        if (!res.ok || res.status === 400) {
          await res.text().then((text) => {
            console.log(text);
            setErrorMessage(text);
            setNotVerified(true);
            throw text;
          });
        } else {
          setErrorMessage("");
          setNotVerified(false);
        }
      });

      console.log();

      await fetch(`http://localhost:5164/Login/GetToken`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: profil.email,
          password: profil.password,
        }),
      })
        .then((res) => res.json())
        .then((data) => {
          localStorage.setItem("token", data.token);
        })
        .then(() => {
          navigate("/MainPage");
        });
    } catch (error) {
      setLoading(false);
      console.log(error);
    }
  };
  


  const sendMail=async()=>{
    await fetch(`http://localhost:5164/Login/ForgotPasswordEmail/${forgotEmail}`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    }).then(res=>{
      if(!res.ok){
        res.text().then(text=>{
          setErrorForgMail(text);
        })
      }
      else{
        setErrorForgMail("");
        setPopup(false);
      }
    }).catch(err=>{
      console.log(err);
    })
  }

    return (
    <div className="div-pozadina">
      {popup === true ? (
        <div className="forgot-password">
          <label style={{textAlign:"center"}}>Unesite vaš E-mail da biste resetovali šifru!</label>
          <input
            className="login-input-forgot"
            type="email"
            placeholder="Email"
            onChange={(e)=>setForgotEmail(e.target.value)}
          ></input>
          <label className="error-mail">{errorForgMail}</label>
          <div className="buttons-forg">
          <button className="login-button" onClick={sendMail}>Potvrdi</button>
          <button className="login-button" onClick={()=>setPopup(false)}>Otkaži</button>
          </div>
        </div>
      ) : (
        ""
            )}
      <div className="container-div">
        <SignUpContainer>
          <form className="form-login" onSubmit={handleSignupSubmit}>
            <h1 className="title">Kreirajte profil</h1>
            <input
              className="login-input"
              type="text"
              placeholder="Korisničko ime"
              onChange={(e) => setUsername(e.target.value)}
            ></input>
            <input
              className="login-input"
              type="email"
              placeholder="Email"
              onChange={(e) => setEmail(e.target.value)}
            ></input>
            <input
              className="login-input"
              type="password"
              placeholder="Lozinka"
              onChange={(e) => setPassword(e.target.value)}
            ></input>
            <input
              className="login-input"
              type="password"
              placeholder="Potvrda lozinke"
              onChange={(e) => setRepeatPassword(e.target.value)}
            ></input>
            {!loading && <button className="login-button">Kreiraj</button>}
            {loading && (
              <button className="login-button" disabled>
                Kreiranje...
              </button>
            )}
            <div className="error">{errorMessage}</div>
          </form>
        </SignUpContainer>
        <SignInContainer signingIn={signIn}>
          <form className="form-login" onSubmit={handleLoginSubmit}>
            <h1 className="title">Prijavite se!</h1>
            <input
              className="login-input"
              type="email"
              placeholder="Email"
              onChange={(e) => setEmail(e.target.value)}
            ></input>
            <input
              className="login-input"
              type="password"
              placeholder="Lozinka"
              onChange={(e) => setPassword(e.target.value)}
            ></input>
            {!loading && <button className="login-button">Prijavi se</button>}
            {loading && (
              <button className="login-button" disabled>
                Prijavljivanje...
              </button>
            )}
            <label className="link-zaboravljena" onClick={forgotPasswordPopup}>
              Zaoboravili ste lozinku? Kliknite ovde
            </label>
            <div className="error">{errorMessage}</div>
          </form>
        </SignInContainer>
        <OverlayContainer signingIn={signIn}>
          <Overlay signingIn={signIn}>
            <LeftOverlayPanel signingIn={signIn}>
              <h1 className="title">Dobrodošli!</h1>
              <p className="parag-login">
                Prijavite se na svoj profil da nastavimo igru!
              </p>
              <button className="ghost-button" onClick={resetInputs}>
                Prijavi se
              </button>
            </LeftOverlayPanel>

            <RightOverlayPanel signingIn={signIn}>
              <h1 className="title">Zdravo!</h1>
              <label className="parag-login">
                Napravite profil i pokrenite zabavu!
                <div className="mt-3 mb-3"></div>
              </label>

              <button className="ghost-button" onClick={resetInputsK}>
                Kreiraj profil
              </button>

            </RightOverlayPanel>
          </Overlay>
        </OverlayContainer>
            </div>
        <div className="desc-logo-div">
            <div className="logo"></div>
            <div className="site-descP">
                <p>ALO BRE OVO JE OPIS SAJTA</p>
            </div>
        </div>
    </div>
  );
};

export default HomePage;