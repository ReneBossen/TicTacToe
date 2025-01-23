using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static event EventHandler<OnGameManagerReadyEventArgs> OnGameManagerReady;

        public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
        public event EventHandler OnGameStarted;
        public event EventHandler OnCurrentPlayablePlayerTypeChanged;
        public event EventHandler OnScoreChanged;
        public event EventHandler<OnGameWinEventArgs> OnGameWin;
        public event EventHandler OnRematch;
        public event EventHandler OnGameTied;
        public event EventHandler OnPlacedObject;

        public class OnGameManagerReadyEventArgs : EventArgs
        {
            public GameManager gameManager;
        }

        public class OnGameWinEventArgs : EventArgs
        {
            public Line line;
            public PlayerType winPlayerType;
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
        [SyncVar(hook = nameof(TriggerOnScoreChanged))]
        private int _playerCrossScore;
        [SyncVar(hook = nameof(TriggerOnScoreChanged))]
        private int _playerCircleScore;

        private PlayerType _startedLastMatchPlayerType;
        private PlayerType[,] _playerTypeArray;
        private List<Line> _lineList;


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one GameManager Instance!");
            }

            Instance = this;

            _playerTypeArray = new PlayerType[3, 3];

            _lineList = new List<Line>
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
        public void OnServerAddPlayer()
        {
            Debug.Log("GameManager OnServerAddPlayer called");
            RpcNotifyClientsToSubscribe();
        }

        [ClientRpc]
        private void RpcNotifyClientsToSubscribe()
        {
            Debug.Log("RpcNotifyClientsToSubscribe called");
            OnGameManagerReady?.Invoke(this, new OnGameManagerReadyEventArgs
            {
                gameManager = this
            });
        }

        [Server]
        public void SetStartingPlayerType(PlayerType playerType)
        {
            _currentPlayablePlayerType = playerType;
            _startedLastMatchPlayerType = playerType;
        }

        public Task<PlayerType> GetCurrentPlayablePlayerType()
        {
            return Task.FromResult(_currentPlayablePlayerType);
        }

        private void TriggerOnCurrentPlayablePlayerTypeChanged(PlayerType oldPlayerType, PlayerType newPlayerType)
        {
            OnCurrentPlayablePlayerTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TriggerOnScoreChanged(int oldScore, int newScore)
        {
            OnScoreChanged?.Invoke(this, EventArgs.Empty);
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
            TriggerOnPlacedObjectRpc();

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

        [Server]
        private void TestWinner()
        {
            foreach (Line line in _lineList.Where(TestWinnerLine))
            {
                _currentPlayablePlayerType = PlayerType.None;
                PlayerType winningPlayerType = _playerTypeArray[line.centerGridPosition.x, line.centerGridPosition.y];

                switch (winningPlayerType)
                {
                    default:
                    case PlayerType.Cross:
                        _playerCrossScore++;
                        break;
                    case PlayerType.Circle:
                        _playerCircleScore++;
                        break;
                }

                TriggerOnGameWinnerRpc(line, winningPlayerType);
                return;
            }

            TestIfTie();
        }

        [Server]
        private void TestIfTie()
        {
            bool hasTie = true;
            for (int x = 0; x < _playerTypeArray.GetLength(0); x++)
            {
                for (int y = 0; y < _playerTypeArray.GetLength(1); y++)
                {
                    if (_playerTypeArray[x, y] == PlayerType.None)
                    {
                        hasTie = false;
                        break;
                    }
                }
            }

            if (hasTie)
            {
                TriggerOnGameTiedRpc();
            }
        }

        [ClientRpc]
        private void TriggerOnPlacedObjectRpc()
        {
            OnPlacedObject?.Invoke(this, EventArgs.Empty);
        }

        [ClientRpc]
        private void TriggerOnGameWinnerRpc(Line line, PlayerType playerType)
        {
            OnGameWin?.Invoke(this, new OnGameWinEventArgs
            {
                line = line,
                winPlayerType = playerType
            });
        }

        [ClientRpc]
        private void TriggerOnGameTiedRpc()
        {
            OnGameTied?.Invoke(this, EventArgs.Empty);
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

        [Server]
        public void Rematch()
        {
            for (int x = 0; x < _playerTypeArray.GetLength(0); x++)
            {
                for (int y = 0; y < _playerTypeArray.GetLength(1); y++)
                {
                    _playerTypeArray[x, y] = PlayerType.None;
                }
            }

            SetRematchStartingPlayerType();
            TriggerOnRematchRpc();
        }

        [ClientRpc]
        private void TriggerOnRematchRpc()
        {
            OnRematch?.Invoke(this, EventArgs.Empty);
        }

        [Server]
        private void SetRematchStartingPlayerType()
        {
            switch (_startedLastMatchPlayerType)
            {
                default:
                case PlayerType.Cross:
                    _currentPlayablePlayerType = PlayerType.Circle;
                    _startedLastMatchPlayerType = PlayerType.Circle;
                    break;
                case PlayerType.Circle:
                    _currentPlayablePlayerType = PlayerType.Cross;
                    _startedLastMatchPlayerType = PlayerType.Cross;
                    break;
            }
        }

        public void GetScores(out int playerCrossScore, out int playerCircleScore)
        {
            playerCrossScore = _playerCrossScore;
            playerCircleScore = _playerCircleScore;
        }
    }
}