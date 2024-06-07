namespace TesteTechNationApplication.Models
{
    public class MinhaCDN
    {
        public int ResponseSize { get; set; }
        public int StatusCode { get; set; }
        public string StatusCache { get; set; }
        public string Payload { get; set; }
        public float TimeTaken { get; set; }

        public MinhaCDN(int responseSize, int statusCode, string statusCache,
            string payload, float timeTaken)
        {
            ResponseSize = responseSize;
            StatusCode = statusCode;
            StatusCache = statusCache;
            Payload = payload;
            TimeTaken = timeTaken;
        }
    }
}
