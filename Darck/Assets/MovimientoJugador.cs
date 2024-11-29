using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovimientoJugador : MonoBehaviour
{
    //Mover el mono
    private Rigidbody2D rigidbody2;

    [Header("Movimiento")]
    private float movimientoHorizontal = 0f;
    [SerializeField] private float velocidadDeMovimiento;
    [Range(0, 0.3f)][SerializeField] private float suavizadoDeMovimiento;
    private Vector3 velocidad = Vector3.zero;
    private bool mirandoDerecha = true;

    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private bool enSuelo;

    private bool salto = false;

    [Header("Dash")]
    [SerializeField] private float fuerzaDash = 20f;
    [SerializeField] private float duracionDash = 0.2f;
    [SerializeField] private float tiempoRecargaDash = 1f;
    private bool puedeHacerDash = true;
    private bool estaHaciendoDash = false;

    [Header("Animacion")]
    private Animator animator;

    [Header("Poderes")]
    [SerializeField] private GameObject poderRayoNegro;
    [SerializeField] private GameObject poderRayo;
    [SerializeField] private GameObject poderDestello;
    [SerializeField] private GameObject poderFuego;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float velocidadProyectil;
    [SerializeField] private float tiempoVidaProyectil = 2f; // Tiempo de vida de los proyectiles

    [Header("Maná")]
    public Slider manaBar;           // Barra de maná (UI)
    public float mana = 100f;        // Maná actual
    public float maxMana = 100f;     // Maná máximo
    public float manaCost = 20f;     // Costo de maná por poder
    private float attackCooldown = 1.0f; // Cooldown duration in seconds
    private float lastAttackTime = 0f;

    private void Start()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator not found on this GameObject.");
        }

        // Configuración inicial de la barra de maná
        if (manaBar != null)
        {
            manaBar.maxValue = maxMana;
            manaBar.value = mana;
        }
    }

    private void Update()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal") * velocidadDeMovimiento;

        animator.SetFloat("Horizontal", Mathf.Abs(movimientoHorizontal));

        if (Input.GetButtonDown("Jump"))
        {
            salto = true;
        }

        if (Input.GetButtonDown("Fire2") && puedeHacerDash) // Usa Fire2 para el Dash (configurable en Input Manager)
        {
            StartCoroutine(HacerDash());
        }

        Attack();
        LanzarPoderes();
    }

    private void FixedUpdate()
    {
        if (controladorSuelo == null)
        {
            Debug.LogError("controladorSuelo is not assigned.");
            return;
        }

        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, queEsSuelo);
        animator.SetBool("enSuelo", enSuelo);

        if (!estaHaciendoDash)
        {
            Mover(movimientoHorizontal * Time.fixedDeltaTime, salto);
        }
        salto = false;
    }

    public void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // Ensure the player is on the ground before attacking
            if (!enSuelo)
            {
                Debug.Log("Player cannot attack while in the air.");
                return; // Exit the method if the player is not grounded
            }

            // Check if the attack is off cooldown
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // Update last attack time
                lastAttackTime = Time.time;

                // Play swing sound if AudioManager exists
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlayAudio(AudioManager.instance.swing);
                }
                else
                {
                    Debug.LogWarning("AudioManager instance is missing.");
                }

                // Trigger attack animation if animator exists
                if (animator != null)
                {
                    animator.SetBool("Attack", true);
                    StartCoroutine(ResetAttackAnimation());
                }
            }
        }
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.1f); // Adjust duration to match the animation
        if (animator != null)
        {
            animator.SetBool("Attack", false);
        }
    }

    private void LanzarPoderes()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Rayo Negro
        {
            LanzarProyectil(poderRayoNegro, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // Rayo
        {
            LanzarProyectil(poderRayo, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // Destello
        {
            LanzarProyectil(poderDestello, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // Poder de Fuego
        {
            LanzarProyectil(poderFuego, false); // No se mueve
        }
    }

    private void LanzarProyectil(GameObject poder, bool esMovil)
    {
        // Verificar si hay suficiente maná
        if (mana < manaCost)
        {
            Debug.Log("No tienes suficiente maná para lanzar este poder.");
            return; // Salir si no hay suficiente maná
        }

        // Reducir el maná y actualizar la barra
        mana -= manaCost;
        if (manaBar != null)
        {
            manaBar.value = mana;
        }

        // Instanciar el proyectil
        GameObject proyectil = Instantiate(poder, puntoDisparo.position, Quaternion.identity);

        // Configurar si el proyectil es el poder de fuego
        Proyectil scriptProyectil = proyectil.GetComponent<Proyectil>();
        if (scriptProyectil != null && poder == poderFuego)
        {
            scriptProyectil.esPoderFuego = true; // Marcar como poder de fuego
        }

        if (esMovil)
        {
            Rigidbody2D rb = proyectil.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float direccion = mirandoDerecha ? 1 : -1;
                rb.velocity = new Vector2(direccion * velocidadProyectil, 0);
            }

            if (!mirandoDerecha)
            {
                proyectil.transform.localScale = new Vector3(
                    -Mathf.Abs(proyectil.transform.localScale.x),
                    proyectil.transform.localScale.y,
                    proyectil.transform.localScale.z
                );
            }

            Destroy(proyectil, tiempoVidaProyectil); // Destruir el proyectil después de un tiempo
        }
        else
        {
            // Configurar escala constante para el poder de fuego
            proyectil.transform.localScale = new Vector3(3.47f, 3.47f, 1f);

            // Ajustar rotación y escala según la dirección
            if (!mirandoDerecha)
            {
                proyectil.transform.localScale = new Vector3(
                    -Mathf.Abs(proyectil.transform.localScale.x), // Invierte horizontalmente
                    proyectil.transform.localScale.y,
                    proyectil.transform.localScale.z
                );
            }

            // Mantener rotación horizontal
            proyectil.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void Mover(float mover, bool saltar)
    {
        Vector3 velocidadObjetivo = new Vector2(mover, rigidbody2.velocity.y);
        rigidbody2.velocity = Vector3.SmoothDamp(rigidbody2.velocity, velocidadObjetivo, ref velocidad, suavizadoDeMovimiento);

        if (mover > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (mover < 0 && mirandoDerecha)
        {
            Girar();
        }

        if (enSuelo && saltar)
        {
            enSuelo = false;
            rigidbody2.velocity = new Vector2(rigidbody2.velocity.x, 0f);
            rigidbody2.AddForce(new Vector2(0f, fuerzaDeSalto), ForceMode2D.Impulse);
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private IEnumerator HacerDash()
    {
        if (!puedeHacerDash) yield break;

        puedeHacerDash = false;
        estaHaciendoDash = true;

        // Activar animación de *dash*
        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }

        // Calcular la dirección del *dash* basado en la dirección actual
        float direccion = mirandoDerecha ? 1 : -1;

        // Guardar la velocidad original
        Vector2 velocidadOriginal = rigidbody2.velocity;

        // Aplicar la fuerza del *dash* sin perder la velocidad horizontal actual
        rigidbody2.velocity = new Vector2(direccion * fuerzaDash, rigidbody2.velocity.y);

        // Ignorar colisiones temporalmente (cambia según tu configuración de capas)
        Physics2D.IgnoreLayerCollision(7, 10, true);

        // Esperar la duración del *dash*
        yield return new WaitForSeconds(duracionDash);

        // Restaurar colisiones
        Physics2D.IgnoreLayerCollision(7, 10, false);

        // Restaurar la velocidad original después del *dash*
        rigidbody2.velocity = new Vector2(velocidadOriginal.x, rigidbody2.velocity.y);

        estaHaciendoDash = false;

        // Tiempo de recarga antes de que el *dash* esté disponible nuevamente
        yield return new WaitForSeconds(tiempoRecargaDash);
        puedeHacerDash = true;
    }

}
