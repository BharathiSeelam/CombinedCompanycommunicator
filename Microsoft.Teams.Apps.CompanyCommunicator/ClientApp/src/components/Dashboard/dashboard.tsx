import * as React from 'react';
import { connect } from 'react-redux';
import * as microsoftTeams from "@microsoft/teams-js";
import { withTranslation, WithTranslation } from "react-i18next";
import { TFunction } from "i18next";
import './dashboard.scss';
import { getMessagesList } from '../../actions';
import { initializeIcons } from 'office-ui-fabric-react/lib/Icons';

export interface IMessage {
    id: string;
    title: string;
    sentDate: string;
    recipients: string;
    acknowledgements?: string;
    reactions?: string;
    responses?: string;
}
export interface IMessageProps extends WithTranslation {
    messagesList: IMessage[];
    getMessagesList?: any;
}
export interface IMessageState {
    message: IMessage[];
    loader: boolean;
}
class Dashboard extends React.Component<IMessageProps, IMessageState> {

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
        };
    }
    public async componentDidMount() {
        microsoftTeams.initialize();
        this.props.getMessagesList();
    }
     public componentWillReceiveProps(nextProps: any) {
        if (this.props !== nextProps) {
            this.setState({
                message: nextProps.messagesList,
                loader: false
            });
        }
    }

  
    public render(): JSX.Element {    
        return (
            <div>
            </div>
          );

    }

}
const mapStateToProps = (state: any) => {
    return { messagesList: state.messagesList };
}

const dashboardWithTranslation = withTranslation()(Dashboard);
export default connect(mapStateToProps, { getMessagesList })(dashboardWithTranslation);
