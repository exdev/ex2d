---
layout: page
title: ex2D的渲染机制 (高效的原因)
permalink: /docs/zh/how-ex2d-renders/
---

# ex2D的渲染机制 (高效的原因)

在以往的2D插件中，渲染方式是每个sprite单独渲染，由Unity负责Dynamic Batching。在新版ex2D中，我们经过严谨的优化实现了独立的dynamic batching，从而获得了超越以往的渲染效率。

ex2D将场景划分为不同的layer，所有sprite都通过所在的layer进行渲染。Layer之间按照渲染次序进行排列，只要设置了layer，就能保证不同layer之间的sprite的正确渲染次序。而些都可以用ex2D Scene Editor很方便的进行编辑。此外layer中的不同sprite允许使用各自的material，用户拥有是否合并贴图的选择权。

Layer分为两种类型，它们满足的需求不同：

* __Static Layer__: ex2D的Static Layer是最紧凑的，适合做静态的元素的批量渲染。在Layer中所有material相同的sprite都会被尽可能放到相同的mesh中，相当于做了static batching。Static Layer允许动态修改和创建，使得它比传统的static batching方便了很多，但频繁操作static layer消耗较高，较为适合放置不经常更新的背景或UI。

* __Dynamic Layer__: ex2D的Dynamic Layer是最灵活的，适合做动态元素的批量渲染。它支持频繁对sprite进行任意修改。ex2D可以给每个Dynamic Layer单独设定不同的mesh大小，根据不同项目的瓶颈，在合并与渲染的开销之间取得平衡。

综上，引入layer带来的性能优势是，可对场景的不同部分分别采取个性化的渲染策略。对于场景中静态的部分，可设置成static layer进行static batching。对于动态的部分，可根据实际项目，设置dynamic batching的参数，平衡CPU和GPU的负载，减少效率瓶颈。

经过手机和PC的复杂测试，ex2D的运行帧率效率完全超越了其它1个drawcall的2D插件，以及利用Unity的dynamic batching实现批量渲染的其余2D插件。ex2D成为了Unity上第一款真正为2D游戏量身优化的渲染套件。