using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Question
{
    public string pregunta;
    public string[] opciones;
    public int respuestaCorrecta;
    public int puntos;
    public GameObject interaccionUI;
}


public class Question_controller : MonoBehaviour
{
    public string tipoPrefab; // Añade esta línea

    public Text preguntaText;
    public Button[] botones;
    public Text[] opcionesText;
    public Text puntajeText; // Texto donde se mostrará el puntaje
    public Text incorrectaText;

    private GameObject objetoQueActivo;

    private int preguntaActual = 0;
    private int puntajeTotal = 0;
    private int incorrectas = 0;
    public List<Question> preguntas = new List<Question>();
    public List<Question> preguntasDisponibles = new List<Question>(); // Lista de preguntas mezcladas



    public AudioClip miClip;
    public GameObject audioManager;

    public AudioClip miClip2;
    public GameObject audioManager2;

    void ReproducirSonido_bien()
    {

        AudioSource fuente = audioManager.AddComponent<AudioSource>();
        fuente.clip = miClip;
        fuente.Play();
        Destroy(fuente, miClip.length);

    }

    void ReproducirSonido_malo()
    {

        AudioSource fuente2 = audioManager2.AddComponent<AudioSource>();
        fuente2.clip = miClip2;
        fuente2.Play();
        Destroy(fuente2, miClip2.length);

    }

    void CargarPreguntas()
    {
        preguntas.Clear();

        switch (tipoPrefab.ToLower())
        {
            case "estadistica":
                CargarPreguntasEstadistica();
                break;
            case "matematicas":
                CargarPreguntasMatematicas();
                break;
            case "biologia":
                CargarPreguntasBiologia();
                break;
            case "sociales":
                CargarPreguntasSociales();
                break;
        }
    }



    void Start()
    {


        CargarPreguntas();
        AsignarEventosBotones();

        // Si preguntasDisponibles está vacío, volver a llenarlo con las preguntas originales
        if (preguntasDisponibles == null || preguntasDisponibles.Count == 0)
        {
            preguntasDisponibles = new List<Question>(preguntas);
            BarajarPreguntas();
        }

        MostrarPregunta();



    }

