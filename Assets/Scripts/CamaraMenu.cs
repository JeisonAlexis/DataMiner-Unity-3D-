using UnityEngine;

public class CamaraMenu : MonoBehaviour
{
    // Sensibilidad y límites de rotación
    private float velocidadRotacion = 4f; // Sensibilidad de la rotación
    private float rotacionMaxX = 5f; // Ángulo máximo en el eje X
    private float rotacionMaxY = 7f; // Ángulo máximo en el eje Y

    private Vector3 rotacionInicial; // Guarda la rotación inicial del objeto

    void Start()
    {
        // Se guarda la rotación inicial del objeto en forma de Euler
        rotacionInicial = transform.rotation.eulerAngles;
    }

    void Update()
    {
        // Obtener la posición del mouse en la pantalla, normalizada de -1 a 1
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Limitar los valores del mouse entre -1 y 1
        mouseX = Mathf.Clamp(mouseX, -1f, 1f);
        mouseY = Mathf.Clamp(mouseY, -1f, 1f);

        // Calcular la rotación relativa basada en la posición del mouse
        float rotacionX = rotacionMaxX * -mouseY;
        float rotacionY = rotacionMaxY * mouseX;

        // Sumar la rotación inicial para obtener la rotación final deseada
        float rotacionFinalX = rotacionInicial.x + rotacionX;
        float rotacionFinalY = rotacionInicial.y + rotacionY;

        // Aplicar límites a la rotación final para no exceder los valores permitidos
        rotacionFinalX = Mathf.Clamp(rotacionFinalX, rotacionInicial.x - rotacionMaxX, rotacionInicial.x + rotacionMaxX);
        rotacionFinalY = Mathf.Clamp(rotacionFinalY, rotacionInicial.y - rotacionMaxY, rotacionInicial.y + rotacionMaxY);

        // Crear la rotación final como un Quaternion
        Quaternion rotacionObjetivo = Quaternion.Euler(rotacionFinalX, rotacionFinalY, rotacionInicial.z);

        // Suavizar la transición hacia la rotación objetivo
        transform.rotation = Quaternion.Lerp(transform.rotation, rotacionObjetivo, Time.deltaTime * velocidadRotacion);
    }
}

