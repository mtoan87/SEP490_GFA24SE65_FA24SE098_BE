﻿using ChildrenVillageSOS_DAL.DTO.IncomeDTO;
using ChildrenVillageSOS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_SERVICE.Interface
{
    public interface IIncomeService
    {
        Task<IEnumerable<Income>> GetAllIncomes();
        Task<Income> GetIncomeById(int id);
        Task<Income> CreateIncome(CreateIncomeDTO createIncome);
        Task<Income> UpdateIncome(int id, UpdateIncomeDTO updateIncome);
        Task<Income> DeleteIncome(int id);
    }
}
