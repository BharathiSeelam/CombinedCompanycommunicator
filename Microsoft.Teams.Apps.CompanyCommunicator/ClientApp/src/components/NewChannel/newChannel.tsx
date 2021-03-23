import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { withTranslation, WithTranslation } from "react-i18next";
import { Input, TextArea} from 'msteams-ui-components-react';
import { initializeIcons } from 'office-ui-fabric-react/lib/Icons';
import * as AdaptiveCards from "adaptivecards";
import { Button, Loader, Dropdown, Label} from '@stardust-ui/react';
import * as microsoftTeams from "@microsoft/teams-js";
import './newChannel.scss';
import './teamTheme.scss';
import { getChannel, createChannel, updateChannel, getTeams } from '../../apis/channelListApi';
import { getDLUsers } from '../../apis/dlUserListApi';
import { getBaseUrl } from '../../configVariables';
import { getDistributionLists } from '../../apis/distributionListApi';
import {
    getInitAdaptiveCard
} from '../AdaptiveCard/adaptiveCard';
import { ImageUtil } from '../../utility/imageutility';
import { TFunction } from "i18next";

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
    channelAdminEmail:string
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
    selectedAdmins: dropdownItem[],    
    selectedDLs: dropdownItem[],
    selectedAdminEmail:any[],
    dlAdminEmail:string,
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
            selectedAdmins: [], 
            selectedDLs: [],             
            card: this.card,
            page: "ChannelCreation",
            channelId: "",
            loader: true,
            loading: false,
            noResultMessage: "",
            unstablePinned: true,
            dlAdminLabel: "",
            dlAdminEmail:"",
            selectedAdminEmail: []
        }
        this.getAdminData();
        this.getDLData();
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
                    const selectedDLs = this.makeDLDropdownItemList(this.state.selectedDLs, this.state.teams);
                    const selectedAdmins = this.makeDropdownItemList(this.state.selectedAdmins, this.state.selectedAdminEmail, this.state.teams);                    
                    this.setState({
                        exists: true,
                        channelId: id,
                        selectedDLs: selectedDLs,
                        selectedAdmins: selectedAdmins,
                        selectedAdminEmail:this.state.selectedAdminEmail,
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

    private makeDropdownItems = (items: any[] | undefined) => {
        const resultedTeams: dropdownItem[] = [];         
        if (items) {
            const key = 'userName';
            //const uniqueItems = [...new Map(items.map(item => [item[key], item])).values()];
            items.forEach((element) => {
                if (element.userEmail !== null || element.userName !== null) {
                    resultedTeams.push({
                        key: element.userEmail,
                        header: element.userName,
                        content: element.userEmail,
                        image: ImageUtil.makeInitialImage(element.userName),
                        team: {
                            id: element.userEmail
                        },

                    });
                }
            });
        }
        return resultedTeams;
    }
    private makeDropdownItemList = (items: any[],emailItems:any[], fromItems: any[] | undefined) => {
        items = items.toString().split(',');
        emailItems = emailItems.toString().split(',');
        const dropdownItemList: dropdownItem[] = [];
        if (items) {
            items.forEach((element,index) => {
                dropdownItemList.push({
                    key: element,
                    header: element,
                    content: emailItems[index],
                    image: ImageUtil.makeInitialImage(element),
                    team: {
                        id: element
                    },
                })

            });
        }
        return dropdownItemList;
    }
    
    private makeDLDropdownItems = (items: any[] | undefined) => {
        const resultedTeams: dropdownItem[] = []; 
        if (items) {            
            items.forEach((element) => {
                resultedTeams.push({
                    key: element.dlid,
                    header: element.dlName+"("+element.dlMemberCount+")",
                    content: element.dlMail,
                    image: ImageUtil.makeInitialImage(element.dlName),
                    team: {
                        id: element.dlid
                    },
                });
            });
        }
        return resultedTeams;
    }
    private makeDLDropdownItemList = (items: any[], fromItems: any[] | undefined) => {
        items = items.toString().split(',');
        const dropdownItemList: dropdownItem[] = [];
        if (items) {
            items.forEach((element) => {
                dropdownItemList.push({
                    key: element,
                    header: element,
                    content: "",
                    image: ImageUtil.makeInitialImage(element),
                    team: {
                        id: element
                    },
                })

            });
        }
        return dropdownItemList;
    }
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
    private getAdminData = async () => {
        try {
            const response = await getDLUsers();
            this.setState({
                admins: response.data
            });
        }
        catch (error) {
            return error;
        }
    }

   

    private getDLData = async () => {
        try {
            const response = await getDistributionLists();
            this.setState({
                dls: response.data
            });
        }
        catch (error) {

        }
    }
    private getItem = async (id: string) => {
        try {
            const response = await getChannel(id);
            const ChannelDetail = response.data;                   
            this.setState({
                channelName: ChannelDetail.channelName,
                channelDescription: ChannelDetail.channelDescription,                
                selectedAdmins: ChannelDetail.channelAdmins, 
                selectedDLs: ChannelDetail.channelAdminDLs,
                selectedAdminEmail: ChannelDetail.channelAdminEmail,
                loader: false
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
                                <Label className="inputField label">{this.localize("AdminsForThisChannel")}</Label>
                                <Dropdown
                                    className="channelDropdown"
                                    placeholder={this.localize("ChannelAdmin")}
                                    search
                                    multiple
                                    loading={this.state.loading}
                                    loadingMessage={this.localize("LoadingText")}
                                    items={this.getAdminItems()}
                                    value={this.state.selectedAdmins}
                                    onSelectedChange={this.onChannelAdminChanged.bind(this)}
                                    unstable_pinned={this.state.unstablePinned}
                                    noResultsMessage={this.localize("NoMatchMessage")}
                                />
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
                                <br/>
                                <Label className="inputField label">{this.localize("AddDLsFor")}{ this.state.dlAdminLabel}</Label>
                                <Dropdown
                                    className="channelDropdown"
                                    placeholder={this.localize("ChannelDLs")}
                                    search
                                    multiple
                                    loading={this.state.loading}
                                    loadingMessage={this.localize("LoadingText")}
                                    items={this.getAdminDLItems()}
                                    value={this.state.selectedDLs}
                                    onSelectedChange={this.onAdminDLChanged.bind(this)}
                                    unstable_pinned={this.state.unstablePinned}
                                    noResultsMessage={this.localize("NoMatchMessage")}
                                />
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
        return !(this.state.channelName !== "" && this.state.selectedAdmins.length !== 0 && this.state.selectedDLs.length !== 0);
    }

    private isNextBtnDisabled = () => {
        return !(this.state.channelName !== "" && this.state.selectedAdmins.length !== 0 );
    }

    private onSave = () => {
        let channelAdmins: any[] = this.state.selectedAdmins.map(a => a.header);
        let channelAdminsEmail: any[] = this.state.selectedAdmins.map(a => a.content);
        let channelAdminDLs: any[] = this.state.selectedDLs.map(a => a.header);
        const channel: IChannel = {
            id: this.state.channelId,
            channelName: this.state.channelName,
            channelDescription: this.state.channelDescription,
            channelAdmins: channelAdmins.join(','), 
            channelAdminDLs: channelAdminDLs.join(','),         
            channelAdminEmail: channelAdminsEmail.join(',')
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
    private getAdminItems = () => {
        if (this.state.admins) {
            return this.makeDropdownItems(this.state.admins);
        }
        const dropdownItems: dropdownItem[] = [];
        return dropdownItems;
    }


    private getAdminDLItems = () => {
        if (this.state.dls) {
            return this.makeDLDropdownItems(this.state.dls);
        }
        const dropdownItems: dropdownItem[] = [];
        return dropdownItems;
    }

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
        let dlAdminLabel: any[] = [];
        let dlAdminEmail: any[] = [];
        let dlAdminLabelString: string = "";
        let dlAdminEmailLabelString: string = ""
        if (this.state.selectedAdmins.length !== 0) {
            this.state.selectedAdmins.forEach((element) => {
                dlAdminLabel.push(element.header); 
                dlAdminEmail.push(element.content);
            });
            dlAdminLabelString = dlAdminLabel.join(',');
            dlAdminEmailLabelString = dlAdminEmail.join(',');
            this.setState({
                dlAdminLabel: dlAdminLabelString,
                dlAdminEmail:dlAdminEmailLabelString
            });              
        }        
        
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

    private onChannelAdminChanged = async (event: any, itemsData: any) => {   
        this.setState({
            selectedAdmins: itemsData.value,
        });
        let dlAdminLabel: any[] = [];
        let dlAdminEmail: any[] = [];
        let dlAdminLabelString: string = "";
        let dlAdminEmailLabelString: string = ""
        if (this.state.selectedAdmins.length !== 0) {
            this.state.selectedAdmins.forEach((element) => {
                dlAdminLabel.push(element.key);
                dlAdminEmail.push(element.content);
            });
            dlAdminLabelString = dlAdminLabel.join(',');
            dlAdminEmailLabelString = dlAdminEmail.join(',');
            this.setState({
                dlAdminLabel: dlAdminLabelString,
                dlAdminEmail: dlAdminEmailLabelString
            }); ;
        }       
    } 
    private onAdminDLChanged = (event: any, itemsData: any) => {
        this.setState({
            selectedDLs: itemsData.value
        })
    }   
}

const newChannelWithTranslation = withTranslation()(NewChannel);
export default newChannelWithTranslation;
