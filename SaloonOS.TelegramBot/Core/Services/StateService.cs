// Path: Core/Services/StateService.cs
using System.Collections.Concurrent;
using SaloonOS.TelegramBot.Features.Common;

namespace SaloonOS.TelegramBot.Core.Services;

/// <summary>
/// A singleton service that provides simple, in-memory state management for user conversations.
/// In a production environment with multiple bot instances, this should be replaced with a distributed
/// cache like Redis to ensure state is shared.
/// </summary>
public class StateService
{
    private readonly ConcurrentDictionary<long, UserState> _userStates = new();

    public UserState GetOrCreateState(long telegramUserId)
    {
        return _userStates.GetOrAdd(telegramUserId, _ => new UserState());
    }

    public void ResetState(long telegramUserId)
    {
        _userStates.TryRemove(telegramUserId, out _);
    }
}