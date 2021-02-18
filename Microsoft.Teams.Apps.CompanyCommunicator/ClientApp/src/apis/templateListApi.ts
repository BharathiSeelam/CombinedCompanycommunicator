import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

export const getTemplate = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/template/" + id;
    return await axios.get(url);
}
export const getTemplates = async (): Promise<any> => {
    let url = baseAxiosUrl + "/template";
    return await axios.get(url);
}
export const deleteTemplate = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/template/" + id;
    return await axios.delete(url);
}

export const updateTemplate = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/template";
    return await axios.put(url, payload);
}

export const createTemplate = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/template";
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
