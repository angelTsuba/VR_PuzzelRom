using System;
using UnityEngine;

public class FnafDoorController : MonoBehaviour
{
    [Header("Referencias de la puerta")]
    [SerializeField] private Transform doorPanel;
    [SerializeField] private Transform closedPoint;
    [SerializeField] private Transform openPoint;

    [Header("Configuración")]
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private bool startClosed = false;

    // Permite que otros scripts sepan cuando la puerta cambió de estado.
    public event Action<bool> DoorStateChanged;

    private bool isClosed;

    // Otros scripts pueden consultar este dato sin modificarlo.
    public bool IsClosed
    {
        get { return isClosed; }
    }

    private void Start()
    {
        // Validamos que las referencias estén conectadas.
        if (doorPanel == null || closedPoint == null || openPoint == null)
        {
            Debug.LogError("Faltan referencias en FnafDoorController.");
            enabled = false;
            return;
        }

        // Definimos el estado inicial.
        isClosed = startClosed;

        // Colocamos la puerta de inmediato en su posición inicial.
        doorPanel.position = isClosed
            ? closedPoint.position
            : openPoint.position;
    }

    private void Update()
    {
        // Elegimos a qué punto debe moverse la puerta.
        Vector3 targetPosition = isClosed
            ? closedPoint.position
            : openPoint.position;

        // Movemos la puerta poco a poco.
        doorPanel.position = Vector3.MoveTowards(
            doorPanel.position,
            targetPosition,
            movementSpeed * Time.deltaTime
        );
    }

    // Alterna entre puerta abierta y cerrada.
    public void ToggleDoor()
    {
        SetDoorClosed(!isClosed);
    }

    public void CloseDoor()
    {
        SetDoorClosed(true);
    }

    public void OpenDoor()
    {
        SetDoorClosed(false);
    }

    private void SetDoorClosed(bool newState)
    {
        // Evita enviar eventos innecesarios si ya estaba en ese estado.
        if (isClosed == newState)
        {
            return;
        }

        isClosed = newState;

        // Avisamos al botón que debe actualizar su color.
        DoorStateChanged?.Invoke(isClosed);

        Debug.Log(isClosed
            ? "Puerta cerrándose."
            : "Puerta abriéndose.");
    }
}