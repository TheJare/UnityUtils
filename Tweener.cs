/* Simple tweening engine for Unity3D + Futile
 * 
 * Easy animation and interpolation of parameters for arbitrary Futile Nodes.
 * 
 * (C) Copyright 2012 by Javier Arevalo
 * Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
 * 
 * Futile Unity 2D Framework by Matt Rix: https://github.com/MattRix/Futile
 * 
 * Quick examples:
 * 
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
         
 *
 * Tracking a group of tweens
 * 
	if (t.Update(Time.deltaTime)) {
		<<regular tweens finished>>;
	}
 *
 * Finding and removing
 * 
	t.Remove(t.Find(myLabel));
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class tweens parameters for a single Futile Node
public class Tweener {
	public FNode node;
	public string id;
	
	// Interpolator function type and a few useful interpolators
	public delegate float Interpolator(float t);
	
	// Ints go from 0 to 1
	public static float IntLinear(float t) 	{ return t; }
	public static float IntSmooth(float t) 	{ return 3.0f*t*t -2.0f*t*t*t; }
	public static float IntSqr(float t) 	{ return t*t; }
	public static float IntSqrt(float t) 	{ return Mathf.Sqrt(t); }
	public static float IntBump(float t) 	{ return -5*t*(1-t)*(t-1)+t; }
	
	// Humps go from 0 to 0, reaching 1 somewhere in between
	public static float Hump(float t) 		{ return t*(1.0f-t)*4.0f; }
	public static float HumpSmooth(float t) { return t*t*(1-t)*(1-t)*16; }
	
	enum Style { INACTIVE, LINEAR, SAW, PINGPONG };
	struct Val {
		public float t, duration, src, delta;
		public Style style;
		public Interpolator fn;
	};
	
	Val[] values = new Val[6];
	
	public Tweener(FNode node) {
		this.node = node;
		int n = values.Length;
		for (int i = 0; i < n; ++i) {
			values[i].style = Style.INACTIVE;
			values[i].fn = IntSqrt;
		}
	}
	
	Tweener Set(int i, float src, float dest, float t, float duration, Style style) {
		values[i].src = src;
		values[i].delta = dest - src;
		values[i].t = t;
		values[i].duration = duration;
		values[i].style = style;
		return this;
	}
	
	public Tweener SetId(string id) { this.id = id; return this; }
	
	public Tweener SetHandler(System.Action<FButton> a) {
		FButton b = node as FButton;
		if (b != null)
			b.SignalRelease += a;
		return this;
	}
	
	public Tweener X(float dest, float duration) 			{ return Set(0, node.x, dest, 0, duration, Style.LINEAR); }
	public Tweener sawX(float dest, float duration) 		{ return Set(0, node.x, dest, 0, duration, Style.PINGPONG); }
	public Tweener pongX(float dest, float duration) 		{ return Set(0, node.x, dest, 0, duration, Style.SAW); }
	public Tweener Y(float dest, float duration) 			{ return Set(1, node.y, dest, 0, duration, Style.LINEAR); }
	public Tweener sawY(float dest, float duration) 		{ return Set(1, node.y, dest, 0, duration, Style.SAW); }
	public Tweener pongY(float dest, float duration) 		{ return Set(1, node.y, dest, 0, duration, Style.PINGPONG); }
	public Tweener Sx(float dest, float duration) 			{ return Set(2, node.scaleX, dest, 0, duration, Style.LINEAR); }
	public Tweener sawSx(float dest, float duration) 		{ return Set(2, node.scaleX, dest, 0, duration, Style.SAW); }
	public Tweener pongSx(float dest, float duration) 		{ return Set(2, node.scaleX, dest, 0, duration, Style.PINGPONG); }
	public Tweener Sy(float dest, float duration) 			{ return Set(3, node.scaleY, dest, 0, duration, Style.LINEAR); }
	public Tweener sawSy(float dest, float duration) 		{ return Set(3, node.scaleY, dest, 0, duration, Style.SAW); }
	public Tweener pongSy(float dest, float duration) 		{ return Set(3, node.scaleY, dest, 0, duration, Style.PINGPONG); }
	public Tweener Rot(float dest, float duration) 			{ return Set(4, node.rotation, dest, 0, duration, Style.LINEAR); }
	public Tweener sawRot(float dest, float duration) 		{ return Set(4, node.rotation, dest, 0, duration, Style.SAW); }
	public Tweener pongRot(float dest, float duration) 		{ return Set(4, node.rotation, dest, 0, duration, Style.PINGPONG); }
	public Tweener Alpha(float dest, float duration) 		{ return Set(5, node.alpha, dest, 0, duration, Style.LINEAR); }
	public Tweener sawAlpha(float dest, float duration)		{ return Set(5, node.alpha, dest, 0, duration, Style.SAW); }
	public Tweener pongAlpha(float dest, float duration)	{ return Set(5, node.alpha, dest, 0, duration, Style.PINGPONG); }
	
	public Tweener fnX(Interpolator fn) 	{ values[0].fn = fn; return this; }
	public Tweener fnY(Interpolator fn) 	{ values[1].fn = fn; return this; }
	public Tweener fnSx(Interpolator fn) 	{ values[2].fn = fn; return this; }
	public Tweener fnSy(Interpolator fn) 	{ values[3].fn = fn; return this; }
	public Tweener fnRot(Interpolator fn) 	{ values[4].fn = fn; return this; }
	public Tweener fnAlpha(Interpolator fn) { values[5].fn = fn; return this; }
	
	public Tweener offX(float t)		{ values[0].t = t/values[0].duration; return this; }
	public Tweener offY(float t)		{ values[1].t = t/values[1].duration; return this; }
	public Tweener offSx(float t)		{ values[2].t = t/values[2].duration; return this; }
	public Tweener offSy(float t)		{ values[3].t = t/values[3].duration; return this; }
	public Tweener offRot(float t)		{ values[4].t = t/values[4].duration; return this; }
	public Tweener offAlpha(float t)	{ values[5].t = t/values[5].duration; return this; }
	
	public Tweener srcX(float v)		{ values[0].delta += values[0].src - v; values[0].src = v; return this; }
	public Tweener srcY(float v)		{ values[1].delta += values[1].src - v; values[1].src = v; return this; }
	public Tweener srcSx(float v)		{ values[2].delta += values[2].src - v; values[2].src = v; return this; }
	public Tweener srcSy(float v)		{ values[3].delta += values[3].src - v; values[3].src = v; return this; }
	public Tweener srcRot(float v)		{ values[4].delta += values[4].src - v; values[4].src = v; return this; }
	public Tweener srcAlpha(float v)	{ values[5].delta += values[5].src - v; values[5].src = v; return this; }

	// Convenience functions combine several values
	// Mix & match as needed
	public Tweener Pos(float x, float y, float duration)	{ X(x, duration); Y(y, duration); return this; }
	public Tweener S(float x, float y, float duration)		{ Sx(x, duration); Sy(y, duration); return this; }
	public Tweener S(float s, float duration)				{ Sx(s, duration); Sy(s, duration); return this; }
	public Tweener srcS(float v)				{ srcSx(v); srcSy(v); return this; }
	public Tweener srcPos(float x, float y)		{ srcX(x); srcY(y); return this; }
	
	public Tweener Fn(Interpolator fn) {
		int n = values.Length;
		for (int i = 0; i < n; ++i)
			values[i].fn = fn;
		return this;
	}
	
	public Tweener off(float v) {
		int n = values.Length;
		for (int i = 0; i < n; ++i)
			values[i].t = v/values[i].duration;
		return this;
	}
	
	private float Calc(int i) {
		float t = System.Math.Max(values[i].t, 0.0f);
		if (values[i].style == Style.PINGPONG) {
			if (t > 0.5f)
				t = 1.0f - t;
			t *= 2.0f;
		}
		t = values[i].fn(t);
		return values[i].src + t*values[i].delta;
	}
	
	public bool Update(float dt) {
		bool finished = true;
		int n = values.Length;
		for (int i = 0; i < n; ++i) {
			if (values[i].style != Style.INACTIVE) {
				float t = values[i].t + dt/values[i].duration;
				// Non-LINEAR styles do NOT keep the tweener active
				// They behave as 'finished'
				if (values[i].style != Style.LINEAR) {
					if (t >= 1.0f)
						t -= 1.0f;
				} else {
					t = System.Math.Min(t, 1.0f);
					if (t < 1.0f)
						finished = false;
				}
				values[i].t = t;
			}
		}
		if (values[0].style != Style.INACTIVE) node.x = Calc(0);
		if (values[1].style != Style.INACTIVE) node.y = Calc(1);
		if (values[2].style != Style.INACTIVE) node.scaleX = Calc(2);
		if (values[3].style != Style.INACTIVE) node.scaleY = Calc(3);
		if (values[4].style != Style.INACTIVE) node.rotation = Calc(4);
		if (values[5].style != Style.INACTIVE) node.alpha = Calc(5);
		
		return finished;
	}
}

// This class tracks a bunch of Tweeners
// Makes it easy to update and detect when they are
// all finished.
public class TweenerManager {
	List<Tweener> mTweeners = new List<Tweener>();
	public bool finished = true;
	
	public bool Update(float dt) {
		finished = true;
		foreach (Tweener tw in mTweeners) {
			if (!tw.Update(dt))
				finished = false;
		}
		return finished;
	}
	
	public Tweener Add(Tweener tw) {
		if (tw != null && !mTweeners.Exists(t => t == tw)) {
			mTweeners.Add(tw);
			if (!tw.Update(0))
				finished = false;
		}
		return tw;
	}
	
	public void Remove(Tweener tw) {
		if (tw != null)
			mTweeners.Remove(tw);
	}

	public Tweener Find(FNode node) {
		foreach (Tweener tw in mTweeners) {
			if (tw.node == node)
				return tw;
		}
		return null;
	}

	public Tweener Find(string id) {
		foreach (Tweener tw in mTweeners) {
			if (tw.id == id)
				return tw;
		}
		return null;
	}

}
