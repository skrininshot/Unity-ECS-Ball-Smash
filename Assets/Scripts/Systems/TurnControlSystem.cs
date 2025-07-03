using System;
using Components;
using Game.Configs;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems
{
    public class TurnControlSystem : IEcsPostRunSystem
    {
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
                        turn.PauseTime += _config.pauseBetweenTurns;
                    }
                    
                    foreach (var e in world.Filter<EndTurnStateRequest>().End())
                        requestPool.Del(e);
                }
                
                if (turn.State == TurnState.Pause)
                {
                    if (turn.PauseTime > 0)
                    {
                        turn.PauseTime -= Time.deltaTime;

                        if (turn.PauseTime <= 0)
                        {
                            turn.State = TurnState.Move;
                            turn.Participant = turn.Participant == TurnParticipant.Player ?
                                TurnParticipant.AI : TurnParticipant.Player;
                        }
                        
                        return;
                    }
                }
            }
        }
    }
}