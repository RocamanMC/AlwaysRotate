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

### [2026-11-07] - AlwaysRotate v3.0 (Marco Nativo de Ventana en Modo Zen y Eliminación de Borde Retro DWM)
- **Corrección del Borde Clásico Retro (Fallback DWM)**:
  - Eliminada la manipulación dinámica de `WindowChrome` en tiempo de ejecución, la cual causaba que Windows perdiera la composición moderna de DWM y dibujara el marco básico clásico de Windows 95/98.
- **Modo Solo Ventana con Marco Nativo Completo**:
  - En **Modo Solo Ventana** (`🗖 Ventana` / `F10`), la ventana conserva 100% su marco nativo moderno de Windows 11 (`WindowStyle.SingleBorderWindow`):
    - Permite agarrar la barra de título superior para mover la ventana con total naturalidad como cualquier app del sistema.
    - Los 4 bordes y esquinas permiten redimensionar de forma nativa fluida.
    - Se ocultan por completo los menús interiores (`HeaderBorder` y `FooterBorder`), dedicando el 100% del área de cliente al contenido rotado sin estorbos.
    - Transición perfecta y sin glitches entre Modo Normal, Modo Solo Ventana y Pantalla Completa.
- **Control de Versiones**: Commits realizados en Git `master`.
