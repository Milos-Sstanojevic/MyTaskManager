import React from "react";
import "../HomePage.css"

export default function OverlayContainer(props){

    const isTrue=props.signingIn;
    const className=!isTrue ? 'overlay-container-transform' : 'overlay-container';

    return(
        <div className={className}>
            {props.children}
        </div>
    )
}