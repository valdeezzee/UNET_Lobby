using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField]
    float shotCooldown = .3f;
    [SerializeField]
    int killsToWin = 5;
    [SerializeField]
    Transform firePosition;
    [SerializeField]
    ShotEffectsManager shotEffects;

    [SyncVar (hook = "OnScoreChanged")]
    int score;

    Player player;
    float ellapsedTime;
    bool canShoot;

    void Start()
    {
        player = GetComponent<Player>();
        shotEffects.Initialize();

        if (isLocalPlayer)
            canShoot = true;
    }
    // only the Server executes the method
    [ServerCallback]
    void OnEnable()
    {
        score = 0;
    }

    void Update()
    {
        if (!canShoot)
            return;

        ellapsedTime += Time.deltaTime;

        if(Input.GetButtonDown("Fire1") && ellapsedTime > shotCooldown)
        {
            ellapsedTime = 0f;
            //Cmd
            CmdFireShot(firePosition.position, firePosition.forward);
        }
    }
    //command attribute. run on the server. client tells the server to do something
    [Command]
    void CmdFireShot(Vector3 origin, Vector3 direction)
    {
        // Research capsule cast! all shape casts
        RaycastHit hit;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);

        bool result = Physics.Raycast(ray, out hit, 50f);

        if(result)
        {
            //health stuff
            PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();

            if (enemy != null)
            {
                bool wasKillShot = enemy.TakeDamage();
                if (wasKillShot && ++score >= killsToWin)
                    player.Won();
            }

        }

        RpcProcessShotEffects(result, hit.point);
    }

    //The server to tell all the clients to do something
    [ClientRpc]
    void RpcProcessShotEffects(bool playImpact, Vector3 point)
    {
        shotEffects.PlayShotEffect();

        // if you had shot impact effects
        /*
        if (playImpact)
            shotEffects.PlayImpactEffect(point);
        */
    }

    void OnScoreChanged(int value)
    {
        score = value;
        if (isLocalPlayer)
            PlayerCanvas.canvas.SetKills(value);        
    }

    public void FireAsBot()
    {
        CmdFireShot(firePosition.position, firePosition.forward);
    }
}
