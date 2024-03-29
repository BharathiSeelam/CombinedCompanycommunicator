import * as microsoftTeams from "@microsoft/teams-js";
import { Button, Flex, Icon, IconProps, List, Loader, Text } from '@stardust-ui/react';
import { TFunction } from "i18next";
import { TooltipHost } from 'office-ui-fabric-react';
import { CommandBarButton } from 'office-ui-fabric-react/lib/Button';
import { initializeIcons } from 'office-ui-fabric-react/lib/Icons';
import * as React from 'react';
import { CSVLink } from "react-csv";
import { withTranslation, WithTranslation } from "react-i18next";
import Modal from 'react-modal';
import { connect } from 'react-redux';
import { getDraftMessagesList, getMessagesList, selectMessage } from '../../actions';
import { exportNotification } from '../../apis/messageListApi';
import { getBaseUrl } from '../../configVariables';
import { formatNumber } from '../../i18n';
import Overflow from '../OverFlow/sentMessageOverflow';
import './messages.scss';

export interface ITaskInfo {
    title?: string;
    height?: number;
    width?: number;
    url?: string;
    card?: string;
    fallbackUrl?: string;
    completionBotId?: string;
}

export interface IMessage {
    title: string;
    sentDate: string;
    edited: string;
    recipients: string;
    acknowledgements?: string;
    reactions?: string;
    responses?: string;
}

export interface IMessageProps extends WithTranslation {
    messagesList: IMessage[];
    selectMessage?: any;
    getMessagesList?: any;
    getDraftMessagesList?: any;
}

export interface IMessageState {
    message: IMessage[];
    loader: boolean;
    page: string;
    teamId?: string;
    pageOpen: boolean;
    pageError: boolean;
}
const customStyles = {
    content: {
        top: '50%',
        left: '50%',
        right: 'auto',
        bottom: 'auto',
        marginRight: '-20%',
        transform: 'translate(-50%, -50%)'
    }
};

class Messages extends React.Component<IMessageProps, IMessageState> {
    readonly localize: TFunction;
    private interval: any;
    private isOpenTaskModuleAllowed: boolean;
    constructor(props: IMessageProps) {
        super(props);
        initializeIcons();
        this.localize = this.props.t;
        this.isOpenTaskModuleAllowed = true;
        this.state = {
            message: this.props.messagesList,
            loader: true,
            page: "",
            teamId: "",
            pageOpen: false,
            pageError:false,
        };
        this.escFunction = this.escFunction.bind(this);
    }

