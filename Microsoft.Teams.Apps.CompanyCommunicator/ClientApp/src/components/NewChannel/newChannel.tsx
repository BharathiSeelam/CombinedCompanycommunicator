import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { withTranslation, WithTranslation } from "react-i18next";
import { Input, TextArea } from 'msteams-ui-components-react';
import { initializeIcons } from 'office-ui-fabric-react/lib/Icons';
import * as AdaptiveCards from "adaptivecards";
import { Button, Loader, Dropdown, Label } from '@stardust-ui/react';
import * as microsoftTeams from "@microsoft/teams-js";
import './newChannel.scss';
import './teamTheme.scss';
import { getChannel, createChannel, updateChannel, getTeams } from '../../apis/channelListApi';
import { getDLUsers, getDLUser } from '../../apis/dlUserListApi';
import { getBaseUrl } from '../../configVariables';
import { getDistributionListsByName } from '../../apis/distributionListApi';
import {
    getInitAdaptiveCard
} from '../AdaptiveCard/adaptiveCard';
import { ImageUtil } from '../../utility/imageutility';
import { TFunction } from "i18next";
import { channelListReducer } from '../../reducers';
type dropdownItem = {
    key: string,
    header: string,
    content: string,
    image: string,
    team: {
        id: string,
    },
}
export interface IChannel {
    id?: string,
    channelName: string,
    channelDescription: string,
    channelAdmins: string,
    channelAdminDLs: string,
    channelAdminEmail: string
}
export interface formState {
    channelName: string,
    channelDescription: string,
    teams?: any[],
    card?: any,
    page: string,
    exists?: boolean,
    channelId: string,
    loader: boolean,
    loading: boolean,
    noResultMessage: string,
    unstablePinned?: boolean,
    dlAdminLabel: string,
    admins?: any[],
    dls?: any[],
    selectedAdmins: string,
    selectedDLs: string,
    selectedAdminEmail: string,
    dlAdminEmail: string,
    userNameReadonly: boolean,
    invalidDlEmailErrorMessageLabel: boolean;
    invalidDlEmailErrorMessage: string;
    selectedDlNames: string;
}
export interface INewChannelProps extends RouteComponentProps, WithTranslation {
    getChannelsList?: any;
}
class NewChannel extends React.Component<INewChannelProps, formState> {
    readonly localize: TFunction;
    private card: any;
    constructor(props: INewChannelProps) {
        super(props);
        initializeIcons();
        this.localize = this.props.t;
        this.card = getInitAdaptiveCard(this.localize);
        this.state = {
            channelName: "",
            channelDescription: "",
            selectedAdmins: "",
            selectedDLs: "",
            card: this.card,
            page: "ChannelCreation",
            channelId: "",
            loader: true,
            loading: false,
            noResultMessage: "",
            unstablePinned: true,
            dlAdminLabel: "",
            dlAdminEmail: "",
            selectedAdminEmail: "",
            userNameReadonly: true,
            invalidDlEmailErrorMessageLabel: false,
            invalidDlEmailErrorMessage: "",
            selectedDlNames: ""
        }
        // this.onChannelAdminChanged = this.onChannelAdminChanged.bind(this);
        // this.getAdminData();
        //this.getDLData();
    }
    public async componentDidMount() {
        microsoftTeams.initialize();
        //- Handle the Esc key
        document.addEventListener("keydown", this.escFunction, false);
        let params = this.props.match.params;
        this.getTeamList().then(() => {
            if ('id' in params) {
                let id = params['id'];
                this.getItem(id).then(() => {
                    //const selectedDLs = this.makeDLDropdownItemList(this.state.selectedDLs, this.state.teams);
                    //const selectedAdmins = this.makeDropdownItemList(this.state.selectedAdmins, this.state.selectedAdminEmail, this.state.teams);
                    this.setState({
                        exists: true,
                        channelId: id,
                        selectedDLs: this.state.selectedDLs,
                        selectedAdmins: this.state.selectedAdmins,
                        selectedAdminEmail: this.state.selectedAdminEmail,
                        selectedDlNames: this.state.selectedDlNames
                    })
                });
            } else {
                this.setState({
                    exists: false,
                    loader: false
                }, () => {
                    let adaptiveCard = new AdaptiveCards.AdaptiveCard();
                    adaptiveCard.parse(this.state.card);
                })
            }
        });
    }
    //private makeDropdownItems = (items: any[] | undefined) => {
    //    const resultedTeams: dropdownItem[] = [];
    //    if (items) {
    //        const key = 'userName';
    //        //const uniqueItems = [...new Map(items.map(item => [item[key], item])).values()];
    //        items.forEach((element) => {
    //            if (element.userEmail !== null || element.userName !== null) {
    //                resultedTeams.push({
    //                    key: element.userName,
    //                    header: element.userName,
    //                    content: element.userEmail,
    //                    image: ImageUtil.makeInitialImage(element.userName),
    //                    team: {
    //                        id: element.userID
    //                    },

