using System;

namespace UnityEngine.UIElements
{
	internal static class PointerDeviceState
	{
		internal static void Reset()
		{
			for (int i = 0; i < PointerId.maxPointers; i++)
			{
				PointerDeviceState.s_PlayerPointerLocations[i].SetLocation(Vector2.zero, null);
				PointerDeviceState.s_PressedButtons[i] = 0;
				PointerDeviceState.s_PlayerPanelWithSoftPointerCapture[i] = null;
				bool flag = PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[i] != null;
				if (flag)
				{
					PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[i].softPointerCaptures = 0;
					PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[i] = null;
				}
			}
			for (int j = 0; j < PointerId.maxPointers; j++)
			{
				PointerDeviceState.RuntimePointerState runtimePointerState = PointerDeviceState.s_RuntimePointerStates[j];
				if (runtimePointerState != null)
				{
					runtimePointerState.Reset();
				}
			}
		}

		internal static void RemovePanelData(IPanel panel)
		{
			for (int i = 0; i < PointerId.maxPointers; i++)
			{
				bool flag = PointerDeviceState.s_PlayerPointerLocations[i].Panel == panel;
				if (flag)
				{
					PointerDeviceState.s_PlayerPointerLocations[i].SetLocation(Vector2.zero, null);
				}
				bool flag2 = PointerDeviceState.s_PlayerPanelWithSoftPointerCapture[i] == panel;
				if (flag2)
				{
					PointerDeviceState.s_PlayerPanelWithSoftPointerCapture[i] = null;
					bool flag3 = PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[i] != null;
					if (flag3)
					{
						PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[i].softPointerCaptures = 0;
						PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[i] = null;
					}
				}
			}
		}

		internal static void RemoveDocumentData(UIDocument document)
		{
			bool flag = document.softPointerCaptures == 0;
			if (!flag)
			{
				for (int i = 0; i < PointerId.maxPointers; i++)
				{
					bool flag2 = PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[i] == document;
					if (flag2)
					{
						PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[i].softPointerCaptures = 0;
						PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[i] = null;
					}
				}
			}
		}

		public static void SavePointerPosition(int pointerId, Vector3 position, IPanel panel, ContextType contextType)
		{
			if (contextType > ContextType.Editor)
			{
			}
			PointerDeviceState.s_PlayerPointerLocations[pointerId].SetLocation(position, panel);
		}

		public static void PressButton(int pointerId, int buttonId)
		{
			Debug.Assert(buttonId >= 0, "PressButton expects buttonId >= 0");
			Debug.Assert(buttonId < 32, "PressButton expects buttonId < 32");
			PointerDeviceState.s_PressedButtons[pointerId] |= 1 << buttonId;
		}

		public static void ReleaseButton(int pointerId, int buttonId)
		{
			Debug.Assert(buttonId >= 0, "ReleaseButton expects buttonId >= 0");
			Debug.Assert(buttonId < 32, "ReleaseButton expects buttonId < 32");
			PointerDeviceState.s_PressedButtons[pointerId] &= ~(1 << buttonId);
		}

		public static void ReleaseAllButtons(int pointerId)
		{
			PointerDeviceState.s_PressedButtons[pointerId] = 0;
		}

		public static Vector3 GetPointerPosition(int pointerId, ContextType contextType)
		{
			if (contextType > ContextType.Editor)
			{
			}
			return PointerDeviceState.s_PlayerPointerLocations[pointerId].Position;
		}

		public static Vector3 GetPointerDeltaPosition(int pointerId, ContextType contextType, Vector3 newPosition)
		{
			if (contextType > ContextType.Editor)
			{
			}
			bool flag = PointerDeviceState.s_PlayerPointerLocations[pointerId].Panel == null;
			Vector3 vector;
			if (flag)
			{
				vector = Vector3.zero;
			}
			else
			{
				vector = newPosition - PointerDeviceState.s_PlayerPointerLocations[pointerId].Position;
			}
			return vector;
		}

