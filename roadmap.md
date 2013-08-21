---
layout: page
title: ex2D Roadmap
permalink: /roadmap/
---

## ex2D v2.0.1

- **3D Sprite**: The 3D Sprite will help users user ex2D developing 3D games. It will generate standalone meshes instead of layer-based meshes for ex2D Renderer. 
This is acutally what we done in ex2D v1.x, but under the new working pipeline with TextureInfo.
- **TextureInfo Editor**: TextureInfo Editor will allow you edit sliced, tiled and diced sprites. Also allow you edit collision box and attach point.
  - Customize Sliced, Tiled and Diced information.
  - Customize Collision Box.
  - Customize Attach Points.
- **Diced Sprite**: Diced sprite allow user pieces textures so that long texture can be fit to atlas texture size and not break the rendering in the scene.
- **Tiled Sprite**: Unlike TileMap. Tiled sprite can let you tile one texture info in x, y direction also apply uv offset. Good for animated background in the game.
- *Sliced Sprite*: This is what we call exSpriteBorder in ex2D v1. We make it easier to use.
- **Polish Sprite Inspector user behavior**

## ex2D v2.1.0

- **Pixel Perfect**: The Pixel-Perfect is totally re-make in ex2D v2. The algorithm in ex2D v1 suffer problem in execute order for pixel-perfect calculation. Also it 
only make sure the size is perfect for camera but didn't martch the pixel in final device (Due to the half-pixel problem for OpenGL and Directx). To correct the things,
we change the way doing pixel-perfect, and provide serveral option based on layer and single sprite units.
- **Clipping**: Since we introduce the layer based meshes, manage clipping draws become more easier than we think. So we can make a great editor pipeline for user to improve
clipping editing. We will provide basica Rectangle-Clipping and Nested-Clipping in this version, and polish it to support Path-Clipping and others in the feature.
- **3D Clipping**: This is the clipping method we going to support for 3D Sprite.
- **Custom Attach Point**
- **Custom Collision Box**

## ex2D v2.2.0

- **Rework Sprite Animation Editor**: We think current layout of sprite animation editor is not so good for designer, and we wish rework on it so edit and preview sprite can
be more easier.
- **Auto Atlasing**: ex2D's atlasing system is basically manually control for user. This is good for advance team, but people with small project or demo project will become slow 
and heavy in developing. Auto-atlasing will detect the directory user specific, and put their textures in atlas automatically without Atlas Editor.
- **Sprite Preview (TextureInfo Library)**: We will create a preview editor just for TextureInfo, this make user easier to select sprites.
