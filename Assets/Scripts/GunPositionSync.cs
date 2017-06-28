using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class GunPositionSync : NetworkBehaviour {

    [SerializeField]
    Transform cameraTransform;
    [SerializeField]
    Transform handMount;
    [SerializeField]
    Transform gunPivot;
    [SerializeField]
    Transform rightHandHold;
    [SerializeField]
    Transform leftHandHold;
    [SerializeField]
    float threshold = 10f;
    [SerializeField]
    float smoothing = 5f;

    [SyncVar]
    float pitch;
    Vector3 lastOfsett;
    float lastSyncedPitch;

    void Start()
    {
        if (isLocalPlayer)
            gunPivot.parent = cameraTransform;
        else
            lastOfsett = handMount.position - transform.position;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            pitch = cameraTransform.localRotation.eulerAngles.x;//the the up and down position
            if (Mathf.Abs(lastSyncedPitch - pitch) >= threshold)
            {
                //Cmd
                CmdUpdatePitch(pitch);
                lastSyncedPitch = pitch;
            }
        }
        else
        {
            Quaternion newRotation = Quaternion.Euler(pitch, 0f, 0f);

            Vector3 currentOffset = handMount.position - transform.position;
            gunPivot.localPosition += currentOffset - lastOfsett;
            lastOfsett = currentOffset;

            gunPivot.localRotation = Quaternion.Lerp(gunPivot.localRotation, newRotation, Time.deltaTime * smoothing);
        }
    }

    [Command]
    void CmdUpdatePitch(float newPitch)
    {
        pitch = newPitch;
    }
}
