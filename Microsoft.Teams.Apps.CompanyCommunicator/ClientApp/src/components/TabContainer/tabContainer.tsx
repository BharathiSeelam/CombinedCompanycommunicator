import * as React from 'react';
import { withTranslation, WithTranslation } from "react-i18next";
import Messages from '../Messages/messages';
import Channels from '../Channels/channels';
import ChannelAdmins from '../Channels/ChannelAdmin';
import DraftMessages from '../DraftMessages/draftMessages';
import './tabContainer.scss';
import './rc-tabs.scss';
import * as microsoftTeams from "@microsoft/teams-js";
import { getBaseUrl } from '../../configVariables';
import { Accordion, Button } from '@stardust-ui/react';
import Tabs, { TabPane } from 'rc-tabs';
import { getDraftMessagesList, getChannelsList } from '../../actions';
import { connect } from 'react-redux';
import { TFunction } from "i18next";

interface ITaskInfo {
    title?: string;
    height?: number;
    width?: number;
    url?: string;
    card?: string;
    fallbackUrl?: string;
    completionBotId?: string;
}

export interface ITaskInfoProps extends WithTranslation {
    getDraftMessagesList?: any;
    getChannelsList?: any;
}

export interface ITabContainerState {
    messageURL: string;
    channelURL: string;
}

class TabContainer extends React.Component<ITaskInfoProps, ITabContainerState> {
    readonly localize: TFunction;
    constructor(props: ITaskInfoProps) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            messageURL: getBaseUrl() + "/newmessage?locale={locale}",
            channelURL: getBaseUrl() + "/newchannel?locale={locale}"
        }
        this.escFunction = this.escFunction.bind(this);
    }

    public componentDidMount() {
        microsoftTeams.initialize();
        //- Handle the Esc key
        document.addEventListener("keydown", this.escFunction, false);
    }

    public componentWillUnmount() {
        document.removeEventListener("keydown", this.escFunction, false);
    }

    public escFunction(event: any) {
        if (event.keyCode === 27 || (event.key === "Escape")) {
            microsoftTeams.tasks.submitTask();
        }
    }

    public render(): JSX.Element {
        const panels = [
            {
                title: this.localize('DraftMessagesSectionTitle'),
                content: {
                    key: 'sent',
                    content: (
                        <div className="messages">
                            <DraftMessages></DraftMessages>
                        </div>
                    ),
                },
            },
            {
                title: this.localize('SentMessagesSectionTitle'),
                content: {
                    key: 'draft',
                    content: (
                        <div className="messages">
                            <Messages></Messages>
                        </div>
                    ),
                },
            }
        ]
        return (
            <div className="tabContainer">
                <Tabs defaultActiveKey="1" onChange={this.callback} >
                    <TabPane tab="Messages" key="1">
                        <div className="newPostBtn">
                            <Button content={this.localize("NewMessage")} onClick={this.onNewMessage} primary />
                        </div>
                        <div className="messageContainer">
                            <Accordion defaultActiveIndex={[0, 1]} panels={panels} />
                        </div>
                    </TabPane>
                    <TabPane tab="Channels" key="2">
                        <div className="newPostBtn">
                            <Button className="new" content={this.localize("NewChannel")} onClick={this.onNewChannel} primary />
                        </div>
                        <div className="channelContainer">
                            <Channels></Channels>
                        </div>
                    </TabPane>
                    <TabPane tab="Channel Admin" key="3">
                        <div className="channelAdminContainer">
                            <ChannelAdmins></ChannelAdmins>
                        </div>
                    </TabPane>
                </Tabs>
            </div>
        );
    }
    public callback = function (key: any) { };
    public onNewMessage = () => {
        let taskInfo: ITaskInfo = {
            url: this.state.messageURL,
            title: this.localize("NewMessage"),
            height: 530,
            width: 1000,
            fallbackUrl: this.state.messageURL,
        }

        let submitHandler = (err: any, result: any) => {
            this.props.getDraftMessagesList();
        };

        microsoftTeams.tasks.startTask(taskInfo, submitHandler);
    }
    public onNewChannel = () => {
        let channelTaskInfo: ITaskInfo = {
            url: this.state.channelURL,
            title: this.localize("NewChannel"),
            height: 530,
            width: 1000,
            fallbackUrl: this.state.channelURL,
        }

        let submitHandler = (err: any, result: any) => {
            this.props.getChannelsList();
        };

        microsoftTeams.tasks.startTask(channelTaskInfo, submitHandler);
    }
}

const mapStateToProps = (state: any) => {
    return { messages: state.draftMessagesList, channels: state.ChannelList};
}

const tabContainerWithTranslation = withTranslation()(TabContainer);
export default connect(mapStateToProps, { getDraftMessagesList, getChannelsList })(tabContainerWithTranslation);