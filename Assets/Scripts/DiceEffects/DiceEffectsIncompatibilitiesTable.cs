using UnityEngine;

[CreateAssetMenu(fileName = "New Dice Effects Incompatibilities Table", menuName = "GMTK/Incompatibilities Table")]
public class DiceEffectsIncompatibilitiesTable : ScriptableObject
{
    [System.Serializable]
    public struct Incompatibility
    {
        public DiceEffectType First;
        public DiceEffectType Second;
    }

    [SerializeField]
    private Incompatibility[] _incompatibilities = null;

    private System.Collections.Generic.Dictionary<DiceEffectType, System.Collections.Generic.List<DiceEffectType>> _incompatibilitiesTable;

    private System.Collections.Generic.Dictionary<DiceEffectType, System.Collections.Generic.List<DiceEffectType>> IncompatibilitiesTable
    {
        get
        {
            if (this._incompatibilitiesTable == null)
            {
                this._incompatibilitiesTable = new System.Collections.Generic.Dictionary<DiceEffectType, System.Collections.Generic.List<DiceEffectType>>();
                
                foreach (Incompatibility incompatibility in this._incompatibilities)
                {
                    if (!this._incompatibilitiesTable.ContainsKey(incompatibility.First))
                    {
                        this._incompatibilitiesTable.Add(incompatibility.First, new System.Collections.Generic.List<DiceEffectType>());
                    }
                    
                    if (!this._incompatibilitiesTable.ContainsKey(incompatibility.Second))
                    {
                        this._incompatibilitiesTable.Add(incompatibility.Second, new System.Collections.Generic.List<DiceEffectType>());   
                    }

                    if (!this._incompatibilitiesTable[incompatibility.First].Contains(incompatibility.Second))
                    {
                        this._incompatibilitiesTable[incompatibility.First].Add(incompatibility.Second);
                    }
                    
                    if (!this._incompatibilitiesTable[incompatibility.Second].Contains(incompatibility.First))
                    {
                        this._incompatibilitiesTable[incompatibility.Second].Add(incompatibility.First);
                    }
                }
            }

            return this._incompatibilitiesTable;
        }
    }

    public bool AreIncompatible(DiceEffectType first, DiceEffectType second)
    {
        return IncompatibilitiesTable.TryGetValue(first, out System.Collections.Generic.List<DiceEffectType> incompatibilities)
               && incompatibilities.Contains(second);
    }
}
