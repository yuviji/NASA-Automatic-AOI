using System;
namespace aoiRouting.Shared.Models
{
    public record AOI
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public Guid CentroidID { get; set; }
		public string Notes { get; set; }
		public DateTime Created { get; set; }
    }
}