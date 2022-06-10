import React, { Component } from 'react';
import { Container, Col, Input } from 'reactstrap';
import './Register.scss';

export class Register extends Component {
    static displayName = Register.name;

    constructor(props) {
        super(props);
        this.state = { name: '', email: '', password: '',form_state: true };
        this.handleNameChange = this.handleNameChange.bind(this);
        this.handleEmailChange = this.handleEmailChange.bind(this);
        this.handlePasswordChange = this.handlePasswordChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleNameChange(event) {
        this.setState({ name: event.target.value });
        this.setState({ form_state: true })
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
            body: JSON.stringify({ username: this.state.name, email: this.state.email, password: this.state.password })
        };

        const response = await fetch('/register', requestOptions)
        if (!response.ok) {

            this.setState({
                form_state: false
            })
        }
        else {

            window.open("/", "_self");
        }
    }

    render() {
        let form_state = this.state.form_state;
        return (
            <Container fluid>
                <div className="register_block">

                    <Col >
                        <div className="register_iteminput">

                            <div className="register_inputbox" style={{ boxShadow: form_state ? "0px 4px 2px rgb(0 0 0 / 35%)" : "0px 3px 3px rgb(200 0 0)" }}>

                                <Input type="text" value={this.state.name} onChange={this.handleNameChange} placeholder="Name" />
                            </div>

                            <div className="register_inputbox" style={{ boxShadow: form_state ? "0px 4px 2px rgb(0 0 0 / 35%)" : "0px 3px 3px rgb(200 0 0)" }}>

                                <Input type="text" value={this.state.email} onChange={this.handleEmailChange} placeholder="E-mail" />
                            </div>

                            <div className="register_inputbox" style={{ boxShadow: form_state ? "0px 4px 2px rgb(0 0 0 / 35%)" : "0px 3px 3px rgb(200 0 0)" }}>

                                <Input type="password" value={this.state.password} onChange={this.handlePasswordChange} placeholder="Password" />

                            </div>
                            <div >
                                    <div className="register_btn" onClick={this.handleSubmit}>Register
                                    </div>
                            </div>

                        </div>
                        


                    </Col>
                    



                </div>
            </Container>
        );
    }
}
