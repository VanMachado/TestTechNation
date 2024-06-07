using TesteTechNationApplication.Models;

namespace TesteTechNationApplication.Repositories.IRepositories
{
    public interface IConvertPayload
    {
        Task<List<MinhaCDN>> GetMinhaCDN(string[] input);
        Task ConvertToAgoraTxt();
    }
}
