using System.Collections.Generic;
using System.Threading.Tasks;
using FateDefiner.Models;

namespace FateDefiner.Services
{
    /// <summary>
    /// Abstraction for campaign persistence.  Swap for SQLite or cloud storage without
    /// touching the ViewModels (MVC/MVVM separation principle).
    /// </summary>
    public interface IDataService
    {
        Task<List<Campaign>> LoadAllCampaignsAsync();
        Task SaveCampaignAsync(Campaign campaign);
        Task DeleteCampaignAsync(string campaignId);
        string DataDirectory { get; }
    }
}
