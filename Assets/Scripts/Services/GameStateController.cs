using Components;
using Systems;
using UnityEngine.SceneManagement;

namespace Services
{
    public static class GameStateController 
    {
        private static BallSpawner _spawner;

        public static void Init(BallSpawner s) 
        {
            _spawner = s;
        }

        public static void Save() 
        {
            GameSaveLoadService.Save(EcsSystemsRef.Systems);
        }

        public static void Load() 
        {
            GameSaveLoadService.Load(EcsSystemsRef.Systems, _spawner);
            Reset();
        }

        public static void Restart() 
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex; 
            SceneManager.LoadScene(currentScene);
            
            Reset();
        }

        public static void Reset() 
        {
            ScoreService.Reset();
        }
    }

}