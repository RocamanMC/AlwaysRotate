<div align="center">

# 🔄 AlwaysRotate

### *Rotación dinámica en tiempo real (60 / 120 FPS) con reenvío matemático de clics para Windows y videollamadas de WhatsApp*

[![Platform](https://img.shields.io/badge/Plataforma-Windows%2010%20%7C%2011-0078d4.svg?style=for-the-badge&logo=windows)](https://microsoft.com)
[![Framework](https://img.shields.io/badge/.NET-10.0%20Windows-512bd4.svg?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![UI](https://img.shields.io/badge/UI-WPF%20Fluent%20Clean-6366f1.svg?style=for-the-badge)](https://github.com/RocamanMC/AlwaysRotate)
[![Status](https://img.shields.io/badge/Modo-100%25%20Portable-10b981.svg?style=for-the-badge)](https://github.com/RocamanMC/AlwaysRotate)
[![Author](https://img.shields.io/badge/Creador-Rocaman-a855f7.svg?style=for-the-badge&logo=vercel)](https://rocaman.vercel.app/)

</div>

---

## 📌 ¿Qué es AlwaysRotate?

**AlwaysRotate** es una utilidad de sistema ligera y de alto rendimiento diseñada para resolver de forma definitiva el problema de las aplicaciones y videollamadas que se transmiten o abren en orientación incorrecta (vertical / apaisada) en monitores de PC.

A diferencia de capturadoras pesadas o soluciones torpes por software, AlwaysRotate captura el buffer gráfico de la ventana objetivo a **60 o 120 FPS**, le aplica rotación geométrica instantánea (**0°, 90°, 180°, 270°**) y transforma matemáticamente las coordenadas del ratón en tiempo real, permitiéndote **hacer clic, escribir, interactuar y hacer scroll directamente sobre la imagen rotada**.

---

## ✨ Características Principales

| Función | Descripción |
| :--- | :--- |
| 🚀 **60 y 120 FPS Ultra-Fluidos** | Motor de captura con búfer de memoria optimizado para tasas de refresco de hasta 120 fotogramas por segundo, ideal para monitores gaming y videollamadas sin stuttering. |
| 📞 **Detección Especial de WhatsApp** | Mapeo nativo de ventanas `ApplicationFrameHost` / `CoreWindow` de WhatsApp y videollamadas en tiempo real sin pantallas negras (soporte multi-tier con captura directa por hardware). |
| 🖱️ **Interacción y Clics Matemáticos** | Conversión vectorial inversa de coordenadas. Puedes pulsar botones, contestar chats y usar la rueda del ratón directamente sobre la app rotada. |
| 👻 **Modo Fantasma (Ghost Window)** | Vuelve la ventana original de Windows **100% invisible** para eliminar duplicados en tu pantalla mientras sigues interactuando sobre el visor de AlwaysRotate. |
| 🖥️ **3 Modos de Visualización** | Alterna al instante entre modo normal, **Ventana Zen (F10)** (sin menús y redimensionable libremente) y **Pantalla Completa (F11)** con barra flotante (HUD) auto-ocultable. |
| 🎨 **6 Temas de Color Personalizables** | Paletas de acento dinámicas en tiempo real: **Índigo** (clásico), **Esmeralda** (WhatsApp/Matrix), **Amatista** (Cyberpunk), **Carmesí** (Rubí), **Ámbar** (Dorado) y **Glaciar** (Cyan). |
| 🔍 **Algoritmos de Escalado Gráfico** | Elige entre **Alta** (suavizado bicúbico continuo), **Bilineal** (bajo coste de CPU/batería) y **Nítida** (píxel puro 1:1 Nearest Neighbor para texto y código). |
| 📦 **100% Portable** | No requiere instalación ni dependencias externas; recuerda tu última ventana, tamaño y rotación en `config.json`, con interruptor para arrancar con Windows. |

---

## ⌨️ Atajos de Teclado Globales

Puedes controlar AlwaysRotate en cualquier momento mediante atajos rápidos de teclado:

| Atajo | Acción |
| :---: | :--- |
| <kbd>Esc</kbd> | Salir de Pantalla Completa o Modo Solo Ventana / Cerrar panel de ajustes |
| <kbd>F10</kbd> | Alternar **Modo Solo Ventana** (sin menús, marco nativo redimensionable) |
| <kbd>F11</kbd> | Alternar **Pantalla Completa** total en el monitor |
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>R</kbd> | Capturar y rotar automáticamente la ventana que tengas activa en Windows |
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>G</kbd> | Alternar **Modo Fantasma** (ocultar / mostrar ventana original) |
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>→</kbd> / <kbd>Espacio</kbd> | Girar **+90°** al vuelo |
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>P</kbd> | Alternar **Fijo** (Always on Top / Siempre encima de otras ventanas) |
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>I</kbd> | Alternar reenvío de clics del ratón (Modo interactivo / Solo visor) |
| <kbd>Alt</kbd> + Clic Arrastre | Mover la ventana libremente arrastrando desde cualquier parte del visor |

---

## 🎨 Modos de Calidad y Escalado

En el panel de **Ajustes** puedes seleccionar y hacer clic sobre cualquier algoritmo para adaptar el renderizado a tu contenido:

- **🔹 Alta (Bicúbica / Fant)**: Muestreo continuo que suaviza diagonales, curvas y elimina dientes de sierra.  
  👉 *Recomendado para: Videollamadas, cámaras web, streaming y vídeos.*
- **🔸 Bilineal**: Interpolación rápida con el mínimo impacto en recursos de GPU y batería.  
  👉 *Recomendado para: Equipos portátiles o PCs con hardware modesto.*
- **🟢 Nítida (Píxel Puro / Nearest Neighbor)**: Mapeo exacto 1:1 sin difuminado ni filtros gaussianos.  
  👉 *Recomendado para: Lectura de textos pequeños, terminales, código fuente, emuladores y pixel art.*

---

## 🚀 Inicio Rápido

1. Abre la carpeta de la aplicación:
   ```text
   bin\AlwaysRotate\AlwaysRotate.exe
   ```
2. **Selecciona la ventana**: AlwaysRotate detecta automáticamente si WhatsApp o una videollamada está activa y la selecciona en primer lugar. También puedes elegir cualquier otra ventana desde el desplegable superior.
3. **Elige la rotación**: Pulsa `0°`, `90°`, `180°` o `270°`.
4. **Interactúa**: Tus clics y la rueda del ratón funcionarán en tiempo real sobre la ventana como si fuera nativa.

> [!TIP]
> Si deseas que AlwaysRotate se abra automáticamente al iniciar sesión en tu PC, ve al botón **Ayuda / Ajustes** y activa el interruptor **"Iniciar con Windows"**.

---

## 🛠️ Tecnologías Utilizadas

- **Lenguaje**: C# 13 / .NET 10.0 (Windows Desktop)
- **Framework de Presentación**: Windows Presentation Foundation (WPF) con renderizado `Display` / `ClearType` y aceleración de hardware DirectX
- **Win32 Interop**: `user32.dll`, `gdi32.dll`, `dwmapi.dll`, Hooks de teclado de bajo nivel (`WH_KEYBOARD_LL`), Hotkeys globales y `SetWindowDisplayAffinity`
- **Formato**: Aplicación 100% portable y autónoma

---

## 👨‍💻 Autor

Desarrollado por **Rocaman**  
🌐 Página web oficial: [rocaman.vercel.app](https://rocaman.vercel.app/)

---

<div align="center">
<sub>AlwaysRotate forma parte del ecosistema de utilidades de sistema personalizadas de Rocaman.</sub>
</div>
