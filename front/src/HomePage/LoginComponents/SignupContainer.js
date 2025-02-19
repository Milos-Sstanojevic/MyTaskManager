import React from "react";
import "../HomePage.css"

const SignUpContainer=(props)=>{
    const className= !props.signingIn ? 'signup-transform' : 'signup';

    return(
        <div className={className}>
            {props.children}
        </div>
    )
}

export default SignUpContainer;