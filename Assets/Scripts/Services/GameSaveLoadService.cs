using Leopotam.EcsLite;
using UnityEngine;
using System.IO;
using Components;
using Services;
using Views;

namespace Systems
{
    public static class GameSaveLoadService 
    {
        private static readonly string _savePath = Application.persistentDataPath + "/save.json";

        public static void Save(IEcsSystems systems) 
        {
            var world = systems.GetWorld();
            var saveData = new GameSaveData();

            var rigidbodyPool = world.GetPool<RigidbodyRef>();
            var playerPool = world.GetPool<PlayerTag>();
            foreach (var entity in world.Filter<RigidbodyRef>().End()) 
            {
                ref var rb = ref rigidbodyPool.Get(entity);
                var data = new BallSaveData {
                    position = rb.Rigidbody.transform.position,
                    velocity = rb.Rigidbody.velocity,
                    isPlayer = playerPool.Has(entity)
                };
                saveData.balls.Add(data);
            }

            saveData.scorePlayer = ScoreService.Player;
            saveData.scoreAI = ScoreService.AI;

            string json = JsonUtility.ToJson(saveData);
            File.WriteAllText(_savePath, json);
            Debug.Log("Game Saved");
        }

        public static void Load(IEcsSystems systems, BallSpawner spawner) 
        {
            if (!File.Exists(_savePath)) 
            {
                Debug.LogWarning("No save file found");
                return;
            }

            var json = File.ReadAllText(_savePath);
            var saveData = JsonUtility.FromJson<GameSaveData>(json);

            var world = systems.GetWorld();
            var ballTag = world.GetPool<BallTag>();
            var rigidbodyPool = world.GetPool<RigidbodyRef>();
            var playerPool = world.GetPool<PlayerTag>();
            var aiPool = world.GetPool<AITag>();

            foreach (var b in Object.FindObjectsOfType<BallView>()) 
            {
                Object.Destroy(b.gameObject);
            }

            foreach (var ball in saveData.balls) 
            {
                var go = spawner.Spawn(ball.position, ball.velocity, ball.isPlayer);

                var entity = world.NewEntity();
                ballTag.Add(entity);
                rigidbodyPool.Add(entity).Rigidbody = go.GetComponent<Rigidbody>();
                if (ball.isPlayer) playerPool.Add(entity);
                else aiPool.Add(entity);
            }

            ScoreService.Player = saveData.scorePlayer;
            ScoreService.AI = saveData.scoreAI;

            Debug.Log("Game Loaded");
        }
    }

}