using System;
using Manager;
using UnityEngine;

namespace Grooming
{
    public class GroomingRaycaster : MonoBehaviour
    {
        public event Action<Vector2, Vector3> OnFurHit;
        public event Action<Vector2> OnFurHover;   
        public event Action OnInteractionStopped; 
        
        [SerializeField] Camera mainCamera;
        
        [Header("Settings")]
        [SerializeField] private LayerMask groomableLayer;
        
        private bool _isPaused;

        private void Start()
        {
            EventManager.PauseSystem.Pause += (state) => _isPaused = state;
        }

        void Update()
        {
            if(_isPaused) return;
            
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groomableLayer))
            {
                Cursor.visible = false;
                OnFurHover?.Invoke(hit.textureCoord);

                if (Input.GetMouseButton(0))
                {
                    OnFurHit?.Invoke(hit.textureCoord,hit.point);
                }
            }
            else
            {
                Cursor.visible = true;
                OnInteractionStopped?.Invoke();
            }
        }

        private void OnValidate()
        {
            if (!mainCamera) mainCamera = Camera.main;
        }
        
        private void OnDisable()
        {
            Cursor.visible = true;
        }
    }
}