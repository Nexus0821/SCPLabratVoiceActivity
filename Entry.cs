using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabratVoiceActivity
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Entry : BaseUnityPlugin
    {
        public const string GUID = "com.nexus.scplabrat.voiceactivity";
        public const string NAME = "LabratVoiceActivity";
        public const string VERSION = "1.0.5";

        static ILogger<Entry> _logger = UnityClassLogger<Entry>.Create();

        private void Awake()
        {
            _logger.Info($"Initializing mod: {GUID}");

            if (ModConfiguration.LoadConfiguration() != ModConfigLoadErrorType.None)
                _logger.Error("Encountered unexpected error while loading ModConfiguration");

            HarmonyPatch.OnLoaded += HarmonyPatch_OnLoaded;
            HarmonyPatch.Enable();
        }

        /*private void OnDestroy()
        {
            HarmonyPatch.OnDisabled += HarmonyPatch_OnDisabled;
            HarmonyPatch.Disable();
        }*/

        private void HarmonyPatch_OnLoaded()
        {
            if (HarmonyPatch.InstanceState == HarmonyState.RUNNING)
                _logger.Info("Enabled harmony patch: " + GUID);

            HarmonyPatch.OnLoaded -= HarmonyPatch_OnLoaded;
        }

        private void HarmonyPatch_OnDisabled()
        {
            if (HarmonyPatch.InstanceState == HarmonyState.INACTIVE)
                _logger.Info("Disabled harmony patch: " + GUID);

            HarmonyPatch.OnDisabled -= HarmonyPatch_OnDisabled;
        }
    }
}
