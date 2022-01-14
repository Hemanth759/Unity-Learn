using UnityEngine;

namespace Tutorials.SphereSizeChanger.Scripts
{
    public class HeartBeat : MonoBehaviour
    {
        
        [HideInInspector]
        public float baseSize = 1f;

        private void Update()
        {
            float animationFloat = baseSize + Mathf.Sin(Time.time * 8f) * baseSize / 7f;
            transform.localScale = Vector3.one * animationFloat;
        }
        
    }
}