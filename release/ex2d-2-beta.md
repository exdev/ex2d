---
layout: page
title: 2.0.1 Beta Release Note 
permalink: /release/ex2d-2-beta/
---

# 2.0.1 Beta Release Note

## New Features

+ 2D Scene Editor to manager sprites, layers
  + Embeded layer manager.
  + Manage sprite draw order with Layer Manager and Depth Bias.
  + Automatically synchornize depth settings with Hierarchy Window.
  + Drag and drop creating Sprite-based GameObject from assets.
  + Move, rotate and scale sprites directly in the scene editor.
  + Edit sprite properties directly through Unity Inspector, share the same result with Unity Scene/Game View.
  + Provide guide lines for easy aligment sprite elements.
+ Highly optimized 2D renderer focus on fast drawing 2D sprites ( [How ex2D Renders]({{ site.baseurl }}/docs/how-ex2d-renders/) )
  + Manually combine Mesh in our own 2D Renderer.
  + Edit layer-meshes through ex2D Scene Editor.
  + Automatically detect and switch between static and dynamic batches. 
  + Smart combine meshes if they are static.
  + Strictly design the initalize, sorting and drawing phase, make user's scripts have no pain to run with ex2D's low-level code.
+ New working pipeline for sprite assets and editors.
  + Assets pipeline is now compatible with team collabration.
  + Introduc basic asset TextureInfo as operation target.
  + Change ex2D Editors (such as atlas editor, sprite animation editor, ...) to work with TextureInfo.
  + Embed preview in the inspector for ex2D assets.
+ Allow rotated atlas element when layout atlas.

## Changes

+ Remove exAtlasInfo. ( There is no need to distinguish exAtlas and exAtlasInfo. Instead, only exAtlas )
+ Temporarily disable auto synchornize atlas assets when texture changes. ( Auto-atlasing will become a new feature in future updates, we still need more time test this feature )
+ Temporarily disable texture to atlas detecting in the scene, use Texture-Info instead. 
( This is a nice feature in ex2D v1.+, however it also lead to crashes, out of memory and break team-work since it depends on local cache file --- "Atlas DB",
  we are seeking a new way to make Texture and TextureInfo can be smartly linked together,
  we also think about provide Manually/Automatically option so different team can choose their way work with ex2D )
+ Embed the layer manager and editor in 2D Scene Editor. ( We totally rewrite the layer sorting algorithm, make it simple and can work together with other plugins )
+ Temporarily disable Sprite-Border. ( Diced, Tiled and Sliced sprite will be added in next version )
+ Temporarily disable clipping. ( Clipping it is the most important task for GUI rendering, we are working hard for our GUI layout editor and still testing different clipping method )
+ Temporarily disable dynamic outline, shadow in exSpriteFont. ( Like clipping, it is under discussion with GUI features ) 

## Improvements

+ Allow multi-target editing in the inspector for assets and components. ( We rewrite our editor code by using Unity3D's SerializedProperty class )
+ Add and optimize Max-Rect algorithm for atlasing textures.
+ Easy add and remove texture element in atlas.
+ Scenes or Prefabs will not broken when atlas changes.
+ Polish the Atlas Editor and Sprite Animation Editor selection ( Rect, ctrl, shift and so on).
+ Better operating events in Sprite Animation Editor.
+ Automatically layout elements in Atlas Editor.

## Fixes

+ Fixed crash and out of memory when working with hundreds of Atlas assets.
