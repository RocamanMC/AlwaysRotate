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

### [2026-11-07] - AlwaysRotate v1.6 (Modo Fantasma: Ocultar Original Invisible)
- **Implementación del Modo Fantasma (`👻 Ocultar Real`)**:
  - Permite volver 100% invisible la ventana original de WhatsApp en pantalla (`SetLayeredWindowAttributes` con `Alpha = 1`).
  - Para los ojos humanos es completamente invisible (no estorba en ningún monitor).
  - Para Windows y la tarjeta gráfica sigue activa y sólida: AlwaysRotate la sigue capturando en tiempo real y puede enviarle clics y eventos de ratón.
  - Al desactivar el botón o al cerrar AlwaysRotate, la ventana original recupera su visibilidad normal (`Alpha = 255`) automáticamente.
- **Control de Versiones**: Commits realizados en Git `master`.
