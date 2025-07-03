using Components;
using Game.Configs;
using Leopotam.EcsLite;
using Services;
using UnityEngine;
using Views;

namespace Systems
{
    public class PlayerBallAimSystem : IEcsRunSystem 
    {
        private readonly TurnService _turnService;
        
        public PlayerBallAimSystem(TurnService turnService)
        {
            _turnService = turnService;
        }
        
        public void Run(IEcsSystems systems) 
        {
            if (_turnService.CurrentTurn().State != TurnState.Move) return;
            if (_turnService.CurrentTurn().Participant != TurnParticipant.Player) return;
            
            if (!Input.GetMouseButtonDown(0)) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (!Physics.Raycast(ray, out var hit)) return;

            if (hit.collider.TryGetComponent(out BallView ballView))
            {
                if (ballView.Owner != TurnParticipant.Player) return;
            }

            var world = systems.GetWorld();

            var filter = world.Filter<BallTag>().Inc<PlayerTag>().End();
            var draggedPool = world.GetPool<DraggedComponent>();
            var rigidbodyPool = world.GetPool<RigidbodyRef>();

            foreach (var entity in filter) 
            {
                ref var rb = ref rigidbodyPool.Get(entity);
                
                if (rb.Rigidbody != hit.rigidbody) continue;
                
                ref var drag = ref draggedPool.Add(entity);
                drag.DragStart = hit.point;
            }
        }
    }
}