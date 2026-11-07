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

### [2026-11-07] - AlwaysRotate v1.7 (Corrección Definitiva del Clic de Ratón)
- **Diagnóstico del problema de clics**:
  - El scroll funcionaba porque en Windows la rueda del ratón no requiere foco ni captura; se despacha a la ventana bajo el cursor.
  - El clic fallaba porque se enviaba durante el evento `MouseDown`: en ese instante, el dedo del usuario seguía físicamente presionando el botón izquierdo sobre AlwaysRotate. AlwaysRotate mantenía la captura del ratón en Windows (`MouseCapture`), absorbiendo la simulación e impidiendo que WhatsApp la procesara. Además, WhatsApp requería activación de foco en primer plano para registrar clics en sus controles WinUI.
- **Solución implementada**:
  - Los clics interactivos ahora se despachan en `MouseLeftButtonUp` (al soltar el botón, cuando el ratón físico está liberado y sin captura).
  - Liberación explícita de captura con `Mouse.Capture(null)`.
  - Activación instantánea de la ventana de destino con `Win32.SetForegroundWindow`.
  - Simulación física con temporización de 40ms y restauración de cursor.
- **Control de Versiones**: Commits realizados en Git `master`.
