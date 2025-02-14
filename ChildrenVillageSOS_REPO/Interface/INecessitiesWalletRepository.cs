﻿using ChildrenVillageSOS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenVillageSOS_REPO.Interface
{
    public interface INecessitiesWalletRepository : IRepositoryGeneric<NecessitiesWallet>
    {
        Task<NecessitiesWallet> GetNecessitiesWalletByUserIdAsync(string userAccountId);
        NecessitiesWallet[] GetNecesWalletsArray();
        Task<decimal> GetWalletBudgetByUserIdAsync(string userAccountId);
    }
}
