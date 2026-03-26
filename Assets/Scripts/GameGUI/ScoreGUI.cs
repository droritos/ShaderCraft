using System.Collections;
using Manager;
using TMPro;
using UnityEngine;

namespace GameGUI
{
    public class ScoreGUI : MonoBehaviour // Later on should be handled by a Manager!
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        private const string Percent = "%";
        void Start()
        {
            EventManager.ScoreSystem.MatchValue += StartDisplay;
        }

        private void StartDisplay(float matchValue) => StartCoroutine(HandleDisplay((int)matchValue));

        IEnumerator HandleDisplay(int matchValue)
        {
            _textMeshProUGUI.text = matchValue + Percent;
            _textMeshProUGUI.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            
            _textMeshProUGUI.gameObject.SetActive(false);
        }
    }
}
