import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './EditQueue.css';

export class EditQueue extends Component {
    static displayName = EditQueue.name;

    render() {
        return (
            <div class="flexbox">
                <div class="flexboxvert">
                    <form class="item" action="">
                        <input type="text" class="iteminput" placeholder="Name your queue" />
                    </form>
                    <form class="item" action="">
                        <input type="text" class="iteminput1" placeholder="Queue description" />
                    </form>
                    <form class="item" action="">
                        <input type="text" class="iteminput" placeholder="Password" />
                    </form>
                </div>
                <div>
                    <form action="">
                        <input class="formbutton" type="submit" value="Edit queue" />
                    </form>
                </div>

            </div>
        );
    }
}
