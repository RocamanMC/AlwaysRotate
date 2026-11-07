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

### [2026-11-07] - AlwaysRotate v2.2 (Eliminación Total de Parpadeo con HTTRANSPARENT y WS_EX_LAYERED)
- **Diagnóstico del parpadeo / cierre momentáneo**:
  - Al desplazar la ventana a $(-15000, -15000)$ para evitar colisiones en la misma pantalla, el Compositor de Windows provocaba que el usuario viera desaparecer y reaparecer la ventana ("como si se cerrara y abriera").
- **Solución definitiva sin parpadeo (Cero movimiento)**:
  - **Eliminación del desplazamiento off-screen**: AlwaysRotate ya no se mueve de sus coordenadas en ningún momento.
  - **Interceptación de mensajes con `WM_NCHITTEST` (`HTTRANSPARENT`)**: Mediante `HwndHook`, durante la simulación de entrada (`_isSimulatingInput` o `_isForwardingWheel`), AlwaysRotate responde a `WM_NCHITTEST` devolviendo `-1` (`HTTRANSPARENT`) y a `WM_MOUSEACTIVATE` devolviendo `MA_NOACTIVATE`. Esto instruye al subsistema de entrada de Windows a ignorar por completo AlwaysRotate y transferir el evento de ratón a la ventana inmediatamente inferior (WhatsApp).
  - **Capa transparente nativa (`WS_EX_LAYERED | WS_EX_TRANSPARENT`)**: En conjunto con `SWP_FRAMECHANGED`, asegura una colisión transparente nativa en el compositor sin redibujar ni mover la ventana.
  - **Resultado**: Clics y scroll 100% operativos en la misma pantalla con **cero parpadeo** y estabilidad visual perfecta.
- **Control de Versiones**: Commits realizados en Git `master`.