		public static IPanel GetPanel(int pointerId, ContextType contextType)
		{
			if (contextType > ContextType.Editor)
			{
			}
			return PointerDeviceState.s_PlayerPointerLocations[pointerId].Panel;
		}

		private static bool HasFlagFast(PointerDeviceState.LocationFlag flagSet, PointerDeviceState.LocationFlag flag)
		{
			return (flagSet & flag) == flag;
		}

		public static bool HasLocationFlag(int pointerId, ContextType contextType, PointerDeviceState.LocationFlag flag)
		{
			if (contextType > ContextType.Editor)
			{
			}
			return PointerDeviceState.HasFlagFast(PointerDeviceState.s_PlayerPointerLocations[pointerId].Flags, flag);
		}

		public static int GetPressedButtons(int pointerId)
		{
			return PointerDeviceState.s_PressedButtons[pointerId];
		}

		internal static bool HasAdditionalPressedButtons(int pointerId, int exceptButtonId)
		{
			return (PointerDeviceState.s_PressedButtons[pointerId] & ~(1 << exceptButtonId)) != 0;
		}

		internal static RuntimePanel GetPlayerPanelWithSoftPointerCapture(int pointerId)
		{
			return PointerDeviceState.s_PlayerPanelWithSoftPointerCapture[pointerId];
		}

		internal static UIDocument GetWorldSpaceDocumentWithSoftPointerCapture(int pointerId)
		{
			return PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[pointerId];
		}

		internal static Camera GetCameraWithSoftPointerCapture(int pointerId)
		{
			return PointerDeviceState.s_CameraWithSoftPointerCapture[pointerId];
		}

		internal static void SetElementWithSoftPointerCapture(int pointerId, VisualElement element, Camera camera)
		{
			RuntimePanel runtimePanel = ((element != null) ? element.elementPanel : null) as RuntimePanel;
			PointerDeviceState.s_PlayerPanelWithSoftPointerCapture[pointerId] = runtimePanel;
			PointerDeviceState.s_CameraWithSoftPointerCapture[pointerId] = camera;
			ref UIDocument ptr = ref PointerDeviceState.s_WorldSpaceDocumentWithSoftPointerCapture[pointerId];
			bool flag = ptr != null;
			if (flag)
			{
				ptr.softPointerCaptures &= ~(1 << pointerId);
			}
			ptr = ((runtimePanel != null && runtimePanel.drawsInCameras) ? UIDocument.FindRootUIDocument(element) : null);
			bool flag2 = ptr != null;
			if (flag2)
			{
				ptr.softPointerCaptures |= 1 << pointerId;
			}
		}

		internal static PointerDeviceState.TrackedPointerState GetTrackedState(int pointerId, bool createIfNull = false)
		{
			int num = pointerId - PointerId.trackedPointerIdBase;
			bool flag = num < 0 || num >= PointerId.trackedPointerCount;
			PointerDeviceState.TrackedPointerState trackedPointerState;
			if (flag)
			{
				trackedPointerState = null;
			}
			else
			{
				if (createIfNull)
				{
					PointerDeviceState.RuntimePointerState[] array = PointerDeviceState.s_RuntimePointerStates;
					if (array[pointerId] == null)
					{
						array[pointerId] = new PointerDeviceState.TrackedPointerState();
					}
				}
				trackedPointerState = (PointerDeviceState.TrackedPointerState)PointerDeviceState.s_RuntimePointerStates[pointerId];
			}
			return trackedPointerState;
		}

		internal static void RemoveTrackedState(int pointerId)
		{
			int num = pointerId - PointerId.trackedPointerIdBase;
			bool flag = num < 0 || num >= PointerId.trackedPointerCount;
			if (!flag)
			{
				PointerDeviceState.s_RuntimePointerStates[pointerId] = null;
			}
		}

