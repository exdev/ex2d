---
layout: page
title: Glossary
permalink: /docs/zh/glossary/
---

# Glossary

### 2D Renderer

2D Renderer 是一个 MonoBehaviour，用于管理场景里面的Layer和Camera。

### 2D Scene Editor 

2D Scene Editor 是用于Unity下的场景编辑插件，专门围绕2D游戏而设计，提供了Layer管理、场景编辑、摄像机设置等功能，并且所见即所得。

### Atlas 

Atlas是一个包含了多个sprite的图集，以确保这些sprite能使用同一张贴图。它实际上包含了 TextureInfo 的列表以及它们的位置信息。

ex2D的Atlas可以同时包含sprite和 Bitmap Font 的贴图，这可以使 draw call 更少。

### Bitmap Font 

Bitmap Font 使用固定大小的字体贴图进行文本渲染，相对于可缩放的矢量字体，它在放大时会变得模糊。但优势是你可以在字体贴图上任意添加不同的字体样式，例如描边和阴影。

一般来说当你创建一个 bitmap font 时，将会有两个文件生成，一张 ".png" 字体贴图和一个 ".fnt" 或 ".txt" 配置文件。你可以使用以下任一工具方便地创建字体。

- [GlyphDesigner][1] 
- [Hiero][2] 
- [BMFont][3] 

[1]: http://www.71squared.com/
[2]: https://code.google.com/p/libgdx/wiki/Hiero
[3]: http://www.angelcode.com/products/bmfont/

### Draw Call 

为了在屏幕上绘制一个物体，Unity需要向图形API(OpenGL 或 Direct3D)提交 draw call. 图形API需要对每个 draw call 做大量的工作，使其成为CPU的一个性能瓶颈。为了减少 draw call，ex2D提供了强大的 Atlas 管理功能，配合独立内建的批量渲染功能，能够将 draw call 降到最低。

前往 [How ex2D Renders][4] 查看更多.

### Dynamic Batching (ex2D)

ex2D中的Layer可以被设置为Dynamic. Dynamic Layer 是最灵活的，适合做动态元素的批量渲染。它支持对layer里的大量sprite进行频繁修改。此外，ex2D可以给每个 Dynamic Layer 单独设定不同的mesh大小，根据不同项目的瓶颈，在CPU与GPU的开销之间取得平衡。

前往 [How ex2D Renders][4] 查看更多.

### Layer (ex2D)

ex2D将场景划分为不同的layer，所有sprite都通过所在的layer进行渲染。Layer之间按照渲染次序进行排列，只要设置了layer，就能保证不同layer之间的sprite的正确渲染次序。而些都可以用2D Scene Editor很方便的进行编辑。此外layer中的不同sprite允许使用各自的material，用户拥有是否合并贴图的选择权。

### Sprite Animation

Sprite Animaiton用于播放Sprite帧动画，帧动画之间可以插入自定义事件。每一张帧动画都引用了一个Texture Info，建议把同一个动画里的Texture Info都放在同一张Altas下。

### Static Layer (ex2D)

ex2D中的Layer可以被设置为Static。Static Layer 是最紧凑的，适合做大量静态元素的批量渲染。在Layer中所有material相同的sprite都会被尽可能放到相同的mesh中，相当于做了 static batching. Static Layer 允许动态修改和创建，使得它比普通的 static batching 灵活了许多，但频繁操作 Static Layer 消耗较高，较为适合放置不经常更新的背景或UI。

前往 [How ex2D Renders][4] 查看更多.

[4]: ./how-ex2d-renders.md 

### TextureInfo



