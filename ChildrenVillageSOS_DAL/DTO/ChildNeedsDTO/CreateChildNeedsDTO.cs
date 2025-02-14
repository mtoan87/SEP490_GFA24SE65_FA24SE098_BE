﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_DAL.DTO.ChildNeedsDTO
{
    public class CreateChildNeedsDTO
    {
        public string ChildId { get; set; } = null!;

        public string? NeedDescription { get; set; }

        public string? NeedType { get; set; }

        public string? Priority { get; set; }

        public DateTime? FulfilledDate { get; set; }

        public string? Remarks { get; set; }

        public string? Status { get; set; }

        public string? CreatedBy { get; set; }
    }
}
