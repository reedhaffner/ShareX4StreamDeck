using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WindowsInput;

namespace ShareX
{
    [PluginActionId("com.reedhaffner.screenshot")]
    public class Screenshot : PluginBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings();
                instance.Type = "ActiveWindow";
                instance.MissingX = Globals.missingx;

                return instance;
            }

            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }
            [JsonProperty(PropertyName = "missingx")]
            public bool MissingX { get; set; }
        }

        #region Private members

        private const int RESET_COUNTER_KEYPRESS_LENGTH = 1;

        private bool inputRunning = false;
        private PluginSettings settings;

        #endregion

        #region Public Methods

        public Screenshot(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                this.settings = PluginSettings.CreateDefaultSettings();
                JObject settingsobject = JObject.FromObject(settings);
                Connection.SetSettingsAsync(settingsobject);
            }
            else
            {
                JObject settingsobject = payload.Settings;
                if (Globals.missingx)
                this.settings = settingsobject.ToObject<PluginSettings>();
            }
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Pressed");
            if (inputRunning)
            {
                return;
            }
            
            XScreenshot();
        }

        public override void KeyReleased(KeyPayload payload)
        {
        }

        public override void OnTick()
        {
            if (Globals.missingx)
            {
                settings.MissingX = true;
            }
            else
            {
                settings.MissingX = false;
            }
        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Destructor called");
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            // New in StreamDeck-Tools v2.0:
            Tools.AutoPopulateSettings(settings, payload.Settings);
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Settings loaded: {payload.Settings}");
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        { }


        #endregion

        #region Private Methods

        private async void XScreenshot()
        {
            await Task.Run(() =>
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = Globals.xpath;
                startInfo.Arguments = "-" + settings.Type;
                process.StartInfo = startInfo;
                process.Start();
                Connection.ShowOk();
            });
        }
        #endregion
    }
}
