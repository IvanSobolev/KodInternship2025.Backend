using Demo.DAL.Dto;
using Microsoft.AspNetCore.SignalR;

namespace Demo.Hubs;

public class TaskNotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}