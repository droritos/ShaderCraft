using System;
using System.Collections.Generic;
using DG.Tweening;
using Global_Data;
using Grooming;
using Manager;
using UnityEngine;

namespace Model_Scripts
{
    public class ModelTarget : MonoBehaviour
    {
        public ObjectiveTarget CurrentObjectiveTarget { get; private set; }
        
        [Header("Model Target References")]
        [SerializeField] private Renderer modelRenderer;
        [SerializeField] private FurShellGenerator targetFurGenerator;
        
        [Header("Level List")]
        [SerializeField] private List<ObjectiveTarget> _objectiveTargets;

        private int _currentTargetIndex = 0; 

        private void Start()
        {
            // Event Sub
            EventManager.ButtonsOnClickEvent.NextLevel += LoadNextObjectiveTarget;
            
            if (_objectiveTargets != null && _objectiveTargets.Count > 0)
            {
                _currentTargetIndex = 0; 
            }

            RotateAnimation();
        }

        private void OnDestroy()
        {
            EventManager.ButtonsOnClickEvent.NextLevel -= LoadNextObjectiveTarget;
        }

        public void ChangeObjectiveTarget(ObjectiveTarget newTarget) 
        {
            CurrentObjectiveTarget = newTarget;
            UpdateMaterialTarget(newTarget);
        }

        private void LoadNextObjectiveTarget() 
        {
            if (_objectiveTargets is { Count: > 0 })
            {
                _currentTargetIndex++; 

                if (_currentTargetIndex >= _objectiveTargets.Count)
                {
                    _currentTargetIndex = 0; 
                }

                ChangeObjectiveTarget(_objectiveTargets[_currentTargetIndex]);
            }
            modelRenderer.gameObject.SetActive(true);
            Debug.Log("Model Target Loaded");
        }
        
        private void UpdateMaterialTarget(ObjectiveTarget newTarget)
        {
            if (modelRenderer != null) 
            {
                Material currentInstanceMat = modelRenderer.material;
                currentInstanceMat.SetTexture(GlobalMembers.ShaderIDs.ShaveMask, newTarget.targetFurTexture);
                currentInstanceMat.SetTexture(GlobalMembers.ShaderIDs.ColorMap, newTarget.targetColorTexture);
            }

            if (targetFurGenerator != null)
            {
                targetFurGenerator.UpdateAllShellTextures(newTarget.targetFurTexture, newTarget.targetColorTexture);
            }
        }

        private void RotateAnimation()
        {
            modelRenderer.transform.DOKill();

            modelRenderer.transform.DORotate(new Vector3(0f, 360f, 0f), 10f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)       
                .SetLoops(-1, LoopType.Restart) 
                .SetRelative(true);        
        }
    }
}