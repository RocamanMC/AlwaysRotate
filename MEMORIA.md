# Memoria del Proyecto: Utilidades PC (PowerToys Custom)

## 📌 Objetivo General
Desarrollar una suite de herramientas y utilidades de sistema para Windows (similar a Microsoft PowerToys, pero optimizada, ligera, robusta y sin los fallos/bloqueos habituales de PowerToys) orientada al PC del usuario y distribuible a otros equipos.

---

## 📋 Normas y Directrices del Proyecto
1. **Fiabilidad ante todo**: Cada utilidad debe ser ligera, estable y no dejar procesos colgados ni fugas de memoria.
2. **Modularidad**: Cada utilidad o módulo debe poder funcionar de manera independiente o bajo un lanzador/bandeja del sistema central.
3. **Registro continuo**: Mantener esta memoria actualizada con decisiones técnicas, componentes implementados y roadmap.
4. **Control de Versiones (Git)**: Control de versiones activo en el repositorio local.

---

## 🛠️ Registro de Actividades y Decisiones

### [2026-11-07] - Inicialización del Proyecto y Repositorio Git
- Creación de la estructura base del repositorio y memoria privada.
- Definición de objetivo: alternativa propia y estable a PowerToys.
- Repositorio Git inicializado con `.gitignore` oficial y commits estructurados.

### [2026-11-07] - AlwaysRotate v2.7 (Auto-ocultado de HUD y Atajos de Teclado con Hook de Bajo Nivel)
- **Auto-ocultado Suave de HUD en Pantalla Completa**:
  - Implementado `_hudFadeTimer` (2.5 segundos): tras inactividad del cursor en pantalla completa, la barra HUD se desvanece suavemente a transparente (`Opacity = 0.0`).
  - Al detectar movimiento del ratón o situar el cursor cerca de la zona, reaparece al instante (`Opacity = 1.0`).
  - Pausa inteligente del temporizador si el puntero está situado sobre cualquiera de los controles del HUD.
- **Hook Global de Teclado de Bajo Nivel (`WH_KEYBOARD_LL`)**:
  - Solucionado el problema donde los atajos no respondían al interactuar con WhatsApp (ya que Windows entrega el foco de teclado a la ventana destino).
  - El hook de bajo nivel captura de forma 100% fiable e instantánea:
    - `Esc`: Salir de pantalla completa en cualquier momento.
    - `F11`: Alternar modo pantalla completa.
    - `Ctrl + Alt + R`: Capturar y rotar la ventana activa actual.
    - `Ctrl + Alt + G`: Alternar modo fantasma (ocultar/mostrar original).
    - `Ctrl + Alt + Flecha Derecha` / `Ctrl + Alt + Espacio`: Rotar +90° al vuelo.
    - `Ctrl + Alt + Flecha Izquierda`: Rotar -90°.
    - `Ctrl + Alt + P`: Alternar Siempre encima (Pin).
    - `Ctrl + Alt + I`: Alternar modo interactivo de clics.
- **Control de Versiones**: Commits realizados en Git `master`.
