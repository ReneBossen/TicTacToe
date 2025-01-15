using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] private Button _startHostButton;
        [SerializeField] private Button _startClientButton;

        private void Awake()
        {
            _startHostButton.onClick.AddListener(() =>
            {
                NetworkManager.singleton.StartHost();
                Hide();
            });
            _startClientButton.onClick.AddListener(() =>
            {
                NetworkManager.singleton.StartClient();
                Hide();
            });
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
