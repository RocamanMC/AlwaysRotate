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

### [2026-11-08] - AlwaysRotate v3.7 (Temas de Decoración Personalizables en la Interfaz)
- **Selector de Temas de Decoración**:
  - Incorporado selector dinámico con 6 paletas de color cuidadosamente diseñadas en la pestaña *Ajustes* del panel de información:
    1. `Indigo` (Índigo Nocturno - Clásico y elegante por defecto).
    2. `Emerald` (Esmeralda Cíber - Inspirado en WhatsApp / Matrix).
    3. `Amethyst` (Amatista Neón - Estilo Cyberpunk / Violeta).
    4. `Crimson` (Carmesí Rubí - Rojo Sunset).
    5. `Amber` (Ámbar Dorado - Cyber Gold).
    6. `Glacier` (Glaciar Nórdico - Azul Hielo / Cyan ártico).
- **Personalización Reactiva en Tiempo Real**:
  - El tema seleccionado transforma instantáneamente:
    - Fondos y bordes de la ventana principal y de cabecera/pie.
    - Superficies y encabezados del modal de ajustes.
    - Distintivo de ángulo en el pie (`BdAngleBadge`) y texto de atajos (`TxtHotkeyHint`).
    - Controles activos de rotación (`0°`, `90°`, `180°`, `270°`), fluidez (FPS) y calidad.
    - Insignia de rotación y borde del HUD flotante en modo pantalla completa.
    - Pestañas activas del modal de configuración.
- **Persistencia**:
  - El tema seleccionado se guarda en `config.json` (`theme`) y se restaura automáticamente en cada inicio de la aplicación.
- **Control de Versiones**: Commits realizados y sincronizados con GitHub `master`.

### [2026-11-08] - AlwaysRotate v3.6 (Detección Completa y Captura de Videollamadas de WhatsApp)
- **Corrección de Filtro Oculto**:
  - Eliminado el filtro previo en `Win32.cs` que descartaba intencionalmente las ventanas con "Video call" o "Call" de WhatsApp.
  - Implementada resolución nativa de UWP para `ApplicationFrameHost` mediante inspección de `Windows.UI.Core.CoreWindow` y mapeo al proceso real `WhatsApp.Root`.
  - Excepción de estilo `WS_EX_TOOLWINDOW`: Se permite capturar ventanas de llamada flotantes y paneles de videollamada de WhatsApp aunque tengan estilo de herramienta.
- **Identificación y Priorización de Videollamadas**:
  - Detección automática y renombrado descriptivo como `WhatsApp - Videollamada` o `WhatsApp (Llamada)`.
  - Ordenación inteligente: si hay una videollamada activa de WhatsApp, aparece en el primer puesto de la lista y se auto-selecciona automáticamente al abrir.
- **Motor de Captura Multi-Tier**:
  - Integrado soporte de captura fallback con `BitBlt` de pantalla directa sobre las coordenadas de la ventana para garantizar que los flujos de vídeo por hardware (GPU / DirectComposition / MediaFoundation) no salgan en negro.
- **Control de Versiones**: Commits realizados y sincronizados con GitHub `master`.

### [2026-11-07] - AlwaysRotate v3.5 (Rediseño Profesional Estilo Fluent con Pestañas y Switches Modernos)
- **Eliminación Total de Listas Tipo IA y Scrollbar Incómodo**:
  - Reemplazado el antiguo modal estrecho y vertical por un panel de dimensiones amplias y proporcionadas (`650x510 px`) sin barras de scroll que corten el texto.
- **Sistema de Pestañas Integrado**:
  - Pestañas modernas en la cabecera: `Ajustes`, `Atajos de Teclado` y `Modos y Guía`.
- **Diseño de Filas Estilo Windows 11**:
  - En la pestaña *Ajustes*, cada opción está en una tarjeta horizontal con título y descripción a la izquierda, y su control a la derecha:
    1. `Fluidez (FPS)`: Segmentado con `30 FPS`, `60 FPS` y `120 FPS`.
    2. `Calidad de Renderizado`: Segmentado con `Alta`, `Bilineal` y `Nítida`.
    3. `Iniciar con Windows`: Toggle Switch estilo Fluent/iOS.
    4. `Recordar Sesión`: Toggle Switch estilo Fluent/iOS.
- **Tabla de Atajos Espaciosa**:
  - Presentación clara en 2 columnas con insignias `KeyCap` para cada atajo sin cortes ni truncamientos.
- **Tarjetas de Modos y Guía**:
  - Cuadrícula de 4 tarjetas dedicadas para *Solo Ventana (F10)*, *Pantalla Completa (F11)*, *Interacción y Clics* y *Modo Fantasma*.
- **Control de Versiones**: Commits realizados y sincronizados con GitHub `master`.

### [2026-11-07] - AlwaysRotate v3.4 (60 FPS por Defecto y Selector de Calidad y Fluidez en Ajustes)
- **Tasa de Refresco a 60 FPS**:
  - Actualizado el motor de captura a 60 FPS por defecto (intervalo de 16 ms) para garantizar scroll ultra suave y respuesta instantánea en WhatsApp y demás ventanas.
- **Selector de Fluidez / FPS en Ajustes**:
  - Incorporado control segmentado en la sección de Ajustes para elegir entre:
    1. `30 FPS (Ahorro)`: Menor consumo de CPU y batería (33 ms).
    2. `60 FPS (Fluido)`: Configuración recomendada y suave (16 ms).
    3. `120 FPS (Ultra)`: Máxima fluidez para monitores gaming de alto refresco (8 ms).
- **Selector de Calidad de Renderizado / Escalado**:
  - Control para alternar el algoritmo de escalado de imagen de WPF en tiempo real:
    1. `Alta (Bicúbica)`: Suavizado Fant/Bicúbico de alta calidad.
    2. `Bilineal`: Escalado estándar rápido.
    3. `Píxel Puro`: Escalado NearestNeighbor con bordes nítidos sin desenfoque.
- **Persistencia en Configuración**: Ambas opciones se guardan en `config.json` y se restauran automáticamente al iniciar.
- **Compilación y Control de Versiones**: Compilado con 0 errores y sincronizado con GitHub `master`.

### [2026-11-07] - AlwaysRotate v3.2 (Estilo Limpio Nativo, Iconos SVG y Corrección de Truncamiento)
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
- **Panel Modal de Información y Ajustes (`InfoOverlay`)**:
  - Diseño sobrio, moderno y nativo de Windows (tipografía del sistema `Segoe UI Variable / Segoe UI`, sin fuentes externas ni bloques innecesarios).
  - Secciones organizadas:
    1. **Ajustes del Sistema**: Toggles integrados para *Iniciar con Windows* y *Recordar última ventana y rotación*.
    2. **Atajos Globales de Teclado**: Tabla de accesos directos con teclas visuales (`KeyCap`).
    3. **Modos y Funciones**: Explicación de los botones de interacción y vistas.
    4. **Pie de Ventana**: Enlace directo y limpio a `rocaman.vercel.app` que abre el navegador predeterminado.
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
