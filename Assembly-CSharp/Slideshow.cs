using System;
using System.IO;
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
				if (this.onBeforePlay != null)
				{
					this.onBeforePlay();
				}
				SlideshowUpdateType slideshowUpdateType = this.updateType;
				if (slideshowUpdateType == SlideshowUpdateType.preloadedSprites)
				{
					VideoScreen.Instance.PlaySlideShow(this.sprites);
					return;
				}
				if (slideshowUpdateType != SlideshowUpdateType.loadOnDemand)
				{
					return;
				}
				VideoScreen.Instance.PlaySlideShow(this.files);
			};
		}
		if (this.nextButton != null)
		{
			this.nextButton.onClick += delegate
			{
				this.nextSlide();
			};
		}
		if (this.prevButton != null)
		{
			this.prevButton.onClick += delegate
			{
				this.prevSlide();
			};
		}
		if (this.pauseButton != null)
		{
			this.pauseButton.onClick += delegate
			{
				this.SetPaused(!this.paused);
			};
		}
		if (this.closeButton != null)
		{
			this.closeButton.onClick += delegate
			{
				VideoScreen.Instance.Stop();
				if (this.onEndingPlay != null)
				{
					this.onEndingPlay();
				}
			};
		}
	}

	public void SetPaused(bool state)
	{
		this.paused = state;
		if (this.pauseIcon != null)
		{
			this.pauseIcon.gameObject.SetActive(!this.paused);
		}
		if (this.unpauseIcon != null)
		{
			this.unpauseIcon.gameObject.SetActive(this.paused);
		}
		if (this.prevButton != null)
		{
			this.prevButton.gameObject.SetActive(this.paused);
		}
		if (this.nextButton != null)
		{
			this.nextButton.gameObject.SetActive(this.paused);
		}
	}

	private void resetSlide(bool enable)
	{
		this.timeUntilNextSlide = this.timePerSlide;
		this.currentSlide = 0;
		if (enable)
		{
			this.imageTarget.color = Color.white;
			return;
		}
		if (this.transparentIfEmpty)
		{
			this.imageTarget.color = Color.clear;
		}
	}

	private Sprite loadSlide(string file)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		Texture2D texture2D = new Texture2D(512, 768);
		texture2D.filterMode = FilterMode.Point;
		texture2D.LoadImage(File.ReadAllBytes(file));
		return Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2((float)texture2D.width, (float)texture2D.height)), new Vector2(0.5f, 0.5f), 100f, 0U, SpriteMeshType.FullRect);
	}

	public void SetFiles(string[] files, int loadFrame = -1)
	{
		if (files == null)
		{
			return;
		}
		this.files = files;
		bool flag = files.Length != 0 && files[0] != null;
		this.resetSlide(flag);
		if (flag)
		{
			int num = ((loadFrame != -1) ? loadFrame : (files.Length - 1));
			string text = files[num];
			Sprite sprite = this.loadSlide(text);
			this.setSlide(sprite);
			this.currentSlideImage = sprite;
		}
	}

	public void updateSize(Sprite sprite)
	{
		Vector2 fittedSize = this.GetFittedSize(sprite, 960f, 960f);
		base.GetComponent<RectTransform>().sizeDelta = fittedSize;
	}

	public void SetSprites(Sprite[] sprites)
	{
		if (sprites == null)
		{
			return;
		}
		this.sprites = sprites;
		this.resetSlide(sprites.Length != 0 && sprites[0] != null);
		if (sprites.Length != 0 && sprites[0] != null)
		{
			this.setSlide(sprites[0]);
		}
	}

	public Vector2 GetFittedSize(Sprite sprite, float maxWidth, float maxHeight)
	{
		if (sprite == null || sprite.texture == null)
		{
			return Vector2.zero;
		}
		int width = sprite.texture.width;
		int height = sprite.texture.height;
		float num = maxWidth / (float)width;
		float num2 = maxHeight / (float)height;
		if (num < num2)
		{
			return new Vector2((float)width * num, (float)height * num);
		}
		return new Vector2((float)width * num2, (float)height * num2);
	}

	public void setSlide(Sprite slide)
	{
		if (slide == null)
		{
			return;
		}
		this.imageTarget.texture = slide.texture;
		this.updateSize(slide);
	}

	public void nextSlide()
	{
		this.setSlideIndex(this.currentSlide + 1);
	}

	public void prevSlide()
	{
		this.setSlideIndex(this.currentSlide - 1);
	}

	private void setSlideIndex(int slideIndex)
	{
		this.timeUntilNextSlide = this.timePerSlide;
		SlideshowUpdateType slideshowUpdateType = this.updateType;
		if (slideshowUpdateType != SlideshowUpdateType.preloadedSprites)
		{
			if (slideshowUpdateType != SlideshowUpdateType.loadOnDemand)
			{
				return;
			}
			if (slideIndex < 0)
			{
				slideIndex = this.files.Length + slideIndex;
			}
			this.currentSlide = slideIndex % this.files.Length;
			if (this.currentSlide == this.files.Length - 1)
			{
				this.timeUntilNextSlide *= this.timeFactorForLastSlide;
			}
			if (this.playInThumbnail)
			{
				if (this.currentSlideImage != null)
				{
					global::UnityEngine.Object.Destroy(this.currentSlideImage.texture);
					global::UnityEngine.Object.Destroy(this.currentSlideImage);
					GC.Collect();
				}
				this.currentSlideImage = this.loadSlide(this.files[this.currentSlide]);
				this.setSlide(this.currentSlideImage);
			}
		}
		else
		{
			if (slideIndex < 0)
			{
				slideIndex = this.sprites.Length + slideIndex;
			}
			this.currentSlide = slideIndex % this.sprites.Length;
			if (this.currentSlide == this.sprites.Length - 1)
			{
				this.timeUntilNextSlide *= this.timeFactorForLastSlide;
			}
			if (this.playInThumbnail)
			{
				this.setSlide(this.sprites[this.currentSlide]);
				return;
			}
		}
	}

	private void Update()
	{
		if (this.updateType == SlideshowUpdateType.preloadedSprites && (this.sprites == null || this.sprites.Length == 0))
		{
			return;
		}
		if (this.updateType == SlideshowUpdateType.loadOnDemand && (this.files == null || this.files.Length == 0))
		{
			return;
		}
		if (this.paused)
		{
			return;
		}
		this.timeUntilNextSlide -= Time.unscaledDeltaTime;
		if (this.timeUntilNextSlide <= 0f)
		{
			this.nextSlide();
		}
	}

	public RawImage imageTarget;

	private string[] files;

	private Sprite currentSlideImage;

	private Sprite[] sprites;

	public float timePerSlide = 1f;

	public float timeFactorForLastSlide = 3f;

	private int currentSlide;

	private float timeUntilNextSlide;

	private bool paused;

	public bool playInThumbnail;

	public SlideshowUpdateType updateType;

	[SerializeField]
	private bool isExpandable;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private bool transparentIfEmpty = true;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton prevButton;

	[SerializeField]
	private KButton nextButton;

	[SerializeField]
	private KButton pauseButton;

	[SerializeField]
	private Image pauseIcon;

	[SerializeField]
	private Image unpauseIcon;

	public Slideshow.onBeforeAndEndPlayDelegate onBeforePlay;

	public Slideshow.onBeforeAndEndPlayDelegate onEndingPlay;

	public delegate void onBeforeAndEndPlayDelegate();
}
