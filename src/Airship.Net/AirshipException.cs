/* Copyright Airship and Contributors */

using System;

namespace AirshipDotNet
{
    /// <summary>
    /// Exception thrown by Airship operations.
    /// </summary>
    public class AirshipException : Exception
    {
        public AirshipException(string message) : base(message)
        {
        }

        public AirshipException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}