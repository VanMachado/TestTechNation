using TesteTechNationApplication.Repositories;

HttpClient client = new HttpClient();
ConvertPayload service = new ConvertPayload(client);

await service.ConvertToAgoraTxt();