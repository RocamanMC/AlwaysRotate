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

### [2026-11-07] - AlwaysRotate v2.4 (Simulación de Clics Sin Esperas y Sub-Frame VSync en Misma Pantalla)
- **Corrección Definitiva de Clics en la Misma Pantalla**:
  - Se eliminó el gancho `WM_NCHITTEST` que interfería con la cola de eventos de WPF.
  - Se restauró el despacho por hardware que demostró funcionar al 100% en WhatsApp en v2.1.
  - **Eliminación del parpadeo visual**:
    - El desplazamiento off-screen se condiciona estrictamente a `isCoveredByMe` (si AlwaysRotate no solapa el punto de clic, no se mueve en absoluto).
    - Si se solapa, el desplazamiento, clic y restauración se ejecutan de forma **síncrona e instantánea sin ningún `Thread.Sleep`** intermedio (< 0.1 ms). Al completarse en una fracción minúscula de un solo ciclo de refresco (16.6 ms a 60 Hz), el Compositor de Windows (DWM) no llega a renderizar ningún fotograma ausente, resultando en clics inmediatos en WhatsApp sin ningún parpadeo visible.
- **Control de Versiones**: Commits realizados en Git `master`.
