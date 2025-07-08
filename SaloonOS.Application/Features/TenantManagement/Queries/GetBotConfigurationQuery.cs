// Path: SaloonOS.Application/Features/TenantManagement/Queries/GetBotConfigurationQuery.cs
using MediatR;
using SaloonOS.Application.DTOs;

namespace SaloonOS.Application.Features.TenantManagement.Queries;

/// <summary>
/// Represents a query to get all bootstrap configuration for the bot client.
/// This is typically the first call a bot makes after authentication.
/// </summary>
public record GetBotConfigurationQuery : IRequest<BotConfigurationDto>;