using Auth.Data;
using Auth.Entities;
using Microsoft.EntityFrameworkCore;


namespace Auth.Services
{
    public class ApiKeyService
    {
        private readonly AppDbContext _context;

        public ApiKeyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiKey> CreateApiKeyAsync(string owner)
        {
            var apiKey = new ApiKey
            {
                Key = Guid.NewGuid().ToString("N"), // random string
                Owner = owner
            };

            _context.ApiKeys.Add(apiKey);
            await _context.SaveChangesAsync();
            return apiKey;
        }

        public async Task<ApiKey?> ValidateApiKeyAsync(string key)
        {
            return await _context.ApiKeys
                .FirstOrDefaultAsync(x => x.Key == key && x.IsActive);
        }
    }
}


