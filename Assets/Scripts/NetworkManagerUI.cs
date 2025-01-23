using Mirror;
using System;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] private Button _startHostButton;
        [SerializeField] private Button _startClientButton;
        [SerializeField] private TextMeshProUGUI _codeText;
        [SerializeField] private TMP_InputField _joinInput;

        private TicTacToeNetworkManager _networkManager;
        private void Awake()
        {
            _startHostButton.onClick.AddListener(CreateRelay);
            _startClientButton.onClick.AddListener(() =>
            {
                JoinRelay(_joinInput.text);
            });
        }

        private async void Start()
        {
            _networkManager = FindFirstObjectByType<TicTacToeNetworkManager>();
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

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
        }

        private void GameManager_OnGameManagerReady(object sender, GameManager.OnGameManagerReadyEventArgs onGameManagerReadyEventArgs)
        {
            onGameManagerReadyEventArgs.gameManager.OnGameStarted += GameManager_OnGameStarted;
        }

        private void GameManager_OnGameStarted(object sender, EventArgs args)
        {
            Hide();
        }

        private async void CreateRelay()
        {
            try
            {
                await _networkManager.UnityLogin();

                await _networkManager.StartRelayHost(2);

                _codeText.text = $"Code: {_networkManager.relayJoinCode}";

                Debug.Log($"CreateRelay JoinCode: {_networkManager.relayJoinCode}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating Relay: {ex.Message}");
            }
        }

        private async void JoinRelay(string joinCode)
        {
            try
            {
                await _networkManager.UnityLogin();

                _networkManager.relayJoinCode = joinCode;

                _networkManager.JoinRelayServer();
                Debug.Log("Successfully joined Relay!");
            }
            catch (RelayServiceException ex)
            {
                Debug.LogError($"Relay join failed: {ex.Message}");
            }
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
