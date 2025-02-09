    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Facepunch.Rust;
    using Newtonsoft.Json.Linq;
    using UnityEngine;

    namespace RustDataHarmony
    {
        public static class UpdateChecker 
        {
            private const string SteamAPIUrl = "https://api.steamcmd.net/v1/info/258550";
            private static readonly HttpClient Client = new HttpClient();
            private static  Timer _timer;
            public static void Start()
            {
                _timer = new Timer(async _ => await CheckForUpdate(), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
                CheckForUpdate();
            }

            private static async Task CheckForUpdate()
            {
                int latestBuildId = await FetchLatestRustBuildId();
                int currentBuildId = GetCurrentBuildId();
                
                if (currentBuildId < latestBuildId)
                {
                    Debug.Log("RESTARTING SERVER");
                    ServerMgr.RestartServer("Server outdated!", 0);
                }
            }
            
            private static int GetCurrentBuildId()
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), Directory.GetFiles("./steamapps", "*.acf")[0]);
                var text = File.ReadAllText(path);

                Match match = Regex.Match(text, "\"buildid\"\\s*\"(\\d+)\"");

                if (match.Success)
                {
                    int buildId = int.Parse(match.Groups[1].Value);
                    return buildId;
                }

                throw new Exception("Bag alert");
            }

            private static async Task<int> FetchLatestRustBuildId()
            {
                try
                {
                    string json = await Client.GetStringAsync(SteamAPIUrl);
                    JObject data = JObject.Parse(json);
                    int buildId = data["data"]?["258550"]?["depots"]?["branches"]?["public"]?["buildid"]?.Value<int>() ?? -1;

                    if (buildId == -1)
                        Debug.LogError("Failed to parse the latest Rust build ID from Steam API.");

                    return buildId;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error fetching latest Rust version: {ex.Message}");
                    return -1;
                }
            }
            
        }
    }