using Components;
using Game.Configs;
using Leopotam.EcsLite;
using Services;
using UnityEngine;

namespace Systems
{
    public class AIStrategySystem : IEcsRunSystem 
    {
        private readonly TurnService _turnService;
        private readonly GameConfig _config;
        
        public AIStrategySystem(GameConfig config, TurnService turnService) 
        {
            _config = config;
            _turnService = turnService;
        }

        public void Run(IEcsSystems systems)
        {
            if (_turnService.CurrentTurn().State != TurnState.Move) return;
            if (_turnService.CurrentTurn().Participant != TurnParticipant.AI) return;
            
            var world = systems.GetWorld();
            var rigidbodyPool = world.GetPool<RigidbodyRef>();

            var aiFilter = world.Filter<BallTag>().Inc<AITag>().Inc<RigidbodyRef>().Exc<DraggedComponent>().End();
            var aiList = new System.Collections.Generic.List<int>();
            foreach (var entity in aiFilter)
                aiList.Add(entity);
            
            if (aiList.Count == 0) return;
            
            var playerFilter = world.Filter<BallTag>().Inc<PlayerTag>().Inc<RigidbodyRef>().End();
            var playerList = new System.Collections.Generic.List<int>();
            foreach (var entity in playerFilter)
                playerList.Add(entity);

            if (playerList.Count == 0) return;

            int choiceAI = Random.Range(0, aiList.Count);
            var selectedAI = aiList[choiceAI];
            ref var selectedRb = ref rigidbodyPool.Get(selectedAI);

            int choiceTarget = Random.Range(0, playerList.Count);
            var selectedTarget = playerList[choiceTarget];
            var targetPosition = rigidbodyPool.Get(selectedTarget).Rigidbody.position;

            var dir = (targetPosition - selectedRb.Rigidbody.position);
            dir.y = 0f;
            dir.Normalize();
            
            var inaccuracy = Random.Range(-_config.aiInaccuracyMultiplier, _config.aiInaccuracyMultiplier);
            
            var offset = Quaternion.Euler(0, inaccuracy, 0);
            var inaccurateDir = offset * dir;
            
            var force = inaccurateDir * _config.aiForce;
            selectedRb.Rigidbody.velocity = force;

            world.GetPool<EndTurnStateRequest>().Add(world.NewEntity());
        }
    }
}