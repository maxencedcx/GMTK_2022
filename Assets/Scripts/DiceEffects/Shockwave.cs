using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [System.Serializable]
    public struct ShockwaveData
    {
        public float Range;
        public Vector2 ForceMinMax;
    }

    [SerializeField]
    private ShockwaveData _shockwaveData = new ShockwaveData();

    [SerializeField]
    private LayerMask _layerMask = 0;

    [SerializeField, Range(0f, 1f)]
    private float _shakeTrauma = 0.4f;

    [SerializeField]
    private GameObject _shockwaveView = null;

    private Collider[] _colliders;
    
    public void ApplyShockwave(Vector3 position, ShockwaveData shockwaveData)
    {
        int collidersCount = Physics.OverlapSphereNonAlloc(position, this._shockwaveData.Range, this._colliders, this._layerMask);

        if (collidersCount > 0)
        {
            for (int i = 0; i < collidersCount; ++i)
            {
                Collider target = _colliders[i];

                if (target.transform == this.transform)
                {
                    continue;
                }
                
                if (!target.TryGetComponent(out Rigidbody targetRigidbody))
                {
                    continue;
                }

                Vector3 targetPosition = target.transform.position;
                Vector3 direction = (targetPosition - position).normalized;
                float distance = Vector3.Distance(position, targetPosition);
                float force = RSLib.Maths.Maths.Normalize(distance, 0f, shockwaveData.Range, shockwaveData.ForceMinMax.y, shockwaveData.ForceMinMax.x);
                
                targetRigidbody.AddForce(direction * force, ForceMode.Impulse);
            }
        }

        if (this._shockwaveView != null)
        {
            Instantiate(this._shockwaveView, position, this._shockwaveView.transform.rotation);
        }
        
        FindObjectOfType<CameraShake>().SetTrauma(_shakeTrauma); // TODO: Remove FindObjectOfType.
    }
    
    private void Awake()
    {
        _colliders = new Collider[10]; // TODO: Replace with some static max number of players.
    }

    [ContextMenu("Apply Shockwave")]
    public void DebugApplyShockwave()
    {
        ApplyShockwave(transform.position, this._shockwaveData);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, this._shockwaveData.Range);
    }
    
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Shockwave))]
    public class ShockwaveEditor : RSLib.EditorUtilities.ButtonProviderEditor<Shockwave>
    {
        protected override void DrawButtons()
        {
            DrawButton("Apply", Obj.DebugApplyShockwave);
        }
    }
#endif
}
