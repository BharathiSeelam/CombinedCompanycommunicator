import { formatDate } from '../i18n';
import { getSentNotifications, getDraftNotifications } from '../apis/messageListApi';
import { getChannels } from '../apis/channelListApi';

type Notification = {
    createdDateTime: string,
    failed: number,
    id: string,
    isCompleted: boolean,
    sentDate: string,
    edited: string,
    sendingStartedDate: string,
    sendingDuration: string,
    succeeded: number,
    throttled: number,
    title: string,
    totalMessageCount: number,
}
type Channel = {
    id: string,
    channelName: string,
}

export const selectMessage = (message: any) => {
    return {
        type: 'MESSAGE_SELECTED',
        payload: message
    };
};

export const getMessagesList = () => async (dispatch: any) => {
    const response = await getSentNotifications();
    const notificationList: Notification[] = response.data;
    notificationList.forEach(notification => {
        notification.sendingStartedDate = formatDate(notification.sendingStartedDate);
        notification.sentDate = formatDate(notification.sentDate);
        notification.edited = formatDate(notification.edited);
    });
    dispatch({ type: 'FETCH_MESSAGES', payload: notificationList });
};

export const getDraftMessagesList = () => async (dispatch: any) => {
    const response = await getDraftNotifications();
    dispatch({ type: 'FETCH_DRAFTMESSAGES', payload: response.data });
};
export const getSentMessagesList = () => async (dispatch: any) => {
    const response = await getSentNotifications();
    dispatch({ type: 'FETCH_MESSAGES', payload: response.data });
};
export const selectChannel = (channel: any) => {
    return {
        type: 'CHANNEL_SELECTED',
        payload: channel
    };
};
export const getChannelsList = () => async (dispatch: any) => {
    const response = await getChannels();
    const channelList: Channel[] = response.data;
    dispatch({ type: 'FETCH_CHANNELS', payload: channelList });
};