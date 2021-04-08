import * as React from 'react';
import { connect } from 'react-redux';
import { withTranslation, WithTranslation } from "react-i18next";
import { initializeIcons } from 'office-ui-fabric-react/lib/Icons';
import { Loader, List, Flex, Text } from '@stardust-ui/react';
import * as microsoftTeams from "@microsoft/teams-js";

import './channels.scss';
import { selectChannel, getChannelsList } from '../../actions';
import { getBaseUrl } from '../../configVariables';
import Overflow from '../OverFlow/channelOverflow';
import { TFunction } from "i18next";

export interface ITaskInfo {
    title?: string;
    height?: number;
    width?: number;
    url?: string;
    card?: string;
    fallbackUrl?: string;
    completionBotId?: string;
}

export interface IChannel {
    id: string;
    channelName: string;
    channelAdmins: string;
}

export interface IChannelProps extends WithTranslation {
    channels: IChannel[];
    selectedChannel: any;
    selectChannel?: any;
    getChannelsList?: any;
}

export interface IChannelState {
    channel: IChannel[];
    itemsAccount: number;
    loader: boolean;
    teamsTeamId?: string;
    teamsChannelId?: string;
}

class Channels extends React.Component<IChannelProps, IChannelState> {
    readonly localize: TFunction;
    private interval: any;
    private isOpenTaskModuleAllowed: boolean;

    constructor(props: IChannelProps) {
        super(props);
        initializeIcons();
        this.localize = this.props.t;
        this.isOpenTaskModuleAllowed = true;
        this.state = {
            channel: props.channels,
            itemsAccount: this.props.channels.length,
            loader: true,
            teamsTeamId: "",
            teamsChannelId: "",
        };
    }

    public componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.setState({
                teamsTeamId: context.teamId,
                teamsChannelId: context.channelId,
            });
        });
        this.props.getChannelsList();
        this.interval = setInterval(() => {
            this.props.getChannelsList();
        }, 60000);
    }

    public componentWillReceiveProps(nextProps: any) {
        this.setState({
            channel: nextProps.channels,
            loader: false
        })
    }

    public componentWillUnmount() {
        clearInterval(this.interval);
    }

    public render(): JSX.Element {
        let keyCount = 0;
        const processItem = (channel: any) => {
            keyCount++;
            const out = {
                key: keyCount,
                content: (
                    <Flex className="listContainer" vAlign="center" fill gap="gap.small">
                        <Flex.Item size="size.small" variables={{ 'size.small': '24%' }}>
                            <Text>{channel.channelName}</Text>
                        </Flex.Item>
                        <Flex.Item size="size.small" variables={{ 'size.small': '73%' }}>
                            <span className="channelAdmin"><Text>{channel.channelAdmins}</Text></span>
                        </Flex.Item>
                        <Flex.Item shrink={0} hAlign="end" vAlign="center">
                            <Overflow channel={channel} title="" />
                        </Flex.Item>
                    </Flex>
                ),
                styles: { margin: '0.2rem 0.2rem 0 0' },
                onClick: (): void => {
                    let url = getBaseUrl() + "/newchannel/" + channel.id + "?locale={locale}";
                    this.onOpenTaskModule(null, url, this.localize("EditChannel"));
                },
            };
            return out;
        };

        const label = this.processLabels();
        const outList = this.state.channel.map(processItem);
        const allChannels = [...label, ...outList];

        if (this.state.loader) {
            return (
                <Loader />
            );
        } else if (this.state.channel.length === 0) {
            return (<div className="results">{this.localize("EmptyChannels")}</div>);
        }
        else {
            return (
                <List selectable items={allChannels} className="list" />
            );
        }
    }

    private processLabels = () => {
        const out = [{
            key: "labels",
            content: (
                <Flex className="listContainer" vAlign="center" fill gap="gap.small">
                    <Flex.Item size="size.small" variables={{ 'size.small': '24%' }}>
                        <Text
                            truncated
                            weight="bold"
                            content={this.localize("ChannelName")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.small" variables={{ 'size.small': '73%' }}>
                        <Text
                            truncated
                            weight="bold"
                            content={this.localize("ChannelAdmins")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item shrink={0} hAlign="end" vAlign="center">
                        <Text
                            truncated
                            weight="bold"
                            content={""}
                        >
                        </Text>
                    </Flex.Item>
                </Flex>
            ),
            styles: { margin: '0.2rem 0.2rem 0 0' },
        }];
        return out;
    }

    private onOpenTaskModule = (event: any, url: string, title: string) => {
        if (this.isOpenTaskModuleAllowed) {
            this.isOpenTaskModuleAllowed = false;
            let taskInfo: ITaskInfo = {
                url: url,
                title: title,
                height: 530,
                width: 1000,
                fallbackUrl: url,
            }

            let submitHandler = (err: any, result: any) => {
                this.props.getChannelsList().then(() => {
                    this.isOpenTaskModuleAllowed = true;
                });
            };

            microsoftTeams.tasks.startTask(taskInfo, submitHandler);
        }
    }
}

const mapStateToProps = (state: any) => {
    return { channels: state.channelList, selectedChannel: state.selectedChannel};
}

const channelsWithTranslation = withTranslation()(Channels);
export default connect(mapStateToProps, { selectChannel, getChannelsList })(channelsWithTranslation);