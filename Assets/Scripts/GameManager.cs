using Mirror;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
        public event EventHandler OnGameStarted;
        public event EventHandler OnCurrentPlayablePlayerTypeChanged;

        public class OnClickedOnGridPositionEventArgs : EventArgs
        {
            public int x;
            public int y;
            public PlayerType playerType;
        }

        public enum PlayerType
        {
            None,
            Cross,
            Circle,
        }

        [SyncVar(hook = nameof(TriggerOnCurrentPlayablePlayerTypeChanged))]
        private PlayerType _currentPlayablePlayerType;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one GameManager Instance!");
            }

            Instance = this;
        }

        [Server]
        public void SetStartingPlayerType(PlayerType playerType)
        {
            _currentPlayablePlayerType = playerType;
        }

        public PlayerType GetCurrentPlayablePlayerType()
        {
            return _currentPlayablePlayerType;
        }
        private void TriggerOnCurrentPlayablePlayerTypeChanged(PlayerType oldPlayerType, PlayerType newPlayerType)
        {
            OnCurrentPlayablePlayerTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        [Server]
        public void StartGame()
        {
            TriggerOnGameStartedRpc();
        }

        [ClientRpc]
        private void TriggerOnGameStartedRpc()
        {
            OnGameStarted?.Invoke(this, EventArgs.Empty);
        }

        [Server]
        public void ClickedOnGridPosition(int x, int y, PlayerType playerType)
        {
            if (playerType != _currentPlayablePlayerType)
                return;

            OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs
            {
                x = x,
                y = y,
                playerType = playerType
            });

            switch (_currentPlayablePlayerType)
            {
                default:
                case PlayerType.Cross:
                    _currentPlayablePlayerType = PlayerType.Circle;
                    break;
                case PlayerType.Circle:
                    _currentPlayablePlayerType = PlayerType.Cross;
                    break;
            }
        }
    }
}