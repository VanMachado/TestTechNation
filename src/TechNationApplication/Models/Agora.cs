namespace TesteTechNationApplication.Models
{
    public class Agora
    {        
        public string HttpMethod { get; set; }
        public int StatusCode { get; set; }
        public string UriPath { get; set; }
        public int TimeTaken { get; set; }
        public int ResponseSize { get; set; }
        public string CacheStatus { get; set; }

        public Agora(string httpMethod, int statusCode, string uriPath, 
            int timeTaken, int responseSize, string cacheStatus)
        {            
            HttpMethod = httpMethod;
            StatusCode = statusCode;
            UriPath = uriPath;
            TimeTaken = timeTaken;
            ResponseSize = responseSize;
            CacheStatus = cacheStatus == "INVALIDATE" ? "REFRESH_HIT" : cacheStatus;
        }

        public override string ToString()
        {
            return $"\"Minha CDN\" {HttpMethod} {StatusCode} {UriPath} " +
                $"{TimeTaken} {ResponseSize} {CacheStatus}";
        }
    }
}
