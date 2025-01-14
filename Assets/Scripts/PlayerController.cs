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
                CmdSendGridClickToServer(x, y);
            }
        }

        [Command]
        private void CmdSendGridClickToServer(int x, int y)
        {
            GameManager.Instance.ClickedOnGridPosition(x, y, GetPlayerType());
        }
    }
}
