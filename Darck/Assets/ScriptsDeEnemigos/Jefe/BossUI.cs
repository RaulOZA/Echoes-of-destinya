using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossUI : MonoBehaviour
{
    public GameObject boosPanel;
    public GameObject muros;

    public static BossUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        boosPanel.SetActive(false);
        muros.SetActive(false);

    }

    public void BossActivator()
    {
        boosPanel.SetActive(true); 
        muros.SetActive(true);
    }

    public void BossDeactivator()
    {
        boosPanel.SetActive(false);
        muros.SetActive(false);
    }
}
