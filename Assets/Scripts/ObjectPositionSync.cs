using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectPositionSync : NetworkBehaviour {

    [SyncVar]
    private Vector3 syncPos;

    [SerializeField]
    Transform myTransform;
    [SerializeField]
    float lerpRate = 15;

    public bool isMoving = true;
    // Reduce the number of time sending messages over the network
    void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();

    }

    void LerpPosition()
    {
        if (!hasAuthority)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
        }

    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if(hasAuthority)
            CmdProvidePositionToServer(myTransform.position);
        
    }


    [Command]
    public void Cmd_AssignLocalAuthority(GameObject obj)
    {
        NetworkInstanceId nIns = obj.GetComponent<NetworkIdentity>().netId;
        GameObject client = NetworkServer.FindLocalObject(nIns);
        NetworkIdentity ni = client.GetComponent<NetworkIdentity>();
        ni.AssignClientAuthority(connectionToClient);
    }

    [Command]
    public void Cmd_RemoveLocalAuthority(GameObject obj)
    {
        NetworkInstanceId nIns = obj.GetComponent<NetworkIdentity>().netId;
        GameObject client = NetworkServer.FindLocalObject(nIns);
        NetworkIdentity ni = client.GetComponent<NetworkIdentity>();
        ni.RemoveClientAuthority(ni.clientAuthorityOwner);
    }
}
