using System;

namespace CloudBuild{
    public static class CloudEnv
    {
        public static string appKey = "";
        public static string clientKey = "";

        public static void GetEnvironmentVariables()
        {
            appKey = Environment.GetEnvironmentVariable("appKey");
            clientKey = Environment.GetEnvironmentVariable("clientKey");
        }
    }
}
