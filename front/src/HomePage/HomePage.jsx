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
  const [errorForgMail, setErrorForgMail] = useState("");
  const [forgotEmail, setForgotEmail] = useState("");


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
      `http://localhost:5164/Login/SignUp/${profil.username
      }/${encodeURIComponent(profil.email)}/${profil.password}/${profil.repeatPassword
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



  const sendMail = async () => {
    await fetch(`http://localhost:5164/Login/ForgotPasswordEmail/${forgotEmail}`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      }).then(res => {
        if (!res.ok) {
          res.text().then(text => {
            setErrorForgMail(text);
          })
        }
        else {
          setErrorForgMail("");
          setPopup(false);
        }
      }).catch(err => {
        console.log(err);
      })
  }

  return (
    <div className="div-pozadina">
      {popup === true && (
        <div className="forgot-password">
          <label style={{ textAlign: "center" }}>Introduceți adresa dvs. de e-mail pentru a reseta parola</label>
          <input
            className="login-input-forgot"
            type="email"
            placeholder="Email"
            onChange={(e) => setForgotEmail(e.target.value)}
          ></input>
          <label className="error-mail">{errorForgMail}</label>
          <div className="buttons-forg">
            <button className="login-button" onClick={sendMail}>Potvrdi</button>
            <button className="login-button" onClick={() => setPopup(false)}>Anulează</button>
          </div>
        </div>
      )}
      <div className="desc-login-div">
          <div className="site-descP">
            <div className="desc-box">
              <p>Unde ideile tale strălucesc și sarcinile rămân organizate!✨ </p>
            </div>
            <div className="desc-box">
              <p>📅 Planificați, urmăriți 👀 și colaborați fără efort. 🤝</p>
            </div>
            <div className="desc-box">
              <p>💡Transformă haosul în claritate și productivitatea într-o briză!💨🚀</p>
            </div>
        </div>
                <div className="container-div">
          <SignUpContainer>
            <form className="form-login" onSubmit={handleSignupSubmit}>
              <h1 className="title">Creați un profil</h1>
              <input
                className="login-input"
                type="text"
                placeholder="Nume de utilizator"
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
                placeholder="Parolă"
                onChange={(e) => setPassword(e.target.value)}
              ></input>
              <input
                className="login-input"
                type="password"
                placeholder="Confirmarea parolei"
                onChange={(e) => setRepeatPassword(e.target.value)}
              ></input>
              {!loading && <button className="login-button">Crează</button>}
              {loading && (
                <button className="login-button" disabled>
                  Creare...
                </button>
              )}
              <div className="error">{errorMessage}</div>
            </form>
          </SignUpContainer>
          <SignInContainer signingIn={signIn}>
            <form className="form-login" onSubmit={handleLoginSubmit}>
              <h1 className="title">Conectați-vă!</h1>
              <input
                className="login-input"
                type="email"
                placeholder="Email"
                onChange={(e) => setEmail(e.target.value)}
              ></input>
              <input
                className="login-input"
                type="password"
                placeholder="Parolă"
                onChange={(e) => setPassword(e.target.value)}
              ></input>
              {!loading && <button className="login-button">Conectează-te</button>}
              {loading && (
                <button className="login-button" disabled>
                  Conectare...
                </button>
              )}
              <label className="link-zaboravljena" onClick={forgotPasswordPopup}>
                Ați uitat parola? Faceți clic aici
              </label>
              <div className="error">{errorMessage}</div>
            </form>
          </SignInContainer>
          <OverlayContainer signingIn={signIn}>
            <Overlay signingIn={signIn}>
              <LeftOverlayPanel signingIn={signIn}>
                <h1 className="title">Bine ați venit!</h1>
                <p className="parag-login">
                  Conectați-vă la profilul dvs. pentru a continua!
                </p>
                <button className="ghost-button" onClick={resetInputs}>
                  Conectează-te
                </button>
              </LeftOverlayPanel>

              <RightOverlayPanel signingIn={signIn}>
                <h1 className="title">Salut!</h1>
                <label className="parag-login">
                  Crează un profil și notează-ți sarcinile!
                  <div className="mt-3 mb-3"></div>
                </label>

                <button className="ghost-button" onClick={resetInputsK}>
                  Crează profil
                </button>

              </RightOverlayPanel>
            </Overlay>
          </OverlayContainer>
        </div>
      </div>
      <div className="info-section">
        <h1>Work Smarter</h1>
        <h2>Do more with Trello</h2>
        <p>Customize the way you organize with easy integrations, automation, and mirroring of your to-dos across multiple locations.</p>

        <div className="info-cards">
          <div className="info-card">
            <h3>Integrations</h3>
            <p>Connect the apps your team already uses into your Trello workflow or add a Power-Up to fine-tune your specific needs.</p>
            <button>Browse Integrations</button>
          </div>
          <div className="info-card">
            <h3>Butler Automation</h3>
            <p>No-code automation is built into every Trello board. Focus on the work that matters most and let the robots do the rest.</p>
            <button>Get to know Automation</button>
          </div>
          <div className="info-card">
            <h3>Card Mirroring</h3>
            <p>View your to-dos from different boards in more than one place. Mirror a card to keep track of work wherever you need it!</p>
          </div>
        </div>
      </div>
    </div>


  );
};

export default HomePage;