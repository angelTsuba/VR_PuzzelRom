using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSimpleInteractable))]
public class DoorPokeButton : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FnafDoorController door;
    [SerializeField] private Transform visual;
    [SerializeField] private Renderer visualRenderer;

    [Header("Animación del botón")]
    [Tooltip("Qué tanto baja visualmente el botón al presionarlo.")]
    [SerializeField] private float pressDistance = 0.025f;

    [Tooltip("Velocidad con la que el botón baja y regresa.")]
    [SerializeField] private float movementSpeed = 0.45f;

    [Header("Colores según el estado de la puerta")]
    [SerializeField] private Color closedDoorColor = Color.red;
    [SerializeField] private Color openDoorColor = Color.green;

    private XRSimpleInteractable interactable;

    // Posición del visual cuando no se presiona.
    private Vector3 releasedLocalPosition;

    // Posición del visual cuando se presiona.
    private Vector3 pressedLocalPosition;

    // Indica si el control está presionando actualmente el botón.
    private bool isPressed;

    private void Awake()
    {
        // Busca el XR Simple Interactable en este mismo objeto.
        interactable = GetComponent<XRSimpleInteractable>();

        if (visual == null)
        {
            Debug.LogError("No asignaste el objeto Visual en DoorPokeButton.");
            return;
        }

        // Guardamos la posición inicial del cubo visible.
        releasedLocalPosition = visual.localPosition;

        // IMPORTANTE:
        // Vector3.down hace que el botón se mueva hacia abajo
        // usando el eje Y LOCAL del objeto Visual.
        pressedLocalPosition = releasedLocalPosition +
                               Vector3.down * pressDistance;
    }

    private void OnEnable()
    {
        // Evento cuando el botón recibe una presión válida.
        interactable.selectEntered.AddListener(OnButtonPressed);

        // Evento cuando el control deja de presionar.
        interactable.selectExited.AddListener(OnButtonReleased);

        // Escuchamos los cambios de estado de la puerta
        // para actualizar el color del botón.
        if (door != null)
        {
            door.DoorStateChanged += UpdateButtonColor;
        }
    }

    private void Start()
    {
        if (door == null)
        {
            Debug.LogError("No asignaste DoorSystem en el campo Door.");
            return;
        }

        // Al iniciar, el color se adapta al estado inicial de la puerta.
        UpdateButtonColor(door.IsClosed);
    }

    private void OnDisable()
    {
        // Quitamos los eventos al desactivar el objeto.
        interactable.selectEntered.RemoveListener(OnButtonPressed);
        interactable.selectExited.RemoveListener(OnButtonReleased);

        if (door != null)
        {
            door.DoorStateChanged -= UpdateButtonColor;
        }
    }

    private void Update()
    {
        if (visual == null)
        {
            return;
        }

        // Elegimos la posición que debe tener el cubo visual.
        Vector3 targetPosition = isPressed
            ? pressedLocalPosition
            : releasedLocalPosition;

        // Mueve el botón suavemente hacia abajo o de regreso hacia arriba.
        visual.localPosition = Vector3.MoveTowards(
            visual.localPosition,
            targetPosition,
            movementSpeed * Time.deltaTime
        );
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        // Solo permitimos la interacción proveniente de un Poke Interactor.
        if (!(args.interactorObject is XRPokeInteractor))
        {
            return;
        }

        // Hace que el visual baje.
        isPressed = true;

        if (door != null)
        {
            // Alterna la puerta entre abierta y cerrada.
            door.ToggleDoor();
        }
    }

    private void OnButtonReleased(SelectExitEventArgs args)
    {
        // Solo respondemos si quien soltó fue un Poke Interactor.
        if (!(args.interactorObject is XRPokeInteractor))
        {
            return;
        }

        // El visual vuelve a subir cuando retiras el control.
        isPressed = false;
    }

    private void UpdateButtonColor(bool doorIsClosed)
    {
        if (visualRenderer == null)
        {
            Debug.LogWarning("No asignaste Visual Renderer en DoorPokeButton.");
            return;
        }

        // Rojo = puerta cerrada.
        // Verde = puerta abierta.
        Color targetColor = doorIsClosed
            ? closedDoorColor
            : openDoorColor;

        Material material = visualRenderer.material;

        // URP Lit normalmente usa _BaseColor.
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", targetColor);
        }
        // Algunos materiales antiguos usan _Color.
        else if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", targetColor);
        }
    }
}

