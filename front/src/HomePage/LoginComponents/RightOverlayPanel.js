import React from "react";
import "../HomePage.css"

const RightOverlayPanel=(props)=>{
    const isTrue = props.signingIn;
    const className=!isTrue ? 'right-panel-transform' : 'right-panel';

    return(
        <div className={className}>
            {props.children}
        </div>
    )
}

export default RightOverlayPanel;