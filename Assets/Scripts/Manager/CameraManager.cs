using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    using System.Linq;

    public class CameraManager : RSLib.Singleton<CameraManager>
    {
        private readonly HashSet<Transform> CameraTargets = new();

        [SerializeField]
        private float _smoothTime = 0.5f;

        [SerializeField]
        private Vector2 _xPositionBounds;

        [SerializeField]
        private Vector2 _zoomBounds;

        [SerializeField]
        private float _zoomlimiter;
        
        [SerializeField]
        private Camera _camera;

        private Vector3 _velocity;

        public void RegisterTarget(Transform target)
        {
            this.CameraTargets.Add(target);
        }

        public void UnregisterTarget(Transform target)
        {
            this.CameraTargets.Remove(target);
        }

        private void LateUpdate()
        {
            // MOVE
            Vector3 centerPoint = this.GetCenterPoint();
            Vector3 currentPosition = this.transform.position;
            Vector3 newPosition = currentPosition;
            newPosition.x = Mathf.Clamp(centerPoint.x, this._xPositionBounds.x, this._xPositionBounds.y);
            this.transform.position = Vector3.SmoothDamp(currentPosition, newPosition, ref this._velocity, this._smoothTime);
            
            // ZOOM
            float newZoom = Mathf.Lerp(this._zoomBounds.x, this._zoomBounds.y, this.GetGreatestDistance() / this._zoomlimiter);
            this._camera.fieldOfView = Mathf.Lerp(this._camera.fieldOfView, newZoom, Time.deltaTime);
        }

        private Vector3 GetCenterPoint()
        {
            if (this.CameraTargets.Count == 0)
            {
                return Vector3.zero;
            }

            
            Bounds bounds = new Bounds(this.CameraTargets.First().transform.position, Vector3.zero);

            foreach (Transform cameraTarget in this.CameraTargets)
            {
                bounds.Encapsulate(cameraTarget.position);
            }

            return bounds.center;
        }

        private float GetGreatestDistance()
        {
            if (this.CameraTargets.Count == 0)
            {
                return this._zoomBounds.y;
            }
            
            Bounds bounds = new Bounds(this.CameraTargets.First().transform.position, Vector3.zero);

            foreach (Transform cameraTarget in this.CameraTargets)
            {
                bounds.Encapsulate(cameraTarget.position);
            }

            return bounds.size.x;
        }
    }
}
