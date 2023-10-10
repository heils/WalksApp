namespace Client.Models.Dto
{
    public class CreateWalkDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Length { get; set; }
        public string? WalkImageUrl { get; set; }
        public Guid RegionId { get; set; }
        public Guid DifficultyId { get; set; }
        
    }
}
