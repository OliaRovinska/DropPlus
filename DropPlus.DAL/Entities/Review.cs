using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DropPlus.DAL.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public double Rating { get; set; }

        public string Text { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int ResortId { get; set; }
        public Resort Resort { get; set; }
    }
}