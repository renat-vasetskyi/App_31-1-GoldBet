using System;

namespace Analytics_Logic
{
    [Serializable]
    public class AvailabilityResponse
    {
        public bool result;          // не Result
        public string postback_url;  // не PostbackUrl
    }
}