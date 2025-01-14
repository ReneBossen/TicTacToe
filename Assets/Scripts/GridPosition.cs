using Mirror;
using UnityEngine;

namespace Assets.Scripts
{
    public class GridPosition : MonoBehaviour
    {
        [SerializeField] private int _x;
        [SerializeField] private int _y;

        private void OnMouseDown()
        {
            if (!NetworkClient.active)
                return;

            PlayerController player = NetworkClient.localPlayer.GetComponent<PlayerController>();

            if (player != null)
                player.NotifyServerOnGridClick(_x, _y);
        }
    }
}