using UnityEngine;

namespace Assets.Scripts
{
    public class TargetFps : MonoBehaviour
    {
        private readonly int _targetFps = 60;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _targetFps;
        }
    }
}
