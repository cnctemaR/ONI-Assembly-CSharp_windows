using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public static class GUIScaleUtility
	{
		public static Rect getTopRect
		{
			get
			{
				return GUIScaleUtility.GetTopRectDelegate();
			}
		}

		public static Rect getTopRectScreenSpace
		{
			get
			{
				return GUIScaleUtility.topmostRectDelegate();
			}
		}

		public static List<Rect> currentRectStack { get; private set; }

		public static void CheckInit()
		{
			if (!GUIScaleUtility.initiated)
			{
				GUIScaleUtility.Init();
			}
		}

		public static void Init()
		{
			Type type = Assembly.GetAssembly(typeof(GUI)).GetType("UnityEngine.GUIClip", true);
			PropertyInfo property = type.GetProperty("topmostRect", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method = type.GetMethod("GetTopRect", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo method2 = type.GetMethod("Clip", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder, new Type[] { typeof(Rect) }, new ParameterModifier[0]);
			if (type == null || property == null || method == null || method2 == null)
			{
				global::Debug.LogWarning("GUIScaleUtility cannot run on this system! Compability mode enabled. For you that means you're not able to use the Node Editor inside more than one group:( Please PM me (Seneral @UnityForums) so I can figure out what causes this! Thanks!");
				global::Debug.LogWarning(((type == null) ? "GUIClipType is Null, " : "") + ((property == null) ? "topmostRect is Null, " : "") + ((method == null) ? "GetTopRect is Null, " : "") + ((method2 == null) ? "ClipRect is Null, " : ""));
				GUIScaleUtility.compabilityMode = true;
				GUIScaleUtility.initiated = true;
				return;
			}
			GUIScaleUtility.GetTopRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), method);
			GUIScaleUtility.topmostRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), property.GetGetMethod());
			if (GUIScaleUtility.GetTopRectDelegate == null || GUIScaleUtility.topmostRectDelegate == null)
			{
				global::Debug.LogWarning("GUIScaleUtility cannot run on this system! Compability mode enabled. For you that means you're not able to use the Node Editor inside more than one group:( Please PM me (Seneral @UnityForums) so I can figure out what causes this! Thanks!");
				global::Debug.LogWarning(((type == null) ? "GUIClipType is Null, " : "") + ((property == null) ? "topmostRect is Null, " : "") + ((method == null) ? "GetTopRect is Null, " : "") + ((method2 == null) ? "ClipRect is Null, " : ""));
				GUIScaleUtility.compabilityMode = true;
				GUIScaleUtility.initiated = true;
				return;
			}
			GUIScaleUtility.currentRectStack = new List<Rect>();
			GUIScaleUtility.rectStackGroups = new List<List<Rect>>();
			GUIScaleUtility.GUIMatrices = new List<Matrix4x4>();
			GUIScaleUtility.adjustedGUILayout = new List<bool>();
			GUIScaleUtility.initiated = true;
		}

		public static Vector2 getCurrentScale
		{
			get
			{
				return new Vector2(1f / GUI.matrix.GetColumn(0).magnitude, 1f / GUI.matrix.GetColumn(1).magnitude);
			}
		}

		public static Vector2 BeginScale(ref Rect rect, Vector2 zoomPivot, float zoom, bool adjustGUILayout)
		{
			Rect rect2;
			if (GUIScaleUtility.compabilityMode)
			{
				GUI.EndGroup();
				rect2 = rect;
			}
			else
			{
				GUIScaleUtility.BeginNoClip();
				rect2 = GUIScaleUtility.GUIToScaledSpace(rect);
			}
			rect = GUIScaleUtility.Scale(rect2, rect2.position + zoomPivot, new Vector2(zoom, zoom));
			GUI.BeginGroup(rect);
			rect.position = Vector2.zero;
			Vector2 vector = rect.center - rect2.size / 2f + zoomPivot;
			GUIScaleUtility.adjustedGUILayout.Add(adjustGUILayout);
			if (adjustGUILayout)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Space(rect.center.x - rect2.size.x + zoomPivot.x);
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				GUILayout.Space(rect.center.y - rect2.size.y + zoomPivot.y);
			}
			GUIScaleUtility.GUIMatrices.Add(GUI.matrix);
			GUIUtility.ScaleAroundPivot(new Vector2(1f / zoom, 1f / zoom), vector);
			return vector;
		}

		public static void EndScale()
		{
			if (GUIScaleUtility.GUIMatrices.Count == 0 || GUIScaleUtility.adjustedGUILayout.Count == 0)
			{
				throw new UnityException("GUIScaleUtility: You are ending more scale regions than you are beginning!");
			}
			GUI.matrix = GUIScaleUtility.GUIMatrices[GUIScaleUtility.GUIMatrices.Count - 1];
			GUIScaleUtility.GUIMatrices.RemoveAt(GUIScaleUtility.GUIMatrices.Count - 1);
			if (GUIScaleUtility.adjustedGUILayout[GUIScaleUtility.adjustedGUILayout.Count - 1])
			{
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
			GUIScaleUtility.adjustedGUILayout.RemoveAt(GUIScaleUtility.adjustedGUILayout.Count - 1);
			GUI.EndGroup();
			if (!GUIScaleUtility.compabilityMode)
			{
				GUIScaleUtility.RestoreClips();
				return;
			}
			if (!Application.isPlaying)
			{
				GUI.BeginClip(new Rect(0f, 23f, (float)Screen.width, (float)(Screen.height - 23)));
				return;
			}
			GUI.BeginClip(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
		}

		public static void BeginNoClip()
		{
			List<Rect> list = new List<Rect>();
			Rect rect = GUIScaleUtility.getTopRect;
			while (rect != new Rect(-10000f, -10000f, 40000f, 40000f))
			{
				list.Add(rect);
				GUI.EndClip();
				rect = GUIScaleUtility.getTopRect;
			}
			list.Reverse();
			GUIScaleUtility.rectStackGroups.Add(list);
			GUIScaleUtility.currentRectStack.AddRange(list);
		}

		public static void MoveClipsUp(int count)
		{
			List<Rect> list = new List<Rect>();
			Rect rect = GUIScaleUtility.getTopRect;
			while (rect != new Rect(-10000f, -10000f, 40000f, 40000f) && count > 0)
			{
				list.Add(rect);
				GUI.EndClip();
				rect = GUIScaleUtility.getTopRect;
				count--;
			}
			list.Reverse();
			GUIScaleUtility.rectStackGroups.Add(list);
			GUIScaleUtility.currentRectStack.AddRange(list);
		}

		public static void RestoreClips()
		{
			if (GUIScaleUtility.rectStackGroups.Count == 0)
			{
				global::Debug.LogError("GUIClipHierarchy: BeginNoClip/MoveClipsUp - RestoreClips count not balanced!");
				return;
			}
			List<Rect> list = GUIScaleUtility.rectStackGroups[GUIScaleUtility.rectStackGroups.Count - 1];
			for (int i = 0; i < list.Count; i++)
			{
				GUI.BeginClip(list[i]);
				GUIScaleUtility.currentRectStack.RemoveAt(GUIScaleUtility.currentRectStack.Count - 1);
			}
			GUIScaleUtility.rectStackGroups.RemoveAt(GUIScaleUtility.rectStackGroups.Count - 1);
		}

		public static void BeginNewLayout()
		{
			if (GUIScaleUtility.compabilityMode)
			{
				return;
			}
			Rect getTopRect = GUIScaleUtility.getTopRect;
			if (getTopRect != new Rect(-10000f, -10000f, 40000f, 40000f))
			{
				GUILayout.BeginArea(new Rect(0f, 0f, getTopRect.width, getTopRect.height));
				return;
			}
			GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
		}

		public static void EndNewLayout()
		{
			if (!GUIScaleUtility.compabilityMode)
			{
				GUILayout.EndArea();
			}
		}

		public static void BeginIgnoreMatrix()
		{
			GUIScaleUtility.GUIMatrices.Add(GUI.matrix);
			GUI.matrix = Matrix4x4.identity;
		}

		public static void EndIgnoreMatrix()
		{
			if (GUIScaleUtility.GUIMatrices.Count == 0)
			{
				throw new UnityException("GUIScaleutility: You are ending more ignoreMatrices than you are beginning!");
			}
			GUI.matrix = GUIScaleUtility.GUIMatrices[GUIScaleUtility.GUIMatrices.Count - 1];
			GUIScaleUtility.GUIMatrices.RemoveAt(GUIScaleUtility.GUIMatrices.Count - 1);
		}

		public static Vector2 Scale(Vector2 pos, Vector2 pivot, Vector2 scale)
		{
			return Vector2.Scale(pos - pivot, scale) + pivot;
		}

		public static Rect Scale(Rect rect, Vector2 pivot, Vector2 scale)
		{
			rect.position = Vector2.Scale(rect.position - pivot, scale) + pivot;
			rect.size = Vector2.Scale(rect.size, scale);
			return rect;
		}

		public static Vector2 ScaledToGUISpace(Vector2 scaledPosition)
		{
			if (GUIScaleUtility.rectStackGroups == null || GUIScaleUtility.rectStackGroups.Count == 0)
			{
				return scaledPosition;
			}
			List<Rect> list = GUIScaleUtility.rectStackGroups[GUIScaleUtility.rectStackGroups.Count - 1];
			for (int i = 0; i < list.Count; i++)
			{
				scaledPosition -= list[i].position;
			}
			return scaledPosition;
		}

		public static Rect ScaledToGUISpace(Rect scaledRect)
		{
			if (GUIScaleUtility.rectStackGroups == null || GUIScaleUtility.rectStackGroups.Count == 0)
			{
				return scaledRect;
			}
			scaledRect.position = GUIScaleUtility.ScaledToGUISpace(scaledRect.position);
			return scaledRect;
		}

		public static Vector2 GUIToScaledSpace(Vector2 guiPosition)
		{
			if (GUIScaleUtility.rectStackGroups == null || GUIScaleUtility.rectStackGroups.Count == 0)
			{
				return guiPosition;
			}
			List<Rect> list = GUIScaleUtility.rectStackGroups[GUIScaleUtility.rectStackGroups.Count - 1];
			for (int i = 0; i < list.Count; i++)
			{
				guiPosition += list[i].position;
			}
			return guiPosition;
		}

		public static Rect GUIToScaledSpace(Rect guiRect)
		{
			if (GUIScaleUtility.rectStackGroups == null || GUIScaleUtility.rectStackGroups.Count == 0)
			{
				return guiRect;
			}
			guiRect.position = GUIScaleUtility.GUIToScaledSpace(guiRect.position);
			return guiRect;
		}

		public static Vector2 GUIToScreenSpace(Vector2 guiPosition)
		{
			return guiPosition + GUIScaleUtility.getTopRectScreenSpace.position;
		}

		public static Rect GUIToScreenSpace(Rect guiRect)
		{
			guiRect.position += GUIScaleUtility.getTopRectScreenSpace.position;
			return guiRect;
		}

		private static bool compabilityMode;

		private static bool initiated;

		private static FieldInfo currentGUILayoutCache;

		private static FieldInfo currentTopLevelGroup;

		private static Func<Rect> GetTopRectDelegate;

		private static Func<Rect> topmostRectDelegate;

		private static List<List<Rect>> rectStackGroups;

		private static List<Matrix4x4> GUIMatrices;

		private static List<bool> adjustedGUILayout;
	}
}
