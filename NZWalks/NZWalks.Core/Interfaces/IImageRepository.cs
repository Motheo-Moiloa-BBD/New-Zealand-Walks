using NZWalks.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Core.Interfaces
{
   public interface IImageRepository : IGenericRepository<Image>
    {
        //Add methods that are specific to the Walk Entity
    }
}
