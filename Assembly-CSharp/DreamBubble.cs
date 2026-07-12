using System;
using UnityEngine;

public class DreamBubble : KMonoBehaviour
{
	public bool IsVisible { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.dreamBackgroundComponent.SetSymbolVisiblity(this.snapToPivotSymbol, false);
		this._color = this.dreamContentComponent.color;
		this.SetVisibility(false);
	}

	public void Tick(float dt)
	{
		if (this._currentDream != null && this._currentDream.Icons.Length != 0)
		{
			float num = this._timePassedSinceDreamStarted / this._currentDream.secondPerImage;
			int num2 = Mathf.FloorToInt(num);
			float num3 = num - (float)num2;
			int num4 = (int)Mathf.Repeat((float)Mathf.FloorToInt(num), (float)this._currentDream.Icons.Length);
			if (this.dreamContentComponent.sprite != this._currentDream.Icons[num4])
			{
				this.dreamContentComponent.sprite = this._currentDream.Icons[num4];
			}
			this.dreamContentComponent.rectTransform.localScale = Vector3.one * num3;
			this._color.a = (Mathf.Sin(num3 * 6.2831855f - 1.5707964f) + 1f) * 0.5f;
			this.dreamContentComponent.color = this._color;
			this._timePassedSinceDreamStarted += dt;
		}
	}

	public void SetDream(Dream dream)
	{
		this._currentDream = dream;
		this.dreamBackgroundComponent.Stop();
		this.dreamBackgroundComponent.AnimFiles = new KAnimFile[] { Assets.GetAnim(dream.BackgroundAnim) };
		this._color.a = 0f;
		this.dreamContentComponent.color = this._color;
		this._timePassedSinceDreamStarted = 0f;
	}

	public void SetVisibility(bool visible)
	{
		this.IsVisible = visible;
		this.dreamBackgroundComponent.SetVisiblity(visible);
		this.dreamContentComponent.gameObject.SetActive(visible);
		if (this._currentDream != null && visible)
		{
			this.dreamBackgroundComponent.Play("dream_loop", KAnim.PlayMode.Loop, 1f, 0f);
		}
		if (!visible)
		{
			this.dreamBackgroundComponent.Stop();
		}
	}

	public void StopDreaming()
	{
		this._currentDream = null;
		this.SetVisibility(false);
	}

	public KBatchedAnimController dreamBackgroundComponent;

	public KImage dreamContentComponent;

	private const string dreamBackgroundAnimationName = "dream_loop";

	private HashedString snapToPivotSymbol = new HashedString("snapto_pivot");

	private Dream _currentDream;

	private float _timePassedSinceDreamStarted;

	private Color _color;

	private const float PI_2 = 6.2831855f;

	private const float HALF_PI = 1.5707964f;
}
