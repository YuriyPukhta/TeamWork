import React, { Component } from 'react';
import { Container, Row, Col, Input } from 'reactstrap';
import Cookies from 'js-cookie'
import './Account.scss';
import {Link, Redirect, withRouter} from "react-router-dom";

import {AppContext} from './AppContext.jsx';

export class Account extends Component {
    static displayName = Account.name;

    constructor(props) {
        super(props);

        this.state = { username: '', email: '', form_state:true};
        this.handleUsernameChange = this.handleUsernameChange.bind(this);
        /*this.handleEmailChange = this.handleEmailChange.bind(this);*/
        /*this.handlePasswordChange = this.handlePasswordChange.bind(this);*/
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleUsernameChange(event) {
        this.setState({ username: event.target.value, form_state: true });
    }

    /*handleEmailChange(event) {
        this.setState({ email: event.target.value });
    }*/

    componentDidMount() {
        this.getUser();
    }

    async getUser() {
        if (Cookies.get('JWT') != null) {

            const token = "Bearer " + Cookies.get('JWT');
            const requestOptions = {
                method: 'GET',
                headers: { 'Authorization': token }
            };

            const response = await fetch(`/my_account`, requestOptions);
            const data = await response.json();

            if (response.ok) {
                this.setState({ username: data["username"], email: data["email"] });
            }
            else {
                this.props.history.push(`/`);
            }

        }
        else {
            /*this.setState({ redirect: true });*/
        }
    }

    async handleSubmit(event) {
        const token = "Bearer " + Cookies.get('JWT');
        const requestOptions = {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json', 
                 'Authorization': token },
            body: JSON.stringify({
                username: this.state.username
            })
        };

        const response = await fetch('/user/change', requestOptions)

        if (response.ok) {
            window.open("/", "_self");
        }
        else {
            this.setState({ form_state: false });
        }
        
    }

    render() {
        let form_state = this.state.form_state;
        return (
            <Container fluid>
                <div className="mainbox">

                    <Col >
                        <div className="inputitem">
                            <div className="inputbox" style={{ boxShadow: form_state ? "0px 4px 2px rgb(0 0 0 / 35%)" : "0px 4px 2px rgb(250 50 0)" }}>
                                <Input type="text" value={this.state.username} onChange={this.handleUsernameChange} placeholder="Username" />
                            </div>
                            <div className="inputbox">
                                <Input type="text" disabled value={this.state.email} onChange={this.handleEmailChange} placeholder="E-mail" />
                            </div>

                            {/*<div className="inputbox">

                                <Input type="text" value={this.state.password} onChange={this.handlePasswordChange} placeholder="Password" />

                            </div>*/}
                            {/*<div className="inputbox">

                                <Input type="text" value={this.state.phone_number} onChange={this.handlePhoneChange} placeholder="Phone number" />

                            </div>*/}
                        </div>


                    </Col>
                    <Col className="btn">
                        <br></br>
                        <div>
                            <a className="change_btn" onClick={this.handleSubmit}>Change</a>
                        </div>
                    </Col>

                </div>
            </Container>
        );
    }
}

Account.contextType = AppContext;
export default withRouter(Account);

