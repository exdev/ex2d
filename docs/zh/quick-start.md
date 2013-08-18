---
layout: page
title: Quick Start
permalink: /docs/zh/quick-start/
---

## 创建一份 Atlas 资源

在 **Project 窗口** 中选择想要创建的文件夹，在文件夹中点击**鼠标右键**弹出对话框，选择 **Create -> ex2D -> Atlas** 创建出一份 Atlas 资源。

![create_atlas](../../images/create_atlas.png)

## 将贴图导入到 Atlas 中

选择刚刚创建的 Atlas 资源，在 **Inspector** 中点击 **Edit...** 按钮弹出 Atlas 编辑器。

![edit_atlas](../../images/edit_atlas.png)

选择希望导入到 Atlas 的贴图。将选中的贴图拖动到 Atlas 编辑器中的 **预览窗口** 中。

![import_texture_to_atlas](../../images/import_texture_to_atlas.png)

耐心等待进度条完成后，可以看到在 Atlas 的创建目录中出现了一个和Atlas名字相同的文件夹，里头包含了导入的贴图信息文件(TextureInfo)。这些贴图信息文件将会是我们之后主要的操作对象，可以把它们当作包含了原贴图信息和附加信息的资源。

当一切都准备就绪后，我们通过点击 Atlas 编辑器右上角的 **Build** 按钮生成一份 Atlas 贴图。这份贴图也将会保存在刚刚的 Atlas 同名文件夹中。在创建了贴图以后，可以通过点击预览 TextureInfo 资源来检查创建出的 Atlas 贴图是否映射正确。

## 在场景中创建一份 Sprite

首先我们需要创建一个全新的场景，并且设置一个摄像机。点击场景中的主相机，在 **Inspector** 中点击 **Add Component** 按钮并选择 **2D Renderer**。

![add_2d_renderer](../../images/add_2d_Renderer.png)

添加好 **2D Renderer** Component 以后，点击该Component中的 **Edit...** 按钮弹出 2D Scene 编辑器。这是 ex2D 为了方便用户编辑 2D 场景而专门设计的一个场景编辑器。

![edit_2d_renderer](../../images/edit_2d_manager.png)

在创建 Sprite 之前，我们需要为 Sprite 准备一层 Layer 来承载他的绘制信息。通过点击 2D Scene 编辑器中的 Layers 栏目下的 **+** 按钮我们将会创建一份新的 Layer。选中这份 Layer，从 **Project 窗口** 中选择我们希望创建的 TextureInfo 数据，将它们拖入到 2D Scene 编辑器的预览窗口中。这个时候，新的 Sprite 就产生了。

![add_new_layer](../../images/add_new_layer.png)

## 创建一份 Sprite Animation 资源

和创建 Atlas 资源相似，通过在选中目录中，右击鼠标，选择 Create -> ex2D -> Sprite Animation Clip 即可创建一份 Sprite Animation 资源。

![create_sprite_animation_clip](../../images/create_sprite_animation_clip.png)

当选中该份资源后，通过点击 **Inspector** 中的 **Edit...** 将会弹出 Sprite Animation 编辑器来编辑这份动画。

![Edit Sprite Animation](../../images/edit_sprite_animation_clip.jpeg)

我们可以看到 Sprite Animation 编辑器中包含了编辑窗口和预览窗口，将我们希望编辑的 TextureInfo ( **注意：不是贴图而是TextureInfo** ) 拖入到 Sprite Animation 编辑器中的 **编辑窗口**，排列成你希望播放的动画序列帧。

![Import TextureInfo to Sprite Animation](../../images/import_textureinfo_to_sprite_animation.jpeg)

## 在场景中创建一份带动画的 Sprite

如果你已经学会如何创建 Sprite，那么创建一份带动画的 Sprite 是一件非常简单的事情。将你刚刚创建的 Sprite Animation Clip 拖动到 2D Scene 编辑器中即可。

## 创建一份 Bitmap Font 资源

我们假设你已经知道如何通过诸如: [Hiero](https://code.google.com/p/libgdx/wiki/Hiero), [GlyphDesigner](http://www.71squared.com/) 或者 [BMFont](http://www.angelcode.com/products/bmfont/) 的字体工具生成一份BimapFont贴图(.png)和BitmapFont字体信息(.txt,.fnt)。

将生成的字体贴图(.png)和字体信息(.txt,.fnt)文件导入Unity3D项目中。选中字体信息(.txt,.fnt)文件，并右击鼠标。在弹出的菜单中选择: Create -> ex2D -> Bitmap Font，ex2D就会对字体信息文件做解析并且创建出一份 BitmapFont 资源。

![Create Bitmap Font](../../images/create_bitmapfont.jpeg)

## 在场景中创建一份 Sprite Font

我想你已经预先知道该如何创建一份 Sprite Font 了。和 Sprite, Sprite Animation 类似，你只需要将 Bitmap Font 拖入到 2D Scene 编辑器中的编辑窗口中即可。

![Add Bitmap Font](../../images/add_bitmapfont_to_scene.jpeg)
