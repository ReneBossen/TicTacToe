using Mirror;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _resultTextMesh;
        [SerializeField] private Color _winColor;
        [SerializeField] private Color _loseColor;

        private void Start()
        {
            GameManager.Instance.OnGameWin += GameManager_OnGameWin;
            Hide();
        }

        private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs args)
        {
            Debug.Log("Received OnGameWin");
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
