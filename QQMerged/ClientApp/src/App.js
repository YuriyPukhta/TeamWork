import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home.jsx';
import { CreateQueue } from './components/CreateQueue';
import { MyQueues } from './components/MyQueues';
import { About } from './components/About';
import { Account } from './components/Account.jsx';
import { Login } from './components/Login';
import { ForgotPass } from './components/ForgotPass';
import { GeneralQueue } from './components/GeneralQueue';
import { EditQueue } from './components/EditQueue';
import { Register } from './components/Register';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/createqueue' component={CreateQueue} />
        <Route path='/myqueues' component={MyQueues} />
        <Route path='/about' component={About} />
        <Route path='/account' component={Account} />
        <Route path='/editqueue' component={EditQueue} />
        <Route path='/login' component={Login} />
        <Route path='/forgotpass' component={ForgotPass} />
        <Route path='/queue/:id' component={GeneralQueue} />
        <Route path='/register' component={Register} />
      </Layout>
    );
  }
}
