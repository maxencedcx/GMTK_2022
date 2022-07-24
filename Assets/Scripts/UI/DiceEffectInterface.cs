using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DiceEffectInterface : MonoBehaviour
{
    [System.Serializable]
    public struct DiceEffectSprite
    {
        public DiceEffectType EffectType;
        public Sprite Sprite;
    }

    [SerializeField]
    private DiceEffectSprite[] _diceEffectSprites = null;
    
    [SerializeField]
    private Image _full = null;

    [SerializeField]
    private Image _background = null;

    public DiceEffect DiceEffect { get; private set; }

    public bool IsOver => this.DiceEffect.IsOver;
    
    public void Init(DiceEffect diceEffect)
    {
        this.DiceEffect = diceEffect;

        Sprite sprite = _diceEffectSprites.FirstOrDefault(o => o.EffectType == diceEffect.EffectType).Sprite;
        this._full.sprite = sprite;
        this._background.sprite = sprite;
    }
    
    public void Refresh()
    {
        this._full.fillAmount = 1f - this.DiceEffect.LifetimePercentage;
    }
}
