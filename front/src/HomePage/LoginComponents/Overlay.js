import "../HomePage.css"

export default function Overlay(props){
    const isTrue=props.signingIn;
    const className=!isTrue ? 'overlay-transform' : 'overlay';

    return(
        <div className={className}>
            {props.children}
        </div>
    )
}