﻿using ChildrenVillageSOS_DAL.DTO.IncomeDTO;
using ChildrenVillageSOS_DAL.Models;
using ChildrenVillageSOS_REPO.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_REPO.Implement
{
    public class IncomeRepository : RepositoryGeneric<Income> ,IIncomeRepository
    {
        public IncomeRepository(SoschildrenVillageDbContext context) : base(context)
        {
            
        }
        public async Task<Income> GetIncomeByDonationIdAsync(int donationId)
        {
            return await _context.Incomes
                .FirstOrDefaultAsync(p => p.DonationId == donationId);
        }

        public IncomeResponseDTO[] GetAllIncome()
        {
            var incomes = _context.Incomes
                .Where(i => !i.IsDeleted) // Exclude deleted records
                .Select(i => new IncomeResponseDTO
                {
                    Id = i.Id,
                    DonationId = i.DonationId,
                    Amount = i.Amount,
                    ReceiveDay = i.Receiveday,
                    Status = i.Status,
                    UserAccountId = i.UserAccountId,
                    FacilitiesWalletId = i.FacilitiesWalletId,
                    FoodStuffWalletId = i.FoodStuffWalletId,
                    HealthWalletId = i.HealthWalletId,
                    NecessitiesWalletId = i.NecessitiesWalletId,
                    SystemWalletId = i.SystemWalletId,
                    CreatedDate = i.CreatedDate,
                    ModifiedDate = i.ModifiedDate
                })
                .ToArray(); // Convert to an array

            return incomes;
        }
    }
}
