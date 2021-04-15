import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { withTranslation, WithTranslation } from "react-i18next";
import * as AdaptiveCards from "adaptivecards";
import { Loader, Button, Text, List, Image } from '@stardust-ui/react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Radiobutton, RadiobuttonGroup } from 'msteams-ui-components-react';
import DatePicker from 'react-datepicker';
import "react-datepicker/dist/react-datepicker.css";
import 'bootstrap/dist/css/bootstrap.min.css';
import './sendConfirmationTaskModule.scss';
import { getDraftNotification, getConsentSummaries, sendDraftNotification, updateDraftNotificationPublish } from '../../apis/messageListApi';
import {
    getInitAdaptiveCard, setCardTitle, setCardImageLink, setCardSummary,
    setCardAuthor, setCardBtn
} from '../AdaptiveCard/adaptiveCard';
import {getTemplate } from '../../apis/templateListApi';
import { ImageUtil } from '../../utility/imageutility';
import { TFunction } from "i18next";
import dateFormat from 'dateformat';
// Import Turndown module
const TurndownService = require('turndown').default;
const MarkdownIt = require('markdown-it');

export interface IListItem {
    header: string,
    media: JSX.Element,
}

export interface IMessage {
    id: string;
    title: string;
    acknowledgements?: number;
    reactions?: number;
    responses?: number;
    succeeded?: number;
    failed?: number;
    throttled?: number;
    sentDate?: string;
    imageLink?: string;
    summary?: string;
    author?: string;
    buttonLink?: string;
    buttonTitle?: string;
    publishOn: string;
}

export interface SendConfirmationTaskModuleProps extends RouteComponentProps, WithTranslation {
}

export interface IStatusState {
    message: IMessage;
    loader: boolean;
    teamNames: string[];
    rosterNames: string[];
    groupNames: string[];
    allUsers: boolean;
    messageId: number;
    selectedPublishBtn: string;
    startDate: string;
}

class SendConfirmationTaskModule extends React.Component<SendConfirmationTaskModuleProps, IStatusState> {
    readonly localize: TFunction;
    private initMessage = {
        id: "",
        title: "",
        publishOn:""
    };

    private card: any;

    constructor(props: SendConfirmationTaskModuleProps) {
        super(props);
        this.localize = this.props.t;
        this.card = getInitAdaptiveCard(this.localize);

        this.state = {
            message: this.initMessage,
            loader: true,
            teamNames: [],
            rosterNames: [],
            groupNames: [],
            allUsers: false,
            messageId: 0,
            selectedPublishBtn: "publishNow",
            startDate: "",
        };
    }

    public componentDidMount() {
        microsoftTeams.initialize();

        let params = this.props.match.params;

        if ('id' in params) {
            let id = params['id'];
            this.getItem(id).then(() => {
                if (this.state.message.publishOn !== null) {
                    this.setState({
                        selectedPublishBtn: "publishOn",
                        startDate: this.state.message.publishOn,

                    })
                }
                getConsentSummaries(id).then((response) => {
                  
                    this.setState({
                        teamNames: response.data.teamNames.sort(),
                        rosterNames: response.data.rosterNames.sort(),
                        groupNames: response.data.groupNames.sort(),
                        allUsers: response.data.allUsers,
                        messageId: id,
                    }, () => {
                        this.setState({
                            loader: false
                        }, () => {
                            setCardTitle(this.card, this.state.message.title);
                            setCardImageLink(this.card, this.state.message.imageLink);
                            setCardSummary(this.card, this.state.message.summary);
                            setCardAuthor(this.card, this.state.message.author);
                            if (this.state.message.buttonTitle && this.state.message.buttonLink) {
                                setCardBtn(this.card, this.state.message.buttonTitle, this.state.message.buttonLink);
                            }

                            let adaptiveCard = new AdaptiveCards.AdaptiveCard();
                            AdaptiveCards.AdaptiveCard.onProcessMarkdown = (text, result) => {
                                result.outputHtml = MarkdownIt().render(text);
                                result.didProcess = true;
                            };
                            adaptiveCard.parse(this.card);
                            let renderedCard = adaptiveCard.render();
                            document.getElementsByClassName('adaptiveCardContainer')[0].appendChild(renderedCard);
                            if (this.state.message.buttonLink) {
                                let link = this.state.message.buttonLink;
                                adaptiveCard.onExecuteAction = function (action) { window.open(link, '_blank'); };
                            }
                        });
                    });
                });
            });
        }
    }

    private getItem = async (id: number) => {
        try {
            const response = await getDraftNotification(id);
            const responseTemplate = await getTemplate(response.data.templateID);
            const templateDetails = responseTemplate.data;
            this.card = JSON.parse(templateDetails["templateJSON"]);
            const draftMessageDetail = response.data;
            setCardTitle(this.card, draftMessageDetail.title);
            setCardImageLink(this.card, draftMessageDetail.imageLink);
            // Create an instance of the turndown service
            let turndownService = new TurndownService();
            // Use the turndown method from the created instance
            // to convert the first argument (HTML string) to Markdown
            let markdown = turndownService.turndown(draftMessageDetail.summary);
            setCardSummary(this.card, markdown);
            response.data.summary = markdown;
            setCardAuthor(this.card, draftMessageDetail.author);
            setCardBtn(this.card, draftMessageDetail.buttonTitle, draftMessageDetail.buttonLink);
           
            this.setState({
                message: response.data
            });
        } catch (error) {
            return error;
        }
    }

