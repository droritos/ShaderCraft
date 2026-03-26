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
    }
}
