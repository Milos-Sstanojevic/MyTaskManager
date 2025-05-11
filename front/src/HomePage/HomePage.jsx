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
  const [popup, setPopup] = useState(false);
  const [errorForgMail, setErrorForgMail] = useState("");
  const [forgotEmail, setForgotEmail] = useState("");
  const [showCards, setShowCards] = useState(false);


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
            throw text;
          });
        } else {
          setErrorMessage("");
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
  const toggleCards = () => {
    setShowCards(!showCards); // Toggle the visibility of the cards
  };

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
      <div className="desc-login-div" id="login">
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
      <div className="info-section" id="about-us">
        <h1>Creșteți productivitatea</h1>
        <h2>Obțineți mai mult cu SparkleNest</h2>
        <p>Rămâneți la curent cu sarcinile dvs. cu integrări perfecte, automatizare puternică și sincronizare în timp real pentru toate proiectele dvs.</p>
        <button className="toggle-button" onClick={toggleCards}>
          {showCards ? "🔼 Ascundeți cardurile" : "🔽 Arătați cardurile"} {/* Toggle button with arrows */}
        </button>
        <div className={`info-cards ${showCards ? "show" : "hide"}`}>
          <div className="info-card">
            <h3>Notează-l, rămâi în control</h3>
            <p>Transformă-ți gândurile în acțiune - adaugă sarcini de oriunde și oricând.</p>
          </div>
          <div className="info-card">
            <h3>Structurați-vă fluxul de lucru</h3>
            <p>De la mici comisioane la proiecte mari, ține totul sub control cu ​​un sistem care funcționează pentru tine. Marcați sarcinile ca „în desfășurare”, „în așteptare” sau „finalizate”.</p>
          </div>
          <div className="info-card">
            <h3>Gestionați-vă timpul fără efort</h3>
            <p>Mutați sarcinile, setați prioritățile și programați-vă cea mai importantă activitate în doar câteva clicuri</p>
          </div>
        </div>

      </div>
      <div className="contact-section" id="contact">
        <h2>Contactați-ne</h2>
        <p>Dacă aveți întrebări sau sugestii, nu ezitați să ne contactați!</p>
        <form className="contact-form">
          <input
            type="text"
            placeholder="Nume"
            required
            className="contact-input" />
          <input
            type="text"
            placeholder="Prenume"
            required
            className="contact-input" />
          <input
            type="email"
            placeholder="Email"
            required
            className="contact-input" />
          <textarea
            placeholder="Mesaj"
            required
            className="contact-textarea"></textarea>
          <button type="submit" className="contact-button">Trimite</button>
        </form>
      </div>
      <footer className="footer">
        <p>
          Contact: <a href="mailto:hello@sparklentest.com">ivana@sparklentest.com</a>
        </p>
        <p>
          © {new Date().getFullYear()} SparkleNest. All rights reserved.
        </p>
      </footer>

    </div>



  );
};

export default HomePage;