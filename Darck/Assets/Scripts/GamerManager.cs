using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamerManager : MonoBehaviour
{
    //Guardar textura
    public Renderer fondo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Mover el fondo
        fondo.material.mainTextureOffset = fondo.material.mainTextureOffset + new Vector2(0.015f, 0) * Time.deltaTime;
    }
}
