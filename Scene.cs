using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scene: FContainer
{
	protected TweenerManager mTw = new TweenerManager();
	protected float mTime = 0;
	protected bool mActive = false;
	protected bool mActiveTransition = false;

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
	// Called when the screen is created and added to the stack
	// The place to set up screen state but NOT visual controls
	public virtual void Init () {
	}
	// Called when the screen is removed from the stack
	public virtual void End () {
		manager = null;
	}
	
	// Called when the screen becomes the topmost, visible screen.
	public virtual void Activate () {
		mActive = true;
		mActiveTransition = true; // We add a transition for testing
		mTw.Add(new Tweener(this).X(0, 0.5f).srcX(-Futile.screen.width));
		OnActivate();
	}
	// Called when the screen is no longer the topmost, visible screen.
	public virtual void Deactivate () {
		mActive = false;
		mActiveTransition = true; // We add a transition for testing
		mTw.Add(new Tweener(this).X(-Futile.screen.width, 0.5f));
	}

	// The place to create controls and transitions for them
	public virtual void OnActivate () {
		Futile.stage.AddChild(this);
	}
	// The place to destroy controls because the transition out is finished
	public virtual void OnDeactivate () {
		ClearChildren(this);
		RemoveFromContainer();
	}
	public virtual bool FinishedActivation() {
		return !mActiveTransition || mTw.finished;
	}
	public bool isActive {
		get { return mActive; }
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
	public Tweener NewSlice(string elementName, float x, float y, float width, float height, float inset) {
		return NewTweener(new FSliceSprite(elementName, width, height, inset, inset, inset, inset), x, y);
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
			else {
				oldScene.OnDeactivate();
				oldScene.End();
			}
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
		// Run the screens in the stack
		for (int i = 0; i < mStack.Count; ++i) {
			Scene scene = mStack[i];
			scene.Update(dt);
			if (!scene.isActive && scene.FinishedActivation())
				scene.OnDeactivate();
		}
		// Keep running the screens that have been removed and are transitioning out
		for (int i = mPopped.Count-1; i >= 0; --i) {
			Scene scene = mPopped[i];
			scene.Update(dt);
			if (scene.FinishedActivation()) {
				scene.OnDeactivate();
				mPopped.RemoveAt(i);
				scene.End();
			}
		}
	}
}
