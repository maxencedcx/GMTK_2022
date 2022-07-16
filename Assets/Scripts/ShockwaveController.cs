using RSLib.Extensions;
using UnityEngine;

public class ShockwaveController
{
    public void Apply(Transform sourceTransform, ShockwaveData shockwaveData)
    {
        Vector3 sourcePosition = sourceTransform.position;

        Collider[] colliders = new Collider[20]; // TODO: Replace with some static max number of players.

        int collidersCount = Physics.OverlapSphereNonAlloc(sourcePosition, shockwaveData.Range, colliders, shockwaveData.LayerMask);
        if (collidersCount > 0)
        {
            for (int i = 0; i < collidersCount; ++i)
            {
                Collider target = colliders[i];

                if (target.transform == sourceTransform)
                {
                    continue;
                }

                if (!target.TryGetComponent(out Rigidbody targetRigidbody))
                {
                    continue;
                }

                Vector3 targetPosition = target.transform.position;
                Vector3 direction = (targetPosition - sourcePosition).normalized;
                float distance = Vector3.Distance(sourcePosition, targetPosition);
                float force = RSLib.Maths.Maths.Normalize(distance, 0f, shockwaveData.Range, shockwaveData.ForceMinMax.y, shockwaveData.ForceMinMax.x);

                targetRigidbody.AddForce(direction.WithZ(0f) * force, ForceMode.Impulse);
            }
        }
    }
}
