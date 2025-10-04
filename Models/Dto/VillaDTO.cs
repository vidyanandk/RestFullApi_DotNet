namespace RestFullApi.Models.Dto
{
    public class VillaDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public int Sqft { get; set; }

        public int Occupancy { get; set; }
    }
}
