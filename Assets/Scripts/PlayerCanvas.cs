using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour {
    //singleton
    public static PlayerCanvas canvas;

    [Header("Compenent References")]
    [SerializeField]
    Text healthValue;
    [SerializeField]
    Text killsValue;

    void Awake()
    {
        if (canvas == null)
            canvas = this;
        else if (canvas != this)
            Destroy(gameObject);
    }

    void Reset()
    {
        healthValue = GameObject.Find("HealthValue").GetComponent<Text>();
        killsValue = GameObject.Find("KillsValue").GetComponent<Text>();
    }

    public void SetKills(int amount)
    {
        killsValue.text = amount.ToString();
    }

    public void Sethealth(int amount)
    {
        healthValue.text = amount.ToString();
    }
}
