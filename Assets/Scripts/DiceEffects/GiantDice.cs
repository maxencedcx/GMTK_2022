using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantDice : SizeModifier
{
    //public override void Revert()
    //{
    //    base.Revert();
    //    _rigidbody.AddTorque(new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360)) * 10);
    //}


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GiantDice))]
    public class GiantDiceEditor : SizeModifierEditor
    {
    }
#endif
}
