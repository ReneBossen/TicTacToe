using Mirror;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : NetworkBehaviour
    {
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
            Debug.Log($"Server received grid click at {x}, {y}");

            GameManager.Instance.ClickedOnGridPosition(x, y);
        }
    }
}