    //                });
    //            }
    //        });
    //    }
    //    return resultedTeams;
    //}
    //private makeDropdownItemList = (items: any[], emailItems: any[], fromItems: any[] | undefined) => {
    //    items = items.toString().split(',');
    //    emailItems = emailItems.toString().split(',');
    //    const dropdownItemList: dropdownItem[] = [];
    //    if (items) {
    //        items.forEach((element, index) => {
    //            dropdownItemList.push({
    //                key: element,
    //                header: element,
    //                content: emailItems[index],
    //                image: ImageUtil.makeInitialImage(element),
    //                team: {
    //                    id: element
    //                },
    //            })
    //        });
    //    }
    //    return dropdownItemList;
    //}

    //private makeDLDropdownItems = (items: any[] | undefined) => {
    //    const resultedTeams: dropdownItem[] = [];
    //    if (items) {
    //        items.forEach((element) => {
    //            resultedTeams.push({
    //                key: element.dlid,
    //                header: element.dlName + "(" + element.dlMemberCount + ")",
    //                content: element.dlMail,
    //                image: ImageUtil.makeInitialImage(element.dlName),
    //                team: {
    //                    id: element.dlid
    //                },
    //            });
    //        });
    //    }
    //    return resultedTeams;
    //}
    //private makeDLDropdownItemList = (items: any[], fromItems: any[] | undefined) => {
    //    items = items.toString().split(',');
    //    const dropdownItemList: dropdownItem[] = [];
    //    if (items) {
    //        items.forEach((element) => {
    //            dropdownItemList.push({
    //                key: element,
    //                header: element,
    //                content: "",
    //                image: ImageUtil.makeInitialImage(element),
    //                team: {
    //                    id: element
    //                },
    //            })
    //        });
    //    }
    //    return dropdownItemList;
    //}
    private getTeamList = async () => {
        try {
            const response = await getTeams();
            this.setState({
                teams: response.data
            });
        } catch (error) {
            return error;
        }
    }
    //private getAdminData = async () => {
    //    try {
    //        const response = await getDLUsers();
    //        this.setState({
    //            admins: response.data
    //        });
    //    }
    //    catch (error) {
    //        return error;
    //    }
    //}

