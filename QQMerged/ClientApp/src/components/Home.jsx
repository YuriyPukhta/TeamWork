import React, { Component } from 'react';
import { Container, Row, Col, Input, Modal, ModalFooter} from 'reactstrap';
import './Home.scss';
import Cookies from 'js-cookie'
import {Link, Redirect, withRouter} from "react-router-dom";

import {AppContext} from './AppContext.jsx';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props, context) {
        super(props, context);
        this.state = { idQ: "", showNfmsg: false, errmsg:"Queue not found", redirect: false};

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.toggleNfmsg = this.toggleNfmsg.bind(this);
    }

    handleChange(event) {
        this.setState({ idQ: event.target.value });
    }

    async handleSubmit() {
        if (Cookies.get('JWT') != null){
            if (this.state.idQ == "") {
            
            }
            else {
                const token = "Bearer " + Cookies.get('JWT');
                const requestOptions = {
                    method: 'GET',
                    headers: { 'Authorization': token }
                };

                const response = await fetch(`event/${this.state.idQ}`, requestOptions);

                if (!response.ok) {
                    this.toggleNfmsg();
                }
                else {
                    this.props.history.push(`queue/${this.state.idQ}`);
                }
            }
        }
        else {
            this.setState({ redirect: true });
        }
    }


    toggleNfmsg() {
        this.setState({ showNfmsg: !this.state.showNfmsg });
    }


    render() {
        let id = this.state.idQ
        let msg = this.state.errmsg

        if (this.state.redirect) {
            return (<Redirect push to={`login`} />);
        }

        return (
            <Container fluid>
                <div className="home_background"></div>
                <Row style={{flexWrap: "wrap-reverse"}}>
                    <Col sm="6">
                        <div className="input_id_block">
                            <div className="input_background_background">
                                <div className="input_background">
                                    <Input type="text" value={this.state.idQ} onChange={this.handleChange} placeholder="ID..." />
                                </div>
                            </div>

                            <Link to="" onClick={this.handleSubmit}>
                                <div className="submit_button">
                                    Join
                                </div>
                            </Link>
                        </div>
                    </Col>

                    <Col sm="6">
                        <div className="about_block">
                            <h1>Stop wasting time in queues</h1>
                            <h2>use smart queue service today</h2>
                        </div>
                    </Col>
                </Row>


                <Modal isOpen={this.state.showNfmsg} toggle={this.toggleNfmsg}>
                    <ModalFooter>
                        {msg}
                        <div className="ok_button1" onClick={this.toggleNfmsg}>OK</div>
                    </ModalFooter>
                </Modal>
            </Container>
        );
    }
}

Home.contextType = AppContext;
export default withRouter(Home);