import "../HomePage.css"

export default function SignInContainer(props){
    const className=null ? 'signin-transform' : 'signin';
    return(
        <div className={className}>
            {props.children}
        </div>
    )
}