import * as React from 'react';
import { connect } from 'react-redux';
import { withTranslation, WithTranslation } from "react-i18next";
import { Flex, Text, Image, Label } from '@stardust-ui/react';
import * as microsoftTeams from "@microsoft/teams-js";
import './messageCards.scss';
import 'office-ui-fabric-react/dist/css/fabric.css';
import { TFunction } from "i18next";
import { IMessage } from "../Dashboard/dashboard";
import { getBaseUrl } from '../../configVariables';

export interface ITaskInfo {
    title?: string;
    height?: number;
    width?: number;
    url?: string;
    card?: string;
    fallbackUrl?: string;
    completionBotId?: string;
}


export interface IMessageProps extends WithTranslation {
    messagesList: IMessage;
    getUserDashboardMessagesList?: any;
}

export interface IMessageState {
    message: IMessage[];
    loader: boolean;
}

class MessageCard extends React.Component<IMessageProps, IMessageState> {
    readonly localize: TFunction;
    private interval: any;
    constructor(props: IMessageProps) {
        super(props);
        this.localize = this.props.t;
    }

    public componentDidMount() {
        microsoftTeams.initialize();
    }

    public render(): JSX.Element {
        return (
            <div className="card-bg">
                        <Flex gap="gap.smaller" vAlign="center">
                            <Image className="card-img" src={this.props.messagesList.imageLink} data-testid="group-img" />
                        </Flex>
                        <div className="card-body">
                            <Flex gap="gap.smaller" column vAlign="start">
                                <Flex gap="gap.smaller" className="title-flex">
                                    <Text className="card-title-text" size="large" content={this.props.messagesList.title} weight="bold" data-testid="group-name" />
                                </Flex>
                                <div className="footer-flex">
                                    <Flex gap="gap.smaller" className="tags-flex">
                                        < Label className="tags-label-wrapper" circular content={this.props.messagesList.account} />
                                    </Flex>
                                    <Flex className="card-content-flex" gap="gap.small">
                                       <div dangerouslySetInnerHTML={{__html: this.props.messagesList.summary}}/>
                                    </Flex>
                                </div>
                            </Flex>
                        </div>
                    </div>
        );
    }
}

export default withTranslation()(MessageCard);