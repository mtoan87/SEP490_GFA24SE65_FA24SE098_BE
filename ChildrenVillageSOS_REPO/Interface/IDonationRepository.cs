﻿using ChildrenVillageSOS_DAL.DTO.DonationDTO;
using ChildrenVillageSOS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_REPO.Interface
{
    public interface IDonationRepository : IRepositoryGeneric<Donation>
    {
        Task<List<Donation>> GetDonationsByUserIdAsync(string userId);
        Task<List<DonationResponseDTO>> GetDonationsByUserId(string userId);
        Task<List<Village>> GetDonatedVillageByUserId(string userAccountId);
        FormatDonationResponseDTO[] GetDonationArray();
    }
}
