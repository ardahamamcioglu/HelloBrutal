# HelloBrutal

A minimal example project built on the [BRUTAL framework](https://github.com/ahwootech/BRUTAL). It opens a window and
renders a glTF/GLB mesh with a simple orbit camera. Rendering uses Vulkan 1.3 where it is available and
falls back to OpenGL 3.3 where it isn't.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Vulkan 1.3 capable GPU with current drivers, where a Vulkan loader is present
- Otherwise an OpenGL 3.3 capable driver (for example macOS runs on OpenGL unless a Vulkan
  translation layer such as MoltenVK is installed)

## Run

```sh
dotnet run
```

## Controls

- `Tab` — import a `.gltf`/`.glb` model
- Left-drag — orbit
- `Shift` + left-drag — pan
- Scroll, `+`, `-` — zoom
- `Esc` — quit

## Built with BRUTAL

This project is an example of using **BRUTAL**, the free and open source C# game development framework by
[Ahwoo Limited](https://www.ahwoo.com/), available at <https://github.com/ahwootech/BRUTAL>.

BRUTAL is Copyright (c) 2024 Ahwoo Limited and is used here under the **BRUTAL License v1.0** (an MIT-based licence
with attribution requirements). See `LICENSE.txt` in the BRUTAL repository for the full terms.

<img src="LOGO_BRUTAL.png" alt="BRUTAL logo" width="200">
