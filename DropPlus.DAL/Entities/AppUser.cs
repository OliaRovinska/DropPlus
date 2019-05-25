using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DropPlus.DAL.Enums;
using Microsoft.AspNetCore.Identity;

namespace DropPlus.DAL.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Birth { get; set; }

        public string Photo { get; set; }

        public string City { get; set; }

        public int Height { get; set; }

        public int Weight { get; set; }

        public int WakeUpHour { get; set; }

        public int SleepHour { get; set; }

        public int DailyGoal { get; set; }

        public int NumberOfDays { get; set; }

        public int ActivityLevel { get; set; }

        public virtual int SexId
        {
            get => (int)Sex;
            set => Sex = (SexEnum)value;
        }
        [NotMapped]
        public SexEnum Sex { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? RefreshTokenValidUntil { get; set; }


        public virtual ICollection<AppUserResort> AppUserResorts { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}