    public render(): JSX.Element {
        if (this.state.loader) {
            return (
                <div className="Loader">
                    <Loader />
                </div>
            );
        } else {
            return (
                <div className="taskModule">
                    <div className="formContainer">
                        <div className="formContentContainer" >
                            <div className="contentField">
                                <h3>{this.localize("ConfirmToSend")}</h3>
                                <span>{this.localize("SendToRecipientsLabel")}</span>
                            </div>

                            <div className="results">
                                {this.renderAudienceSelection()}
                            </div>
                            <div className="schedule">
                                <span className="label">{this.localize("ScheduleMessage")}</span>
                            <RadiobuttonGroup
                                className="radioBtns"
                                value={this.state.selectedPublishBtn}
                                    onSelected={this.onPublishSelected}>
                                    <Radiobutton name="publish" value="publishNow" label={this.localize("PublishNow")} />
                                    <Radiobutton name="publish" value="publishOn" label={this.localize("PublishOn")} />
                                    <Text
                                        content={this.localize("TimezoneInformation")} />
                                    <div className={this.state.selectedPublishBtn === "publishOn" ? "" : "hide"}>
                                        {this.state.startDate === "" ? <DatePicker selected={new Date()} onChange={this.handleChange} showTimeSelect timeIntervals={60} inline /> : <DatePicker maxDetail="Hour" selected={new Date(this.state.startDate)} onChange={this.handleChange} showTimeSelect timeIntervals={60} inline />}
                                        
                                        </div>
                                
                                </RadiobuttonGroup>
                            </div>
                        </div>
                        <div className="adaptiveCardContainer">
                        </div>
                    </div>

                    <div className="footerContainer">
                        <div className="buttonContainer">
                            <Loader id="sendingLoader" className="hiddenLoader sendingLoader" size="smallest" label={this.localize("PreparingMessageLabel")} labelPosition="end" />
                            {this.state.selectedPublishBtn === "publishOn" ? <Button content={this.localize("Send")} id="saveBtn" onClick={this.onSaveMessage} primary /> : <Button content={this.localize("Send")} id="sendBtn" onClick={this.onSendMessage} primary />}
                        </div>
                    </div>
                </div>
            );
        }
    }
    private handleChange = date => {
        let PublishDate = new Date(date).toJSON();
        console.log(PublishDate);
        this.setState({
            startDate: PublishDate,
            
        })
    }

    private onSaveMessage = () => {
        updateDraftNotificationPublish(this.state.messageId, dateFormat(this.state.startDate, "yyyy-mm-dd HH:MM")).then(() => {
            microsoftTeams.tasks.submitTask();
        });
    }

    private onPublishSelected = (value: any) => {
        this.setState({
            selectedPublishBtn: value,
        });
    }
    private onSendMessage = () => {
        let spanner = document.getElementsByClassName("sendingLoader");
        spanner[0].classList.remove("hiddenLoader");
        if (this.state.selectedPublishBtn === "publishNow") {
            sendDraftNotification(this.state.message).then(() => {
                microsoftTeams.tasks.submitTask();
            });
        }
    }

    private getItemList = (items: string[]) => {
        let resultedTeams: IListItem[] = [];
        if (items) {
            resultedTeams = items.map((element) => {
                const resultedTeam: IListItem = {
                    header: element,
                    media: <Image src={ImageUtil.makeInitialImage(element)} avatar />
                }
                return resultedTeam;
            });
        }
        return resultedTeams;
    }

    private renderAudienceSelection = () => {
        if (this.state.teamNames && this.state.teamNames.length > 0) {
            return (
                <div key="teamNames"> <span className="label">{this.localize("TeamsLabel")}</span>
                    <List items={this.getItemList(this.state.teamNames)} />
                </div>
            );
        } else if (this.state.rosterNames && this.state.rosterNames.length > 0) {
            return (
                <div key="rosterNames"> <span className="label">{this.localize("TeamsMembersLabel")}</span>
                    <List items={this.getItemList(this.state.rosterNames)} />
                </div>);
        } else if (this.state.groupNames && this.state.groupNames.length > 0) {
            return (
                <div key="groupNames" > <span className="label">{this.localize("GroupsMembersLabel")}</span>
                    <List items={this.getItemList(this.state.groupNames)} />
                </div>);
        } else if (this.state.allUsers) {
            return (
                <div key="allUsers">
                    <span className="label">{this.localize("AllUsersLabel")}</span>
                    <div className="noteText">
                        <Text error content={this.localize("SendToAllUsersNote")} />
                    </div>
                </div>);
        } else {
            return (<div></div>);
        }
    }
}

const sendConfirmationTaskModuleWithTranslation = withTranslation()(SendConfirmationTaskModule);
export default sendConfirmationTaskModuleWithTranslation;