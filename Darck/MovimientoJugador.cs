using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{

    //Mover el mono
    private Rigidbody2D rigidbody2;

    [Header("Movimiento")]
    //Velocidad de movimiento
    private float movimientoHorizontal = 0f;
    [SerializeField] private float velocidadDeMovimiento;
    [Range(0,0.3f)][SerializeField] private float suavizadoDeMovimiento;
    private Vector3 velocidad = Vector3.zero;
    private bool mirandoDerecha = true;

    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private bool enSuelo;

    private bool salto = false;

    [Header ("Animacion")]
    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator not found on this GameObject.");
        }
    }


    // Update is called once per frame
    private void Update()
    {
        //Movimiento con 1 para botones 
        movimientoHorizontal = Input.GetAxisRaw("Horizontal") * velocidadDeMovimiento;
        
        animator.SetFloat("Horizontal", Mathf.Abs(movimientoHorizontal));

        //Saltar con el boton
        if (Input.GetButtonDown("Jump"))
        {
            salto = true;
        }

        Attack();
    }

    //Cambio en las fisicas para que funcione en equipos potentes y no potentes
    private void FixedUpdate()
    {
        if (controladorSuelo == null)
        {
            Debug.LogError("controladorSuelo is not assigned.");
            return;
        }

        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, queEsSuelo);
        animator.SetBool("enSuelo", enSuelo);

        Mover(movimientoHorizontal * Time.fixedDeltaTime, salto);
        salto = false;
    }


    public void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayAudio(AudioManager.instance.swing);
            }
            else
            {
                Debug.LogWarning("AudioManager instance is missing.");
            }

            if (animator != null)
            {
                animator.SetBool("Attack", true);
            }
        }
        else if (animator != null)
        {
            animator.SetBool("Attack", false);
        }
    }


    //Suavizado a la hora de acelerar o frenar y luego a la velocidad que queremos llegar y que tan rapido
    private void Mover (float mover, bool saltar)
    {
        Vector3 velocidadObjetivo = new Vector2 (mover, rigidbody2.velocity.y);
        rigidbody2.velocity = Vector3.SmoothDamp(rigidbody2.velocity, velocidadObjetivo, ref velocidad, suavizadoDeMovimiento);

        //Para que el personaje volte a lado que estemos moviendo

        if (mover > 0 && !mirandoDerecha)
        {
            //Girar
            Girar();
        }
        else if (mover <  0 && mirandoDerecha) 
        {
            Girar();
        }

        if(enSuelo && saltar)
        {
            enSuelo = false;
            rigidbody2.AddForce(new Vector2(0f, fuerzaDeSalto));
        }
    }

    //Coloca solo la variable en el sentido contrario 
    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;

    }

    //Para la caja que creamos de salto en unity
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }
}
