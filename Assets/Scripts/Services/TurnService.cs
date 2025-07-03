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
            _world.GetPool<TurnComponent>().Add(turnEntity);
        }
        
        public ref TurnComponent CurrentTurn()
        {
            var e = _world.Filter<TurnComponent>().End().GetRawEntities()[0];
            return ref _world.GetPool<TurnComponent>().Get(e);
        }
    }
}