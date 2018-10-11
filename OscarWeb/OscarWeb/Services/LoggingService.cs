using System;
using System.Collections.Generic;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace OscarWeb.Services
{
    /// <summary>
    /// Class that is responsible for all logging. 
    /// Azure Application Insights is used for all logging functionality.
    /// </summary>
    public class LoggingService
    {
        private readonly TelemetryClient _telemetryClient = new TelemetryClient();

        /// <summary>
        /// Track an event 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="timespan"></param>
        /// <param name="properties"></param>
        public void TrackEvent(string eventName, TimeSpan timespan, IDictionary<string, string> properties = null)
        {
            var telemetry = new EventTelemetry(eventName);
            telemetry.Metrics.Add("Elapsed", timespan.TotalMilliseconds);

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    telemetry.Properties.Add(property.Key, property.Value);
                }
            }

            this._telemetryClient.TrackEvent(eventName, properties);
            this._telemetryClient.TrackEvent(telemetry);
        }

        /// <summary>
        /// Track diagnostics information 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="properties"></param>
        public void TrackTrace(string message, IDictionary<string, string> properties = null)
        {
            if (properties == null)
            {
                this._telemetryClient.TrackTrace(message);
            }
            else
            {
                this._telemetryClient.TrackTrace(message, properties);
            }
        }

        /// <summary>
        /// Track exceptions
        /// </summary>
        /// <param name="exception"></param>
        public void TrackException(Exception exception)
        {
            if (exception != null)
            {
                this._telemetryClient.TrackException(exception);
            }
        }
    }
}
 