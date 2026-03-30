using Manager;
using TMPro;
using UnityEngine;
using DG.Tweening; 

namespace GameGUI
{
    public class ScoreGUI : MonoBehaviour 
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField] private ResultGUI _resultGUI;
        [SerializeField] private float popDuration = 0.5f;

        void Start()
        {
            EventManager.ScoreSystem.MatchValue += HandleDisplay;

            _resultGUI.transform.localScale = Vector3.zero;
            _resultGUI.gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            EventManager.ScoreSystem.MatchValue -= HandleDisplay;
        }

        private void HandleDisplay(float matchValue)
        {
            _resultGUI.DisplayResults(matchValue);
            _resultGUI.gameObject.SetActive(true);
            _resultGUI.transform.DOScale(Vector3.one, popDuration).SetEase(Ease.OutBack);
        }

        public void HideDisplay()
        {
            _resultGUI.transform.DOScale(Vector3.zero, popDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => 
                {
                    _resultGUI.gameObject.SetActive(false);
                });
        }
    }
}