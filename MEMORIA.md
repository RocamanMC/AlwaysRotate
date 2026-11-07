# Memoria del Proyecto: Utilidades PC (PowerToys Custom)

## 📌 Objetivo General
Desarrollar una suite de herramientas y utilidades de sistema para Windows (similar a Microsoft PowerToys, pero optimizada, ligera, robusta y sin los fallos/bloqueos habituales de PowerToys) orientada al PC del usuario y distribuible a otros equipos.

---

## 📋 Normas y Directrices del Proyecto
1. **Fiabilidad ante todo**: Cada utilidad debe ser ligera, estable y no dejar procesos colgados ni fugas de memoria.
2. **Modularidad**: Cada utilidad o módulo debe poder funcionar de manera independiente o bajo un lanzador/bandeja del sistema central.
3. **Registro continuo**: Mantener esta memoria actualizada con decisiones técnicas, componentes implementados y roadmap.

---

## 🛠️ Registro de Actividades y Decisiones

### [2026-11-07] - Inicialización del Proyecto
- Creación de la estructura base del repositorio y memoria privada.
- Definición de objetivo: alternativa propia y estable a PowerToys.

### [2026-11-07] - AlwaysRotate v1.4 (Iconos Nativos Reales, Filtrado Alt-Tab y Corrección de Clics)
- **Iconos Nativos de Cada Aplicación**:
  - Reemplazados los emojis por extracción directa del icono nativo de cada programa (`ExtractAssociatedIcon` + `WM_GETICON` + `GetClassLongPtr`). Ahora WhatsApp, Chrome, Discord, Obsidian, Spotify, etc., muestran su icono oficial original en alta resolución.
- **Filtrado de Ventanas Fantasma / Ocultas (DWM Cloaking)**:
  - Implementado filtro con `DwmGetWindowAttribute(DWMWA_CLOAKED)`. Se eliminan todas las ventanas en segundo plano o servicios UWP invisibles ("Video call", interfaces fantasma de configuración, etc.). Solo aparecen las ventanas reales abiertas estilo Alt+Tab.
- **Auto-Restauración de Ventanas**:
  - Si una ventana seleccionada está minimizada o no visible físicamente, AlwaysRotate la restaura automáticamente con `SW_RESTORE` para que se abra y esté lista para capturarse.
- **Corrección Definitiva del Modo Interactivo**:
  - Descubierto y corregido error en la transformación de coordenadas: `LayoutTransform` en WPF ya invierte la rotación en `GetPosition()`, por lo que re-aplicar la rotación invertía dos veces las coordenadas. Ahora el mapeo es 1:1 exacto al píxel.
  - Implementado clic físico suave en segundo plano con temporización de 35ms para evitar cancelaciones de Windows/Chromium.
- **Icono Oficial de Barra de Tareas**:
  - Reconstruido el archivo `.ico` con las 7 resoluciones estándar de Windows (16x16 hasta 256x256).
  - Forzado mediante `WM_SETICON` y `BitmapFrame` para que Windows no muestre los dos cuadrados blancos por defecto.