		internal static PointerDeviceState.ScreenPointerState GetScreenPointerState(int pointerId, bool createIfNull = false)
		{
			int num = pointerId - PointerId.trackedPointerIdBase;
			bool flag = num >= 0 && num < PointerId.trackedPointerCount;
			PointerDeviceState.ScreenPointerState screenPointerState;
			if (flag)
			{
				screenPointerState = null;
			}
			else
			{
				if (createIfNull)
				{
					PointerDeviceState.RuntimePointerState[] array = PointerDeviceState.s_RuntimePointerStates;
					if (array[pointerId] == null)
					{
						array[pointerId] = new PointerDeviceState.ScreenPointerState();
					}
				}
				screenPointerState = (PointerDeviceState.ScreenPointerState)PointerDeviceState.s_RuntimePointerStates[pointerId];
			}
			return screenPointerState;
		}

		private static PointerDeviceState.RuntimePointerState[] s_RuntimePointerStates = new PointerDeviceState.RuntimePointerState[PointerId.maxPointers];

		private static PointerDeviceState.PointerLocation[] s_PlayerPointerLocations = new PointerDeviceState.PointerLocation[PointerId.maxPointers];

		private static int[] s_PressedButtons = new int[PointerId.maxPointers];

		private static readonly RuntimePanel[] s_PlayerPanelWithSoftPointerCapture = new RuntimePanel[PointerId.maxPointers];

		private static readonly UIDocument[] s_WorldSpaceDocumentWithSoftPointerCapture = new UIDocument[PointerId.maxPointers];

		private static readonly Camera[] s_CameraWithSoftPointerCapture = new Camera[PointerId.maxPointers];

		[Flags]
		internal enum LocationFlag
		{
			None = 0,
			OutsidePanel = 1
		}

		private struct PointerLocation
		{
			internal Vector3 Position { readonly get; private set; }

			internal IPanel Panel { readonly get; private set; }

			internal PointerDeviceState.LocationFlag Flags { readonly get; private set; }

			internal void SetLocation(Vector3 position, IPanel panel)
			{
				this.Position = position;
				this.Panel = panel;
				this.Flags = PointerDeviceState.LocationFlag.None;
				bool flag;
				if (panel != null)
				{
					BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
					flag = baseVisualElementPanel != null && baseVisualElementPanel.isFlat && !panel.visualTree.layout.Contains(position);
				}
				else
				{
					flag = true;
				}
				bool flag2 = flag;
				if (flag2)
				{
					this.Flags |= PointerDeviceState.LocationFlag.OutsidePanel;
				}
			}
		}

		public class RuntimePointerState
		{
			public virtual void Reset()
			{
				this.hit = default(PointerDeviceState.RuntimePointerState.RaycastHit);
				this.updateFrameCount = 0;
			}

			public PointerDeviceState.RuntimePointerState.RaycastHit hit;

			public int updateFrameCount = 0;

			public struct RaycastHit
			{
				public float distance;

				public Collider collider;

				public UIDocument document;

				public VisualElement element;
			}
		}

		public class ScreenPointerState : PointerDeviceState.RuntimePointerState
		{
			public override void Reset()
			{
				base.Reset();
				this.mousePosition = Vector2.zero;
				this.targetDisplay = null;
			}

			public Vector2 mousePosition;

			public int? targetDisplay;
		}

		public class TrackedPointerState : PointerDeviceState.RuntimePointerState
		{
			public Ray worldRay
			{
				get
				{
					return new Ray(this.worldPosition, this.worldOrientation * Vector3.forward);
				}
			}

			public override void Reset()
			{
				base.Reset();
				this.worldPosition = Vector3.zero;
				this.worldOrientation = Quaternion.identity;
				this.maxDistance = float.PositiveInfinity;
			}

			public Vector3 worldPosition = Vector3.zero;

			public Quaternion worldOrientation = Quaternion.identity;

			public float maxDistance = float.PositiveInfinity;
		}
	}
}
