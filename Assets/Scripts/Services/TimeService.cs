using UnityEngine;

namespace Services
{
    public static class TimeService 
    {
        private static readonly float _defaultTimeScale = 1f;
        
        public static void PauseTime()
        {
            Time.timeScale = 0f;
        }

        public static void ResumeTime()
        {
            Time.timeScale = _defaultTimeScale;
        }
    }
}