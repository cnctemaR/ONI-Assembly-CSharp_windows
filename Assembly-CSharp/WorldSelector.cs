using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelector : KScreen, ISim4000ms
{
	public static void DestroyInstance()
	{
		WorldSelector.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		WorldSelector.Instance = this;
	}

	protected override void OnSpawn()
	{
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			this.Deactivate();
			return;
		}
		base.OnSpawn();
		this.worldRows = new Dictionary<int, MultiToggle>();
		this.SpawnToggles();
		this.RefreshToggles();
		Game.Instance.Subscribe(1983128072, delegate(object data)
		{
			this.RefreshToggles();
		});
		Game.Instance.Subscribe(-521212405, delegate(object data)
		{
			this.RefreshToggles();
		});
		Game.Instance.Subscribe(880851192, delegate(object data)
		{
			this.SortRows();
		});
		ClusterManager.Instance.Subscribe(-1280433810, delegate(object data)
		{
			this.AddWorld(data);
		});
		ClusterManager.Instance.Subscribe(-1078710002, delegate(object data)
		{
			this.RemoveWorld(data);
		});
		ClusterManager.Instance.Subscribe(1943181844, delegate(object data)
		{
			this.RefreshToggles();
		});
	}

	private void SpawnToggles()
	{
		foreach (KeyValuePair<int, MultiToggle> keyValuePair in this.worldRows)
		{
			Util.KDestroyGameObject(keyValuePair.Value);
		}
		this.worldRows.Clear();
		foreach (int num in ClusterManager.Instance.GetWorldIDsSorted())
		{
			MultiToggle component = Util.KInstantiateUI(this.worldRowPrefab, this.worldRowContainer, false).GetComponent<MultiToggle>();
			this.worldRows.Add(num, component);
			this.previousWorldDiagnosticStatus.Add(num, ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
			int id = num;
			MultiToggle multiToggle = component;
			multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
			{
				this.OnWorldRowClicked(id);
			}));
			component.GetComponentInChildren<AlertVignette>().worldID = num;
		}
	}

	private void AddWorld(object data)
	{
		int num = (int)data;
		MultiToggle component = Util.KInstantiateUI(this.worldRowPrefab, this.worldRowContainer, false).GetComponent<MultiToggle>();
		this.worldRows.Add(num, component);
		this.previousWorldDiagnosticStatus.Add(num, ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
		int id = num;
		MultiToggle multiToggle = component;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.OnWorldRowClicked(id);
		}));
		component.GetComponentInChildren<AlertVignette>().worldID = num;
		this.RefreshToggles();
	}

	private void RemoveWorld(object data)
	{
		int num = (int)data;
		MultiToggle multiToggle;
		if (this.worldRows.TryGetValue(num, out multiToggle))
		{
			multiToggle.DeleteObject();
		}
		this.worldRows.Remove(num);
		this.previousWorldDiagnosticStatus.Remove(num);
		this.RefreshToggles();
	}

	public void OnWorldRowClicked(int id)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(id);
		if (world != null && world.IsDiscovered)
		{
			CameraController.Instance.ActiveWorldStarWipe(id, null);
		}
	}

	private void RefreshToggles()
	{
		foreach (KeyValuePair<int, MultiToggle> keyValuePair in this.worldRows)
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(keyValuePair.Key);
			ClusterGridEntity component = world.GetComponent<ClusterGridEntity>();
			HierarchyReferences component2 = keyValuePair.Value.GetComponent<HierarchyReferences>();
			if (world != null)
			{
				component2.GetReference<Image>("Icon").sprite = component.GetUISprite();
				component2.GetReference<LocText>("Label").SetText(world.GetComponent<ClusterGridEntity>().Name);
			}
			else
			{
				component2.GetReference<Image>("Icon").sprite = Assets.GetSprite("unknown_far");
			}
			if (keyValuePair.Key == CameraController.Instance.cameraActiveCluster)
			{
				keyValuePair.Value.ChangeState(1);
				keyValuePair.Value.gameObject.SetActive(true);
			}
			else if (world != null && world.IsDiscovered)
			{
				keyValuePair.Value.ChangeState(0);
				keyValuePair.Value.gameObject.SetActive(true);
			}
			else
			{
				keyValuePair.Value.ChangeState(0);
				keyValuePair.Value.gameObject.SetActive(false);
			}
			this.RefreshToggleTooltips();
			keyValuePair.Value.GetComponentInChildren<AlertVignette>().worldID = keyValuePair.Key;
		}
		this.RefreshWorldStatus();
		this.SortRows();
	}

	private void RefreshWorldStatus()
	{
		foreach (KeyValuePair<int, MultiToggle> keyValuePair in this.worldRows)
		{
			if (!this.worldStatusIcons.ContainsKey(keyValuePair.Key))
			{
				this.worldStatusIcons.Add(keyValuePair.Key, new List<GameObject>());
			}
			foreach (GameObject gameObject in this.worldStatusIcons[keyValuePair.Key])
			{
				Util.KDestroyGameObject(gameObject);
			}
			LocText reference = keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("StatusLabel");
			reference.SetText(ClusterManager.Instance.GetWorld(keyValuePair.Key).GetStatus());
			reference.color = ColonyDiagnosticScreen.GetDiagnosticIndicationColor(ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(keyValuePair.Key));
		}
	}

	private void RefreshToggleTooltips()
	{
		int num = 0;
		List<int> discoveredAsteroidIDsSorted = ClusterManager.Instance.GetDiscoveredAsteroidIDsSorted();
		foreach (KeyValuePair<int, MultiToggle> keyValuePair in this.worldRows)
		{
			ClusterGridEntity component = ClusterManager.Instance.GetWorld(keyValuePair.Key).GetComponent<ClusterGridEntity>();
			ToolTip component2 = keyValuePair.Value.GetComponent<ToolTip>();
			component2.ClearMultiStringTooltip();
			WorldContainer world = ClusterManager.Instance.GetWorld(keyValuePair.Key);
			if (world != null)
			{
				component2.AddMultiStringTooltip(component.Name, this.titleTextSetting);
				if (!world.IsModuleInterior)
				{
					int num2 = discoveredAsteroidIDsSorted.IndexOf(world.id);
					if (num2 != -1 && num2 <= 9)
					{
						component2.AddMultiStringTooltip(" ", this.bodyTextSetting);
						if (KInputManager.currentControllerIsGamepad)
						{
							component2.AddMultiStringTooltip(UI.FormatAsHotkey(GameUtil.GetActionString(this.IdxToHotkeyAction(num2))), this.bodyTextSetting);
						}
						else
						{
							component2.AddMultiStringTooltip(UI.FormatAsHotkey("[" + GameUtil.GetActionString(this.IdxToHotkeyAction(num2)) + "]"), this.bodyTextSetting);
						}
					}
				}
			}
			else
			{
				component2.AddMultiStringTooltip(UI.CLUSTERMAP.UNKNOWN_DESTINATION, this.titleTextSetting);
			}
			if (ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(world.id) < ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
			{
				component2.AddMultiStringTooltip(ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResultTooltip(world.id), this.bodyTextSetting);
			}
			num++;
		}
	}

	private void SortRows()
	{
		List<KeyValuePair<int, MultiToggle>> list = this.worldRows.ToList<KeyValuePair<int, MultiToggle>>();
		list.Sort(delegate(KeyValuePair<int, MultiToggle> x, KeyValuePair<int, MultiToggle> y)
		{
			float num = (ClusterManager.Instance.GetWorld(x.Key).IsModuleInterior ? float.PositiveInfinity : ClusterManager.Instance.GetWorld(x.Key).DiscoveryTimestamp);
			float num2 = (ClusterManager.Instance.GetWorld(y.Key).IsModuleInterior ? float.PositiveInfinity : ClusterManager.Instance.GetWorld(y.Key).DiscoveryTimestamp);
			return num.CompareTo(num2);
		});
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Value.transform.SetSiblingIndex(i);
		}
		foreach (KeyValuePair<int, MultiToggle> keyValuePair in list)
		{
			keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indent").anchoredPosition = Vector2.zero;
			keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Status").anchoredPosition = Vector2.right * 24f;
			WorldContainer world = ClusterManager.Instance.GetWorld(keyValuePair.Key);
			if (world.ParentWorldId != world.id && world.ParentWorldId != 255)
			{
				foreach (KeyValuePair<int, MultiToggle> keyValuePair2 in list)
				{
					if (keyValuePair2.Key == world.ParentWorldId)
					{
						int siblingIndex = keyValuePair2.Value.gameObject.transform.GetSiblingIndex();
						keyValuePair.Value.gameObject.transform.SetSiblingIndex(siblingIndex + 1);
						keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indent").anchoredPosition = Vector2.right * 32f;
						keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Status").anchoredPosition = Vector2.right * -8f;
						break;
					}
				}
			}
		}
	}

	private global::Action IdxToHotkeyAction(int idx)
	{
		global::Action action;
		switch (idx)
		{
		case 0:
			action = global::Action.SwitchActiveWorld1;
			break;
		case 1:
			action = global::Action.SwitchActiveWorld2;
			break;
		case 2:
			action = global::Action.SwitchActiveWorld3;
			break;
		case 3:
			action = global::Action.SwitchActiveWorld4;
			break;
		case 4:
			action = global::Action.SwitchActiveWorld5;
			break;
		case 5:
			action = global::Action.SwitchActiveWorld6;
			break;
		case 6:
			action = global::Action.SwitchActiveWorld7;
			break;
		case 7:
			action = global::Action.SwitchActiveWorld8;
			break;
		case 8:
			action = global::Action.SwitchActiveWorld9;
			break;
		case 9:
			action = global::Action.SwitchActiveWorld10;
			break;
		default:
			global::Debug.LogError("Action must be a SwitchActiveWorld Action");
			action = global::Action.SwitchActiveWorld1;
			break;
		}
		return action;
	}

	public void Sim4000ms(float dt)
	{
		foreach (KeyValuePair<int, MultiToggle> keyValuePair in this.worldRows)
		{
			ColonyDiagnostic.DiagnosticResult.Opinion worldDiagnosticResult = ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(keyValuePair.Key);
			ColonyDiagnosticScreen.SetIndication(worldDiagnosticResult, keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference("Indicator").gameObject);
			if (this.previousWorldDiagnosticStatus[keyValuePair.Key] > worldDiagnosticResult && ClusterManager.Instance.activeWorldId != keyValuePair.Key)
			{
				this.TriggerVisualNotification(keyValuePair.Key, worldDiagnosticResult);
			}
			this.previousWorldDiagnosticStatus[keyValuePair.Key] = worldDiagnosticResult;
		}
		this.RefreshWorldStatus();
		this.RefreshToggleTooltips();
	}

	public void TriggerVisualNotification(int worldID, ColonyDiagnostic.DiagnosticResult.Opinion result)
	{
		foreach (KeyValuePair<int, MultiToggle> keyValuePair in this.worldRows)
		{
			if (keyValuePair.Key == worldID)
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound(ColonyDiagnosticScreen.notificationSoundsInactive[result], false));
				if (keyValuePair.Value.gameObject.activeInHierarchy)
				{
					keyValuePair.Value.StartCoroutine(this.VisualNotificationRoutine(keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").gameObject, keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indicator"), keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Spacer").gameObject));
				}
			}
		}
	}

	private IEnumerator VisualNotificationRoutine(GameObject contentGameObject, RectTransform indicator, GameObject spacer)
	{
		spacer.GetComponent<NotificationAnimator>().Begin(false);
		Vector2 defaultIndicatorSize = new Vector2(8f, 8f);
		float bounceDuration = 1.5f;
		for (float i = 0f; i < bounceDuration; i += Time.unscaledDeltaTime)
		{
			indicator.sizeDelta = defaultIndicatorSize + Vector2.one * (float)Mathf.RoundToInt(Mathf.Sin(6f * (3.1415927f * (i / bounceDuration))));
			yield return 0;
		}
		for (float i = 0f; i < bounceDuration; i += Time.unscaledDeltaTime)
		{
			indicator.sizeDelta = defaultIndicatorSize + Vector2.one * (float)Mathf.RoundToInt(Mathf.Sin(6f * (3.1415927f * (i / bounceDuration))));
			yield return 0;
		}
		for (float i = 0f; i < bounceDuration; i += Time.unscaledDeltaTime)
		{
			indicator.sizeDelta = defaultIndicatorSize + Vector2.one * (float)Mathf.RoundToInt(Mathf.Sin(6f * (3.1415927f * (i / bounceDuration))));
			yield return 0;
		}
		defaultIndicatorSize = new Vector2(8f, 8f);
		indicator.sizeDelta = defaultIndicatorSize;
		contentGameObject.rectTransform().localPosition = Vector2.zero;
		yield break;
	}

	public static WorldSelector Instance;

	public Dictionary<int, MultiToggle> worldRows;

	public TextStyleSetting titleTextSetting;

	public TextStyleSetting bodyTextSetting;

	public GameObject worldRowPrefab;

	public GameObject worldRowContainer;

	private Dictionary<int, ColonyDiagnostic.DiagnosticResult.Opinion> previousWorldDiagnosticStatus = new Dictionary<int, ColonyDiagnostic.DiagnosticResult.Opinion>();

	private Dictionary<int, List<GameObject>> worldStatusIcons = new Dictionary<int, List<GameObject>>();
}
