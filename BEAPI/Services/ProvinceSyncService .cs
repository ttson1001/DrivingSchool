using BEAPI.Dtos.location;
using BEAPI.Dtos.Location;
using BEAPI.Entities;
using BEAPI.Repositories;
using BEAPI.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BEAPI.Services
{
    public class ProvinceSyncService : IProvinceSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly IRepository<Ward> _wardRepo;
        private readonly IRepository<Province> _provinceRepo;

        private const string ApiUrl = "https://tinhthanhpho.com/api/v1/new-provinces?limit=999&page=1";
        private const string BaseUrl = "https://tinhthanhpho.com/api/v1";
        private const string ApiKey = "hvn_n4IsFUf8g2iSqjAPIfD9hvoxMlFgY1ms";

        public ProvinceSyncService(HttpClient httpClient, IRepository<Ward> wardRepo, IRepository<Province> provinceRepo)
        {
            _httpClient = httpClient;
            _provinceRepo = provinceRepo;
            _wardRepo = wardRepo;
        }

        public async Task SyncProvincesAsync()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            var response = await _httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var data = System.Text.Json.JsonSerializer.Deserialize<ProvinceApiResponse>(json,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (data?.Data == null || !data.Data.Any())
                return;

            var allProvinces = _provinceRepo.Get().ToList();
            if (allProvinces.Any())
            {
                _provinceRepo.DeleteRange(allProvinces);
            }

            var newProvinces = data.Data.Select(p => new Province
            {
                Code = p.Code,
                Name = p.Name,
                Type = p.Type
            }).ToList();

            await _provinceRepo.AddRangeAsync(newProvinces);
            await _provinceRepo.SaveChangesAsync();
        }

        public async Task SyncAllWardsAsync()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            var provinces = _provinceRepo.Get().ToList();
            if (!provinces.Any()) return;

            // Xóa hết dữ liệu xã/phường cũ
            var allWards = _wardRepo.Get().ToList();
            if (allWards.Any()) _wardRepo.DeleteRange(allWards);

            foreach (var province in provinces)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{BaseUrl}/new-provinces/{province.Code}/wards?limit=999&page=1");;
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();

                    var data = JsonSerializer.Deserialize<WardApiResponseDto>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (data?.Data == null || !data.Data.Any())
                        continue;

                    var wards = data.Data.Select(w => new Ward
                    {
                        Code = w.Code,
                        Name = w.Name,
                        Type = w.Type,
                        ProvinceCode = province.Code
                    }).ToList();

                    await _wardRepo.AddRangeAsync(wards);
                    Console.WriteLine($"Synced {wards.Count} wards for {province.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi sync {province.Name}: {ex.Message}");
                }
            }

            await _wardRepo.SaveChangesAsync();
        }
        public async Task<List<ProvinceExternalDto>> GetAllProvincesAsync()
        {
            return await _provinceRepo.Get()
                .Select(p => new ProvinceExternalDto
                {
                    Code = p.Code,
                    Name = p.Name,
                    Type = p.Type
                })
                .ToListAsync();
        }

        public async Task<List<WardDto>> GetWardsByProvinceCodeAsync(string provinceCode)
        {
            return await _wardRepo.Get()
                .Where(w => w.ProvinceCode == provinceCode)
                .Select(w => new WardDto
                {
                    Code = w.Code,
                    Name = w.Name,
                    Type = w.Type,
                    ProvinceCode = w.ProvinceCode
                })
                .ToListAsync();
        }
    }

}

