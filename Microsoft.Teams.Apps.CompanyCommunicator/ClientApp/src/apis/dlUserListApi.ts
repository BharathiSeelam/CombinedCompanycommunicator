import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

export const getDLUser = async (id: string): Promise<any> => {
    let url = baseAxiosUrl + "/dlUsers/" + id;
    return await axios.get(url);
}
export const getDLUsers = async (): Promise<any> => {
    let url = baseAxiosUrl + "/dlUsers";
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
