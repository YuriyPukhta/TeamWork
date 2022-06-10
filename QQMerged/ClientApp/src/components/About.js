import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './About.css';

export class About extends Component {
  static displayName = About.name;

  render() {
    return (
        <Container fluid>
            <div>
                <br></br>
                <h2 align="center" className="htext">Existing problem</h2>
                <br></br>
                <p className="problem" align="center">This situation occurs when people form a queue, such as at the checkout in a self-service store.
                    Fluctuations in the length of the queue in front of the cash register can be described by a convolution
                    of two probability distributions,
                    one of which characterizes the arrival of customers, and the other at the end of their service.<br></br><br></br>
                    Then you can inform the administration about the expected maximum length of the queue.
                    Obviously, the availability of such information will allow the organization of normal customer service. 
                    At the same time, he can make a decision based on the allowable degree of risk,
                    believing that buyers can wait longer than the previously determined time.
                    Probably the answer will vary depending on the day of the week and even the time of day.
                    Conducting operational research will lead to more scientific and more efficient use of available labor.</p>
                <br></br>
                <h2 align="center" className="htext">We can fix it!</h2>
                <br></br>
                <div className="box alt">
                <div class="row gtr-uniform">
                <section class="col-4 col-6-medium col-12-xsmall">
                <img src="https://i.ibb.co/6mycNNY/comfortable-icon-1.png" className="comfort"/>
                <h3 align="center">Comfortable interface</h3>
                <p align="center">Our startup has a comfortable and wonderful design and is easy to use</p>
                 </section>
                <section class="col-4 col-6-medium col-12-xsmall">
                <img src="https://i.ibb.co/XWRSsQv/time.png" className="comfort" />
                <h3 align="center">Planning </h3>
                <p align="center">Plan your day, stand in queue and don't worry about someone taking your place </p>
                 </section>
                <section class="col-4 col-6-medium col-12-xsmall">
                <img src="https://i.ibb.co/Qr53sfg/img-456510.png" className="comfort" />
                <h3 align="center">Free</h3>
                <p align="center">Another advantage of our service is that it is free to use</p>
                </section>
                </div>
                </div>
                <br></br>
                <br></br>
                <a href="/createqueue" className="floating-button">Start</a>
                <br></br>

          </div>
        </Container>
    );
  }
}
