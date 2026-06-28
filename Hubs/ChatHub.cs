using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Concurrent;

namespace ReseauUniversitaire.Hubs;

[Authorize]
public class ChatHub : Hub
{
    // Garde en mémoire qui est connecté : userId → liste de connectionIds
    private static readonly ConcurrentDictionary<int, HashSet<string>> _utilisateursConnectes = new();

    private int GetUserId()
    {
        var id = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(id!);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        _utilisateursConnectes.AddOrUpdate(
            userId,
            new HashSet<string> { Context.ConnectionId },
            (key, set) => { set.Add(Context.ConnectionId); return set; }
        );

        // Informe tout le monde que cet utilisateur est en ligne
        await Clients.All.SendAsync("UtilisateurEnLigne", userId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();

        if (_utilisateursConnectes.TryGetValue(userId, out var connections))
        {
            connections.Remove(Context.ConnectionId);
            if (connections.Count == 0)
            {
                _utilisateursConnectes.TryRemove(userId, out _);
                // Informe tout le monde que cet utilisateur est hors ligne
                await Clients.All.SendAsync("UtilisateurHorsLigne", userId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Envoyer un message à un utilisateur précis (en plus de l'enregistrer en BDD via le Controller)
    public async Task EnvoyerMessagePrive(int destinataireId, object messageDto)
    {
        if (_utilisateursConnectes.TryGetValue(destinataireId, out var connections))
        {
            foreach (var connectionId in connections)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", messageDto);
            }
        }
    }

    // Indicateur "est en train d'écrire..."
    public async Task EnTrainDecrire(int destinataireId)
    {
        var userId = GetUserId();

        if (_utilisateursConnectes.TryGetValue(destinataireId, out var connections))
        {
            foreach (var connectionId in connections)
            {
                await Clients.Client(connectionId).SendAsync("UtilisateurEcrit", userId);
            }
        }
    }

    // Vérifier si un utilisateur est en ligne
    public static bool EstEnLigne(int userId)
    {
        return _utilisateursConnectes.ContainsKey(userId);
    }
}