using System;
using System.Collections.Generic;
using Global_Data;
using Manager;
using UnityEngine;

namespace Model_Scripts
{
    public class ModelTarget : MonoBehaviour
    {
        public ObjectiveTarget CurrentObjectiveTarget { get; private set; }
        
        [Header("Model Target References")]
        [SerializeField] private Material _targetMaterial;
        [SerializeField] private Renderer modelRenderer;
        
        [Header("Level List")]
        [SerializeField] private List<ObjectiveTarget> _objectiveTargets;

        private int _currentTargetIndex = 0; 

        private void Start()
        {
            //Event Sub
            EventManager.ButtonsOnClickEvent.NextLevel += LoadNextObjectiveTarget;
            
            if (_objectiveTargets != null && _objectiveTargets.Count > 0)
            {
                _currentTargetIndex = 0; 
            }
        }

        private void OnDestroy()
        {
            EventManager.ButtonsOnClickEvent.NextLevel -= LoadNextObjectiveTarget;
        }

        public void ChangeObjectiveTarget(ObjectiveTarget newTarget) 
        {
            if (_targetMaterial != null)
            {
                CurrentObjectiveTarget = newTarget;

                _targetMaterial.SetTexture(GlobalMembers.ShaderIDs.ShaveMask, newTarget.targetFurTexture);
        
                _targetMaterial.SetTexture(GlobalMembers.ShaderIDs.ColorMap, newTarget.targetColorTexture);
                
                if (modelRenderer != null)
                {
                    modelRenderer.material = _targetMaterial; 
                }

            }
        }

        public void LoadNextObjectiveTarget() 
        {
            if (_objectiveTargets != null && _objectiveTargets.Count > 0)
            {
                _currentTargetIndex++; 

                if (_currentTargetIndex >= _objectiveTargets.Count)
                {
                    _currentTargetIndex = 0; 
                }

                ChangeObjectiveTarget(_objectiveTargets[_currentTargetIndex]);
            }
        }
        
    }
}