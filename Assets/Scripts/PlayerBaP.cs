using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBaP : MonoBehaviour
{
    public float rango; //distancia maxima a la que puede poner o quitar bloques
    public LayerMask capaObjetivo; //la mascara o capa con la que vamos a interacturar
    public Image imagenBloque; //imagen individual del bloque
    public Sprite[] spritesBloque; //todas la imagenes de cada bloque

    private RaycastHit impacto; 
    private byte bloqueEnMano;  

    private GameObject UIPregunta;

    void Start()
    {
        bloqueEnMano = 1;
        imagenBloque.sprite = spritesBloque[bloqueEnMano - 1];

        // Buscar el canvas y luego el objeto "Preguntas" dentro del canvas
        GameObject canvasGO = GameObject.Find("PlayerUICanvas");
        if (canvasGO != null)
        {
            Transform preguntasTransform = canvasGO.transform.Find("Preguntas");
            if (preguntasTransform != null)
            {
                UIPregunta = preguntasTransform.gameObject;
            }
        }
    }

    void Update()
    {
        // Cambiar el bloque seleccionado con la rueda del mouse, omitiendo el bloque 5 porque en esta caso es la Bedrock "no se puede romper"
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (bloqueEnMano < 16)
                {
                    bloqueEnMano++;
                    if (bloqueEnMano == 5)
                    {
                        bloqueEnMano++;
                    }
                }
                else
                {
                    bloqueEnMano = 1;
                }
            }
            else
            {
                if (bloqueEnMano > 1)
                {
                    bloqueEnMano--;
                    if (bloqueEnMano == 5)
                    {
                        bloqueEnMano--;
                    }
                }
                else
                {
                    bloqueEnMano = 16;
                }
            }
        }

        imagenBloque.sprite = spritesBloque[bloqueEnMano - 1]; //setiar la imagen de la textura

        // Al hacer clic izquierdo, destruir el bloque (si no es bloque 5) si la UI de preguntas NO está activa
        if (Input.GetMouseButtonDown(0) && UIPregunta.activeSelf == false)
        {
            RealizarRaycast();

            if (impacto.collider != null)
            {
                Chunk chunk = impacto.collider.gameObject.GetComponent<Chunk>();

                // Calcular la posición del bloque relativa al chunk
                Vector3 posBloque = impacto.point - (impacto.normal / 2) - chunk.transform.position;

                posBloque.x = Mathf.Floor(posBloque.x);
                posBloque.y = Mathf.Floor(posBloque.y);
                posBloque.z = Mathf.Floor(posBloque.z);

                int x = Mathf.FloorToInt(posBloque.x);
                int y = Mathf.FloorToInt(posBloque.y);
                int z = Mathf.FloorToInt(posBloque.z);

                // Verificar que el bloque a destruir no sea el bloque 5
                if (chunk.GetBlock(x, y, z) != 5)
                {
                    chunk.SetBlock(new Vector3(x, y, z), 0);
                    chunk.GenerarMallaVisual();
                }
            }
        }
        else if (Input.GetMouseButtonDown(1)) // Al hacer clic derecho, colocar el bloque seleccionado
        {
            RealizarRaycast();

            if (impacto.collider != null)
            {
                // Calcular la posición donde se colocará el bloque
                Vector3 posMundo = impacto.point + (impacto.normal / 2);
                Chunk chunk = World.actual.BuscarChunk(posMundo);

                // Definir la distancia mínima (en el plano XZ) para evitar quedar atrapado
                float distanciaMinima = 0.9f;
                // Umbral de altura para considerar que el bloque se coloca "al mismo nivel" del jugador
                float alturaIgual = 1.2f;

                // Calcular la distancia horizontal (ignorando la componente Y)
                float distanciaHorizontal = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z),new Vector2(posMundo.x, posMundo.z));

                // Si el bloque se va a colocar muy cerca horizontalmente y a la misma altura, no permitirlo
                if (distanciaHorizontal < distanciaMinima && Mathf.Abs(posMundo.y - this.transform.position.y) < alturaIgual)
                {
                    //Debug.Log("Bloque demasiado cerca del jugador, no se coloca.");
                    return;
                }

                if (chunk == null) //si no encuenta el chunk pa que poner bloque
                {

                    return;
                }

                // Ajustar la posición relativa dentro del chunk
                Vector3 posBloque = posMundo - chunk.transform.position;

                // Si la posición está fuera del rango del chunk, buscar el chunk correcto
                if (posBloque.x < 0 || posBloque.x >= World.actual.anchoChunk || posBloque.z < 0 || posBloque.z >= World.actual.anchoChunk)
                {
                    
                    posBloque = posMundo - chunk.transform.position;
                }

                posBloque.x = Mathf.Floor(posBloque.x);
                posBloque.y = Mathf.Floor(posBloque.y);
                posBloque.z = Mathf.Floor(posBloque.z);

                chunk.SetBlock(posBloque, bloqueEnMano);
                chunk.GenerarMallaVisual();
            }
        }
    }

    void RealizarRaycast()
    {
        Physics.Raycast(transform.position, transform.forward, out impacto, rango, capaObjetivo);
    }
}



