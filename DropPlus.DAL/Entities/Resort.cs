using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DropPlus.DAL.Enums;

namespace DropPlus.DAL.Entities
{
    public class Resort
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public double Rating { get; set; }

        public string Location { get; set; } // change all locations/cities

        public bool IsSanatorium { get; set; }

        [NotMapped]
        public SeasonEnum[] Seasons
        {
            get => SeasonsId.Split(',').Select(seasonId => (SeasonEnum)int.Parse(seasonId)).ToArray();
            set => SeasonsId = string.Join(',', value);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SeasonsId { get; set; }


        public virtual ICollection<AppUserResort> AppUserResorts { get; set; }

        public virtual ICollection<ResortTag> ResortTags { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}