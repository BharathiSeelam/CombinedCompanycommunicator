import * as React from 'react';
import { connect } from 'react-redux';
import { withTranslation, WithTranslation } from "react-i18next";
import { initializeIcons } from 'office-ui-fabric-react/lib/Icons';
import { Loader, List, Flex, Text } from '@stardust-ui/react';
import * as microsoftTeams from "@microsoft/teams-js";
import './channelAdmins.scss';
import { getChannelsList } from '../../actions';
import { TFunction } from "i18next";


export interface IChannelAdmin {
    id: string;
    channelName: string;
    channelAdmins: string;
    channelAdminDLs: string;
}

export interface IChannelAdminProps extends WithTranslation {
    channels: IChannelAdmin[];
    selectedChannel: any;
    selectChannel?: any;
    getChannelsList?: any;
}

export interface IChannelAdminState {
    channel: IChannelAdmin[];
    itemsAccount: number;
    loader: boolean;
    teamsTeamId?: string;
    teamsChannelId?: string;
}

class ChannelAdmins extends React.Component<IChannelAdminProps, IChannelAdminState> {
    readonly localize: TFunction;
    private interval: any;
    private isOpenTaskModuleAllowed: boolean;

    constructor(props: IChannelAdminProps) {
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
        let reg = /(\(.*?\))/gi;
        const processItem = (channel: any) => {
            keyCount++;
            const out = {
                key: keyCount,
                content: (
                    <Flex className="listContainer" vAlign="center" fill gap="gap.small">
                        <Flex.Item size="size.small" shrink={0} grow={1}>
                            <Text>{channel.channelName}</Text>
                        </Flex.Item>
                        <Flex.Item size="size.small" variables={{ 'size.small': '20%' }} shrink={0} grow={1}>
                            <Text>{channel.channelAdmins}</Text>
                        </Flex.Item>
                        <Flex.Item size="size.small" variables={{ 'size.small': '20%' }} shrink={0} grow={1}>
                            <Text>{channel.channelAdminDLs.replace(reg, "")}</Text>
                        </Flex.Item>
                   
                    </Flex>
                ),
                styles: { margin: '0.2rem 0.2rem 0 0' },
                onClick: (): void => {
                  //  let url = getBaseUrl() + "/newchannel/" + channel.id + "?locale={locale}";
                    //this.onOpenTaskModule(null, url, this.localize("EditChannel"));
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
                    <Flex.Item size="size.small" variables={{ 'size.small': '24%' }} grow={1}>
                        <Text
                            truncated
                            weight="bold"
                            content={this.localize("ChannelName")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.small" variables={{ 'size.small': '36%' }} shrink={true}>
                        <Text
                            truncated
                            weight="bold"
                            content={this.localize("ChannelAdmins")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.small" variables={{ 'size.small': '36%' }} shrink={true}>
                        <Text
                            truncated
                            weight="bold"
                            content={this.localize("ChannelAdminDLs")}
                        >
                        </Text>
                    </Flex.Item>
                </Flex>
            ),
            styles: { margin: '0.2rem 0.2rem 0 0' },
        }];
        return out;
    }

}
const mapStateToProps = (state: any) => {
    return { channels: state.channelList, selectedChannel: state.selectedChannel };
}

const channelAdminsWithTranslation = withTranslation()(ChannelAdmins);
export default connect(mapStateToProps, {  getChannelsList })(channelAdminsWithTranslation);
