using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private GameObject _placeSfxPrefab;

        private void Start()
        {
            GameManager.Instance.OnPlacedObject += GameManager_OnPlacedObject;
        }

        private void GameManager_OnPlacedObject(object sender, EventArgs args)
        {
            GameObject sfxGameObject = Instantiate(_placeSfxPrefab);
            Destroy(sfxGameObject.gameObject, 3f);
        }
    }
}