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
            Debug.Log("GameManager_OnClickedOnGridPosition");
            SpawnObjectCmd(args.x, args.y);
        }

        [Server]
        private void SpawnObjectCmd(int x, int y)
        {
            Debug.Log("SpawnObject");
            GameObject spawnedCrossObject = Instantiate(_crossPrefab, GetGridWorldPosition(x, y), Quaternion.identity);
            NetworkServer.Spawn(spawnedCrossObject);
        }

        private Vector2 GetGridWorldPosition(int x, int y)
        {
            return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
        }
    }
}