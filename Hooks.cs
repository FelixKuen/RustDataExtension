using HarmonyLib;
using RustDataHarmony;

namespace RustDataHarmonyMod
{
    public class Hooks
    {
        
        [HarmonyPatch(typeof(ServerMgr), nameof(ServerMgr.OpenConnection))]
        public class StartPatch
        {

            [HarmonyPostfix]
            private static void Postfix(bool useSteamServer = true)
            {
                Main.Start();
            }
        }
  
    }
}