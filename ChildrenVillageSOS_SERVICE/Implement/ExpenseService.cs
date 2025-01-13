﻿using ChildrenVillageSOS_DAL.DTO.ExpenseDTO;
using ChildrenVillageSOS_DAL.DTO.VillageDTO;
using ChildrenVillageSOS_DAL.Enum;
using ChildrenVillageSOS_DAL.Models;
using ChildrenVillageSOS_REPO.Implement;
using ChildrenVillageSOS_REPO.Interface;
using ChildrenVillageSOS_SERVICE.Interface;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_SERVICE.Implement
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IChildRepository _childRepository;
        private readonly INecessitiesWalletRepository _necessitiesWalletService;
        private readonly IHealthWalletRepository _healthWalletService;
        private readonly IFacilitiesWalletRepository _facilitiesWalletService;
        private readonly IFoodStuffWalletRepository _foodStuffWalletService;   
        private readonly ISystemWalletRepository _systemWalletService;
        public ExpenseService(IExpenseRepository expenseRepository,
            IChildRepository childRepository,
            INecessitiesWalletRepository necessitiesWalletService,
            IHealthWalletRepository healthWalletService,
            IFoodStuffWalletRepository foodStuffWalletService,
            IFacilitiesWalletRepository facilitiesWalletService,
            IWalletRepository walletRepository,
            ISystemWalletRepository systemWalletService)
        {
            _childRepository = childRepository;
            _expenseRepository = expenseRepository;
            _necessitiesWalletService = necessitiesWalletService;
            _healthWalletService = healthWalletService;
            _facilitiesWalletService = facilitiesWalletService;
            _foodStuffWalletService = foodStuffWalletService;          
            _systemWalletService = systemWalletService;
        }

        public DataTable getExpense()
        {
            return _expenseRepository.getExpense();
        }
        public ExpenseResponseDTO[] GetFormatedExpenses()
        {
            return _expenseRepository.GetAllExpenses();
        }
        public Expense[] GetExpenseByFacilitiesWalletId(int id)
        {
            return _expenseRepository.GetExpenseByFacilitiesWalletId(id);
        }
        public Expense[] GetExpenseByFoodWalletId(int id)
        {
            return _expenseRepository.GetExpenseByFoodWalletId(id);
        }
        public Expense[] GetExpenseByHealthWalletId(int id)
        {
            return _expenseRepository.GetExpenseByHealthWalletId(id);
        }
        public Expense[] GetExpenseByNesceWalletId(int id)
        {
            return _expenseRepository.GetExpenseByNecessilitiesWalletId(id);
        }
        public Expense[] GetExpenseBySysWalletId(int id)
        {
            return _expenseRepository.GetExpenseBySystemWalletId(id);
        }
        public async Task<IEnumerable<Expense>> GetAllExpenses()
        {
            return await _expenseRepository.GetAllAsync();
        }
        public async Task<Expense> GetExpenseById(int id)
        {
            return await _expenseRepository.GetByIdAsync(id);
        }

        public async Task<Expense> UpdateExpense(int id, UpdateExpenseDTO updateExpense)
        {
            var updExpense = await _expenseRepository.GetByIdAsync(id);
            if (updExpense == null)
            {
                throw new Exception($"Expense with ID {id} not found!");
            }

            decimal oldExpenseAmount = updExpense.ExpenseAmount;
            updExpense.ExpenseAmount = updateExpense.ExpenseAmount;
            updExpense.Description = updateExpense.Description;
            updExpense.ModifiedDate = DateTime.Now;
            updExpense.HouseId = updateExpense.HouseId;

            // Determine which wallet to update based on the expense's wallet
            if (updExpense.FacilitiesWalletId.HasValue)
            {
                var wallet = await _facilitiesWalletService.GetByIdAsync(updExpense.FacilitiesWalletId.Value);
                if (wallet != null)
                {
                    // Adjust the wallet's balance (subtract old expense, add new expense)
                    wallet.Budget += oldExpenseAmount;  // Revert the old expense
                    wallet.Budget -= updateExpense.ExpenseAmount;  // Subtract the new expense
                    await _facilitiesWalletService.UpdateAsync(wallet);
                }
            }

            // Repeat for other wallets like FoodStuffWallet, HealthWallet, etc.
            if (updExpense.FoodStuffWalletId.HasValue)
            {
                var wallet = await _foodStuffWalletService.GetByIdAsync(updExpense.FoodStuffWalletId.Value);
                if (wallet != null)
                {
                    wallet.Budget += oldExpenseAmount;
                    wallet.Budget -= updateExpense.ExpenseAmount;
                    await _foodStuffWalletService.UpdateAsync(wallet);
                }
            }

            if (updExpense.HealthWalletId.HasValue)
            {
                var wallet = await _healthWalletService.GetByIdAsync(updExpense.HealthWalletId.Value);
                if (wallet != null)
                {
                    wallet.Budget += oldExpenseAmount;
                    wallet.Budget -= updateExpense.ExpenseAmount;
                    await _healthWalletService.UpdateAsync(wallet);
                }
            }

            if (updExpense.SystemWalletId.HasValue)
            {
                var wallet = await _systemWalletService.GetByIdAsync(updExpense.SystemWalletId.Value);
                if (wallet != null)
                {
                    wallet.Budget += oldExpenseAmount;
                    wallet.Budget -= updateExpense.ExpenseAmount;
                    await _systemWalletService.UpdateAsync(wallet);
                }
            }

            if (updExpense.NecessitiesWalletId.HasValue)
            {
                var wallet = await _necessitiesWalletService.GetByIdAsync(updExpense.NecessitiesWalletId.Value);
                if (wallet != null)
                {
                    wallet.Budget += oldExpenseAmount;
                    wallet.Budget -= updateExpense.ExpenseAmount;
                    await _necessitiesWalletService.UpdateAsync(wallet);
                }
            }

            // Update the Expense record
            await _expenseRepository.UpdateAsync(updExpense);
            return updExpense;
        }
        public async Task<Expense> CreateExpense(CreateExepenseDTO createExepense)
        {
            var newExpense = new Expense
            {
                ExpenseAmount = createExepense.ExpenseAmount,
                Description = createExepense.Description,
                Expenseday = DateTime.Now,
                CreatedDate = DateTime.Now,
                Status = DonateStatus.Pending.ToString(),
                ExpenseType = ExpenseType.Regular.ToString(),
                HouseId = createExepense.HouseId,
                IsDeleted = false,
                SystemWalletId = createExepense.SystemWalletId,
                FacilitiesWalletId = createExepense.FacilitiesWalletId,
                FoodStuffWalletId = createExepense.FoodStuffWalletId,
                HealthWalletId = createExepense.HealthWalletId,
                NecessitiesWalletId = createExepense.NecessitiesWalletId,
                ChildId = createExepense.ChildId,
                RequestedBy = createExepense.RequestedBy,            
            };
                     
            await _expenseRepository.AddAsync(newExpense);                     
            return newExpense;
        }
        public async Task<Expense> RequestChildExpense(RequestSpecialExpenseDTO requestSpecialExpense)
        {
            // Lấy danh sách các trẻ em dựa trên danh sách ChildId được chọn
            var selectedChildren = await _childRepository.GetChildrenByIdsAsync(requestSpecialExpense.SelectedChildrenIds);

            if (selectedChildren == null || !selectedChildren.Any())
            {
                throw new InvalidOperationException("No children selected or children do not exist.");
            }

            // Lọc những trẻ có HealthStatus là "Bad"
            var childrenWithBadHealth = selectedChildren
                .Where(c => c.HealthStatus.Equals("Bad", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!childrenWithBadHealth.Any())
            {
                throw new InvalidOperationException("None of the selected children have a health status of 'Bad'.");
            }

            // Tính tổng số tiền của những trẻ có HealthStatus là "Bad"
            var totalAmount = childrenWithBadHealth.Sum(c => c.Amount);

            // Tạo đối tượng Special Expense
            var specialExpense = new Expense
            {
                ExpenseAmount = totalAmount ?? 0,
                Description = requestSpecialExpense.Description,
                Expenseday = DateTime.Now,
                CreatedDate = DateTime.Now,
                Status = DonateStatus.Pending.ToString(),
                ExpenseType = ExpenseType.Special.ToString(),
                HouseId = requestSpecialExpense.HouseId,
                IsDeleted = false,
                HealthWalletId = 1,
                RequestedBy = requestSpecialExpense.RequestedBy,
            };

            // Lưu vào cơ sở dữ liệu
            await _expenseRepository.AddAsync(specialExpense);

            return specialExpense;
        }
        public async Task<Expense> DeleteExpense(int id)
        {
            var exp = await _expenseRepository.GetByIdAsync(id);
            if(exp == null)
            {
                throw new Exception($"Expense with ID{id} not found");
            }
            await _expenseRepository.RemoveAsync(exp);
            return exp;
        }
        public async Task<Expense> SoftDelete(int id)
        {
            var exp = await _expenseRepository.GetByIdAsync(id);
            if (exp == null)
            {
                throw new Exception($"Expense with ID{id} not found!");
            }
            exp.IsDeleted = true;
            await _expenseRepository.UpdateAsync(exp);
            return exp;
        }

        public async Task<Expense> ConfirmExpense(int id)
        {
            var exp = await _expenseRepository.GetByIdAsync(id);
            if (exp == null)
            {
                throw new Exception($"Expense with ID{id} is not found");
            }
            exp.Status = "Approved";
            exp.ModifiedDate = DateTime.Now;
            await _expenseRepository.UpdateAsync(exp);
            if (exp.FacilitiesWalletId.HasValue)
            {
                var facilitiesWallet = await _facilitiesWalletService.GetByIdAsync(exp.FacilitiesWalletId.Value);
                if (facilitiesWallet != null)
                {
                    facilitiesWallet.Budget -= exp.ExpenseAmount;
                    if (facilitiesWallet.Budget < 0)
                        throw new InvalidOperationException("Insufficient budget in Facilities Wallet.");
                }
            }


            if (exp.FoodStuffWalletId.HasValue)
            {
                var foodStuffWallet = await _foodStuffWalletService.GetByIdAsync(exp.FoodStuffWalletId.Value);
                if (foodStuffWallet != null)
                {
                    foodStuffWallet.Budget -= exp.ExpenseAmount;
                    if (foodStuffWallet.Budget < 0)
                        throw new InvalidOperationException("Insufficient budget in Food Stuff Wallet.");
                }
            }
            if (exp.HealthWalletId.HasValue)
            {
                var healthWallet = await _healthWalletService.GetByIdAsync(exp.HealthWalletId.Value);
                if (healthWallet != null)
                {
                    healthWallet.Budget -= exp.ExpenseAmount;
                    if (healthWallet.Budget < 0)
                        throw new InvalidOperationException("Insufficient budget in Health Wallet.");
                }
            }
            if (exp.NecessitiesWalletId.HasValue)
            {
                var necessitiesWallet = await _necessitiesWalletService.GetByIdAsync(exp.NecessitiesWalletId.Value);
                if (necessitiesWallet != null)
                {
                    necessitiesWallet.Budget -= exp.ExpenseAmount;
                    if (necessitiesWallet.Budget < 0)
                        throw new InvalidOperationException("Insufficient budget in Necessities Wallet.");
                }
            }           
            return exp;
        }
        public async Task<Expense> ConfirmSpecialExpense(List<string> selectedHouseIds)
        {
            // Kiểm tra danh sách HouseId
            if (selectedHouseIds == null || !selectedHouseIds.Any())
            {
                throw new ArgumentException("No houses selected for confirmation.");
            }

            // Lấy danh sách Expense từ các House đã chọn
            var houseExpenses = await _expenseRepository.GetExpensesByHouseIdsAsync(
                selectedHouseIds, "Special", "Pending");

            if (!houseExpenses.Any())
            {
                throw new InvalidOperationException("No pending special expenses found for the selected houses.");
            }

            // Lấy VillageId từ các House và kiểm tra xem chúng có thuộc cùng một Village
            var villageIds = houseExpenses
                .Select(e => e.VillageId)
                .Distinct()
                .ToList();

            if (villageIds.Count > 1)
            {
                throw new InvalidOperationException("Selected houses must belong to the same village.");
            }

            // Lấy VillageId (tất cả House đã xác nhận thuộc cùng một Village)
            var villageId = villageIds.First();

            // Cập nhật trạng thái của các Expense đã chọn
            foreach (var expense in houseExpenses)
            {
                expense.Status = "EventRequest";
                expense.ModifiedDate = DateTime.Now;
                await _expenseRepository.UpdateAsync(expense);
            }

            // Tính tổng ExpenseAmount
            var totalExpenseAmount = houseExpenses.Sum(e => e.ExpenseAmount);

            // Tạo Expense mới cho Village
            var villageExpense = new Expense
            {
                ExpenseType = "Special",
                Status = "EventRequest",
                ExpenseAmount = totalExpenseAmount,
                Expenseday = DateTime.Now,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                IsDeleted = false,
                HouseId = null, // Đây là Expense của Village
                VillageId = villageId,
            };

            // Lưu Expense mới cho Village
            await _expenseRepository.AddAsync(villageExpense);

            return villageExpense;
        }



        public async Task<List<Expense>> SearchExpenses(SearchExpenseDTO searchExpenseDTO)
        {
            return await _expenseRepository.SearchExpenses(searchExpenseDTO);
        }
    }
}
