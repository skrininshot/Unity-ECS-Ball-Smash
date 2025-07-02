using System;
using Components;
using Game.Configs;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems
{
    public class TurnControlSystem : IEcsPostRunSystem
    {
        private float _pauseTimer = 0;
        private readonly GameConfig _config;
        
        public TurnControlSystem(GameConfig config)
        {
            _config = config;
        }
        
        public void PostRun(IEcsSystems systems) 
        {
            var world = systems.GetWorld();
            var turnPool = world.GetPool<TurnComponent>();
            var requestPool = world.GetPool<EndTurnStateRequest>();

            
            
            foreach (var entity in world.Filter<TurnComponent>().End())
            {
                ref var turn = ref turnPool.Get(entity);
                
                foreach (var request in world.Filter<EndTurnStateRequest>().End()) 
                {
                    if (turn.State == TurnState.Move)
                    {
                        turn.State = TurnState.Pause;
                        _pauseTimer += _config.pauseBetweenTurns;
                    }
                    
                    foreach (var e in world.Filter<EndTurnStateRequest>().End())
                        requestPool.Del(e);
                }
                
                if (turn.State == TurnState.Pause)
                {
                    if (_pauseTimer > 0)
                    {
                        _pauseTimer -= Time.deltaTime;

                        if (_pauseTimer <= 0)
                        {
                            turn.State = TurnState.Move;
                            turn.Participant = turn.Participant == TurnParticipant.Player ?
                                TurnParticipant.AI : TurnParticipant.Player;
                            
                            Debug.Log("Move");
                        }
                        
                        return;
                    }
                }
            }
            
            // var world = systems.GetWorld();
            // var turnPool = world.GetPool<TurnStateComponent>();
            // var requestPool = world.GetPool<EndTurnStateRequest>();
            //
            // foreach (var e in world.Filter<EndTurnStateRequest>().End()) 
            // {
            //     foreach (var entity in world.Filter<TurnStateComponent>().End()) 
            //     {
            //         ref var turn = ref turnPool.Get(entity);
            //         turn.State = NextEnumValue(turn.State);
            //         _pauseTimer = _config.pauseBetweenTurns;
            //     }
            //     
            //     requestPool.Del(e);
            // }
        }
    }
}