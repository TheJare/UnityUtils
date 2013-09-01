# UnityUtils

Utility classes for working with Unity 3D and the Futile library

Copyright 2012 by Javier Arevalo


* Licensed under the MIT license: [http://www.opensource.org/licenses/mit-license.php](http://www.opensource.org/licenses/mit-license.php)

* GitHub:[https://github.com/TheJare/UnityUtils.git](https://github.com/TheJare/UnityUtils.git)

* Futile Unity 2D Framework by Matt Rix: [https://github.com/MattRix/Futile](https://github.com/MattRix/Futile)

Sorry the docs are not the best yet :)

## Tweener.cs

Easy animation and interpolation of parameters for arbitrary Futile Nodes.


Quick examples:

    TweenerManager t;
    t.Add(new Tweener(mySprite)
          .sawSx(0.5f, 1.0f).offSx(0.3f).fnSx(Tweener.IntSlow)
          .pongSy(0.5f, 2.0f));
    t.Add(new Tweener(myLabel)
          .Fn(Tweener.IntParab)
          .pongRot(45.0f, 1.0f).srcRot(-45.0f).offRot(0.25f)
          .Y(100.0f, 5.0f)
          .sawAlpha(0.0f, 1.0f).fnAlpha(Tweener.Hump));
    t.Add(new Tweener(myLabel).Fn(bumpFn).S(1.0f, 1.0f, 0.5f).srcS(0).off(-1.0f));
    t.Add(new Tweener(myButton).Fn(bumpFn).Pos(0.0f, -40.0f, 0.5f).srcY(-80.0f));

Tracking a group of tweens

    if (t.Update(Time.deltaTime)) {
        <<regular tweens finished>>;
    }

Finding and removing

    t.Remove(t.Find(myLabel));

## Scene.cs

More or less classic Scene Manager (a stack of UI screens and overall game flow) with built-in support for Tweener. Still work in progress...
