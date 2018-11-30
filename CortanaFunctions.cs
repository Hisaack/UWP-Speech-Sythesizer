using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace Cortana2
{
    public sealed class CortanaFunctions : IBackgroundTask
    {
        private VoiceCommandServiceConnection voiceServiceConnection;
        private BackgroundTaskDeferral serviceDeferral;


        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Create the deferral by requesting it from the task instance
            serviceDeferral = taskInstance.GetDeferral();




            AppServiceTriggerDetails triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails != null && triggerDetails.Name.Equals("VoiceCommandService"))
            {
                voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

                VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                // Perform the appropriate command depending on the operation defined in VCD
                switch (voiceCommand.CommandName)
                {
                    case "CheckTemperature":
                        VoiceCommandUserMessage userMessage = new VoiceCommandUserMessage();
                        userMessage.DisplayMessage = "The current temperature is 23 degrees";
                        userMessage.SpokenMessage = "The current temperature is 23 degrees";

                        VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                        break;

                    default:
                        VoiceCommandUserMessage userMessageX = new VoiceCommandUserMessage();
                        userMessageX.DisplayMessage = "The current temperature is 0 degrees";
                        userMessageX.SpokenMessage = "The current temperature is 0 degrees";

                        VoiceCommandResponse response1 = VoiceCommandResponse.CreateResponse(userMessageX, null);
                        await voiceServiceConnection.ReportSuccessAsync(response1);

                        break;
                }

                if (voiceCommand.CommandName == null)
                {
                    VoiceCommandUserMessage userMessageX = new VoiceCommandUserMessage();
                    userMessageX.DisplayMessage = "Please say it again";
                    userMessageX.SpokenMessage = "Please say it again";
                }

            }

            // Once the asynchronous method(s) are done, close the deferral
            serviceDeferral.Complete();
        }
        /*
      Register Custom Cortana Commands from VCD file
      */
        public static async void RegisterVCD()
        {
            StorageFile vcd = await Package.Current.InstalledLocation.GetFileAsync(
                @"CustomVoiceCommandDefinitions.xml");

            await VoiceCommandDefinitionManager
                .InstallCommandDefinitionsFromStorageFileAsync(vcd);
        }

        
    }
}