using System;
using UnityEngine;
using UnityEngine.UI;

public class Slideshow : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.timeUntilNextSlide = this.timePerSlide;
		if (this.transparentIfEmpty && this.sprites != null && this.sprites.Length == 0)
		{
			this.imageTarget.color = Color.clear;
		}
		if (this.isExpandable)
		{
			this.button = base.GetComponent<KButton>();
			this.button.onClick += delegate
			{
				VideoScreen.Instance.PlaySlideShow(this.sprites);
			};
		}
		if (this.closeButton != null)
		{
			this.closeButton.onClick += delegate
			{
				VideoScreen.Instance.Stop();
			};
		}
	}

	public void SetSprites(Sprite[] sprites)
	{
		this.sprites = sprites;
		this.timeUntilNextSlide = this.timePerSlide;
		this.currentSlide = 0;
		if (sprites.Length > 0 && sprites[0] != null)
		{
			this.imageTarget.color = Color.white;
			this.imageTarget.texture = sprites[0].texture;
		}
		else if (this.transparentIfEmpty)
		{
			this.imageTarget.color = Color.clear;
		}
	}

	private void Update()
	{
		if (this.sprites == null || this.sprites.Length <= 0)
		{
			return;
		}
		this.timeUntilNextSlide -= Time.unscaledDeltaTime;
		if (this.timeUntilNextSlide <= 0f)
		{
			this.timeUntilNextSlide = this.timePerSlide;
			this.currentSlide = (this.currentSlide + 1) % this.sprites.Length;
			if (this.playInThumbnail && this.sprites[this.currentSlide] != null)
			{
				this.imageTarget.texture = this.sprites[this.currentSlide].texture;
			}
		}
	}

	public RawImage imageTarget;

	private Sprite[] sprites;

	public float timePerSlide = 1f;

	private int currentSlide;

	private float timeUntilNextSlide;

	public bool playInThumbnail;

	[SerializeField]
	private bool isExpandable;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private bool transparentIfEmpty = true;

	[SerializeField]
	private KButton closeButton;
}
