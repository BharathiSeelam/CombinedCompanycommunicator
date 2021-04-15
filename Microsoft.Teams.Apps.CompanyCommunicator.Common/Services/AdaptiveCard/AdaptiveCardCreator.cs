// <copyright file="AdaptiveCardCreator.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.AdaptiveCard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AdaptiveCards;
    using AdaptiveCards.Templating;
    using Html2Markdown;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Adaptive Card Creator service.
    /// </summary>
    public class AdaptiveCardCreator
    {
        /// <summary>
        /// Creates an adaptive card.
        /// </summary>
        /// <param name="notificationDataEntity">Notification data entity.</param>
        /// <returns>An adaptive card.</returns>
        public virtual AdaptiveCard CreateAdaptiveCard(NotificationDataEntity notificationDataEntity)
        {
            return this.CreateAdaptiveCard(
                notificationDataEntity.Title,
                notificationDataEntity.ImageLink,
                notificationDataEntity.Summary,
                notificationDataEntity.Author,
                notificationDataEntity.ButtonTitle,
                notificationDataEntity.ButtonLink);
        }

        /// <summary>
        /// Creates an adaptive card without header.
        /// </summary>
        /// <param name="notificationDataEntity">Notification data entity.</param>
        /// <param name="jsonformat">jsonformat.</param>
        /// <returns>An adaptive card.</returns>
        public virtual string CreateAdaptiveCardWithoutHeader(NotificationDataEntity notificationDataEntity, string jsonformat)
        {
            return this.CreateAdaptiveCardWithoutHeader(
                notificationDataEntity.Title,
                notificationDataEntity.ImageLink,
                notificationDataEntity.Summary,
                notificationDataEntity.Author,
                notificationDataEntity.ButtonTitle,
                notificationDataEntity.ButtonLink,
                jsonformat);
        }

        /// <summary>
        /// Create an adaptive card instance.
        /// </summary>
        /// <param name="title">The adaptive card's title value.</param>
        /// <param name="imageUrl">The adaptive card's image URL.</param>
        /// <param name="summary">The adaptive card's summary value.</param>
        /// <param name="author">The adaptive card's author value.</param>
        /// <param name="buttonTitle">The adaptive card's button title value.</param>
        /// <param name="buttonUrl">The adaptive card's button url value.</param>
        /// <param name="jsonfromat">The adaptive card's payload.</param>
        /// <returns>The created adaptive card instance.</returns>
        public string CreateAdaptiveCardWithoutHeader(
            string title,
            string imageUrl,
            string summary,
            string author,
            string buttonTitle,
            string buttonUrl,
            string jsonfromat)
        {
            var mSummary = string.Empty;
            var mauthor = string.Empty;
            var mimageUrl = string.Empty;
            var mbuttonUrl = string.Empty;

            var templateJson = jsonfromat;

            if (!string.IsNullOrWhiteSpace(summary))
            {
                var summaryHtmlString = summary;
                var converter = new Converter();
                var summaryMarkdownString = converter.Convert(summaryHtmlString);
                mSummary = summaryMarkdownString;
            }
           else
            {
                JToken jToken = JToken.Parse(templateJson);
                var result = jToken["body"].ToList();
                foreach (var item in result)
                {
                    if (item["text"] != null)
                    {
                        var text = item["text"];
                        if (text.ToString() == "${Description}")
                        {
                            item.Remove();
                        }
                    }
                }

                var output = jToken.ToString(Formatting.Indented);
                templateJson = output.ToString();
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                mauthor = author;
            }
            else
            {
                JToken jToken = JToken.Parse(templateJson);
                var result = jToken["body"].ToList();
                foreach (var item in result)
                {
                    if (item["text"] != null)
                    {
                        var text = item["text"];
                        if (text.ToString() == "${Author}")
                        {
                            item.Remove();
                        }
                    }
                }

                var output = jToken.ToString(Formatting.Indented);
                templateJson = output.ToString();
            }

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                mimageUrl = imageUrl;
            }
            else
            {
                JToken jToken = JToken.Parse(templateJson);
                var result = jToken["body"].ToList();
                foreach (var item in result)
                {
                    if (item["url"] != null)
                    {
                        var url = item["url"];
                        if (url.ToString() == "${ImageUri}")
                        {
                            item.Remove();
                        }
                    }
                }

                var output = jToken.ToString(Formatting.Indented);
                templateJson = output.ToString();
            }

            if (!string.IsNullOrWhiteSpace(buttonUrl) || !string.IsNullOrWhiteSpace(buttonTitle))
            {
                mbuttonUrl = buttonUrl;
            }
            else
            {
                JToken jToken = JToken.Parse(templateJson);
                var result = jToken["actions"].ToList();
                foreach (var item in result)
                {
                    if (item["url"] != null)
                    {
                        var url = item["url"];
                        if (url.ToString() == "${ActionUri}")
                        {
                            item.Remove();
                        }
                    }
                }

                var output = jToken.ToString(Formatting.Indented);
                templateJson = output.ToString();
            }

            AdaptiveCardTemplate template = new AdaptiveCardTemplate(templateJson);

            string cardJson;
            var myData = new
                {
                    Title = title,
                    ImageUri = mimageUrl,
                    Description = mSummary,
                    Author = mauthor,
                    ActionTitle = buttonTitle,
                    ActionUri = buttonUrl,
                };
            cardJson = template.Expand(myData);
            JObject.Parse(cardJson);

            var jobj = JToken.Parse(cardJson).ToString(Formatting.Indented);

            // string cardJson = template.Expand(myData);
            return cardJson;
        }

        /// <summary>
        /// Create an adaptive card instance.
        /// </summary>
        /// <param name="title">The adaptive card's title value.</param>
        /// <param name="imageUrl">The adaptive card's image URL.</param>
        /// <param name="summary">The adaptive card's summary value.</param>
        /// <param name="author">The adaptive card's author value.</param>
        /// <param name="buttonTitle">The adaptive card's button title value.</param>
        /// <param name="buttonUrl">The adaptive card's button url value.</param>
        /// <returns>The created adaptive card instance.</returns>
        public AdaptiveCard CreateAdaptiveCard(
            string title,
            string imageUrl,
            string summary,
            string author,
            string buttonTitle,
            string buttonUrl)
        {
            var version = new AdaptiveSchemaVersion(1, 0);
            AdaptiveCard card = new AdaptiveCard(version);

            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = title,
                Size = AdaptiveTextSize.Default,
                Weight = AdaptiveTextWeight.Bolder,
                Wrap = true,
            });
            card.Speak = title;

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                card.Body.Add(new AdaptiveImage()
                {
                    Url = new Uri(imageUrl, UriKind.RelativeOrAbsolute),
                    Spacing = AdaptiveSpacing.Default,
                    Size = AdaptiveImageSize.Stretch,
                    AltText = string.Empty,
                });
            }

            if (!string.IsNullOrWhiteSpace(summary))
            {
                card.Body.Add(new AdaptiveTextBlock()
                {
                    Text = summary,
                    Wrap = true,
                });
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                card.Body.Add(new AdaptiveTextBlock()
                {
                    Text = author,
                    Size = AdaptiveTextSize.Small,
                    Weight = AdaptiveTextWeight.Lighter,
                    Wrap = true,
                });
            }

            if (!string.IsNullOrWhiteSpace(buttonTitle)
                && !string.IsNullOrWhiteSpace(buttonUrl))
            {
                card.Actions.Add(new AdaptiveOpenUrlAction()
                {
                    Title = buttonTitle,
                    Url = new Uri(buttonUrl, UriKind.RelativeOrAbsolute),
                });
            }

            return card;
        }
    }
}
