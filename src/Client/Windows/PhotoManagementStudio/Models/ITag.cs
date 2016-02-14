using System.Collections.ObjectModel;

namespace PhotoManagementStudio.Models
{
    public enum TagTypes
    {
        Parent,
        Bucket,
        Tag
    }

    public interface ITag
    {
        TagTypes TagType { get; }
        string TagId { get; set; }
        string Name { get; set; }
        int Colour { get; set; }
        ITag Parent { get; set; }
        ObservableCollection<ITag> Children { get; set; }
    }
}
