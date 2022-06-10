import React, { Component } from 'react';
import {Redirect} from "react-router-dom";
import {Input, Row, Col} from 'reactstrap';
import Cookies from 'js-cookie'
import './CreateQueue.css';
import {AppContext} from './AppContext.jsx';

export class CreateQueue extends Component {
    static displayName = CreateQueue.name;
    constructor(props) {
        super(props);

        this.state = { title: '', redirect: false, form_state: true};
        this.handleTitleCreate = this.handleTitleCreate.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.checkAuth = this.checkAuth.bind(this);
    }

    handleTitleCreate(event) {
        this.setState({ title: event.target.value });
        this.setState({ form_state: true })
    }

    componentDidMount() {
        this.checkAuth();
    }

    checkAuth() {
        if (Cookies.get('JWT') == null) {
            this.setState({ redirect: true });
        } else {
            this.setState({ redirect: false });
        }
    }

    async handleSubmit(event) {
        if (Cookies.get('JWT') != null) {
            const token = "Bearer " + Cookies.get('JWT');

            const requestOptions = {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': token
                },
                body: JSON.stringify({
                    title: this.state.title
                })
            };

            const response = await fetch('/queue/create', requestOptions)
            if (!response.ok) {

                this.setState({
                    form_state: false
                })
            }
            else {
                const data = await response.json();

                this.props.history.push(`queue/${data.eventId}`);
            }
        }
        else {
            this.setState({ redirect: true });
        }
    }

    render() {
        if (this.state.redirect) {
            return (<Redirect push to={`/login`} />);
        }
        let form_state = this.state.form_state;

    
        return (

            <div className="createQBlock">

                <div className="input_name_block">
                    <div className="input_name_background" style={{ boxShadow: form_state ? "0px 4px 2px rgb(0 0 0 / 35%)" : "0px 3px 3px rgb(200 0 0)" }}>
                        <Input type="text" value={this.state.title} onChange={this.handleTitleCreate}  placeholder="Name your queue" />
                    </div>
                    <div className="sub_button" onClick={this.handleSubmit}>
                        Create
                    </div>
                </div>

                <br></br>
				<div className="moreread">Instruction for creating and moderating queues:</div>
                <br></br><br></br>
				<section id="one" className="wrapper alt style2">
                    <Row className="instRow" style={{flexWrap: "wrap-reverse"}}>
                        <Col md="6" className="instCol">
                            <img style={{width:"100%"}} src="/pic01.jpg" alt="" />
                        </Col>
                        <Col md="6" className="instCol">
                            <h2>Step 1</h2>
							<p>Enter the name of your queue and click on the button</p>
                        </Col>
                    </Row>
                    <Row className="instRow">
                        <Col md="6" className="instCol">
                            <h2>Step 2</h2>
							<p>Then you are taken to a page where you can edit and manage the queue.<br></br>
                              On this page you can change the name of the queue, remove the user from the queue, specify that the next to come.		</p>
                        </Col>

                        <Col md="6" className="instCol">
                            <img style={{width:"100%"}} src="/pic02.jpg" alt="" />
                        </Col>
                    </Row>

				</section>
				<br></br><br></br><br></br>
				<h2 align="center">Happy using!</h2>
				<br></br><br></br>
                </div>
        );
    }
}

CreateQueue.contextType = AppContext;