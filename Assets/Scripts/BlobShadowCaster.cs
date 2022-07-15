using RSLib.Extensions;
using UnityEngine;

public class BlobShadowCaster : MonoBehaviour
{
    [SerializeField]
    private Transform _blobShadow = null;

    [SerializeField]
    private float _additionalHeight = 0.01f;

    [SerializeField]
    private LayerMask _layerMask = 0;
    
    private void Update()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, this._layerMask))
        {
            this._blobShadow.gameObject.SetActive(true);
            this._blobShadow.position = hit.point.AddY(this._additionalHeight);
            this._blobShadow.up = hit.normal;
        }
        else
        {
            this._blobShadow.gameObject.SetActive(false);
        }
    }
}
