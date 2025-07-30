using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplexNoise;


    [RequireComponent (typeof(MeshFilter))]
    [RequireComponent (typeof(MeshRenderer))]
    [RequireComponent (typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{

    private byte[,,] blocks;

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private List<Vector3> posicionCasas = new List<Vector3>();

    public GameObject endermanPrefab;
    public GameObject golemPrefab;
    public GameObject girlPrefab;
    public GameObject orcoPrefab;



    void Start()
    {

        

        World.actual.AgregarChunk(this);
        this.meshFilter = GetComponent<MeshFilter>();
        this.meshCollider = GetComponent<MeshCollider>();
        this.blocks = new byte[World.actual.anchoChunk, World.actual.alturaChunk, World.actual.anchoChunk];

        
        this.GenerarEstructura();
        this.GenerarCasas();
        this.GenerarArboles();
        this.GenerarMallaVisual();


    }



    void GenerarArboles()
    {

        //pasar coordenadas locales a globales
        int chunkX = (int)(transform.position.x / World.actual.anchoChunk);
        int chunkZ = (int)(transform.position.z / World.actual.anchoChunk); 

        int distanciaMinima = 8; // Distancia mínima entre árboles y casas (en bloques)

        for (int x = 3; x < World.actual.anchoChunk - 3; x++)
        {
            for (int z = 3; z < World.actual.anchoChunk - 3; z++) //se inicia y se resta 3 porque (no queremos generar arboles cerca de los bordes de un chunk ya que se cortan
            {
                for (int y = World.actual.alturaChunk - 1; y > 0; y--) //se busca en y el bloque mas arriba (luego se comprueba si es pasto o no para poner un arbol
                {
                    if (this.blocks[x, y, z] != 2) continue; // Si no hay pasto, sigue buscando.

                    if (y + 1 >= World.actual.alturaChunk) break; // si nos salimos de la altuma maxima nos salimos de este ciclo (porque ya no hay bloques que revisar)

                    // Convertir la posición del árbol a coordenadas globales (una cosa son las coordenadas dentro del chunk y otras las globales OJO)
                    Vector2 treePos = new Vector2(
                        x + chunkX * World.actual.anchoChunk,
                        z + chunkZ * World.actual.anchoChunk
                    );

                    //Evitar generar árboles cerca de casas (porque puede afectar la estructura interna de la casa)
                    bool cercaDeCasa = false;
                    foreach (Vector3 casaPos in posicionCasas)
                    {
                        float distance = Vector2.Distance(treePos, new Vector2(casaPos.x, casaPos.z));
                        if (distance < distanciaMinima)
                        {
                            cercaDeCasa = true;
                            break;
                        }
                    }

                    if (cercaDeCasa) continue; // Si está demasiado cerca de una casa, salta este árbol

                    bool plantarArbol = true; // si pasó todas las pruebas anteriores significa que el arbol se puede plantar (solo queda revisar que no se salga en y)

                    for (int dx = -8; dx <= 8 && plantarArbol; dx++)
                    {
                        for (int dz = -8; dz <= 8 && plantarArbol; dz++)
                        {
                            int checkX = x + dx, checkZ = z + dz;

                            if (checkX < 0 || checkX >= World.actual.anchoChunk ||
                                checkZ < 0 || checkZ >= World.actual.anchoChunk) continue; //Evitamos salirnos de los limites x z del chunk

                            for (int dy = 0; dy < 4; dy++)
                            {
                                int checkY = y + dy;
                                if (checkY >= World.actual.alturaChunk) break; //Evitamos salirnos de los limites y del chunk 

                                byte block = this.blocks[checkX, checkY, checkZ];
                                if (block == 3 || block == 9 || block == 11 || block == 14) //evita que parezcan arboles en la misma posicion
                                {
                                    plantarArbol = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (!plantarArbol) break;

                    int hash = HashCoherente(chunkX, chunkZ);

                    int semillaLocal = World.actual.semilla + chunkX * 73856093 + chunkZ * 19349663;
                    System.Random rng = new System.Random(semillaLocal);
                    int tipoDeArbol = rng.Next(0, 4);


                    if (tipoDeArbol == 0)
                        ArbolNormal(x, y + 1, z);
                    else if (tipoDeArbol == 1)
                        ArbolSakura(x, y + 1, z);
                    else if (tipoDeArbol == 2)
                        ArbolRedondo(x, y + 1, z);
                    else if (tipoDeArbol == 3)
                        ArbolesLargos(x, y + 1, z);


                    break; // si ya plantamos nos salimos del bucle
                }
            }
        }
    }





    void ArbolNormal(int x, int y, int z)
    {
        int alturatronco = Random.Range(4, 6); //una altura de tronco variable
        int alturaMaxima = Mathf.Min(y + alturatronco, World.actual.alturaChunk - 1); //me ayuda a controlar que la altura del arbol no exceda la del chunk

        // Generar tronco
        for (int i = 0; i < alturatronco; i++)
        {
            int actualY = y + i;
            if (actualY < World.actual.alturaChunk)
            {
                this.blocks[x, actualY, z] = 3; // Tronco
            }
        }

        // Generar hojas sin esquinas
        int inicioHojas = alturaMaxima - 2;
        for (int lx = -2; lx <= 2; lx++)
        {
            for (int lz = -2; lz <= 2; lz++)
            {
                if (Mathf.Abs(lx) == 2 && Mathf.Abs(lz) == 2) continue; // Omite las esquinas en las hojas para que no quede cuadrado el arbol

                for (int ly = 0; ly < 3; ly++)
                {
                    int hojaX = x + lx;
                    int hojaY = inicioHojas + ly;
                    int hojaZ = z + lz;

                    if (hojaX >= 0 && hojaX < World.actual.anchoChunk &&
                        hojaY >= 0 && hojaY < World.actual.alturaChunk &&
                        hojaZ >= 0 && hojaZ < World.actual.anchoChunk)
                    {
                        this.blocks[hojaX, hojaY, hojaZ] = 4; // Hojas
                    }
                }
            }
        }
    }


    void ArbolesLargos(int x, int y, int z)
    {
        int trunkHeight = Random.Range(7, 10); // Árbol un poco más alto
        int maxTreeHeight = Mathf.Min(y + trunkHeight, World.actual.alturaChunk - 1);

        for (int i = 0; i < trunkHeight; i++)
        {
            int currentY = y + i;
            if (currentY < World.actual.alturaChunk)
            {
                this.blocks[x, currentY, z] = 9; // Tronco de arbol grande
            }
        }

        int leafStart = maxTreeHeight - 3;
        for (int lx = -3; lx <= 3; lx++)
        {
            for (int lz = -3; lz <= 3; lz++)
            {
                for (int ly = 0; ly < 4; ly++) // Capa superior con más hojas
                {
                    int leafX = x + lx;
                    int leafY = leafStart + ly;
                    int leafZ = z + lz;

                    // Forma "cupula" o "escalera" (segun se mire) para las hojas
                    if (Mathf.Abs(lx) + Mathf.Abs(lz) + ly < 5)
                    {
                        if (leafX >= 0 && leafX < World.actual.anchoChunk &&
                            leafY >= 0 && leafY < World.actual.alturaChunk &&
                            leafZ >= 0 && leafZ < World.actual.anchoChunk)
                        {
                            this.blocks[leafX, leafY, leafZ] = 10; // Hojas grandes
                        }
                    }
                }
            }
        }
    }


    void ArbolRedondo(int x, int y, int z)
    {
        int trunkHeight = Random.Range(6, 9); // Árbol con tronco un poco mas bajo que el anterior
        int maxTreeHeight = Mathf.Min(y + trunkHeight, World.actual.alturaChunk - 1);

        for (int i = 0; i < trunkHeight; i++)
        {
            int currentY = y + i;
            if (currentY < World.actual.alturaChunk)
            {
                this.blocks[x, currentY, z] = 11; // Tronco de tipo 11
            }
        }

        int leafStart = maxTreeHeight - 3;
        for (int lx = -3; lx <= 3; lx++)
        {
            for (int lz = -3; lz <= 3; lz++)
            {
                for (int ly = 0; ly < 4; ly++) // Capa superior con más hojas
                {
                    int leafX = x + lx;
                    int leafY = leafStart + ly;
                    int leafZ = z + lz;

                    // Genera una forma redonda usando distancia euclidiana
                    float distance = Mathf.Sqrt(lx * lx + lz * lz + ly * ly);
                    if (distance < 3.5f) // entre mas alto el valor mas redondo
                    {
                        if (leafX >= 0 && leafX < World.actual.anchoChunk && leafY >= 0 && leafY < World.actual.alturaChunk && leafZ >= 0 && leafZ < World.actual.anchoChunk)
                        {
                            this.blocks[leafX, leafY, leafZ] = 12; // Hojas del redondo (bloque 12)
                        }
                    }
                }
            }
        }
    }


    void ArbolSakura(int x, int y, int z)
    {
        int trunkHeight = 4; // Altura fija de 4 bloques
        int maxTreeHeight = Mathf.Min(y + trunkHeight, World.actual.alturaChunk - 1);

        // Generar tronco
        for (int i = 0; i < trunkHeight; i++)
        {
            int currentY = y + i;
            if (currentY < World.actual.alturaChunk)
            {
                this.blocks[x, currentY, z] = 14; // Tronco de Sakura
            }
        }

        int leafStart = maxTreeHeight - 1;

        // Generar hojas con forma de sakura
        for (int lx = -3; lx <= 3; lx++)
        {
            for (int lz = -3; lz <= 3; lz++)
            {
                for (int ly = 0; ly < 3; ly++) // Capa superior con más hojas
                {
                    int leafX = x + lx;
                    int leafY = leafStart + ly;
                    int leafZ = z + lz;

                    // Forma más redonda y con ramas sobresaliendo
                    float distance = Mathf.Sqrt(lx * lx + lz * lz);
                    if (distance < 2.8f || (distance < 3.5f && Random.Range(0, 4) == 0)) //usamos el concepto anterior para hacerlo redondo pero ponemos bloques random por ahi
                    {
                        if (leafX >= 0 && leafX < World.actual.anchoChunk && leafY >= 0 && leafY < World.actual.alturaChunk && leafZ >= 0 && leafZ < World.actual.anchoChunk)
                        {
                            this.blocks[leafX, leafY, leafZ] = 15; // Hojas de Sakura
                        }
                    }
                }
            }
        }
    }




    void GenerarEstructura()
    {
        float noiseValue;
        float stoneValue;
        Vector3 noisePos = new Vector3();

        // Establecer la semilla para que los chunks tengan coherencia entre si
        Random.InitState(World.actual.semilla);

        // Generar un offset aleatorio con valores más dispersos
        Vector3 offset = new Vector3(
            Random.Range(-5000f, 5000f),
            Random.Range(-5000f, 5000f),
            Random.Range(-5000f, 5000f)
        );

        Vector3 chunkOffset = new Vector3(transform.position.x, transform.position.y, transform.position.z); 

        for (int x = 0; x < World.actual.anchoChunk; x++)
        {
            for (int z = 0; z < World.actual.anchoChunk; z++) //recorremos las dimensiones en z y x del chunk
            {
                this.blocks[x, 0, z] = 5; // Capa más baja de bloque 5 (para mi es como la bedrock por asi decirlo)

                float tipoTerreno = this.GenerarRuido(new Vector3(x, 0, z) + chunkOffset, offset, 50f);
                bool montaña = tipoTerreno > 0.35f;

                float heightNoise = this.GenerarRuido(new Vector3(x, 0, z) + chunkOffset, offset, montaña ? 30f : 100f); 
                //montaña es true entonces devuelve 30f sies false devuelve 100f (basicamente para controlar el ruido de perlin 
                int terrainHeight = Mathf.FloorToInt((heightNoise + 1) * (World.actual.alturaChunk - 10) * 0.4f + (montaña ? 10 : 0));
                //0.4 reduciendo la escala de la variación para que el terreno no sea demasiado alto.

                for (int y = 1; y < terrainHeight; y++)
                {
                    noisePos.Set(x + transform.position.x, y + transform.position.y, z + transform.position.z); //pasamos las coordenadas locales del chunk a globales

                    stoneValue = this.GenerarRuido(noisePos, offset, 20f); //con esto generaremos variaciones de piedra
                    float tmp = (5 - y) / 15f;
                    stoneValue += tmp;

                    noiseValue = this.GenerarRuido(noisePos, offset, 50f); //con esto generaremos variaciones de piedra
                    tmp = (50 - y) / 50f;
                    noiseValue += tmp;

                    //tmp ajusta los valores de ruido según la altura favorece la piedra en la parte baja y la tierra en otras capa

                    noiseValue += stoneValue; //hacemos que los ruidos se junten para que la tierra y la piedra se lleguen a combinar

                    if (stoneValue > 0.6f)
                    {
                        this.blocks[x, y, z] = 6; // Piedra
                    }
                    else if (noiseValue > 0.2f)
                    {
                        this.blocks[x, y, z] = 1; // Tierra
                    }
                    else
                    {
                        this.blocks[x, y, z] = 0; // Aire
                    }

                    
                }
            }
        }

        // **Colocar pasto en la superficie**
        for (int x = 0; x < World.actual.anchoChunk; x++)
        {
            for (int z = 0; z < World.actual.anchoChunk; z++)
            {
                for (int y = World.actual.alturaChunk - 1; y > 0; y--)
                {
                    if (this.blocks[x, y, z] == 1)
                    {
                        if (this.blocks[x, y + 1, z] == 0)
                        {
                            this.blocks[x, y, z] = 2; // Convertir en pasto
                        }
                    }
                }
            }
        }
    }






    float GenerarRuido(Vector3 position, Vector3 offset, float escala){

        

        float nX = (position.x + offset.x) / escala; 
        float nY = (position.y + offset.y) / escala;
        float nZ = (position.z + offset.z) / escala;

        //la división por la escala controla cuán "estirado" o "comprimido" se ve el patrón de ruido en el espacio.

        return Noise.Generate(nX, nY, nZ);

    }

    public void SetBlock(Vector3 pos, byte block){

        if (!((pos.x < 0 ) || (pos.y < 0) || (pos.z < 0) || (pos.x >= World.actual.anchoChunk) || (pos.y >= World.actual.alturaChunk) || (pos.z >= World.actual.anchoChunk)))
        {

            this.blocks[((int)pos.x),((int)pos.y),((int)pos.z)] = block;

        }
    }

    public byte GetBlock(int x, int y, int z){

        if((x < 0 ) || (y < 0) || (z < 0) || (x >= World.actual.anchoChunk) || (y >= World.actual.alturaChunk) || (z >= World.actual.anchoChunk))
        {

            return 0;

        }else{

            return this.blocks[x,y,z];
        }
    }

    bool EsTransparente(int x, int y, int z)
    {

        byte block = this.GetBlock(x, y, z);

        if (block == 0)
        {

            return true;
        }
        else
        {

            return false;
        }

    }

    public void GenerarMallaVisual()
    {

        Mesh mesh = new Mesh();
        mesh.name = "Chunk";

        List<Vector3> verts = new List<Vector3>(); //contendra los vertices 
        List<Vector2> uvs = new List<Vector2>(); //contendra las coordenadas de textura que indican cómo una textura debe colocarse sobre la superficie del cubo
        List<int> tris = new List<int>(); //contendra los triangulos

        for (int x = 0; x < World.actual.anchoChunk; x++)
        {
            for (int y = 0; y < World.actual.alturaChunk; y++)
            {
                for (int z = 0; z < World.actual.anchoChunk; z++) //Tenemos que recorrer todo el chunk osea todas sus dimensiones x y z
                {
                    if (this.blocks[x, y, z] != 0) //obviamente no vamos a renderizar un bloque vacio
                    {

                        // izquierda
                        if (this.EsTransparente(x - 1, y, z))
                        {
                            //al metodo auxiliar le pasamos el bloque a modelar, la posicion de la esquina, los vectores que indica la dirección de la cara, el bool (cambia el orden de los triángulos)
                            //y le pasamos los vertices, las coordenadas de textura y los triangulos
                            this.CrearCara(this.GetBlock(x, y, z), new Vector3(x, y, z), Vector3.up, Vector3.forward, true, verts, uvs, tris);
                        }

                        // derecha
                        if (this.EsTransparente(x + 1, y, z))
                        {

                            this.CrearCara(this.GetBlock(x, y, z), new Vector3(x + 1, y, z), Vector3.up, Vector3.forward, false, verts, uvs, tris);
                        }

                        // arriba
                        if (this.EsTransparente(x, y + 1, z))
                        {

                            this.CrearCara(this.GetBlock(x, y, z), new Vector3(x, y + 1, z), Vector3.forward, Vector3.right, false, verts, uvs, tris);
                        }

                        // abajo
                        if (this.EsTransparente(x, y - 1, z) && (y > 0))
                        {

                            this.CrearCara(this.GetBlock(x, y, z), new Vector3(x, y, z), Vector3.forward, Vector3.right, true, verts, uvs, tris);
                        }

                        // frente
                        if (this.EsTransparente(x, y, z - 1))
                        {

                            this.CrearCara(this.GetBlock(x, y, z), new Vector3(x, y, z), Vector3.up, Vector3.right, false, verts, uvs, tris);
                        }

                        // atras
                        if (this.EsTransparente(x, y, z + 1))
                        {

                            this.CrearCara(this.GetBlock(x, y, z), new Vector3(x, y, z + 1), Vector3.up, Vector3.right, true, verts, uvs, tris);
                        }


                    }

                }
            }
        }


        //asignamos valores al componente mesh

        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        

        this.meshFilter.mesh = mesh;
        this.meshFilter.sharedMesh = mesh;
        this.meshCollider.sharedMesh = null; // Asegurar actualización
        this.meshCollider.sharedMesh = mesh; // Aplicar la nueva malla
        this.meshCollider.convex = false; // Mantener como colisión de malla completa
        this.meshCollider.isTrigger = false; // Asegurar que no sea un trigger
    }


    void CrearCara(byte block, Vector3 corner, Vector3 up, Vector3 side, bool reversed, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
    {

        int index = verts.Count;

        //añade los vertices

        verts.Add(corner);
        verts.Add(corner + up);
        verts.Add(corner + up + side);
        verts.Add(corner + side);



        int x;
        int y;

        //en este caso la textura es de 4 x 4 es por ello que se divide por 4

        int tilling = 4;
        float uvWidth = 1f / tilling;

        x = block % tilling;
        y = block / tilling;

        if (x == 0)
        {

            x = tilling;
        }
        else
        {

            y++;
        }

        //donde se cuadran las texturas de los bloques (el algoritmo es supermalo, se debe mejorar en proximas versiones)

        uvs.Add(new Vector2(x * uvWidth, uvWidth * (tilling - y)));
        uvs.Add(new Vector2(x * uvWidth, uvWidth * (tilling - y) + uvWidth));
        uvs.Add(new Vector2((x * uvWidth) - uvWidth, uvWidth * (tilling - y) + uvWidth));
        uvs.Add(new Vector2((x * uvWidth) - uvWidth, uvWidth * (tilling - y)));

        //aqui simplemente revertimos la cara en caso de ser necesario, la izquierda es la inversa que la derecha, arriba es la inversa de abajo etc...
        //segun esto se cuadran los triangulos de la cara

        if (!reversed)
        {

            tris.Add(index + 0);
            tris.Add(index + 1);
            tris.Add(index + 2);
            tris.Add(index + 2);
            tris.Add(index + 3);
            tris.Add(index + 0);
        }
        else
        {

            tris.Add(index + 1);
            tris.Add(index + 0);
            tris.Add(index + 2);
            tris.Add(index + 3);
            tris.Add(index + 2);
            tris.Add(index + 0);

        }

    }


    void GenerarCasas()
    {
        //Obtener coordenadas globales apartir de las locales

        int chunkX = (int)(transform.position.x / World.actual.anchoChunk);
        int chunkZ = (int)(transform.position.z / World.actual.anchoChunk);

        // hacer que las casas aparezcan cada 4 chunks (aproximadamente) dado que se debe evaluar si se genera la casa o no
        if (chunkX % 3 == 0 && chunkZ % 3 == 0)
        {
            //evitar que la casa se genere en los bordes del chunk porque se cortaría

            int maxX = World.actual.anchoChunk - 6;
            int maxZ = World.actual.anchoChunk - 6;

            for (int x = 1; x < maxX; x++)
            {
                for (int z = 1; z < maxZ; z++) //recorremos las dimensiones x z 
                {
                    int baseY = ObtenerAltura(x, z); //obtenemos la altura del chunk

                    if (EstaLibre(x, baseY, z, 6, 6)) //comprobamos que este libre el terreno
                    {
                        ConstruirCasa(x, baseY, z, chunkX, chunkZ); //contruimos la casa

                        int hash = HashCoherente(chunkX, chunkZ);

                        int semillaLocal = World.actual.semilla + chunkX * 73856093 + chunkZ * 19349663;
                        System.Random rng = new System.Random(semillaLocal);
                        int tipoDeNpc = rng.Next(0, 4); 

                        Debug.Log($"[CHUNK {chunkX},{chunkZ}] hash: {hash} → tipo NPC: {tipoDeNpc}");

                        if (tipoDeNpc == 0)
                            SpawnEnderman();
                        else if (tipoDeNpc == 1)
                            SpawnGolem();
                        else if (tipoDeNpc == 2)
                            SpawnGirl();
                        else
                            SpawnOrco();

                        return; 
                    }
                }
            }
        }
    }

    int HashCoherente(int x, int z)
    {
        // Usa una combinación típica con números primos
        int semillaHash = World.actual.semilla;
        int n = x * 73856093 ^ z * 19349663 ^ semillaHash * 83492791;
        return Mathf.Abs(n);
    }


    int ObtenerAltura(int x, int z)
    {
        if (x < 0 || x >= World.actual.anchoChunk || z < 0 || z >= World.actual.anchoChunk) //evitamos salirnos de las dimensiones del chunk
        {
            return 0; 
        }

        for (int y = World.actual.alturaChunk - 1; y > 0; y--) //recorremos en y
        {
            if (this.blocks[x, y, z] != 0) // Asegurar que no sea aire
            {
                return Mathf.Clamp(y, 1, World.actual.alturaChunk - 2); 
            }
        }
        return 1; // Si no encontró superficie, devolver altura mínima segura
    }



    bool EstaLibre(int x, int y, int z, int width, int depth)
    {
        int baseHeight = ObtenerAltura(x, z); //debemos saber la altura de la base

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                int checkX = x + i;
                int checkZ = z + j;

                if (checkX < 0 || checkX >= World.actual.anchoChunk || checkZ < 0 || checkZ >= World.actual.anchoChunk) //nos aseguramos de no salirnos delas dimensiones del chunk, toda precausión es poca
                {

                    return false;
                }
                    

                int checkY = ObtenerAltura(checkX, checkZ);

                // Permitir una pequeña variación en la altura del terreno (1 bloque maximo)
                if (Mathf.Abs(checkY - baseHeight) > 1)
                {

                    return false;
                }
                    
            }
        }
        return true; //si pasa todas las pruebas significa que el terreno es lo suficiente mente plano para crearse
    }



    void ConstruirCasa(int x, int y, int z, int chunkX, int chunkZ)
    {
        int width = 8, height = 6, depth = 8; // Dimensiones de la casa
        int roofHeight = 4; // Altura del techo escalonado

        // Evitar que la casa se genere en el borde del chunk
        if (x + width >= World.actual.anchoChunk - 1 || z + depth >= World.actual.anchoChunk - 1)
        {
            return; // Si la casa no cabe en el chunk, no se genera
        }

        // Estructura principal 
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                for (int h = 0; h < height; h++)
                {
                    int blockX = x + i;
                    int blockY = y + h;
                    int blockZ = z + j;

                    if (blockX >= 0 && blockX < World.actual.anchoChunk &&
                        blockY >= 0 && blockY < World.actual.alturaChunk &&
                        blockZ >= 0 && blockZ < World.actual.anchoChunk)
                    {
                        if (h == 0)
                            this.blocks[blockX, blockY, blockZ] = 16; // Piso de madera
                        else if (h == height - 1)
                            this.blocks[blockX, blockY, blockZ] = 7; // Base del techo
                        else if (i == 0 || i == width - 1 || j == 0 || j == depth - 1)
                            this.blocks[blockX, blockY, blockZ] = 7; // Paredes de madera clara
                        else
                            this.blocks[blockX, blockY, blockZ] = 0; // Espacio vacío
                    }
                }
            }
        }

        // Bordes decorativos 
        for (int i = -1; i <= width; i++)
        {
            for (int j = -1; j <= depth; j++)
            {
                if ((i == -1 || i == width) || (j == -1 || j == depth))
                {
                    int blockX = x + i;
                    int blockY = y;
                    int blockZ = z + j;

                    if (blockX >= 0 && blockX < World.actual.anchoChunk &&
                        blockZ >= 0 && blockZ < World.actual.anchoChunk)
                    {
                        this.blocks[blockX, blockY, blockZ] = 8; // Bordes de piedra
                    }
                }
            }
        }

        // Puerta (2 bloques de ancho y centrada)
        int doorX = x + (width / 2) - 1;
        int doorY = y + 1;
        int doorZ = z;

        for (int dx = 0; dx < 2; dx++) // Puerta de 2 bloques de ancho
        {
            for (int h = 0; h < 2; h++) // 2 bloques de alto
            {
                int px = doorX + dx;
                int py = doorY + h;
                if (px >= 0 && px < World.actual.anchoChunk &&
                    py >= 0 && py < World.actual.anchoChunk &&
                    doorZ >= 0 && doorZ < World.actual.anchoChunk)
                {
                    this.blocks[px, py, doorZ] = 0; // Espacio para la puerta (osea simplente esta vacia esa parte y ya)
                }
            }
        }

        // Ventanas (2x2 bloques y centradas)
        int windowY = y + (height / 2) - 1;

        // Ventana trasera (en la pared del fondo)
        for (int dx = 0; dx < 2; dx++)
        {
            for (int dy = 0; dy < 2; dy++)
            {
                int wx = x + (width / 2) - 1 + dx;
                int wy = windowY + dy;
                int wz = z + depth - 1;

                if (wx >= 0 && wx < World.actual.anchoChunk && wy >= 0 && wy < World.actual.alturaChunk && wz >= 0 && wz < World.actual.anchoChunk)
                {
                    this.blocks[wx, wy, wz] = 13; // Bloque de ventana
                }
            }
        }

        // Ventana lateral derecha
        for (int dz = 0; dz < 2; dz++)
        {
            for (int dy = 0; dy < 2; dy++)
            {
                int wx = x + width - 1;
                int wy = windowY + dy;
                int wz = z + (depth / 2) - 1 + dz;

                if (wx >= 0 && wx < World.actual.anchoChunk &&
                    wy >= 0 && wy < World.actual.alturaChunk &&
                    wz >= 0 && wz < World.actual.anchoChunk)
                {
                    this.blocks[wx, wy, wz] = 13; // Bloque de ventana
                }
            }
        }

        // Ventana lateral izquierda
        for (int dz = 0; dz < 2; dz++)
        {
            for (int dy = 0; dy < 2; dy++)
            {
                int wx = x;
                int wy = windowY + dy;
                int wz = z + (depth / 2) - 1 + dz;

                if (wx >= 0 && wx < World.actual.anchoChunk &&
                    wy >= 0 && wy < World.actual.alturaChunk &&
                    wz >= 0 && wz < World.actual.anchoChunk)
                {
                    this.blocks[wx, wy, wz] = 13; // Bloque de ventana
                }
            }
        }

        // Techo en forma de escalera
        for (int h = 0; h < roofHeight; h++)
        {
            int roofStart = x + h;
            int roofEnd = x + width - 1 - h;
            int roofY = y + height + h;

            if (roofY >= World.actual.alturaChunk) break; // recordemos que no nos podemos salir del limite en y del chunk

            for (int i = roofStart; i <= roofEnd; i++)
            {
                for (int j = z + h; j <= z + depth - 1 - h; j++)
                {
                    if (i >= 0 && i < World.actual.anchoChunk &&
                        j >= 0 && j < World.actual.anchoChunk)
                    {
                        this.blocks[i, roofY, j] = 7; // madera oscura
                    }
                }
            }
        }

        // Guardar la posición de la casa
        Vector3 housePos = new Vector3(
            chunkX * World.actual.anchoChunk + x + width / 2 ,
            (y + height / 2) + 1,
            chunkZ * World.actual.anchoChunk + z + depth / 2 + 2.5f 
        );

        posicionCasas.Add(housePos);
    }








    void SpawnEnderman()
    {
        if (posicionCasas.Count > 0) // Verifica que haya al menos una casa
        {
            Vector3 spawnPos = posicionCasas[posicionCasas.Count - 1]; // Toma solo la última casa agregada
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
            Instantiate(endermanPrefab, spawnPos, rotation);
        }
    }

    void SpawnGirl()
    {
        if (posicionCasas.Count > 0) // Verifica que haya al menos una casa
        {
            Vector3 casaPosi = posicionCasas[posicionCasas.Count - 1];
            Vector3 spawnPos = new Vector3(casaPosi.x, casaPosi.y - 1f, casaPosi.z - 0.5f); // Toma solo la última casa agregada
            Quaternion rotation = Quaternion.Euler(0f, -180f, 0f);
            Instantiate(girlPrefab, spawnPos, rotation);
        }
    }

    void SpawnGolem()
    {
        if (posicionCasas.Count > 0) // Verifica que haya al menos una casa
        {
            Vector3 casaPosi = posicionCasas[posicionCasas.Count - 1];
            Vector3 spawnPos = new Vector3(casaPosi.x, casaPosi.y, casaPosi.z + 0.2f); // Toma solo la última casa agregada
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
            Instantiate(golemPrefab, spawnPos, rotation);
        }
    }

    void SpawnOrco()
    {
        if (posicionCasas.Count > 0) // Verifica que haya al menos una casa
        {
            Vector3 casaPosi = posicionCasas[posicionCasas.Count - 1];
            Vector3 spawnPos = new Vector3(casaPosi.x, casaPosi.y - 0.3f, casaPosi.z); // Toma solo la última casa agregada
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
            Instantiate(orcoPrefab, spawnPos, rotation);
        }
    }




}