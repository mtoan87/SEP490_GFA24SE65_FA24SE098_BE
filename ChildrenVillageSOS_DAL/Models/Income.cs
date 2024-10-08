﻿using ChildrenVillageSOS_DAL.Helpers;
using System;
using System.Collections.Generic;

namespace ChildrenVillageSOS_DAL.Models;

public partial class Income : ISoftDelete
{
    public int Id { get; set; }

    public int? DonationId { get; set; }

    public DateTime Receiveday { get; set; }

    public string? Status { get; set; }

    public string? UserAccountId { get; set; }

    public string? HouseId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Donation? Donation { get; set; }

    public virtual House? House { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual UserAccount? UserAccount { get; set; }
}
