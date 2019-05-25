using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DropPlus.DAL.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public virtual ICollection<ResortTag> ResortTags { get; set; }
    }
}