using System;
using UnityEngine;

namespace Microgame.PlatformMinigame
{
    public class DestroyObjects : MonoBehaviour
    {
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Brick") || other.gameObject.CompareTag("Player"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
