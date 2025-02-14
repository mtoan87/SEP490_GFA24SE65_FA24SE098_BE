﻿using ChildrenVillageSOS_DAL.DTO.ChildDTO;
using ChildrenVillageSOS_DAL.DTO.House;
using ChildrenVillageSOS_DAL.DTO.HouseDTO;
using ChildrenVillageSOS_DAL.DTO.VillageDTO;
using ChildrenVillageSOS_DAL.Helpers;
using ChildrenVillageSOS_DAL.Models;
using ChildrenVillageSOS_REPO.Implement;
using ChildrenVillageSOS_REPO.Interface;
using ChildrenVillageSOS_SERVICE.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_SERVICE.Implement
{
    public class HouseService : IHouseService
    {
        private readonly IHouseRepository _houseRepository;
        private readonly IImageService _imageService;
        private readonly IImageRepository _imageRepository;
        private readonly IChildRepository _childRepository;
        private readonly IVillageRepository _villageRepository;


        public HouseService(IHouseRepository houseRepository, IImageService imageService, IImageRepository imageRepository, IChildRepository childRepository, IVillageRepository villageRepository)
        {
            _houseRepository = houseRepository;
            _imageRepository = imageRepository;
            _imageService = imageService;
            _childRepository = childRepository;
            _villageRepository = villageRepository;
        }

        public  DataTable getHouse()
        {
            return _houseRepository.getHouse();
        }
        public async Task<IEnumerable<House>> GetAllHouses()
        {
            // Chỉ lấy những nhà chưa bị soft delete (IsDeleted = false)
            return await _houseRepository.GetAllNotDeletedAsync();
        }

        public async Task<IEnumerable<HouseResponseDTO>> GetAllHousesWithImg()
        {
            var houses = await _houseRepository.GetAllNotDeletedAsync();

            var houseIds = houses.Select(h => h.Id).ToList();

            var housesWithRelations = await _houseRepository.GetHousesWithRelationsAsync(houseIds);

            var houseResponseDTOs = new List<HouseResponseDTO>();

            foreach (var house in housesWithRelations)
            {
                int currentMembers = await _childRepository.CountChildrenByHouseIdAsync(house.Id);

                // Tạo HouseResponseDTO với tất cả thông tin
                houseResponseDTOs.Add(new HouseResponseDTO
                {
                    Id = house.Id,
                    HouseName = house.HouseName,
                    HouseNumber = house.HouseNumber,
                    Location = house.Location,
                    Description = house.Description,
                    HouseMember = house.HouseMember,
                    CurrentMembers = currentMembers,
                    //HouseOwner = house.HouseOwner,
                    Status = house.Status,
                    UserAccountId = house.UserAccountId,
                    UserName = house.UserAccount?.UserName ?? "Unknown",
                    VillageId = house.VillageId,
                    VillageName = house.Village?.VillageName ?? "Unknown",
                    FoundationDate = house.FoundationDate,
                    LastInspectionDate = house.LastInspectionDate,
                    MaintenanceStatus = house.MaintenanceStatus,
                    CreatedBy = house.CreatedBy,
                    ModifiedBy = house.ModifiedBy,
                    IsDeleted = house.IsDeleted,
                    CreatedDate = house.CreatedDate,
                    ModifiedDate = house.ModifiedDate,
                    ImageUrls = house.Images.Where(img => !img.IsDeleted).Select(img => img.UrlPath).ToArray() // Lấy danh sách ảnh
                });
            }

            return houseResponseDTOs.ToArray();
        }

        public async Task<IEnumerable<HouseResponseDTO>> GetHousesByRoleWithImg(string userId, string role)
        {
            // Lấy danh sách các nhà chưa bị xóa
            var houses = await _houseRepository.GetAllNotDeletedAsync();

            // Khởi tạo danh sách các house sẽ trả về
            var houseResponseDTOs = new List<HouseResponseDTO>();

            // Kiểm tra vai trò người dùng và xử lý theo từng trường hợp
            if (role == "Admin")
            {
                // Nếu là Admin, trả về toàn bộ danh sách house
                var housesWithRelations = await _houseRepository.GetHousesWithRelationsAsync(houses.Select(h => h.Id).ToList());
                houseResponseDTOs = await GetHouseDTOs(housesWithRelations);
            }
            else if (role == "Director")
            {
                // Nếu là Director, lọc theo VillageId
                var village = await _villageRepository.GetVillageByUserAccountIdAsync(userId);
                if (village != null)
                {
                    var housesInVillage = houses.Where(h => h.VillageId == village.Id).ToList();
                    var housesWithRelations = await _houseRepository.GetHousesWithRelationsAsync(housesInVillage.Select(h => h.Id).ToList());
                    houseResponseDTOs = await GetHouseDTOs(housesWithRelations);
                }
            }
            else if (role == "HouseMother")
            {
                // Nếu là HouseMother, chỉ xem được house mà họ quản lý
                var house = await _houseRepository.GetHouseByUserAccountIdAsync(userId);
                if (house != null)
                {
                    var housesWithRelations = await _houseRepository.GetHousesWithRelationsAsync(new List<string> { house.Id });
                    houseResponseDTOs = await GetHouseDTOs(housesWithRelations);
                }
            }

            // Trả về danh sách các house đã được lọc và tạo DTO
            return houseResponseDTOs.ToArray();
        }

        private async Task<List<HouseResponseDTO>> GetHouseDTOs(IEnumerable<House> houses)
        {
            var houseResponseDTOs = new List<HouseResponseDTO>();

            foreach (var house in houses)
            {
                int currentMembers = await _childRepository.CountChildrenByHouseIdAsync(house.Id);

                // Tạo HouseResponseDTO với tất cả thông tin
                houseResponseDTOs.Add(new HouseResponseDTO
                {
                    Id = house.Id,
                    HouseName = house.HouseName,
                    HouseNumber = house.HouseNumber,
                    Location = house.Location,
                    Description = house.Description,
                    HouseMember = house.HouseMember,
                    CurrentMembers = currentMembers,
                    Status = house.Status,
                    UserAccountId = house.UserAccountId,
                    UserName = house.UserAccount?.UserName ?? "Unknown",
                    VillageId = house.VillageId,
                    VillageName = house.Village?.VillageName ?? "Unknown",
                    FoundationDate = house.FoundationDate,
                    LastInspectionDate = house.LastInspectionDate,
                    MaintenanceStatus = house.MaintenanceStatus,
                    CreatedBy = house.CreatedBy,
                    ModifiedBy = house.ModifiedBy,
                    IsDeleted = house.IsDeleted,
                    CreatedDate = house.CreatedDate,
                    ModifiedDate = house.ModifiedDate,
                    ImageUrls = house.Images.Where(img => !img.IsDeleted).Select(img => img.UrlPath).ToArray() // Lấy danh sách ảnh
                });
            }

            return houseResponseDTOs;
        }

        public async Task<House> GetHouseById(string id)
        {
            return await _houseRepository.GetByIdAsync(id);
        }

        public async Task<HouseDetailsDTO> GetHouseDetails(string houseId)
        {
            return await _houseRepository.GetHouseDetails(houseId);
        }

        public async Task<HouseResponseDTO> GetHouseByIdWithImg(string houseId)
        {
            return _houseRepository.GetHouseByIdWithImg(houseId);
        }
        public Task<HouseResponseDTO[]> SearchHousesAsync(string searchTerm)
        {
            return _houseRepository.SearchHousesAsync(searchTerm);
        }
        public HouseNameDTO[] GetHouseByUserIdWithImg(string userId)
        {
            return _houseRepository.GetHouseByUserIdWithImg(userId);
        }

        public async Task<House> CreateHouse(CreateHouseDTO createHouse)
        {
            // Lấy toàn bộ danh sách HouseId hiện có
            var allHouseIds = await _houseRepository.Entities()
                                                    .Select(h => h.Id)
                                                    .ToListAsync();

            // Sử dụng hàm GenerateId từ IdGenerator
            string newHouseId = IdGenerator.GenerateId(allHouseIds, "H");

            var newHouse = new House
            {
                Id = newHouseId,
                HouseName = createHouse.HouseName,
                HouseNumber = createHouse.HouseNumber,
                Location = createHouse.Location,
                Description = createHouse.Description,
                HouseMember = createHouse.HouseMember,
                CurrentMembers = 0,
                //HouseOwner = createHouse.HouseOwner,
                Status = "Active", // Nếu không được cung cấp, mặc định là "Active"
                UserAccountId = createHouse.UserAccountId,
                VillageId = createHouse.VillageId,
              
                FoundationDate = createHouse.FoundationDate,
                LastInspectionDate = createHouse.LastInspectionDate,
                MaintenanceStatus = createHouse.MaintenanceStatus,
                IsDeleted = false,
                CreatedBy = createHouse.CreatedBy,              
                CreatedDate = DateTime.Now
            };
            await _houseRepository.AddAsync(newHouse);

            // Upload danh sách ảnh và nhận về các URL
            List<string> imageUrls = await _imageService.UploadHouseImage(createHouse.Img, newHouse.Id);

            // Lưu thông tin các ảnh vào bảng Image
            foreach (var url in imageUrls)
            {
                var image = new Image
                {
                    UrlPath = url,
                    HouseId = newHouse.Id,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,
                };
                await _imageRepository.AddAsync(image);
            }
            return newHouse;
        }

        public async Task<HouseResponseDTO[]> getHouseByVillageId(string villageId)
        {
            return await _houseRepository.GetHouseByVillageIdAsync(villageId); // Await the async call
        }
        public  House? GetHouseByUserAccountId(string userAccountId)
        {
            return _houseRepository.GetHouseByUserAccountId(userAccountId);
        }
        public async Task<string?> GetUserAccountIdByHouseId(string houseId)
        {
            return await _houseRepository.GetUserAccountIdByHouseId(houseId);
        }
        public async Task<House> UpdateHouse(string id, UpdateHouseDTO updateHouse)
        {
            var existingHouse = await _houseRepository.GetByIdAsync(id);
            if (existingHouse == null)
            {
                throw new Exception($"House with ID {id} not found!");
            }

            existingHouse.HouseName = updateHouse.HouseName;
            existingHouse.HouseNumber = updateHouse.HouseNumber;
            existingHouse.Location = updateHouse.Location;
            existingHouse.Description = updateHouse.Description;
            existingHouse.HouseMember = updateHouse.HouseMember;
            existingHouse.CurrentMembers = updateHouse.CurrentMembers;
            //existingHouse.HouseOwner = updateHouse.HouseOwner;
            existingHouse.Status = updateHouse.Status;
            existingHouse.UserAccountId = updateHouse.UserAccountId;
            existingHouse.VillageId = updateHouse.VillageId;
           
            existingHouse.FoundationDate = updateHouse.FoundationDate;
            existingHouse.LastInspectionDate = updateHouse.LastInspectionDate;
            existingHouse.MaintenanceStatus = updateHouse.MaintenanceStatus;
            existingHouse.IsDeleted = false;
            existingHouse.ModifiedBy = updateHouse.ModifiedBy;
            existingHouse.ModifiedDate = DateTime.Now;

            var existingImages = await _imageRepository.GetByHouseIdAsync(existingHouse.Id);

            // Xóa các ảnh được yêu cầu xóa
            if (updateHouse.ImgToDelete != null && updateHouse.ImgToDelete.Any())
            {
                foreach (var imageIdToDelete in updateHouse.ImgToDelete)
                {
                    var imageToDelete = existingImages.FirstOrDefault(img => img.UrlPath == imageIdToDelete);
                    if (imageToDelete != null)
                    {
                        imageToDelete.IsDeleted = true;
                        imageToDelete.ModifiedDate = DateTime.Now;

                        // Cập nhật trạng thái ảnh trong database
                        await _imageRepository.UpdateAsync(imageToDelete);

                        // Xóa ảnh khỏi Cloudinary
                        bool isDeleted = await _imageService.DeleteImageAsync(imageToDelete.UrlPath, "HouseImages");
                        if (isDeleted)
                        {
                            await _imageRepository.RemoveAsync(imageToDelete);
                        }
                    }
                }
            }

            // Thêm các ảnh mới nếu có
            if (updateHouse.Img != null && updateHouse.Img.Any())
            {
                var newImageUrls = await _imageService.UploadHouseImage(updateHouse.Img, existingHouse.Id);
                foreach (var newImageUrl in newImageUrls)
                {
                    var newImage = new Image
                    {
                        UrlPath = newImageUrl,
                        HouseId = existingHouse.Id,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = false,
                    };
                    await _imageRepository.AddAsync(newImage);
                }
            }

            // Lưu thông tin cập nhật
            await _houseRepository.UpdateAsync(existingHouse);
            return existingHouse;
        }

        public async Task<House> DeleteHouse(string id)
        {
            var house = await _houseRepository.GetByIdAsync(id);
            if (house == null)
            {
                throw new Exception($"House with ID {id} not found!");
            }

            if (house.IsDeleted == true)
            {
                // Hard delete nếu IsDeleted = true
                await _houseRepository.RemoveAsync(house);
            }
            else
            {
                // Soft delete (IsDeleted = true)
                house.IsDeleted = true;
                await _houseRepository.UpdateAsync(house);
            }
            return house;
        }

        public async Task<House> RestoreHouse(string id)
        {
            var house = await _houseRepository.GetByIdAsync(id);
            if (house == null)
            {
                throw new Exception($"House with ID {id} not found!");
            }

            if (house.IsDeleted == true) // Nếu đã bị soft delete
            {
                house.IsDeleted = false;  // Khôi phục bằng cách đặt lại IsDeleted = false
                await _houseRepository.UpdateAsync(house);
            }
            return house;
        }

        //public Task<HouseResponseDTO[]> GetAllHouseAsync()
        //{
        //    return _houseRepository.GetAllHouseAsync();
        //}

        public Task<HouseResponseDTO[]> GetAllHouseIsDeleteAsync()
        {
            return _houseRepository.GetAllHouseIsDeleteAsync();
        }
        public async Task<string> GetHouseNameByIdAsync(string houseId)
        {
            return await _houseRepository.GetHouseNameByIdAsync(houseId);
        }

        public async Task<List<House>> SearchHouses(SearchHouseDTO searchHouseDTO)
        {
            return await _houseRepository.SearchHouses(searchHouseDTO);
        }
    }
}
