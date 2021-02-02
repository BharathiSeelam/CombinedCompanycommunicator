import React from 'react';
import { connect } from 'react-redux';
import { withTranslation, WithTranslation } from "react-i18next";
import { Menu } from '@stardust-ui/react';
import * as microsoftTeams from "@microsoft/teams-js";

import { getBaseUrl } from '../../configVariables';
import { selectChannel, getChannelsList } from '../../actions';
import { deleteChannel} from '../../apis/channelListApi';
import { TFunction } from "i18next";

export interface ChannelOverflowProps extends WithTranslation {
    channel: any;
    styles?: object;
    title?: string;
    selectChannel?: any;
    getChannelsList?: any;
}

export interface ChannelOverflowState {
    teamsTeamId?: string;
    teamsChannelId?: string;
    menuOpen: boolean;
}

export interface ITaskInfo {
    title?: string;
    height?: number;
    width?: number;
    url?: string;
    card?: string;
    fallbackUrl?: string;
    completionBotId?: string;
}

class Overflow extends React.Component<ChannelOverflowProps, ChannelOverflowState> {
    readonly localize: TFunction;
    constructor(props: ChannelOverflowProps) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            teamsChannelId: '',
            teamsTeamId: '',
            menuOpen: false,
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
    }

    public render(): JSX.Element {
        const items = [
            {
                key: 'more',
                icon: {
                    name: 'more',
                    outline: true,
                },
                menuOpen: this.state.menuOpen,
                active: this.state.menuOpen,
                indicator: false,
                menu: {
                    items: [                        
                        {
                            key: 'edit',
                            content: this.localize("Edit"),
                            onClick: (event: any) => {
                                event.stopPropagation();
                                this.setState({
                                    menuOpen: false,
                                });
                                let url = getBaseUrl() + "/newchannel/" + this.props.channel.id + "?locale={locale}";
                                this.onOpenTaskModule(null, url, this.localize("EditChannel"));
                            }
                        },                        
                        {
                            key: 'delete',
                            content: this.localize("Delete"),
                            onClick: (event: any) => {
                                event.stopPropagation();
                                this.setState({
                                    menuOpen: false,
                                });
                                this.deleteChannel(this.props.channel.id).then(() => {
                                    this.props.getChannelsList();
                                });
                            }
                        }
                    ],
                },
                onMenuOpenChange: (e: any, { menuOpen }: any) => {
                    this.setState({
                        menuOpen: !this.state.menuOpen
                    });
                },
            },
        ];

        return <Menu className="menuContainer" iconOnly items={items} styles={this.props.styles} title={this.props.title} />;
    }

    private onOpenTaskModule = (event: any, url: string, title: string) => {
        let taskInfo: ITaskInfo = {
            url: url,
            title: title,
            height: 530,
            width: 1000,
            fallbackUrl: url,
        };

        let submitHandler = (err: any, result: any) => {
            this.props.getChannelsList();
        };

        microsoftTeams.tasks.startTask(taskInfo, submitHandler);
    }

    private deleteChannel = async (id: number) => {
        try {
            await deleteChannel(id);
        } catch (error) {
            return error;
        }
    }
}

const mapStateToProps = (state: any) => {
    return { channels: state.channelList, selectedChannel: state.selectedChannel };
}

const ChannelOverflowWithTranslation = withTranslation()(Overflow);
export default connect(mapStateToProps, { selectChannel, getChannelsList })(ChannelOverflowWithTranslation);
