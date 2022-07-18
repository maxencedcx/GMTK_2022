using UnityEngine;

public class Galaxy : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer[] _galaxies = null;

    [SerializeField]
    private Vector2 _speed = Vector2.one;

    private Vector2 _offset;
    
    private void Update()
    {
        this._offset += _speed * Time.deltaTime;

        for (int i = 0; i < _galaxies.Length; i++)
        {
            this._galaxies[i].material.SetTextureOffset("_MainTex", this._offset);
        }
    }
}
