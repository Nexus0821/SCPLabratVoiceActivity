using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace LabratVoiceActivity
{
    public enum HarmonyState
    {
        INACTIVE,
        RUNNING
    }

    public static class HarmonyPatch
    {
        public static event Action OnLoaded;
        public static event Action OnDisabled;

        public static Harmony _harmonyInstance;
        public static HarmonyState _harmonyInstanceState;

        public static Harmony Instance => _harmonyInstance;
        public static HarmonyState InstanceState => _harmonyInstanceState;

        public static void Enable()
        {
            if (_harmonyInstanceState == HarmonyState.RUNNING ||
                _harmonyInstance != null)
                return;

            _harmonyInstance = new Harmony(Entry.GUID);
            _harmonyInstance.PatchAll();

            _harmonyInstanceState = HarmonyState.RUNNING;

            if (OnLoaded != null) OnLoaded();
        }

        public static void Disable()
        {
            if (_harmonyInstanceState == HarmonyState.INACTIVE ||
                _harmonyInstance == null)
                return;

            _harmonyInstance.UnpatchSelf();
            _harmonyInstance = null;

            _harmonyInstanceState = HarmonyState.INACTIVE;

            if (OnDisabled != null) OnDisabled();
        }

        public static MethodInfo Patch(MethodBase original,
                                             HarmonyMethod prefix = null,
                                             HarmonyMethod postfix = null,
                                             HarmonyMethod transpiler = null,
                                             HarmonyMethod finalizer = null,
                                             HarmonyMethod ilManipulator = null)
        {
            if (_harmonyInstanceState == HarmonyState.INACTIVE ||
                _harmonyInstance == null)
                return null;

            return _harmonyInstance.Patch(original, prefix, postfix, transpiler, finalizer, ilManipulator);
        }

        public static void Unpatch(MethodBase original, HarmonyPatchType type)
        {
            if (_harmonyInstanceState == HarmonyState.INACTIVE ||
                _harmonyInstance == null)
                return;

            _harmonyInstance.Unpatch(original, type, Entry.GUID);
        }
    }
}
