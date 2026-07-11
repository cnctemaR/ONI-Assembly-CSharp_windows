using System;
using System.Diagnostics;
using System.IO;
using Klei;
using STRINGS;
using UnityEngine;

public class DebugHandler : IInputHandler
{
	public DebugHandler()
	{
		DebugHandler.enabled = File.Exists(Path.Combine(Application.dataPath, "debug_enable.txt"));
		DebugHandler.enabled = DebugHandler.enabled || File.Exists(Path.Combine(Application.dataPath, "../debug_enable.txt"));
		DebugHandler.enabled = DebugHandler.enabled || GenericGameSettings.instance.debugEnable;
	}

	public static bool enabled { get; private set; }

	public string handlerName
	{
		get
		{
			return "DebugHandler";
		}
	}

	public KInputHandler inputHandler { get; set; }

	public static int GetMouseCell()
	{
		Vector3 mousePos = KInputManager.GetMousePos();
		mousePos.z = -Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
		Vector3 vector = Camera.main.ScreenToWorldPoint(mousePos);
		return Grid.PosToCell(vector);
	}

	public static Vector3 GetMousePos()
	{
		Vector3 mousePos = KInputManager.GetMousePos();
		mousePos.z = -Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
		return Camera.main.ScreenToWorldPoint(mousePos);
	}

