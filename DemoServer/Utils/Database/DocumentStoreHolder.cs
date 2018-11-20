﻿using System;
using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents;

namespace DemoServer.Utils.Database
{
    public class DocumentStoreHolder
    {
        private readonly Settings.DatabaseSettings _databaseSettings;

        public DocumentStoreHolder(Settings settings)
        {
            _databaseSettings = settings.Database;
            ValidateSettings();
        }

        public IDocumentStore InitializeFor(string databaseName)
        {
            var store = new DocumentStore
            {
                Urls = _databaseSettings.Urls.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                Database = databaseName
            };

            if (_databaseSettings.CertificatePath != null)
            {
                var certificatePassword = _databaseSettings.CertificatePassword;
                var certificate = new X509Certificate2(_databaseSettings.CertificatePath, certificatePassword);
                store.Certificate = certificate;
            }

            if (_databaseSettings.RequestsTimeoutInSec.HasValue)
            {
                store.Conventions.RequestTimeout = TimeSpan.FromSeconds(_databaseSettings.RequestsTimeoutInSec.Value);
            }

            return store.Initialize();
        }

        private void ValidateSettings()
        {
            if (string.IsNullOrEmpty(_databaseSettings.Urls))
                throw new InvalidOperationException($"{nameof(_databaseSettings.Urls)} was not provided in the application settings");
        }
    }
}
