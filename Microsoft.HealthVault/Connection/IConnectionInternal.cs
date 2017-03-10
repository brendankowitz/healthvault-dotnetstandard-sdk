﻿using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Connection
{
    // TODO: Do we need an internal connection
    public interface IConnectionInternal : IHealthVaultConnection
    {
        CryptoData GetAuthData(HealthVaultMethods method, byte[] data);

        CryptoData GetInfoHash(byte[] data);

        void PrepareAuthSessionHeader(XmlWriter writer, Guid? recordId);

        // TODO: Temp. fix to quick run
        void StoreSessionCredentialInCookieXml(XmlWriter writer);

        void SetSessionCredentialFromCookieXml(XPathNavigator navigator);
    }
}