	private void SpawnMinion()
	{
		if (Immigration.Instance == null)
		{
			return;
		}
		if (!Grid.IsValidBuildingCell(DebugHandler.GetMouseCell()))
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.DEBUG_TOOLS.INVALID_LOCATION, null, DebugHandler.GetMousePos(), 1.5f, false, true);
			return;
		}
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 vector = Grid.CellToPosCBC(DebugHandler.GetMouseCell(), Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(vector);
		gameObject.SetActive(true);
		MinionStartingStats minionStartingStats = new MinionStartingStats(false, null);
		minionStartingStats.Apply(gameObject);
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (!DebugHandler.enabled)
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
			if (Game.Instance == null)
			{
				return;
			}
			if (PlanScreen.Instance != null)
			{
				PlanScreen.Instance.Refresh();
			}
			if (BuildMenu.Instance != null)
			{
				BuildMenu.Instance.Refresh();
			}
			if (OverlayMenu.Instance != null)
			{
				OverlayMenu.Instance.Refresh();
			}
			if (ConsumerManager.instance != null)
			{
				ConsumerManager.instance.RefreshDiscovered(null);
			}
			if (ManagementMenu.Instance != null)
			{
				ManagementMenu.Instance.CheckResearch(null);
				ManagementMenu.Instance.CheckSkills(null);
				ManagementMenu.Instance.CheckStarmap(null);
			}
			Game.Instance.Trigger(1594320620, "all_the_things");
		}
		else if (e.TryConsume(global::Action.DebugExplosion))
		{
			Vector3 mousePos = KInputManager.GetMousePos();
			mousePos.z = -Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
			Vector3 vector = Camera.main.ScreenToWorldPoint(mousePos);
			GameUtil.CreateExplosion(vector);
		}
		else if (e.TryConsume(global::Action.DebugLockCursor))
		{
			if (GenericGameSettings.instance.developerDebugEnable)
			{
				KInputManager.isMousePosLocked = !KInputManager.isMousePosLocked;
				KInputManager.lockedMousePos = KInputManager.GetMousePos();
			}
		}
		else if (e.TryConsume(global::Action.DebugDiscoverAllElements))
		{
			if (WorldInventory.Instance != null)
			{
				foreach (Element element in ElementLoader.elements)
				{
					WorldInventory.Instance.Discover(element.tag, element.GetMaterialCategoryTag());
				}
			}
		}
		else if (e.TryConsume(global::Action.DebugToggleUI))
		{
			DebugHandler.ToggleScreenshotMode();
		}
		else if (e.TryConsume(global::Action.SreenShot1x))
		{
			string text = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			ScreenCapture.CaptureScreenshot(text, 1);
		}
		else if (e.TryConsume(global::Action.SreenShot2x))
		{
			string text2 = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			ScreenCapture.CaptureScreenshot(text2, 2);
		}
		else if (e.TryConsume(global::Action.SreenShot8x))
		{
			string text3 = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			ScreenCapture.CaptureScreenshot(text3, 8);
		}
		else if (e.TryConsume(global::Action.SreenShot32x))
		{
			string text4 = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			ScreenCapture.CaptureScreenshot(text4, 32);
		}
		else if (e.TryConsume(global::Action.DebugCellInfo))
		{
			DebugHandler.DebugCellInfo = !DebugHandler.DebugCellInfo;
		}
		else if (e.TryConsume(global::Action.DebugToggle))
		{
			if (Game.Instance != null)
			{
				Game.Instance.UpdateGameActiveRegion(0, 0, Grid.WidthInCells, Grid.HeightInCells);
				SaveGame.Instance.worldGenSpawner.SpawnEverything();
			}
			if (DebugPaintElementScreen.Instance != null)
			{
				bool activeSelf = DebugPaintElementScreen.Instance.gameObject.activeSelf;
				DebugPaintElementScreen.Instance.gameObject.SetActive(!activeSelf);
				if (DebugElementMenu.Instance && DebugElementMenu.Instance.root.activeSelf)
				{
					DebugElementMenu.Instance.root.SetActive(false);
				}
				DebugBaseTemplateButton.Instance.gameObject.SetActive(!activeSelf);
				PropertyTextures.FogOfWarScale = (float)(activeSelf ? 0 : 1);
				if (CameraController.Instance != null)
				{
					CameraController.Instance.EnableFreeCamera(!activeSelf);
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
		else if (e.TryConsume(global::Action.ToggleProfiler))
		{
			Sim.SIM_HandleMessage(-409964931, 0, null);
		}
		else if (e.TryConsume(global::Action.DebugRefreshNavCell))
		{
			Pathfinding.Instance.RefreshNavCell(DebugHandler.GetMouseCell());
		}
		else if (e.TryConsume(global::Action.DebugToggleSelectInEditor))
		{
			DebugHandler.SetSelectInEditor(!DebugHandler.SelectInEditor);
		}
		else if (e.TryConsume(global::Action.DebugGotoTarget))
		{
			global::Debug.Log("Debug GoTo");
			Game.Instance.Trigger(775300118, null);
			foreach (Brain brain in Components.Brains.Items)
			{
				DebugGoToMonitor.Instance smi = brain.GetSMI<DebugGoToMonitor.Instance>();
				if (smi != null)
				{
					smi.GoToCursor();
				}
				CreatureDebugGoToMonitor.Instance smi2 = brain.GetSMI<CreatureDebugGoToMonitor.Instance>();
				if (smi2 != null)
				{
					smi2.GoToCursor();
				}
			}
		}
		else if (e.TryConsume(global::Action.DebugTeleport))
		{
			if (SelectTool.Instance == null)
			{
				return;
			}
			KSelectable selected = SelectTool.Instance.selected;
			if (selected != null)
			{
				int mouseCell2 = DebugHandler.GetMouseCell();
				if (!Grid.IsValidBuildingCell(mouseCell2))
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.DEBUG_TOOLS.INVALID_LOCATION, null, DebugHandler.GetMousePos(), 1.5f, false, true);
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
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						Tutorial.Instance.DebugNotification();
					}
				}
				else if (e.TryConsume(global::Action.DebugNotificationMessage))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						Tutorial.Instance.DebugNotificationMessage();
					}
				}
				else if (e.TryConsume(global::Action.DebugSuperSpeed))
				{
					if (SpeedControlScreen.Instance != null)
					{
						SpeedControlScreen.Instance.ToggleRidiculousSpeed();
					}
				}
				else if (e.TryConsume(global::Action.DebugGameStep))
				{
					if (SpeedControlScreen.Instance != null)
					{
						SpeedControlScreen.Instance.DebugStepFrame();
					}
				}
				else if (e.TryConsume(global::Action.DebugSimStep))
				{
					Game.Instance.ForceSimStep();
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
					global::Debug.Log("DebugPathFinding=" + DebugHandler.DebugPathFinding);
				}
				else if (!e.TryConsume(global::Action.DebugFocus))
				{
					if (e.TryConsume(global::Action.DebugReportBug))
					{
						if (GenericGameSettings.instance.developerDebugEnable)
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
							string text5 = "No save file (front end)";
							if (SaveLoader.Instance != null)
							{
								text5 = SaveLoader.Instance.Save(validSaveFilename, false, false);
							}
							KCrashReporter.ReportBug("Bug Report", text5);
						}
						else
						{
							global::Debug.Log("Debug crash keys are not enabled.");
						}
					}
					else if (e.TryConsume(global::Action.DebugTriggerException))
					{
						if (GenericGameSettings.instance.developerDebugEnable)
						{
							string text6 = Guid.NewGuid().ToString();
							StackTrace stackTrace = new StackTrace(1, true);
							text6 = text6 + "\n" + stackTrace.ToString();
							KCrashReporter.ReportError("Debug crash with random stack", text6, null, ScreenPrefabs.Instance.ConfirmDialogScreen, string.Empty);
						}
					}
					else if (e.TryConsume(global::Action.DebugTriggerError))
					{
						if (GenericGameSettings.instance.developerDebugEnable)
						{
							global::Debug.LogError("Oooops! Testing error!");
						}
					}
					else if (e.TryConsume(global::Action.DebugDumpGCRoots))
					{
						GarbageProfiler.DebugDumpRootItems();
					}
					else if (e.TryConsume(global::Action.DebugDumpGarbageReferences))
					{
						GarbageProfiler.DebugDumpGarbageStats();
					}
					else if (e.TryConsume(global::Action.DebugDumpEventData))
					{
						if (GenericGameSettings.instance.developerDebugEnable)
						{
							KObjectManager.Instance.DumpEventData();
						}
					}
					else if (e.TryConsume(global::Action.DebugDumpSceneParitionerLeakData))
					{
						if (GenericGameSettings.instance.developerDebugEnable)
						{
						}
					}
					else if (e.TryConsume(global::Action.DebugCrashSim))
					{
						if (GenericGameSettings.instance.developerDebugEnable)
						{
							Sim.SIM_DebugCrash();
						}
					}
					else if (e.TryConsume(global::Action.DebugNextCall))
					{
						DebugHandler.DebugNextCall = true;
					}
					else if (e.TryConsume(global::Action.DebugTogglePersonalPriorityComparison))
					{
						Chore.ENABLE_PERSONAL_PRIORITIES = !Chore.ENABLE_PERSONAL_PRIORITIES;
					}
				}
			}
		}
		if (e.Consumed && Game.Instance != null)
		{
			Game.Instance.debugWasUsed = true;
			KCrashReporter.debugWasUsed = true;
		}
	}

	public static void SetSelectInEditor(bool select_in_editor)
	{
	}

	public static void ToggleScreenshotMode()
	{
		DebugHandler.SetHideUI(!DebugHandler.HideUI);
		if (CameraController.Instance != null)
		{
			CameraController.Instance.EnableFreeCamera(DebugHandler.HideUI);
		}
		if (KScreenManager.Instance != null)
		{
			KScreenManager.Instance.DisableInput(DebugHandler.HideUI);
		}
	}

	public static void SetHideUI(bool hide)
	{
		DebugHandler.HideUI = hide;
		float num = ((!DebugHandler.HideUI) ? 1f : 0f);
		GameScreenManager.Instance.ssHoverTextCanvas.GetComponent<CanvasGroup>().alpha = num;
		GameScreenManager.Instance.ssCameraCanvas.GetComponent<CanvasGroup>().alpha = num;
		GameScreenManager.Instance.ssOverlayCanvas.GetComponent<CanvasGroup>().alpha = num;
		GameScreenManager.Instance.worldSpaceCanvas.GetComponent<CanvasGroup>().alpha = num;
		GameScreenManager.Instance.screenshotModeCanvas.GetComponent<CanvasGroup>().alpha = 1f - num;
	}

	public static bool InstantBuildMode;

	public static bool InvincibleMode;

	public static bool SelectInEditor;

	public static bool DebugPathFinding;

	public static bool HideUI;

	public static bool DebugCellInfo;

	public static bool DebugNextCall;

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
