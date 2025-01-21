using Mirror;
using System;
using UnityEngine;
using static Assets.Scripts.GameManager;

namespace Assets.Scripts
{
    public class TicTacToeNetworkManager : NetworkManager
    {
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

            if (NetworkServer.connections.Count != 2)
                return;

            //Start Game
            GameManager.Instance.SetStartingPlayerType(PlayerType.Cross);
            GameManager.Instance.OnServerAddPlayer();
            GameManager.Instance.StartGame();
        }
    }
}