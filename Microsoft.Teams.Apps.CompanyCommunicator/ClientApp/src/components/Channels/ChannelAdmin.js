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
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_redux_1 = require("react-redux");
var react_i18next_1 = require("react-i18next");
var Icons_1 = require("office-ui-fabric-react/lib/Icons");
var react_1 = require("@stardust-ui/react");
var microsoftTeams = require("@microsoft/teams-js");
require("./newMessage.scss");
require("./teamTheme.scss");
var channelOverflow_1 = require("../OverFlow/channelOverflow");
var configVariables_1 = require("../../configVariables");
var actions_1 = require("../../actions");
var ChannelAdmins = /** @class */ (function (_super) {
    __extends(ChannelAdmins, _super);
    function ChannelAdmins(props) {
        var _this = _super.call(this, props) || this;
        _this.processLabels = function () {
            var out = [{
                    key: "labels",
                    content: (React.createElement(react_1.Flex, { className: "listContainer", vAlign: "center", fill: true, gap: "gap.small" },
                        React.createElement(react_1.Flex.Item, { size: "size.small", variables: { 'size.small': '24%' }, grow: 1 },
                            React.createElement(react_1.Text, { truncated: true, weight: "bold", content: _this.localize("ChannelName") })),
                        React.createElement(react_1.Flex.Item, { size: "size.small", variables: { 'size.small': '76%' }, shrink: false },
                            React.createElement(react_1.Text, { truncated: true, weight: "bold", content: _this.localize("ChannelAdmins") })),
                        React.createElement(react_1.Flex.Item, { size: "size.small", variables: { 'size.small': '76%' }, shrink: false },
                            React.createElement(react_1.Text, { truncated: true, weight: "bold", content: _this.localize("ChannelAdminDLs") })))),
                    styles: { margin: '0.2rem 0.2rem 0 0' },
                }];
            return out;
        };
        Icons_1.initializeIcons();
        _this.localize = _this.props.t;
        _this.isOpenTaskModuleAllowed = true;
        _this.state = {
            channel: props.channels,
            itemsAccount: _this.props.channels.length,
            loader: true,
            teamsTeamId: "",
            teamsChannelId: "",
        };
        return _this;
    }
    ChannelAdmins.prototype.componentDidMount = function () {
        var _this = this;
        microsoftTeams.initialize();
        microsoftTeams.getContext(function (context) {
            _this.setState({
                teamsTeamId: context.teamId,
                teamsChannelId: context.channelId,
            });
        });
        this.props.getChannelsList();
        this.interval = setInterval(function () {
            _this.props.getChannelsList();
        }, 60000);
    };
    ChannelAdmins.prototype.componentWillReceiveProps = function (nextProps) {
        this.setState({
            channel: nextProps.channels,
            loader: false
        });
    };
    ChannelAdmins.prototype.componentWillUnmount = function () {
        clearInterval(this.interval);
    };
    ChannelAdmins.prototype.render = function () {
        var keyCount = 0;
        var processItem = function (channel) {
            keyCount++;
            var out = {
                key: keyCount,
                content: (React.createElement(react_1.Flex, { className: "listContainer", vAlign: "center", fill: true, gap: "gap.small" },
                    React.createElement(react_1.Flex.Item, { size: "size.small", shrink: 0, grow: 1 },
                        React.createElement(react_1.Text, null, channel.channelName)),
                    React.createElement(react_1.Flex.Item, { size: "size.small", variables: { 'size.small': '73%' }, shrink: 0 },
                        React.createElement(react_1.Text, null, channel.channelAdmins)),
                    React.createElement(react_1.Flex.Item, { size: "size.small", variables: { 'size.small': '73%' }, shrink: 0 },
                        React.createElement(react_1.Text, null, channel.channelAdminDLs)),
                    React.createElement(react_1.Flex.Item, { shrink: 0, hAlign: "end", vAlign: "center" },
                        React.createElement(channelOverflow_1.default, { channel: channel, title: "" })))),
                styles: { margin: '0.2rem 0.2rem 0 0' },
                onClick: function () {
                    var url = configVariables_1.getBaseUrl() + "/newchannel/" + channel.id + "?locale={locale}";
                    //this.onOpenTaskModule(null, url, this.localize("EditChannel"));
                },
            };
            return out;
        };
        var label = this.processLabels();
        var outList = this.state.channel.map(processItem);
        var allChannels = __spreadArrays(label, outList);
        if (this.state.loader) {
            return (React.createElement(react_1.Loader, null));
        }
        else if (this.state.channel.length === 0) {
            return (React.createElement("div", { className: "results" }, this.localize("EmptyChannels")));
        }
        else {
            return (React.createElement(react_1.List, { selectable: true, items: allChannels, className: "list" }));
        }
    };
    return ChannelAdmins;
}(React.Component));
var mapStateToProps = function (state) {
    return { channels: state.channelList, selectedChannel: state.selectedChannel };
};
var channelAdminsWithTranslation = react_i18next_1.withTranslation()(ChannelAdmins);
exports.default = react_redux_1.connect(mapStateToProps, { getChannelsList: actions_1.getChannelsList })(channelAdminsWithTranslation);
//# sourceMappingURL=ChannelAdmin.js.map