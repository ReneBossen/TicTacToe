using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameVisualManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _circlePrefab;
        [SerializeField] private GameObject _crossPrefab;
        [SerializeField] private GameObject _winnerPrefab;

        private List<GameObject> _visualGameObjectList;

        private const float GRID_SIZE = 2.4f;

        private void Awake()
        {
            _visualGameObjectList = new List<GameObject>();
        }

        private void Start()
        {
            GameManager.Instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPosition;

            if (!isServer)
                return;

            GameManager.Instance.OnGameWin += GameManager_OnGameWin;
            GameManager.Instance.OnRematch += GameManager_OnRematch;
        }

        private void GameManager_OnRematch(object sender, EventArgs args)
        {
            foreach (GameObject visualGameObject in _visualGameObjectList)
            {
                Destroy(visualGameObject);
            }

            _visualGameObjectList.Clear();
        }

        private void GameManager_OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs args)
        {
            SpawnObject(args.x, args.y, args.playerType);
        }

        [Server]
        private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs args)
        {
            float eulerZ = 0f;
            switch (args.line.orientation)
            {
                default:
                case GameManager.Orientation.Horizontal:
                    eulerZ = 0f;
                    break;
                case GameManager.Orientation.Vertical:
                    eulerZ = 90f;
                    break;
                case GameManager.Orientation.DiagonalA:
                    eulerZ = 45f;
                    break;
                case GameManager.Orientation.DiagonalB:
                    eulerZ = -45f;
                    break;
            }

            GameObject winnerBar = Instantiate(
                _winnerPrefab,
                GetGridWorldPosition(args.line.centerGridPosition.x, args.line.centerGridPosition.y),
                Quaternion.Euler(0, 0, eulerZ));

            NetworkServer.Spawn(winnerBar);

            _visualGameObjectList.Add(winnerBar);
        }

        [Server]
        private void SpawnObject(int x, int y, GameManager.PlayerType playerType)
        {
            GameObject prefab;

            switch (playerType)
            {
                default:
                case GameManager.PlayerType.Cross:
                    prefab = _crossPrefab;
                    break;
                case GameManager.PlayerType.Circle:
                    prefab = _circlePrefab;
                    break;
            }

            GameObject spawnedCrossObject = Instantiate(prefab, GetGridWorldPosition(x, y), Quaternion.identity);
            NetworkServer.Spawn(spawnedCrossObject);

            _visualGameObjectList.Add(spawnedCrossObject);
        }

        private Vector2 GetGridWorldPosition(int x, int y)
        {
            return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
        }
    }
}