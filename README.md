# 🧭 DataMiner 🧭

**DataMiner** es un videojuego desarrollado en Unity que combina mecánicas de exploración, construcción y aprendizaje, inspirado en la generación procedural de mundos al estilo voxel como *Minecraft*. Utiliza algoritmos de **Perlin Noise** para crear terrenos únicos en cada partida, ofreciendo una experiencia dinámica y educativa.

---

## 🎮 Características principales

- 🌍 **Generación procedural de terreno**  
  Cada mundo se crea aleatoriamente utilizando Perlin Noise, garantizando una experiencia diferente en cada sesión.

- 🏗️ **Mecánicas de exploración**  
  El jugador puede recorrer y explorar libremente el mapa en busca de estructuras.

- 🧠 **Preguntas**  
  Al interactuar con los **NPCs** dentro de ciertas estructuras, se presentan preguntas relacionadas con estadística, matematicas, sociales y biología.

- 💎 **Sistema de puntuación por gemas**  
  Cada respuesta correcta otorga una cantidad de **gemas** visibles en la parte superior derecha de la pantalla.

- ❌ **Condiciones de victoria y derrota**  
  - El juego se completa al acumular **35 gemas**.  
  - Se pierde al fallar **3 preguntas**.


## 🛠️ Tecnologías utilizadas

- [Unity](https://unity.com/) (versión recomendada: 2019.4 o superior)
- C# para lógica de juego y control de eventos
- Perlin Noise para generación de mapas
- Sketchfab (complemento) para modelos en 3D gratuitos

## 📷 **Capturas del juego**
- Menu principal
![Captura](imgs/menu_principal.png)

- Generacion procedural
<div align="center">
  <img src="imgs/principal.png" width="800" />
  <img src="imgs/mundo.png" width="800" />
</div>

- Chunk
![Captura](imgs/chunk.png)

- Estructura
<div align="center">
  <img src="imgs/casa.png" width="800" />
  <img src="imgs/casa_interior.png" width="800" />
</div>

- NPCs
<div align="center">
  <img src="imgs/npc1.png" width="800" />
  <img src="imgs/npc2.png" width="800" />
  <img src="imgs/npc3.png" width="800" />
  <img src="imgs/npc4.png" width="800" />
</div>

- Estructura de las preguntas
![Captura](imgs/pregunta.png)


## ▶️ **Trailer**  
<a href="https://www.youtube.com/watch?v=8-ZmSIuK3M8" target="_blank">
  <img src="https://img.youtube.com/vi/8-ZmSIuK3M8/hqdefault.jpg" width="950" height="700" alt="Ver video">
</a>


## 🚀 Modo de Uso

1. Clona el repositorio:
   ```bash
   git clone https://github.com/JeisonAlexis/DataMiner-Unity-3D-.git
   cd DataMiner-Unity-3D-
2. Abrir Unity Hub
3. Click en Add
4. Click en open project from disk (Version recomendada de Unity 2019.4
5. Abrir el proyecto

## 📈📉 Observaciones y/o posibles mejoras

1. Mejoras de rendimiento:
   - Mitigar la alta carga computacional en la creacion de chunks
   - Sistema de guardado y cargado de chunks (los chunks siempre estan cargados incluso aunque el jugador este lejos)
   - Mejorar el sistemas de cargado de la mesh de los cubos (la cara al exterior de cada chunk se carga aunque este tapada)
2. Bugs:
   - Ciertos parametros de generacion permiten escenarios donde hay mas de 1 NPC por estructura lo que no esta contemplado
   - Con ciertos paramatros de distancia el juego permite colocar bloques de forma que obstaculicen directamente al jugador
3. Mejoras funcionales:
   - Sistema de guardado y cargado de mundos
   - Sistema de inventario
   - Sistema de combate
   - Sistema de ciclo dia/noche

**Autor**
- Jeison Alexis Rodriguez Angarita 🙍‍♂️
- Programación Orientada a Plataformas / Ingenieria de Sistemas / Universidad de Pamplona 👨‍🎓
- 2025 📅 
