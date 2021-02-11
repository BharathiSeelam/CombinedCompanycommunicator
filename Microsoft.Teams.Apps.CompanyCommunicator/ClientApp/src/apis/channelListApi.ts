import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

export const getChannel = async (id: string): Promise<any> => {
    let url = baseAxiosUrl + "/channeldata/" + id;
    return await axios.get(url);
}
export const getAdminChannels = async (id : string, channelName : string): Promise<any> => {
    let url = baseAxiosUrl + "/channeldata/channelAdmin/" + id + "/" + channelName;
    return await axios.get(url);
}
export const getChannels = async (): Promise<any> => {
    let url = baseAxiosUrl + "/channeldata";
    return await axios.get(url);
}

export const deleteChannel = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/channeldata/" + id;
    return await axios.delete(url);
}

export const updateChannel = async (id: string, payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/channeldata/" + id
    return await axios.put(url, payload);
}

export const createChannel = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/channeldata";
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
