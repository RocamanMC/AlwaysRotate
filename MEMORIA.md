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

### [2026-11-07] - AlwaysRotate v2.5 (Nuevo Icono Oficial: Cristales Orbitantes Púrpura y Azul)
- **Actualización de Identidad Visual**:
  - Incorporado el nuevo icono oficial con cristales morados y órbitas rotatorias cian.
  - Procesamiento con algoritmo de inundación (*flood-fill*) para transparentar el fondo exterior blanco manteniendo intactos los brillos y reflejos especulares interiores de los cristales.
  - Generado paquete multi-resolución en `app_icon.ico` (desde 16x16 hasta 256x256) y `app_icon.png`.
  - Integración nativa en la barra de tareas de Windows:
    - Registro de `SetCurrentProcessExplicitAppUserModelID("Rocaman.AlwaysRotate")` en `App.OnStartup` para desvincular el proceso de hosts genéricos y agruparlo con identidad propia en la Shell de Windows.
    - Soporte dinámico de DPI con `GetSystemMetrics(SM_CXICON)` / `GetSystemMetrics(SM_CYICON)` para envío por `WM_SETICON`.
    - Incrustación de recurso nativo PE en `AlwaysRotate.exe`.
- **Control de Versiones**: Commits realizados en Git `master`.
