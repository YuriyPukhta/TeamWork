import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './ForgotPass.css';

export class ForgotPass extends Component {
    static displayName = ForgotPass.name;

    render() {
        return (
            <div class="flexbox">
                <div class="flexboxvert">
                    <form class="item" action="">
                        <input type="text" class="iteminput" placeholder="Enter your e-mail or name" />
                    </form>
                </div>
                <div>
                    <form action="">
                        <input class="formbutton" type="submit" value="Remember" />
                    </form>
                </div>

            </div>
        );
    }
}
