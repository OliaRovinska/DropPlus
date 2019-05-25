namespace DropPlus.DAL.Entities
{
    public class AppUserResort
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int ResortId { get; set; }
        public Resort Resort { get; set; }
    }
}