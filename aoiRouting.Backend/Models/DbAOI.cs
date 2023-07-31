using LinqToDB.Mapping;
using aoiRouting.Shared.Models;

namespace aoiRouting.Backend.Models
{
    [Table("aois")]
    [Column("id", nameof(ID))]
    [Column("user_id", nameof(UserID))]
    [Column("centroid_id", nameof(CentroidID))]
    [Column("created", nameof(Created))]
    [Column("notes", nameof(Notes))]
    public record DbAOI : AOI { }
}