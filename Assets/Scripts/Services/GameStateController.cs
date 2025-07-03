using Components;
using Systems;
using UnityEngine.SceneManagement;

namespace Services
{
    public static class GameStateController 
    {
        private static BallSpawner _spawner;
        private static TurnService _turnService;

        public static void Init(BallSpawner spawner, TurnService turnService) 
        {
            _spawner = spawner;
            _turnService = turnService;
        }

        public static void PauseGame(bool pause)
        {
            if (pause)
                TimeService.PauseTime();
            else
                TimeService.ResumeTime();
        }
        
        public static void Save() 
        {
            GameSaveLoadService.Save(EcsSystemsRef.Systems, _turnService);
        }

        public static void Load()
        {
            ScoreService.Reset();
            GameSaveLoadService.Load(EcsSystemsRef.Systems, _spawner, _turnService);
        }

        public static void Restart()
        {
            GameReset();
                
            int currentScene = SceneManager.GetActiveScene().buildIndex; 
            SceneManager.LoadScene(currentScene);
        }

        public static void GameReset()
        {
            ScoreService.Reset();
            TimeService.ResumeTime();
        }
    }
}