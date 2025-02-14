﻿using ChildrenVillageSOS_DAL.DTO.VillageDTO;
using ChildrenVillageSOS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_SERVICE.Interface
{
    public interface IVillageService
    {
        DataTable getVillage();
        Task<IEnumerable<Village>> GetAllVillage();
        //Task<IEnumerable<VillageResponseDTO>> GetAllVillageWithImg();
        Task<Village> GetVillageById(string villageId);
        Task<VillageResponseDTO[]> GetVillageByEventIDAsync(int eventId);
        Task<VillageResponseDTO[]> SearchVillagesAsync(string searchTerm);
        VillageNameDTO[] GetVillageByUserIdWithImg(string userId);
        Task<VillageResponseDTO[]> GetVillageByIdWithImg(string villageId);
        Task<Village> CreateVillage(CreateVillageDTO createVillage);
        Task<Village> UpdateVillage(string villageId, UpdateVillageDTO updateVillage);
        Task<Village> DeleteVillage(string villageId);
        Task<Village> RestoreVillage(string villageId);
        List<Village> GetVillagesDonatedByUser(string userAccountId);
        Task<VillageResponseDTO[]> GetAllVillageIsDelete();
        Task<VillageDetailsDTO> GetVillageDetails(string villageId);
        Task<List<Village>> SearchVillages(SearchVillageDTO searchVillageDTO);
        Task<IEnumerable<VillageResponseDTO>> GetVillagesByRoleWithImg(string userId, string role);
    }
}
