using System.Collections;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField]
    private ShockwaveData _shockwaveData = null;

    private SphereCollider _collider;

    private MeshRenderer _meshRenderer;

    [SerializeField]
    private float _waitForNewMine = 3f;

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Respawn()
    {
        _collider.enabled = true;
        _meshRenderer.enabled = true;
    }

    private void Explode()
    {
        ShockwaveController shockwaveController = new();
        shockwaveController.Apply(transform, _shockwaveData);
        ExplodeFeedback();

        Destroy();
        StartCoroutine(NewMine());
    }

    private void ExplodeFeedback()
    {
        _shockwaveData.ParticlesSpawner.SpawnParticles(gameObject.transform.position);
        Manager.GameManager.Instance.CameraShake?.SetTrauma(this._shockwaveData.Trauma);
    }

    private void Destroy()
    {
        _collider.enabled = false;
        _meshRenderer.enabled = false;
    }

    private IEnumerator NewMine()
    {
        yield return RSLib.Yield.SharedYields.WaitForSeconds(_waitForNewMine);
        Respawn();
    }
}
