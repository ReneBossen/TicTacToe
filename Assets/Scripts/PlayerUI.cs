using Mirror;
using System;
using TMPro;
using UnityEngine;

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
            GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
            GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;
            GameManager.Instance.OnScoreChanged += GameManager_OnScoreChanged;
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