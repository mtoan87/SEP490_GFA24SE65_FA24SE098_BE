﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_DAL.DTO.DonationDTO
{
    public class CreateDonationPayment
    {
        public string? UserAccountId { get; set; }

        public string DonationType { get; set; } = null!;

        public DateTime DateTime { get; set; }

        public decimal Amount { get; set; }
        public bool? IsDeleted { get; set; }
        public string Description { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