    public componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.setState({
                teamId: context.teamId,
            });
        });
        this.props.getMessagesList();
        document.addEventListener("keydown", this.escFunction, false);
        this.interval = setInterval(() => {
            this.props.getMessagesList();
        }, 60000);
    }

    public componentWillUnmount() {
        document.removeEventListener("keydown", this.escFunction, false);
        clearInterval(this.interval);
    }

    public componentWillReceiveProps(nextProps: any) {
        if (this.props !== nextProps) {
            this.setState({
                message: nextProps.messagesList,
                loader: false
            });
        }
    }
    
    public toggleModalError() {
        this.setState({ pageError: false });
    }
    public toggleModal() {
        this.setState({ pageOpen: false});
    }
    public render(): JSX.Element {
        let keyCount = 0;
        const processItem = (message: any) => {
            keyCount++;
            const out = {
                key: keyCount,
                content: this.messageContent(message),
                onClick: (): void => {
                    let url = getBaseUrl() + "/viewstatus/" + message.id + "?locale={locale}";
                    this.onOpenTaskModule(null, url, this.localize("ViewStatus"));
                },
                styles: { margin: '0.2rem 0.2rem 0 0' },
            };
            return out;
        };
      
        const csvLink = this.createCSV(this.state.message);
        const label = this.processLabels();
        const outList = this.state.message.map(processItem);
        const allMessages = [...label, ...outList];
        const csv = [...csvLink];

        if (this.state.loader) {
            return (
                <Loader />
            );
        } else if (this.state.message.length === 0) {
            return (<div className="results">{this.localize("EmptySentMessages")}</div>);
        }
        else
        {
            const downloadIcon: IconProps = { name: 'download', size: "medium" };
            return (
                <div>
                    <tr>
                        <td><List selectable items={csv} className="list" /></td>
                        <td>
                            <div className="buttonContainer">
                                
                                <TooltipHost content={"Export All Notification Details"} calloutProps={{ gapSpace: 0 }}>
                                    <Button icon={downloadIcon} content={"Export Details"} id="exportBtn" onClick={this.onExportDetails} primary />
                                </TooltipHost>
                                <div>
                                <Modal
                                                            isOpen={this.state.pageOpen}
                                        style={customStyles}
                                    onDismiss={this.toggleModal}
                                    isBlocking={false}
                                      >
                                        <div>
                                            <div className="displayMessageField">
                                                <br />
                                                <br />
                                            <span><Icon className="iconStyle" name="stardust-checkmark" xSpacing="before" size="largest" outline /></span>
                                    <h1>{this.localize("ExportQueueTitle")}</h1></div>
                                <span>{this.localize("ExportQueueSuccessMessage1")}</span>
                                <br />
                                <br />
                                <span>{this.localize("ExportQueueSuccessMessage2")}</span>
                                <br />
                                            <span>{this.localize("ExportQueueSuccessMessage3")}</span>
                                            <div className="footerContainer">
                                                <div className="buttonContainer">
                                                    <Button content={this.localize("CloseText")} id="closeBtn" onClick={this.toggleModal.bind(this)} primary />
                                                </div>
                                            </div>
                                            </div>
                                        
                                    </Modal>

                                    <Modal
                                        isOpen={this.state.pageError}
                                        style={customStyles}
                                        onDismiss={this.toggleModalError}
                                        isBlocking={false}
                                    >
                                        <div>
                                            <div className="displayMessageField">
                                                <br />
                                                <br />
                                                <div><span><Icon className="iconStyle" name="stardust-close" xSpacing="before" size="largest" outline /></span>
                                                    <h1 className="light">{this.localize("ExportErrorTitle")}</h1></div>
                                                <span>{this.localize("ExportErrorMessage")}</span>
                                            </div>
                                            <div className="footerContainer">
                                                <div className="buttonContainer">
                                                    <Button content={this.localize("CloseText")} id="closeBtn" onClick={this.toggleModalError.bind(this)} primary />
                                                </div>
                                                </div>
                                        </div>

                                    </Modal>
                                    </div>
                            </div>


                         </td>
                    </tr>
                    <List selectable items={allMessages} className="list" />
                </div>
            );
        }
    }

    private onExportDetails = async () => {
        //let spanner = document.getElementsByClassName("sendingLoader");
        //spanner[0].classList.remove("hiddenLoader");
        let payload = {
            id: "dummy",
            teamId: this.state.teamId
        };
        await exportNotification(payload).then(() => {
            this.setState({ pageOpen: true });
        }).catch(() => {
            this.setState({ pageError: true });
        });
    }

    private onClose = () => {
        microsoftTeams.tasks.submitTask();
    }

    private createCSV = (message: any) => {

        const link = [{
            key: "csvlink",
            content: (
                <CSVLink data={message} filename={"TeamActivity.csv"}>
                    <CommandBarButton iconProps={{ iconName: 'ExcelLogoInverse' }} text='Export Summary' />
                </CSVLink>
            ),
            styles: { margin: '0.2rem 0.2rem 0 0' },
        }];
        return link;
    }
    private processLabels = () => {
        const out = [{
            key: "labels",
            content: (
                <Flex vAlign="center" fill gap="gap.small">
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '48%' }}>
                        <Text
                            truncated
                            weight="bold"
                            content={this.localize("TitleText")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item>
                        <Text></Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '25%' }} >
                        <Text
                            truncated
                            content={this.localize("Recipients")}
                            weight="bold"
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '20%' }}>
                        <Text
                            truncated
                            content={this.localize("Likes")}
                            weight="bold"
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '30%' }} >
                        <Text
                            truncated
                            content={this.localize("Sent")}
                            weight="bold"
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '30%' }} >
                        <Text
                            truncated
                            content={this.localize("Edited")}
                            weight="bold"
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

    private renderSendingText = (message: any) => {
        var text = "";
        switch (message.status) {
            case "Queued":
                text = this.localize("Queued");
                break;
            case "SyncingRecipients":
                text = this.localize("SyncingRecipients");
                break;
            case "InstallingApp":
                text = this.localize("InstallingApp");
                break;
            case "Sending":
                let sentCount =
                    (message.succeeded ? message.succeeded : 0) +
                    (message.failed ? message.failed : 0) +
                    (message.unknown ? message.unknown : 0);

                text = this.localize("SendingMessages", { "SentCount": formatNumber(sentCount), "TotalCount": formatNumber(message.totalMessageCount) });
                break;
            case "Sent":
            case "Edited":
            case "Failed":
                text = "";
        }

        return (<Text truncated content={text} />);
    }

    private messageContent = (message: any) => {
        return (
            <Flex className="listContainer" vAlign="center" fill gap="gap.small">
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '48%' }}>
                    <Text
                        truncated
                        content={message.title}
                    >
                    </Text>
                </Flex.Item>
                <Flex.Item>
                    {this.renderSendingText(message)}
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '30%' }}>
                    <div>
                        <TooltipHost content={this.props.t("TooltipSuccess")} calloutProps={{ gapSpace: 0 }}>
                            <Icon name="stardust-checkmark" xSpacing="after" className="succeeded" outline />
                            <span className="semiBold">{formatNumber(message.succeeded)}</span>
                        </TooltipHost>
                        <TooltipHost content={this.props.t("TooltipFailure")} calloutProps={{ gapSpace: 0 }}>
                            <Icon name="stardust-close" xSpacing="both" className="failed" outline />
                            <span className="semiBold">{formatNumber(message.failed)}</span>
                        </TooltipHost>
                        {
                            message.unknown &&
                            <TooltipHost content="Unknown" calloutProps={{ gapSpace: 0 }}>
                                <Icon name="exclamation-circle" xSpacing="both" className="unknown" outline />
                                <span className="semiBold">{formatNumber(message.unknown)}</span>
                            </TooltipHost>
                        }
                    </div>
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '20%' }} >
                    <Text
                        truncated
                        className="semiBold"
                        content={message.likes}
                    />
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '30%' }} >
                    <Text
                        truncated
                        className="semiBold"
                        content={message.sentDate}
                    />
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '30%' }} >
                    <Text
                        truncated
                        className="semiBold"
                        content={message.edited}
                    />
                </Flex.Item>
                <Flex.Item shrink={0} hAlign="end" vAlign="center">
                    <Overflow message={message} title="" />
                </Flex.Item>
            </Flex>
        );
    }

    private escFunction = (event: any) => {
        if (event.keyCode === 27 || (event.key === "Escape")) {
            microsoftTeams.tasks.submitTask();
        }
    }

    

    public onOpenTaskModule = (event: any, url: string, title: string) => {
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
                this.isOpenTaskModuleAllowed = true;
            };

            microsoftTeams.tasks.startTask(taskInfo, submitHandler);
        }
    }
}

const mapStateToProps = (state: any) => {
    return { messagesList: state.messagesList };
}

const messagesWithTranslation = withTranslation()(Messages);
export default connect(mapStateToProps, { selectMessage, getMessagesList, getDraftMessagesList })(messagesWithTranslation);