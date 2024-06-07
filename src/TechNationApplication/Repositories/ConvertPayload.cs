using System.Globalization;
using TesteTechNationApplication.Models;
using TesteTechNationApplication.Repositories.IRepositories;

namespace TesteTechNationApplication.Repositories
{
    public class ConvertPayload : IConvertPayload
    {
        private readonly HttpClient _client;

        public ConvertPayload(HttpClient client)
        {
            _client = client;
        }

        public async Task ConvertToAgoraTxt()
        {
            Console.WriteLine("Input data in this pattern: convert <sourceUrl> <fileName.txt>");
            var input = Console.ReadLine().Split(" ");

            while (input.Length != 3 || !input[1].Contains("http") || !input[2].Contains("txt"))
            {
                Console.WriteLine("Please, Input data in this pattern: convert <sourceUrl> <fileName.txt>");
                input = Console.ReadLine().Split(" ");
            }

            var targetPath = input[2];
            var list = await GetMinhaCDN(input);

            try
            {
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var folders = targetPath.Split("/");

                for (int i = 0; i < folders.Length - 1; i++)
                {
                    if (folders[i] == ".")
                        folderPath += "";
                    else
                        folderPath += $@"\{folders[i]}";
                }

                Directory.CreateDirectory(folderPath);
                var filePath = folderPath + $@"\{folders[folders.Length - 1]}";

                using (StreamWriter sw = File.AppendText(filePath))
                {
                    if (new FileInfo(filePath).Length == 0)
                    {
                        await sw.WriteLineAsync($"#Version: 1.0\n#Date: {DateTime.Now}" +
                                "\n#Fields: provider http-method status-code uri-path time-taken response-size cache-status\n");
                    }
                    else
                    {
                        await sw.WriteLineAsync($"\n#Version: 1.0\n#Date: {DateTime.Now}");
                    }

                    foreach (var item in list)
                    {
                        var payloadSplited = item.Payload.Split(" ");
                        var timeTaken = (int)Math.Round(item.TimeTaken, 0);
                        Agora agora = new Agora(payloadSplited[0].Trim('"'), item.StatusCode,
                            payloadSplited[1], timeTaken, item.ResponseSize, item.StatusCache);

                        await sw.WriteLineAsync(agora.ToString());
                    }
                }

                Console.WriteLine($"Your doc was saved here: {folderPath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred! {e.Message}");
            }
        }

        public async Task<List<MinhaCDN>> GetMinhaCDN(string[] input)
        {
            List<MinhaCDN> minhaCDN = new List<MinhaCDN>();

            var sourceUrl = input[1];

            try
            {
                var request = await _client.GetAsync(sourceUrl);

                if (request.IsSuccessStatusCode)
                {
                    var response = await request.Content.ReadAsStringAsync();
                    var lines = response.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        var parts = line.Split('|');
                        int.TryParse(parts[0], out var responseSize);
                        int.TryParse(parts[1], out var status);
                        string cacheStatus = parts[2];
                        string payload = parts[3];
                        float.TryParse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture, out var timeTaken);

                        MinhaCDN temporaryCDN = new MinhaCDN(responseSize, status, cacheStatus, payload, timeTaken);
                        minhaCDN.Add(temporaryCDN);
                    }

                    return minhaCDN;
                }
                else
                {
                    throw new Exception("Something went wrong when calling API");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error mensage: {e.Message}");
            }
        }
    }
}