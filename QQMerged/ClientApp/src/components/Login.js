import React, { Component } from 'react';
import { Container, Row, Col, Input, Modal, ModalFooter } from 'reactstrap';
import Cookies from 'js-cookie'
import './Login.css';
import { Link } from "react-router-dom";


export class Login extends Component {
    static displayName = Login.name;

    constructor(props) {
        super(props);

        this.state = { email: '', password: '', inputClass: "inputbox", form_state:true};

        this.handleEmailChange = this.handleEmailChange.bind(this);
        this.handlePasswordChange = this.handlePasswordChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);


    }

    handleEmailChange(event) {
        this.setState({ email: event.target.value });
        this.setState({ form_state: true })
    }

    handlePasswordChange(event) {
        this.setState({ password: event.target.value });
        this.setState({ form_state: true })
    }


    async handleSubmit(event) {

        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: this.state.email, password: this.state.password })
        };

        const response = await fetch('/login', requestOptions)
        if (!response.ok) {

            this.setState({
                form_state: false
            })
        }
        else {

            const token = await response.text();


            Cookies.set('JWT', token, { path: '/' });
            Cookies.set('Click', 0, { path: '/' });

            window.open("/", "_self");
        }
        /*sessionStorage.setItem('id', data["idUser"]);*/
    }
    render() {
        let form_state = this.state.form_state;
        return (
            <Container fluid>
                <div className="mainbox">

                    <Col >

                        <div className="inputitem">

                            <div className="inputbox" style={{ boxShadow: form_state ? "0px 4px 2px rgb(0 0 0 / 35%)" : "0px 3px 3px rgb(250 50 0)" }} >

                                <Input type="text" value={this.state.email} onChange={this.handleEmailChange} onSubmit={this.handleSubmit} placeholder="E-mail" />
                            </div>

                            <div className="inputbox" style={{ boxShadow: form_state ? "0px 4px 2px rgb(0 0 0 / 35%)" : "0px 3px 3px rgb(250 50 0)" }} >

                                <Input type="password" value={this.state.password} onChange={this.handlePasswordChange} onSubmit={this.handleSubmit} placeholder="Password" />

                            </div>

                        </div>


                    </Col>
                    <Col >
                        <Row className="btns">
                            <Col sm="6" style={{ padding: "0.375rem 0.75rem" }}>
                                <label class="checkbox_container">Remember me
                                    <input type="checkbox" />
                                    <span class="checkmark"></span>
                                </label>
                            </Col>
                            <Col sm="6" className="btn">
                                <div >
                                    <a className="login_btn" onClick={this.handleSubmit}>LogIn</a>
                                </div>
                            </Col>
                         
                            <Col className="btn">
                                <div >
                                    <a href="/forgotpass" className="forgotpass_btn" >Forgot password?</a>
                                </div>
                            </Col>
                        </Row>
                    </Col>


                </div>

            </Container>
        );
    }
}
