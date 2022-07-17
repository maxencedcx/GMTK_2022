using UnityEngine;

[CreateAssetMenu(fileName = "New SizeModifier Data", menuName = "GMTK/SizeModifier Data")]
public class SizeModifierData : ScriptableObject
{
    [SerializeField, Min(0f)]
    private float _scale = 1f;

    [SerializeField, Min(0f)]
    private float _scaleDuration = 0.2f;

    public RSLib.Audio.ClipProvider Clip;
    
    public float Scale => _scale;
    public float ScaleDuration => _scaleDuration;
}