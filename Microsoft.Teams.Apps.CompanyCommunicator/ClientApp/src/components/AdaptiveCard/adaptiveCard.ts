import { TFunction } from "i18next";
export const getInitAdaptiveCard = (t: TFunction) => {
    const titleTextAsString = t("TitleText");
    return (
        {
            "type": "AdaptiveCard",
            "body": [
                {
                    "type": "TextBlock",
                    "weight": "Bolder",
                    "text": "",
                    "size": "ExtraLarge",
                    "wrap": true,
                    "id": "Title"
                },
                {
                    "type": "Image",
                    "spacing": "Default",
                    "url": "",
                    "size": "Stretch",
                    "width": "400px",
                    "altText": "",
                    "id": "ImageUri"
                },
                {
                    "type": "TextBlock",
                    "text": "",
                    "wrap": true,
                    "id": "Description"
                },
                {
                    "type": "TextBlock",
                    "wrap": true,
                    "size": "Small",
                    "weight": "Lighter",
                    "text": "",
                    "id": "Author"
                }
            ],
            "$schema": "https://adaptivecards.io/schemas/adaptive-card.json",
            "version": "1.0"
        }
    );
}



export const getCardTitle = (card: any) => {
    var index = card.body.map(t => { return t.id }).indexOf('Title');
    return card.body[index].text;
}


export const setCardTitle = (card: any, title: string) => {
    var titleTextAsString = "title";
    if (title != "") {
        titleTextAsString = title;
    }
    var index = card.body.map(t => { return t.id }).indexOf('Title');
    if (index != -1) {
        card.body[index].text = titleTextAsString;
    }
}


export const getCardImageLink = (card: any) => {
    var index = card.body.map(t => { return t.id }).indexOf('ImageUri');
    return card.body[index].url;
}


export const setCardImageLink = (card: any, imageLink?: string) => {
    var index = card.body.map(t => { return t.id }).indexOf('ImageUri');
    if (index != -1) {
        card.body[index].url = imageLink;
    }
}


export const getCardSummary = (card: any) => {
    var index = card.body.map(t => { return t.id }).indexOf('Description');
    return card.body[index].text;
}


export const setCardSummary = (card: any, summary?: string) => {
    var index = card.body.map(t => { return t.id }).indexOf('Description');
    if (index != -1) {
        card.body[index].text = summary;
    }
}


export const getCardAuthor = (card: any) => {
    var index = card.body.map(t => { return t.id }).indexOf('Author');
    return card.body[index].text;
}


export const setCardAuthor = (card: any, author?: string) => {
    var index = card.body.map(t => { return t.id }).indexOf('Author');
    if (index != -1) {
        card.body[index].text = author;
    }
}


export const getCardBtnTitle = (card: any) => {
    var index = card.actions.map(t => { return t.id }).indexOf('ActionTitle');
    return card.actions[index].title;
}





export const getCardBtnLink = (card: any) => {
    var index = card.actions.map(t => { return t.id }).indexOf('ActionUri');
    return card.actions[index].url;
}





export const setCardBtn = (card: any, buttonTitle?: string, buttonLink?: string) => {
    if (buttonTitle && buttonLink) {
        card.actions = [
            {
                "type": "Action.OpenUrl",
                "title": "buttonTitle",
                "url": "buttonLink"
            }
        ];
    } else {
        delete card.actions;
    }
}