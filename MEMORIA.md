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

### [2026-11-07] - AlwaysRotate v2.6 (Barra de Herramientas Flotante HUD en Pantalla Completa)
- **HUD Flotante Integrado en Pantalla Completa**:
  - Sustituido el botón único de salida por una barra flotante (*glassmorphism HUD*) en la esquina superior derecha (`FullscreenHud`) visible exclusivamente en pantalla completa.
  - Botones funcionales integrados:
    1. **`↻ +90°`**: Rota en incrementos horarios de 90° mostrando el ángulo actual en vivo.
    2. **`📌 Fijado`**: Conmuta AlwaysOnTop con respuesta visual (ámbar activo).
    3. **`🖱️ Clics`**: Conmuta el modo de interacción directa y reenvío de clics/scroll (esmeralda activo).
    4. **`👻 Fantasma`**: Conmuta el modo de invisibilidad de la ventana original (púrpura activo).
    5. **`✕ Salir`**: Sale del modo pantalla completa devolviendo el tamaño y controles estándar.
  - Sincronización bidireccional de estilos y estados reactivos entre la interfaz normal y el HUD de pantalla completa.
- **Control de Versiones**: Commits realizados en Git `master`.
