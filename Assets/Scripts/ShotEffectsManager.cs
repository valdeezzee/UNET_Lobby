using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotEffectsManager : MonoBehaviour {

    [SerializeField]
    ParticleSystem muzzleFlash;
    [SerializeField]
    GameObject impactPrefab;

    //ParticleSystem impactEffect;

    public void Initialize()
    {
        //impactEffect = Instantiate(impactPrefab).GetComponent<ParticleSystem>();
    }

    public void PlayShotEffect()
    {
        muzzleFlash.Stop(true);
        muzzleFlash.Play(true);
    }
    /*
    public void PlayImpactEffect(Vector3 impactPosition)
    {
        impactEffect.transform.position = impactPosition;
        impactEffect.Stop();
        impactEffect.Play();
    }
    */
}
