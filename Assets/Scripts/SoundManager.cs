using Mirror;
using System;
using UnityEngine;
using static Assets.Scripts.GameManager;

namespace Assets.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private GameObject _placeSfxPrefab;
        [SerializeField] private GameObject _winSfxPrefab;
        [SerializeField] private GameObject _loseSfxPrefab;

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
            gameManager.OnPlacedObject += GameManager_OnPlacedObject;
            gameManager.OnGameWin += GameManager_OnGameWin;
        }

        private void GameManager_OnGameManagerReady(object sender, GameManager.OnGameManagerReadyEventArgs onGameManagerReadyEventArgs)
        {
            onGameManagerReadyEventArgs.gameManager.OnPlacedObject += GameManager_OnPlacedObject;
            onGameManagerReadyEventArgs.gameManager.OnGameWin += GameManager_OnGameWin;
        }

        private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs args)
        {
            NetworkIdentity localPlayerIdentity = NetworkClient.connection.identity;

            if (localPlayerIdentity == null)
                return;

            PlayerController player = localPlayerIdentity.GetComponent<PlayerController>();

            if (player.GetPlayerType() == args.winPlayerType)
            {
                GameObject sfxGameObject = Instantiate(_winSfxPrefab);
                Destroy(sfxGameObject.gameObject, 3f);
            }
            else
            {
                GameObject sfxGameObject = Instantiate(_loseSfxPrefab);
                Destroy(sfxGameObject.gameObject, 3f);
            }
        }

        private void GameManager_OnPlacedObject(object sender, EventArgs args)
        {
            GameObject sfxGameObject = Instantiate(_placeSfxPrefab);
            Destroy(sfxGameObject.gameObject, 3f);
        }
    }
}