using System;
using UnityEngine;

namespace Manager
{
    public class VFXManager : MonoBehaviour
    {
        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem fallingHairParticles; 
        [SerializeField] private ParticleSystem regrowthSparkleParticles; 
        [SerializeField] private ParticleSystem sprayPaintBurstParticles; 
        
        private Color currentSprayColor = Color.white; // Default to white just in case!

        private void Start()
        {
            if (ToolBoxManager.Instance != null)
                ToolBoxManager.Instance.OnColorSelected += ChangeSprayColor;
        }

        private void OnDestroy()
        {
            if (ToolBoxManager.Instance != null)
                ToolBoxManager.Instance.OnColorSelected -= ChangeSprayColor;
        }

        public void PlayCutVFX(Vector3 hitPosition, Color hairColor) 
        {
            fallingHairParticles.transform.position = hitPosition;
            var mainModule = fallingHairParticles.main;
            mainModule.startColor = hairColor;
            fallingHairParticles.Emit(2); 
        }

        public void PlayRegrowVFX(Vector3 hitPosition)
        {
            regrowthSparkleParticles.transform.position = hitPosition;
            regrowthSparkleParticles.Emit(1); 
        }

        public void PlaySprayVFX(Vector3 hitPosition) 
        {
            sprayPaintBurstParticles.transform.position = hitPosition;
            
            var mainModule = sprayPaintBurstParticles.main;
            mainModule.startColor = currentSprayColor; 

            sprayPaintBurstParticles.Emit(1); 
        }

        private void ChangeSprayColor(Color color)
        {
            currentSprayColor = color;
        }
    }
}