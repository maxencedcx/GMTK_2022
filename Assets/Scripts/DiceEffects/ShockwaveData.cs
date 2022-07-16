[UnityEngine.CreateAssetMenu(fileName = "New Shockwave Data", menuName = "GMTK/Shockwave Data")]
public class ShockwaveData : UnityEngine.ScriptableObject
{
    public float Range;
    public UnityEngine.Vector2 ForceMinMax;
        
    [UnityEngine.RangeAttribute(0f, 1f)]
    public float Trauma;

    public RSLib.ParticlesSpawner ParticlesSpawner;
}