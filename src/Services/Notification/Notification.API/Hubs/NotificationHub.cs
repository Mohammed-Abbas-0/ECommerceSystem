using Microsoft.AspNetCore.SignalR;

namespace Notification.API.Hubs;

public class NotificationHub:Hub
{
    public async Task JoinGroup(string customerId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, customerId);
    }

    public async Task LeaveGroup(string customerId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, customerId);
    }
}
