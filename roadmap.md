---
layout: page
title: ex2D Roadmap
permalink: /roadmap/
---
# Roadmap

This page listed upcoming major version releases and the upcoming feature introduction.

## ex2D v2.0.1

- **3D Sprite**: The 3D Sprite will help users user ex2D developing 3D games. It will generate standalone meshes instead of layer-based meshes for ex2D Renderer. 
This is acutally what Sprites were back in ex2D v1.x, but under the new working pipeline with TextureInfo.
- **TextureInfo Editor**: TextureInfo Editor will allow you edit sliced, tiled and diced sprites. Also allow you edit custom collision and attach point.
  - Customize Sliced, Tiled and Diced information.
  - Customize 2D Collision.
  - Attach Points: Those are custom position defined in TextureInfo that you can use to attach Game Object to specific point on texture during runtime. You can define a hand position in a character sprite texture and attach a sword onto it during sprite animation.
- **Diced Sprite**: Diced sprite allow user dived textures into pieces so you can fit bigger textures into your atlas.
- **Tiled Sprite**: Not to be confused with TileMap. Tiled sprite let you tile one TextureInfo in x, y direction of a given size. You can also apply uv offset. Good for animated background in game.
- *Sliced Sprite*: This is what we call exSpriteBorder in ex2D v1. We make it easier to use.
- **Polish Sprite Inspector user behavior**

## ex2D v2.1.0

- **Pixel Perfect**: The Pixel-Perfect is totally remade in ex2D v2. The algorithm in ex2D v1 suffer problem in execute order for pixel-perfect calculation. Also it 
only make sure the size is perfect for camera but didn't match the pixel in final device (Due to the half-pixel problem for OpenGL and Directx). To correct those things,
we change the way pixel-perfect is done, and provide serveral option based on layer and single sprite units.
- **Clipping**: Since we introduce the layer based meshes, manage clipping rendering become much easier than the previous method. We can make a better editing pipeline for users. We will provide basic Rectangle-Clipping and Nested-Clipping in this version, and polish it to support Path-Clipping and other methods in the feature.
- **3D Clipping**: This is the clipping method we going to support for 3D Sprite.
- **Custom Attach Point**
- **Custom Collision Box**

## ex2D v2.2.0

- **Rework Sprite Animation Editor**: The current layout of sprite animation editor is not so good for designer, and we would like to rework the editor to make preview sprite much easier.
- **Auto Atlasing**: ex2D's atlas system is manually controlled by user. This is good for advance team, but people with small project or demo project can skip the atlas managing process by using Auto-atlas. Auto-atlasing will let user specify a folder, and put the textures inside into an atlas automatically without Atlas Editor.
- **Sprite Preview (TextureInfo Library)**: We will create a preview editor just for TextureInfo, this make user easier to select sprites.
