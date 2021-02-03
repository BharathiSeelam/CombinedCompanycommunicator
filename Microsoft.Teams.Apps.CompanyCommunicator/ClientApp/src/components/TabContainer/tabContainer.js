"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_i18next_1 = require("react-i18next");
var messages_1 = require("../Messages/messages");
var channels_1 = require("../Channels/channels");
var ChannelAdmin_1 = require("../Channels/ChannelAdmin");
var draftMessages_1 = require("../DraftMessages/draftMessages");
require("./tabContainer.scss");
require("./rc-tabs.scss");
var microsoftTeams = require("@microsoft/teams-js");
var configVariables_1 = require("../../configVariables");
var react_1 = require("@stardust-ui/react");
var rc_tabs_1 = require("rc-tabs");
var actions_1 = require("../../actions");
var react_redux_1 = require("react-redux");
var TabContainer = /** @class */ (function (_super) {
    __extends(TabContainer, _super);
    function TabContainer(props) {
        var _this = _super.call(this, props) || this;
        _this.callback = function (key) { };
        _this.onNewMessage = function () {
            var taskInfo = {
                url: _this.state.messageURL,
                title: _this.localize("NewMessage"),
                height: 530,
                width: 1000,
                fallbackUrl: _this.state.messageURL,
            };
            var submitHandler = function (err, result) {
                _this.props.getDraftMessagesList();
            };
            microsoftTeams.tasks.startTask(taskInfo, submitHandler);
        };
        _this.onNewChannel = function () {
            var channelTaskInfo = {
                url: _this.state.channelURL,
                title: _this.localize("NewChannel"),
                height: 530,
                width: 1000,
                fallbackUrl: _this.state.channelURL,
            };
            var submitHandler = function (err, result) {
                _this.props.getChannelsList();
            };
            microsoftTeams.tasks.startTask(channelTaskInfo, submitHandler);
        };
        _this.localize = _this.props.t;
        _this.state = {
            messageURL: configVariables_1.getBaseUrl() + "/newmessage?locale={locale}",
            channelURL: configVariables_1.getBaseUrl() + "/newchannel?locale={locale}"
        };
        _this.escFunction = _this.escFunction.bind(_this);
        return _this;
    }
    TabContainer.prototype.componentDidMount = function () {
        microsoftTeams.initialize();
        //- Handle the Esc key
        document.addEventListener("keydown", this.escFunction, false);
    };
    TabContainer.prototype.componentWillUnmount = function () {
        document.removeEventListener("keydown", this.escFunction, false);
    };
    TabContainer.prototype.escFunction = function (event) {
        if (event.keyCode === 27 || (event.key === "Escape")) {
            microsoftTeams.tasks.submitTask();
        }
    };
    TabContainer.prototype.render = function () {
        var panels = [
            {
                title: this.localize('DraftMessagesSectionTitle'),
                content: {
                    key: 'sent',
                    content: (React.createElement("div", { className: "messages" },
                        React.createElement(draftMessages_1.default, null))),
                },
            },
            {
                title: this.localize('SentMessagesSectionTitle'),
                content: {
                    key: 'draft',
                    content: (React.createElement("div", { className: "messages" },
                        React.createElement(messages_1.default, null))),
                },
            }
        ];
        return (React.createElement("div", { className: "tabContainer" },
            React.createElement(rc_tabs_1.default, { defaultActiveKey: "1", onChange: this.callback },
                React.createElement(rc_tabs_1.TabPane, { tab: "Messages", key: "1" },
                    React.createElement("div", { className: "newPostBtn" },
                        React.createElement(react_1.Button, { content: this.localize("NewMessage"), onClick: this.onNewMessage, primary: true })),
                    React.createElement("div", { className: "messageContainer" },
                        React.createElement(react_1.Accordion, { defaultActiveIndex: [0, 1], panels: panels }))),
                React.createElement(rc_tabs_1.TabPane, { tab: "Channels", key: "2" },
                    React.createElement("div", { className: "newPostBtn" },
                        React.createElement(react_1.Button, { className: "new", content: this.localize("NewChannel"), onClick: this.onNewChannel, primary: true })),
                    React.createElement("div", { className: "channelContainer" },
                        React.createElement(channels_1.default, null))),
                React.createElement(rc_tabs_1.TabPane, { tab: "Channel Admin", key: "3" },
                    React.createElement("div", { className: "channelAdminContainer" },
                        React.createElement(ChannelAdmin_1.default, null))))));
    };
    return TabContainer;
}(React.Component));
var mapStateToProps = function (state) {
    return { messages: state.draftMessagesList, channels: state.ChannelList };
};
var tabContainerWithTranslation = react_i18next_1.withTranslation()(TabContainer);
exports.default = react_redux_1.connect(mapStateToProps, { getDraftMessagesList: actions_1.getDraftMessagesList, getChannelsList: actions_1.getChannelsList })(tabContainerWithTranslation);
//# sourceMappingURL=tabContainer.js.map