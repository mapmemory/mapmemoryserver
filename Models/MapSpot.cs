namespace Models;



public class MapSpot
{
    public int Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string Description { get; set; } = string.Empty;
    public SpotType Type { get; set; }
    public string picture { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int userId { get; set; }
    public virtual User? User { get; set; }
}