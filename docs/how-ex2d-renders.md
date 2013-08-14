---
layout: page
title: How ex2D Renders (and why it's so fast!)
permalink: /docs/how-ex2d-renders/
---

# How ex2D Renders (And why it's so fast!)

In most Unity sprite engine (include ex2D 1.x), sprites are rendered one by one separately and then let Unity do Dynamic Batching for the sprites. The new ex2D handles things differently. We use our own Dynamic Batching solution for all sprites to make it even faster.
 
A ex2D 2.0 scene has layers as the sprite container. All sprites in the scene will be put into layers. Layers define the render order of the sprites they contain. If a sprite A is in a layer that on top of sprite B in another layer, sprite A will be rendered on top of sprite B. Set up the layers in correct order will ensure your sprites renders in correct order.

ex2D Layer also has smart interface on making sure all sprites in a layer use the same atlas and material. So that your batching will work without any hidden issue.

There are two types of layer for different batching:

* __Static Layer__: it's the most efficient way of rendering a group of sprites. Every sprites with the same material in the layer will be combined into a single mesh. It's essentially the same as Unity's static batching so it's the fastest. However ex2D allows you to dynamically create and modify Static Layer. This makes it possible to create static batched sprite groups during runtime, thus more convenient for developers. Beware that creating and modifying Static Layer can cost a lot of CPU time. So it's more suitable for placing background sprites that don't get rearranged often.

* __Dynamic Layer__: Sprites in ex2D's Dynamic Layer will be dynamic batched in ex2D's own way. Dynamic Layer allows users to frequently modify sprites in the layer. Compare to Unity's Dynamic Batching, ex2D users can setup different batching parameter such as the mesh size. Depending on the project, you can find a good balance in spending your CPU and GPU time doing batching and rendering.

Simply put, the less sprite changes, the more we can batches sprite together to reduce drawcall. And the biggest advantage we have by using Layer as render groups, is to apply different render strategy for different part of the scene. Static object or background can be put into Static Layer for static batching. For the moving part there are different parameter for dynamic batching so you can balance the load of CPU and GPU to reduce bottleneck.

From the extensive tests we made on both PC/Mac and Mobile, ex2D's own Dynamic Batching performs much better than other 2D sprite solution that have done a good job on reducing draw calls and utilizing Unity's native Dynamic Batching. The new ex2D is now customized completely for realizing the full potential of 2D sprite rendering. Also we are looking forward to the possibility of further optimization on the performance before ex2D 2.0's official release.


