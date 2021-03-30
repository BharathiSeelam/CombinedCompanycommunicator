// <copyright file="searchBox.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from "react";
import { Flex, Input, Text, Button, AddIcon } from "@fluentui/react-northstar";
import * as microsoftTeams from "@microsoft/teams-js";
import { initializeIcons } from 'office-ui-fabric-react/lib/Icons';
import { Icon } from '@fluentui/react';
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import Constants from '../../constants/constants';
import "./searchBox.scss";

interface ISearchBoxProps extends WithTranslation {
    searchText: string;
    onSearchInputChange: (searchString: string) => void;
}

interface ISearchBoxState {
    screenWidth: number;
}

class SearchBox extends React.Component<ISearchBoxProps, ISearchBoxState> {
    localize: TFunction;
    constructor(props: ISearchBoxProps) {
        super(props);
        initializeIcons();
        this.localize = this.props.t;
        this.state = {
            screenWidth: Constants.screenWidth,
        }

    }

     submitHandler = async () => {
     };

    componentDidMount() {
        window.addEventListener("resize", this.resize.bind(this));
        this.resize();
    }

    /**
   * Invokes when screen is resized
   */
    resize = () => {
        if (window.innerWidth !== this.state.screenWidth) {
            this.setState({ screenWidth: window.innerWidth });
        }
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.resize.bind(this));
    }

    /**
    * Renders the component
    */
    public render(): JSX.Element {
        return (
            <div>
                <Flex>
                    {this.state.screenWidth > 750 &&
                        <Flex gap="gap.small" vAlign="center" className="filter-bar-wrapper">
                            <Flex.Item grow>
                                <div className="searchbar-wrapper">
                                <Input className="searchbar-input" value={this.props.searchText} inverted fluid icon={<Icon iconName="Search" className="search-icon" />} iconPosition="end" placeholder={this.localize("SearchByTitle")} onChange={(event: any) => this.props.onSearchInputChange(event.target.value)} />
                                </div>                            
                            </Flex.Item>

                        </Flex>}

                    {this.state.screenWidth <= 750 && <Flex gap="gap.small" vAlign="start" className="filter-bar-wrapper">
                        <Flex.Item grow>
                            <Flex column gap="gap.small" vAlign="stretch">
                                <div className="searchbar-wrapper-mobile">
                                    <Input className="searchbar-input" value={this.props.searchText} placeholder={this.localize("SearchByTitle")} onChange={(event: any) => this.props.onSearchInputChange(event.target.value)} />
                                    <Icon iconName="Search" className="search-icon" />
                                </div>                                
                            </Flex>
                        </Flex.Item>
                    </Flex>}
                </Flex>
            </div>
        );
    }
}

export default withTranslation()(SearchBox)