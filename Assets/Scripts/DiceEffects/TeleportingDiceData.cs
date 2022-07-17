using UnityEngine;

[UnityEngine.CreateAssetMenu(fileName = "New Teleporting Dice Data", menuName = "GMTK/Teleporting Dice Data")]
public class TeleportingDiceData : ScriptableObject
{
    public bool NullifyMovement = false;

    public float ScaleTweenDuration = 0.2f;
    
    [UnityEngine.Range(0f, 1f)]
    public float Trauma = 0.5f;

    public RSLib.ParticlesSpawner ParticlesSpawnerOnTeleported;
    public RSLib.ParticlesSpawner ParticlesSpawnerOnPrepare;

    public RSLib.Audio.ClipProvider Clip;
}

