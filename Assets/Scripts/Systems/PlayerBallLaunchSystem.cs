using Components;
using Game.Configs;
using Leopotam.EcsLite;
using Services;
using UnityEngine;

namespace Systems
{
    public class PlayerBallLaunchSystem : IEcsRunSystem 
    {
        private readonly GameConfig _config;
        private readonly TurnService _turnService;
        
        public PlayerBallLaunchSystem(GameConfig config, TurnService turnService) 
        {
            _config = config;
            _turnService = turnService;
        }
        
        public void Run(IEcsSystems systems) 
        {
            if (!Input.GetMouseButtonUp(0)) return;

            var world = systems.GetWorld();
            var filter = world.Filter<DraggedComponent>().Inc<RigidbodyRef>().End();
            var draggedPool = world.GetPool<DraggedComponent>();
            var rigidbodyPool = world.GetPool<RigidbodyRef>();

            foreach (var entity in filter) {
                ref var drag = ref draggedPool.Get(entity);
                ref var rb = ref rigidbodyPool.Get(entity);

                var dragEnd = GetMouseGroundPoint();
                var dragStart = ProjectToGround(drag.DragStart);
                var direction = dragStart - dragEnd;

                direction.y = 0f;

                var force = direction.magnitude * _config.playerForce;
                rb.Rigidbody.velocity = direction.normalized * force;

                draggedPool.Del(entity);
            }
            
            world.GetPool<EndTurnStateRequest>().Add(world.NewEntity());
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