using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ReseauUniversitaire.Hubs;

[Authorize]
public class GroupChatHub : Hub
{
    // Rejoindre la "salle" SignalR du canal pour recevoir ses messages
    public async Task RejoindreCanal(int canalId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"canal-{canalId}");
    }

    public async Task QuitterCanal(int canalId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"canal-{canalId}");
    }
}