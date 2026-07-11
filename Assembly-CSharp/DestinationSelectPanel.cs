using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.CustomSettings;
using ProcGen;
using ProcGenGame;
using UnityEngine;

public class DestinationSelectPanel : KMonoBehaviour
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<ColonyDestinationAsteroidData> OnAsteroidClicked;

	private float min
	{
		get
		{
			return this.asteroidContainer.rect.x + this.offset;
		}
	}

	private float max
	{
		get
		{
			return this.min + this.asteroidContainer.rect.width;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.dragTarget.onBeginDrag += this.BeginDrag;
		this.dragTarget.onDrag += this.Drag;
		this.dragTarget.onEndDrag += this.EndDrag;
		MultiToggle multiToggle = this.leftArrowButton;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.ClickLeft));
		MultiToggle multiToggle2 = this.rightArrowButton;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(this.ClickRight));
	}

	private void BeginDrag()
	{
		this.dragStartPos = Input.mousePosition;
		this.dragLastPos = this.dragStartPos;
		this.isDragging = true;
		KFMOD.PlayUISound(GlobalAssets.GetSound("DestinationSelect_Scroll_Start", false));
	}

	private void Drag()
	{
		Vector2 vector = Input.mousePosition;
		float num = vector.x - this.dragLastPos.x;
		this.dragLastPos = vector;
		this.offset += num;
		int num2 = this.selectedIndex;
		this.selectedIndex = Mathf.RoundToInt(-this.offset / this.asteroidXSeparation);
		this.selectedIndex = Mathf.Clamp(this.selectedIndex, 0, this.worldNames.Count - 1);
		if (num2 != this.selectedIndex)
		{
			this.OnAsteroidClicked(this.asteroidData[this.worldNames[this.selectedIndex]]);
			KFMOD.PlayUISound(GlobalAssets.GetSound("DestinationSelect_Scroll", false));
		}
	}

	private void EndDrag()
	{
		this.Drag();
		this.isDragging = false;
		KFMOD.PlayUISound(GlobalAssets.GetSound("DestinationSelect_Scroll_Stop", false));
	}

	private void ClickLeft()
	{
		this.selectedIndex = Mathf.Clamp(this.selectedIndex - 1, 0, this.worldNames.Count - 1);
		this.OnAsteroidClicked(this.asteroidData[this.worldNames[this.selectedIndex]]);
	}

	private void ClickRight()
	{
		this.selectedIndex = Mathf.Clamp(this.selectedIndex + 1, 0, this.worldNames.Count - 1);
		this.OnAsteroidClicked(this.asteroidData[this.worldNames[this.selectedIndex]]);
	}

	protected override void OnSpawn()
	{
		WorldGen.LoadSettings();
		this.worldNames = SettingsCache.worlds.GetNames();
		foreach (string text in this.worldNames)
		{
			ColonyDestinationAsteroidData colonyDestinationAsteroidData = new ColonyDestinationAsteroidData(text, 0);
			this.asteroidData[text] = colonyDestinationAsteroidData;
		}
		this.worldNames.Sort(delegate(string a, string b)
		{
			ColonyDestinationAsteroidData colonyDestinationAsteroidData2 = this.asteroidData[a];
			ColonyDestinationAsteroidData colonyDestinationAsteroidData3 = this.asteroidData[b];
			return colonyDestinationAsteroidData2.difficulty.CompareTo(colonyDestinationAsteroidData3.difficulty);
		});
	}

	private void Update()
	{
		if (!this.isDragging)
		{
			float num = this.offset + (float)this.selectedIndex * this.asteroidXSeparation;
			float num2 = 0f;
			if (num != 0f)
			{
				num2 = -num;
			}
			num2 = Mathf.Clamp(num2, -this.asteroidXSeparation * 2f, this.asteroidXSeparation * 2f);
			if (num2 != 0f)
			{
				float num3 = this.centeringSpeed * Time.unscaledDeltaTime;
				float num4 = num2 * this.centeringSpeed * Time.unscaledDeltaTime;
				if (num4 > 0f && num4 < num3)
				{
					num4 = Mathf.Min(num3, num2);
				}
				else if (num4 < 0f && num4 > -num3)
				{
					num4 = Mathf.Max(-num3, num2);
				}
				this.offset += num4;
			}
		}
		float x = this.asteroidContainer.rect.min.x;
		float x2 = this.asteroidContainer.rect.max.x;
		this.offset = Mathf.Clamp(this.offset, (float)(-(float)(this.worldNames.Count - 1)) * this.asteroidXSeparation + x, x2);
		this.RePlaceAsteroids();
	}

	[ContextMenu("RePlaceAsteroids")]
	public void RePlaceAsteroids()
	{
		this.BeginAsteroidDrawing();
		for (int i = 0; i < this.worldNames.Count; i++)
		{
			if (i != this.selectedIndex)
			{
				float num = this.offset + (float)i * this.asteroidXSeparation;
				if (num + this.offset + this.asteroidXSeparation >= this.min && num + this.offset - this.asteroidXSeparation <= this.max)
				{
					DestinationAsteroid2 asteroid = this.GetAsteroid(this.worldNames[i], 1f);
					asteroid.transform.SetLocalPosition(new Vector3(num, 0f, 0f));
					if (this.numAsteroids > 100)
					{
						break;
					}
				}
			}
		}
		float num2 = this.offset + (float)this.selectedIndex * this.asteroidXSeparation;
		DestinationAsteroid2 asteroid2 = this.GetAsteroid(this.worldNames[this.selectedIndex], this.asteroidFocusScale);
		asteroid2.transform.SetLocalPosition(new Vector3(num2, 0f, 0f));
		this.EndAsteroidDrawing();
	}

	private void BeginAsteroidDrawing()
	{
		this.numAsteroids = 0;
	}

	private DestinationAsteroid2 GetAsteroid(string name, float scale)
	{
		DestinationAsteroid2 destinationAsteroid;
		if (this.numAsteroids < this.asteroids.Count)
		{
			destinationAsteroid = this.asteroids[this.numAsteroids];
		}
		else
		{
			destinationAsteroid = global::Util.KInstantiateUI<DestinationAsteroid2>(this.asteroidPrefab, this.asteroidContainer.gameObject, false);
			destinationAsteroid.OnClicked += this.OnAsteroidClicked;
			this.asteroids.Add(destinationAsteroid);
		}
		this.asteroidData[name].TargetScale = scale;
		this.asteroidData[name].Scale += (this.asteroidData[name].TargetScale - this.asteroidData[name].Scale) * this.focusScaleSpeed * Time.unscaledDeltaTime;
		destinationAsteroid.transform.localScale = Vector3.one * this.asteroidData[name].Scale;
		destinationAsteroid.SetAsteroid(this.asteroidData[name]);
		this.numAsteroids++;
		return destinationAsteroid;
	}

	private void EndAsteroidDrawing()
	{
		for (int i = 0; i < this.asteroids.Count; i++)
		{
			this.asteroids[i].gameObject.SetActive(i < this.numAsteroids);
		}
	}

	public ColonyDestinationAsteroidData SelectAsteroid(string name, int seed)
	{
		this.selectedIndex = this.worldNames.IndexOf(name);
		this.asteroidData[name].ReInitialize(seed);
		return this.asteroidData[name];
	}

	public void ScrollLeft()
	{
		int num = Mathf.Max(this.selectedIndex - 1, 0);
		this.OnAsteroidClicked(this.asteroidData[this.worldNames[num]]);
	}

	public void ScrollRight()
	{
		int num = Mathf.Min(this.selectedIndex + 1, this.worldNames.Count - 1);
		this.OnAsteroidClicked(this.asteroidData[this.worldNames[num]]);
	}

	private void DebugCurrentSetting()
	{
		ColonyDestinationAsteroidData colonyDestinationAsteroidData = this.asteroidData[this.worldNames[this.selectedIndex]];
		string text = "{world}: {seed} [{traits}] {{settings}}";
		string properName = colonyDestinationAsteroidData.properName;
		string text2 = colonyDestinationAsteroidData.seed.ToString();
		text = text.Replace("{world}", properName);
		text = text.Replace("{seed}", text2);
		List<AsteroidDescriptor> traitDescriptors = colonyDestinationAsteroidData.GetTraitDescriptors();
		string[] array = new string[traitDescriptors.Count];
		for (int i = 0; i < traitDescriptors.Count; i++)
		{
			array[i] = traitDescriptors[i].text;
		}
		string text3 = string.Join(", ", array);
		text = text.Replace("{traits}", text3);
		CustomGameSettings.CustomGameMode customGameMode = CustomGameSettings.Instance.customGameMode;
		if (customGameMode != CustomGameSettings.CustomGameMode.Survival)
		{
			if (customGameMode != CustomGameSettings.CustomGameMode.Nosweat)
			{
				if (customGameMode == CustomGameSettings.CustomGameMode.Custom)
				{
					List<string> list = new List<string>();
					foreach (KeyValuePair<string, SettingConfig> keyValuePair in CustomGameSettings.Instance.QualitySettings)
					{
						if (keyValuePair.Value.coordinate_dimension >= 0 && keyValuePair.Value.coordinate_dimension_width >= 0)
						{
							SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(keyValuePair.Key);
							if (currentQualitySetting.id != keyValuePair.Value.default_level_id)
							{
								list.Add(string.Format("{0}={1}", keyValuePair.Value.label, currentQualitySetting.label));
							}
						}
					}
					text = text.Replace("{settings}", string.Join(", ", list.ToArray()));
				}
			}
			else
			{
				text = text.Replace("{settings}", "Nosweat");
			}
		}
		else
		{
			text = text.Replace("{settings}", "Survival");
		}
		global::Debug.Log(text);
	}

	[SerializeField]
	private GameObject asteroidPrefab;

	[SerializeField]
	private KButtonDrag dragTarget;

	[SerializeField]
	private MultiToggle leftArrowButton;

	[SerializeField]
	private MultiToggle rightArrowButton;

	[SerializeField]
	private RectTransform asteroidContainer;

	[SerializeField]
	private float asteroidFocusScale = 2f;

	[SerializeField]
	private float asteroidXSeparation = 240f;

	[SerializeField]
	private float focusScaleSpeed = 0.5f;

	[SerializeField]
	private float centeringSpeed = 0.5f;

	private float offset;

	private int selectedIndex = -1;

	private List<DestinationAsteroid2> asteroids = new List<DestinationAsteroid2>();

	private int numAsteroids;

	private List<string> worldNames;

	private Dictionary<string, ColonyDestinationAsteroidData> asteroidData = new Dictionary<string, ColonyDestinationAsteroidData>();

	private Vector2 dragStartPos;

	private Vector2 dragLastPos;

	private bool isDragging;

	private const string debugFmt = "{world}: {seed} [{traits}] {{settings}}";
}
