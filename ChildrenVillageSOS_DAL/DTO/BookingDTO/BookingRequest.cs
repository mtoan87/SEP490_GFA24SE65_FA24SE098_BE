﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_DAL.DTO.BookingDTO
{
    public class BookingRequest
    {
        public string HouseId { get; set; }
        public DateOnly Visitday { get; set; }
        public int BookingSlotId { get; set; }
        public string UserAccountId { get; set; }
    }
}