using Dissonance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabratVoiceActivity
{
    [HarmonyLib.HarmonyPatch(typeof(VoiceBroadcastTrigger), "Mode", HarmonyLib.MethodType.Getter)]
    public class VoiceBroadcastTriggerPatch
    {
        static ILogger<VoiceBroadcastTrigger> _logger = UnityClassLogger<VoiceBroadcastTrigger>.Create();
        static bool _init;

        [HarmonyLib.HarmonyPrefix]
        public static bool Prefix(ref CommActivationMode __result)
        {
            if (!_init)
            {
                _logger.Warning("VoiceBroadcastTrigger patch (is enabled: " + ModConfiguration.Instance.IsVoiceActivationEnabled + ")");
                _init = true;
            }

            if (ModConfiguration.Instance.IsVoiceActivationEnabled)
                __result = CommActivationMode.VoiceActivation;

            return !ModConfiguration.Instance.IsVoiceActivationEnabled;
        }
    }
}
