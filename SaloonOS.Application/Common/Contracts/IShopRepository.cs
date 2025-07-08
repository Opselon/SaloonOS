using SaloonOS.Domain.TenantManagement.Entities;

namespace SaloonOS.Application.Common.Contracts;

public interface IShopRepository : IRepository<Shop>
{
    Task<Shop?> GetByHashedApiKeyAsync(string hashedApiKey);
}