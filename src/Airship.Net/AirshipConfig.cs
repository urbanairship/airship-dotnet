/* Copyright Airship and Contributors */

using System;

namespace AirshipDotNet
{
    /// <summary>
    /// Airship configuration.
    /// </summary>
    public class AirshipConfig
    {
        /// <summary>
        /// The default environment.
        /// </summary>
        public AirshipEnvironment DefaultEnvironment { get; set; } = AirshipEnvironment.Production;

        /// <summary>
        /// The production configuration.
        /// </summary>
        public EnvironmentConfig? ProductionConfig { get; set; }

        /// <summary>
        /// The development configuration.
        /// </summary>
        public EnvironmentConfig? DevelopmentConfig { get; set; }

        /// <summary>
        /// The site.
        /// </summary>
        public AirshipSite Site { get; set; } = AirshipSite.US;

        /// <summary>
        /// Data collection opt-in enabled.
        /// </summary>
        public bool DataCollectionOptInEnabled { get; set; }

        /// <summary>
        /// In production flag.
        /// </summary>
        public bool InProduction { get; set; } = true;

        /// <summary>
        /// Environment configuration.
        /// </summary>
        public class EnvironmentConfig
        {
            /// <summary>
            /// The app key.
            /// </summary>
            public string? AppKey { get; set; }

            /// <summary>
            /// The app secret.
            /// </summary>
            public string? AppSecret { get; set; }

            /// <summary>
            /// The log level.
            /// </summary>
            public AirshipLogLevel LogLevel { get; set; } = AirshipLogLevel.Error;
        }
    }

    /// <summary>
    /// Airship environment.
    /// </summary>
    public enum AirshipEnvironment
    {
        /// <summary>
        /// Development environment.
        /// </summary>
        Development,

        /// <summary>
        /// Production environment.
        /// </summary>
        Production
    }

    /// <summary>
    /// Airship site.
    /// </summary>
    public enum AirshipSite
    {
        /// <summary>
        /// US site.
        /// </summary>
        US,

        /// <summary>
        /// EU site.
        /// </summary>
        EU
    }

    /// <summary>
    /// Airship log level.
    /// </summary>
    public enum AirshipLogLevel
    {
        /// <summary>
        /// No logging.
        /// </summary>
        None,

        /// <summary>
        /// Error level logging.
        /// </summary>
        Error,

        /// <summary>
        /// Warning level logging.
        /// </summary>
        Warning,

        /// <summary>
        /// Info level logging.
        /// </summary>
        Info,

        /// <summary>
        /// Debug level logging.
        /// </summary>
        Debug,

        /// <summary>
        /// Verbose level logging.
        /// </summary>
        Verbose
    }
}