using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.StyleSheets;
using UnityEngine.Yoga;

namespace UnityEngine.Experimental.UIElements
{
	public class VisualElement : Focusable, ITransform, IUIElementDataWatch, IEnumerable<VisualElement>, IVisualElementScheduler, IStyle, IEnumerable
	{
		public VisualElement()
		{
			this.controlid = (VisualElement.s_NextId += 1U);
			this.shadow = new VisualElement.Hierarchy(this);
			this.m_ClassList = VisualElement.s_EmptyClassList;
			this.m_FullTypeName = string.Empty;
			this.m_TypeName = string.Empty;
			this.SetEnabled(true);
			this.focusIndex = VisualElement.defaultFocusIndex;
			this.name = string.Empty;
			this.yogaNode = new YogaNode(null);
		}

		public string persistenceKey
		{
			get
			{
				return this.m_PersistenceKey;
			}
			set
			{
				if (this.m_PersistenceKey != value)
				{
					this.m_PersistenceKey = value;
					if (!string.IsNullOrEmpty(value))
					{
						this.IncrementVersion(VersionChangeType.PersistentData);
					}
				}
			}
		}

		internal bool enablePersistence { get; private set; }

		public object userData { get; set; }

		public override bool canGrabFocus
		{
			get
			{
				return this.visible && this.enabledInHierarchy && base.canGrabFocus;
			}
		}

		public override FocusController focusController
		{
			get
			{
				return (this.panel != null) ? this.panel.focusController : null;
			}
		}

		internal RenderData renderData
		{
			get
			{
				RenderData renderData;
				if ((renderData = this.m_RenderData) == null)
				{
					renderData = (this.m_RenderData = new RenderData());
				}
				return renderData;
			}
		}

		public ITransform transform
		{
			get
			{
				return this;
			}
		}

