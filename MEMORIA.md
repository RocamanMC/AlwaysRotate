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

### [2026-11-07] - AlwaysRotate v1.9 (Ventana Fantasma Click-Through y Scroll Hardware Serializado)
- **Problema 1: La app oculta bloqueaba la segunda pantalla**:
  - Al ocultar la ventana original (WhatsApp), esta quedaba como una capa invisible (`alpha = 1`) ocupando espacio físico en el segundo monitor, bloqueando cualquier clic del usuario en su navegador, escritorio o carpetas.
  - **Solución**: Se añadió el estilo extendido `WS_EX_TRANSPARENT` a la ventana original en modo fantasma. Ahora es **100% traspasable al ratón**, permitiendo utilizar la segunda pantalla sin ninguna interferencia. Al interactuar desde AlwaysRotate, se desactiva la transparencia durante 30ms para transferir clics/scroll y se vuelve a reactivar de inmediato.
- **Problema 2: El scroll no funcionaba en WhatsApp y se saturaba en la misma pantalla**:
  - WhatsApp Desktop (WinUI 3) descarta mensajes sintéticos `WM_MOUSEWHEEL` por `PostMessage` si no proceden de un evento de hardware. Al volver a simular `mouse_event(WHEEL)` en la misma pantalla, se producía un bucle infinito de eventos en AlwaysRotate.
  - **Solución**:
    - **Candado Antirrecurrencia (`_isForwardingWheel`)**: Corta de inmediato cualquier eco o retorno de evento generado por la propia simulación de la rueda.
    - **Serialización con `lock (_wheelLock)`**: Evita saturación de hilos compitiendo por mover el cursor.
    - **Transparencia dinámica cruzada**: AlwaysRotate se vuelve transparente durante el scroll si se solapa con el destino, y la ventana fantasma recibe el evento de forma instantánea y limpia.
- **Control de Versiones**: Commits realizados en Git `master`.
