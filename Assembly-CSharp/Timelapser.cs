using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class Timelapser : KMonoBehaviour
{
	public bool CapturingTimelapseScreenshot
	{
		get
		{
			return this.screenshotActive;
		}
	}

	public Texture2D freezeTexture { get; private set; }

	protected override void OnPrefabInit()
	{
		this.RefreshRenderTextureSize(null);
		Game.Instance.Subscribe(75424175, new Action<object>(this.RefreshRenderTextureSize));
		this.freezeCamera = CameraController.Instance.timelapseFreezeCamera;
		if (this.CycleTimeToScreenshot() > 0f)
		{
			this.OnNewDay(null);
		}
		GameClock.Instance.Subscribe(631075836, new Action<object>(this.OnNewDay));
		this.OnResize();
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.OnResize));
		base.StartCoroutine(this.Render());
	}

	private void OnResize()
	{
		if (this.freezeTexture != null)
		{
			global::UnityEngine.Object.Destroy(this.freezeTexture);
		}
		this.freezeTexture = new Texture2D(Camera.main.pixelWidth, Camera.main.pixelHeight, TextureFormat.ARGB32, false);
	}

	private void RefreshRenderTextureSize(object data = null)
	{
		if (this.timelapseUserEnabled)
		{
			this.bufferRenderTexture = new RenderTexture(SaveGame.Instance.TimelapseResolution.x, SaveGame.Instance.TimelapseResolution.y, 32, RenderTextureFormat.ARGB32);
		}
	}

	private bool timelapseUserEnabled
	{
		get
		{
			return SaveGame.Instance.TimelapseResolution.x > 0;
		}
	}

	private void OnNewDay(object data = null)
	{
		int cycle = GameClock.Instance.GetCycle();
		if (cycle > this.timelapseScreenshotCycles[this.timelapseScreenshotCycles.Length - 1])
		{
			if (cycle % 10 == 0)
			{
				this.screenshotToday = true;
			}
		}
		else
		{
			for (int i = 0; i < this.timelapseScreenshotCycles.Length; i++)
			{
				if (cycle == this.timelapseScreenshotCycles[i])
				{
					this.screenshotToday = true;
				}
			}
		}
	}

	private void Update()
	{
		if (this.screenshotToday && this.CycleTimeToScreenshot() <= 0f)
		{
			if (!this.timelapseUserEnabled)
			{
				this.screenshotToday = false;
			}
			else if (!PlayerController.Instance.IsDragging())
			{
				CameraController.Instance.ForcePanningState(false);
				this.screenshotToday = false;
				this.SaveScreenshot();
			}
		}
	}

	private float CycleTimeToScreenshot()
	{
		return 300f - GameClock.Instance.GetTime() % 600f;
	}

	private IEnumerator Render()
	{
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		for (;;)
		{
			yield return wait;
			if (this.screenshotPending)
			{
				if (!this.freezeCamera.enabled)
				{
					this.freezeTexture.ReadPixels(new Rect(0f, 0f, (float)Camera.main.pixelWidth, (float)Camera.main.pixelHeight), 0, 0);
					this.freezeTexture.Apply();
					this.freezeCamera.gameObject.GetComponent<FillRenderTargetEffect>().SetFillTexture(this.freezeTexture);
					this.freezeCamera.enabled = true;
					this.screenshotActive = true;
					this.SetPostionAndOrtho();
					DebugHandler.SetHideUI(true);
				}
				else
				{
					this.RenderAndPrint();
					this.freezeCamera.enabled = false;
					DebugHandler.SetHideUI(false);
					this.screenshotPending = false;
				}
			}
		}
		yield break;
	}

	public void SaveScreenshot()
	{
		global::Debug.Log("Screenshot!");
		this.screenshotPending = true;
	}

	private void SetPostionAndOrtho()
	{
		float num = 0f;
		GameObject telepad = GameUtil.GetTelepad();
		if (telepad == null)
		{
			return;
		}
		int num2 = Grid.PosToCell(telepad);
		for (int i = 0; i < Grid.CellCount; i++)
		{
			if (Grid.Revealed[i])
			{
				num = Mathf.Max(new float[]
				{
					num,
					(float)Grid.GetOffset(i, num2).x * (1f / ((float)Grid.WidthInCells / (float)Grid.HeightInCells)),
					(float)Grid.GetOffset(i, num2).y * (1f / ((float)Grid.HeightInCells / (float)Grid.WidthInCells))
				});
			}
		}
		num = Mathf.Max(num, 18f);
		Camera overlayCamera = CameraController.Instance.overlayCamera;
		this.camSize = overlayCamera.orthographicSize;
		CameraController.Instance.SetOrthographicsSize(num);
		this.camPosition = CameraController.Instance.transform.position;
		CameraController.Instance.SetPosition(new Vector3(telepad.transform.position.x, telepad.transform.position.y, CameraController.Instance.transform.position.z));
		CameraController.Instance.SetTargetPos(new Vector3(telepad.transform.position.x, telepad.transform.position.y, CameraController.Instance.transform.position.z), this.camSize, false);
	}

	private void RenderAndPrint()
	{
		GameObject telepad = GameUtil.GetTelepad();
		if (telepad == null)
		{
			return;
		}
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = this.bufferRenderTexture;
		CameraController.Instance.SetPosition(new Vector3(telepad.transform.position.x, telepad.transform.position.y, CameraController.Instance.transform.position.z));
		CameraController.Instance.RenderForTimelapser(ref this.bufferRenderTexture);
		this.WriteToPng(this.bufferRenderTexture);
		CameraController.Instance.SetOrthographicsSize(this.camSize);
		CameraController.Instance.SetPosition(this.camPosition);
		CameraController.Instance.SetTargetPos(this.camPosition, this.camSize, false);
		RenderTexture.active = active;
		this.screenshotActive = false;
		this.debugScreenShot = false;
	}

	public void WriteToPng(RenderTexture renderTex)
	{
		Texture2D texture2D = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTex.width, (float)renderTex.height), 0, 0);
		texture2D.Apply();
		byte[] array = texture2D.EncodeToPNG();
		global::UnityEngine.Object.Destroy(texture2D);
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string text = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = RetireColonyUtility.StripInvalidCharacters(SaveGame.Instance.BaseName);
		string text3 = Path.Combine(text, text2);
		global::Debug.Log(text3);
		if (!Directory.Exists(text3))
		{
			Directory.CreateDirectory(text3);
		}
		string text4 = Path.Combine(text3, text2);
		string text5 = "0000.##";
		text4 = text4 + "_cycle_" + GameClock.Instance.GetCycle().ToString(text5);
		if (this.debugScreenShot)
		{
			string text6 = text4;
			text4 = string.Concat(new object[]
			{
				text6,
				"_",
				global::System.DateTime.Now.Day,
				"-",
				global::System.DateTime.Now.Month,
				"_",
				global::System.DateTime.Now.Hour,
				"-",
				global::System.DateTime.Now.Minute,
				"-",
				global::System.DateTime.Now.Second
			});
		}
		File.WriteAllBytes(text4 + ".png", array);
	}

	private bool screenshotActive;

	private bool screenshotPending;

	private bool screenshotToday = true;

	private Camera freezeCamera;

	private RenderTexture bufferRenderTexture;

	private Vector3 camPosition;

	private float camSize;

	private bool debugScreenShot;

	private const int DEFAULT_SCREENSHOT_INTERVAL = 10;

	private int[] timelapseScreenshotCycles = new int[]
	{
		1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
		11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
		21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
		31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
		41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
		55, 60, 65, 70, 75, 80, 85, 90, 95, 100,
		110, 120, 130, 140, 150, 160, 170, 180, 190, 200,
		210, 220, 230, 240, 250, 260, 270, 280, 290, 200,
		310, 320, 330, 340, 350, 360, 370, 380, 390, 400,
		410, 420, 430, 440, 450, 460, 470, 480, 490, 500
	};
}
