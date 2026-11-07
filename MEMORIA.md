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

### [2026-11-07] - AlwaysRotate v1.5 (Arreglo Total de la Interacción y Rotación por Bitmap)
- **Causa raíz del fallo en interacción identificada**:
  - `LayoutTransform` en WPF causaba que `GetPosition()` arrojara coordenadas negativas al rotar 90°, provocando que la validación de límites descartara el 100% de los clics.
  - Además, se estaban enviando mensajes `PostMessage` duplicados simultáneamente con `mouse_event`, causando conflicto de eventos en Chromium/WhatsApp.
- **Solución implementada**:
  - Migración a rotación física por tarjeta gráfica en el bitmap (`TransformedBitmap`).
  - El control de imagen en WPF opera en coordenadas estándar sin distorsión de matriz.
  - Mapeo trigonométrico directo y limpio:
    - 90°: `X = v * W`, `Y = (1 - u) * H`.
    - 180°: `X = (1 - u) * W`, `Y = (1 - v) * H`.
    - 270°: `X = (1 - v) * W`, `Y = u * H`.
    - 0°: `X = u * W`, `Y = v * H`.
  - Despacho de clics físicos puros con temporización de 35ms y desplazamiento de rueda (`MOUSEEVENTF_WHEEL`).
- **Control de Versiones**: Commits realizados en Git `master`.
