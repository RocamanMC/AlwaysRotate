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

### [2026-11-07] - AlwaysRotate v2.3 (Modo Pantalla Completa / Vista Zen con Botón Flotante 'X')
- **Nueva Funcionalidad de Pantalla Completa**:
  - Incorporado botón `⛶ Pantalla` en la barra de herramientas superior y atajos globales `F11` y `Esc`.
  - Al activarse:
    1. Se ocultan por completo todas las barras de herramientas (selector, rotación, opacidad, botones) y la barra de estado inferior.
    2. La ventana se expande a pantalla completa sin bordes (*borderless fullscreen*), permitiendo que la ventana clonada ocupe el 100% del monitor con escalado limpio.
    3. Aparece un **botón flotante '✕'** elegante y translúcido en la esquina superior derecha (`BtnExitFullscreen`).
    4. Al hacer clic en la '✕' o pulsar la tecla `Esc` (o `F11`), se sale instantáneamente de pantalla completa, restaurando todos los botones y dimensiones originales.
- **Control de Versiones**: Commits realizados en Git `master`.