		Vector3 ITransform.position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				if (!(this.m_Position == value))
				{
					this.m_Position = value;
					this.IncrementVersion(VersionChangeType.Transform);
				}
			}
		}

		Quaternion ITransform.rotation
		{
			get
			{
				return this.m_Rotation;
			}
			set
			{
				if (!(this.m_Rotation == value))
				{
					this.m_Rotation = value;
					this.IncrementVersion(VersionChangeType.Transform);
				}
			}
		}

		Vector3 ITransform.scale
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				if (!(this.m_Scale == value))
				{
					this.m_Scale = value;
					this.IncrementVersion(VersionChangeType.Transform);
					this.IncrementVersion(VersionChangeType.Layout);
				}
			}
		}

		internal Vector3 ComputeGlobalScale()
		{
			Vector3 scale = this.m_Scale;
			for (VisualElement visualElement = this.shadow.parent; visualElement != null; visualElement = visualElement.shadow.parent)
			{
				scale.Scale(visualElement.m_Scale);
			}
			return scale;
		}

		Matrix4x4 ITransform.matrix
		{
			get
			{
				return Matrix4x4.TRS(this.m_Position, this.m_Rotation, this.m_Scale);
			}
		}

		public Rect layout
		{
			get
			{
				Rect layout = this.m_Layout;
				if (this.yogaNode != null && this.style.positionType.value != PositionType.Manual)
				{
					layout.x = this.yogaNode.LayoutX;
					layout.y = this.yogaNode.LayoutY;
					layout.width = this.yogaNode.LayoutWidth;
					layout.height = this.yogaNode.LayoutHeight;
				}
				return layout;
			}
			set
			{
				if (this.yogaNode == null)
				{
					this.yogaNode = new YogaNode(null);
				}
				if (this.style.positionType.value != PositionType.Manual || !(this.m_Layout == value))
				{
					this.m_Layout = value;
					((IStyle)this).positionType = PositionType.Manual;
					((IStyle)this).marginLeft = 0f;
					((IStyle)this).marginRight = 0f;
					((IStyle)this).marginBottom = 0f;
					((IStyle)this).marginTop = 0f;
					((IStyle)this).positionLeft = value.x;
					((IStyle)this).positionTop = value.y;
					((IStyle)this).positionRight = float.NaN;
					((IStyle)this).positionBottom = float.NaN;
					((IStyle)this).width = value.width;
					((IStyle)this).height = value.height;
					this.IncrementVersion(VersionChangeType.Transform);
				}
			}
		}

		public Rect contentRect
		{
			get
			{
				Spacing spacing = new Spacing(this.m_Style.paddingLeft, this.m_Style.paddingTop, this.m_Style.paddingRight, this.m_Style.paddingBottom);
				return this.paddingRect - spacing;
			}
		}

		protected Rect paddingRect
		{
			get
			{
				Spacing spacing = new Spacing(this.style.borderLeftWidth, this.style.borderTopWidth, this.style.borderRightWidth, this.style.borderBottomWidth);
				return this.rect - spacing;
			}
		}

		public Rect worldBound
		{
			get
			{
				Matrix4x4 worldTransform = this.worldTransform;
				Vector3 vector = GUIUtility.Internal_MultiplyPoint(new Vector3(this.rect.min.x, this.rect.min.y, 1f), worldTransform);
				Vector3 vector2 = GUIUtility.Internal_MultiplyPoint(new Vector3(this.rect.max.x, this.rect.max.y, 1f), worldTransform);
				return Rect.MinMaxRect(Math.Min(vector.x, vector2.x), Math.Min(vector.y, vector2.y), Math.Max(vector.x, vector2.x), Math.Max(vector.y, vector2.y));
			}
		}

		public Rect localBound
		{
			get
			{
				Matrix4x4 matrix = this.transform.matrix;
				Vector3 vector = GUIUtility.Internal_MultiplyPoint(this.layout.min, matrix);
				Vector3 vector2 = GUIUtility.Internal_MultiplyPoint(this.layout.max, matrix);
				return Rect.MinMaxRect(Math.Min(vector.x, vector2.x), Math.Min(vector.y, vector2.y), Math.Max(vector.x, vector2.x), Math.Max(vector.y, vector2.y));
			}
		}

		internal Rect rect
		{
			get
			{
				return new Rect(0f, 0f, this.layout.width, this.layout.height);
			}
		}

		internal bool isWorldTransformDirty { get; set; } = true;

		public Matrix4x4 worldTransform
		{
			get
			{
				if (this.isWorldTransformDirty)
				{
					this.UpdateWorldTransform();
					this.isWorldTransformDirty = false;
				}
				return this.m_WorldTransform;
			}
		}

		private void UpdateWorldTransform()
		{
			Matrix4x4 matrix4x = Matrix4x4.Translate(new Vector3(this.layout.x, this.layout.y, 0f));
			if (this.shadow.parent != null)
			{
				this.m_WorldTransform = this.shadow.parent.worldTransform * matrix4x * this.transform.matrix;
			}
			else
			{
				this.m_WorldTransform = matrix4x * this.transform.matrix;
			}
		}

		internal bool isWorldClipDirty { get; set; } = true;

		internal Rect worldClip
		{
			get
			{
				if (this.isWorldClipDirty)
				{
					this.UpdateWorldClip();
					this.isWorldClipDirty = false;
				}
				return this.m_WorldClip;
			}
		}

		private void UpdateWorldClip()
		{
			if (this.shadow.parent != null)
			{
				this.m_WorldClip = this.shadow.parent.worldClip;
				if (this.ShouldClip())
				{
					Rect rect = VisualElement.ComputeAAAlignedBound(this.rect, this.worldTransform);
					float num = Mathf.Max(rect.x, this.m_WorldClip.x);
					float num2 = Mathf.Min(rect.x + rect.width, this.m_WorldClip.x + this.m_WorldClip.width);
					float num3 = Mathf.Max(rect.y, this.m_WorldClip.y);
					float num4 = Mathf.Min(rect.y + rect.height, this.m_WorldClip.y + this.m_WorldClip.height);
					this.m_WorldClip = new Rect(num, num3, num2 - num, num4 - num3);
				}
			}
			else
			{
				this.m_WorldClip = ((this.panel == null) ? GUIClip.topmostRect : this.panel.visualTree.rect);
			}
		}

		internal static Rect ComputeAAAlignedBound(Rect position, Matrix4x4 mat)
		{
			Rect rect = position;
			Vector3 vector = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y, 0f));
			Vector3 vector2 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y, 0f));
			Vector3 vector3 = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y + rect.height, 0f));
			Vector3 vector4 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y + rect.height, 0f));
			return Rect.MinMaxRect(Mathf.Min(vector.x, Mathf.Min(vector2.x, Mathf.Min(vector3.x, vector4.x))), Mathf.Min(vector.y, Mathf.Min(vector2.y, Mathf.Min(vector3.y, vector4.y))), Mathf.Max(vector.x, Mathf.Max(vector2.x, Mathf.Max(vector3.x, vector4.x))), Mathf.Max(vector.y, Mathf.Max(vector2.y, Mathf.Max(vector3.y, vector4.y))));
		}

		internal PseudoStates pseudoStates
		{
			get
			{
				return this.m_PseudoStates;
			}
			set
			{
				if (this.m_PseudoStates != value)
				{
					this.m_PseudoStates = value;
					if ((this.triggerPseudoMask & this.m_PseudoStates) != (PseudoStates)0 || (this.dependencyPseudoMask & ~(this.m_PseudoStates != (PseudoStates)0)) != (PseudoStates)0)
					{
						this.IncrementVersion(VersionChangeType.StyleSheet);
					}
				}
			}
		}

		public PickingMode pickingMode { get; set; }

		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				if (!(this.m_Name == value))
				{
					this.m_Name = value;
					this.IncrementVersion(VersionChangeType.StyleSheet);
				}
			}
		}

		internal List<string> classList
		{
			get
			{
				return this.m_ClassList;
			}
		}

		internal string fullTypeName
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_FullTypeName))
				{
					this.m_FullTypeName = base.GetType().FullName;
				}
				return this.m_FullTypeName;
			}
		}

		internal string typeName
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_TypeName))
				{
					Type type = base.GetType();
					bool isGenericType = type.IsGenericType;
					this.m_TypeName = ((!isGenericType) ? type.Name : type.Name.Remove(type.Name.IndexOf('`')));
				}
				return this.m_TypeName;
			}
		}

		internal YogaNode yogaNode { get; private set; }

		protected virtual void OnStyleResolved(ICustomStyle style)
		{
			this.FinalizeLayout();
		}

		internal VisualElementStylesData sharedStyle
		{
			get
			{
				return this.m_SharedStyle;
			}
		}

		internal VisualElementStylesData effectiveStyle
		{
			get
			{
				return this.m_Style;
			}
		}

		internal bool hasInlineStyle
		{
			get
			{
				return this.m_Style != this.m_SharedStyle;
			}
		}

		private VisualElementStylesData inlineStyle
		{
			get
			{
				if (!this.hasInlineStyle)
				{
					VisualElementStylesData visualElementStylesData = new VisualElementStylesData(false);
					visualElementStylesData.Apply(this.m_SharedStyle, StylePropertyApplyMode.Copy);
					this.m_Style = visualElementStylesData;
				}
				return this.m_Style;
			}
		}

		internal float opacity
		{
			get
			{
				return this.style.opacity.value;
			}
			set
			{
				this.style.opacity = value;
			}
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			if (evt.GetEventTypeId() == EventBase<MouseOverEvent>.TypeId() || evt.GetEventTypeId() == EventBase<MouseOutEvent>.TypeId())
			{
				this.UpdateCursorStyle(evt.GetEventTypeId());
			}
			else if (evt.GetEventTypeId() == EventBase<MouseEnterEvent>.TypeId())
			{
				this.pseudoStates |= PseudoStates.Hover;
			}
			else if (evt.GetEventTypeId() == EventBase<MouseLeaveEvent>.TypeId())
			{
				this.pseudoStates &= ~PseudoStates.Hover;
			}
			else if (evt.GetEventTypeId() == EventBase<BlurEvent>.TypeId())
			{
				this.pseudoStates &= ~PseudoStates.Focus;
			}
			else if (evt.GetEventTypeId() == EventBase<FocusEvent>.TypeId())
			{
				this.pseudoStates |= PseudoStates.Focus;
			}
		}

		public sealed override void Focus()
		{
			if (!this.canGrabFocus && this.shadow.parent != null)
			{
				this.shadow.parent.Focus();
			}
			else
			{
				base.Focus();
			}
		}

		internal void SetPanel(BaseVisualElementPanel p)
		{
			if (this.panel != p)
			{
				List<VisualElement> list = VisualElementListPool.Get(0);
				try
				{
					list.Add(this);
					this.GatherAllChildren(list);
					EventDispatcher.Gate? gate = null;
					if (((p != null) ? p.dispatcher : null) != null)
					{
						gate = new EventDispatcher.Gate?(new EventDispatcher.Gate(p.dispatcher));
					}
					EventDispatcher.Gate? gate2 = null;
					IPanel panel = this.panel;
					if (((panel != null) ? panel.dispatcher : null) != null && this.panel.dispatcher != ((p != null) ? p.dispatcher : null))
					{
						gate2 = new EventDispatcher.Gate?(new EventDispatcher.Gate(this.panel.dispatcher));
					}
					using (gate)
					{
						using (gate2)
						{
							foreach (VisualElement visualElement in list)
							{
								visualElement.ChangePanel(p);
							}
						}
					}
				}
				finally
				{
					VisualElementListPool.Release(list);
				}
			}
		}

		private void ChangePanel(BaseVisualElementPanel p)
		{
			if (this.panel != p)
			{
				if (this.panel != null)
				{
					using (DetachFromPanelEvent pooled = PanelChangedEventBase<DetachFromPanelEvent>.GetPooled(this.panel, p))
					{
						pooled.target = this;
						this.elementPanel.SendEvent(pooled, DispatchMode.Immediate);
					}
				}
				IPanel panel = this.panel;
				this.elementPanel = p;
				if (this.panel != null)
				{
					using (AttachToPanelEvent pooled2 = PanelChangedEventBase<AttachToPanelEvent>.GetPooled(panel, p))
					{
						pooled2.target = this;
						this.elementPanel.SendEvent(pooled2, DispatchMode.Default);
					}
				}
				this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Transform);
				if (!string.IsNullOrEmpty(this.persistenceKey))
				{
					this.IncrementVersion(VersionChangeType.PersistentData);
				}
			}
		}

		public sealed override void SendEvent(EventBase e)
		{
			BaseVisualElementPanel elementPanel = this.elementPanel;
			if (elementPanel != null)
			{
				elementPanel.SendEvent(e, DispatchMode.Default);
			}
		}

		internal void IncrementVersion(VersionChangeType changeType)
		{
			BaseVisualElementPanel elementPanel = this.elementPanel;
			if (elementPanel != null)
			{
				elementPanel.OnVersionChanged(this, changeType);
			}
		}

		private void IncrementVersion(ChangeType changeType)
		{
			this.IncrementVersion(this.GetVersionChange(changeType));
		}

		private VersionChangeType GetVersionChange(ChangeType type)
		{
			VersionChangeType versionChangeType = (VersionChangeType)0;
			if ((type & (ChangeType.PersistentData | ChangeType.PersistentDataPath)) > (ChangeType)0)
			{
				versionChangeType |= VersionChangeType.PersistentData;
			}
			if ((type & ChangeType.Layout) == ChangeType.Layout)
			{
				versionChangeType |= VersionChangeType.Layout;
			}
			if ((type & (ChangeType.Styles | ChangeType.StylesPath)) > (ChangeType)0)
			{
				versionChangeType |= VersionChangeType.StyleSheet;
			}
			if ((type & ChangeType.Transform) == ChangeType.Transform)
			{
				versionChangeType |= VersionChangeType.Transform;
			}
			if ((type & ChangeType.Repaint) == ChangeType.Repaint)
			{
				versionChangeType |= VersionChangeType.Repaint;
			}
			return versionChangeType;
		}

		[Obsolete("Dirty is deprecated. Use MarkDirtyRepaint to trigger a new repaint of the VisualElement.")]
		public void Dirty(ChangeType type)
		{
			this.IncrementVersion(type);
		}

		[Obsolete("IsDirty is deprecated. Avoid using it, will always return false.")]
		public bool IsDirty(ChangeType type)
		{
			return false;
		}

		[Obsolete("AnyDirty is deprecated. Avoid using it, will always return false.")]
		public bool AnyDirty(ChangeType type)
		{
			return false;
		}

		[Obsolete("ClearDirty is deprecated. Avoid using it, it's now a no-op.")]
		public void ClearDirty(ChangeType type)
		{
		}

		[Obsolete("enabled is deprecated. Use SetEnabled as setter, and enabledSelf/enabledInHierarchy as getters.")]
		public virtual bool enabled
		{
			get
			{
				return this.enabledInHierarchy;
			}
			set
			{
				this.SetEnabled(value);
			}
		}

		protected internal bool SetEnabledFromHierarchy(bool state)
		{
			bool flag;
			if (state == ((this.pseudoStates & PseudoStates.Disabled) != PseudoStates.Disabled))
			{
				flag = false;
			}
			else
			{
				if (state && this.m_Enabled && (this.parent == null || this.parent.enabledInHierarchy))
				{
					this.pseudoStates &= ~PseudoStates.Disabled;
				}
				else
				{
					this.pseudoStates |= PseudoStates.Disabled;
				}
				flag = true;
			}
			return flag;
		}

		public bool enabledInHierarchy
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Disabled) != PseudoStates.Disabled;
			}
		}

		public bool enabledSelf
		{
			get
			{
				return this.m_Enabled;
			}
		}

		public void SetEnabled(bool value)
		{
			if (this.m_Enabled != value)
			{
				this.m_Enabled = value;
				this.PropagateEnabledToChildren(value);
			}
		}

		private void PropagateEnabledToChildren(bool value)
		{
			if (this.SetEnabledFromHierarchy(value))
			{
				for (int i = 0; i < this.shadow.childCount; i++)
				{
					this.shadow[i].PropagateEnabledToChildren(value);
				}
			}
		}

		public bool visible
		{
			get
			{
				return this.style.visibility.GetSpecifiedValueOrDefault(Visibility.Visible) == Visibility.Visible;
			}
			set
			{
				this.style.visibility = ((!value) ? Visibility.Hidden : Visibility.Visible);
			}
		}

		public void MarkDirtyRepaint()
		{
			this.IncrementVersion(VersionChangeType.Repaint);
		}

		internal void Repaint(IStylePainter painter)
		{
			if (this.visible)
			{
				IStylePainterInternal stylePainterInternal = (IStylePainterInternal)painter;
				stylePainterInternal.DrawBackground();
				this.DoRepaint(stylePainterInternal);
				stylePainterInternal.DrawBorder();
			}
		}

		protected virtual void DoRepaint(IStylePainter painter)
		{
		}

		private void GetFullHierarchicalPersistenceKey(StringBuilder key)
		{
			if (this.parent != null)
			{
				this.parent.GetFullHierarchicalPersistenceKey(key);
			}
			if (!string.IsNullOrEmpty(this.persistenceKey))
			{
				key.Append("__");
				key.Append(this.persistenceKey);
			}
		}

		public string GetFullHierarchicalPersistenceKey()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.GetFullHierarchicalPersistenceKey(stringBuilder);
			return stringBuilder.ToString();
		}

		public T GetOrCreatePersistentData<T>(object existing, string key) where T : class, new()
		{
			Debug.Assert(this.elementPanel != null, "VisualElement.elementPanel is null! Cannot load persistent data.");
			ISerializableJsonDictionary serializableJsonDictionary = ((this.elementPanel != null && this.elementPanel.getViewDataDictionary != null) ? this.elementPanel.getViewDataDictionary() : null);
			T t;
			if (serializableJsonDictionary == null || string.IsNullOrEmpty(this.persistenceKey) || !this.enablePersistence)
			{
				if (existing != null)
				{
					t = existing as T;
				}
				else
				{
					t = new T();
				}
			}
			else
			{
				string text = key + "__" + typeof(T).ToString();
				if (!serializableJsonDictionary.ContainsKey(text))
				{
					serializableJsonDictionary.Set<T>(text, new T());
				}
				t = serializableJsonDictionary.Get<T>(text);
			}
			return t;
		}

		public T GetOrCreatePersistentData<T>(ScriptableObject existing, string key) where T : ScriptableObject
		{
			Debug.Assert(this.elementPanel != null, "VisualElement.elementPanel is null! Cannot load persistent data.");
			ISerializableJsonDictionary serializableJsonDictionary = ((this.elementPanel != null && this.elementPanel.getViewDataDictionary != null) ? this.elementPanel.getViewDataDictionary() : null);
			T t;
			if (serializableJsonDictionary == null || string.IsNullOrEmpty(this.persistenceKey) || !this.enablePersistence)
			{
				if (existing != null)
				{
					t = existing as T;
				}
				else
				{
					t = ScriptableObject.CreateInstance<T>();
				}
			}
			else
			{
				string text = key + "__" + typeof(T).ToString();
				if (!serializableJsonDictionary.ContainsKey(text))
				{
					serializableJsonDictionary.Set<T>(text, ScriptableObject.CreateInstance<T>());
				}
				t = serializableJsonDictionary.GetScriptable<T>(text);
			}
			return t;
		}

		public void OverwriteFromPersistedData(object obj, string key)
		{
			Debug.Assert(this.elementPanel != null, "VisualElement.elementPanel is null! Cannot load persistent data.");
			ISerializableJsonDictionary serializableJsonDictionary = ((this.elementPanel != null && this.elementPanel.getViewDataDictionary != null) ? this.elementPanel.getViewDataDictionary() : null);
			if (serializableJsonDictionary != null && !string.IsNullOrEmpty(this.persistenceKey) && this.enablePersistence)
			{
				string text = key + "__" + obj.GetType();
				if (!serializableJsonDictionary.ContainsKey(text))
				{
					serializableJsonDictionary.Set<object>(text, obj);
				}
				else
				{
					serializableJsonDictionary.Overwrite(obj, text);
				}
			}
		}

		public void SavePersistentData()
		{
			if (this.elementPanel != null && this.elementPanel.savePersistentViewData != null && !string.IsNullOrEmpty(this.persistenceKey))
			{
				this.elementPanel.savePersistentViewData();
			}
		}

		internal bool IsPersitenceSupportedOnChildren()
		{
			return base.GetType() == typeof(VisualElement) || !string.IsNullOrEmpty(this.persistenceKey);
		}

		internal void OnPersistentDataReady(bool enablePersistence)
		{
			this.enablePersistence = enablePersistence;
			this.OnPersistentDataReady();
		}

		public virtual void OnPersistentDataReady()
		{
		}

		public virtual bool ContainsPoint(Vector2 localPoint)
		{
			return this.rect.Contains(localPoint);
		}

		public virtual bool Overlaps(Rect rectangle)
		{
			return this.rect.Overlaps(rectangle, true);
		}

		internal bool requireMeasureFunction
		{
			get
			{
				return this.m_RequireMeasureFunction;
			}
			set
			{
				this.m_RequireMeasureFunction = value;
				if (this.m_RequireMeasureFunction && !this.yogaNode.IsMeasureDefined)
				{
					this.yogaNode.SetMeasureFunction(new MeasureFunction(this.Measure));
				}
				else if (!this.m_RequireMeasureFunction && this.yogaNode.IsMeasureDefined)
				{
					this.yogaNode.SetMeasureFunction(null);
				}
			}
		}

		protected internal virtual Vector2 DoMeasure(float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			return new Vector2(float.NaN, float.NaN);
		}

		internal YogaSize Measure(YogaNode node, float width, YogaMeasureMode widthMode, float height, YogaMeasureMode heightMode)
		{
			Debug.Assert(node == this.yogaNode, "YogaNode instance mismatch");
			Vector2 vector = this.DoMeasure(width, (VisualElement.MeasureMode)widthMode, height, (VisualElement.MeasureMode)heightMode);
			return MeasureOutput.Make((float)Mathf.RoundToInt(vector.x), (float)Mathf.RoundToInt(vector.y));
		}

		public void SetSize(Vector2 size)
		{
			Rect layout = this.layout;
			layout.width = size.x;
			layout.height = size.y;
			this.layout = layout;
		}

		private void FinalizeLayout()
		{
			if (this.hasInlineStyle)
			{
				this.effectiveStyle.SyncWithLayout(this.yogaNode);
			}
			else
			{
				this.yogaNode.CopyStyle(this.effectiveStyle.yogaNode);
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event OnStylesResolved onStylesResolved;

		internal void SetInlineStyles(VisualElementStylesData inlineStyle)
		{
			Debug.Assert(!inlineStyle.isShared);
			inlineStyle.Apply(this.m_Style, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity);
			this.m_Style = inlineStyle;
		}

		internal void SetSharedStyles(VisualElementStylesData sharedStyle)
		{
			Debug.Assert(sharedStyle.isShared);
			if (sharedStyle != this.m_SharedStyle)
			{
				if (this.hasInlineStyle)
				{
					this.m_Style.Apply(sharedStyle, StylePropertyApplyMode.CopyIfNotInline);
				}
				else
				{
					this.m_Style = sharedStyle;
				}
				this.m_SharedStyle = sharedStyle;
				if (this.onStylesResolved != null)
				{
					this.onStylesResolved(this.m_Style);
				}
				this.OnStyleResolved(this.m_Style);
				this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}

		public void ResetPositionProperties()
		{
			if (this.hasInlineStyle)
			{
				VisualElementStylesData inlineStyle = this.inlineStyle;
				inlineStyle.positionType = StyleValue<int>.nil;
				inlineStyle.marginLeft = StyleValue<float>.nil;
				inlineStyle.marginRight = StyleValue<float>.nil;
				inlineStyle.marginBottom = StyleValue<float>.nil;
				inlineStyle.marginTop = StyleValue<float>.nil;
				inlineStyle.positionLeft = StyleValue<float>.nil;
				inlineStyle.positionTop = StyleValue<float>.nil;
				inlineStyle.positionRight = StyleValue<float>.nil;
				inlineStyle.positionBottom = StyleValue<float>.nil;
				inlineStyle.width = StyleValue<float>.nil;
				inlineStyle.height = StyleValue<float>.nil;
				this.m_Style.Apply(this.sharedStyle, StylePropertyApplyMode.CopyIfNotInline);
				this.FinalizeLayout();
				this.IncrementVersion(VersionChangeType.Layout);
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.GetType().Name,
				" ",
				this.name,
				" ",
				this.layout,
				" world rect: ",
				this.worldBound
			});
		}

		internal IEnumerable<string> GetClasses()
		{
			return this.m_ClassList;
		}

		public void ClearClassList()
		{
			if (this.m_ClassList.Count > 0)
			{
				this.m_ClassList = VisualElement.s_EmptyClassList;
				this.IncrementVersion(VersionChangeType.StyleSheet);
			}
		}

		public void AddToClassList(string className)
		{
			if (this.m_ClassList == VisualElement.s_EmptyClassList)
			{
				this.m_ClassList = new List<string> { className };
			}
			else
			{
				if (this.m_ClassList.Contains(className))
				{
					return;
				}
				this.m_ClassList.Capacity++;
				this.m_ClassList.Add(className);
			}
			this.IncrementVersion(VersionChangeType.StyleSheet);
		}

		public void RemoveFromClassList(string className)
		{
			if (this.m_ClassList.Remove(className))
			{
				this.IncrementVersion(VersionChangeType.StyleSheet);
			}
		}

		public void ToggleInClassList(string className)
		{
			if (this.ClassListContains(className))
			{
				this.RemoveFromClassList(className);
			}
			else
			{
				this.AddToClassList(className);
			}
		}

		public void EnableInClassList(string className, bool enable)
		{
			if (enable)
			{
				this.AddToClassList(className);
			}
			else
			{
				this.RemoveFromClassList(className);
			}
		}

		public bool ClassListContains(string cls)
		{
			for (int i = 0; i < this.m_ClassList.Count; i++)
			{
				if (this.m_ClassList[i] == cls)
				{
					return true;
				}
			}
			return false;
		}

		public object FindAncestorUserData()
		{
			for (VisualElement visualElement = this.parent; visualElement != null; visualElement = visualElement.parent)
			{
				if (visualElement.userData != null)
				{
					return visualElement.userData;
				}
			}
			return null;
		}

		private void UpdateCursorStyle(long eventType)
		{
			if (this.elementPanel != null)
			{
				if (eventType == EventBase<MouseOverEvent>.TypeId())
				{
					this.elementPanel.cursorManager.SetCursor(this.style.cursor.value);
				}
				else
				{
					this.elementPanel.cursorManager.ResetCursor();
				}
			}
		}

		public IUIElementDataWatch dataWatch
		{
			get
			{
				return this;
			}
		}

		IUIElementDataWatchRequest IUIElementDataWatch.RegisterWatch(Object toWatch, Action<Object> watchNotification)
		{
			VisualElement.DataWatchRequest dataWatchRequest = new VisualElement.DataWatchRequest(this)
			{
				notification = watchNotification,
				watchedObject = toWatch
			};
			dataWatchRequest.Start();
			return dataWatchRequest;
		}

		void IUIElementDataWatch.UnregisterWatch(IUIElementDataWatchRequest requested)
		{
			VisualElement.DataWatchRequest dataWatchRequest = requested as VisualElement.DataWatchRequest;
			if (dataWatchRequest != null)
			{
				dataWatchRequest.Stop();
			}
		}

		public VisualElement.Hierarchy shadow { get; private set; }

		public VisualElement.ClippingOptions clippingOptions
		{
			get
			{
				return this.m_ClippingOptions;
			}
			set
			{
				if (this.m_ClippingOptions != value)
				{
					this.m_ClippingOptions = value;
					this.IncrementVersion(VersionChangeType.Repaint);
				}
			}
		}

		internal bool ShouldClip()
		{
			return this.style.overflow != Overflow.Visible || this.clippingOptions != VisualElement.ClippingOptions.NoClipping;
		}

		public VisualElement parent
		{
			get
			{
				return this.m_LogicalParent;
			}
		}

		internal BaseVisualElementPanel elementPanel { get; private set; }

		public IPanel panel
		{
			get
			{
				return this.elementPanel;
			}
		}

		public virtual VisualElement contentContainer
		{
			get
			{
				return this;
			}
		}

		public void Add(VisualElement child)
		{
			if (this.contentContainer == this)
			{
				this.shadow.Add(child);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				if (contentContainer != null)
				{
					contentContainer.Add(child);
				}
			}
			child.m_LogicalParent = this;
		}

		public void Insert(int index, VisualElement element)
		{
			if (this.contentContainer == this)
			{
				this.shadow.Insert(index, element);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				if (contentContainer != null)
				{
					contentContainer.Insert(index, element);
				}
			}
			element.m_LogicalParent = this;
		}

		public void Remove(VisualElement element)
		{
			if (this.contentContainer == this)
			{
				this.shadow.Remove(element);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				if (contentContainer != null)
				{
					contentContainer.Remove(element);
				}
			}
		}

		public void RemoveAt(int index)
		{
			if (this.contentContainer == this)
			{
				this.shadow.RemoveAt(index);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				if (contentContainer != null)
				{
					contentContainer.RemoveAt(index);
				}
			}
		}

		public void Clear()
		{
			if (this.contentContainer == this)
			{
				this.shadow.Clear();
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				if (contentContainer != null)
				{
					contentContainer.Clear();
				}
			}
		}

		public VisualElement ElementAt(int index)
		{
			VisualElement visualElement;
			if (this.contentContainer == this)
			{
				visualElement = this.shadow.ElementAt(index);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				visualElement = ((contentContainer != null) ? contentContainer.ElementAt(index) : null);
			}
			return visualElement;
		}

		public VisualElement this[int key]
		{
			get
			{
				return this.ElementAt(key);
			}
		}

		public int childCount
		{
			get
			{
				int num;
				if (this.contentContainer == this)
				{
					num = this.shadow.childCount;
				}
				else
				{
					VisualElement contentContainer = this.contentContainer;
					int? num2 = ((contentContainer != null) ? new int?(contentContainer.childCount) : null);
					num = ((num2 == null) ? 0 : num2.Value);
				}
				return num;
			}
		}

		public int IndexOf(VisualElement element)
		{
			int num;
			if (this.contentContainer == this)
			{
				num = this.shadow.IndexOf(element);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				int? num2 = ((contentContainer != null) ? new int?(contentContainer.IndexOf(element)) : null);
				num = ((num2 == null) ? (-1) : num2.Value);
			}
			return num;
		}

		public IEnumerable<VisualElement> Children()
		{
			IEnumerable<VisualElement> enumerable;
			if (this.contentContainer == this)
			{
				enumerable = this.shadow.Children();
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				enumerable = ((contentContainer != null) ? contentContainer.Children() : null) ?? VisualElement.s_EmptyList;
			}
			return enumerable;
		}

		public void Sort(Comparison<VisualElement> comp)
		{
			if (this.contentContainer == this)
			{
				this.shadow.Sort(comp);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				if (contentContainer != null)
				{
					contentContainer.Sort(comp);
				}
			}
		}

		public void BringToFront()
		{
			if (this.shadow.parent != null)
			{
				this.shadow.parent.shadow.BringToFront(this);
			}
		}

		public void SendToBack()
		{
			if (this.shadow.parent != null)
			{
				this.shadow.parent.shadow.SendToBack(this);
			}
		}

		public void PlaceBehind(VisualElement sibling)
		{
			if (this.shadow.parent == null || sibling.shadow.parent != this.shadow.parent)
			{
				throw new ArgumentException("VisualElements are not siblings");
			}
			this.shadow.parent.shadow.PlaceBehind(this, sibling);
		}

		public void PlaceInFront(VisualElement sibling)
		{
			if (this.shadow.parent == null || sibling.shadow.parent != this.shadow.parent)
			{
				throw new ArgumentException("VisualElements are not siblings");
			}
			this.shadow.parent.shadow.PlaceInFront(this, sibling);
		}

		public void RemoveFromHierarchy()
		{
			if (this.shadow.parent != null)
			{
				this.shadow.parent.shadow.Remove(this);
			}
		}

		public T GetFirstOfType<T>() where T : class
		{
			T t = this as T;
			T t2;
			if (t != null)
			{
				t2 = t;
			}
			else
			{
				t2 = this.GetFirstAncestorOfType<T>();
			}
			return t2;
		}

		public T GetFirstAncestorOfType<T>() where T : class
		{
			for (VisualElement visualElement = this.shadow.parent; visualElement != null; visualElement = visualElement.shadow.parent)
			{
				T t = visualElement as T;
				if (t != null)
				{
					return t;
				}
			}
			return (T)((object)null);
		}

		public bool Contains(VisualElement child)
		{
			while (child != null)
			{
				if (child.shadow.parent == this)
				{
					return true;
				}
				child = child.shadow.parent;
			}
			return false;
		}

		private void GatherAllChildren(List<VisualElement> elements)
		{
			if (this.m_Children != null && this.m_Children.Count > 0)
			{
				int i = elements.Count;
				elements.AddRange(this.m_Children);
				while (i < elements.Count)
				{
					VisualElement visualElement = elements[i];
					if (visualElement.m_Children != null && visualElement.m_Children.Count > 0)
					{
						elements.AddRange(visualElement.m_Children);
					}
					i++;
				}
			}
		}

		public VisualElement FindCommonAncestor(VisualElement other)
		{
			VisualElement visualElement;
			if (this.panel != other.panel)
			{
				visualElement = null;
			}
			else
			{
				VisualElement visualElement2 = this;
				int i = 0;
				while (visualElement2 != null)
				{
					i++;
					visualElement2 = visualElement2.shadow.parent;
				}
				VisualElement visualElement3 = other;
				int j = 0;
				while (visualElement3 != null)
				{
					j++;
					visualElement3 = visualElement3.shadow.parent;
				}
				visualElement2 = this;
				visualElement3 = other;
				while (i > j)
				{
					i--;
					visualElement2 = visualElement2.shadow.parent;
				}
				while (j > i)
				{
					j--;
					visualElement3 = visualElement3.shadow.parent;
				}
				while (visualElement2 != visualElement3)
				{
					visualElement2 = visualElement2.shadow.parent;
					visualElement3 = visualElement3.shadow.parent;
				}
				visualElement = visualElement2;
			}
			return visualElement;
		}

		public IEnumerator<VisualElement> GetEnumerator()
		{
			IEnumerator<VisualElement> enumerator;
			if (this.contentContainer == this)
			{
				enumerator = this.shadow.Children().GetEnumerator();
			}
			else
			{
				IEnumerator<VisualElement> enumerator2;
				if (this.contentContainer != null)
				{
					enumerator2 = this.contentContainer.GetEnumerator();
				}
				else
				{
					enumerator2 = (IEnumerator<VisualElement>)VisualElement.s_EmptyList.GetEnumerator();
				}
				enumerator = enumerator2;
			}
			return enumerator;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			IEnumerator enumerator;
			if (this.contentContainer == this)
			{
				enumerator = this.shadow.Children().GetEnumerator();
			}
			else
			{
				IEnumerator enumerator2;
				if (this.contentContainer != null)
				{
					enumerator2 = ((IEnumerable)this.contentContainer).GetEnumerator();
				}
				else
				{
					enumerator2 = VisualElement.s_EmptyList.GetEnumerator();
				}
				enumerator = enumerator2;
			}
			return enumerator;
		}

		public IVisualElementScheduler schedule
		{
			get
			{
				return this;
			}
		}

		IVisualElementScheduledItem IVisualElementScheduler.Execute(Action<TimerState> timerUpdateEvent)
		{
			VisualElement.TimerStateScheduledItem timerStateScheduledItem = new VisualElement.TimerStateScheduledItem(this, timerUpdateEvent)
			{
				timerUpdateStopCondition = ScheduledItem.OnceCondition
			};
			timerStateScheduledItem.Resume();
			return timerStateScheduledItem;
		}

		IVisualElementScheduledItem IVisualElementScheduler.Execute(Action updateEvent)
		{
			VisualElement.SimpleScheduledItem simpleScheduledItem = new VisualElement.SimpleScheduledItem(this, updateEvent)
			{
				timerUpdateStopCondition = ScheduledItem.OnceCondition
			};
			simpleScheduledItem.Resume();
			return simpleScheduledItem;
		}

		public IStyle style
		{
			get
			{
				return this;
			}
		}

		StyleValue<float> IStyle.width
		{
			get
			{
				return this.effectiveStyle.width;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.width, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.Width = value.value;
				}
			}
		}

		StyleValue<float> IStyle.height
		{
			get
			{
				return this.effectiveStyle.height;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.height, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.Height = value.value;
				}
			}
		}

		StyleValue<float> IStyle.maxWidth
		{
			get
			{
				return this.effectiveStyle.maxWidth;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.maxWidth, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.MaxWidth = value.value;
				}
			}
		}

		StyleValue<float> IStyle.maxHeight
		{
			get
			{
				return this.effectiveStyle.maxHeight;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.maxHeight, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.MaxHeight = value.value;
				}
			}
		}

		StyleValue<float> IStyle.minWidth
		{
			get
			{
				return this.effectiveStyle.minWidth;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.minWidth, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.MinWidth = value.value;
				}
			}
		}

		StyleValue<float> IStyle.minHeight
		{
			get
			{
				return this.effectiveStyle.minHeight;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.minHeight, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.MinHeight = value.value;
				}
			}
		}

		StyleValue<Flex> IStyle.flex
		{
			get
			{
				return new Flex(this.style.flexGrow, this.style.flexShrink, this.style.flexBasis);
			}
			set
			{
				this.style.flexGrow = new StyleValue<float>(value.value.grow, value.specificity);
				this.style.flexShrink = new StyleValue<float>(value.value.shrink, value.specificity);
				this.style.flexBasis = new StyleValue<float>(value.value.basis, value.specificity);
			}
		}

		StyleValue<float> IStyle.flexBasis
		{
			get
			{
				return this.effectiveStyle.FlexBasisToFloat();
			}
			set
			{
				float num;
				if (this.inlineStyle.flexBasis.value.isKeyword)
				{
					if (this.inlineStyle.flexBasis.value.keyword == StyleValueKeyword.Auto)
					{
						num = -1f;
					}
					else
					{
						num = float.NaN;
					}
				}
				else
				{
					num = this.inlineStyle.flexBasis.value.floatValue;
				}
				StyleValue<float> styleValue = new StyleValue<float>(num, this.inlineStyle.flexBasis.specificity);
				if (StyleValueUtils.ApplyAndCompare(ref styleValue, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					if (value.value == -1f)
					{
						this.inlineStyle.flexBasis.value = new FloatOrKeyword(StyleValueKeyword.Auto);
						this.yogaNode.FlexBasis = YogaValue.Auto();
					}
					else
					{
						this.inlineStyle.flexBasis.value = new FloatOrKeyword(value.value);
						this.yogaNode.FlexBasis = value.value;
					}
				}
				this.inlineStyle.flexBasis.specificity = styleValue.specificity;
			}
		}

		StyleValue<float> IStyle.flexGrow
		{
			get
			{
				return this.effectiveStyle.flexGrow;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.flexGrow, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.FlexGrow = value.value;
				}
			}
		}

		StyleValue<float> IStyle.flexShrink
		{
			get
			{
				return this.effectiveStyle.flexShrink;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.flexShrink, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.FlexShrink = value.value;
				}
			}
		}

		StyleValue<Overflow> IStyle.overflow
		{
			get
			{
				return new StyleValue<Overflow>((Overflow)this.effectiveStyle.overflow.value, this.effectiveStyle.overflow.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.overflow, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.Overflow = (YogaOverflow)value.value;
				}
			}
		}

		StyleValue<float> IStyle.positionLeft
		{
			get
			{
				return this.effectiveStyle.positionLeft;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.positionLeft, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.Left = value.value;
				}
			}
		}

		StyleValue<float> IStyle.positionTop
		{
			get
			{
				return this.effectiveStyle.positionTop;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.positionTop, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.Top = value.value;
				}
			}
		}

		StyleValue<float> IStyle.positionRight
		{
			get
			{
				return this.effectiveStyle.positionRight;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.positionRight, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.Right = value.value;
				}
			}
		}

		StyleValue<float> IStyle.positionBottom
		{
			get
			{
				return this.effectiveStyle.positionBottom;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.positionBottom, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.Bottom = value.value;
				}
			}
		}

		StyleValue<float> IStyle.marginLeft
		{
			get
			{
				return this.effectiveStyle.marginLeft;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.marginLeft, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.MarginLeft = value.value;
				}
			}
		}

		StyleValue<float> IStyle.marginTop
		{
			get
			{
				return this.effectiveStyle.marginTop;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.marginTop, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.MarginTop = value.value;
				}
			}
		}

		StyleValue<float> IStyle.marginRight
		{
			get
			{
				return this.effectiveStyle.marginRight;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.marginRight, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.MarginRight = value.value;
				}
			}
		}

		StyleValue<float> IStyle.marginBottom
		{
			get
			{
				return this.effectiveStyle.marginBottom;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.marginBottom, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.MarginBottom = value.value;
				}
			}
		}

		[Obsolete("Use borderLeftWidth instead")]
		StyleValue<float> IStyle.borderLeft
		{
			get
			{
				return ((IStyle)this).borderLeftWidth;
			}
			set
			{
				((IStyle)this).borderLeftWidth = value;
			}
		}

		[Obsolete("Use borderTopWidth instead")]
		StyleValue<float> IStyle.borderTop
		{
			get
			{
				return ((IStyle)this).borderTopWidth;
			}
			set
			{
				((IStyle)this).borderTopWidth = value;
			}
		}

		[Obsolete("Use borderRightWidth instead")]
		StyleValue<float> IStyle.borderRight
		{
			get
			{
				return ((IStyle)this).borderRightWidth;
			}
			set
			{
				((IStyle)this).borderRightWidth = value;
			}
		}

		[Obsolete("Use borderBottomWidth instead")]
		StyleValue<float> IStyle.borderBottom
		{
			get
			{
				return ((IStyle)this).borderBottomWidth;
			}
			set
			{
				((IStyle)this).borderBottomWidth = value;
			}
		}

		StyleValue<float> IStyle.borderLeftWidth
		{
			get
			{
				return this.effectiveStyle.borderLeftWidth;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderLeftWidth, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.BorderLeftWidth = value.value;
				}
			}
		}

		StyleValue<float> IStyle.borderTopWidth
		{
			get
			{
				return this.effectiveStyle.borderTopWidth;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderTopWidth, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.BorderTopWidth = value.value;
				}
			}
		}

		StyleValue<float> IStyle.borderRightWidth
		{
			get
			{
				return this.effectiveStyle.borderRightWidth;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderRightWidth, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.BorderRightWidth = value.value;
				}
			}
		}

		StyleValue<float> IStyle.borderBottomWidth
		{
			get
			{
				return this.effectiveStyle.borderBottomWidth;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderBottomWidth, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.BorderBottomWidth = value.value;
				}
			}
		}

		StyleValue<float> IStyle.borderRadius
		{
			get
			{
				return this.style.borderTopLeftRadius;
			}
			set
			{
				this.style.borderTopLeftRadius = value;
				this.style.borderTopRightRadius = value;
				this.style.borderBottomLeftRadius = value;
				this.style.borderBottomRightRadius = value;
			}
		}

		StyleValue<float> IStyle.borderTopLeftRadius
		{
			get
			{
				return this.effectiveStyle.borderTopLeftRadius;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderTopLeftRadius, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<float> IStyle.borderTopRightRadius
		{
			get
			{
				return this.effectiveStyle.borderTopRightRadius;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderTopRightRadius, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<float> IStyle.borderBottomRightRadius
		{
			get
			{
				return this.effectiveStyle.borderBottomRightRadius;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderBottomRightRadius, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<float> IStyle.borderBottomLeftRadius
		{
			get
			{
				return this.effectiveStyle.borderBottomLeftRadius;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderBottomLeftRadius, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<float> IStyle.paddingLeft
		{
			get
			{
				return this.effectiveStyle.paddingLeft;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.paddingLeft, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.PaddingLeft = value.value;
				}
			}
		}

		StyleValue<float> IStyle.paddingTop
		{
			get
			{
				return this.effectiveStyle.paddingTop;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.paddingTop, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.PaddingTop = value.value;
				}
			}
		}

		StyleValue<float> IStyle.paddingRight
		{
			get
			{
				return this.effectiveStyle.paddingRight;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.paddingRight, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.PaddingRight = value.value;
				}
			}
		}

		StyleValue<float> IStyle.paddingBottom
		{
			get
			{
				return this.effectiveStyle.paddingBottom;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.paddingBottom, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.PaddingBottom = value.value;
				}
			}
		}

		StyleValue<PositionType> IStyle.positionType
		{
			get
			{
				return new StyleValue<PositionType>((PositionType)this.effectiveStyle.positionType.value, this.effectiveStyle.positionType.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.positionType, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					PositionType value2 = value.value;
					if (value2 != PositionType.Absolute && value2 != PositionType.Manual)
					{
						if (value2 == PositionType.Relative)
						{
							this.yogaNode.PositionType = YogaPositionType.Relative;
						}
					}
					else
					{
						this.yogaNode.PositionType = YogaPositionType.Absolute;
					}
				}
			}
		}

		StyleValue<Align> IStyle.alignSelf
		{
			get
			{
				return new StyleValue<Align>((Align)this.effectiveStyle.alignSelf.value, this.effectiveStyle.alignSelf.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.alignSelf, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.AlignSelf = (YogaAlign)value.value;
				}
			}
		}

		[Obsolete("Use unityTextAlign instead")]
		StyleValue<TextAnchor> IStyle.textAlignment
		{
			get
			{
				return ((IStyle)this).unityTextAlign;
			}
			set
			{
				((IStyle)this).unityTextAlign = value;
			}
		}

		StyleValue<TextAnchor> IStyle.unityTextAlign
		{
			get
			{
				return new StyleValue<TextAnchor>((TextAnchor)this.effectiveStyle.unityTextAlign.value, this.effectiveStyle.unityTextAlign.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.unityTextAlign, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		[Obsolete("Use fontStyleAndWeight instead")]
		StyleValue<FontStyle> IStyle.fontStyle
		{
			get
			{
				return ((IStyle)this).fontStyleAndWeight;
			}
			set
			{
				((IStyle)this).fontStyleAndWeight = value;
			}
		}

		StyleValue<FontStyle> IStyle.fontStyleAndWeight
		{
			get
			{
				return new StyleValue<FontStyle>((FontStyle)this.effectiveStyle.fontStyleAndWeight.value, this.effectiveStyle.fontStyleAndWeight.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.fontStyleAndWeight, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				}
			}
		}

		StyleValue<TextClipping> IStyle.textClipping
		{
			get
			{
				return new StyleValue<TextClipping>((TextClipping)this.effectiveStyle.textClipping.value, this.effectiveStyle.textClipping.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.textClipping, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<Font> IStyle.font
		{
			get
			{
				return this.effectiveStyle.font;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompareObject<Font>(ref this.inlineStyle.font, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				}
			}
		}

		StyleValue<int> IStyle.fontSize
		{
			get
			{
				return this.effectiveStyle.fontSize;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.fontSize, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				}
			}
		}

		StyleValue<bool> IStyle.wordWrap
		{
			get
			{
				return this.effectiveStyle.wordWrap;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.wordWrap, value))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				}
			}
		}

		[Obsolete("Use color instead")]
		StyleValue<Color> IStyle.textColor
		{
			get
			{
				return ((IStyle)this).color;
			}
			set
			{
				((IStyle)this).color = value;
			}
		}

		StyleValue<Color> IStyle.color
		{
			get
			{
				return this.effectiveStyle.color;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.color, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<FlexDirection> IStyle.flexDirection
		{
			get
			{
				return new StyleValue<FlexDirection>((FlexDirection)this.effectiveStyle.flexDirection.value, this.effectiveStyle.flexDirection.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.flexDirection, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
					this.yogaNode.FlexDirection = (YogaFlexDirection)value.value;
				}
			}
		}

		StyleValue<Color> IStyle.backgroundColor
		{
			get
			{
				return this.effectiveStyle.backgroundColor;
			}
			set
			{
				if (value.specificity == 0 && value == default(Color))
				{
					this.inlineStyle.backgroundColor = this.sharedStyle.backgroundColor;
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
				else if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.backgroundColor, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<Color> IStyle.borderColor
		{
			get
			{
				return this.effectiveStyle.borderColor;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderColor, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<Texture2D> IStyle.backgroundImage
		{
			get
			{
				return this.effectiveStyle.backgroundImage;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompareObject<Texture2D>(ref this.inlineStyle.backgroundImage, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		[Obsolete("Use backgroundScaleMode instead")]
		StyleValue<ScaleMode> IStyle.backgroundSize
		{
			get
			{
				return ((IStyle)this).backgroundScaleMode;
			}
			set
			{
				((IStyle)this).backgroundScaleMode = value;
			}
		}

		StyleValue<ScaleMode> IStyle.backgroundScaleMode
		{
			get
			{
				return new StyleValue<ScaleMode>((ScaleMode)this.effectiveStyle.backgroundScaleMode.value, this.effectiveStyle.backgroundScaleMode.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.backgroundScaleMode, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<Align> IStyle.alignItems
		{
			get
			{
				return new StyleValue<Align>((Align)this.effectiveStyle.alignItems.value, this.effectiveStyle.alignItems.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.alignItems, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.AlignItems = (YogaAlign)value.value;
				}
			}
		}

		StyleValue<Align> IStyle.alignContent
		{
			get
			{
				return new StyleValue<Align>((Align)this.effectiveStyle.alignContent.value, this.effectiveStyle.alignContent.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.alignContent, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.AlignContent = (YogaAlign)value.value;
				}
			}
		}

		StyleValue<Justify> IStyle.justifyContent
		{
			get
			{
				return new StyleValue<Justify>((Justify)this.effectiveStyle.justifyContent.value, this.effectiveStyle.justifyContent.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.justifyContent, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.JustifyContent = (YogaJustify)value.value;
				}
			}
		}

		StyleValue<Wrap> IStyle.flexWrap
		{
			get
			{
				return new StyleValue<Wrap>((Wrap)this.effectiveStyle.flexWrap.value, this.effectiveStyle.flexWrap.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.flexWrap, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.yogaNode.Wrap = (YogaWrap)value.value;
				}
			}
		}

		StyleValue<int> IStyle.sliceLeft
		{
			get
			{
				return this.effectiveStyle.sliceLeft;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.sliceLeft, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<int> IStyle.sliceTop
		{
			get
			{
				return this.effectiveStyle.sliceTop;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.sliceTop, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<int> IStyle.sliceRight
		{
			get
			{
				return this.effectiveStyle.sliceRight;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.sliceRight, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<int> IStyle.sliceBottom
		{
			get
			{
				return this.effectiveStyle.sliceBottom;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.sliceBottom, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<float> IStyle.opacity
		{
			get
			{
				return this.effectiveStyle.opacity;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.opacity, value))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleValue<CursorStyle> IStyle.cursor
		{
			get
			{
				return this.effectiveStyle.cursor;
			}
			set
			{
				StyleValueUtils.ApplyAndCompare<CursorStyle>(ref this.inlineStyle.cursor, value);
			}
		}

		StyleValue<Visibility> IStyle.visibility
		{
			get
			{
				return new StyleValue<Visibility>((Visibility)this.effectiveStyle.visibility.value, this.effectiveStyle.visibility.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.visibility, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		internal IList<StyleSheet> styleSheets
		{
			get
			{
				if (this.m_StyleSheets == null && this.m_StyleSheetPaths != null)
				{
					this.LoadStyleSheetsFromPaths();
				}
				return this.m_StyleSheets;
			}
		}

		public void AddStyleSheetPath(string sheetPath)
		{
			if (this.m_StyleSheetPaths == null)
			{
				this.m_StyleSheetPaths = new List<string>();
			}
			this.m_StyleSheetPaths.Add(sheetPath);
			this.m_StyleSheets = null;
			this.IncrementVersion(VersionChangeType.StyleSheet);
		}

		public void RemoveStyleSheetPath(string sheetPath)
		{
			if (this.m_StyleSheetPaths == null)
			{
				Debug.LogWarning("Attempting to remove from null style sheet path list");
			}
			else
			{
				this.m_StyleSheetPaths.Remove(sheetPath);
				this.m_StyleSheets = null;
				this.IncrementVersion(VersionChangeType.StyleSheet);
			}
		}

		public bool HasStyleSheetPath(string sheetPath)
		{
			return this.m_StyleSheetPaths != null && this.m_StyleSheetPaths.Contains(sheetPath);
		}

		internal void ReplaceStyleSheetPath(string oldSheetPath, string newSheetPath)
		{
			if (this.m_StyleSheetPaths == null)
			{
				Debug.LogWarning("Attempting to replace a style from null style sheet path list");
			}
			else
			{
				int num = this.m_StyleSheetPaths.IndexOf(oldSheetPath);
				if (num >= 0)
				{
					this.m_StyleSheetPaths[num] = newSheetPath;
					this.m_StyleSheets = null;
					this.IncrementVersion(VersionChangeType.StyleSheet);
				}
			}
		}

		internal void LoadStyleSheetsFromPaths()
		{
			if (this.m_StyleSheetPaths != null && this.elementPanel != null)
			{
				this.m_StyleSheets = new List<StyleSheet>();
				foreach (string text in this.m_StyleSheetPaths)
				{
					StyleSheet styleSheet = Panel.loadResourceFunc(text, typeof(StyleSheet)) as StyleSheet;
					if (styleSheet != null)
					{
						if (!styleSheet.hasSelectorsCached)
						{
							int i = 0;
							int num = styleSheet.complexSelectors.Length;
							while (i < num)
							{
								styleSheet.complexSelectors[i].CachePseudoStateMasks();
								i++;
							}
							styleSheet.hasSelectorsCached = true;
						}
						this.m_StyleSheets.Add(styleSheet);
					}
					else
					{
						Debug.LogWarning(string.Format("Style sheet not found for path \"{0}\"", text));
					}
				}
			}
		}

		public string tooltip
		{
			get
			{
				string text;
				base.TryGetUserArgs<TooltipEvent, string>(new EventCallback<TooltipEvent, string>(VisualElement.OnTooltip), TrickleDown.NoTrickleDown, out text);
				return text ?? string.Empty;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.UnregisterCallback<TooltipEvent, string>(new EventCallback<TooltipEvent, string>(VisualElement.OnTooltip), TrickleDown.NoTrickleDown);
				}
				else
				{
					base.RegisterCallback<TooltipEvent, string>(new EventCallback<TooltipEvent, string>(VisualElement.OnTooltip), value, TrickleDown.NoTrickleDown);
				}
			}
		}

		private static void OnTooltip(TooltipEvent e, string tooltip)
		{
			VisualElement visualElement = e.currentTarget as VisualElement;
			if (visualElement != null)
			{
				e.rect = visualElement.worldBound;
			}
			e.tooltip = tooltip;
			e.StopImmediatePropagation();
		}

		public static readonly int defaultFocusIndex = -1;

		private static uint s_NextId;

		private static List<string> s_EmptyClassList = new List<string>(0);

		private string m_Name;

		private List<string> m_ClassList;

		private string m_TypeName;

		private string m_FullTypeName;

		private string m_PersistenceKey;

		private RenderData m_RenderData;

		private Vector3 m_Position = Vector3.zero;

		private Quaternion m_Rotation = Quaternion.identity;

		private Vector3 m_Scale = Vector3.one;

		private Rect m_Layout;

		private Matrix4x4 m_WorldTransform = Matrix4x4.identity;

		private Rect m_WorldClip = Rect.zero;

		internal PseudoStates triggerPseudoMask;

		internal PseudoStates dependencyPseudoMask;

		private PseudoStates m_PseudoStates;

		internal VisualElementStylesData m_SharedStyle = VisualElementStylesData.none;

		internal VisualElementStylesData m_Style = VisualElementStylesData.none;

		internal readonly uint controlid;

		private bool m_Enabled;

		private bool m_RequireMeasureFunction = false;

		private VisualElement.ClippingOptions m_ClippingOptions = VisualElement.ClippingOptions.NoClipping;

		private VisualElement m_PhysicalParent;

		private VisualElement m_LogicalParent;

		private static readonly VisualElement[] s_EmptyList = new VisualElement[0];

		private List<VisualElement> m_Children;

		private List<StyleSheet> m_StyleSheets;

		private List<string> m_StyleSheetPaths;

		public class UxmlFactory : UxmlFactory<VisualElement, VisualElement.UxmlTraits>
		{
		}

		public class UxmlTraits : UnityEngine.Experimental.UIElements.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield return new UxmlChildElementDescription(typeof(VisualElement));
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				ve.name = this.m_Name.GetValueFromBag(bag, cc);
				ve.pickingMode = this.m_PickingMode.GetValueFromBag(bag, cc);
				ve.focusIndex = this.m_FocusIndex.GetValueFromBag(bag, cc);
				ve.tooltip = this.m_Tooltip.GetValueFromBag(bag, cc);
			}

			private UxmlStringAttributeDescription m_Name = new UxmlStringAttributeDescription
			{
				name = "name"
			};

			private UxmlEnumAttributeDescription<PickingMode> m_PickingMode = new UxmlEnumAttributeDescription<PickingMode>
			{
				name = "picking-mode",
				obsoleteNames = new string[] { "pickingMode" }
			};

			private UxmlStringAttributeDescription m_Tooltip = new UxmlStringAttributeDescription
			{
				name = "tooltip"
			};

			protected UxmlIntAttributeDescription m_FocusIndex = new UxmlIntAttributeDescription
			{
				name = "focus-index",
				obsoleteNames = new string[] { "focusIndex" },
				defaultValue = VisualElement.defaultFocusIndex
			};
		}

		public enum MeasureMode
		{
			Undefined,
			Exactly,
			AtMost
		}

		private class DataWatchRequest : IUIElementDataWatchRequest, IVisualElementPanelActivatable, IDisposable
		{
			public DataWatchRequest(VisualElement handler)
			{
				this.element = handler;
				this.m_Activator = new VisualElementPanelActivator(this);
			}

			public Action<Object> notification { get; set; }

			public Object watchedObject { get; set; }

			public IDataWatchHandle requestedHandle { get; set; }

			public VisualElement element { get; set; }

			public void Start()
			{
				this.m_Activator.SetActive(true);
			}

			public void Stop()
			{
				this.m_Activator.SetActive(false);
			}

			public bool CanBeActivated()
			{
				return this.element != null && this.element.elementPanel != null && this.element.elementPanel.dataWatch != null;
			}

			public void OnPanelActivate()
			{
				if (this.requestedHandle == null)
				{
					this.requestedHandle = this.element.elementPanel.dataWatch.AddWatch(this.watchedObject, this.notification);
				}
			}

			public void OnPanelDeactivate()
			{
				if (this.requestedHandle != null)
				{
					this.element.elementPanel.dataWatch.RemoveWatch(this.requestedHandle);
					this.requestedHandle = null;
				}
			}

			public void Dispose()
			{
				this.Stop();
			}

			private VisualElementPanelActivator m_Activator;
		}

		public enum ClippingOptions
		{
			ClipContents,
			NoClipping,
			ClipAndCacheContents
		}

		public struct Hierarchy
		{
			internal Hierarchy(VisualElement element)
			{
				this.m_Owner = element;
			}

			public VisualElement parent
			{
				get
				{
					return this.m_Owner.m_PhysicalParent;
				}
			}

			public void Add(VisualElement child)
			{
				if (child == null)
				{
					throw new ArgumentException("Cannot add null child");
				}
				this.Insert(this.childCount, child);
			}

			public void Insert(int index, VisualElement child)
			{
				if (child == null)
				{
					throw new ArgumentException("Cannot insert null child");
				}
				if (index > this.childCount)
				{
					throw new IndexOutOfRangeException("Index out of range: " + index);
				}
				if (child == this.m_Owner)
				{
					throw new ArgumentException("Cannot insert element as its own child");
				}
				child.RemoveFromHierarchy();
				child.shadow.SetParent(this.m_Owner);
				if (this.m_Owner.m_Children == null)
				{
					this.m_Owner.m_Children = VisualElementListPool.Get(0);
				}
				if (this.m_Owner.yogaNode.IsMeasureDefined)
				{
					this.m_Owner.yogaNode.SetMeasureFunction(null);
				}
				this.PutChildAtIndex(child, index);
				child.SetEnabledFromHierarchy(this.m_Owner.enabledInHierarchy);
				child.IncrementVersion(VersionChangeType.Hierarchy);
				this.m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
			}

			public void Remove(VisualElement child)
			{
				if (child == null)
				{
					throw new ArgumentException("Cannot remove null child");
				}
				if (child.shadow.parent != this.m_Owner)
				{
					throw new ArgumentException("This visualElement is not my child");
				}
				if (this.m_Owner.m_Children != null)
				{
					int num = this.m_Owner.m_Children.IndexOf(child);
					this.RemoveAt(num);
				}
			}

			public void RemoveAt(int index)
			{
				if (index < 0 || index >= this.childCount)
				{
					throw new IndexOutOfRangeException("Index out of range: " + index);
				}
				VisualElement visualElement = this.m_Owner.m_Children[index];
				this.RemoveChildAtIndex(index);
				visualElement.shadow.SetParent(null);
				if (this.childCount == 0)
				{
					this.ReleaseChildList();
					if (this.m_Owner.requireMeasureFunction)
					{
						this.m_Owner.yogaNode.SetMeasureFunction(new MeasureFunction(this.m_Owner.Measure));
					}
				}
				BaseVisualElementPanel elementPanel = this.m_Owner.elementPanel;
				if (elementPanel != null)
				{
					elementPanel.OnVersionChanged(visualElement, VersionChangeType.Hierarchy);
				}
				this.m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
			}

			public void Clear()
			{
				if (this.childCount > 0)
				{
					foreach (VisualElement visualElement in this.m_Owner.m_Children)
					{
						visualElement.shadow.SetParent(null);
						visualElement.m_LogicalParent = null;
						BaseVisualElementPanel elementPanel = this.m_Owner.elementPanel;
						if (elementPanel != null)
						{
							elementPanel.OnVersionChanged(visualElement, VersionChangeType.Hierarchy);
						}
					}
					this.ReleaseChildList();
					this.m_Owner.yogaNode.Clear();
					if (this.m_Owner.requireMeasureFunction)
					{
						this.m_Owner.yogaNode.SetMeasureFunction(new MeasureFunction(this.m_Owner.Measure));
					}
					this.m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
				}
			}

			internal void BringToFront(VisualElement child)
			{
				if (this.childCount > 1)
				{
					int num = this.m_Owner.m_Children.IndexOf(child);
					if (num >= 0 && num < this.childCount - 1)
					{
						this.MoveChildElement(child, num, this.childCount);
					}
				}
			}

			internal void SendToBack(VisualElement child)
			{
				if (this.childCount > 1)
				{
					int num = this.m_Owner.m_Children.IndexOf(child);
					if (num > 0)
					{
						this.MoveChildElement(child, num, 0);
					}
				}
			}

			internal void PlaceBehind(VisualElement child, VisualElement over)
			{
				if (this.childCount > 0)
				{
					int num = this.m_Owner.m_Children.IndexOf(child);
					if (num >= 0)
					{
						int num2 = this.m_Owner.m_Children.IndexOf(over);
						if (num2 > 0 && num < num2)
						{
							num2--;
						}
						this.MoveChildElement(child, num, num2);
					}
				}
			}

			internal void PlaceInFront(VisualElement child, VisualElement under)
			{
				if (this.childCount > 0)
				{
					int num = this.m_Owner.m_Children.IndexOf(child);
					if (num >= 0)
					{
						int num2 = this.m_Owner.m_Children.IndexOf(under);
						if (num > num2)
						{
							num2++;
						}
						this.MoveChildElement(child, num, num2);
					}
				}
			}

			private void MoveChildElement(VisualElement child, int currentIndex, int nextIndex)
			{
				this.RemoveChildAtIndex(currentIndex);
				this.PutChildAtIndex(child, nextIndex);
				this.m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
			}

			public int childCount
			{
				get
				{
					return (this.m_Owner.m_Children == null) ? 0 : this.m_Owner.m_Children.Count;
				}
			}

			public VisualElement this[int key]
			{
				get
				{
					return this.ElementAt(key);
				}
			}

			public int IndexOf(VisualElement element)
			{
				int num;
				if (this.m_Owner.m_Children != null)
				{
					num = this.m_Owner.m_Children.IndexOf(element);
				}
				else
				{
					num = -1;
				}
				return num;
			}

			public VisualElement ElementAt(int index)
			{
				if (this.m_Owner.m_Children != null)
				{
					return this.m_Owner.m_Children[index];
				}
				throw new IndexOutOfRangeException("Index out of range: " + index);
			}

			public IEnumerable<VisualElement> Children()
			{
				IEnumerable<VisualElement> enumerable;
				if (this.m_Owner.m_Children != null)
				{
					enumerable = this.m_Owner.m_Children;
				}
				else
				{
					enumerable = VisualElement.s_EmptyList;
				}
				return enumerable;
			}

			private void SetParent(VisualElement value)
			{
				this.m_Owner.m_PhysicalParent = value;
				this.m_Owner.m_LogicalParent = value;
				if (value != null)
				{
					this.m_Owner.SetPanel(this.m_Owner.m_PhysicalParent.elementPanel);
				}
				else
				{
					this.m_Owner.SetPanel(null);
				}
			}

			public void Sort(Comparison<VisualElement> comp)
			{
				if (this.childCount > 0)
				{
					this.m_Owner.m_Children.Sort(comp);
					this.m_Owner.yogaNode.Clear();
					for (int i = 0; i < this.m_Owner.m_Children.Count; i++)
					{
						this.m_Owner.yogaNode.Insert(i, this.m_Owner.m_Children[i].yogaNode);
					}
					this.m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
				}
			}

			private void PutChildAtIndex(VisualElement child, int index)
			{
				if (index >= this.childCount)
				{
					this.m_Owner.m_Children.Add(child);
					this.m_Owner.yogaNode.Insert(this.m_Owner.yogaNode.Count, child.yogaNode);
				}
				else
				{
					this.m_Owner.m_Children.Insert(index, child);
					this.m_Owner.yogaNode.Insert(index, child.yogaNode);
				}
			}

			private void RemoveChildAtIndex(int index)
			{
				this.m_Owner.m_Children.RemoveAt(index);
				this.m_Owner.yogaNode.RemoveAt(index);
			}

			private void ReleaseChildList()
			{
				List<VisualElement> children = this.m_Owner.m_Children;
				if (children != null)
				{
					this.m_Owner.m_Children = null;
					VisualElementListPool.Release(children);
				}
			}

			private readonly VisualElement m_Owner;
		}

		private abstract class BaseVisualElementScheduledItem : ScheduledItem, IVisualElementScheduledItem, IVisualElementPanelActivatable
		{
			protected BaseVisualElementScheduledItem(VisualElement handler)
			{
				this.element = handler;
				this.m_Activator = new VisualElementPanelActivator(this);
			}

			public VisualElement element { get; private set; }

			public bool isActive
			{
				get
				{
					return this.m_Activator.isActive;
				}
			}

			public IVisualElementScheduledItem StartingIn(long delayMs)
			{
				base.delayMs = delayMs;
				return this;
			}

			public IVisualElementScheduledItem Until(Func<bool> stopCondition)
			{
				if (stopCondition == null)
				{
					stopCondition = ScheduledItem.ForeverCondition;
				}
				this.timerUpdateStopCondition = stopCondition;
				return this;
			}

			public IVisualElementScheduledItem ForDuration(long durationMs)
			{
				base.SetDuration(durationMs);
				return this;
			}

			public IVisualElementScheduledItem Every(long intervalMs)
			{
				base.intervalMs = intervalMs;
				if (this.timerUpdateStopCondition == ScheduledItem.OnceCondition)
				{
					this.timerUpdateStopCondition = ScheduledItem.ForeverCondition;
				}
				return this;
			}

			internal override void OnItemUnscheduled()
			{
				base.OnItemUnscheduled();
				this.isScheduled = false;
				if (!this.m_Activator.isDetaching)
				{
					this.m_Activator.SetActive(false);
				}
			}

			public void Resume()
			{
				this.m_Activator.SetActive(true);
			}

			public void Pause()
			{
				this.m_Activator.SetActive(false);
			}

			public void ExecuteLater(long delayMs)
			{
				if (!this.isScheduled)
				{
					this.Resume();
				}
				base.ResetStartTime();
				this.StartingIn(delayMs);
			}

			public void OnPanelActivate()
			{
				if (!this.isScheduled)
				{
					this.isScheduled = true;
					base.ResetStartTime();
					this.element.elementPanel.scheduler.Schedule(this);
				}
			}

			public void OnPanelDeactivate()
			{
				if (this.isScheduled)
				{
					this.isScheduled = false;
					this.element.elementPanel.scheduler.Unschedule(this);
				}
			}

			public bool CanBeActivated()
			{
				return this.element != null && this.element.elementPanel != null && this.element.elementPanel.scheduler != null;
			}

			public bool isScheduled = false;

			private VisualElementPanelActivator m_Activator;
		}

		private abstract class VisualElementScheduledItem<ActionType> : VisualElement.BaseVisualElementScheduledItem
		{
			public VisualElementScheduledItem(VisualElement handler, ActionType upEvent)
				: base(handler)
			{
				this.updateEvent = upEvent;
			}

			public static bool Matches(ScheduledItem item, ActionType updateEvent)
			{
				VisualElement.VisualElementScheduledItem<ActionType> visualElementScheduledItem = item as VisualElement.VisualElementScheduledItem<ActionType>;
				return visualElementScheduledItem != null && EqualityComparer<ActionType>.Default.Equals(visualElementScheduledItem.updateEvent, updateEvent);
			}

			public ActionType updateEvent;
		}

		private class TimerStateScheduledItem : VisualElement.VisualElementScheduledItem<Action<TimerState>>
		{
			public TimerStateScheduledItem(VisualElement handler, Action<TimerState> updateEvent)
				: base(handler, updateEvent)
			{
			}

			public override void PerformTimerUpdate(TimerState state)
			{
				if (this.isScheduled)
				{
					this.updateEvent(state);
				}
			}
		}

		private class SimpleScheduledItem : VisualElement.VisualElementScheduledItem<Action>
		{
			public SimpleScheduledItem(VisualElement handler, Action updateEvent)
				: base(handler, updateEvent)
			{
			}

			public override void PerformTimerUpdate(TimerState state)
			{
				if (this.isScheduled)
				{
					this.updateEvent();
				}
			}
		}
	}
}
