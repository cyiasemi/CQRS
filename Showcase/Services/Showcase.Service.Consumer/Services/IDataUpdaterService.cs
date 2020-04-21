using System;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Services
{
    public interface IDataUpdaterService
    {
        void StartMonitoring();
        Task<(DateTimeOffset LastUpdate, DateTimeOffset NextUpdate)> Update();
    }
}