using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEngine.EventSystems
{
	public class PointerEventData : BaseEventData
	{
		public GameObject pointerEnter { get; set; }

		public GameObject lastPress { get; private set; }

		public GameObject rawPointerPress { get; set; }

		public GameObject pointerDrag { get; set; }

		public GameObject pointerClick { get; set; }

		public RaycastResult pointerCurrentRaycast { get; set; }

		public RaycastResult pointerPressRaycast { get; set; }

		public bool eligibleForClick { get; set; }

		public int displayIndex { get; set; }

		public int pointerId { get; set; }

		public Vector2 position { get; set; }

		public Vector2 delta { get; set; }

		public Vector2 pressPosition { get; set; }

		[Obsolete("Use either pointerCurrentRaycast.worldPosition or pointerPressRaycast.worldPosition")]
		public Vector3 worldPosition { get; set; }

		[Obsolete("Use either pointerCurrentRaycast.worldNormal or pointerPressRaycast.worldNormal")]
		public Vector3 worldNormal { get; set; }

		public float clickTime { get; set; }

		public int clickCount { get; set; }

		public Vector2 scrollDelta { get; set; }

		public bool useDragThreshold { get; set; }

		public bool dragging { get; set; }

		public PointerEventData.InputButton button { get; set; }

		public float pressure { get; set; }

		public float tangentialPressure { get; set; }

		public float altitudeAngle { get; set; }

		public float azimuthAngle { get; set; }

		public float twist { get; set; }

		public Vector2 tilt { get; set; }

		public PenStatus penStatus { get; set; }

		public Vector2 radius { get; set; }

		public Vector2 radiusVariance { get; set; }

		public bool fullyExited { get; set; }

		public bool reentered { get; set; }

		public PointerEventData(EventSystem eventSystem)
			: base(eventSystem)
		{
			this.eligibleForClick = false;
			this.displayIndex = 0;
			this.pointerId = -1;
			this.position = Vector2.zero;
			this.delta = Vector2.zero;
			this.pressPosition = Vector2.zero;
			this.clickTime = 0f;
			this.clickCount = 0;
			this.scrollDelta = Vector2.zero;
			this.useDragThreshold = true;
			this.dragging = false;
			this.button = PointerEventData.InputButton.Left;
			this.pressure = 0f;
			this.tangentialPressure = 0f;
			this.altitudeAngle = 0f;
			this.azimuthAngle = 0f;
			this.twist = 0f;
			this.tilt = new Vector2(0f, 0f);
			this.penStatus = PenStatus.None;
			this.radius = Vector2.zero;
			this.radiusVariance = Vector2.zero;
		}

		public bool IsPointerMoving()
		{
			return this.delta.sqrMagnitude > 0f;
		}

		public bool IsScrolling()
		{
			return this.scrollDelta.sqrMagnitude > 0f;
		}

		public Camera enterEventCamera
		{
			get
			{
				if (!(this.pointerCurrentRaycast.module == null))
				{
					return this.pointerCurrentRaycast.module.eventCamera;
				}
				return null;
			}
		}

		public Camera pressEventCamera
		{
			get
			{
				if (!(this.pointerPressRaycast.module == null))
				{
					return this.pointerPressRaycast.module.eventCamera;
				}
				return null;
			}
		}

		public GameObject pointerPress
		{
			get
			{
				return this.m_PointerPress;
			}
			set
			{
				if (this.m_PointerPress == value)
				{
					return;
				}
				this.lastPress = this.m_PointerPress;
				this.m_PointerPress = value;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<b>Position</b>: " + this.position.ToString());
			stringBuilder.AppendLine("<b>delta</b>: " + this.delta.ToString());
			stringBuilder.AppendLine("<b>eligibleForClick</b>: " + this.eligibleForClick.ToString());
			string text = "<b>pointerEnter</b>: ";
			GameObject pointerEnter = this.pointerEnter;
			stringBuilder.AppendLine(text + ((pointerEnter != null) ? pointerEnter.ToString() : null));
			string text2 = "<b>pointerPress</b>: ";
			GameObject pointerPress = this.pointerPress;
			stringBuilder.AppendLine(text2 + ((pointerPress != null) ? pointerPress.ToString() : null));
			string text3 = "<b>lastPointerPress</b>: ";
			GameObject lastPress = this.lastPress;
			stringBuilder.AppendLine(text3 + ((lastPress != null) ? lastPress.ToString() : null));
			string text4 = "<b>pointerDrag</b>: ";
			GameObject pointerDrag = this.pointerDrag;
			stringBuilder.AppendLine(text4 + ((pointerDrag != null) ? pointerDrag.ToString() : null));
			stringBuilder.AppendLine("<b>Use Drag Threshold</b>: " + this.useDragThreshold.ToString());
			stringBuilder.AppendLine("<b>Current Raycast:</b>");
			stringBuilder.AppendLine(this.pointerCurrentRaycast.ToString());
			stringBuilder.AppendLine("<b>Press Raycast:</b>");
			stringBuilder.AppendLine(this.pointerPressRaycast.ToString());
			stringBuilder.AppendLine("<b>Display Index:</b>");
			stringBuilder.AppendLine(this.displayIndex.ToString());
			stringBuilder.AppendLine("<b>pressure</b>: " + this.pressure.ToString());
			stringBuilder.AppendLine("<b>tangentialPressure</b>: " + this.tangentialPressure.ToString());
			stringBuilder.AppendLine("<b>altitudeAngle</b>: " + this.altitudeAngle.ToString());
			stringBuilder.AppendLine("<b>azimuthAngle</b>: " + this.azimuthAngle.ToString());
			stringBuilder.AppendLine("<b>twist</b>: " + this.twist.ToString());
			stringBuilder.AppendLine("<b>tilt</b>: " + this.tilt.ToString());
			stringBuilder.AppendLine("<b>penStatus</b>: " + this.penStatus.ToString());
			stringBuilder.AppendLine("<b>radius</b>: " + this.radius.ToString());
			stringBuilder.AppendLine("<b>radiusVariance</b>: " + this.radiusVariance.ToString());
			return stringBuilder.ToString();
		}

		private GameObject m_PointerPress;

		public List<GameObject> hovered = new List<GameObject>();

		public enum InputButton
		{
			Left,
			Right,
			Middle
		}

		public enum FramePressState
		{
			Pressed,
			Released,
			PressedAndReleased,
			NotChanged
		}
	}
}
