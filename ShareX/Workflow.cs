using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace ShareX
{
    [PluginActionId("com.reedhaffner.workflow")]
    public class Workflow : KeypadBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings();
                instance.WorkflowName = string.Empty; ;

                return instance;
            }

            [JsonProperty(PropertyName = "workflow")]
            public string WorkflowName { get; set; }
        }

        #region Private members

        private const int RESET_COUNTER_KEYPRESS_LENGTH = 1;

        private bool inputRunning = false;
        private PluginSettings settings;

        #endregion

        #region Public Methods

        public Workflow(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                this.settings = PluginSettings.CreateDefaultSettings();
                Connection.SetSettingsAsync(JObject.FromObject(settings));
            }
            else
            {
                this.settings = payload.Settings.ToObject<PluginSettings>();
            }
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Pressed");
            if (Globals.xpath == null)
            {
                MessageBox.Show("Unable to find ShareX. Please try running ShareX first, then starting StreamDeck.", "Error in ShareX4StreamDeck", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (inputRunning)
            {
                return;
            }
            
            XWorkflow();
        }

        public override void KeyReleased(KeyPayload payload)
        {
        }

        public override void OnTick()
        {
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

        private async void XWorkflow()
        {
            await Task.Run(() =>
            {
                if (settings.WorkflowName == string.Empty)
                {
                    Connection.ShowAlert();
                    MessageBox.Show("You did not name a workflow!");
                }
                else
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo.FileName = Globals.xpath;
                    startInfo.Arguments = "-workflow \"" + settings.WorkflowName + "\"";
                    process.StartInfo = startInfo;
                    process.Start();
                    Connection.ShowOk();
                }
            });
        }
        #endregion
    }
}
