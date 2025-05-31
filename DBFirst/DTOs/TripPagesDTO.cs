namespace DBFirst.DTOs;

public class TripPagesDTO
{
    public int pageNum { get; set; }
    public int pageSize { get; set; }
    public int allPages { get; set; }
    public List<TripDTO> trips { get; set; }
}