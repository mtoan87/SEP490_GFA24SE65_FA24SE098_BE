﻿using ChildrenVillageSOS_DAL.DTO.HealthReportDTO;
using ChildrenVillageSOS_DAL.DTO.InventoryDTO;
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
    public class HealthReportService : IHealthReportService
    {
        private readonly IHealthReportRepository _healthReportRepository;
        private readonly IImageService _imageService;
        private readonly IImageRepository _imageRepository;

        public HealthReportService(IHealthReportRepository healthReportRepository,
            IImageService imageService,
            IImageRepository imageRepository)
        {
            _healthReportRepository = healthReportRepository;
            _imageService = imageService;
            _imageRepository = imageRepository;
        }

        public async Task<IEnumerable<HealthReport>> GetAllHealthReports()
        {
            return await _healthReportRepository.GetAllAsync();
        }

        public Task<HealthReportResponseDTO[]> GetAllHealthReportIsDeleteAsync()
        {
            return _healthReportRepository.GetAllHealthReportIsDeleteAsync();
        }

        public async Task<IEnumerable<HealthReportResponseDTO>> GetAllHealthReportWithImg()
        {
            // Lấy tất cả các HealthReports chưa bị xóa từ Repository
            var healthReports = await _healthReportRepository.GetAllNotDeletedAsync();

            // Chuyển đổi sang HealthReportResponseDTO
            var healthReportResponseDTOs = healthReports.Select(hr => new HealthReportResponseDTO
            {
                Id = hr.Id,
                ChildId = hr.ChildId,
                NutritionalStatus = hr.NutritionalStatus,
                MedicalHistory = hr.MedicalHistory,
                VaccinationStatus = hr.VaccinationStatus,
                Weight = hr.Weight,
                Height = hr.Height,
                CheckupDate = hr.CheckupDate,
                DoctorName = hr.DoctorName,
                Recommendations = hr.Recommendations,
                HealthStatus = hr.HealthStatus,
                FollowUpDate = hr.FollowUpDate,
                Illnesses = hr.Illnesses,
                Allergies = hr.Allergies,
                HealthCertificate = hr.HealthCertificate,
                Status = hr.Status,
                IsDeleted = hr.IsDeleted,
                CreatedBy = hr.CreatedBy,
                CreatedDate = hr.CreatedDate,
                ModifiedBy = hr.ModifiedBy,
                ModifiedDate = hr.ModifiedDate,
                ImageUrls = hr.Images
                    .Where(img => !img.IsDeleted) // Lấy các ảnh chưa bị xóa
                    .Select(img => img.UrlPath)  // Lấy URL của ảnh
                    .ToArray()
            }).ToArray();

            return healthReportResponseDTOs;
        }

        public async Task<HealthReport> GetHealthReportById(int id)
        {
            var report = await _healthReportRepository.GetByIdAsync(id);
            if (report == null)
            {
                throw new Exception($"HealthReport with ID {id} not found!");
            }
            return report;
        }

        public async Task<HealthReportResponseDTO> GetHealthReportByIdWithImg(int id)
        {
            return _healthReportRepository.GetHealthReportByIdWithImg(id);
        }

        public async Task<HealthReport> CreateHealthReport(CreateHealthReportDTO createReport)
        {
            var existingReport = await _healthReportRepository.FindAsync(r => r.ChildId == createReport.ChildId);

            if (existingReport != null)
            {
                throw new InvalidOperationException($"An health report already exists for ChildId {createReport.ChildId}.");
            }

            var newReport = new HealthReport
            {
                ChildId = createReport.ChildId,
                NutritionalStatus = createReport.NutritionalStatus,
                MedicalHistory = createReport.MedicalHistory,
                VaccinationStatus = createReport.VaccinationStatus,
                Weight = createReport.Weight,
                Height = createReport.Height,
                CheckupDate = createReport.CheckupDate,
                DoctorName = createReport.DoctorName,
                Recommendations = createReport.Recommendations,
                HealthStatus = createReport.HealthStatus,
                FollowUpDate = createReport.FollowUpDate,
                Illnesses = createReport.Illnesses,
                Allergies = createReport.Allergies,
                HealthCertificate = createReport.HealthCertificate,
                Status = createReport.Status,
                CreatedBy = createReport.CreatedBy,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            await _healthReportRepository.AddAsync(newReport);

            // Tải lên danh sách hình ảnh và nhận các URL
            List<string> imageUrls = await _imageService.UploadHealthReportImage(createReport.Img, newReport.Id);

            // Lưu thông tin các ảnh vào bảng Image
            foreach (var url in imageUrls)
            {
                var image = new Image
                {
                    UrlPath = url,
                    HealthReportId = newReport.Id, // Liên kết với HealthReport
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,
                };
                await _imageRepository.AddAsync(image);
            }

            return newReport;
        }

        public async Task<HealthReport> UpdateHealthReport(int id, UpdateHealthReportDTO updateReport)
        {
            var existingReport = await _healthReportRepository.GetByIdAsync(id);
            if (existingReport == null)
            {
                throw new Exception($"HealthReport with ID {id} not found!");
            }

            existingReport.ChildId = updateReport.ChildId;
            existingReport.NutritionalStatus = updateReport.NutritionalStatus;
            existingReport.MedicalHistory = updateReport.MedicalHistory;
            existingReport.VaccinationStatus = updateReport.VaccinationStatus;
            existingReport.Weight = updateReport.Weight;
            existingReport.Height = updateReport.Height;
            existingReport.CheckupDate = updateReport.CheckupDate;
            existingReport.DoctorName = updateReport.DoctorName;
            existingReport.Recommendations = updateReport.Recommendations;
            existingReport.HealthStatus = updateReport.HealthStatus;
            existingReport.FollowUpDate = updateReport.FollowUpDate;
            existingReport.Illnesses = updateReport.Illnesses;
            existingReport.Allergies = updateReport.Allergies;
            existingReport.HealthCertificate = updateReport.HealthCertificate;
            existingReport.Status = updateReport.Status;
            existingReport.ModifiedBy = updateReport.ModifiedBy;
            existingReport.ModifiedDate = DateTime.Now;

            // Lấy danh sách ảnh hiện tại liên kết với báo cáo sức khỏe
            var existingImages = await _imageRepository.GetByHealthReportIdAsync(existingReport.Id);

            // Xóa các ảnh được yêu cầu xóa
            if (updateReport.ImgToDelete != null && updateReport.ImgToDelete.Any())
            {
                foreach (var imageUrlToDelete in updateReport.ImgToDelete)
                {
                    var imageToDelete = existingImages.FirstOrDefault(img => img.UrlPath == imageUrlToDelete);
                    if (imageToDelete != null)
                    {
                        imageToDelete.IsDeleted = true;
                        imageToDelete.ModifiedDate = DateTime.Now;

                        // Cập nhật trạng thái ảnh trong database
                        await _imageRepository.UpdateAsync(imageToDelete);

                        // Xóa ảnh khỏi Cloudinary
                        bool isDeleted = await _imageService.DeleteImageAsync(imageToDelete.UrlPath, "HealthReportImages");
                        if (isDeleted)
                        {
                            await _imageRepository.RemoveAsync(imageToDelete);
                        }
                    }
                }
            }

            // Thêm các ảnh mới nếu có
            if (updateReport.Img != null && updateReport.Img.Any())
            {
                var newImageUrls = await _imageService.UploadHealthReportImage(updateReport.Img, existingReport.Id);
                foreach (var newImageUrl in newImageUrls)
                {
                    var newImage = new Image
                    {
                        UrlPath = newImageUrl,
                        HealthReportId = existingReport.Id,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false,
                    };
                    await _imageRepository.AddAsync(newImage);
                }
            }

            await _healthReportRepository.UpdateAsync(existingReport);
            return existingReport;
        }

        public async Task<HealthReport> DeleteHealthReport(int id)
        {
            var report = await _healthReportRepository.GetByIdAsync(id);
            if (report == null)
            {
                throw new Exception($"Health report with ID {id} not found!");
            }

            await _healthReportRepository.RemoveAsync(report);
            return report;
        }

        public async Task<HealthReport> RestoreHealthReport(int id)
        {
            var report = await _healthReportRepository.GetByIdAsync(id);
            if (report == null)
            {
                throw new Exception($"Health report with ID {id} not found");
            }

            if (report.IsDeleted)
            {
                report.IsDeleted = false;
                await _healthReportRepository.UpdateAsync(report);
            }

            return report;
        }
    }
}
