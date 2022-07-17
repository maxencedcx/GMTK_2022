using UnityEngine;

public class DiceTeleportTargets : RSLib.Singleton<DiceTeleportTargets>
{
    [SerializeField]
    private Transform[] _targets = null;

    public Transform GetTarget()
    {
        return this._targets[Random.Range(0, this._targets.Length)];
    }
}
