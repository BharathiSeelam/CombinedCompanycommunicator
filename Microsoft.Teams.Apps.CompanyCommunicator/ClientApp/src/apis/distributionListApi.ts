import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

export const getDistributionLists = async (): Promise<any> => {
    let url = baseAxiosUrl + "/distributionLists";
    return await axios.get(url);
}
export const getDistributionListsByName = async (DLName :string): Promise<any> => {
    let url = baseAxiosUrl + "/distributionLists/"+ DLName;
    return await axios.get(url);
}
export const getDistributionListsByID = async (dLID: string): Promise<any> => {
    let url = baseAxiosUrl + "/distributionLists/draft/" + dLID;
    return await axios.get(url);
}

export const getTeams = async (): Promise<any> => {
    let url = baseAxiosUrl + "/teamdata";
    return await axios.get(url);
}

export const getAuthenticationConsentMetadata = async (windowLocationOriginDomain: string, login_hint: string): Promise<any> => {
    let url = `${baseAxiosUrl}/authenticationMetadata/consentUrl?windowLocationOriginDomain=${windowLocationOriginDomain}&loginhint=${login_hint}`;
    return await axios.get(url, undefined, false);
}