    //private getDLData = async () => {
    //    try {
    //        const response = await getDistributionLists();
    //        this.setState({
    //            dls: response.data
    //        });
    //    }
    //    catch (error) {
    //    }
    //}
    private getItem = async (id: string) => {
        try {
            const response = await getChannel(id);
            const ChannelDetail = response.data;
            let dlNames: any[] = [];
            let dlEmails: any[] = ChannelDetail.channelAdminDLs.split(",");
            await Promise.all(dlEmails.map(async (data) => {
                await getDistributionListsByName(data).then(result => {
                    dlNames.push(result.data[0]["dlName"] + "(" + result.data[0]["dlMemberCount"] + ")");
                });
            })).then(result => {
                this.setState({
                    channelName: ChannelDetail.channelName,
                    channelDescription: ChannelDetail.channelDescription,
                    selectedAdmins: ChannelDetail.channelAdmins,
                    selectedDLs: ChannelDetail.channelAdminDLs,
                    selectedAdminEmail: ChannelDetail.channelAdminEmail,
                    selectedDlNames: dlNames.join(","),
                    loader: false
                });
            });
        } catch (error) {
            return error;
        }
    }
    public componentWillUnmount() {
        document.removeEventListener("keydown", this.escFunction, false);
    }
    public render(): JSX.Element {
        if (this.state.loader) {
            return (
                <div className="Loader">
                    <Loader />
                </div>
            );
        } else {
            if (this.state.page === "ChannelCreation") {
                return (
                    <div className="taskModule">
                        <div className="formContainer">
                            <div className="formChannelContainer" >
                                <Input
                                    className="inputField"
                                    value={this.state.channelName}
                                    label={this.localize("ChannelName")}
                                    placeholder={this.localize("PlaceHolderChannelName")}
                                    onChange={this.onChannelNameChanged}
                                    autoComplete="off"
                                />
                                <br />
                                <TextArea
                                    className="inputField textArea"
                                    autoFocus
                                    placeholder={this.localize("PlaceHolderDescription")}
                                    label={this.localize("Description")}
                                    value={this.state.channelDescription}
                                    onChange={this.onDescriptionChanged}
                                />
                                <br />

                                <Input
                                    className="inputField"
                                    label={this.localize("AdminsForThisChannel")}
                                    placeholder={this.localize("ChannelAdmin")}
                                    defaultValue={this.state.selectedAdminEmail}
                                    onChange={this.onChannelAdminChanged}

                                    // onBlur={this.onChannelAdminChanged.bind(this)}
                                    autoComplete="off"
                                />
                                <br />
                                <Label className="inputField label">{this.localize("AdminUserNameLabel")}</Label>
                                <br />
                                <Label className="inputField adminLabel">{this.state.selectedAdmins}</Label>
                            </div>
                        </div>
                        <div className="footerContainer">
                            <div className="buttonContainer">
                                <Button content={this.localize("Next")} id="saveBtn" disabled={this.isNextBtnDisabled()} onClick={this.onNext} primary />
                            </div>
                        </div>
                    </div>
                );
            }
            else if (this.state.page === "DLSelection") {
                return (
                    <div className="taskModule">
                        <div className="formContainer">
                            <div className="formChannelContainer" >
                                <br />
                                <Label className="inputField label">{this.localize("AddDLsFor")}</Label>
                                {/*<Dropdown*/}
                                {/*    className="channelDropdown"*/}
                                {/*    placeholder={this.localize("ChannelDLs")}*/}
                                {/*    search*/}
                                {/*    multiple*/}
                                {/*    loading={this.state.loading}*/}
                                {/*    loadingMessage={this.localize("LoadingText")}*/}
                                {/*    items={this.getAdminDLItems()}*/}
                                {/*    value={this.state.selectedDLs}*/}
                                {/*    onSelectedChange={this.onAdminDLChanged.bind(this)}*/}
                                {/*    unstable_pinned={this.state.unstablePinned}*/}
                                {/*    noResultsMessage={this.localize("NoMatchMessage")}*/}
                                {/*/>*/}
                                <Input
                                    className="inputField"
                                    placeholder={this.localize("ChannelDLs")}
                                    defaultValue={this.state.selectedDLs}
                                    onChange={this.onAdminDLChanged}

                                    // onBlur={this.onChannelAdminChanged.bind(this)}
                                    autoComplete="off"
                                />
                                {this.state.invalidDlEmailErrorMessageLabel ? <Label className="inputField dlLabel">{this.state.invalidDlEmailErrorMessage}</Label> : ""}
                                <br />
                                <Label className="inputField label">{this.localize("DLNameLabel")}</Label>
                                <br />
                                <Label className="inputField adminLabel">{this.state.selectedDlNames}</Label>
                            </div>
                        </div>
                        <div className="footerContainer">
                            <div className="buttonContainer">
                                <Button content={this.localize("Back")} onClick={this.onBack} secondary />
                                <Button content={this.localize("SaveChannel")} id="saveBtn" disabled={this.isSaveBtnDisabled()} onClick={this.onSave} primary />
                            </div>
                        </div>
                    </div>
                );
            } else {
                return (<div>Error</div>);
            }
        }
    }
    private isSaveBtnDisabled = () => {
        return !(this.state.channelName !== "" && this.state.selectedAdmins.length !== 0 && this.state.selectedDLs.length !== 0 && this.state.invalidDlEmailErrorMessage.length == 0);
    }
    private isNextBtnDisabled = () => {
        return !(this.state.channelName !== "" && this.state.selectedAdmins.length !== 0);
    }
    private onSave = () => {

        let channelAdmins: string = this.state.selectedAdmins;

        let channelAdminsEmail: string = this.state.selectedAdminEmail;
        let channelAdminDLs: string = this.state.selectedDLs;
        const channel: IChannel = {
            id: this.state.channelId,
            channelName: this.state.channelName,
            channelDescription: this.state.channelDescription,
            channelAdmins: channelAdmins,
            channelAdminDLs: channelAdminDLs,
            channelAdminEmail: channelAdminsEmail
        };
        if (this.state.exists) {
            this.editChannel(this.state.channelId, channel).then(() => {
                microsoftTeams.tasks.submitTask();
            });
        } else {
            this.postChannel(channel).then(() => {
                microsoftTeams.tasks.submitTask();
            });
        }
    }
    //private getAdminItems = () => {
    //    if (this.state.admins) {
    //        return this.makeDropdownItems(this.state.admins);
    //    }
    //    const dropdownItems: dropdownItem[] = [];
    //    return dropdownItems;
    //}
    //private getAdminDLItems = () => {
    //    if (this.state.dls) {
    //        return this.makeDLDropdownItems(this.state.dls);
    //    }
    //    const dropdownItems: dropdownItem[] = [];
    //    return dropdownItems;
    //}
    private editChannel = async (id: string, channel: IChannel) => {
        try {
            await updateChannel(id, channel);
        } catch (error) {
            return error;
        }
    }
    private postChannel = async (channel: IChannel) => {
        try {
            await createChannel(channel);
        } catch (error) {
            throw error;
        }
    }
    public escFunction(event: any) {
        if (event.keyCode === 27 || (event.key === "Escape")) {
            microsoftTeams.tasks.submitTask();
        }
    }
    private onNext = (event: any) => {
        this.setState({
            page: "DLSelection"
        });

    }
    private onBack = (event: any) => {
        this.setState({
            page: "ChannelCreation"
        });
    }

