﻿using ChildrenVillageSOS_DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_SERVICE.Interface
{
    public interface IImageService
    {
        Task<IEnumerable<Image>> GetAllImages();
        Task<bool> DeleteImageAsync(string urlImage, string path);
        Task<List<string>> UploadChildImage(List<IFormFile> files, string ChildId);
        Task<List<string>> UploadHouseImage(List<IFormFile> files, string HouseId);
        Task<List<string>> UploadVillageImage(List<IFormFile> files, string VillageId);
        Task<List<string>> UploadEventImage(List<IFormFile> files, int EventId);
        Task<List<string>> UploadUserAccountImage(List<IFormFile> files, string UserAccountId);

    }
}
