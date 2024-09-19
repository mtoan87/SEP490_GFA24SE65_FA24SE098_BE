﻿using ChildrenVillageSOS_DAL.Helpers;
using System;
using System.Collections.Generic;

namespace ChildrenVillageSOS_DAL.Models;

public partial class Village : ISoftDelete
{
    public string Id { get; set; }

    public string VillageName { get; set; }

    public string Location { get; set; }

    public string Description { get; set; }

    public string Status { get; set; }

    public string UserAccountId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<House> Houses { get; set; } = new List<House>();

    public virtual UserAccount UserAccount { get; set; }
}
