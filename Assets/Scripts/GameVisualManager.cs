using Mirror;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameVisualManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _circlePrefab;
        [SerializeField] private GameObject _crossPrefab;

        private const float GRID_SIZE = 2.4f;

        private void Start()
        {
            GameManager.Instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPosition;
        }

        private void GameManager_OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs args)
        {
            SpawnObject(args.x, args.y, args.playerType);
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
        }

        private Vector2 GetGridWorldPosition(int x, int y)
        {
            return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
        }
    }
}