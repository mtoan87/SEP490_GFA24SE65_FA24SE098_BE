﻿using ChildrenVillageSOS_DAL.DTO.PaymentDTO;
using ChildrenVillageSOS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_SERVICE.Interface
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllPayments();
        Task<Payment> GetPaymentById(int id);
        //Task<Payment> CreatePayment(CreatePaymentDTO createPayment);
        Task<Payment> UpdatePayment(int id, UpdatePaymentDTO updatePayment);
        Task<Payment> DeletePayment(int id);
        Task<string> CreatePayment(PaymentRequest paymentRequest);
        Task<Payment> GetPaymentByDonationIdAsync(int donationId);
        Task<string> CreateFacilitiesWalletPayment(PaymentRequest paymentRequest);
        Task<string> CreateFoodStuffWalletPayment(PaymentRequest paymentRequest);
        Task<string> CreateHealthWalletPayment(PaymentRequest paymentRequest);
        Task<string> CreateNecesstiesWalletPayment(PaymentRequest paymentRequest);
        Task<string> CreateSystemWalletPayment(PaymentRequest paymentRequest);

        Task<Payment> SoftDelete(int id);
    }
}
