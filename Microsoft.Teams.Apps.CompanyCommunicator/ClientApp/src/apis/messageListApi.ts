import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

export const getSentNotifications = async (): Promise<any> => {
    let url = baseAxiosUrl + "/sentnotifications";
    return await axios.get(url);
}

export const getDraftSentNotification = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/sentnotifications/sent/" + id;
    return await axios.get(url);
}
export const getDraftNotifications = async (): Promise<any> => {
    let url = baseAxiosUrl + "/draftnotifications";
    return await axios.get(url);
}

export const verifyGroupAccess = async (): Promise<any> => {
    let url = baseAxiosUrl + "/groupdata/verifyaccess";
    return await axios.get(url, false);
}

export const getGroups = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/groupdata/" + id;
    return await axios.get(url);
}

export const getsentGroups = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/groupdata/sent/" + id;
    return await axios.get(url);
}

export const getChannels = async (): Promise<any> => {
    let url = baseAxiosUrl + "/channeldata";
    return await axios.get(url);
}

export const getChannnelID = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/channeldata/" + id;
    return await axios.get(url);
}

export const searchGroups = async (query: string): Promise<any> => {
    let url = baseAxiosUrl + "/groupdata/search/" + query;
    return await axios.get(url);
}

export const exportNotification = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/exportnotification/export";
    return await axios.put(url, payload);
}

export const getSentNotification = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/sentnotifications/" + id;
    return await axios.get(url);
}

export const getDraftNotification = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/draftnotifications/" + id;
    return await axios.get(url);
}


export const deleteDraftNotification = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/draftnotifications/" + id;
    return await axios.delete(url);
}
export const deleteSendNotification = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/sentnotifications/" + id;
    return await axios.delete(url);
}
export const duplicateDraftNotification = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/draftnotifications/duplicates/" + id;
    return await axios.post(url);
}
export const updateDraftNotificationPublish = async (id: number, publish: String): Promise<any> => {
    let url = baseAxiosUrl + "/draftnotifications/publishOn/" + id + "/" + publish;
    return await axios.post(url);
}


export const sendDraftNotification = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/sentnotifications";
    return await axios.post(url, payload);
}

export const updateDraftNotification = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/draftnotifications";
    return await axios.put(url, payload);
}

export const updateSentNotification = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/sentnotifications";
    return await axios.put(url, payload);
}

export const createDraftNotification = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/draftnotifications";
    return await axios.post(url, payload);
}

export const getTeams = async (): Promise<any> => {
    let url = baseAxiosUrl + "/teamdata";
    return await axios.get(url);
}

export const getConsentSummaries = async (id: number): Promise<any> => {
    let url = baseAxiosUrl + "/draftnotifications/consentSummaries/" + id;
    return await axios.get(url);
}

export const sendPreview = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/draftnotifications/previews";
    return await axios.post(url, payload);
}

export const getAuthenticationConsentMetadata = async (windowLocationOriginDomain: string, login_hint: string): Promise<any> => {
    let url = `${baseAxiosUrl}/authenticationMetadata/consentUrl?windowLocationOriginDomain=${windowLocationOriginDomain}&loginhint=${login_hint}`;
    return await axios.get(url, undefined, false);
}
