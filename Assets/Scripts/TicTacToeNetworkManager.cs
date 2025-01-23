using Mirror;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Utp;
using static Assets.Scripts.GameManager;

namespace Assets.Scripts
{
    public class TicTacToeNetworkManager : RelayNetworkManager
    {
        public bool IsLoggedIn = false;

        /// <summary>
        /// List of players currently connected to the server.
        /// </summary>
        private readonly List<PlayerController> _players = new();

        public async Task UnityLogin()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Logged into Unity, player ID: {AuthenticationService.Instance.PlayerId}");
                IsLoggedIn = true;
            }
            catch (Exception e)
            {
                IsLoggedIn = false;
                Debug.Log(e);
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

            Dictionary<uint, NetworkIdentity> spawnedPlayers = NetworkServer.spawned;

            // Update players list on client disconnect
            foreach (PlayerController player in _players)
            {
                bool playerFound = false;

                foreach (KeyValuePair<uint, NetworkIdentity> kvp in spawnedPlayers)
                {
                    PlayerController comp = kvp.Value.GetComponent<PlayerController>();

                    // Verify the player is still in the match
                    if (comp != null && player == comp)
                    {
                        playerFound = true;
                        break;
                    }
                }

                if (!playerFound)
                {
                    _players.Remove(player);
                    break;
                }
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient client)
        {
            base.OnServerAddPlayer(client);

            PlayerController player = client.identity.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetPlayerType(client.connectionId == 0 ? PlayerType.Cross : PlayerType.Circle);
            }
            else
            {
                Debug.LogError("Player component not found on NetworkIdentity.");
            }

            foreach (KeyValuePair<uint, NetworkIdentity> kvp in NetworkServer.spawned)
            {
                PlayerController comp = kvp.Value.GetComponent<PlayerController>();

                // Add to player list if new
                if (comp != null && !_players.Contains(comp))
                {
                    _players.Add(comp);
                }
            }

            if (NetworkServer.connections.Count != 2)
                return;

            //Start Game
            GameManager.Instance.SetStartingPlayerType(PlayerType.Cross);
            GameManager.Instance.OnServerAddPlayer();
            GameManager.Instance.StartGame();
        }
    }
}