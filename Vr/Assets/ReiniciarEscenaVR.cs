using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReiniciarEscenaVR : MonoBehaviour
{
    // Evita que el botón intente reiniciar varias veces
    // si se presiona repetidamente en pocos instantes.
    private bool cargandoEscena = false;

    // Esta función será llamada desde el evento del botón XR.
    public void ReiniciarEscena()
    {
        // Si ya se está cargando la escena, no hacemos nada.
        if (cargandoEscena)
        {
            return;
        }

        // Iniciamos la corrutina para recargar la escena.
        StartCoroutine(RecargarEscenaActual());
    }

    private IEnumerator RecargarEscenaActual()
    {
        // Activamos el seguro para evitar dobles clics.
        cargandoEscena = true;

        // Obtenemos la escena que se está ejecutando actualmente.
        Scene escenaActual = SceneManager.GetActiveScene();

        // Si la escena no está agregada a Build Settings,
        // Unity no podrá cargarla mediante su Build Index.
        if (escenaActual.buildIndex < 0)
        {
            Debug.LogError(
                "La escena actual no está agregada a Build Settings. " +
                "Ve a File > Build Settings > Add Open Scenes."
            );

            cargandoEscena = false;
            yield break;
        }

        // Recarga la escena actual.
        // LoadSceneMode.Single reemplaza la escena actual por una nueva copia.
        yield return SceneManager.LoadSceneAsync(
            escenaActual.buildIndex,
            LoadSceneMode.Single
        );
    }
}