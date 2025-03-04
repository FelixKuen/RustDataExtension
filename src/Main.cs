using System;
using System.Linq;
using API.Assembly;
using Carbon;
using Carbon.Components;
using RustDataHarmonyMod;
using UnityEngine;
using VLB;

namespace RustDataHarmony
{
    
    public class Main : IHarmonyModHooks
    {
        public static Main Instance;

        public static void Start()
        {
            Debug.Log("IT STARTED");
            Instance = new Main();
            ElementChecker.Start();
            FileBuilder.Start();
        }

        public void OnLoaded(OnHarmonyModLoadedArgs args)
        {
            Start();
        }

        public void OnUnloaded(OnHarmonyModUnloadedArgs args)
        {
            Debug.Log("BYEYAKSJDALSD");
        }
    }
}