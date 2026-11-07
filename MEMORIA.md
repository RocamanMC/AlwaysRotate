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

### [2026-11-07] - AlwaysRotate v3.1 (Pestaña Completa de Información, Atajos y Ajustes del Sistema)
- **Panel Modal de Información y Ajustes (`InfoOverlay`)**:
  - Incorporado botón `ℹ️ Ayuda` en la barra superior junto al selector de ventana.
  - Diseño moderno y desplazable con fondo atenuado (*backdrop blur/dim*):
    1. **¿Cómo funciona?**: Explicación del motor de captura en vivo a 30 FPS y reenvío matemático de ratón y scroll.
    2. **Atajos de Teclado Globales**: Tabla visual con distintivos de teclas (`Esc`, `F10`, `F11`, `Ctrl+Alt+R`, `Ctrl+Alt+G`, `Ctrl+Alt+→/Espacio`, `Ctrl+Alt+P`, `Alt+Clic`).
    3. **¿Para qué sirve cada botón?**: Guía clara de cada función (`0°/90°/180°/-90°`, `Fijo`, `Clics`, `Ocultar Real`, `Ventana Zen`, `Pantalla Completa`).
    4. **Ajustes del Sistema Integrados**:
       - *Iniciar AlwaysRotate al encender Windows*: Persistencia nativa en el registro de Windows (`HKCU\Software\Microsoft\Windows\CurrentVersion\Run`).
       - *Recordar última aplicación y rotación*: Almacenamiento y restauración automática de preferencias en `config.json`.
    5. **Créditos Oficiales**: Autoría Rocaman con enlace a https://rocaman.vercel.app/.
  - El modal se puede cerrar con el botón `✕`, el botón `Cerrar` o pulsando `Esc`.
- **Repositorio Remoto en GitHub**:
  - Repositorio privado creado en https://github.com/RocamanMC/Utilidades con rama principal `master` vinculada y sincronizada.
- **Control de Versiones**: Commits realizados en Git `master`.
