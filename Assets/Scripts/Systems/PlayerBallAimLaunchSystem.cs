using Components;
using Game.Configs;
using Leopotam.EcsLite;
using Services;
using UnityEngine;
using Views;

namespace Systems
{
    public class PlayerBallAimLaunchSystem : IEcsRunSystem 
    {
        private readonly GameConfig _config;
        private readonly TurnService _turnService;
        
        public PlayerBallAimLaunchSystem(GameConfig config, TurnService turnService)
        {
            _config = config;
            _turnService = turnService;
        }
        
        public void Run(IEcsSystems systems) 
        {
            if (_turnService.CurrentTurn().State != TurnState.Move) return;
            if (_turnService.CurrentTurn().Participant != TurnParticipant.Player) return;

            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButton(0) && !(Input.GetMouseButtonUp(0))) return;
            
            var world = systems.GetWorld();
            var draggedPool = world.GetPool<DraggedComponent>();
            var rigidbodyPool = world.GetPool<RigidbodyRef>();
            
            //start
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
                if (!Physics.Raycast(ray, out var hit)) return;

                if (hit.collider.TryGetComponent(out BallView ballView))
                {
                    if (ballView.Owner != TurnParticipant.Player) return;
                }
                
                var filter = world.Filter<BallTag>().Inc<PlayerTag>().End();
                
                foreach (var entity in filter) 
                {
                    if (rigidbodyPool.Get(entity).Rigidbody != hit.rigidbody) continue;
                
                    draggedPool.Add(entity);
                    
                    return;
                }
            }

            //hold
            if (Input.GetMouseButton(0))
            {
                var filter = world.Filter<DraggedComponent>().Inc<RigidbodyRef>().End();
                
                foreach (var entity in filter) 
                {
                    ref var drag = ref draggedPool.Get(entity);
                    ref var rb = ref rigidbodyPool.Get(entity);

                    var dragEnd = GetMouseGroundPoint();
                    var dragStart = ProjectToGround(rb.Rigidbody.position);
                    var direction = dragStart - dragEnd;
                    direction.y = 0f;
                    
                    drag.Direction = direction;

                    return;
                }
            }

            //release
            if (Input.GetMouseButtonUp(0))
            {
                var filter = world.Filter<DraggedComponent>().Inc<RigidbodyRef>().End();

                foreach (var entity in filter)
                {
                    ref var drag = ref draggedPool.Get(entity);
                    ref var rb = ref rigidbodyPool.Get(entity);
                    var direction = drag.Direction;
                    
                    var force = direction.magnitude * _config.playerForce;
                    rb.Rigidbody.velocity = direction.normalized * force;

                    draggedPool.Del(entity);
                    
                    world.GetPool<EndTurnStateRequest>().Add(world.NewEntity());

                    return;
                }   
            }
        }
        
        Vector3 GetMouseGroundPoint() 
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var ground = new Plane(Vector3.up, Vector3.zero);

            return ground.Raycast(ray, out var enter) ? ray.GetPoint(enter) : Vector3.zero;
        }

        Vector3 ProjectToGround(Vector3 worldPos) 
        {
            return new Vector3(worldPos.x, 0f, worldPos.z);
        }
    }
}