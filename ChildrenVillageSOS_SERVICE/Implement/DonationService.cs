﻿//using ChildrenVillageSOS_DAL.DTO.DonationDTO;
//using ChildrenVillageSOS_DAL.Models;
//using ChildrenVillageSOS_REPO.Implement;
//using ChildrenVillageSOS_REPO.Interface;
//using ChildrenVillageSOS_SERVICE.Interface;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ChildrenVillageSOS_SERVICE.Implement
//{
//    public class DonationService : IDonationService
//    {
//        private readonly IDonationRepository _donationRepository;
//        public DonationService(IDonationRepository donationRepository)
//        {
//            _donationRepository = donationRepository;
//        }
//        public async Task<Donation> CreateDonation(CreateDonationDTO createDonation)
//        {
//            var donation = new Donation
//            {
//                UserAccountId = createDonation.UserAccountId,
//                IsDeleted = createDonation.IsDelete,
//            };
//            await _donationRepository.AddAsync(donation);
//            return donation;
//        }

//        public async Task<Donation> DeleteDonation(int id)
//        {
//            var donation = await _donationRepository.GetByIdAsync(id);
//            if (donation == null)
//            {
//                throw new Exception($"Donation with ID{id} is not found");
//            }
//            await _donationRepository.RemoveAsync(donation);
//            return donation;
//        }

//        public async Task<IEnumerable<Donation>> GetAllDonations()
//        {
//            return await _donationRepository.GetAllAsync();
//        }

//        public async Task<Donation> GetDonationById(int id)
//        {
//            return await _donationRepository.GetByIdAsync(id);
//        }

//        public async Task<Donation> UpdateDonation(int id, UpdateDonationDTO updateDonation)
//        {
//            var donation = await _donationRepository.GetByIdAsync(id);
//            if (donation == null)
//            {
//                throw new Exception($"Donation with ID{id} is not found");
//            }
//            donation.UserAccountId = updateDonation.UserAccountId;
//            donation.IsDeleted = updateDonation.IsDelete;
//            await _donationRepository.UpdateAsync(donation);
//            return donation;
//        }
//    }
//}
