// <copyright file="TeamDataMap.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Mappers
{
    using System;
    using CsvHelper.Configuration;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Resources;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Model;

    /// <summary>
    /// Mapper class for NotificationDetailsExport.
    /// </summary>
    public sealed class NotificationDetailsExportMap : ClassMap<NotificationDetailsExport>
    {
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDetailsExportMap"/> class.
        /// </summary>
        /// <param name="localizer">Localization service.</param>
        public NotificationDetailsExportMap(IStringLocalizer<Strings> localizer)
        {
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            this.Map(x => x.Id).Name(this.localizer.GetString("ColumnName_Id"));
            this.Map(x => x.Title).Name(this.localizer.GetString("ColumnName_MessageTitle"));
            this.Map(x => x.Summary).Name(this.localizer.GetString("ColumnName_Summary"));
            this.Map(x => x.Account).Name(this.localizer.GetString("ColumnName_Account"));
            this.Map(x => x.CreatedDateTime).Name(this.localizer.GetString("ColumnName_CreatedDateTime"));
            this.Map(x => x.SentDate).Name(this.localizer.GetString("ColumnName_SentDate"));
            this.Map(x => x.Edited).Name(this.localizer.GetString("ColumnName_Edited"));
            this.Map(x => x.SendingStartedDate).Name(this.localizer.GetString("ColumnName_SendingStartedDate"));
            this.Map(x => x.RecipientType).Name(this.localizer.GetString("ColumnName_RecipientType"));
            this.Map(x => x.UserId).Name(this.localizer.GetString("ColumnName_UserId"));
            this.Map(x => x.Upn).Name(this.localizer.GetString("ColumnName_Upn"));
            this.Map(x => x.UserName).Name(this.localizer.GetString("ColumnName_UserName"));
            this.Map(x => x.TeamId).Name(this.localizer.GetString("ColumnName_TeamId"));
            this.Map(x => x.TeamName).Name(this.localizer.GetString("ColumnName_TeamName"));
            this.Map(x => x.DeliveryStatus).Name(this.localizer.GetString("ColumnName_DeliveryStatus"));
            this.Map(x => x.StatusReason).Name(this.localizer.GetString("ColumnName_StatusReason"));
        }
    }
}
