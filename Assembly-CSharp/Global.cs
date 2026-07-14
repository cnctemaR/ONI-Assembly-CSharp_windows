using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Klei;
using KMod;
using KSerialization;
using ProcGenGame;
using STRINGS;
using UnityEngine;
using UnityEngine.U2D;

public class Global : MonoBehaviour
{
	public static Global Instance { get; private set; }

	public static BindingEntry[] GenerateDefaultBindings(bool hotKeyBuildMenuPermitted = true)
	{
		List<BindingEntry> list = new List<BindingEntry>
		{
			new BindingEntry(null, GamepadButton.Start, KKeyCode.Escape, Modifier.None, global::Action.Escape, false, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, global::Action.PanUp, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, global::Action.PanDown, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, global::Action.PanLeft, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, global::Action.PanRight, true, false),
			new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, global::Action.RotateBuilding, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.L, Modifier.None, global::Action.ManagePriorities, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, global::Action.ManageConsumables, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.V, Modifier.None, global::Action.ManageVitals, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, global::Action.ManageResearch, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, global::Action.ManageReport, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.U, Modifier.None, global::Action.ManageDatabase, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.J, Modifier.None, global::Action.ManageSkills, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.Period, Modifier.None, global::Action.ManageSchedule, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.Z, Modifier.None, global::Action.ManageStarmap, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.G, Modifier.None, global::Action.Dig, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.M, Modifier.None, global::Action.Mop, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.K, Modifier.None, global::Action.Clear, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, global::Action.Disinfect, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, global::Action.Attack, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.N, Modifier.None, global::Action.Capture, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Y, Modifier.None, global::Action.Harvest, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Insert, Modifier.None, global::Action.EmptyPipe, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.D, Modifier.Shift, global::Action.Disconnect, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.P, Modifier.None, global::Action.Prioritize, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.S, Modifier.Alt, global::Action.ToggleScreenshotMode, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, global::Action.BuildingCancel, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.X, Modifier.None, global::Action.BuildingDeconstruct, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Tab, Modifier.None, global::Action.CycleSpeed, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.H, Modifier.None, global::Action.CameraHome, true, false),
			new BindingEntry("Root", GamepadButton.A, KKeyCode.Mouse0, Modifier.None, global::Action.MouseLeft, false, false),
			new BindingEntry("Root", GamepadButton.A, KKeyCode.Mouse0, Modifier.Shift, global::Action.ShiftMouseLeft, false, false),
			new BindingEntry("Root", GamepadButton.B, KKeyCode.Mouse1, Modifier.None, global::Action.MouseRight, false, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse2, Modifier.None, global::Action.MouseMiddle, false, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.None, global::Action.Plan1, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.None, global::Action.Plan2, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.None, global::Action.Plan3, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.None, global::Action.Plan4, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.None, global::Action.Plan5, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.None, global::Action.Plan6, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.None, global::Action.Plan7, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.None, global::Action.Plan8, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.None, global::Action.Plan9, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.None, global::Action.Plan10, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.None, global::Action.Plan11, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.None, global::Action.Plan12, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.Shift, global::Action.Plan13, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.Shift, global::Action.Plan14, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Backspace, Modifier.Shift, global::Action.Plan15, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.B, Modifier.None, global::Action.CopyBuilding, true, false),
			new BindingEntry("Root", GamepadButton.RT, KKeyCode.MouseScrollUp, Modifier.None, global::Action.ZoomIn, true, false),
			new BindingEntry("Root", GamepadButton.LT, KKeyCode.MouseScrollDown, Modifier.None, global::Action.ZoomOut, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F1, Modifier.None, global::Action.Overlay1, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F2, Modifier.None, global::Action.Overlay2, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F3, Modifier.None, global::Action.Overlay3, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F4, Modifier.None, global::Action.Overlay4, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F5, Modifier.None, global::Action.Overlay5, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F6, Modifier.None, global::Action.Overlay6, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F7, Modifier.None, global::Action.Overlay7, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F8, Modifier.None, global::Action.Overlay8, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F9, Modifier.None, global::Action.Overlay9, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F10, Modifier.None, global::Action.Overlay10, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F11, Modifier.None, global::Action.Overlay11, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Shift, global::Action.Overlay12, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F2, Modifier.Shift, global::Action.Overlay13, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Shift, global::Action.Overlay14, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F4, Modifier.Shift, global::Action.Overlay15, DlcManager.EXPANSION1),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.KeypadPlus, Modifier.None, global::Action.SpeedUp, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.KeypadMinus, Modifier.None, global::Action.SlowDown, true, false),
			new BindingEntry("Root", GamepadButton.Back, KKeyCode.Space, Modifier.None, global::Action.TogglePause, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F, Modifier.Ctrl, global::Action.Find, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Ctrl, global::Action.SetUserNav1, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Ctrl, global::Action.SetUserNav2, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Ctrl, global::Action.SetUserNav3, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Ctrl, global::Action.SetUserNav4, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Ctrl, global::Action.SetUserNav5, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.Ctrl, global::Action.SetUserNav6, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.Ctrl, global::Action.SetUserNav7, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.Ctrl, global::Action.SetUserNav8, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Ctrl, global::Action.SetUserNav9, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Ctrl, global::Action.SetUserNav10, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Shift, global::Action.GotoUserNav1, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Shift, global::Action.GotoUserNav2, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Shift, global::Action.GotoUserNav3, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Shift, global::Action.GotoUserNav4, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Shift, global::Action.GotoUserNav5, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.Shift, global::Action.GotoUserNav6, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.Shift, global::Action.GotoUserNav7, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.Shift, global::Action.GotoUserNav8, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Shift, global::Action.GotoUserNav9, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Shift, global::Action.GotoUserNav10, true, false),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, global::Action.CinemaCamEnable, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, global::Action.CinemaPanLeft, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, global::Action.CinemaPanRight, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, global::Action.CinemaPanUp, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, global::Action.CinemaPanDown, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, global::Action.CinemaZoomIn, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, global::Action.CinemaZoomOut, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.Z, Modifier.None, global::Action.CinemaZoomSpeedPlus, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.Z, Modifier.Shift, global::Action.CinemaZoomSpeedMinus, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.P, Modifier.None, global::Action.CinemaUnpauseOnMove, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, global::Action.CinemaToggleLock, true, true),
			new BindingEntry("CinematicCamera", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, global::Action.CinemaToggleEasing, true, true),
			new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Slash, Modifier.None, global::Action.ToggleOpen, true, false),
			new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Return, Modifier.None, global::Action.ToggleEnabled, true, false),
			new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Backslash, Modifier.None, global::Action.BuildingUtility1, true, false),
			new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.LeftBracket, Modifier.None, global::Action.BuildingUtility2, true, false),
			new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.RightBracket, Modifier.None, global::Action.BuildingUtility3, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.LeftAlt, Modifier.Alt, global::Action.AlternateView, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.RightAlt, Modifier.Alt, global::Action.AlternateView, true, false),
			new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.LeftShift, Modifier.Shift, global::Action.DragStraight, true, false),
			new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.RightShift, Modifier.Shift, global::Action.DragStraight, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.T, Modifier.Ctrl, global::Action.DebugFocus, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.U, Modifier.Ctrl, global::Action.DebugUltraTestMode, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Alt, global::Action.DebugToggleUI, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Alt, global::Action.DebugCollectGarbage, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F7, Modifier.Alt, global::Action.DebugInvincible, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Alt, global::Action.DebugForceLightEverywhere, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Shift, global::Action.DebugElementTest, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, Modifier.Shift, global::Action.DebugTileTest, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.N, Modifier.Alt, global::Action.DebugRefreshNavCell, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Q, Modifier.Ctrl, global::Action.DebugGotoTarget, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.S, Modifier.Ctrl, global::Action.DebugSelectMaterial, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.M, Modifier.Ctrl, global::Action.DebugToggleMusic, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F, Modifier.Alt, global::Action.DebugToggleClusterFX, DlcManager.EXPANSION1),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Backspace, Modifier.None, global::Action.DebugToggle, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Backspace, Modifier.Ctrl, global::Action.DebugToggleFastWorkers, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Q, Modifier.Alt, global::Action.DebugTeleport, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F2, Modifier.Alt, global::Action.DebugSpawnMinionAtmoSuit, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F2, Modifier.Ctrl, global::Action.DebugSpawnMinion, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Ctrl, global::Action.DebugPlace, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F4, Modifier.Ctrl, global::Action.DebugInstantBuildMode, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F5, Modifier.Ctrl, global::Action.DebugSlowTestMode, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F6, Modifier.Ctrl, global::Action.DebugDig, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F8, Modifier.Ctrl, global::Action.DebugExplosion, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F9, Modifier.Ctrl, global::Action.DebugDiscoverAllElements, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.T, Modifier.Alt, global::Action.DebugToggleSelectInEditor, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.P, Modifier.Alt, global::Action.DebugPathFinding, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.C, Modifier.Ctrl, global::Action.DebugCheerEmote, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Z, Modifier.Alt, global::Action.DebugSuperSpeed, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.Alt, global::Action.DebugGameStep, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.Alt, global::Action.DebugSimStep, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.X, Modifier.Alt, global::Action.DebugNotification, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.C, Modifier.Alt, global::Action.DebugNotificationMessage, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.BackQuote, Modifier.None, global::Action.ToggleProfiler, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.BackQuote, Modifier.Alt, global::Action.ToggleChromeProfiler, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Ctrl, global::Action.DebugDumpSceneParitionerLeakData, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, Modifier.Ctrl, global::Action.DebugTriggerException, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, (Modifier)6, global::Action.DebugTriggerError, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Ctrl, global::Action.DebugDumpGCRoots, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, (Modifier)3, global::Action.DebugDumpGarbageReferences, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F11, Modifier.Ctrl, global::Action.DebugDumpEventData, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F7, (Modifier)3, global::Action.DebugCrashSim, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Alt, global::Action.DebugNextCall, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Alt, global::Action.SreenShot1x, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Alt, global::Action.SreenShot2x, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Alt, global::Action.SreenShot8x, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Alt, global::Action.SreenShot32x, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Alt, global::Action.DebugLockCursor, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Alt, global::Action.DebugTogglePersonalPriorityComparison, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Return, Modifier.None, global::Action.DialogSubmit, false, false),
			new BindingEntry("Analog", GamepadButton.NumButtons, KKeyCode.None, Modifier.None, global::Action.AnalogCamera, false, false),
			new BindingEntry("Analog", GamepadButton.NumButtons, KKeyCode.None, Modifier.None, global::Action.AnalogCursor, false, false),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, global::Action.BuildMenuKeyA, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.B, Modifier.None, global::Action.BuildMenuKeyB, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, global::Action.BuildMenuKeyC, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, global::Action.BuildMenuKeyD, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, global::Action.BuildMenuKeyE, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, global::Action.BuildMenuKeyF, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.G, Modifier.None, global::Action.BuildMenuKeyG, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.H, Modifier.None, global::Action.BuildMenuKeyH, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, global::Action.BuildMenuKeyI, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.J, Modifier.None, global::Action.BuildMenuKeyJ, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.K, Modifier.None, global::Action.BuildMenuKeyK, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.L, Modifier.None, global::Action.BuildMenuKeyL, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.M, Modifier.None, global::Action.BuildMenuKeyM, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.N, Modifier.None, global::Action.BuildMenuKeyN, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, global::Action.BuildMenuKeyO, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.P, Modifier.None, global::Action.BuildMenuKeyP, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Q, Modifier.None, global::Action.BuildMenuKeyQ, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, global::Action.BuildMenuKeyR, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, global::Action.BuildMenuKeyS, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, global::Action.BuildMenuKeyT, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.U, Modifier.None, global::Action.BuildMenuKeyU, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.V, Modifier.None, global::Action.BuildMenuKeyV, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, global::Action.BuildMenuKeyW, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.X, Modifier.None, global::Action.BuildMenuKeyX, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Y, Modifier.None, global::Action.BuildMenuKeyY, false, true),
			new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Z, Modifier.None, global::Action.BuildMenuKeyZ, false, true),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.B, Modifier.Shift, global::Action.SandboxBrush, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.N, Modifier.Shift, global::Action.SandboxSprinkle, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.F, Modifier.Shift, global::Action.SandboxFlood, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.K, Modifier.Shift, global::Action.SandboxSample, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.H, Modifier.Shift, global::Action.SandboxHeatGun, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.J, Modifier.Shift, global::Action.SandboxStressTool, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.C, Modifier.Shift, global::Action.SandboxClearFloor, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.X, Modifier.Shift, global::Action.SandboxDestroy, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.E, Modifier.Shift, global::Action.SandboxSpawnEntity, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.S, Modifier.Shift, global::Action.ToggleSandboxTools, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.R, Modifier.Shift, global::Action.SandboxReveal, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.Z, Modifier.Shift, global::Action.SandboxCritterTool, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.T, Modifier.Shift, global::Action.SandboxStoryTraitTool, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.Mouse0, Modifier.Ctrl, global::Action.SandboxCopyElement, true, false),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Backtick, global::Action.SwitchActiveWorld1, true, false, DlcManager.EXPANSION1),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Backtick, global::Action.SwitchActiveWorld2, true, false, DlcManager.EXPANSION1),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Backtick, global::Action.SwitchActiveWorld3, true, false, DlcManager.EXPANSION1),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Backtick, global::Action.SwitchActiveWorld4, true, false, DlcManager.EXPANSION1),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Backtick, global::Action.SwitchActiveWorld5, true, false, DlcManager.EXPANSION1),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.Backtick, global::Action.SwitchActiveWorld6, true, false, DlcManager.EXPANSION1),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.Backtick, global::Action.SwitchActiveWorld7, true, false, DlcManager.EXPANSION1),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.Backtick, global::Action.SwitchActiveWorld8, true, false, DlcManager.EXPANSION1),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Backtick, global::Action.SwitchActiveWorld9, true, false, DlcManager.EXPANSION1),
			new BindingEntry("SwitchActiveWorld", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Backtick, global::Action.SwitchActiveWorld10, true, false, DlcManager.EXPANSION1)
		};
		IList<BuildMenu.DisplayInfo> list2 = (IList<BuildMenu.DisplayInfo>)BuildMenu.OrderedBuildings.data;
		if (BuildMenu.UseHotkeyBuildMenu() && hotKeyBuildMenuPermitted)
		{
			foreach (BuildMenu.DisplayInfo displayInfo in list2)
			{
				Global.AddBindings(HashedString.Invalid, displayInfo, list);
			}
		}
		return list.ToArray();
	}

	private static void AddBindings(HashedString parent_category, BuildMenu.DisplayInfo display_info, List<BindingEntry> bindings)
	{
		if (display_info.data != null)
		{
			Type type = display_info.data.GetType();
			if (typeof(IList<BuildMenu.DisplayInfo>).IsAssignableFrom(type))
			{
				using (IEnumerator<BuildMenu.DisplayInfo> enumerator = ((IList<BuildMenu.DisplayInfo>)display_info.data).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BuildMenu.DisplayInfo displayInfo = enumerator.Current;
						Global.AddBindings(display_info.category, displayInfo, bindings);
					}
					return;
				}
			}
			if (typeof(IList<BuildMenu.BuildingInfo>).IsAssignableFrom(type))
			{
				string text = HashCache.Get().Get(parent_category);
				string text2 = new CultureInfo("en-US", false).TextInfo.ToTitleCase(text) + " Menu";
				BindingEntry bindingEntry = new BindingEntry(text2, GamepadButton.NumButtons, display_info.keyCode, Modifier.None, display_info.hotkey, true, true);
				bindings.Add(bindingEntry);
			}
		}
	}

	private void Awake()
	{
		KCrashReporter crash_reporter = base.GetComponent<KCrashReporter>();
		if ((crash_reporter != null) & (SceneInitializerLoader.ReportDeferredError == null))
		{
			SceneInitializerLoader.ReportDeferredError = delegate(SceneInitializerLoader.DeferredError deferred_error)
			{
				crash_reporter.ShowDialog(deferred_error.msg, deferred_error.stack_trace, true, null, null);
			};
		}
		this.globalCanvas = GameObject.Find("Canvas");
		global::UnityEngine.Object.DontDestroyOnLoad(this.globalCanvas.gameObject);
		this.OutputSystemInfo();
		global::Debug.Assert(Global.Instance == null);
		Global.Instance = this;
		global::Debug.Log("Initializing at " + global::System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"));
		global::Debug.Log("Save path: " + Util.RootFolder());
		MyCmp.Init();
		MySmi.Init();
		DevToolManager.Instance.Init();
		if (this.forcedAtlasInitializationList != null)
		{
			foreach (SpriteAtlas spriteAtlas in this.forcedAtlasInitializationList)
			{
				Sprite[] array2 = new Sprite[spriteAtlas.spriteCount];
				spriteAtlas.GetSprites(array2);
				Sprite[] array3 = array2;
				for (int j = 0; j < array3.Length; j++)
				{
					Texture2D texture = array3[j].texture;
					if (texture != null)
					{
						texture.filterMode = FilterMode.Bilinear;
						texture.anisoLevel = 4;
						texture.mipMapBias = 0f;
					}
				}
			}
		}
		FileSystem.Initialize();
		Singleton<StateMachineUpdater>.CreateInstance();
		Singleton<StateMachineManager>.CreateInstance();
		Localization.RegisterForTranslation(typeof(UI));
		this.modManager = new global::KMod.Manager();
		this.modManager.LoadModDBAndInitialize();
		this.modManager.SetModLoadingInProgress(true);
		this.modManager.Load(Content.DLL);
		this.modManager.Load(Content.Strings);
		global::KSerialization.Manager.Initialize();
		Global.InitializeGlobalInput();
		Global.InitializeGlobalSound();
		Global.InitializeGlobalAnimation();
		Localization.Initialize();
		this.modManager.Load(Content.Translation);
		this.modManager.distribution_platforms.Add(new Local("Local", Label.DistributionPlatform.Local, false));
		this.modManager.distribution_platforms.Add(new Local("Dev", Label.DistributionPlatform.Dev, true));
		TuningSystem.main_thread = Thread.CurrentThread;
		this.RestoreLegacyMetricsSetting();
		this.TestDataLocations();
		DistributionPlatform.onExitRequest += this.OnExitRequest;
		DistributionPlatform.onDlcAuthenticationFailed += this.OnDlcAuthenticationFailed;
		if (DistributionPlatform.Initialized)
		{
			if (!KPrivacyPrefs.instance.disableDataCollection)
			{
				string[] array4 = new string[6];
				array4[0] = "Logged into ";
				array4[1] = DistributionPlatform.Inst.Name;
				array4[2] = " with ID:";
				int num = 3;
				DistributionPlatform.UserId id = DistributionPlatform.Inst.LocalUser.Id;
				array4[num] = ((id != null) ? id.ToString() : null);
				array4[4] = ", NAME:";
				array4[5] = DistributionPlatform.Inst.LocalUser.Name;
				global::Debug.Log(string.Concat(array4));
				ThreadedHttps<KleiAccount>.Instance.AuthenticateUser(new KleiAccount.GetUserIDdelegate(this.OnGetUserIdKey), false);
			}
			else
			{
				global::Debug.Log("Data collection disabled, account will not be used.");
			}
		}
		else
		{
			global::Debug.LogWarning("Can't init " + DistributionPlatform.Inst.Name + " distribution platform...");
			this.OnGetUserIdKey();
		}
		ThreadedHttps<KleiItems>.Instance.LoadInventoryCache();
		this.modManager.Load(Content.LayerableFiles);
		WorldGen.LoadSettings(true);
		base.StartCoroutine(WorldGen.ListenForLoadSettingsErrorRoutine());
		GlobalResources.Instance();
	}

	private static void InitializeGlobalInput()
	{
		if (Game.IsQuitting())
		{
			return;
		}
		Global.mInputManager = new GameInputManager(Global.GenerateDefaultBindings(true));
	}

	private static void InitializeGlobalSound()
	{
		Audio.Get();
		Singleton<SoundEventVolumeCache>.CreateInstance();
	}

	private static void InitializeGlobalAnimation()
	{
		KAnimBatchManager.CreateInstance();
		Singleton<AnimEventManager>.CreateInstance();
		Singleton<KBatchedAnimUpdater>.CreateInstance();
	}

	private void OnExitRequest()
	{
		bool flag = true;
		if (Game.Instance != null)
		{
			string filename = SaveLoader.GetActiveSaveFilePath();
			if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
			{
				flag = false;
				KScreen component = KScreenManager.AddChild(this.globalCanvas, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<KScreen>();
				component.Activate();
				component.GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(string.Format(UI.FRONTEND.RAILFORCEQUIT.SAVE_EXIT, Path.GetFileNameWithoutExtension(filename)), delegate
				{
					SaveLoader.Instance.Save(filename, false, true);
					App.Quit();
				}, delegate
				{
					App.Quit();
				}, null, null, null, null, null, null);
			}
		}
		if (flag)
		{
			KScreen component2 = KScreenManager.AddChild(this.globalCanvas, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<KScreen>();
			component2.Activate();
			component2.GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(UI.FRONTEND.RAILFORCEQUIT.WARN_EXIT, delegate
			{
				App.Quit();
			}, null, null, null, null, null, null, null);
		}
	}

	private void OnDlcAuthenticationFailed()
	{
		if (DlcManager.IsExpansion1Active())
		{
			KScreen component = KScreenManager.AddChild(this.globalCanvas, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<KScreen>();
			component.Activate();
			ConfirmDialogScreen component2 = component.GetComponent<ConfirmDialogScreen>();
			component2.deactivateOnCancelAction = false;
			component2.PopupConfirmDialog(UI.FRONTEND.RAILFORCEQUIT.DLC_NOT_PURCHASED, delegate
			{
				App.Quit();
			}, null, null, null, null, null, null, null);
		}
	}

	private void RestoreLegacyMetricsSetting()
	{
		if (KPlayerPrefs.GetInt("ENABLE_METRICS", 1) == 0)
		{
			KPlayerPrefs.DeleteKey("ENABLE_METRICS");
			KPlayerPrefs.Save();
			KPrivacyPrefs.instance.disableDataCollection = true;
			KPrivacyPrefs.Save();
		}
	}

	private void TestDataLocations()
	{
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			try
			{
				string text = Util.RootFolder();
				string text2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				text2 = Path.Combine(text2, "Klei");
				text2 = Path.Combine(text2, Util.GetTitleFolderName());
				global::Debug.Log("Test Data Location / docs / " + text);
				global::Debug.Log("Test Data Location / local / " + text2);
				if (!global::System.IO.Directory.Exists(text2))
				{
					global::System.IO.Directory.CreateDirectory(text2);
				}
				if (!global::System.IO.Directory.Exists(text))
				{
					global::System.IO.Directory.CreateDirectory(text);
				}
				string text3 = Path.Combine(text, "test");
				string text4 = Path.Combine(text2, "test");
				string[] array = new string[] { text3, text4 };
				bool[] array2 = new bool[2];
				bool[] array3 = new bool[2];
				bool[] array4 = new bool[2];
				for (int i = 0; i < array.Length; i++)
				{
					try
					{
						using (FileStream fileStream = File.Open(array[i], FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
						{
							byte[] bytes = Encoding.UTF8.GetBytes("test");
							fileStream.Write(bytes, 0, bytes.Length);
							array2[i] = true;
						}
					}
					catch (Exception ex)
					{
						array2[i] = false;
						DebugUtil.LogWarningArgs(new object[] { "Test Data Locations / failed to write " + array[i] + ": " + ex.Message });
					}
					try
					{
						using (FileStream fileStream2 = File.Open(array[i], FileMode.Open, FileAccess.Read))
						{
							Encoding utf = Encoding.UTF8;
							byte[] array5 = new byte[fileStream2.Length];
							if ((long)fileStream2.Read(array5, 0, array5.Length) == fileStream2.Length)
							{
								string @string = utf.GetString(array5);
								if (@string == "test")
								{
									array3[i] = true;
								}
								else
								{
									array3[i] = false;
									DebugUtil.LogWarningArgs(new object[] { string.Concat(new string[]
									{
										"Test Data Locations / failed to validate contents ",
										array[i],
										", got: `",
										@string,
										"`"
									}) });
								}
							}
						}
					}
					catch (Exception ex2)
					{
						array3[i] = false;
						DebugUtil.LogWarningArgs(new object[] { "Test Data Locations / failed to read " + array[i] + ": " + ex2.Message });
					}
					try
					{
						File.Delete(array[i]);
						array4[i] = true;
					}
					catch (Exception ex3)
					{
						array4[i] = false;
						DebugUtil.LogWarningArgs(new object[] { "Test Data Locations / failed to remove " + array[i] + ": " + ex3.Message });
					}
				}
				for (int j = 0; j < array.Length; j++)
				{
					global::Debug.Log(string.Concat(new string[]
					{
						"Test Data Locations / ",
						array[j],
						" / write ",
						array2[j].ToString(),
						" / read ",
						array3[j].ToString(),
						" / removed ",
						array4[j].ToString()
					}));
				}
				bool flag = array2[0] && array3[0];
				bool flag2 = array2[1] && array3[1];
				if (flag && flag2)
				{
					Global.saveFolderTestResult = "both";
				}
				else if (flag && !flag2)
				{
					Global.saveFolderTestResult = "docs_only";
				}
				else if (!flag && flag2)
				{
					Global.saveFolderTestResult = "local_only";
				}
				else
				{
					Global.saveFolderTestResult = "neither";
				}
			}
			catch (Exception ex4)
			{
				KCrashReporter.Assert(false, "Test Data Locations / failed: " + ex4.Message, new string[] { KCrashReporter.CRASH_CATEGORY.FILEIO });
			}
		}
	}

	public static GameInputManager GetInputManager()
	{
		if (Global.mInputManager == null)
		{
			Global.InitializeGlobalInput();
		}
		return Global.mInputManager;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (Global.mInputManager != null)
		{
			Global.mInputManager.OnApplicationFocus(focus);
		}
	}

	private void OnGetUserIdKey()
	{
		this.gotKleiUserID = true;
	}

	private void Update()
	{
		ImGuiRenderer instance = ImGuiRenderer.GetInstance();
		if (instance)
		{
			this.DevTools.UpdateShouldShowTools();
			instance.gameObject.transform.parent.gameObject.SetActive(this.DevTools.Show);
			if (this.DevTools.Show)
			{
				instance.NewFrame();
			}
			this.DevTools.UpdateTools();
		}
		Global.mInputManager.Update();
		if (Singleton<AnimEventManager>.Instance != null)
		{
			Singleton<AnimEventManager>.Instance.Update();
		}
		if (DistributionPlatform.Initialized && !this.updated_with_initialized_distribution_platform)
		{
			this.updated_with_initialized_distribution_platform = true;
			SteamUGCService.Initialize();
			Steam steam = new Steam();
			SteamUGCService.Instance.AddClient(steam);
			this.modManager.distribution_platforms.Add(steam);
			SteamAchievementService.Initialize();
		}
		if (this.gotKleiUserID)
		{
			this.gotKleiUserID = false;
			ThreadedHttps<KleiMetrics>.Instance.SetCallBacks(new global::System.Action(this.SetONIStaticSessionVariables), new Action<Dictionary<string, object>>(this.SetONIDynamicSessionVariables));
			ThreadedHttps<KleiMetrics>.Instance.StartSession();
			KleiItems.AddRequestInventoryRefresh(null);
			KleiItems.AddRequestGetPricingInfo(null);
		}
		ThreadedHttps<KleiMetrics>.Instance.SetLastUserAction(KInputManager.lastUserActionTicks);
		Localization.VerifyTranslationModSubscription(this.globalCanvas);
		if (DistributionPlatform.Initialized)
		{
			ThreadedHttps<KleiItems>.Instance.Update();
		}
	}

	private void SetONIStaticSessionVariables()
	{
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Branch", "release");
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Build", 722606U);
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("SaveFolderWriteTest", Global.saveFolderTestResult);
		if (KPlayerPrefs.HasKey(UnitConfigurationScreen.MassUnitKey))
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.MassUnitKey, ((GameUtil.MassUnit)KPlayerPrefs.GetInt(UnitConfigurationScreen.MassUnitKey)).ToString());
		}
		if (KPlayerPrefs.HasKey(UnitConfigurationScreen.TemperatureUnitKey))
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.TemperatureUnitKey, ((GameUtil.TemperatureUnit)KPlayerPrefs.GetInt(UnitConfigurationScreen.TemperatureUnitKey)).ToString());
		}
		int selectedLanguageType = (int)Localization.GetSelectedLanguageType();
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(Global.LanguageCodeKey, Localization.GetCurrentLanguageCode());
		if (selectedLanguageType == 2)
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(Global.LanguageModKey, LanguageOptionsScreen.GetSavedLanguageMod());
		}
	}

	private void SetONIDynamicSessionVariables(Dictionary<string, object> data)
	{
		if (Game.Instance != null && GameClock.Instance != null)
		{
			data.Add("GameTimeSeconds", (uint)GameClock.Instance.GetTime());
			data.Add("WasDebugEverUsed", Game.Instance.debugWasUsed);
			data.Add("IsSandboxEnabled", SaveGame.Instance.sandboxEnabled);
		}
	}

	private void LateUpdate()
	{
		StreamedTextures.UpdateRequests();
		Singleton<KBatchedAnimUpdater>.Instance.LateUpdate();
		if (this.DevTools.Show)
		{
			ImGuiRenderer instance = ImGuiRenderer.GetInstance();
			if (instance == null)
			{
				return;
			}
			instance.EndFrame();
		}
	}

	private void OnDestroy()
	{
		if (this.modManager != null)
		{
			this.modManager.Shutdown();
		}
		Global.Instance = null;
		if (Singleton<AnimEventManager>.Instance != null)
		{
			Singleton<AnimEventManager>.Instance.FreeResources();
		}
		Singleton<KBatchedAnimUpdater>.DestroyInstance();
	}

	private void OnApplicationQuit()
	{
		KGlobalAnimParser.DestroyInstance();
		ThreadedHttps<KleiMetrics>.Instance.EndSession(false);
	}

	private void OutputSystemInfo()
	{
		try
		{
			Console.WriteLine("SYSTEM INFO:");
			foreach (KeyValuePair<string, object> keyValuePair in KleiMetrics.GetHardwareStats())
			{
				try
				{
					Console.WriteLine(string.Format("    {0}={1}", keyValuePair.Key.ToString(), keyValuePair.Value.ToString()));
				}
				catch
				{
				}
			}
			Console.WriteLine(string.Format("    {0}={1}", "System Language", Application.systemLanguage.ToString()));
		}
		catch
		{
		}
	}

	public SpriteAtlas[] forcedAtlasInitializationList;

	public GameObject modErrorsPrefab;

	public GameObject globalCanvas;

	private static GameInputManager mInputManager;

	private DevToolManager DevTools = new DevToolManager();

	public global::KMod.Manager modManager;

	private bool gotKleiUserID;

	private static string saveFolderTestResult = "unknown";

	private bool updated_with_initialized_distribution_platform;

	public static readonly string LanguageModKey = "LanguageMod";

	public static readonly string LanguageCodeKey = "LanguageCode";
}
