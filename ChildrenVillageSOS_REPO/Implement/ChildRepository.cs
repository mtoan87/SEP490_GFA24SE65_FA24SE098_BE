﻿using ChildrenVillageSOS_DAL.DTO.ChildDTO;
using ChildrenVillageSOS_DAL.DTO.DashboardDTO.TopStatCards;
using ChildrenVillageSOS_DAL.DTO.EventDTO;
using ChildrenVillageSOS_DAL.DTO.HouseDTO;
using ChildrenVillageSOS_DAL.Models;
using ChildrenVillageSOS_REPO.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_REPO.Implement
{
    public class ChildRepository : RepositoryGeneric<Child>, IChildRepository
    {
        public ChildRepository(SoschildrenVillageDbContext context) : base(context)
        {

        }
        public async Task<IEnumerable<Child>> GetAllNotDeletedAsync()
        {
            // Sử dụng Include để lấy các hình ảnh liên quan đến Event
            return await _context.Children
                                 .Include(e => e.Images)  // Dùng Include để lấy các hình ảnh liên quan
                                 .Where(e => !e.IsDeleted) // Nếu cần, lọc các sự kiện không bị xóa
                                 .ToListAsync();
        }

        public async Task<ChildResponseDTO[]> GetAllChildIsDeleteAsync()
        {
            return await _context.Children
                .Where(x => x.IsDeleted)
                .Select(x => new ChildResponseDTO
                {
                    Id = x.Id,
                    ChildName = x.ChildName,
                    HealthStatus = x.HealthStatus,
                    HouseId = x.HouseId,
                    FacilitiesWalletId = x.FacilitiesWalletId,
                    SystemWalletId = x.SystemWalletId,
                    FoodStuffWalletId = x.FoodStuffWalletId,
                    HealthWalletId = x.HealthWalletId,
                    NecessitiesWalletId = x.NecessitiesWalletId,
                    Amount = x.Amount ?? 0,
                    CurrentAmount = x.CurrentAmount ?? 0,
                    AmountLimit = x.AmountLimit ?? 0,
                    Gender = x.Gender,
                    Dob = x.Dob,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    Status = x.Status,
                    ImageUrls = x.Images.Where(img => !img.IsDeleted)
                                     .Select(img => img.UrlPath)
                                     .ToArray()               // Convert to array
                })
                .ToArrayAsync();  // Execute query and convert the result to an array asynchronously
        }

        public ChildResponseDTO GetChildByIdWithImg(string childId)
        {
            var childDetails = _context.Children
                .Where(x => x.Id == childId && !x.IsDeleted) // Ensure the event is not deleted
                .Select(x => new ChildResponseDTO
                {
                    Id = x.Id,
                    ChildName = x.ChildName,
                    HealthStatus = x.HealthStatus,
                    HouseId = x.HouseId,
                    FacilitiesWalletId = x.FacilitiesWalletId,
                    SystemWalletId = x.SystemWalletId,
                    FoodStuffWalletId = x.FoodStuffWalletId,
                    HealthWalletId = x.HealthWalletId,
                    NecessitiesWalletId = x.NecessitiesWalletId,
                    Amount = x.Amount ?? 0,
                    CurrentAmount = x.CurrentAmount ?? 0,
                    AmountLimit = x.AmountLimit ?? 0,
                    Gender = x.Gender,
                    Dob = x.Dob,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    Status = x.Status,
                    ImageUrls = x.Images.Where(img => !img.IsDeleted)
                                     .Select(img => img.UrlPath)
                                     .ToArray()
                })
                .FirstOrDefault();  // Get the first (or default) result

            return childDetails;
        }

        // Lấy toàn bộ danh sách các Child thuộc HouseId cụ thể.
        public async Task<List<Child>> GetChildByHouseIdAsync(string houseId)
        {
            return await _context.Children
                .Where(c => c.HouseId == houseId && (c.IsDeleted == null || c.IsDeleted == false))
                .ToListAsync();
        }

        // Đếm số lượng Child thuộc HouseId cụ thể.
        public async Task<int> CountChildrenByHouseIdAsync(string houseId)
        {
            return await _context.Children
                .Where(child => child.HouseId == houseId && !child.IsDeleted)
                .CountAsync();
        }


        //Dashboard phần tính Active children trong hệ thống
        public async Task<ActiveChildrenStatDTO> GetActiveChildrenStatAsync()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            // Đếm tổng số trẻ em đang active
            var totalActive = await _context.Children
                .Where(c => c.Status == "Active" && !c.IsDeleted)
                .CountAsync();

            // Tính số trẻ em được thêm mới trong tháng này
            var addedThisMonth = await _context.Children
                .Where(c => c.Status == "Active"
                        && !c.IsDeleted
                        && c.CreatedDate >= firstDayOfMonth)
                .CountAsync();

            // Tính số trẻ em bị xóa hoặc chuyển sang inactive trong tháng này
            var removedThisMonth = await _context.Children
                .Where(c => (c.Status != "Active" || c.IsDeleted)
                        && c.ModifiedDate >= firstDayOfMonth)
                .CountAsync();

            // Tính tăng giảm theo tháng
            var netChange = addedThisMonth - removedThisMonth;

            return new ActiveChildrenStatDTO
            {
                TotalActiveChildren = totalActive,
                ChangeThisMonth = netChange
            };
        }

        // Dashboard phần Children For Demographics
        public async Task<IEnumerable<Child>> GetChildrenForDemographics()
        {
            return await _context.Children
                .Where(x => !x.IsDeleted && x.Status == "Active")
                .Select(x => new Child
                {
                    Dob = x.Dob,
                    Gender = x.Gender
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
