using RSLib.EditorUtilities;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField]
    private DiceFace[] _diceFaces = null;

    private DiceFace highestFace;

    [ContextMenu("Get Highest Face")]
    private DiceFace GetHighestFace()
    {
        float highestPos = float.MinValue;
        
        for (int i = 0; i < _diceFaces.Length; i++)
        {
            float posY = _diceFaces[i].transform.position.y;

            if (posY > highestPos)
            {
                highestPos = posY;
                highestFace = _diceFaces[i];
            }
        }

        return highestFace;
    }
}
