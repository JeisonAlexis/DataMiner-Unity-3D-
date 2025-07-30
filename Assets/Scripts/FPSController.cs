using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    CharacterController controladorPersonaje;

    [Header("Opciones de Personaje")]
    public float velocidadCaminata = 6.0f;
    public float velocidadCarrera = 10.0f;
    public float velocidadSalto = 8.0f;
    public float gravedad = 20.0f;

    [Header("Opciones de Cámara")]
    public Camera camara;
    public float sensibilidadMouse = 3.0f;
    public float anguloVerticalMinimo = -90.0f;  // Permitir mirar hasta el suelo (-90°)
    public float anguloVerticalMaximo = 90.0f;    // Permitir mirar hacia arriba (90°)

    public bool bloquearCamara = false; // Controla si la cámara se mueve

    private float movimientoMouseX, movimientoMouseY;
    private Vector3 movimiento = Vector3.zero;
    private float rotacionVertical = 0f;  // Acumulador de la rotación vertical

    void OnEnable()
    {
        // Bloquear y ocultar el cursor cuando se activa este objeto
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        controladorPersonaje = GetComponent<CharacterController>();
        rotacionVertical = 0f;
        
        camara.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        
    }

    void Update()
    {
        if (!bloquearCamara)
        {
            // Rotación horizontal del personaje
            movimientoMouseX = sensibilidadMouse * Input.GetAxis("Mouse X");
            transform.Rotate(0, movimientoMouseX, 0);

            // Rotación vertical de la cámara
            movimientoMouseY = sensibilidadMouse * Input.GetAxis("Mouse Y");
            rotacionVertical -= movimientoMouseY;

            // Limitar la rotación vertical entre -90 y 90 grados segun las variables anguloVerticalMinimo y anguloVerticalMaximo
            rotacionVertical = Mathf.Clamp(rotacionVertical, anguloVerticalMinimo, anguloVerticalMaximo);
            camara.transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
        }

        
        if (controladorPersonaje.isGrounded) //isGrounded controla si esta en el suelo y es un metodo del componente CharacterController
        {
            movimiento = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            movimiento = transform.TransformDirection(movimiento) * (Input.GetKey(KeyCode.LeftShift) ? velocidadCarrera : velocidadCaminata);

            if (Input.GetKey(KeyCode.Space))
                movimiento.y = velocidadSalto;
        }

        movimiento.y -= gravedad * Time.deltaTime;
        controladorPersonaje.Move(movimiento * Time.deltaTime);

        // controlar el cursor, de momento solo sirve para el desarrollo del juego (es mas practico darle al pausa) en un futuro se quirá
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown(0) && !bloquearCamara)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
