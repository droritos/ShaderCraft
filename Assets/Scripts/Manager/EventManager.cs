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
            // Menu GUI
            public static event UnityAction<DifficultyType> ChangeDifficulty;
            public static event UnityAction Quit;
            public static void RaiseChangeDifficulty(DifficultyType difficulty) => ChangeDifficulty?.Invoke(difficulty);
            public static void RaiseQuit() => Quit?.Invoke();
            
            // Score GUI
            public static event UnityAction Replay;
            public static void RaiseReplay() => Replay?.Invoke();
            
            public static event UnityAction NextLevel;
            public static void RaiseNextLevel() => NextLevel?.Invoke();
            

        }
    }
}
