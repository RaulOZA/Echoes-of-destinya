using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActivate : MonoBehaviour
{
    public GameObject bossGo; // Objeto del jefe
    public float bossAppearanceDuration = 5.0f; // Tiempo de animación o retraso antes de que aparezca el jefe
    public float playerDelayBeforeMoving = 2.0f; // Tiempo que el jugador estará parado antes de poder moverse

    private void Start()
    {
        // Aseguramos que el jefe esté desactivado al inicio
        bossGo.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ActivateBossSequence(collision.gameObject));
        }
    }

    private IEnumerator ActivateBossSequence(GameObject player)
    {
        // Validar que el jugador tiene los componentes necesarios
        MovimientoJugador movimientoJugador = player.GetComponent<MovimientoJugador>();
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        Animator playerAnimator = player.GetComponent<Animator>();

        if (movimientoJugador == null || playerRb == null || playerAnimator == null)
        {
            Debug.LogWarning("El jugador no tiene todos los componentes necesarios. Revisa la configuración.");
            yield break;
        }

        // Desactivar el movimiento del jugador
        movimientoJugador.enabled = false;

        // Detener cualquier velocidad residual
        playerRb.velocity = Vector2.zero;

        // Detener animaciones de movimiento del jugador
        playerAnimator.SetFloat("Horizontal", 0);
        playerAnimator.SetBool("enSuelo", true); // Asegurar animación de reposo

        // Activar la interfaz de usuario del jefe (opcional)
        BossUI.instance?.BossActivator();

        // Retraso antes de mostrar al jefe
        yield return new WaitForSeconds(playerDelayBeforeMoving);

        // Activar el jefe
        bossGo.SetActive(true);

        // Forzar la escala correcta del jefe
        bossGo.transform.localScale = new Vector3(8.29f, 8.29f, 1.03625f);
        Debug.Log($"Escala del jefe después de activarlo: {bossGo.transform.localScale}");

        // Esperar la duración de la animación del jefe (si tiene animación)
        Animator bossAnimator = bossGo.GetComponent<Animator>();
        if (bossAnimator != null)
        {
            bossAppearanceDuration = bossAnimator.GetCurrentAnimatorStateInfo(0).length;
        }
        yield return new WaitForSeconds(bossAppearanceDuration);

        // Reactivar el movimiento del jugador
        movimientoJugador.enabled = true;

        // Destruir el objeto del trigger para evitar múltiples activaciones
        Destroy(gameObject);
    }



}
