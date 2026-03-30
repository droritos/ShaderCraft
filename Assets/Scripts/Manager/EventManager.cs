using Global_Data;
using UnityEngine.Events;

namespace Manager
{
    public static class EventManager
    {
        public static class ScoreSystem
        {
            public static event UnityAction<float> MatchValue;

            public static void RaiseMatchValue(float score)
            {
                MatchValue?.Invoke(score);
            }
            
        }

        public static class ButtonsOnClickEvent
        {
            public static event UnityAction<DifficultyType> ChangeDifficulty;
            public static event UnityAction Quit;
            public static void RaiseChangeDifficulty(DifficultyType difficulty) => ChangeDifficulty?.Invoke(difficulty);
            public static void RaiseQuit() => Quit?.Invoke();

        }
    }
}
