







using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ToggelEvent : UnityEvent<bool>{}

public class Player : NetworkBehaviour
{
    [SyncVar(hook = "OnNameChanged")]
    public string playerName;
    [SyncVar(hook = "OnColorChanged")]
    public Color playerColor;


    [SerializeField]
    ToggelEvent onToggleShared;
    [SerializeField]
    ToggelEvent onToggleLocal;
    [SerializeField]
    ToggelEvent onToggleRemote;
    [SerializeField]
    float respawnTime = 5f;

    static List<Player> players = new List<Player>();

    GameObject mainCamera;

    private GameObject cube;

    void Start()
    {
        mainCamera = Camera.main.gameObject;

        EnablePlayer();


        cube = GameObject.Find("Cube");
        if (cube != null)
            print("NotNull");
    }

    // Only the server can add itself
    [ServerCallback]
    void OnEnable()
    {
        if (!players.Contains(this))
            players.Add(this);
    }

    [ServerCallback]
    void OnDisable()
    {
        if (players.Contains(this))
            players.Remove(this);
    }


    public override void OnStartClient()
    {
        OnNameChanged(playerName);
        OnColorChanged(playerColor);
    }

    void DisablePlayer()
    {
        if (isLocalPlayer)
        {
            mainCamera.SetActive(true);

        }

        onToggleShared.Invoke(false);

        if (isLocalPlayer)
        {
            onToggleLocal.Invoke(false);
        }
        else
        {
            onToggleRemote.Invoke(false);
        }
    }

    void EnablePlayer()
    {
        if (isLocalPlayer)
        {
            mainCamera.SetActive(false);
        }

        onToggleShared.Invoke(true);

        if(isLocalPlayer)
        {
            onToggleLocal.Invoke(true);
        }
        else
        {
            onToggleRemote.Invoke(true);
        }
    }

    public void Die()
    {

        DisablePlayer();

        Invoke("Respawn", respawnTime);
    }

    void Respawn()
    {
        if(isLocalPlayer)
        {
            Transform spawn = NetworkManager.singleton.GetStartPosition();
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }

        EnablePlayer();
    }

    void OnNameChanged(string value)
    {
        playerName = value;
        gameObject.name = playerName;
        GetComponentInChildren<Text>(true).text = playerName;
    }

    void OnColorChanged(Color value)
    {
        playerColor = value;
        GetComponentInChildren<RendererToggler>().ChangeColor(playerColor);
    }

    [Server]
    public void Won()
    {
        for(int i = 0; i < players.Count; i++)
        {
            players[i].RpcGameOver(netId, name);
        }

        //go to Lobby
        Invoke("BackToLobby", 5f);
    }

    [ClientRpc]
    void RpcGameOver(NetworkInstanceId networkID, string name)
    {
        // id is unique for each instance
        DisablePlayer();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(isLocalPlayer)
        {
            if (netId == networkID)
                print("YOU WON");
            else
                print("YOU LOST" + name + "WON");
        }

    }

    void BackToLobby()
    {
        FindObjectOfType<NetworkLobbyManager>().SendReturnToLobby();
    }


    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKey(KeyCode.J))
            {
                Cmd_AssignLocalAuthority(cube);
                //cube.GetComponent<ObjectPositionSync>().Cmd_AssignLocalAuthority(cube);
            }

            if (Input.GetKey(KeyCode.K))
            {
                Cmd_RemoveLocalAuthority(cube);
                //cube.GetComponent<ObjectPositionSync>().Cmd_RemoveLocalAuthority(cube);
            }
        }
    }

    [Command]
    void Cmd_AssignLocalAuthority(GameObject obj)
    {
        NetworkInstanceId nIns = obj.GetComponent<NetworkIdentity>().netId;
        GameObject client = NetworkServer.FindLocalObject(nIns);
        NetworkIdentity ni = client.GetComponent<NetworkIdentity>();
        ni.AssignClientAuthority(connectionToClient);
    }

    [Command]
    void Cmd_RemoveLocalAuthority(GameObject obj)
    {
        NetworkInstanceId nIns = obj.GetComponent<NetworkIdentity>().netId;
        GameObject client = NetworkServer.FindLocalObject(nIns);
        NetworkIdentity ni = client.GetComponent<NetworkIdentity>();
        ni.RemoveClientAuthority(ni.clientAuthorityOwner);
    }

    
}
