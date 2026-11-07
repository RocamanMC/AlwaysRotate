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

### [2026-11-07] - AlwaysRotate v3.3 (Rediseño Estético de Información y Ajustes con Logo Rocaman y Monocraft)
- **Cabecera de Marca con Logo Oficial**:
  - Incorporado el logotipo 3D "ROCAMAN" centrado en alta definición (`rocaman_banner.png`), eliminando texto genérico de autoría.
  - Integrada la tipografía pixelada `Monocraft` tanto como recurso embebido como fuente instalada en el sistema para títulos, insignias y teclas.
- **Tarjetas de Ajustes Modernas**:
  - Toggles visuales de configuración del sistema ("Iniciar con Windows" y "Recordar última ventana y rotación") en tarjetas elevadas y nítidas.
- **Teclas de Atajos Estilo Gaming**:
  - Teclas representadas con insignia `KeyCap` (`Esc`, `F10`, `F11`, `Ctrl+Alt+R`, etc.) con tipografía Monocraft.
- **Tarjetas del Ecosistema Rocaman Interactivas**:
  - 3 accesos directos clickeables con iconos vectoriales y apertura en navegador por defecto:
    1. **Rocaman**: `rocaman.vercel.app` (Portfolio y web principal).
    2. **RocaStore**: `rocastore.vercel.app` (Catálogo y tienda de apps).
    3. **Ziroka**: `ziroka.vercel.app` (Plataforma y proyectos).
- **Control de Versiones**: Commits realizados y sincronizados con GitHub `master`.
- **Eliminación Total de Emojis Obsoletos**:
  - Reemplazados todos los emojis del sistema (`📌`, `🖱️`, `👻`, `⛶`, `🔄`, `⚡`, `ℹ️`, etc.) por vectores `Path` SVG nítidos, estilizados y escalables sin distorsión.
  - El color de los iconos se adapta dinámicamente mediante enlace (`RelativeSource AncestorType=Button`) al estado activo/inactivo de cada control.
- **Solución Definitiva a los Textos Cortados en Botones**:
  - Eliminadas las cadenas largas inyectadas dinámicamente (`Siempre Encima (ON)`, `Interactivo (Clics ON)`).
  - Etiquetas compactas y limpias (`Fijo`, `Clics`, `Ocultar`, `Ventana`, `Completa`) que nunca se truncan ni se desbordan en ninguna resolución de ventana.
  - Indicación de estado ON/OFF visualmente evidente mediante brillo de fondo, bordes de acento de color e iluminación del icono vector.
- **HUD Flotante y Placeholder Vectorizados**:
  - Iconos del HUD (`IcoFsPin`, `IcoFsInteract`, `IcoFsGhost`, `IcoFsToggleMode`) rediseñados en vectores modernos.
  - Placeholder de bienvenida rediseñado con contenedor vectorizado moderno.
- **Compilación y Despliegue**: Compilado con 0 errores y 0 advertencias; binarios publicados en `bin/AlwaysRotate`.
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
