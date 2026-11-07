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

### [2026-11-07] - AlwaysRotate v2.1 (Desplazamiento Físico Off-Screen Instantáneo en Clics Misma Pantalla)
- **Diagnóstico definitivo de clics en la misma pantalla**:
  - Aunque se modificaran los estilos y flags Win32 (`WS_EX_TRANSPARENT`, `HWND_NOTOPMOST`), el Compositor de Windows (DWM) no actualizaba instantáneamente el árbol de colisiones en la GPU en ventanas WPF con aceleración hardware, provocando que el ratón físico siguiera colisionando con AlwaysRotate.
- **Solución física absoluta implementada en v2.1**:
  - **Desplazamiento Win32 fuera de pantalla (-15000, -15000)**: En el instante de despachar el clic o el scroll, AlwaysRotate se mueve físicamente a $(-15000, -15000)$ mediante `SetWindowPos` con `SWP_NOSIZE | SWP_NOACTIVATE`. Al no estar físicamente en esa zona de la pantalla durante esos 45ms, es **físicamente imposible** que AlwaysRotate intercepte el clic.
  - El clic o la rueda de ratón cae sin obstáculo alguno sobre la ventana de WhatsApp.
  - Al concluir la simulación (50ms), AlwaysRotate regresa a sus coordenadas exactas de pantalla `(origX, origY)` con su propiedad `HWND_TOPMOST` intacta, sin parpadeo perceptible para el ojo humano.
- **Control de Versiones**: Commits realizados en Git `master`.
