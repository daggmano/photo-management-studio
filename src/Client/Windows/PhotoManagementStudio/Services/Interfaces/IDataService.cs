using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoManagementStudio.Models;

namespace PhotoManagementStudio.Services.Interfaces
{
    public interface IDataService
    {
        Task<IEnumerable<Media>> GetAllMedia();
        Task<IEnumerable<ITag>> GetAllTagsAsHierarchy();
        Task<IEnumerable<Collection>> GetAllCollections(bool includeMedia = false);
        Task<IEnumerable<Import>> GetAllImports(bool includeMedia = false);
    }
}
