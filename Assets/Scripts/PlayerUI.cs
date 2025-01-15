using Mirror;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private GameObject _crossYouTextGameObject;
        [SerializeField] private GameObject _circleYouTextGameObject;
        [SerializeField] private GameObject _crossArrowGameObject;
        [SerializeField] private GameObject _circleArrowGameObject;

        private void Awake()
        {
            _crossYouTextGameObject.SetActive(false);
            _circleYouTextGameObject.SetActive(false);
            _crossArrowGameObject.SetActive(false);
            _circleArrowGameObject.SetActive(false);
        }

        private void Start()
        {
            GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
            GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;
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
        }

        private void GameManager_OnCurrentPlayablePlayerTypeChanged(object sender, EventArgs args)
        {
            UpdateCurrentArrow();
        }

        private void UpdateCurrentArrow()
        {
            Debug.Log($"CurrentPlayablePlayerType: {GameManager.Instance.GetCurrentPlayablePlayerType()}");
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