import * as React from 'react';
import { connect } from 'react-redux';
import * as microsoftTeams from "@microsoft/teams-js";
import { withTranslation, WithTranslation } from "react-i18next";
import { Loader, Text, Grid, gridBehavior } from "@fluentui/react-northstar";
import SearchBox from "../SearchBox/searchBox";
import { TFunction } from "i18next";
import './dashboard.scss';
import { getUserDashboardMessagesList } from '../../actions';
import { initializeIcons } from 'office-ui-fabric-react/lib/Icons';
import '../MessageCards/messageCards.scss';
import MessageCard from '../MessageCards/messageCards';
import { getBaseUrl } from '../../configVariables';
import { EyeIcon } from "@fluentui/react-icons-northstar";
import Constants from '../../constants/constants';
let loggedinUser;
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
    imageLink: string;
    account: string;
    summary: string;
}
export interface IDashboardProps extends WithTranslation {
    getUserDashboardMessagesList?: any;
    messagesList: IMessage[];  
}

export interface IDashboardContainerState {
    messageURL: string;
    message: IMessage[];
    loader: boolean;
    windowWidth: number;
    searchText: string;
}

class Dashboard extends React.Component<IDashboardProps, IDashboardContainerState> {

    readonly localize: TFunction;
    private interval: any;
    private isOpenTaskModuleAllowed: boolean;
    constructor(props: IDashboardProps) {
        super(props);
        initializeIcons();
        this.localize = this.props.t;
        this.isOpenTaskModuleAllowed = true;
        this.state = {
            messageURL: getBaseUrl() + "/newmessage?locale={locale}",
            message: this.props.messagesList,
            windowWidth: window.innerWidth,
            loader: true,
            searchText: "",
        };
       
    }

    public async componentDidMount() {
        //microsoftTeams.initialize();
        microsoftTeams.initialize();
      
            //alert("Dashboard");
            window.addEventListener("resize", this.setWindowWidth);
            // this.props.getUserDashboardMessagesList();
            this.interval = setInterval(() => {
                this.props.getUserDashboardMessagesList();
            }, 6000);
       
      
    }
   public componentWillReceiveProps(nextProps: any) {
       if (this.props.messagesList.length !== nextProps.messagesList.length) {
            this.setState({
                message: nextProps.messagesList,
                loader: false
            });

        }
    }

    /**
  * Get window width real time
  */
    private setWindowWidth = () => {
        if (window.innerWidth !== this.state.windowWidth) {
            this.setState({ windowWidth: window.innerWidth });
        }
    };

    /**
  * Method to set search text given in the search box.
  */
    public handleSearchInputChange = async (searchText: string) => {
        let filterMessage = this.state.message;
        const messagefilter = filterMessage.filter((obj) => {
            let search = searchText.toLowerCase();
            let title = obj.title.toLowerCase();
            if (title.includes(search))
                {
                return obj;
            }
        }); 
        console.log(messagefilter);
        if (searchText.length > 0) {
            await this.setState({
                message: messagefilter,
                searchText: searchText
            });
        }
        else {
            await this.setState({
                message: this.props.messagesList,
                searchText: ""
            });
        }
       // await this.setState({
         //   searchText: searchText
       // })
    }

    public render(): JSX.Element {
        if (this.state.loader) {
            return (
                <div className="container-div">
                    <div className="container-subdiv">
                        <div className="loader">
                            <Loader />
                        </div>
                    </div>
                </div>
            );
        }
        else {
            const cards = new Array<any>();
            const tiles = this.state.message.map((value: IMessage) => (
                <MessageCard messagesList={value} />
            ));

            // Cards component array to be rendered in grid.
            let columns = (this.state.windowWidth > Constants.screenWidthMax) ? 4
                : (this.state.windowWidth >= Constants.screenWidthDefault && this.state.windowWidth < Constants.screenWidthMax) ? 3
                    : (this.state.windowWidth >= Constants.screenWidthMin && this.state.windowWidth < Constants.screenWidthDefault) ? 2
                        : 1;
            cards.push(<Grid columns={columns}
                accessibility={gridBehavior}
                className="tile-render"
                content={tiles}>
            </Grid>)
            let scrollViewStyle = { height: "92vh" };
            return (
                <div className="site-div">
                    <div className="container-subdiv-cardview">
                        <SearchBox
                            searchText={this.state.searchText}
                            onSearchInputChange={this.handleSearchInputChange}
                        />
                        <div className="scroll-view" style={scrollViewStyle}>
                            {
                                tiles.length > 0 ? cards : <div className="no-post-added-container">                                    
                                    <div className="results">{this.localize("EmptySearchResults")}
                                    </div>
                                </div>
                            }
                        </div>
                    </div>
                </div>
            );

        }
    }
}   

const mapStateToProps = (state: any) => {
    return { messagesList: state.messagesList };
}

const dashboardWithTranslation = withTranslation()(Dashboard);
export default connect(mapStateToProps, { getUserDashboardMessagesList })(dashboardWithTranslation);