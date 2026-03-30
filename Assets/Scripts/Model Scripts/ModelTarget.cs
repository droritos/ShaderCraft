using System.Collections.Generic;
using Global_Data;
using UnityEngine;

namespace Model_Scripts
{
    public class ModelTarget : MonoBehaviour
    {
        [SerializeField] private List<ObjectiveTarget> _objectiveTargets;
       
        public ObjectiveTarget CurrentObjectiveTarget { get; private set; }

        private int _currentTargetIndex = 0; 

        private void Start()
        {
            if (_objectiveTargets != null && _objectiveTargets.Count > 0)
            {
                // Start the game on the very first target (Index 0)
                _currentTargetIndex = 0; 
                ChangeObjectiveTarget(_objectiveTargets[_currentTargetIndex]);
            }
        }

        public void ChangeObjectiveTarget(ObjectiveTarget newTarget) 
        {
            CurrentObjectiveTarget = newTarget;
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