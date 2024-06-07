using Moq;
using Moq.Protected;
using TesteTechNationApplication.Repositories;

namespace TesteTechNationApplication.Tests
{
    public class ConvertPayloadTests
    {
        [Fact]
        public async Task GetMinhaCDN_ShouldReturnListOfMinhaCDN()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2\n101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4\n199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9\n312|200|INVALIDATE|\"GET /robots.txt HTTP/1.1\"|245.1")
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);
            var convertPayload = new ConvertPayload(httpClient);

            var input = new[] { "convert", "http://example.com", "output.txt" };

            var result = await convertPayload.GetMinhaCDN(input);

            Assert.Equal(4, result.Count);
            Assert.Equal(312, result[0].ResponseSize);
            Assert.Equal(200, result[0].StatusCode);
            Assert.Equal("HIT", result[0].StatusCache);
            Assert.Equal("\"GET /robots.txt HTTP/1.1\"", result[0].Payload);
            Assert.Equal(100.2f, result[0].TimeTaken);
        }

        [Fact]
        public async Task ConvertToAgoraTxt_ShouldCreateFileWithCorrectContent()
        {            
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2\n101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4\n199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9\n312|200|INVALIDATE|\"GET /robots.txt HTTP/1.1\"|245.1")
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);
            var convertPayload = new ConvertPayload(httpClient);

            var input = new[] { "convert", "http://example.com", "output/output.txt" };

            var sw = new StringWriter();
            Console.SetOut(sw);
            Console.SetIn(new StringReader("convert http://example.com output/output.txt\n"));

            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(folderPath, "output/output.txt");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
                        
            await convertPayload.ConvertToAgoraTxt();
                        
            Assert.True(File.Exists(filePath));
            var lines = await File.ReadAllLinesAsync(filePath);
                        
            var dataLines = lines.Skip(3).ToArray();

            Assert.Contains(dataLines, line => line.Contains("\"Minha CDN\" GET 200 /robots.txt 100 312 HIT"));
            Assert.Contains(dataLines, line => line.Contains("\"Minha CDN\" POST 200 /myImages 319 101 MISS"));
            Assert.Contains(dataLines, line => line.Contains("\"Minha CDN\" GET 404 /not-found 143 199 MISS"));
            Assert.Contains(dataLines, line => line.Contains("\"Minha CDN\" GET 200 /robots.txt 245 312 REFRESH_HIT"));
        }
    }
}
