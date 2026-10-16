# HelloBrutal

A minimal example project built on the [BRUTAL framework](https://github.com/ahwootech/BRUTAL). It opens a window and
renders a glTF/GLB mesh with a simple orbit camera. Rendering uses OpenGL 3.3 on macOS and Vulkan everywhere else.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Windows/Linux: a Vulkan 1.3 capable GPU with current drivers
- macOS: runs on the OpenGL path, no extra setup

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

![alt text](LOGO_BRUTAL.png)
