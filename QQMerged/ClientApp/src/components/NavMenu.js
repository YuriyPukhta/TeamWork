import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarToggler, Row, Col} from 'reactstrap';
import { Link } from 'react-router-dom';
import Cookies from 'js-cookie'
import './NavMenu.css';

import {AppContext} from './AppContext.jsx';

export class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor(props, context) {
        super(props, context);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.collapseState = this.collapseState.bind(this);
        this.LogOut = this.LogOut.bind(this);

        this.state = { collapsed: true , authorized: false, username: "", collapseExited: true, loading: true};
    }

    toggleNavbar() {
        this.setState({collapsed: !this.state.collapsed});
    }

    collapseState() {
        this.setState({collapseExited: !this.state.collapseExited});
    }

    componentDidMount() {
        this.update();
        this.intervalID = setInterval(() => { this.update(); }, 300000);
    }

    componentWillUnmount() {
        clearInterval(this.intervalID);
    }

    async update() {
        if (Cookies.get('JWT') != null) {

            const token = "Bearer " + Cookies.get('JWT');

            const requestOptions = {
                method: 'GET',
                headers: { 'Authorization': token }
            };

            const response = await fetch('/my_account', requestOptions);

            if (response.ok) {
                const data = await response.json();
                sessionStorage.setItem('id', data["idUser"]);
                this.context.toggleAuth(true);
                this.setState({ authorized: true, username: data["username"], loading: false});
            }
            else {
                this.context.toggleAuth(false);
                Cookies.remove('JWT');
                this.setState({ authorized: false, loading: false});
            }
        }
        this.setState({ loading: false});
    }

    LogOut() {
        Cookies.remove('JWT');
        this.context.toggleAuth(false);
        window.open("/", "_self");
    }

    render() {
        let authorized = this.state.authorized;
        let username = this.state.username;

        const renderLogauntButton = () => {
            if ( authorized ) {
                return <Link style={{ marginRight: "20px" }} onClick={this.LogOut}>Log Out</Link>;
            } else {
                return <Link style={{ marginRight: "20px" }} to="/login">Log In</Link>;
            }
        }

        const renderSignauntButton = () => {
            if ( authorized ) {
                return <Link to="/account"><div className="signup_button">{username}</div></Link>;
            } else {

                return <Link to="/register"><div className="signup_button">Sign Up</div></Link>;
            }
        }

        if (this.state.loading) {
            return(
            <header>
                <Navbar className="navbar-expand-lg navbar-toggleable-md">
                    <Container fluid>
                        <Row style={{width: "100%"}}>
                            <Col xs="3" style={{padding: "unset"}}>
                                <Link to="/">
                                    <img src="/logo.svg" alt="" height="60px" />
                                </Link>
                            </Col>
                            <Col xs="9"></Col>
                        </Row>
                    </Container>
                </Navbar>
            </header>)
        }

        const navMenu = () => {
            if (!this.state.collapsed) {
                return (
                    <div className="navCollapsed">
                        <Link to="/createqueue">Create Queue</Link>
                        <Link to="/myqueues">My Queues</Link>
                        <Link to="/about">About</Link>
                        {renderLogauntButton()}
                        {renderSignauntButton()}
                    </div>
                )
            } else if (this.state.collapseExited) {
                return (
                    <div className="navNotCollapsed">
                        <Col xs="8">
                            <ul className="navbar-nav">
                                <Link style={{ width: "170px" }} to="/createqueue">Create Queue</Link>
                                <Link style={{ width: "170px" }} to="/myqueues">My Queues</Link>
                                <Link style={{ width: "170px" }} to="/about">About</Link>
                            </ul>
                        </Col>
                        <Col xs="4">
                            <ul className="navbar-nav" style={{ justifyContent: "flex-end" }}>
                                {renderLogauntButton()}
                                {renderSignauntButton()}
                            </ul>
                        </Col>
                    </div>
                )
            }
            else {
                return (
                    <div className="navCollapsed">
                        <Link to="/createqueue">Create Queue</Link>
                        <Link to="/myqueues">My Queues</Link>
                        <Link to="/about">About</Link>
                        {renderLogauntButton()}
                        {renderSignauntButton()}
                    </div>
                )
            }
        }

        return (
            <header>
                <Navbar className="navbar-expand-lg navbar-toggleable-md">
                    <Container fluid>
                        <Row style={{width: "100%"}}>
                            <Col xs="3" style={{padding: "unset"}}>
                                <Link to="/">
                                    <img src="/logo.svg" alt="" height="60px" />
                                </Link>
                            </Col>

                            <Col xs="9">
                                <NavbarToggler onClick={this.toggleNavbar} />
                                <Collapse isOpen={!this.state.collapsed} onExited={this.collapseState} onEntering={this.collapseState} navbar>
                                    {navMenu()}
                                </Collapse>
                            </Col>
                        </Row>
                    </Container>
                </Navbar>
            </header>
        );
    }
}

NavMenu.contextType = AppContext;
export default NavMenu;