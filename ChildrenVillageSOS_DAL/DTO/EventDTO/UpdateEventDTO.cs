﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_DAL.DTO.EventDTO
{
    public class UpdateEventDTO
    {
        public string? EventCode { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }
        public int? FacilitiesWalletId { get; set; }

        public int? FoodStuffWalletId { get; set; }

        public int? SystemWalletId { get; set; }

        public int? HealthWalletId { get; set; }

        public int? NecessitiesWalletId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? Status { get; set; }

        public bool IsDeleted { get; set; }

        public decimal? CurrentAmount { get; set; }

        public decimal? Amount { get; set; }

        public decimal? AmountLimit { get; set; }
       
        public List<IFormFile>? Img { get; set; }

        public List<string>? ImgToDelete { get; set; }
    }
}
