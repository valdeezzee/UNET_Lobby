using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerNetworkSetup : NetworkBehaviour {

    [SerializeField]
    Camera FPSCharacterCam;
    [SerializeField]
    AudioListener audioListener;

	// Use this for initialization
	void Start () {
		if(isLocalPlayer)
        {
            GameObject.Find("Scene Camera").SetActive(false);

            GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
            GetComponent<CharacterController>().enabled = true;
            FPSCharacterCam.enabled = true;
            audioListener.enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
