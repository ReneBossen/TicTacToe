using Mirror;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
        public event EventHandler OnGameStarted;
        public event EventHandler OnCurrentPlayablePlayerTypeChanged;
        public event EventHandler<OnGameWinEventArgs> OnGameWin;

        public class OnGameWinEventArgs : EventArgs
        {
            public Line line;
        }

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

        public enum Orientation
        {
            Horizontal,
            Vertical,
            DiagonalA,
            DiagonalB,
        }

        public struct Line
        {
            public List<Vector2Int> gridVector2IntList;
            public Vector2Int centerGridPosition;
            public Orientation orientation;
        }

        [SyncVar(hook = nameof(TriggerOnCurrentPlayablePlayerTypeChanged))]
        private PlayerType _currentPlayablePlayerType;

        private PlayerType[,] _playerTypeArray;
        private List<Line> lineList;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one GameManager Instance!");
            }

            Instance = this;

            _playerTypeArray = new PlayerType[3, 3];

            lineList = new List<Line>
            {
                //Horizontal
                new() {gridVector2IntList = new List<Vector2Int> { new(0, 0), new(1, 0), new(2, 0) }, centerGridPosition = new Vector2Int(1,0), orientation = Orientation.Horizontal},
                new() {gridVector2IntList = new List<Vector2Int> { new(0, 1), new(1, 1), new(2, 1) }, centerGridPosition = new Vector2Int(1,1), orientation = Orientation.Horizontal},
                new() {gridVector2IntList = new List<Vector2Int> { new(0, 2), new(1, 2), new(2, 2) }, centerGridPosition = new Vector2Int(1,2), orientation = Orientation.Horizontal},

                //Vertical
                new() {gridVector2IntList = new List<Vector2Int> { new(0, 0), new(0, 1), new(0, 2) }, centerGridPosition = new Vector2Int(0,1), orientation = Orientation.Vertical},
                new() {gridVector2IntList = new List<Vector2Int> { new(1, 0), new(1, 1), new(1, 2) }, centerGridPosition = new Vector2Int(1,1), orientation = Orientation.Vertical},
                new() {gridVector2IntList = new List<Vector2Int> { new(2, 0), new(2, 1), new(2, 2) }, centerGridPosition = new Vector2Int(2,1), orientation = Orientation.Vertical},

                //Diagonals
                new() {gridVector2IntList = new List<Vector2Int> { new(0, 0), new(1, 1), new(2, 2) }, centerGridPosition = new Vector2Int(1,1), orientation = Orientation.DiagonalA},
                new() {gridVector2IntList = new List<Vector2Int> { new(0, 2), new(1, 1), new(2, 0) }, centerGridPosition = new Vector2Int(1,1), orientation = Orientation.DiagonalB},
            };
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

            if (_playerTypeArray[x, y] != PlayerType.None) //Already occupied
                return;

            _playerTypeArray[x, y] = playerType;

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

            TestWinner();
        }

        private void TestWinner()
        {
            foreach (Line line in lineList.Where(TestWinnerLine))
            {
                //Win
                Debug.Log("Winner!");
                _currentPlayablePlayerType = PlayerType.None;
                OnGameWin?.Invoke(this, new OnGameWinEventArgs
                {
                    line = line
                });
                break;
            }
        }

        private bool TestWinnerLine(Line line)
        {
            return TestWinnerLine(
                _playerTypeArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y],
                _playerTypeArray[line.gridVector2IntList[1].x, line.gridVector2IntList[1].y],
                _playerTypeArray[line.gridVector2IntList[2].x, line.gridVector2IntList[2].y]
                );
        }

        private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
        {
            return aPlayerType != PlayerType.None &&
                   aPlayerType == bPlayerType &&
                   bPlayerType == cPlayerType;
        }
    }
}