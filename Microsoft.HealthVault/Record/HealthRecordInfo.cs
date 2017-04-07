// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Record
{
    /// <summary>
    /// Represents the APIs and information about a health record for an individual.
    /// </summary>
    ///
    /// <remarks>
    /// A HealthRecordInfo represents a person's view of a health record and
    /// information about the health record such as the state, name, date of
    /// expiration, and so on. This view may vary based upon the access rights the
    /// person has to the record and multiple people may have access to the
    /// same record but have different views. For instance, a husband may
    /// have a HealthRecordInfo instance for himself and another for his
    /// wife's health record which she shared with him.
    /// </remarks>
    ///
    public class HealthRecordInfo
    {
        #region Constructors

        /// <summary>
        /// Copy constructor
        /// </summary>
        ///
        /// <param name="recordInfo">
        /// The record info object which is to be used as the source
        /// for the data.
        /// </param>
        ///
        internal HealthRecordInfo(HealthRecordInfo recordInfo)
        {
            this.Id = recordInfo.Id;
            this.custodian = recordInfo.IsCustodian;
            this.dateAuthorizationExpires = recordInfo.DateAuthorizationExpires;
            this.name = recordInfo.Name;
            this.relationshipType = recordInfo.RelationshipType;
            this.relationshipName = recordInfo.RelationshipName;
            this.displayName = recordInfo.DisplayName;

            if (recordInfo.Location != null)
            {
                this.Location = new Location(recordInfo.Location.Country, recordInfo.Location.StateProvince);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordInfo"/> class
        /// for deserialization purposes.
        /// </summary>
        /// <remarks>
        /// This constructor is only useful if ParseXml is called.
        /// </remarks>
        public HealthRecordInfo()
        {
        }

        #endregion

        /// <summary>
        /// Creates an instance of a HealthRecordInfo object using
        /// the specified XML.
        /// </summary>
        ///
        /// <param name="connection">
        /// A connection for the current user.
        /// </param>
        ///
        /// <param name="navigator">
        /// The XML containing the record information.
        /// </param>
        ///
        /// <returns>
        /// A new instance of a HealthRecordInfo object populated with the
        /// record information.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> or <paramref name="navigator"/>
        /// parameter is <b>null</b>.
        /// </exception>
        ///
        public static HealthRecordInfo CreateFromXml(
            IHealthVaultConnection connection,
            XPathNavigator navigator)
        {
            Validator.ThrowIfArgumentNull(connection, nameof(connection), Resources.PersonInfoConnectionNull);
            Validator.ThrowIfArgumentNull(navigator, nameof(navigator), Resources.ParseXmlNavNull);

            HealthRecordInfo recordInfo = new HealthRecordInfo();
            recordInfo.ParseXml(navigator);
            return recordInfo;
        }

        /// <summary>
        /// Gets the record identifier.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier (GUID) for the record.
        /// </value>
        ///
        /// <remarks>
        /// The record identifier is issued when the record is created. Creating
        /// the account automatically creates a self record as well.
        /// </remarks>
        ///
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the location of the person that this record is for.
        /// </summary>
        ///
        public Location Location { get; protected set; }

        /// <summary>
        /// Creates an instance of a HealthRecordInfo object using  the specified XML.
        /// </summary>
        /// <param name="navigator">The navigator.</param>
        /// <returns>HealthRecordInfo</returns>
        public static HealthRecordInfo CreateFromXml(XPathNavigator navigator)
        {
            Validator.ThrowIfArgumentNull(navigator, nameof(navigator), Resources.ParseXmlNavNull);

            HealthRecordInfo recordInfo = new HealthRecordInfo();
            recordInfo.ParseXml(navigator);
            return recordInfo;
        }

        #region Xml

        /// <summary>
        /// Parses HealthRecordInfo member data from the specified XPathNavigator.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the record information.
        /// </param>
        ///
        internal void ParseXml(XPathNavigator navigator)
        {
            string id = navigator.GetAttribute("id", string.Empty);
            this.Id = new Guid(id);

            string country = navigator.GetAttribute("location-country", string.Empty);
            string state = navigator.GetAttribute("location-state-province", string.Empty);
            if (!string.IsNullOrEmpty(country))
            {
                this.Location = new Location(country, string.IsNullOrEmpty(state) ? null : state);
            }

            this.custodian = XPathHelper.ParseAttributeAsBoolean(navigator, "record-custodian", false);

            long? relationshipNumber;

            relationshipNumber = XPathHelper.ParseAttributeAsLong(navigator, "rel-type", 0);
            if (relationshipNumber.HasValue &&
               (relationshipNumber <= (int)RelationshipType.Daughter))
            {
                this.relationshipType = (RelationshipType)relationshipNumber;
            }

            this.relationshipName = navigator.GetAttribute("rel-name", string.Empty);

            this.dateAuthorizationExpires = XPathHelper.ParseAttributeAsDateTime(navigator, "auth-expires", DateTime.MinValue);

            this.authExpired = XPathHelper.ParseAttributeAsBoolean(navigator, "auth-expired", false);

            this.name = navigator.Value;

            this.displayName = navigator.GetAttribute("display-name", string.Empty);

            this.State = XPathHelper.ParseAttributeAsEnum(navigator, "state", HealthRecordState.Unknown);
            if (this.State > HealthRecordState.Deleted)
            {
                this.State = HealthRecordState.Unknown;
            }

            this.DateCreated = XPathHelper.ParseAttributeAsDateTime(navigator, "date-created", DateTime.MinValue);
            this.DateUpdated = XPathHelper.ParseAttributeAsDateTime(navigator, "date-updated", DateTime.MinValue);

            this.QuotaInBytes = XPathHelper.ParseAttributeAsLong(navigator, "max-size-bytes", null);
            this.QuotaUsedInBytes = XPathHelper.ParseAttributeAsLong(navigator, "size-bytes", null);

            this.HealthRecordAuthorizationStatus = XPathHelper.ParseAttributeAsEnum(
                    navigator,
                    "app-record-auth-action",
                    HealthRecordAuthorizationStatus.Unknown);

            this.ApplicationSpecificRecordId = navigator.GetAttribute("app-specific-record-id", string.Empty);

            this.LatestOperationSequenceNumber = XPathHelper.ParseAttributeAsLong(navigator, "latest-operation-sequence-number", 0).Value;

            this.RecordAppAuthCreatedDate = XPathHelper.ParseAttributeAsDateTime(navigator, "record-app-auth-created-date", DateTime.MinValue);
        }

        /// <summary>
        /// Gets the XML representation of the HealthRecordInfo.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the XML representation of the HealthRecordInfo.
        /// </returns>
        ///
        public string GetXml()
        {
            StringBuilder recordInfoXml = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(recordInfoXml, settings))
            {
                this.WriteXml("record", writer);
                writer.Flush();
            }

            return recordInfoXml.ToString();
        }

        internal void WriteXml(string nodeName, XmlWriter writer)
        {
            writer.WriteStartElement(nodeName);

            writer.WriteAttributeString("id", this.Id.ToString());

            if (this.Location != null)
            {
                writer.WriteAttributeString("location-country", this.Location.Country);
                if (!string.IsNullOrEmpty(this.Location.StateProvince))
                {
                    writer.WriteAttributeString("location-state-province", this.Location.StateProvince);
                }
            }

            writer.WriteAttributeString(
                "record-custodian",
                XmlConvert.ToString(this.custodian));

            writer.WriteAttributeString(
                "rel-type",
                XmlConvert.ToString((int)this.relationshipType));

            if (!string.IsNullOrEmpty(this.relationshipName))
            {
                writer.WriteAttributeString(
                    "rel-name",
                    this.relationshipName);
            }

            writer.WriteAttributeString(
                "auth-expires",
                SDKHelper.XmlFromDateTime(this.dateAuthorizationExpires));

            writer.WriteAttributeString(
                "auth-expired",
                SDKHelper.XmlFromBool(this.authExpired));

            if (!string.IsNullOrEmpty(this.displayName))
            {
                writer.WriteAttributeString(
                    "display-name",
                    this.displayName);
            }

            writer.WriteAttributeString(
                "state",
                this.State.ToString());

            writer.WriteAttributeString(
                "date-created",
                SDKHelper.XmlFromDateTime(this.DateCreated));

            if (this.QuotaInBytes.HasValue)
            {
                writer.WriteAttributeString(
                    "max-size-bytes",
                    XmlConvert.ToString(this.QuotaInBytes.Value));
            }

            if (this.QuotaUsedInBytes.HasValue)
            {
                writer.WriteAttributeString(
                    "size-bytes",
                    XmlConvert.ToString(this.QuotaUsedInBytes.Value));
            }

            writer.WriteAttributeString(
                "app-record-auth-action",
                this.HealthRecordAuthorizationStatus.ToString());

            writer.WriteAttributeString(
                "app-specific-record-id",
                this.ApplicationSpecificRecordId);

            writer.WriteAttributeString(
                "date-updated",
                SDKHelper.XmlFromDateTime(this.DateUpdated));

            writer.WriteValue(this.name);

            writer.WriteEndElement();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether or not the person is a custodian
        /// of the record.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the person is a custodian of the record; otherwise,
        /// <b>false</b>.
        /// </value>
        ///
        /// <remarks>
        /// A person is considered a custodian if they have been given
        /// ownership of the record. The owner can give ownership to another
        /// as an explicit action when sharing the record.
        /// </remarks>
        public bool IsCustodian
        {
            get
            {
                return this.custodian;
            }

            set
            {
                this.custodian = value;
            }
        }

        private bool custodian;

        /// <summary>
        /// Gets the date/time that the authorization for the record expires.
        /// </summary>
        ///
        /// <value>
        /// A DateTime in UTC indicating when the record is no longer
        /// accessible to the user.
        /// </value>
        ///
        /// <remarks>
        /// When a person shares their record with another HealthVault account,
        /// they can specify the date when that sharing is revoked (if ever).
        /// This property indicates that date. If the person tries to access
        /// the record after the indicated date, they receive a
        /// <see cref="HealthServiceAccessDeniedException"/>.
        /// </remarks>
        ///
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID.
        /// </exception>
        ///
        public DateTime DateAuthorizationExpires
        {
            get
            {
                return this.dateAuthorizationExpires;
            }

            set
            {
                this.dateAuthorizationExpires = value;
            }
        }

        private DateTime dateAuthorizationExpires;

        /// <summary>
        /// <b>true</b> if the authorization of the authenticated person has
        /// expired for this record; otherwise, <b>false</b>.
        /// </summary>
        ///
        public bool HasAuthorizationExpired
        {
            get
            {
                return this.authExpired;
            }

            set
            {
                this.authExpired = value;
            }
        }

        private bool authExpired;

        /// <summary>
        /// Gets the name of the record.
        /// </summary>
        ///
        /// <value>
        /// A string indicating the name of the record.
        /// </value>
        ///
        /// <remarks>
        /// The name defaults to the name of the person to whom the record
        /// belongs. See <see cref="DisplayName"/> for how to override the
        /// name to customize the view for a person authorized to view the
        /// record.
        /// </remarks>
        ///
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID.
        /// </exception>
        ///
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        private string name;

        /// <summary>
        /// Gets the relationship the person authorized to view this record
        /// has with the "owner" of the record.
        /// </summary>
        ///
        /// <value>
        /// An enumeration value indicating the relationship between the
        /// record owner and the person authorized to use the record.
        /// </value>
        ///
        /// <remarks>
        /// See <see cref="RelationshipType"/> for more information on the
        /// relationships and what they mean.
        /// </remarks>
        ///
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID.
        /// </exception>
        ///
        public RelationshipType RelationshipType
        {
            get
            {
                return this.relationshipType;
            }

            set
            {
                this.relationshipType = value;
            }
        }

        private RelationshipType relationshipType = RelationshipType.Unknown;

        /// <summary>
        /// Gets the localized string representing the relationship between
        /// the person authorized to view this record and the owner of the
        /// record.
        /// </summary>
        ///
        /// <value>
        /// A string representation of the enumeration value indicating the
        /// relationship between the record owner and the person authorized
        /// to use the record.
        /// </value>
        ///
        /// <remarks>
        /// See <see cref="RelationshipType"/> for more information on the
        /// relationships and what they mean.
        /// </remarks>
        ///
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID.
        /// </exception>
        ///
        public string RelationshipName
        {
            get
            {
                return this.relationshipName;
            }

            set
            {
                this.relationshipName = value;
            }
        }

        private string relationshipName;

        /// <summary>
        /// Gets the display name of the record.
        /// </summary>
        ///
        /// <value>
        /// A string representing the name of the record as seen by the
        /// current user.
        /// </value>
        ///
        /// <remarks>
        /// A record has a name that defaults to the name of the owner of
        /// the record. A nickname can override the record name for each
        /// person authorized to use the record. If the nickname is specified,
        /// it is returned in this property. If the nickname is not specified,
        /// the record name is returned.
        /// </remarks>
        ///
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID.
        /// </exception>
        ///
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                this.displayName = value;
            }
        }

        private string displayName;

        /// <summary>
        /// Gets the state of a <see cref="HealthRecordInfo"/>.
        /// </summary>
        ///
        public HealthRecordState State { get; protected internal set; }

        /// <summary>
        /// Gets the date the record was created, in UTC.
        /// </summary>
        ///
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets the date the record was updated, in UTC.
        /// </summary>
        ///
        public DateTime DateUpdated { get; set; }

        /// <summary>
        /// Gets the maximum total size in bytes that the <see cref="ThingBase" />s in
        /// the <see cref="HealthRecordInfo" /> can occupy.
        /// </summary>
        ///
        /// <remarks>
        /// This data value is only available when the <see cref="HealthRecordInfo"/> object is
        /// fetched from the HealthVault platform as opposed to created on the fly.
        /// </remarks>
        ///
        public long? QuotaInBytes { get; set; }

        /// <summary>
        /// Gets the total size in bytes that the <see cref="ThingBase" />s in
        /// the <see cref="HealthRecordInfo" /> currently occupy.
        /// </summary>
        ///
        /// <remarks>
        /// This data value is only available when the <see cref="HealthRecordInfo"/> object is
        /// fetched from the HealthVault platform as opposed to created on the fly.
        /// </remarks>
        ///
        public long? QuotaUsedInBytes { get; set; }

        /// <summary>
        /// Gets the record's latest operation sequence number.
        /// </summary>
        ///
        /// <remarks>
        /// The record's operation sequence number is used when syncing data from a
        /// record. Anytime an operation is performed against a thing HealthVault
        /// stamps it with the next increment of the operation sequence number for the record.
        /// For example, the first item added to the record would be stamped with the sequence
        /// number 1, the next operation would stamp the thing with 2, etc. Applications can
        /// determine all operations that have occurred since a known point by calling
        /// GetRecordOperations and passing the sequence number of the known point.
        /// </remarks>
        ///
        public long LatestOperationSequenceNumber { get; set; }

        /// <summary>
        /// Gets the <see cref="HealthRecordAuthorizationStatus"/> for the record.
        /// </summary>
        /// <remarks>
        /// The status indicates whether, at the time of retrieval, the application
        /// is able to access the record.  Any status other than NoActionRequired
        /// requires user intervention in HealthVault before the application may
        /// successfully access the record.
        /// </remarks>
        public HealthRecordAuthorizationStatus HealthRecordAuthorizationStatus { get; set; }

        /// <summary>
        /// Gets the application specific record id for the specified
        /// record and application.
        /// </summary>
        ///
        public string ApplicationSpecificRecordId { get; set; }

        /// <summary>
        /// Gets the date when the user authorized the application to the record, in UTC.
        /// </summary>
        public DateTime RecordAppAuthCreatedDate { get; set; }

        #endregion Public properties

        /// <summary>
        /// Gets the name of the record.
        /// </summary>
        ///
        /// <value>
        /// The name of the record.
        /// </value>
        ///
        public override string ToString()
        {
            return this.Name;
        }
    }
}
