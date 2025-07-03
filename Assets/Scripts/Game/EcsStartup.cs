using Components;
using Game.Configs;
using Game.UI;
using Leopotam.EcsLite;
using Services;
using Systems;
using UnityEngine;

namespace Game
{
    public class EcsStartup : MonoBehaviour 
    {
        private EcsSystems _systems;

        [SerializeField] private GameConfig config;
        [SerializeField] private BallSpawner spawner;
        [SerializeField] private  UIController uiController;
        
        private void Start() 
        {
            var world = new EcsWorld();
            _systems = new EcsSystems(world);
            EcsSystemsRef.Systems = _systems;
            
            var turnService = new TurnService(world);
            
            GameStateController.Init(spawner, turnService); 
            uiController.Init(turnService);

            _systems
                .Add(new PlayerBallAimSystem(turnService))
                .Add(new PlayerBallLaunchSystem(config, turnService))
                .Add(new AIStrategySystem(config, turnService))
                .Add(new BallOutSystem(config))
                .Add(new TurnControlSystem(config))
                .Add(new EndGameConditionSystem(config, uiController))
                .Init();
            
            spawner.SpawnBalls(_systems);
        }

        private void Update() => _systems?.Run();

        private void OnDestroy()
        {
            if (_systems == null) return;
            
            _systems.GetWorld()?.Destroy();
            _systems.Destroy();
            _systems = null;
        }
    }
}