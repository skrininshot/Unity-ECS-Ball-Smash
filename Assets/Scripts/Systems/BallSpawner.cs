using System.Collections.Generic;
using Components;
using Game.Configs;
using Leopotam.EcsLite;
using UnityEngine;
using Views;

namespace Systems
{
    public class BallSpawner : MonoBehaviour 
    {
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private GameConfig config;
        
        public GameObject Spawn(Vector3 pos, Vector3 vel, bool isPlayer) 
        {
            var go = Instantiate(ballPrefab, pos, Quaternion.identity);
            go.GetComponent<Rigidbody>().velocity = vel;
            go.GetComponent<BallView>().isPlayerOwned = isPlayer;
            go.GetComponent<Renderer>().material = isPlayer ? config.playerMaterial : config.aiMaterial;
            return go;
        }
        
        public void SpawnBalls(IEcsSystems systems) 
        {
            var world = systems.GetWorld();
            var ballTag = world.GetPool<BallTag>();
            var rigidbodyPool = world.GetPool<RigidbodyRef>();
            var playerPool = world.GetPool<PlayerTag>();
            var aiPool = world.GetPool<AITag>();

            List<Vector3> usedPositions = new();

            int totalBalls = config.playerBallsCount + config.aiBallsCount;
            int triesLimit = 100;

            for (int i = 0; i < totalBalls; i++) {
                bool isPlayer = i < config.playerBallsCount;
                var pos = Vector3.zero;
                int tries = 0;

                do {
                    var offset = Random.insideUnitCircle * config.spawnRadius;
                    pos = new Vector3(offset.x, 1f, offset.y);
                    tries++;
                } while (IsTooClose(pos, usedPositions) && tries < triesLimit);

                usedPositions.Add(pos);

                var go = Spawn(pos, Vector3.zero, isPlayer);
                
                var entity = world.NewEntity();
                ballTag.Add(entity);
                rigidbodyPool.Add(entity).Rigidbody = go.GetComponent<Rigidbody>();
                
                if (isPlayer) 
                    playerPool.Add(entity);
                else 
                    aiPool.Add(entity);
            }
        }
        
        private bool IsTooClose(Vector3 newPos, List<Vector3> existing) 
        {
            foreach (var pos in existing) {
                if (Vector3.Distance(newPos, pos) < config.spawnMinDistance)
                    return true;
            }
            
            return false;
        }
    }
}