﻿using ChildrenVillageSOS_DAL.DTO.ChildDTO;
using ChildrenVillageSOS_DAL.DTO.DonationDTO;
using ChildrenVillageSOS_DAL.DTO.EventDTO;
using ChildrenVillageSOS_DAL.Helpers;
using ChildrenVillageSOS_DAL.Models;
using ChildrenVillageSOS_REPO.Implement;
using ChildrenVillageSOS_REPO.Interface;
using ChildrenVillageSOS_SERVICE.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_SERVICE.Implement
{
    public class ChildService : IChildService
    {
        private readonly IChildRepository _childRepository;
        private readonly IDonationRepository _donationRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IImageService _imageService;
        private readonly IImageRepository _imageRepository;
        private readonly IConfiguration _configuration;
        private readonly IDonationService _donationService;
        private readonly IPaymentService _paymentService;
        private readonly IFacilitiesWalletRepository _failitiesWalletRepository;
        private readonly IFoodStuffWalletRepository _foodStuffWalletRepository;
        private readonly INecessitiesWalletRepository _necessitiesWalletRepository;
        private readonly ISystemWalletRepository _systemWalletRepository;
        private readonly IHealthWalletRepository _healthWalletRepository;
        private readonly IIncomeRepository _incomeRepository;


        public ChildService(IChildRepository childRepository, IImageService imageService, IImageRepository imageRepository, IDonationRepository donationRepository, IPaymentRepository paymentRepository, ITransactionRepository transactionRepository, IConfiguration configuration, IDonationService donationService, IFacilitiesWalletRepository facilitiesWalletRepository, IPaymentService paymentService, ISystemWalletRepository systemWalletRepository, INecessitiesWalletRepository necessitiesWalletRepository, IFoodStuffWalletRepository foodStuffWalletRepository, IHealthWalletRepository healthWalletRepository, IIncomeRepository incomeRepository)
        {
            _childRepository = childRepository;
            _imageService = imageService;
            _imageRepository = imageRepository;
            _donationRepository = donationRepository;
            _paymentRepository = paymentRepository;
            _transactionRepository = transactionRepository;
            _configuration = configuration;
            _donationService = donationService;
            _failitiesWalletRepository = facilitiesWalletRepository;
            _paymentService = paymentService;
            _systemWalletRepository = systemWalletRepository;
            _foodStuffWalletRepository = foodStuffWalletRepository;
            _necessitiesWalletRepository = necessitiesWalletRepository;
            _healthWalletRepository = healthWalletRepository;
            _incomeRepository = incomeRepository;

        }

        public async Task<IEnumerable<Child>> GetAllChildren()
        {
            return await _childRepository.GetAllNotDeletedAsync();
        }
        public async Task<IEnumerable<ChildResponseDTO>> GetAllChildrenWithImg()
        {
            var childs = await _childRepository.GetAllNotDeletedAsync();
            
            var childResponseDTOs = childs.Select(x => new ChildResponseDTO
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
                Status = x.Status,
                CreatedDate = x.CreatedDate,
                ImageUrls = x.Images.Where(img => !img.IsDeleted)  
                                     .Select(img => img.UrlPath)  
                                     .ToArray()
            }).ToArray();

            return childResponseDTOs;

        }

        // GET ID
        public async Task<Child> GetChildById(string id)
        {
            return await _childRepository.GetByIdAsync(id);
        }

        public async Task<List<Child>> GetChildByHouseIdAsync(string houseId)
        {
            return await _childRepository.GetChildByHouseIdAsync(houseId);
        }

        public async Task<ChildResponseDTO> GetChildByIdWithImg(string childid)
        {
            return _childRepository.GetChildByIdWithImg(childid);
        }

        public async Task<Child> CreateChild(CreateChildDTO createChild)
        {
            // Lấy toàn bộ danh sách ChildId hiện có
            var allChildIds = await _childRepository.Entities()
                                                    .Select(c => c.Id)
                                                    .ToListAsync();

            // Sử dụng hàm GenerateId từ IdGenerator
            string newChildId = IdGenerator.GenerateId(allChildIds, "C");

            var newChild = new Child
            {
                Id = newChildId,  // Gán ID mới
                ChildName = createChild.ChildName,
                HealthStatus = createChild.HealthStatus,
                HouseId = createChild.HouseId,
                Gender = createChild.Gender,
                Dob = createChild.Dob,
                CreatedDate = DateTime.Now,
                Status = createChild.Status,
                IsDeleted = false
                
            };
            await _childRepository.AddAsync(newChild);

            // Upload danh sách ảnh và nhận về các URL
            List<string> imageUrls = await _imageService.UploadChildImage(createChild.Img, newChild.Id);

            // Lưu thông tin các ảnh vào bảng Image
            foreach (var url in imageUrls)
            {
                var image = new Image
                {
                    UrlPath = url,
                    ChildId = newChild.Id,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,
                };
                await _imageRepository.AddAsync(image);
            }
            return newChild;
        }
        public async Task<string> DonateChild(string id, ChildDonateDTO updateChild)
        {
            // Step 1: Retrieve the event by ID
            var editChild = await _childRepository.GetByIdAsync(id);
            if (editChild == null)
            {
                throw new Exception($"Event with ID {id} not found!");
            }

            // Step 2: Check if new donation will exceed the AmountLimit
            var newTotalAmount = (editChild.CurrentAmount ?? 0) + (updateChild.Amount ?? 0);
            if (newTotalAmount > (editChild.AmountLimit ?? 0))
            {
                throw new InvalidOperationException("Donation amount exceeds the allowed limit.");
            }

            // Step 3: Update the CurrentAmount and ModifiedDate of the event
            editChild.CurrentAmount = newTotalAmount;
            editChild.ModifiedDate = DateTime.Now;
            await _childRepository.UpdateAsync(editChild);

            // Step 4: Create Donation
            var donationDto = new CreateDonationPayment
            {
                UserAccountId = updateChild.UserAccountId,
                DonationType = "Online",
                DateTime = DateTime.Now,
                Amount = updateChild.Amount ?? 0,
                Description = $"Donation for Child: {editChild.ChildName}",
                IsDeleted = false,
                Status = "Pending"
            };

            var donation = await _donationService.CreateDonationPayment(donationDto);

            // Step 5: Create VNPay URL for payment
            var vnp_ReturnUrl = _configuration["VNPay:ReturnUrl"];
            var vnp_Url = _configuration["VNPay:Url"];
            var vnp_TmnCode = _configuration["VNPay:TmnCode"];
            var vnp_HashSecret = _configuration["VNPay:HashSecret"];

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (updateChild.Amount.GetValueOrDefault() * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", "192.168.1.105");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán cho Donation {donation.Id}");
            vnpay.AddRequestData("vnp_OrderType", "donation");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", donation.Id.ToString());
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            // Step 6: Update the first wallet with an ID
            if (editChild.FacilitiesWalletId.HasValue)
            {
                await UpdateFacilitiesWalletBudget(editChild.FacilitiesWalletId.Value, updateChild.Amount ?? 0);
            }
            else if (editChild.FoodStuffWalletId.HasValue)
            {
                await UpdateFoodStuffWalletBudget(editChild.FoodStuffWalletId.Value, updateChild.Amount ?? 0);
            }
            else if (editChild.SystemWalletId.HasValue)
            {
                await UpdateSystemWalletBudget(editChild.SystemWalletId.Value, updateChild.Amount ?? 0);
            }
            else if (editChild.HealthWalletId.HasValue)
            {
                await UpdateHealthWalletBudget(editChild.HealthWalletId.Value, updateChild.Amount ?? 0);
            }
            else if (editChild.NecessitiesWalletId.HasValue)
            {
                await UpdateNecessitiesWalletBudget(editChild.NecessitiesWalletId.Value, updateChild.Amount ?? 0);
            }

            // Step 7: Create Transaction
            var income = new Income
            {
                UserAccountId = updateChild.UserAccountId,
                FacilitiesWalletId = editChild.FacilitiesWalletId,
                FoodStuffWalletId = editChild.FoodStuffWalletId,
                SystemWalletId = editChild.SystemWalletId,
                HealthWalletId = editChild.HealthWalletId,
                NecessitiesWalletId = editChild.NecessitiesWalletId,
                Amount = updateChild.Amount ?? 0,
                Receiveday = DateTime.Now,
                Status = "Completed",
                DonationId = donation.Id
            };
            await _incomeRepository.AddAsync(income);

            // Step 8: Create Payment
            var payment = new Payment
            {
                DonationId = donation.Id,
                Amount = updateChild.Amount ?? 0,
                PaymentMethod = "Banking",
                DateTime = DateTime.Now,
                CreatedDate = DateTime.Now,
                IsDeleted = false,
                Status = "Pending"
            };
            await _paymentRepository.AddAsync(payment);

            return paymentUrl;
        }
        private async Task UpdateFacilitiesWalletBudget(int walletId, decimal amount)
        {
            var wallet = await _failitiesWalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                wallet.Budget += amount;
                await _failitiesWalletRepository.UpdateAsync(wallet);
            }
        }
        private async Task UpdateFoodStuffWalletBudget(int walletId, decimal amount)
        {
            var wallet = await _foodStuffWalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                wallet.Budget += amount;
                await _foodStuffWalletRepository.UpdateAsync(wallet);
            }
        }
        private async Task UpdateNecessitiesWalletBudget(int walletId, decimal amount)
        {
            var wallet = await _necessitiesWalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                wallet.Budget += amount;
                await _necessitiesWalletRepository.UpdateAsync(wallet);
            }
        }
        private async Task UpdateHealthWalletBudget(int walletId, decimal amount)
        {
            var wallet = await _healthWalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                wallet.Budget += amount;
                await _healthWalletRepository.UpdateAsync(wallet);
            }
        }
        private async Task UpdateSystemWalletBudget(int walletId, decimal amount)
        {
            var wallet = await _systemWalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                wallet.Budget += amount;
                await _systemWalletRepository.UpdateAsync(wallet);
            }
        }
        public async Task<Child> UpdateChild(string id, UpdateChildDTO updateChild)
        {
            var existingChild = await _childRepository.GetByIdAsync(id);
            if (existingChild == null)
            {
                throw new Exception($"Child with ID {id} not found!");
            }

            // Cập nhật các thuộc tính cơ bản
            existingChild.ChildName = updateChild.ChildName;
            existingChild.HealthStatus = updateChild.HealthStatus;
            existingChild.HouseId = updateChild.HouseId;
            existingChild.Gender = updateChild.Gender;
            existingChild.Dob = updateChild.Dob;
            existingChild.Status = updateChild.Status;
            existingChild.IsDeleted = updateChild.IsDeleted;
            existingChild.ModifiedDate = DateTime.Now;

            // Nếu có danh sách ảnh được upload trong yêu cầu cập nhật
            if (updateChild.Img != null && updateChild.Img.Any())
            {
                // Lấy danh sách ảnh hiện tại của CHild từ database Image
                var existingImages = await _imageRepository.GetByChildIdAsync(existingChild.Id);

                // Xóa tất cả các ảnh cũ trên Cloudinary và trong cơ sở dữ liệu
                foreach (var existingImage in existingImages)
                {
                    // Xóa ảnh trên Cloudinary
                    bool isDeleted = await _imageService.DeleteImageAsync(existingImage.UrlPath, "ChildImages");
                    if (!isDeleted)
                    {
                        throw new Exception("Không thể xóa ảnh cũ trên Cloudinary");
                    }
                    // Xóa ảnh khỏi database
                    await _imageRepository.RemoveAsync(existingImage);
                }
            }

            // Upload danh sách ảnh mới và lưu thông tin vào database
            List<string> newImageUrls = await _imageService.UploadChildImage(updateChild.Img, existingChild.Id);
            foreach (var newImageUrl in newImageUrls)
            {
                var newImage = new Image
                {
                    UrlPath = newImageUrl,
                    ChildId = existingChild.Id,
                    ModifiedDate = DateTime.Now,
                    IsDeleted = false,
                };
                await _imageRepository.AddAsync(newImage);
            }

            // Lưu thay đổi
            await _childRepository.UpdateAsync(existingChild);
            return existingChild;
        }

    public async Task<Child> DeleteChild(string id)
        {
            var child = await _childRepository.GetByIdAsync(id);
            if (child == null)
            {
                throw new Exception($"Child with ID {id} not found");
            }

            if (child.IsDeleted == true)
            {
                // Hard delete
                await _childRepository.RemoveAsync(child);
            }
            else
            {
                // Soft delete (đặt IsDeleted = true)
                child.IsDeleted = true;
                await _childRepository.UpdateAsync(child);
            }
            return child;
        }

        public async Task<Child> RestoreChild(string id)
        {
            var child = await _childRepository.GetByIdAsync(id);
            if (child == null)
            {
                throw new Exception($"Child with ID {id} not found");
            }

            if (child.IsDeleted == true) // Nếu đã bị soft delete
            {
                child.IsDeleted = false;  // Khôi phục bằng cách đặt lại IsDeleted = false
                await _childRepository.UpdateAsync(child);
            }
            return child;
        }
    }
}
