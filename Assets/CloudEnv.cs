using System;
using UnityEngine;

namespace CloudBuild{
    public static class CloudEnv
    {
        public static string appKey = "";
        public static string clientKey = "";

        public static void GetEnvironmentVariables()
        {
            appKey = Environment.GetEnvironmentVariable("appKey");
            clientKey = Environment.GetEnvironmentVariable("clientKey");
            PlayerPrefs.SetString("appKey", appKey);
            PlayerPrefs.SetString("clientKey", clientKey);
        }

        public static void ResolveEnvData()
        {
            appKey = PlayerPrefs.GetString("appKey", appKey);
            clientKey = PlayerPrefs.GetString("clientKey", clientKey);
        }
    }
}
