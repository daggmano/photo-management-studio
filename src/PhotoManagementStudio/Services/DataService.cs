using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Catel;
using Catel.Collections;
using MyCouch;
using MyCouch.Requests;
using PhotoManagementStudio.Models;
using PhotoManagementStudio.Services.Interfaces;

namespace PhotoManagementStudio.Services
{
    public class DataService : IDataService
    {
        private readonly string _couchDbUrl;

        public DataService()
        {
            _couchDbUrl = ConfigurationManager.AppSettings["CouchDbPath"];
            Argument.IsNotNullOrWhitespace(() => _couchDbUrl);
        }

        public async Task<IEnumerable<Media>> GetAllMedia()
        {
            using (var store = new MyCouchStore(_couchDbUrl))
            {
                var mediaQuery = new QueryViewRequest("media", "all");
                var mediaRows = await store.Client.Views.QueryAsync<Media>(mediaQuery);
                var allMedia = mediaRows.Rows
                    .Select(x => x.Value)
                    .OrderByDescending(x => x.ShotDate)
                    .ThenBy(x => x.FullFilePath);

                var tagQuery = new QueryViewRequest("tags", "tags");
                var tagRows = await store.Client.Views.QueryAsync<Tag>(tagQuery);
                var tags = tagRows.Rows.Select(x => x.Value).ToList();

                foreach (var media in allMedia)
                {
                    media.Tags.AddRange(tags.Where(x => media.TagIds.Contains(x.TagId)));
                }

                return allMedia;
            }
        }

        public async Task<IEnumerable<ITag>> GetAllTagsAsHierarchy()
        {
            using (var store = new MyCouchStore(_couchDbUrl))
            {
                var parentTagQuery = new QueryViewRequest("tags", "parents");
                var bucketTagQuery = new QueryViewRequest("tags", "buckets");
                var tagQuery = new QueryViewRequest("tags", "tags");

                var parentTagRows = await store.Client.Views.QueryAsync<TagParent>(parentTagQuery);
                var parentTags = parentTagRows.Rows
                    .Select(x => x.Value)
                    .OrderBy(x => x.Name)
                    .ToList();

                var bucketTagRows = await store.Client.Views.QueryAsync<TagBucket>(bucketTagQuery);
                var bucketTags = bucketTagRows.Rows.Select(x => x.Value).ToList();
                
                var tagRows = await store.Client.Views.QueryAsync<Tag>(tagQuery);
                var tags = tagRows.Rows.Select(x => x.Value).ToList();

                var allTags = ((IEnumerable<ITag>)parentTags).Concat(bucketTags).Concat(tags).ToList();

                foreach (var parentTag in parentTags)
                {
                    FillSubTags(parentTag, allTags);
                }

                return parentTags;
            }
        }

        public async Task<IEnumerable<Collection>> GetAllCollections(bool includeMedia = false)
        {
            using (var store = new MyCouchStore(_couchDbUrl))
            {
                var collectionQuery = new QueryViewRequest("collections", "all");
                var collectionRows = await store.Client.Views.QueryAsync<Collection>(collectionQuery);
                var collections = collectionRows.Rows
                    .Select(x => x.Value)
                    .OrderBy(x => x.Name)
                    .ToList();

                if (includeMedia)
                {
                    foreach (var collection in collections)
                    {
                        if (collection.MediaIds != null && collection.MediaIds.Any())
                        {
                            await
                                store.GetByIdsAsync<Media>(collection.MediaIds.ToArray(), m => collection.Medias.Add(m));
                        }
                    }
                }

                return collections;
            }
        }

        public async Task<IEnumerable<Import>> GetAllImports(bool includeMedia = false)
        {
            using (var store = new MyCouchStore(_couchDbUrl))
            {
                var importQuery = new QueryViewRequest("imports", "all");
                var importRows = await store.Client.Views.QueryAsync<Import>(importQuery);
                var imports = importRows.Rows.Select(x => x.Value)
                    .OrderBy(x => x.ImportDate)
                    .ToList();

                if (includeMedia)
                {
                    foreach (var import in imports)
                    {
                        if (import.MediaIds != null && import.MediaIds.Any())
                        {
                            await store.GetByIdsAsync<Media>(import.MediaIds.ToArray(), m => import.Medias.Add(m));
                        }
                    }
                }

                return imports;
            }
        }

        private static void FillSubTags(ITag tag, IEnumerable<ITag> allTags)
        {
            string parentId = null;
            IEnumerable<string> childIds = null;
            switch (tag.TagType)
            {
                case TagTypes.Tag:
                    var child = tag as Tag;
                    parentId = child == null ? null : child.ParentId;
                    break;

                case TagTypes.Bucket:
                    var bucket = tag as TagBucket;
                    parentId = bucket == null ? null : bucket.ParentId;
                    childIds = bucket == null ? null : bucket.ChildrenIds;
                    break;

                case TagTypes.Parent:
                    var parent = tag as TagParent;
                    childIds = parent == null ? null : parent.ChildrenIds;
                    break;
            }

            if (parentId != null)
            {
                tag.Parent = allTags.SingleOrDefault(x => x.TagId == parentId);
            }

            if (childIds != null)
            {
                tag.Children = new ObservableCollection<ITag>(allTags.Where(x => childIds.Contains(x.TagId)).OrderBy(x => x.Name));
                foreach (var t in tag.Children)
                {
                    FillSubTags(t, allTags);
                }
            }
        }
    }
}
