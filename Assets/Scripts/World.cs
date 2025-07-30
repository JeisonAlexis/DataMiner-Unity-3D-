using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public static World actual;

    public int semilla;
    public int anchoChunk = 16;
    public int alturaChunk = 50;
    public float rangoVista = 5;
    public GameObject prefabChunk;

    private List<Chunk> chunks;
    private Transform jugador;
    private float tiempo;

    void Awake()
    {
        World.actual = this;
    }

    void Start()
    {


        
        if (this.semilla == 0)
        {
            this.semilla = Random.Range(0, int.MaxValue);
        }

        
        this.chunks = new List<Chunk>(); 
        this.tiempo = 1; 
        this.jugador = GameObject.FindGameObjectWithTag("Player").transform; 
        this.jugador.position = new Vector3(0, alturaChunk + 10, 0); 
    }

    void Update()
    {
        this.tiempo += Time.deltaTime;
        if (this.tiempo > 1)
        {
            this.tiempo = 0;
            CrearChunks();
        }
    }

    private void CrearChunks()
    {
        float distanciaMaxima = this.rangoVista * this.anchoChunk;
        Chunk chunk;
        Vector3 posicion = new Vector3();
        Vector3 posJugador = this.jugador.position;

        for (float x = (posJugador.x - distanciaMaxima); x < (posJugador.x + distanciaMaxima); x += this.anchoChunk)
        {
            for (float z = (posJugador.z - distanciaMaxima); z < (posJugador.z + distanciaMaxima); z += this.anchoChunk)
            {
                posicion.x = ((int)(x / this.anchoChunk)) * this.anchoChunk;
                posicion.z = ((int)(z / this.anchoChunk)) * this.anchoChunk;

                chunk = BuscarChunk(posicion);

                if (chunk == null)
                {
                    Instantiate(this.prefabChunk, posicion, Quaternion.identity);
                }
            }
        }
    }

    public void AgregarChunk(Chunk chunk)
    {
        this.chunks.Add(chunk);
    }

    public Chunk BuscarChunk(Vector3 posicion)
    {
        Vector3 aux;
        Chunk chunk = null;
        bool encontrado = false;

        for (int i = 0; (i < this.chunks.Count) && (!encontrado); i++)
        {
            aux = this.chunks[i].transform.position;

            if ((aux.x <= posicion.x) && (aux.z <= posicion.z) &&
                (aux.x + this.anchoChunk > posicion.x) && (aux.z + this.anchoChunk > posicion.z))
            {
                chunk = this.chunks[i];
                encontrado = true;
            }
        }
        return chunk;
    }
}





