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

### [2026-11-07] - AlwaysRotate v1.8 (Estabilidad en Misma Pantalla, Scroll sin Jitter y Clics en Modo Fantasma)
- **Problema 1: Se petaba al tener ambas ventanas en la misma pantalla**:
  - Al girar una ventana en la misma pantalla y hacer scroll, `mouse_event(WHEEL)` situaba el cursor sobre la pantalla física donde AlwaysRotate estaba como Topmost. Windows reenviaba el evento a AlwaysRotate, generando un bucle infinito recursivo y bloqueando la UI.
  - Al hacer clic en la misma pantalla, AlwaysRotate interceptaba el propio clic simulado.
- **Problema 2: Los clics no se detectaban en Modo Fantasma**:
  - Al activar el Modo Fantasma se movía la ventana original a $X = -5000$. Windows restringe la posición física del cursor de ratón a los límites de las pantallas activas (Virtual Desktop), por lo que `SetCursorPos` no podía alcanzar $-5000$ y el clic caía en un punto arbitrario.
- **Solución implementada en v1.8**:
  - **Scroll directo por mensajes (`WM_MOUSEWHEEL`)**: Se sustituyó la simulación por hilos físicos por `Win32.PostMessage(WM_MOUSEWHEEL, ...)`, transmitiendo el delta y coordenadas exactas tanto a la ventana principal como a los controles hijos (`WindowFromPoint`). Se eliminó por completo el jitter del cursor, la contención de hilos y la posibilidad de bucles recursivos.
  - **Transparencia temporal al clic (`WS_EX_TRANSPARENT`)**: Si AlwaysRotate y la ventana original se solapan físicamente en la misma pantalla, AlwaysRotate se vuelve momentáneamente transparente a los clics mediante su estilo extendido, asegurando que el clic atraviese limpiamente hasta la app original.
  - **Modo Fantasma en coordenadas reales**: La ventana original se oculta mediante `WS_EX_LAYERED` y opacidad 1/255 (100% invisible para el ojo humano) en su ubicación real, permitiendo que las coordenadas de pantalla para clics y scroll sean siempre válidas y exactas.
  - **Extracción robusta de iconos nativos y filtrado**: Uso de `QueryFullProcessImageName` (con permisos de consulta limitados) para obtener iconos reales de procesos protegidos y UWP (WhatsApp, Herdr, Discord, Chrome, etc.), filtrando además subventanas invisibles como "Video call".
  - **Icono oficial en barra de tareas**: Icono multi-resolución `app_icon.ico` asignado mediante `WM_SETICON` y `ApplicationIcon`.
- **Control de Versiones**: Commits realizados en Git `master`.
