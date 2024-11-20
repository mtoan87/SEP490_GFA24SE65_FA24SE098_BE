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
        public IncomeService(IIncomeRepository incomeRepository, IDonationRepository donationRepository) 
        { 
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
           
            var newExpense = new Income
            {
              
                Amount = createIncome.Amount,
                DonationId = createIncome.DonationId,
                UserAccountId = createIncome.UserAccountId,
                Receiveday = DateTime.Now,
                Status = "Approved",
                IsDeleted = createIncome.IsDeleted,
            };
            await _incomeRepository.AddAsync(newExpense);
            return newExpense;
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
