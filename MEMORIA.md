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

### [2026-11-07] - AlwaysRotate v2.8 (Eliminación de Opacidad y Rediseño Simétrico de Barra Superior)
- **Eliminación del Slider de Opacidad**:
  - Eliminado el control deslizante de opacidad y porcentaje a petición del usuario para simplificar y limpiar la barra de herramientas.
- **Rediseño Armónico de la Barra Superior**:
  - Los 4 botones de rotación (`0° Normal`, `+90° Horario`, `180°`, `-90°`) ahora ocupan una cuadrícula equilibrada de 4 columnas idéntica a la fila inferior de controles (`Fijado`, `Interactuar`, `Ocultar Real`, `Pantalla Completa`), logrando una interfaz limpia, simétrica y espaciosa.
- **Control de Versiones**: Commits realizados en Git `master`.
