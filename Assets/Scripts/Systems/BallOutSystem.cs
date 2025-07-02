using Components;
using Game.Configs;
using Leopotam.EcsLite;
using Services;
using UnityEngine;

namespace Systems
{
    public class BallOutSystem : IEcsRunSystem 
    {
        private readonly GameConfig _config;

        public BallOutSystem(GameConfig config) 
        {
            _config = config;
        }
        
        public void Run(IEcsSystems systems) 
        {
            var world = systems.GetWorld();
            var rigidbodyPool = world.GetPool<RigidbodyRef>();
            var playerPool = world.GetPool<PlayerTag>();
            var aiPool = world.GetPool<AITag>();

            var toDestroy = new System.Collections.Generic.List<int>();

            foreach (var entity in world.Filter<BallTag>().Inc<RigidbodyRef>().End()) 
            {
                ref var rb = ref rigidbodyPool.Get(entity);
                
                if (!(rb.Rigidbody.transform.position.y < _config.destroyHeight)) continue;
                
                if (playerPool.Has(entity)) 
                {
                    ScoreService.AI++;
                } 
                else if (aiPool.Has(entity)) 
                {
                    ScoreService.Player++;
                }

                Object.Destroy(rb.Rigidbody.gameObject);
                toDestroy.Add(entity);
            }

            foreach (var e in toDestroy)
                world.DelEntity(e);
        }
    }   
}