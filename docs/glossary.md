---
layout: page
title: Glossary
permalink: /docs/glossary/
---

# Glossary

### 2D Renderer

2D Renderer是一个MonoBehaviour，用于管理场景里面的Layer和Camera。

### 2D Scene Editor 

2D Scene Editor是用于Unity下的场景编辑插件，专门围绕2D游戏而设计，提供了Layer管理、场景编辑、摄像机设置等功能，并且所见即所得。

### Atlas 

Atlas is a texture container that makes a group of sprites share the same atlas texture and material. It is essentially a list of TextureInfos and their position info.

The Atlas in ex2D can contain sprite textures and Bitmap Font textures at the same time, reducing draw calls even more.

### Bitmap Font 

Bitmap Font is non-scalable font that uses bitmap image to render texts. Compare to scalable fonts like TrueType font, it can't maintain sharpness when scaled up. But the advantage is you can apply different styles like stroke or shadow to the font before generating font texture.

Usually when you create a bitmap font, there will be two files generated, a ".png" font texture and a ".fnt" or ".txt" control file that contain the position information for each character. You can easily create a Bitmap Font using one of the following tools:

- [GlyphDesigner][1] 
- [Hiero][2] 
- [BMFont][3] 

[1]: http://www.71squared.com/
[2]: https://code.google.com/p/libgdx/wiki/Hiero
[3]: http://www.angelcode.com/products/bmfont/

### Draw Call 

To draw an object on the screen, the engine has to issue a draw call to the graphics API (e.g. OpenGL or Direct3D). The graphics API does significant work for every draw call, causing performance overhead on the CPU side. ex2D提供了强大的Atlas管理功能，配合独立内建的批量渲染功能，能够将 draw call 降到最低。

Checkout [How ex2D Renders][4] for more details.

### Dynamic Layer (ex2D)

Sprites in ex2D's Dynamic Layer will be dynamic batched in ex2D's own way. Dynamic Layer allows users to frequently modify sprites in the layer. Compare to Unity's Dynamic Batching, ex2D users can setup different batching parameter such as the mesh size. Depending on the project, you can find a good balance in spending your CPU and GPU time doing batching and rendering.

Checkout [How ex2D Renders][4] for more details.

### Layer (ex2D)

Layer is what ex2D uses to contain sprites in the scene. Not only the GameObject that carries the sprite will be grouped by the layer. All sprites in the same layer will also be batched for faster rendering.

### Sprite Animation

Sprite Animaiton用于播放Sprite帧动画，帧动画之间可以插入自定义事件。每一张帧动画都引用了一个Texture Info，建议把同一个动画里的Texture Info都放在同一张Altas下。

### Static Layer (ex2D)

All sprites in a static layer will be rendered using Static Batching system by ex2D. It's the most efficient way of rendering a group of sprites. Every sprites with the same material in the layer will be combined into a single mesh. It's essentially the same as Unity's static batching so it's the fastest. 

However ex2D allows you to dynamically create and modify Static Layer. This makes it possible to create static batched sprite groups during runtime, thus more convenient for developers. 

Checkout [How ex2D Renders][4] for more details.

[4]: ./how-ex2d-renders.md 

### TextureInfo



