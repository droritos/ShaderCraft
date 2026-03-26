using System;
using UnityEngine;

namespace Grooming
{
    public class GroomingRaycaster : MonoBehaviour
    {
        [SerializeField] Camera mainCamera;

        // We use 'Actions' (Events) so other scripts can listen to this one without being tightly coupled to it.
        public event Action<Vector2> OnFurHit;
        public event Action OnInteractionStopped;

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    // Shout to anyone listening: "We hit the fur at this UV coordinate!"
                    OnFurHit?.Invoke(hit.textureCoord);
                }
                else
                {
                    // We clicked, but missed the fur
                    OnInteractionStopped?.Invoke();
                }
            }
            else
            {
                // We aren't clicking at all
                OnInteractionStopped?.Invoke();
            }
        }

        private void OnValidate()
        {
            if (!mainCamera) mainCamera = Camera.main;
        }
    }
}