using Mirror;
using UnityEngine;
using static Assets.Scripts.GameManager;

namespace Assets.Scripts
{
    public class PlayerController : NetworkBehaviour
    {
        [SyncVar]
        private PlayerType _playerType;

        public PlayerType GetPlayerType()
        {
            return _playerType;
        }

        public void SetPlayerType(PlayerType playerType)
        {
            if (isServer)
                _playerType = playerType;
        }

        public void NotifyServerOnGridClick(int x, int y)
        {
            if (isLocalPlayer)
            {
                SendGridClickToServerCmd(x, y);
            }
        }

        [Command]
        private void SendGridClickToServerCmd(int x, int y)
        {
            GameManager.Instance.ClickedOnGridPosition(x, y, GetPlayerType());
        }

        [Command]
        public void RematchCmd()
        {
            GameManager.Instance.Rematch();
        }
    }
}
