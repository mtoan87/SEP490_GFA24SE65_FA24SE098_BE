﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_DAL.DTO.ExpenseDTO
{
    public class UpdateExpenseDTO
    {
        public decimal ExpenseAmount { get; set; }

        public string Description { get; set; }

        public string HouseId { get; set; }

       
    }
}
