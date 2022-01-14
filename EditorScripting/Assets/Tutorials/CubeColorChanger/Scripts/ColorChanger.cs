using UnityEngine;

namespace Tutorials.CubeColorChanger.Scripts
{
    public class ColorChanger : MonoBehaviour
    {
    
        private Renderer _renderer;
    
        void Start()
        {
            _renderer = this.GetComponent<Renderer>();
            GenerateColor();
        }

        public void GenerateColor()
        {
            _renderer.sharedMaterial.color = Random.ColorHSV();
        }

        public void Reset()
        {
            this._renderer.sharedMaterial.color = Color.white;
        }
    }
}

