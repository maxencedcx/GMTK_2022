using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniDice : SizeModifier
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MiniDice))]
    public class MiniDiceEditor : SizeModifierEditor
    {
    }
#endif
}
