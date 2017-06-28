using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

    [SerializeField]
    int maxHealth = 3;

    // Only the server can set the value of a SyncVar
    // if the health is changed on player 1, all of player 1 health changes/synced
    [SyncVar (hook = "OnHealthChanged")] int health;

    Player player;
    

    void Awake()
    {
        player = GetComponent<Player>();
    }

    //This method can only run on the server and nowhere else
    [ServerCallback]
    void OnEnable()
    {
        health = maxHealth;
    }

    [ServerCallback]
    void Start()
    {
        health = maxHealth;
    }

    //Only the server can run this method. if client runs it, it will do nothing and throw a warning
    [Server]
    public bool TakeDamage()
    {
        bool died = false;

        if (health <= 0)
            return died;

        health--;
        died = health <= 0;

        RpcTakeDamage(died);

        return died;
    }

    [ClientRpc]
    void RpcTakeDamage(bool died)
    {
        if (died)
            player.Die();
    }
    //When my health changes, call this method
    void OnHealthChanged(int value)
    {
        // Creating a callback/hook
        // when the server gives you a new value

        health = value;
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.Sethealth(value);
        }
    } 
}
