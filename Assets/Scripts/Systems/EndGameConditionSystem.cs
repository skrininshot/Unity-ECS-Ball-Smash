using Components;
using Game.Configs;
using Game.UI;
using Leopotam.EcsLite;
using Services;

namespace Systems
{
    public class EndGameConditionSystem : IEcsRunSystem 
    {
        private bool _gameEnded = false;
        private readonly UIController _uiController;
        private readonly GameConfig _config;

        public EndGameConditionSystem(GameConfig config, UIController uiController) 
        {
            _config = config;
            _uiController = uiController;
        }

        public void Run(IEcsSystems systems) 
        {
            if (_gameEnded) return;
            
            if (ScoreService.Player >= _config.aiBallsCount) 
            {
                _uiController.ShowResult(TurnParticipant.Player);
                _gameEnded = true;
            }
            
            if (ScoreService.AI >= _config.playerBallsCount) 
            {
                _uiController.ShowResult(TurnParticipant.AI);
                _gameEnded = true;
            }
        }
    }
}