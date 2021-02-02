import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

export const getChannelTemplate = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/channelTemplate/" + id;
    return await axios.get(url);
}
export const getChannelTemplates = async (): Promise<any> => {
    let url = baseAxiosUrl + "/channelTemplate";
    return await axios.get(url);
}
export const deleteChannelTemplate = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/channelTemplate/" + id;
    return await axios.delete(url);
}

export const updateChannelTemplate = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/channelTemplate";
    return await axios.put(url, payload);
}

export const createChannelTemplate = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/channelTemplate";
    return await axios.post(url, payload);
}

export const getTeams = async (): Promise<any> => {
    let url = baseAxiosUrl + "/teamdata";
    return await axios.get(url);
}

export const getAuthenticationConsentMetadata = async (windowLocationOriginDomain: string, login_hint: string): Promise<any> => {
    let url = `${baseAxiosUrl}/authenticationMetadata/consentUrl?windowLocationOriginDomain=${windowLocationOriginDomain}&loginhint=${login_hint}`;
    return await axios.get(url, undefined, false);
}
