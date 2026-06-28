using System;
using System.IO;
using FMOD.Studio;
using UnityEngine;

public class DebugHandler : IInputHandler
{
	public DebugHandler()
	{
		this.enabled = File.Exists(Path.Combine(Application.dataPath, "debug_enable.txt"));
	}

	public KInputHandler inputHandler { get; set; }

	public static int GetMouseCell()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = -Camera.main.transform.position.z - Grid.CellSizeInMeters;
		Vector3 vector = Camera.main.ScreenToWorldPoint(mousePosition);
		return Grid.PosToCell(vector);
	}

	public static Vector3 GetMousePos()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = -Camera.main.transform.position.z - Grid.CellSizeInMeters;
		return Camera.main.ScreenToWorldPoint(mousePosition);
	}

	private void SpawnMinion()
	{
		GameObject gameObject = global::Util.KInstantiate(EntityPrefabs.Instance.MinionPrefab, SceneOrganizer.Instance.GetFolder(Folder.Entities), null);
		gameObject.name = EntityPrefabs.Instance.MinionPrefab.name;
		Vector3 vector = Grid.CellToPosCBC(DebugHandler.GetMouseCell(), Grid.SceneLayer.Move);
		gameObject.transform.localPosition = vector;
		MinionStartingStats minionStartingStats = new MinionStartingStats(false);
		minionStartingStats.Apply(gameObject);
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (!this.enabled)
		{
			return;
		}
		if (e.TryConsume(global::Action.DebugSpawnMinion))
		{
			this.SpawnMinion();
		}
		else if (e.TryConsume(global::Action.DebugSpawnStressTest))
		{
			for (int i = 0; i < 60; i++)
			{
				this.SpawnMinion();
			}
		}
		else if (e.TryConsume(global::Action.DebugSuperTestMode))
		{
			if (!this.superTestMode)
			{
				Time.timeScale = 15f;
				this.superTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				this.superTestMode = false;
			}
		}
		else if (e.TryConsume(global::Action.DebugUltraTestMode))
		{
			if (!this.ultraTestMode)
			{
				Time.timeScale = 30f;
				this.ultraTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				this.ultraTestMode = false;
			}
		}
		else if (e.TryConsume(global::Action.DebugSlowTestMode))
		{
			if (!this.slowTestMode)
			{
				Time.timeScale = 0.06f;
				this.slowTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				this.slowTestMode = false;
			}
		}
		else if (e.TryConsume(global::Action.DebugDig))
		{
			int mouseCell = DebugHandler.GetMouseCell();
			SimMessages.Dig(mouseCell, -1);
		}
		else if (e.TryConsume(global::Action.DebugInstantBuildMode))
		{
			DebugHandler.InstantBuildMode = !DebugHandler.InstantBuildMode;
			if (PlanScreen.Instance != null)
			{
				PlanScreen.Instance.Refresh();
			}
			if (ManagementMenu.Instance != null)
			{
				ManagementMenu.Instance.CheckResearch(null);
			}
		}
		else if (e.TryConsume(global::Action.DebugExplosion))
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = -Camera.main.transform.position.z - Grid.CellSizeInMeters;
			Vector3 vector = Camera.main.ScreenToWorldPoint(mousePosition);
			GameUtil.CreateExplosion(vector);
		}
		else if (e.TryConsume(global::Action.DebugDiscoverAllElements))
		{
			foreach (SimHashes simHashes in (SimHashes[])Enum.GetValues(typeof(SimHashes)))
			{
				WorldInventory.Instance.Discover(TagManager.Create(simHashes));
			}
		}
		else if (e.TryConsume(global::Action.DebugToggleUI))
		{
			DebugHandler.SetHideUI(!DebugHandler.HideUI);
		}
		else if (e.TryConsume(global::Action.DebugCellInfo))
		{
			DebugHandler.DebugCellInfo = !DebugHandler.DebugCellInfo;
		}
		else if (e.TryConsume(global::Action.DebugToggle))
		{
			PropertyTextures.FogOfWarScale = 1f - PropertyTextures.FogOfWarScale;
			DebugHandler.FreeCameraMode = !DebugHandler.FreeCameraMode;
			if (DebugPaintElementScreen.Instance != null)
			{
				bool activeSelf = DebugPaintElementScreen.Instance.gameObject.activeSelf;
				DebugPaintElementScreen.Instance.gameObject.SetActive(!activeSelf);
				if (DebugElementMenu.Instance && DebugElementMenu.Instance.root.activeSelf)
				{
					DebugElementMenu.Instance.root.SetActive(false);
				}
				if (Application.isEditor)
				{
					DebugBaseTemplateButton.Instance.gameObject.SetActive(!activeSelf);
				}
			}
		}
		else if (e.TryConsume(global::Action.DebugCollectGarbage))
		{
			GC.Collect();
		}
		else if (e.TryConsume(global::Action.DebugInvincible))
		{
			DebugHandler.InvincibleMode = !DebugHandler.InvincibleMode;
		}
		else if (e.TryConsume(global::Action.DebugApplyHighAudioReverb))
		{
			this.PlayAudioEvent("event:/Mixes/Set_test_amb_down");
		}
		else if (e.TryConsume(global::Action.DebugApplyLowAudioReverb))
		{
			this.PlayAudioEvent("event:/Mixes/Set_test_amb_up");
		}
		else if (e.TryConsume(global::Action.DebugVisualTest))
		{
			Scenario.Instance.SetupVisualTest();
		}
		else if (e.TryConsume(global::Action.DebugGameplayTest))
		{
			Scenario.Instance.SetupGameplayTest();
		}
		else if (e.TryConsume(global::Action.DebugElementTest))
		{
			Scenario.Instance.SetupElementTest();
		}
		else if (!e.TryConsume(global::Action.ToggleProfiler))
		{
			if (e.TryConsume(global::Action.DebugRefreshNavCell))
			{
				Pathfinding.Instance.RefreshNavCell(DebugHandler.GetMouseCell());
			}
			else if (e.TryConsume(global::Action.DebugToggleSelectInEditor))
			{
				DebugHandler.SetSelectInEditor(!DebugHandler.SelectInEditor);
			}
			else if (e.TryConsume(global::Action.DebugGotoTarget))
			{
				Debug.Log("Debug GoTo");
				Game.Instance.Trigger(775300118, null);
				foreach (Brain brain in Components.Brains)
				{
					DebugGoToMonitor component = brain.GetComponent<DebugGoToMonitor>();
					if (component != null)
					{
						component.GoToCursor();
					}
				}
			}
			else if (e.TryConsume(global::Action.DebugTeleport))
			{
				KSelectable selected = SelectTool.Instance.selected;
				if (selected != null)
				{
					int mouseCell2 = DebugHandler.GetMouseCell();
					if (!Grid.IsValidCell(mouseCell2))
					{
						return;
					}
					selected.transform.SetPosition(Grid.CellToPosCBC(mouseCell2, Grid.SceneLayer.Move));
				}
			}
			else if (!e.TryConsume(global::Action.DebugPlace))
			{
				if (!e.TryConsume(global::Action.DebugSelectMaterial))
				{
					if (e.TryConsume(global::Action.DebugNotification))
					{
						Tutorial.Instance.DebugNotification();
					}
					else if (e.TryConsume(global::Action.DebugNotificationMessage))
					{
						Tutorial.Instance.DebugNotificationMessage();
					}
					else if (e.TryConsume(global::Action.DebugSuperSpeed))
					{
						SpeedControlScreen.Instance.ToggleRidiculousSpeed();
					}
					else if (e.TryConsume(global::Action.DebugToggleMusic))
					{
						AudioDebug.Get().ToggleMusic();
					}
					else if (e.TryConsume(global::Action.DebugRiverTest))
					{
						Scenario.Instance.SetupRiverTest();
					}
					else if (e.TryConsume(global::Action.DebugTileTest))
					{
						Scenario.Instance.SetupTileTest();
					}
					else if (e.TryConsume(global::Action.DebugForceLightEverywhere))
					{
						PropertyTextures.instance.ForceLightEverywhere = !PropertyTextures.instance.ForceLightEverywhere;
					}
					else if (e.TryConsume(global::Action.DebugPathFinding))
					{
						DebugHandler.DebugPathFinding = !DebugHandler.DebugPathFinding;
						Debug.Log("DebugPathFinding=" + DebugHandler.DebugPathFinding);
					}
					else if (!e.TryConsume(global::Action.DebugFocus))
					{
						if (e.TryConsume(global::Action.DebugReportBug))
						{
							int num = 0;
							string validSaveFilename;
							for (;;)
							{
								validSaveFilename = SaveScreen.GetValidSaveFilename("bug_report_savefile_" + num.ToString());
								if (!File.Exists(validSaveFilename))
								{
									break;
								}
								num++;
							}
							KCrashReporter.ReportBug("Bug Report", SaveLoader.Instance.Save(validSaveFilename, false, false));
						}
						else if (e.TryConsume(global::Action.DebugReloadLevel))
						{
							Debug.Log("Reloading Level.");
							SaveLoader.Instance.InitialSave();
							LoadScreen.ForceStopGame();
							SaveLoader.SetActiveSaveFilePath(SaveLoader.GetAutosaveFilePath());
							App.LoadScene("frontend");
						}
						else if (e.TryConsume(global::Action.DebugTriggerException))
						{
							throw new ArgumentException("My test exception");
						}
					}
				}
			}
		}
	}

	public static void SetSelectInEditor(bool select_in_editor)
	{
	}

	public static void SetHideUI(bool hide)
	{
		DebugHandler.HideUI = hide;
		foreach (Canvas canvas in Resources.FindObjectsOfTypeAll<Canvas>())
		{
			canvas.enabled = !DebugHandler.HideUI;
		}
	}

	private void PlayAudioEvent(string eventName)
	{
		EventInstance eventInstance = KFMOD.CreateInstance(eventName);
		if (eventInstance == null)
		{
			Output.LogError(new object[] { "StartSound() Couldnt Get FMOD event for asset [" + eventName + "]" });
			return;
		}
		eventInstance.start();
		eventInstance.release();
		Output.Log(new object[] { "Starting event [" + eventName + "]" });
	}

	public static bool InstantBuildMode;

	public static bool FreeCameraMode;

	public static bool InvincibleMode;

	public static bool SelectInEditor;

	public static bool DebugPathFinding;

	public static bool HideUI;

	public static bool DebugCellInfo;

	private bool enabled;

	private bool superTestMode;

	private bool ultraTestMode;

	private bool slowTestMode;

	public enum PaintMode
	{
		None,
		Element,
		Hot,
		Cold
	}
}
