﻿using ChildrenVillageSOS_DAL.DTO.ExpenseDTO;
using ChildrenVillageSOS_DAL.DTO.IncomeDTO;
using ChildrenVillageSOS_DAL.Models;
using ChildrenVillageSOS_REPO.Implement;
using ChildrenVillageSOS_REPO.Interface;
using ChildrenVillageSOS_SERVICE.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_SERVICE.Implement
{
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IDonationRepository _donationRepository;
        private readonly IFacilitiesWalletRepository _failitiesWalletRepository;
        private readonly IFoodStuffWalletRepository _foodStuffWalletRepository;
        private readonly INecessitiesWalletRepository _necessitiesWalletRepository;
        private readonly ISystemWalletRepository _systemWalletRepository;
        private readonly IHealthWalletRepository _healthWalletRepository;
        private readonly IWalletRepository _walletRepository;
        public IncomeService(IIncomeRepository incomeRepository,
            IDonationRepository donationRepository,
            IFacilitiesWalletRepository facilitiesWalletRepository,
            ISystemWalletRepository systemWalletRepository,
            INecessitiesWalletRepository necessitiesWalletRepository,
            IFoodStuffWalletRepository foodStuffWalletRepository,
            IHealthWalletRepository healthWalletRepository,
            IWalletRepository walletRepository) 
        {
            _walletRepository = walletRepository;
            _failitiesWalletRepository = facilitiesWalletRepository;
            _systemWalletRepository = systemWalletRepository;
            _foodStuffWalletRepository = foodStuffWalletRepository;
            _necessitiesWalletRepository = necessitiesWalletRepository;
            _healthWalletRepository = healthWalletRepository;
            _incomeRepository = incomeRepository;   
            _donationRepository = donationRepository;
        }
        public async Task<IEnumerable<Income>> GetAllIncomes()
        {
            return await _incomeRepository.GetAllAsync();
        }
        public IncomeResponseDTO[] GetFormatedIncome()
        {
            return _incomeRepository.GetAllIncome();
        }
        public async Task<Income> GetIncomeById(int id)
        {
            return await _incomeRepository.GetByIdAsync(id);
        }

        public async Task<Income> GetIncomeByDonationIdAsync(int donationId)
        {
            return await _incomeRepository.GetIncomeByDonationIdAsync(donationId);
        }
        public async Task<Income> CreateIncome(CreateIncomeDTO createIncome)
        {
            var donate = await _donationRepository.GetByIdAsync(createIncome.DonationId);
            if (donate.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This Donation already paid!");
            }
            var newIncome = new Income
            {
              
                Amount = donate.Amount,
                DonationId = createIncome.DonationId,
                UserAccountId = createIncome.UserAccountId,
                Receiveday = DateTime.Now,
                Status = "Approved",
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                FacilitiesWalletId = donate.FacilitiesWalletId,
                NecessitiesWalletId = donate.NecessitiesWalletId,
                HealthWalletId = donate.HealthWalletId,
                SystemWalletId  = donate.SystemWalletId,
                FoodStuffWalletId = donate.FoodStuffWalletId,

            };           
            if (donate.FacilitiesWalletId.HasValue)
            {
                decimal donationAmount = donate.Amount;
                await _walletRepository.UpdateFacilitiesWalletBudget(donate.FacilitiesWalletId.Value, donationAmount);
            }

            if (donate.NecessitiesWalletId.HasValue)
            {
                decimal donationAmount = donate.Amount;
                await _walletRepository.UpdateNecessitiesWalletBudget(donate.NecessitiesWalletId.Value, donationAmount);
            }

            if (donate.HealthWalletId.HasValue)
            {
                decimal donationAmount = donate.Amount;
                await _walletRepository.UpdateHealthWalletBudget(donate.HealthWalletId.Value, donationAmount);
            }

            if (donate.SystemWalletId.HasValue)
            {
                decimal donationAmount = donate.Amount;
                await _walletRepository.UpdateSystemWalletBudget(donate.SystemWalletId.Value, donationAmount);
            }

            if (donate.FoodStuffWalletId.HasValue)
            {
                decimal donationAmount = donate.Amount;
                await _walletRepository.UpdateFoodStuffWalletBudget(donate.FoodStuffWalletId.Value, donationAmount);
            }
            donate.Status = "Paid";
            await _donationRepository.UpdateAsync(donate);
            await _incomeRepository.AddAsync(newIncome);
            return newIncome;
        }
        public async Task<Income> UpdateIncome(int id, UpdateIncomeDTO updateIncome)
        {
            var updIncome = await _incomeRepository.GetByIdAsync(id);
            if (updIncome == null)
            {
                throw new Exception($"Income with ID{id} not found!");
            }
            updIncome.UserAccountId = updateIncome.UserAccountId;
            updIncome.ModifiedDate = DateTime.Now;
            updIncome.IsDeleted = updateIncome.IsDeleted;
            await _incomeRepository.UpdateAsync(updIncome);
            return updIncome;

        }
        public async Task<Income> DeleteIncome(int id)
        {
            var inc = await _incomeRepository.GetByIdAsync(id);
            if (inc == null)
            {
                throw new Exception($"Income with ID{id} not found");
            }
            await _incomeRepository.RemoveAsync(inc);
            return inc;
        }
        public async Task<Income> SoftDelete(int id)
        {
            var updIncome = await _incomeRepository.GetByIdAsync(id);
            if (updIncome == null)
            {
                throw new Exception($"Income with ID{id} not found!");
            }           
            updIncome.IsDeleted = true;
            await _incomeRepository.UpdateAsync(updIncome);
            return updIncome;
        }
    }
}
