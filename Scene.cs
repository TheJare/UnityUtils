using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scene: FContainer
{
	protected TweenerManager mTw = new TweenerManager();
	protected float mTime = 0;
	protected bool mActive = true;

	public SceneManager manager = null;
	
	static public void ClearChildren(FNode n) {
		FContainer c = n as FContainer;
		if (c != null) {
			while (c.GetChildCount() > 0) {
				FNode s = c.GetChildAt(0);
				ClearChildren(s);
				s.RemoveFromContainer();
			}
		}
	}
	
	public virtual void Init () {
		Futile.stage.AddChild(this);
	}
	public virtual void End () {
		ClearChildren(this);
		RemoveFromContainer();
		manager = null;
	}
	public virtual void Activate () {
		mActive = true;
	}
	public virtual void Deactivate () {
		mActive = false;
	}
	public virtual bool FinishedActivation() {
		return mTw.finished;
	}
	public bool IsActive() {
		return mActive;
	}
	public virtual void Update (float dt) {
		mTime += dt;
		mTw.Update(dt);
	}
	
	// Convenience functions to create controls

	public Tweener NewTweener(FNode n, float x, float y) {
		n.x = x;
		n.y = y;
		AddChild(n);
		Tweener t = new Tweener(n);
		mTw.Add(t);
		return t;
	}

	public Tweener NewLabel(float x, float y, string fontName, string text) {
		return NewTweener(new FLabel(fontName, text), x, y);
	}
	public Tweener NewSlice(float x, float y, float width, float height, float inset, string elementName) {
		return NewTweener(new FSliceSprite(width, height, inset, inset, inset, inset, elementName), x, y);
	}
	public Tweener NewButton(float x, float y, string fontName, string text, string bgName) {
		var n = new FButton(bgName);
		//n.sprite.isVisible = false;
		n.AddLabel(fontName, text, Color.white);
		n.sprite.scaleX = n.label.textRect.width/n.sprite.textureRect.width;
		n.sprite.scaleY = n.label.textRect.height/n.sprite.textureRect.height;
		return NewTweener(n, x, y);
	}
	public Tweener NewButton(float x, float y, string fontName, string text) {
		return NewButton(x, y, fontName, text, "Futile_White");
	}
	
	public Tweener Find(string id) { return mTw.Find(id); }
}

public class SceneManager
{
	private List<Scene> mStack = new List<Scene>();
	private List<Scene> mPopped = new List<Scene>();
	
	void AddSceneToTop(Scene scene) {
		mStack.Add(scene);
		scene.manager = this;
		scene.Init();
		scene.Activate();
	}
	
	void RemoveTopScene() {
		if (mStack.Count > 0) {
			Scene oldScene = mStack[mStack.Count-1];
			oldScene.Deactivate();
			mStack.RemoveAt(mStack.Count-1);
			if (!oldScene.FinishedActivation())
				mPopped.Add(oldScene);
			else
				oldScene.End();
		}
	}
	
	public void Push(Scene scene) {
		if (mStack.Count > 0) {
			Scene oldScene = mStack[mStack.Count-1];
			oldScene.Deactivate();
		}
		AddSceneToTop(scene);
	}
	public void Pop() {
		RemoveTopScene();
		if (mStack.Count > 0) {
			Scene newScene = mStack[mStack.Count-1];
			newScene.Activate();
		}
	}
	public void Replace(Scene scene) {
		RemoveTopScene();
		AddSceneToTop(scene);
	}
	public void ReplacePop(Scene scene, int nToPop) {
		while (mStack.Count > 0 && --nToPop == 0)
			RemoveTopScene();
		AddSceneToTop(scene);
	}
	
	public int Count {
		get { return mStack.Count; }
	}
	
	public void Update(float dt) {
		for (int i = 0; i < mStack.Count; ++i) {
			mStack[i].Update(dt);
		}
		for (int i = mPopped.Count-1; i >= 0; --i) {
			Scene s = mPopped[i];
			s.Update(dt);
			if (s.FinishedActivation()) {
				mPopped.RemoveAt(i);
				s.End();
			}
		}
		
	}
}
