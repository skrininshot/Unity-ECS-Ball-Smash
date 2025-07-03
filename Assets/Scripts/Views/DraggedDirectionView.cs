using UnityEngine;

namespace Views
{
    public class DraggedDirectionView : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform arrow;
    
        public void Draw(Vector3 from, Vector3 to)
        {
            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, to);
            arrow.position = to;
            
            var direction = to - from;
            if (direction != Vector3.zero)
                arrow.rotation = Quaternion.LookRotation(direction);
        }
    }
}
