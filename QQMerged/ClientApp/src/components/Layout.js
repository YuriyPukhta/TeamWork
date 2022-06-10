import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';

import {AppContext} from './AppContext.jsx';

export const Context = React.createContext();

export class Layout extends Component {
    static displayName = Layout.name;

    constructor(props) {
        super(props);

        this.toggleAuth = (auth) => {
            this.setState({ auth });
        };

        this.state = { auth: false, toggleAuth: this.toggleAuth };
    }

    render() {
        return (
            <div>
                <AppContext.Provider value={this.state}>
                    <NavMenu />
                    <Container fluid>
                        {this.props.children}
                    </Container>
                </AppContext.Provider>
            </div>
        );
    }
}
