using System;
using UnityEngine;

namespace Grooming
{
    public class GroomingRaycaster : MonoBehaviour
    {
        [SerializeField] Camera mainCamera;

        public event Action<Vector2> OnFurHit;     
        public event Action<Vector2> OnFurHover;   
        public event Action OnInteractionStopped;  

        void Update()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // 1. Hide the Windows cursor so the player focuses on the 3D ring
                Cursor.visible = false;

                OnFurHover?.Invoke(hit.textureCoord);

                if (Input.GetMouseButton(0))
                {
                    OnFurHit?.Invoke(hit.textureCoord);
                }
            }
            else
            {
                // 2. We missed the fur, bring the Windows cursor back!
                Cursor.visible = true;
                OnInteractionStopped?.Invoke();
            }
        }

        private void OnValidate()
        {
            if (!mainCamera) mainCamera = Camera.main;
        }
        
        // Make sure the cursor always comes back if the object is destroyed
        private void OnDisable()
        {
            Cursor.visible = true;
        }
    }
}