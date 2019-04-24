using System.Collections.Generic;

namespace ManagerLayer.LaunchManagement
{
    // Represents the response from the API for a launch request
    public class LaunchData
    {
        public string url { get; set; }
        public Dictionary<string, string> launchPayload { get; set; }
    }
}
