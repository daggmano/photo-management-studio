using System.Threading.Tasks;
using System;
using DataTypes;

namespace PhotoLibraryImageService.Data.Interfaces
{
    public interface IDataService
    {
        Task<ServerDetail> GetServerDatabaseIdentifier();
		Task<Import> CreateImportTag(Guid tagId, DateTime importDate);
		Task<Media> InsertMedia(Media media);

	}
}
