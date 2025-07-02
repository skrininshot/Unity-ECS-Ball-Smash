using Components;
using Leopotam.EcsLite;

namespace Services
{
    public class TurnService 
    {
        private readonly EcsWorld _world;

        public TurnService(EcsWorld world) 
        {
            _world = world;
            
            var turnEntity = _world.NewEntity();
            _world.GetPool<TurnComponent>().Add(turnEntity).Participant = 0;
        }

        public TurnState CurrentState() 
        {
            var e = _world.Filter<TurnComponent>().End().GetRawEntities()[0];
            return _world.GetPool<TurnComponent>().Get(e).State;
        }
        
        public TurnParticipant CurrentParticipant() 
        {
            var e = _world.Filter<TurnComponent>().End().GetRawEntities()[0];
            return _world.GetPool<TurnComponent>().Get(e).Participant;
        }
    }
}