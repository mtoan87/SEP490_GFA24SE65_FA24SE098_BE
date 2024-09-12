﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_DAL.DTO.ChildDTO
{
    public class CreateChildDTO
    {
        public string ChildName { get; set; }

        public string HealthStatus { get; set; }

        public string HouseId { get; set; }

        public string Gender { get; set; }

        //public DateOnly Dob { get; set; }

        public string AcademicLevel { get; set; }

        public string Certificate { get; set; }

        public bool? IsDelete { get; set; }
    }
}
