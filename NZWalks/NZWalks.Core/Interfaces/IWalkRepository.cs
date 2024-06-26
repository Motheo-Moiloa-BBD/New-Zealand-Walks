﻿using Microsoft.AspNetCore.Mvc;
using NZWalks.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Core.Interfaces
{
    public interface IWalkRepository : IGenericRepository<Walk>
    {
        //Add methods that are specific to the Walk Entity
        Task<Walk> GetWalkByIdAsync(Guid id);
        Task<IEnumerable<Walk>> GetAllWalksAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, string? sortOrder = null, int? pageNumber = 1, int? pageSize = 5);
    }
}
