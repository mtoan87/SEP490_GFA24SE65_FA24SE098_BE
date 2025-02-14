﻿using ChildrenVillageSOS_DAL.DTO.TransferRequestDTO;
using ChildrenVillageSOS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_SERVICE.Interface
{
    public interface ITransferRequestService
    {
        Task<TransferRequest> UpdateTransferRequest(int id, UpdateTransferRequestDTO dto, string currentUserId);
        Task<TransferRequest> CreateTransferRequest(CreateTransferRequestDTO dto);
        //Task<TransferRequest> UpdateTransferRequest(int id, UpdateTransferRequestDTO dto, string currentUserId);
        Task<TransferRequest> UpdateTransferRequest(int id, UpdateTransferRequestDTO dto);
        Task<TransferRequest> GetTransferRequestById(int id);
        Task<IEnumerable<TransferRequest>> GetAllTransferRequests();
        Task<IEnumerable<TransferRequest>> GetTransferRequestsByHouse(string houseId);
        Task<IEnumerable<TransferRequest>> GetAllTransferRequestsWithDetails();
        Task<TransferRequest> DeleteTransferRequest(int id);
        Task<TransferRequest> RestoreTransferRequest(int id);
        Task<List<TransferRequest>> SearchTransferRequests(SearchTransferRequestDTO searchTransferRequestDTO);
    }
}
