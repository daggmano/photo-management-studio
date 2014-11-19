using System.Threading.Tasks;
using Shared;

namespace PhotoLibraryImageService.Data.Interfaces
{
    public interface IDataService
    {
        Task<ServerDatabaseIdentifierObject> GetServerDatabaseIdentifier();
    }
}
