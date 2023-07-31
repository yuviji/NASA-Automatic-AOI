using System;

namespace aoiRouting.Shared.Models
{
    public record Pin
    {
        public Guid ID { get; set; }
        public Guid AOIID { get; set; }
        public Guid UserID { get; set; }
        public int PointID { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public Position Position
        {
            get => new Position(Lat, Lon);
            set
            {
                Lat = value.Lat;
                Lon = value.Lon;
            }
        }
		public string Notes { get; set; }
		public DateTime Created { get; set; }
        public DateTime? Collected { get; set; }
	}
}