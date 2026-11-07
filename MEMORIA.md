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

### [2026-11-07] - AlwaysRotate v2.0 (Desescalado Temporal de Topmost en Clics Misma Pantalla)
- **Problema de clic en misma pantalla**:
  - Al estar AlwaysRotate fijada por encima (*HWND_TOPMOST*) en la misma pantalla, si la coordenada de destino en WhatsApp caía bajo el área física de AlwaysRotate, el subsistema de entrada de Windows priorizaba AlwaysRotate por jerarquía Z-Order, entregándole el `WM_LBUTTONDOWN` a la propia AlwaysRotate.
- **Solución implementada en v2.0**:
  - **Desescalado temporal a HWND_NOTOPMOST**: Durante los 45ms del clic, AlwaysRotate cede temporalmente su estatus Topmost (`HWND_NOTOPMOST`) y activa la transparencia con `SWP_FRAMECHANGED`.
  - **Elevación de la app destino**: La ventana de destino se coloca en primer plano (`SetForegroundWindow`), quedando físicamente por encima de AlwaysRotate.
  - **Entrega garantizada del clic**: El `mouse_event` impacta directamente y al 100% en la app de destino.
  - **Restauración instantánea**: Se devuelve el foco y el estatus `HWND_TOPMOST` a AlwaysRotate de forma imperceptible (20ms) en el bloque `finally`.
  - **Descarte de auto-clics**: `ViewportContainer_MouseLeftButtonDown` y `Up` marcan `e.Handled = true` de inmediato si se detecta simulación activa (`_isSimulatingInput`).
- **Control de Versiones**: Commits realizados en Git `master`.