    private onChannelNameChanged = (event: any) => {
        this.setState({
            channelName: event.target.value,
            card: this.card
        });
    }
    private onDescriptionChanged = (event: any) => {
        this.setState({
            channelDescription: event.target.value,
            card: this.card
        });
    }
    private onChannelAdminChanged = async (event: any) => {
        var dlUserEmails = event.target.value;
        this.setState({ userNameReadonly: false });
        if (dlUserEmails.endsWith(".com") || dlUserEmails.endsWith(".COM")) {
            this.setState({
                selectedAdminEmail: dlUserEmails.toString(),
            });
            var emails = dlUserEmails.split(",");
            const dlUserNames: string[] = [];
            //let dlUserNames = new Promise((resolve, reject) => { this.getUserName(dlUserEmails); })
            await Promise.all(emails.map(async function (obj) {
                await getDLUser(obj).then(result => {
                    let data = result.data;
                    if (data.length > 0) {
                        dlUserNames.push(data[0]["userName"]);
                    }
                    return dlUserNames;
                });

            })).then(result => {
                this.setState({
                    selectedAdmins: dlUserNames.toString(),
                });
            }).then(result => { this.setState({ userNameReadonly: true }); });
        }
        else {
            if (dlUserEmails == "") {
                this.setState({
                    selectedAdmins: "",
                    selectedAdminEmail: ""
                });
            }
        }


    }

    private onAdminDLChanged = async (event: any) => {
        var dlEmails = event.target.value;
        var dlNames: any[] = [];
        let invalidDLEmails: any[] = [];
        if (dlEmails.endsWith(".com") || dlEmails.endsWith(".COM")) {
            var emails = dlEmails.split(",");
            await Promise.all(emails.map(async function (obj) {
                await getDistributionListsByName(obj).then(result => {
                    let data = result.data;
                    if (data.length === 0) {
                        invalidDLEmails.push(obj);
                    }
                    else {
                        dlNames.push(data[0]["dlName"] + "(" + data[0]["dlMemberCount"] + ")");
                    }
                    return invalidDLEmails;
                });
            })).then(result => {
                if (invalidDLEmails.length > 1) {
                    let invalidDLEmailString: string = invalidDLEmails.join(",");
                    this.setState({
                        selectedDLs: dlEmails.toString(),
                        invalidDlEmailErrorMessage: "Please verify the Dl Emails " + invalidDLEmailString + " are valid",
                        invalidDlEmailErrorMessageLabel: true,
                        selectedDlNames: dlNames.join(",")
                    });
                }
                else if (invalidDLEmails.length == 1) {
                    let invalidDLEmailString: string = invalidDLEmails[0];
                    this.setState({
                        selectedDLs: dlEmails.toString(),
                        invalidDlEmailErrorMessage: "Please verify the Dl Emails " + invalidDLEmailString + " is valid",
                        invalidDlEmailErrorMessageLabel: true,
                        selectedDlNames: dlNames.join(",")
                    });
                }
                else {
                    this.setState({
                        selectedDLs: dlEmails.toString(),
                        invalidDlEmailErrorMessage: "",
                        invalidDlEmailErrorMessageLabel: false,
                        selectedDlNames: dlNames.join(",")
                    });
                }
            });
        }
        else {
            if (dlEmails == "") {
                this.setState({
                    selectedDLs: "",
                    selectedDlNames: ""
                });
            }
        }
    }
}
const newChannelWithTranslation = withTranslation()(NewChannel);
export default newChannelWithTranslation;