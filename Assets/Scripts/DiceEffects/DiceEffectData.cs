using UnityEngine;

/// <summary>
/// Common data to all dice effects.
/// </summary>
[UnityEngine.CreateAssetMenu(fileName = "DiceEffectData_New", menuName = "GMTK/Dice Effect Data")]
public class DiceEffectData : ScriptableObject
{
    [SerializeField, Min(0f)]
    private float _duration = 10f;

    [SerializeField]
    private RSLib.Optional<float> _autoApplyCooldown = new(1f, false);

    [SerializeField]
    private bool _applyOnLand = false;

    [SerializeField]
    private bool _applyInstantly = false;
    
    public float Duration => this._duration;

    public bool AutoApply => this._autoApplyCooldown.Enabled;
    public float Cooldown => this._autoApplyCooldown.Value;
    
    public bool ApplyOnLand => this._applyOnLand;
    public bool ApplyInstantly => this._applyInstantly;
}
