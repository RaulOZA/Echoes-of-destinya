using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patronia : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private Transform[] waypoints;

    private int currentWaypoint;
    private bool isWaiting;

    // Start is called before the first frame update
    void Start()
    {
        currentWaypoint = 0;  // Iniciar en el primer waypoint
    }

    // Update is called once per frame
    void Update()
    {
        // Solo moverse si no está esperando y la posición en X no ha sido alcanzada
        if (transform.position.x != waypoints[currentWaypoint].position.x)
        {
            // Moverse solo en el eje X, manteniendo la posición en Y
            Vector2 targetPosition = new Vector2(waypoints[currentWaypoint].position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else if (!isWaiting)
        {
            // Iniciar la espera cuando llegue al waypoint
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        isWaiting = true;  // Señalar que está esperando
        yield return new WaitForSeconds(waitTime);  // Esperar el tiempo definido
        currentWaypoint++;  // Moverse al siguiente waypoint

        // Volver al primer waypoint si hemos llegado al último
        if (currentWaypoint == waypoints.Length)
        {
            currentWaypoint = 0;
        }
        isWaiting = false;  // Terminar la espera
    }
}