    void OnEnable()
    {


        MostrarPregunta();
        ActualizarPuntaje();

        FPSController fpsController = FindObjectOfType<FPSController>();
        if (fpsController != null)
        {
            fpsController.bloquearCamara = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }



    public void SetActivador(GameObject activador)
    {
        objetoQueActivo = activador;

        TipoNPC tipoNPC = activador.GetComponent<TipoNPC>();
        if (tipoNPC != null)
        {
            tipoPrefab = tipoNPC.tipoPreguntas;

            CargarPreguntas();

            preguntasDisponibles = PreguntaManagerPorTipo.instancia.ObtenerPreguntasRestantes(tipoPrefab, preguntas);

            BarajarPreguntas(); 

            MostrarPregunta();
        }
    }

    private void OnDisable()
    {

        // Si hay un NPC registrado, destruirlo
        if (objetoQueActivo != null)
        {
            Destroy(objetoQueActivo);
            objetoQueActivo = null;
        }

        // Desbloquear la cámara y ocultar el cursor
        FPSController fpsController = FindObjectOfType<FPSController>();
        if (fpsController != null)
        {
            fpsController.bloquearCamara = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    void CargarPreguntasEstadistica()
    {
        preguntas.Add(new Question
        {
            pregunta = "¿Qué es la media aritmética?",
            opciones = new string[] { "El número que más se repite", "La suma de los valores dividida entre la cantidad de valores", "El valor central de un conjunto ordenado" },
            respuestaCorrecta = 1,
            puntos = 10
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál de las siguientes medidas de tendencia central NO existe?",
            opciones = new string[] { "Moda", "Mediana", "Rango" },
            respuestaCorrecta = 2,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "Si la media de 5 números es 8, ¿cuál es la suma total de esos números?",
            opciones = new string[] { "8", "40", "5" },
            respuestaCorrecta = 1,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál de estos gráficos es ideal para representar frecuencias absolutas?",
            opciones = new string[] { "Histograma", "Diagrama de dispersión", "Gráfico de líneas" },
            respuestaCorrecta = 0,
            puntos = 10
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cómo se calcula la varianza?",
            opciones = new string[] { "Media de los valores", "Diferencia entre el mayor y el menor valor", "Promedio de los cuadrados de las desviaciones respecto a la media" },
            respuestaCorrecta = 2,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "Si la desviación estándar de un conjunto de datos es baja, ¿qué significa?",
            opciones = new string[] { "Los datos están muy dispersos", "Los datos están muy agrupados alrededor de la media", "No hay datos en la muestra" },
            respuestaCorrecta = 1,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué significa que dos variables tengan una correlación cercana a 0?",
            opciones = new string[] { "Tienen una relación positiva", "No tienen relación", "Tienen una relación negativa" },
            respuestaCorrecta = 1,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "En una distribución normal, ¿qué porcentaje de datos cae dentro de una desviación estándar de la media?",
            opciones = new string[] { "68%", "95%", "99.7%" },
            respuestaCorrecta = 0,
            puntos = 10
        });

        preguntas.Add(new Question
        {
            pregunta = "Si un conjunto de datos tiene una distribución sesgada a la derecha, ¿qué significa?",
            opciones = new string[] { "La media es mayor que la mediana", "La media es menor que la mediana", "La media y la mediana son iguales" },
            respuestaCorrecta = 0,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es el propósito principal de un muestreo aleatorio?",
            opciones = new string[] { "Reducir la variabilidad de los datos", "Garantizar que cada individuo tenga la misma probabilidad de ser seleccionado", "Eliminar la necesidad de hacer cálculos estadísticos" },
            respuestaCorrecta = 1,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué representa la moda en un conjunto de datos?",
            opciones = new string[] { "El promedio de los datos", "El dato central", "El valor que más se repite" },
            respuestaCorrecta = 2,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál de los siguientes NO es un gráfico de representación de datos?",
            opciones = new string[] { "Histograma", "Gráfico de pastel", "Diagrama de flujo" },
            respuestaCorrecta = 2,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la mediana del conjunto {3, 7, 9, 11, 15}?",
            opciones = new string[] { "9", "7", "11" },
            respuestaCorrecta = 0,
            puntos = 10
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué mide la desviación estándar?",
            opciones = new string[] { "La centralidad de los datos", "La dispersión respecto a la media", "La frecuencia absoluta" },
            respuestaCorrecta = 1,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué significa que dos variables tengan una correlación de -1?",
            opciones = new string[] { "No hay relación entre ellas", "Relación positiva perfecta", "Relación negativa perfecta" },
            respuestaCorrecta = 2,
            puntos = 10
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué valor se obtiene si restamos el menor valor al mayor en un conjunto de datos?",
            opciones = new string[] { "La media", "La varianza", "El rango" },
            respuestaCorrecta = 2,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué herramienta estadística se utiliza para predecir valores futuros basados en datos actuales?",
            opciones = new string[] { "Histogramas", "Regresión", "Moda" },
            respuestaCorrecta = 1,
            puntos = 10
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es el nombre del gráfico que muestra la frecuencia acumulada?",
            opciones = new string[] { "Ojiva", "Histograma", "Boxplot" },
            respuestaCorrecta = 0,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es el valor de la media del conjunto {4, 8, 6, 10}?",
            opciones = new string[] { "7", "8", "9" },
            respuestaCorrecta = 0,
            puntos = 10
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué indica un coeficiente de correlación cercano a 1?",
            opciones = new string[] { "Relación positiva fuerte", "Relación negativa fuerte", "Sin relación" },
            respuestaCorrecta = 0,
            puntos = 5
        });

    }

    void CargarPreguntasMatematicas()
    {
        
        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la pendiente de una recta perpendicular a y = 2x + 3?",
            opciones = new string[] { "-1/2", "2", "-2"},
            respuestaCorrecta = 0,
            puntos = 15
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la pendiente de una recta y = 8x + 3 - 2?",
            opciones = new string[] {"3", "-1", "8" },
            respuestaCorrecta = 2,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es el elemento neutro de la suma en los números reales?",
            opciones = new string[] { "0", "1", "No existe"},
            respuestaCorrecta = 0,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cómo se define un número primo?",
            opciones = new string[] {
            "Tiene exactamente dos divisores positivos",
            "Es divisible solo por 10 y por sí mismo",
            "Es mayor que cero y par"
            },
            respuestaCorrecta = 0,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la factorización de x² - 5x + 6?",
            opciones = new string[] { "(x-2)(x-3)", "(x+2)(x+3)", "(x-1)(x-6)"},
            respuestaCorrecta = 0,
            puntos = 10
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la propiedad principal de los ángulos alternos internos cuando dos rectas son paralelas?",
            opciones = new string[] {
            "Suman 90°",
            "Son iguales",
            "Suman 180°"
            },
            respuestaCorrecta = 1,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es el dominio de la función f(x) = √(x - 2)?",
            opciones = new string[] { "x ≥ 0", "x > 2", "x ≥ 2"},
            respuestaCorrecta = 2,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cómo se llama la propiedad que relaciona multiplicación y suma: a·(b + c) = a·b + a·c?",
            opciones = new string[] { "Distributiva", "Conmutativa", "Asociativa"},
            respuestaCorrecta = 0,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "Si lim(x→∞) f(x)/x = 2, el grado del polinomio f(x) es:",
            opciones = new string[] { "0", "1", "2"},
            respuestaCorrecta = 1,
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la derivada de f(x) = x·ln(x)?",
            opciones = new string[] { "1 + ln(x)", "x + ln(x)", "ln(x)", "1/x" },
            respuestaCorrecta = 0,
            puntos = 10
        });
    }

    void CargarPreguntasBiologia()
    {
        // Misma lista, con índices de respuesta correctos variados
        preguntas.Clear();

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la unidad básica de los seres vivos?",
            opciones = new string[] { "Molécula", "Célula", "Tejido" },
            respuestaCorrecta = 1,  
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Dónde ocurre la fotosíntesis en las plantas?",
            opciones = new string[] { "Cloroplasto", "Mitocondria", "Núcleo" },
            respuestaCorrecta = 0,  
            puntos = 15
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es el ácido nucleico que almacena la información genética?",
            opciones = new string[] { "ARN", "ATP", "ADN" },
            respuestaCorrecta = 2,  
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿En qué fase de la mitosis las cromátidas hermanas se separan?",
            opciones = new string[] { "Profase", "Anafase", "Telofase" },
            respuestaCorrecta = 1, 
            puntos = 15
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la función principal de las enzimas?",
            opciones = new string[] { "Transportar oxígeno", "Almacenar energía", "Acelerar reacciones químicas" },
            respuestaCorrecta = 2,  
            puntos = 15
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cómo se llama el proceso de transporte de agua a través de la membrana semipermeable?",
            opciones = new string[] { "Endocitosis", "Ósmosis", "Difusión" },
            respuestaCorrecta = 1,  
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué orgánulo celular produce la mayor parte del ATP?",
            opciones = new string[] { "Ribosoma", "Mitocondria", "Cloroplasto" },
            respuestaCorrecta = 1,  
            puntos = 15
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cómo se denomina el grupo de organismos de la misma especie que viven en un área?",
            opciones = new string[] { "Ecosistema", "Población", "Comunidad" },
            respuestaCorrecta = 1,  
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué relación ecológica beneficia a ambas especies?",
            opciones = new string[] { "Competencia", "Mutualismo", "Parasitismo" },
            respuestaCorrecta = 1,  
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué tipo de división celular reduce a la mitad el número de cromosomas?",
            opciones = new string[] { "Fisión binaria", "Meiosis", "Mitosis" },
            respuestaCorrecta = 1,  
            puntos = 10
        });

    }

    void CargarPreguntasSociales()
    {
        // 10 preguntas teóricas de Sociales (nivel bachillerato), respuestas rápidas
        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la capital de Colombia?",
            opciones = new string[] { "Medellín", "Bogotá", "Cali" },
            respuestaCorrecta = 1,  // Bogotá
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué documento proclama los derechos fundamentales del hombre y del ciudadano (1789)?",
            opciones = new string[] { "Carta Magna", "Declaración de los Derechos del Hombre y del Ciudadano", "Constitución de Cádiz" },
            respuestaCorrecta = 1,  // Declaración de los Derechos...
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cómo se llama el sistema de gobierno donde el poder recae en el pueblo por medio de representantes?",
            opciones = new string[] { "Autocracia", "Monarquía constitucional", "República democrática" },
            respuestaCorrecta = 2,  // República democrática
            puntos = 10
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué civilización antigua construyó pirámides en Teotihuacán?",
            opciones = new string[] { "Azteca", "Maya", "Teotihuacana" },
            respuestaCorrecta = 2,  // Teotihuacana
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es el principal río de Brasil?",
            opciones = new string[] { "Amazonas", "Paraná", "Orinoco" },
            respuestaCorrecta = 0,  // Amazonas
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué concepto económico mide el valor total de bienes y servicios producidos en un país?",
            opciones = new string[] { "PIB", "IPC", "PPA" },
            respuestaCorrecta = 0,  // PIB
            puntos = 5
        });

        
        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la capital de Venezuela?",
            opciones = new string[] { "Caracas", "Bogotá", "Mompos Bolivar" },
            respuestaCorrecta = 0,  
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué tratado puso fin a la Primera Guerra Mundial en 1919?",
            opciones = new string[] { "Versalles", "Utrecht", "Paz de Westfalia" },
            respuestaCorrecta = 0,  // Versalles
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Cuál es la moneda oficial de la Unión Europea?",
            opciones = new string[] { "Euro", "Dólar", "Libra esterlina" },
            respuestaCorrecta = 0,  // Euro
            puntos = 5
        });

        preguntas.Add(new Question
        {
            pregunta = "¿Qué evento marcó el inicio de la Revolución Francesa?",
            opciones = new string[] { "Toma de la Bastilla", "Declaración de Viena", "Batalla de Waterloo" },
            respuestaCorrecta = 0,  // Toma de la Bastilla
            puntos = 15
        });

    }


    void BarajarPreguntas()
    {

        for (int i = 0; i < preguntasDisponibles.Count; i++)
        {
            Question temp = preguntasDisponibles[i];
            int randomIndex = Random.Range(i, preguntasDisponibles.Count);
            preguntasDisponibles[i] = preguntasDisponibles[randomIndex];
            preguntasDisponibles[randomIndex] = temp;
        }
    }

    void AsignarEventosBotones()
    {
        for (int i = 0; i < botones.Length; i++)
        {
            int index = i;
            botones[i].onClick.AddListener(() => SeleccionarRespuesta(index));
        }
    }

    void MostrarPregunta()
    {
        if (preguntaActual >= preguntasDisponibles.Count)
        {

            return;
        }

        Question pregunta = preguntasDisponibles[preguntaActual];
        preguntaText.text = pregunta.pregunta;

        for (int i = 0; i < opcionesText.Length; i++)
        {
            if (i < pregunta.opciones.Length)
            {
                opcionesText[i].text = pregunta.opciones[i];
            }
            else
            {
                opcionesText[i].text = "";
            }
        }
    }

    void ActualizarPuntaje()
    {
        if (puntajeText != null)
        {

            puntajeText.text = puntajeTotal.ToString();
        }


        if (puntajeTotal >= 35)
        {

            SceneManager.LoadScene("ganaste");
            return;
        }
    }


    public void SeleccionarRespuesta(int indice)
    {
        if (indice == preguntasDisponibles[preguntaActual].respuestaCorrecta)
        {
            ReproducirSonido_bien();
            puntajeTotal += preguntasDisponibles[preguntaActual].puntos;
            ActualizarPuntaje();
        }
        else
        {
            ReproducirSonido_malo();
            incorrectas++;
            incorrectaText.text = incorrectas.ToString();
        }

        PreguntaManagerPorTipo.instancia.QuitarPreguntaRespondida(tipoPrefab, preguntasDisponibles[preguntaActual]);

        preguntasDisponibles.RemoveAt(preguntaActual);

        // Verificar condiciones de fin
        if (incorrectas >= 3)
        {
            SceneManager.LoadScene("perdiste");
            return;
        }

        if (puntajeTotal >= 35)
        {
            SceneManager.LoadScene("ganaste");
            return;
        }

        gameObject.SetActive(false); // Ocultar para siguiente activación
    }

}