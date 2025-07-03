using Components;
using Game.Configs;
using Leopotam.EcsLite;
using UnityEngine;
using Views;

namespace Systems
{
    public class DrawDraggedDirectionSystem : IEcsRunSystem
    {
        private readonly DraggedDirectionView _view;
        private bool _isDragging;

        public DrawDraggedDirectionSystem(GameConfig config)
        {
            _view = Object.Instantiate(config.draggedDirectionViewPrefab);
            _view.gameObject.SetActive(false);
        }
        
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<DraggedComponent>().Inc<RigidbodyRef>().End();
            var draggedPool = world.GetPool<DraggedComponent>();
            var rigidbodyPool = world.GetPool<RigidbodyRef>();
            
            _isDragging = false;
            
            foreach (var e in filter)
            {
                _isDragging = true;
                
                var startPosition = rigidbodyPool.Get(e).Rigidbody.position - draggedPool.Get(e).Direction;
                var endPosition = rigidbodyPool.Get(e).Rigidbody.position + draggedPool.Get(e).Direction;
                
                if (!_view.gameObject.activeSelf) _view.gameObject.SetActive(true);
                        
                _view.Draw(startPosition, endPosition);
                
                break;
            }

            if (!_isDragging && _view.gameObject.activeSelf) _view.gameObject.SetActive(false);
        }
    }
}