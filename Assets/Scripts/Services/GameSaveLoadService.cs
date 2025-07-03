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

        public static void Save(IEcsSystems systems, TurnService turnService)
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
                    owner = playerPool.Has(entity) ? TurnParticipant.Player : TurnParticipant.AI
                };
                saveData.balls.Add(data);
            }

            saveData.scorePlayer = ScoreService.Player;
            saveData.scoreAI = ScoreService.AI;

            var currentTurn = turnService.CurrentTurn();
            
            saveData.turnState = currentTurn.State;
            saveData.turnParticipant = currentTurn.Participant;
            saveData.turnPauseTime = currentTurn.PauseTime;

            string json = JsonUtility.ToJson(saveData);
            File.WriteAllText(_savePath, json);
            Debug.Log("Game Saved");
        }

        public static void Load(IEcsSystems systems, BallSpawner spawner, TurnService turnService) 
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

            foreach (var e in world.Filter<BallTag>().End()) 
            {
                world.DelEntity(e);
            }

            foreach (var ball in saveData.balls) 
            {
                var go = spawner.Spawn(ball.position, ball.velocity, ball.owner);

                var entity = world.NewEntity();
                ballTag.Add(entity);
                rigidbodyPool.Add(entity).Rigidbody = go.GetComponent<Rigidbody>();
                
                if (ball.owner == TurnParticipant.Player)
                    playerPool.Add(entity);
                else 
                    aiPool.Add(entity);
            }

            ref var currentTurn = ref turnService.CurrentTurn();
            
            currentTurn.State = saveData.turnState;
            currentTurn.Participant = saveData.turnParticipant;
            currentTurn.PauseTime = saveData.turnPauseTime;
            
            ScoreService.Player = saveData.scorePlayer;
            ScoreService.AI = saveData.scoreAI;

            Debug.Log("Game Loaded");
        }
    }

}