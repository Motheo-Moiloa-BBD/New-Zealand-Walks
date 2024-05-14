using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRegionRepository Regions { get; }
        IWalkRepository Walks { get; }
        ITokenRepository Tokens { get; }
        Task<int> Save();
    }
}
