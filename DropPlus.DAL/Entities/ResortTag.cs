namespace DropPlus.DAL.Entities
{
    public class ResortTag
    {
        public int ResortId { get; set; }
        public Resort Resort { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}