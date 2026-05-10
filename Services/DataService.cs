using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FateDefiner.Models;

namespace FateDefiner.Services
{
    /// <summary>
    /// JSON-based local data service.
    /// Each campaign is stored as its own JSON file under %APPDATA%\FateDefiner\campaigns\.
    /// All IO is async (Task.Run) so the UI thread is never blocked — multithreading technique.
    /// Exception handling wraps every IO operation.
    /// </summary>
    public class DataService : IDataService
    {
        private readonly string _dir;
        private readonly JsonSerializerOptions _opts;

        public string DataDirectory => _dir;

        public DataService()
        {
            _dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FateDefiner", "campaigns");

            _opts = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Exception handling: ensure directory exists
            try { Directory.CreateDirectory(_dir); }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Cannot create data directory at {_dir}: {ex.Message}", ex);
            }
        }

        /// <summary>Loads all campaign files from disk asynchronously.</summary>
        public async Task<List<Campaign>> LoadAllCampaignsAsync()
        {
            var campaigns = new List<Campaign>();

            // Offload IO to thread pool — multithreading technique
            await Task.Run(() =>
            {
                try
                {
                    string[] files = Directory.GetFiles(_dir, "*.json");

                    foreach (string file in files)
                    {
                        try
                        {
                            string json = File.ReadAllText(file);
                            Campaign? c = JsonSerializer.Deserialize<Campaign>(json, _opts);
                            if (c != null) campaigns.Add(c);
                        }
                        catch (JsonException jex)
                        {
                            // Log corrupt file but continue loading others
                            Console.WriteLine($"[DataService] Corrupt JSON in {file}: {jex.Message}");
                        }
                        catch (IOException ioex)
                        {
                            Console.WriteLine($"[DataService] IO error reading {file}: {ioex.Message}");
                        }
                    }

                    // Sort by most recently modified (Sorting technique)
                    campaigns.Sort((a, b) => b.LastModified.CompareTo(a.LastModified));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to load campaigns from {_dir}: {ex.Message}", ex);
                }
            });

            return campaigns;
        }

        /// <summary>Serializes and saves a single campaign file asynchronously.</summary>
        public async Task SaveCampaignAsync(Campaign campaign)
        {
            if (campaign == null) throw new ArgumentNullException(nameof(campaign));
            campaign.LastModified = DateTime.Now;

            string path = Path.Combine(_dir, $"{campaign.Id}.json");

            await Task.Run(() =>
            {
                try
                {
                    string json = JsonSerializer.Serialize(campaign, _opts);
                    File.WriteAllText(path, json);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to save campaign '{campaign.Name}': {ex.Message}", ex);
                }
            });
        }

        /// <summary>Deletes a campaign JSON file asynchronously.</summary>
        public async Task DeleteCampaignAsync(string campaignId)
        {
            if (string.IsNullOrWhiteSpace(campaignId))
                throw new ArgumentException("Campaign ID cannot be empty.", nameof(campaignId));

            string path = Path.Combine(_dir, $"{campaignId}.json");

            await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to delete campaign {campaignId}: {ex.Message}", ex);
                }
            });
        }
    }
}
