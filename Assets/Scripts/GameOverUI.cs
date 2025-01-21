using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.GameManager;

namespace Assets.Scripts
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _resultTextMesh;
        [SerializeField] private Color _winColor;
        [SerializeField] private Color _loseColor;
        [SerializeField] private Color _tieColor;
        [SerializeField] private Button _rematchButton;

        private void Awake()
        {
            _rematchButton.onClick.AddListener(() =>
            {
                NetworkIdentity localPlayerIdentity = NetworkClient.connection.identity;

                if (localPlayerIdentity == null)
                    return;

                PlayerController player = localPlayerIdentity.GetComponent<PlayerController>();
                player.RematchCmd();
            });
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                Debug.Log($"GameOverUI found GameManager.Instance");
                GameManager_OnGameManagerReady(GameManager.Instance);
            }
            else
            {
                Debug.Log($"GameOverUI cannot find GameManager.Instance, so listening to static event OnGameManagerReady");
                GameManager.OnGameManagerReady += GameManager_OnGameManagerReady;
            }

            Hide();
        }

        private void GameManager_OnGameManagerReady(GameManager gameManager)
        {
            gameManager.OnGameWin += GameManager_OnGameWin;
            gameManager.OnRematch += GameManager_OnRematch;
            gameManager.OnGameTied += GameManager_OnGameTied;
        }

        private void GameManager_OnGameManagerReady(object sender, GameManager.OnGameManagerReadyEventArgs onGameManagerReadyEventArgs)
        {
            onGameManagerReadyEventArgs.gameManager.OnGameWin += GameManager_OnGameWin;
            onGameManagerReadyEventArgs.gameManager.OnRematch += GameManager_OnRematch;
            onGameManagerReadyEventArgs.gameManager.OnGameTied += GameManager_OnGameTied;
        }

        private void GameManager_OnGameTied(object sender, EventArgs args)
        {
            _resultTextMesh.text = "TIE!";
            _resultTextMesh.color = _tieColor;
            Show();
        }

        private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs args)
        {
            NetworkIdentity localPlayerIdentity = NetworkClient.connection.identity;

            if (localPlayerIdentity == null)
                return;

            PlayerController player = localPlayerIdentity.GetComponent<PlayerController>();
            if (args.winPlayerType == player.GetPlayerType())
            {
                _resultTextMesh.text = "YOU WIN!";
                _resultTextMesh.color = _winColor;
            }
            else
            {
                _resultTextMesh.text = "YOU LOST!";
                _resultTextMesh.color = _loseColor;
            }

            Show();
        }

        private void GameManager_OnRematch(object sender, EventArgs args)
        {
            Hide();
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}