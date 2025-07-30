using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_question : MonoBehaviour
{
    public GameObject QuestionUI;
    public GameObject interaccionUI;
    private bool show = false;
    private bool playerInTrigger = false; // Verifica si el jugador está en el trigger
    private bool answered = false;       // Para evitar respuestas múltiples

    void Start()
    {
        GameObject canvasGO = GameObject.Find("PlayerUICanvas"); // Busca el canvas específico
        if (canvasGO != null)
        {
            Transform preguntasTransform = canvasGO.transform.Find("Preguntas");
            if (preguntasTransform != null)
                QuestionUI = preguntasTransform.gameObject;

            Transform interaccion = canvasGO.transform.Find("Interaccion");
            if (interaccion != null)
                interaccionUI = interaccion.gameObject;
        }

        if (QuestionUI != null)
            QuestionUI.SetActive(false); // Asegura que esté apagado al inicio
    }

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            show = !show; // Alternar visibilidad

            if (QuestionUI != null)
            {
                QuestionUI.SetActive(show);
                Question_controller questionController = QuestionUI.GetComponent<Question_controller>();

                if (show) // Al abrir, preparamos para una nueva respuesta
                {
                    answered = false;
                    interaccionUI.SetActive(false);
                    if (questionController != null)
                        questionController.SetActivador(gameObject);
                }
                else // Cerrar con E cuenta como respuesta incorrecta
                {
                    if (questionController != null && !answered)
                    {
                        questionController.SeleccionarRespuesta(3);
                        answered = true;
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interaccionUI.SetActive(true);
            playerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Si la pregunta estaba abierta y aún no se respondió, marcamos incorrecta solo una vez
            if (QuestionUI != null && show && !answered)
            {
                Question_controller questionController = QuestionUI.GetComponent<Question_controller>();
                if (questionController != null)
                {
                    questionController.SeleccionarRespuesta(3);
                    answered = true;
                }
            }

            // Ocultamos UI y reseteamos flags
            interaccionUI.SetActive(false);
            playerInTrigger = false;
            show = false;
            QuestionUI.SetActive(false);
        }
    }
}
