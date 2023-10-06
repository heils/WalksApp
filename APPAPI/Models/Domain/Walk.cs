namespace APPAPI.Models.Domain
{
    public class Walk
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Length { get; set; }
        public string? WalkImageUrl { get; set; }
        public Guid RegionId { get; set; }
        public Guid DifficultyId { get; set; }
        


        //This is gonna tell entityframework that a walk has a difficulty same for region 1 to 1 relationship
        public Difficulty Difficulty { get; set; }
        public Region Region { get; set; }
    }
}
