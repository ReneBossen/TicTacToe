using Mirror;
using System;
using TMPro;
using UnityEngine;
using static Assets.Scripts.GameManager;

namespace Assets.Scripts
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private GameObject _crossYouTextGameObject;
        [SerializeField] private GameObject _circleYouTextGameObject;
        [SerializeField] private GameObject _crossArrowGameObject;
        [SerializeField] private GameObject _circleArrowGameObject;
        [SerializeField] private TextMeshProUGUI _playerCrossScoreTextMesh;
        [SerializeField] private TextMeshProUGUI _playerCircleScoreTextMesh;

        private void Awake()
        {
            _crossYouTextGameObject.SetActive(false);
            _circleYouTextGameObject.SetActive(false);
            _crossArrowGameObject.SetActive(false);
            _circleArrowGameObject.SetActive(false);
            _playerCrossScoreTextMesh.text = "";
            _playerCircleScoreTextMesh.text = "";
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager_OnGameManagerReady(GameManager.Instance);
            }
            else
            {
                GameManager.OnGameManagerReady += GameManager_OnGameManagerReady;
            }
        }

        private void GameManager_OnGameManagerReady(GameManager gameManager)
        {
            gameManager.OnGameStarted += GameManager_OnGameStarted;
            gameManager.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;
            gameManager.OnScoreChanged += GameManager_OnScoreChanged;
        }

        private void GameManager_OnGameManagerReady(object sender, GameManager.OnGameManagerReadyEventArgs onGameManagerReadyEventArgs)
        {
            onGameManagerReadyEventArgs.gameManager.OnGameStarted += GameManager_OnGameStarted;
            onGameManagerReadyEventArgs.gameManager.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;
            onGameManagerReadyEventArgs.gameManager.OnScoreChanged += GameManager_OnScoreChanged;
        }

        private void GameManager_OnScoreChanged(object sender, EventArgs args)
        {
            GameManager.Instance.GetScores(out int playerCrossScore, out int playerCircleScore);

            _playerCrossScoreTextMesh.text = playerCrossScore.ToString();
            _playerCircleScoreTextMesh.text = playerCircleScore.ToString();
        }

        private void GameManager_OnGameStarted(object sender, EventArgs args)
        {
            NetworkIdentity localPlayerIdentity = NetworkClient.connection.identity;

            if (localPlayerIdentity == null)
                return;

            PlayerController player = localPlayerIdentity.GetComponent<PlayerController>();

            if (player == null)
                return;

            if (player.GetPlayerType() == GameManager.PlayerType.Cross)
            {
                _crossYouTextGameObject.SetActive(true);
            }
            else
            {
                _circleYouTextGameObject.SetActive(true);
            }

            _playerCrossScoreTextMesh.text = "0";
            _playerCircleScoreTextMesh.text = "0";
        }

        private void GameManager_OnCurrentPlayablePlayerTypeChanged(object sender, EventArgs args)
        {
            UpdateCurrentArrow();
        }

        private void UpdateCurrentArrow()
        {
            if (GameManager.Instance.GetCurrentPlayablePlayerType() == GameManager.PlayerType.Cross)
            {
                _crossArrowGameObject.SetActive(true);
                _circleArrowGameObject.SetActive(false);
            }
            else
            {
                _crossArrowGameObject.SetActive(false);
                _circleArrowGameObject.SetActive(true);
            }
        }
    }
}