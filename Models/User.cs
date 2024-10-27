namespace Models;

public class User
{
    public int Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public CLasses Class { get; set; }
    public ICollection<MapSpot>? MapSpots { get; set; }
}