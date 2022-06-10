import React, { Component } from 'react';
import { Container, Row, Col, Modal, ModalHeader, ModalFooter} from 'reactstrap';
import Cookies from 'js-cookie'
import { Virtuoso } from 'react-virtuoso';
import CustomScrollbar from "./CustomScroller";
import "overlayscrollbars/css/OverlayScrollbars.css";
import './MyQueues.scss';
import {Redirect} from "react-router-dom";
import {AppContext} from './AppContext.jsx';


export class MyQueues extends Component {
    static displayName = MyQueues.name;

    constructor(props) {
        super(props);
        this.state = {loading: true, showContentJoined: true, redirect: false, joinedlist:[], createdlist:[], idToDelete:-1, showNfmsg: false};

        this.deleteAsk = this.deleteAsk.bind(this);
        this.deleteUnAsk = this.deleteUnAsk.bind(this);
        this.deleteQueue = this.deleteQueue.bind(this);
    }

    componentDidMount() {
        this.qupdate();
    }

    async qupdate() {
        if (Cookies.get('JWT') != null) {
            const token = "Bearer " + Cookies.get('JWT');

            const qrequestOptions = {
                method: 'GET',
                headers: { 'Authorization': token }
            };

            let joinedlistData = [];
            let createdlistData = [];

            const joinedlistresponse = await fetch(`get_my_queue`, qrequestOptions);
            if (joinedlistresponse.ok) {
                joinedlistData = await joinedlistresponse.json();
            }

            const createdlistresponse = await fetch(`get_my_event`, qrequestOptions);
            if (createdlistresponse.ok) {
                createdlistData = await createdlistresponse.json();
            }

            this.setState({ createdlist: createdlistData, joinedlist: joinedlistData, loading: false });
        }
        else {
            this.setState({ redirect: true });
        }
    }

    joined() {
        this.setState({showContentJoined: true })
    }

    created() {
        this.setState({showContentJoined: false })
    }

    openQueue(id) {
        this.props.history.push(`queue/${id}`);
    }

    deleteAsk(id) {
        this.setState({ showNfmsg: !this.state.showNfmsg, idToDelete:id});
    }

    deleteUnAsk() {
        this.setState({ showNfmsg: !this.state.showNfmsg, idToDelete:-1});
    }

    async deleteQueue() {
        const token = "Bearer " + Cookies.get('JWT');
        const requestOptions = {
            method: 'PUT',
            headers: { 'Authorization': token }
        };

        const response = await fetch(`/queue/${this.state.idToDelete}/moder/finish`, requestOptions);

        this.setState({ showNfmsg: !this.state.showNfmsg, idToDelete:-1});

        if (response.ok) {
            this.qupdate();
        }

    }

    render() {
        let joinedlist = this.state.joinedlist;
        let createdlist = this.state.createdlist;
        let showJoined = this.state.showContentJoined;

        if (this.state.redirect) {
            return (<Redirect push to={`/login`} />);
        }

        const renderList = () => {
            if (this.state.showContentJoined) {
                return (<div className="list">
                    <Virtuoso
                        components={{ Scroller: CustomScrollbar }}
                        className="EventList"
                        data={joinedlist}
                        itemContent={(index, Queue) => <div className="EventItem" onClick={this.openQueue.bind(this, Queue.eventId)}>{Queue.title}</div>}
                    />
                </div>)
            }
            else {
                return (<div className="list">
                    <Virtuoso
                        components={{ Scroller: CustomScrollbar }}
                        className="EventList"
                        data={createdlist}
                        itemContent={(index, Queue) =>
                            <div className="EventItem">
                                <Col xs="10" onClick={this.openQueue.bind(this, Queue.eventId)}>
                                    {Queue.title}
                                </Col>
                                <Col xs="2" style={{display: "flex"}}>
                                    <div className="trash_btn_box">
                                        <img src="/trash-icon 1.svg" alt="" onClick={this.deleteAsk.bind(this, Queue.eventId)} />
                                    </div>
                                </Col>
                            </div>}
                    />
                </div>)
            }
        }

        return (
            <Container fluid>
                <div className="list_mainblock">
                    <Row>
                        <div className="col padding-0" >
                            <div className="switch_btn" style={{marginTop: showJoined? "1px":"5%"}} onClick={() => this.joined()}>Joined</div>
                        </div>
                        <div className="col padding-0">
                            <div className="switch_btn" style={{marginTop: !showJoined? "1px":"5%"}}onClick={() => this.created()}>Created</div>
                        </div>
                        {renderList()}
                    </Row>
                </div>

                <Modal isOpen={this.state.showNfmsg} toggle={this.deleteAsk}>
                    <ModalHeader>
                        Are you sure you want to delete queue?
                    </ModalHeader>
                    <ModalFooter>
                        <div className="not_ok_button" onClick={this.deleteUnAsk}>Cancel</div>
                        <div className="ok_button" onClick={this.deleteQueue}>DELETE</div>
                    </ModalFooter>
                </Modal>
            </Container>
        );
    }
}

MyQueues.contextType = AppContext;


