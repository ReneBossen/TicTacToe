using Mirror;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;

        public class OnClickedOnGridPositionEventArgs : EventArgs
        {
            public int x;
            public int y;
        }

        public enum PlayerType
        {
            None,
            Cross,
            Circle,
        }

        private PlayerType localPlayerType;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one GameManager Instance!");
            }

            Instance = this;
        }

        public void ClickedOnGridPosition(int x, int y)
        {
            Debug.Log($"ClickedOnGridPosition {x}, {y}");
            OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs
            {
                x = x,
                y = y
            });
        }
    }
}