using Mirror;
using System;
using System.Linq;
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

        private void GameManager_OnCurrentPlayablePlayerTypeChanged(object sender, EventArgs args)
        {
            UpdateCurrentArrow();
        }

        private void GameManager_OnGameStarted(object sender, EventArgs args)
        {
            foreach (PlayerController player in NetworkServer.spawned
                         .Select(networkObject => networkObject.Value.GetComponent<PlayerController>())
                         .Where(player => player != null))
            {
                if (player.GetPlayerType() == GameManager.PlayerType.Cross)
                {
                    _crossYouTextGameObject.SetActive(true);
                }
                else
                {
                    _circleYouTextGameObject.SetActive(true);
                }
            }

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