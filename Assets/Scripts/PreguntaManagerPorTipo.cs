using System.Collections.Generic;
using UnityEngine;

public class PreguntaManagerPorTipo : MonoBehaviour
{
    public static PreguntaManagerPorTipo instancia;

    private Dictionary<string, List<Question>> preguntasRestantes = new Dictionary<string, List<Question>>();

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Question> ObtenerPreguntasRestantes(string tipo, List<Question> preguntasBase)
    {
        if (!preguntasRestantes.ContainsKey(tipo))
        {
            // Primera vez: copiar las preguntas
            preguntasRestantes[tipo] = new List<Question>(preguntasBase);
        }

        return preguntasRestantes[tipo];
    }

    public void QuitarPreguntaRespondida(string tipo, Question pregunta)
    {
        if (preguntasRestantes.ContainsKey(tipo))
        {
            preguntasRestantes[tipo].Remove(pregunta);
        }
    }
}