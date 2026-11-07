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

### [2026-11-07] - AlwaysRotate v2.9 (Doble Modo de Vista Limpia: Ventana Zen y Pantalla Completa)
- **Implementación de 2 Tipos de Visualización Limpia sin Menús**:
  1. **Modo Solo Ventana (`ViewState.WindowZen` / Botón `🗖 Ventana` / Atajo `F10`)**:
     - Oculta todos los botones, cabeceras y barras de estado molestas.
     - Mantiene la app en una ventana libre y flotante que el usuario puede redimensionar desde cualquier borde o esquina (`WindowChrome` con borde de redimensionamiento nativo de 7px).
     - Permite mover la ventana fácilmente:
       - Mediante el tirador de agarre `⋮⋮` en la barra flotante HUD.
       - Mediante la tira superior discreta de agarre.
       - Manteniendo pulsado `Alt + Clic` en cualquier punto de la pantalla.
  2. **Modo Pantalla Completa (`ViewState.Fullscreen` / Botón `⛶ Completa` / Atajo `F11`)**:
     - Ocupa el monitor entero sin bordes.
- **HUD Flotante Unificado con Conmutación Rápida**:
  - Tanto en Ventana Zen como en Pantalla Completa se dispone del HUD flotante con auto-ocultado tras 2.5s.
  - Incluye botón directo `⛶` / `🗖` para alternar entre ambos modos sin tener que salir al modo normal.
  - El botón `✕` o pulsar la tecla `Esc` restaura inmediatamente la ventana normal con todos los menús.
- **Control de Versiones**: Commits realizados en Git `master`.
