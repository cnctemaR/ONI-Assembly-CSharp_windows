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
	/// <summary>
	///   <para>Base class for objects that are part of the UIElements visual tree.</para>
	/// </summary>
	public class VisualElement : Focusable, ITransform, IUIElementDataWatch, IEnumerable<VisualElement>, IVisualElementScheduler, IStyle, IEnumerable
	{
		public VisualElement()
		{
			this.controlid = (VisualElement.s_NextId += 1U);
			this.shadow = new VisualElement.Hierarchy(this);
			this.m_ClassList = new HashSet<string>();
			this.m_FullTypeName = string.Empty;
			this.m_TypeName = string.Empty;
			this.SetEnabled(true);
			this.focusIndex = VisualElement.defaultFocusIndex;
			this.name = string.Empty;
			this.yogaNode = new YogaNode(null);
			this.changesNeeded = ChangeType.All;
			this.clippingOptions = VisualElement.ClippingOptions.ClipContents;
		}

		/// <summary>
		///   <para>Used for view data persistence (ie. tree expanded states, scroll position, zoom level).</para>
		/// </summary>
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
						this.Dirty(ChangeType.PersistentData);
					}
				}
			}
		}

		internal bool enablePersistence { get; private set; }

		/// <summary>
		///   <para>This property can be used to associate application-specific user data with this VisualElement.</para>
		/// </summary>
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
					this.Dirty(ChangeType.Transform);
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
					this.Dirty(ChangeType.Transform);
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
					this.Dirty(ChangeType.Transform);
				}
			}
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
					this.Dirty(ChangeType.Transform);
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
				if (this.IsDirty(ChangeType.Transform))
				{
					Matrix4x4 matrix4x = Matrix4x4.Translate(new Vector3(this.layout.x, this.layout.y, 0f));
					if (this.shadow.parent != null)
					{
						this.renderData.worldTransForm = this.shadow.parent.worldTransform * matrix4x * this.transform.matrix;
					}
					else
					{
						this.renderData.worldTransForm = matrix4x * this.transform.matrix;
					}
					this.ClearDirty(ChangeType.Transform);
				}
				return this.renderData.worldTransForm;
			}
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
						this.Dirty(ChangeType.Styles);
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
					this.Dirty(ChangeType.Styles);
				}
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

		/// <summary>
		///   <para>Callback when the styles of an object have changed.</para>
		/// </summary>
		/// <param name="style"></param>
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
					foreach (VisualElement visualElement in list)
					{
						visualElement.ChangePanel(p);
					}
				}
				finally
				{
					VisualElementListPool.Release(list);
				}
			}
		}

		internal virtual void ChangePanel(BaseVisualElementPanel p)
		{
			if (this.panel != null)
			{
				using (DetachFromPanelEvent pooled = EventBase<DetachFromPanelEvent>.GetPooled())
				{
					pooled.target = this;
					UIElementsUtility.eventDispatcher.DispatchEvent(pooled, this.panel);
				}
			}
			this.elementPanel = p;
			if (this.panel != null)
			{
				using (AttachToPanelEvent pooled2 = EventBase<AttachToPanelEvent>.GetPooled())
				{
					pooled2.target = this;
					UIElementsUtility.eventDispatcher.DispatchEvent(pooled2, this.panel);
				}
			}
			this.Dirty(ChangeType.Styles);
		}

		private void PropagateToChildren(ChangeType type)
		{
			if ((type & this.changesNeeded) != type)
			{
				this.changesNeeded |= type;
				type &= ChangeType.Styles | ChangeType.Transform;
				if (type != (ChangeType)0)
				{
					if (this.m_Children != null)
					{
						foreach (VisualElement visualElement in this.m_Children)
						{
							visualElement.PropagateToChildren(type);
						}
					}
				}
			}
		}

		private void PropagateChangesToParents()
		{
			ChangeType changeType = (ChangeType)0;
			if (this.changesNeeded != (ChangeType)0)
			{
				changeType |= ChangeType.Repaint;
				if ((this.changesNeeded & ChangeType.Styles) > (ChangeType)0)
				{
					changeType |= ChangeType.StylesPath;
				}
				if ((this.changesNeeded & (ChangeType.PersistentData | ChangeType.PersistentDataPath)) > (ChangeType)0)
				{
					changeType |= ChangeType.PersistentDataPath;
				}
			}
			for (VisualElement visualElement = this.shadow.parent; visualElement != null; visualElement = visualElement.shadow.parent)
			{
				if ((visualElement.changesNeeded & changeType) == changeType)
				{
					break;
				}
				visualElement.changesNeeded |= changeType;
			}
		}

		public void Dirty(ChangeType type)
		{
			if ((type & this.changesNeeded) != type)
			{
				if ((type & ChangeType.Layout) == ChangeType.Layout)
				{
					if (this.yogaNode != null && this.yogaNode.IsMeasureDefined)
					{
						this.yogaNode.MarkDirty();
					}
					type |= ChangeType.Repaint;
				}
				if ((type & ChangeType.Transform) == ChangeType.Transform && this.elementPanel != null)
				{
					this.elementPanel.hasDirtyTransform = true;
				}
				this.PropagateToChildren(type);
				this.PropagateChangesToParents();
			}
		}

		internal bool AnyDirty()
		{
			return this.changesNeeded != (ChangeType)0;
		}

		public bool IsDirty(ChangeType type)
		{
			return (this.changesNeeded & type) == type;
		}

		/// <summary>
		///   <para>Checks if any of the ChangeTypes have been marked dirty.</para>
		/// </summary>
		/// <param name="type">The ChangeType(s) to check.</param>
		/// <returns>
		///   <para>True if at least one of the checked ChangeTypes have been marked dirty.</para>
		/// </returns>
		public bool AnyDirty(ChangeType type)
		{
			return (this.changesNeeded & type) > (ChangeType)0;
		}

		public void ClearDirty(ChangeType type)
		{
			this.changesNeeded &= ~type;
		}

		[Obsolete("enabled is deprecated. Use SetEnabled as setter, and enabledSelf/enabledInHierarchy as getters.", true)]
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

		/// <summary>
		///   <para>Returns true if the VisualElement is enabled in its own hierarchy.</para>
		/// </summary>
		public bool enabledInHierarchy
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Disabled) != PseudoStates.Disabled;
			}
		}

		/// <summary>
		///   <para>Returns true if the VisualElement is enabled locally.</para>
		/// </summary>
		public bool enabledSelf
		{
			get
			{
				return this.m_Enabled;
			}
		}

		/// <summary>
		///   <para>Changes whether the current VisualElement is enabled or not. When disabled, a VisualElement does not receive most events.</para>
		/// </summary>
		/// <param name="value">New enabled state</param>
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

		public virtual void DoRepaint()
		{
			IStylePainter stylePainter = this.elementPanel.stylePainter;
			stylePainter.DrawBackground(this);
			stylePainter.DrawBorder(this);
		}

		internal virtual void DoRepaint(IStylePainter painter)
		{
			if (this.visible)
			{
				this.DoRepaint();
			}
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

		/// <summary>
		///   <para>Combine this VisualElement's VisualElement.persistenceKey with those of its parents to create a more unique key for use with VisualElement.GetOrCreatePersistentData.</para>
		/// </summary>
		/// <returns>
		///   <para>Full hierarchical persistence key.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Overwrite object from the persistent data store.</para>
		/// </summary>
		/// <param name="key">The key for the current VisualElement to be used with the persistence store on the EditorWindow.</param>
		/// <param name="obj">Object to overwrite.</param>
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

		/// <summary>
		///   <para>Write persistence data to file.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Called when the persistent data is accessible and/or when the data or persistence key have changed (VisualElement is properly parented).</para>
		/// </summary>
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
			this.yogaNode.Flex = this.style.flex.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.FlexBasis = this.style.flexBasis.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.FlexGrow = this.style.flexGrow.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.FlexShrink = this.style.flexShrink.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.Left = this.style.positionLeft.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.Top = this.style.positionTop.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.Right = this.style.positionRight.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.Bottom = this.style.positionBottom.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.MarginLeft = this.style.marginLeft.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.MarginTop = this.style.marginTop.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.MarginRight = this.style.marginRight.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.MarginBottom = this.style.marginBottom.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.PaddingLeft = this.style.paddingLeft.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.PaddingTop = this.style.paddingTop.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.PaddingRight = this.style.paddingRight.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.PaddingBottom = this.style.paddingBottom.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.BorderLeftWidth = this.style.borderLeft.GetSpecifiedValueOrDefault(this.style.borderLeftWidth.GetSpecifiedValueOrDefault(float.NaN));
			this.yogaNode.BorderTopWidth = this.style.borderTop.GetSpecifiedValueOrDefault(this.style.borderTopWidth.GetSpecifiedValueOrDefault(float.NaN));
			this.yogaNode.BorderRightWidth = this.style.borderRight.GetSpecifiedValueOrDefault(this.style.borderRightWidth.GetSpecifiedValueOrDefault(float.NaN));
			this.yogaNode.BorderBottomWidth = this.style.borderBottom.GetSpecifiedValueOrDefault(this.style.borderBottomWidth.GetSpecifiedValueOrDefault(float.NaN));
			this.yogaNode.Width = this.style.width.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.Height = this.style.height.GetSpecifiedValueOrDefault(float.NaN);
			PositionType positionType = this.style.positionType;
			if (positionType != PositionType.Absolute && positionType != PositionType.Manual)
			{
				if (positionType == PositionType.Relative)
				{
					this.yogaNode.PositionType = YogaPositionType.Relative;
				}
			}
			else
			{
				this.yogaNode.PositionType = YogaPositionType.Absolute;
			}
			this.yogaNode.Overflow = (YogaOverflow)this.style.overflow.value;
			this.yogaNode.AlignSelf = (YogaAlign)this.style.alignSelf.value;
			this.yogaNode.MaxWidth = this.style.maxWidth.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.MaxHeight = this.style.maxHeight.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.MinWidth = this.style.minWidth.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.MinHeight = this.style.minHeight.GetSpecifiedValueOrDefault(float.NaN);
			this.yogaNode.FlexDirection = (YogaFlexDirection)this.style.flexDirection.value;
			this.yogaNode.AlignContent = (YogaAlign)this.style.alignContent.GetSpecifiedValueOrDefault(Align.FlexStart);
			this.yogaNode.AlignItems = (YogaAlign)this.style.alignItems.GetSpecifiedValueOrDefault(Align.Stretch);
			this.yogaNode.JustifyContent = (YogaJustify)this.style.justifyContent.value;
			this.yogaNode.Wrap = (YogaWrap)this.style.flexWrap.value;
			this.Dirty(ChangeType.Layout);
			this.Dirty(ChangeType.Transform);
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
			this.ClearDirty(ChangeType.Styles | ChangeType.StylesPath);
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
				this.Dirty(ChangeType.Repaint);
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
				this.Dirty(ChangeType.Layout);
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
			if (this.m_ClassList != null && this.m_ClassList.Count > 0)
			{
				this.m_ClassList.Clear();
				this.Dirty(ChangeType.Styles);
			}
		}

		public void AddToClassList(string className)
		{
			if (this.m_ClassList == null)
			{
				this.m_ClassList = new HashSet<string>();
			}
			if (this.m_ClassList.Add(className))
			{
				this.Dirty(ChangeType.Styles);
			}
		}

		public void RemoveFromClassList(string className)
		{
			if (this.m_ClassList != null && this.m_ClassList.Remove(className))
			{
				this.Dirty(ChangeType.Styles);
			}
		}

		/// <summary>
		///   <para>Toggles between adding and removing the given class name from the class list.</para>
		/// </summary>
		/// <param name="className">The class name to add or remove from the class list.</param>
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

		/// <summary>
		///   <para>Enables or disables the class with the given name.</para>
		/// </summary>
		/// <param name="className">The name of the class to enable or disable.</param>
		/// <param name="enable">A boolean flag that adds or removes the class name from the class list. If true, EnableInClassList adds the class name to the class list. If false, EnableInClassList removes the class name from the class list.</param>
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
			return this.m_ClassList != null && this.m_ClassList.Contains(cls);
		}

		/// <summary>
		///   <para>Searchs up the hierachy of this VisualElement and retrieves stored userData, if any is found.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Access to this element data watch interface.</para>
		/// </summary>
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

		/// <summary>
		///   <para> Access to this element physical hierarchy
		///           </para>
		/// </summary>
		public VisualElement.Hierarchy shadow { get; private set; }

		/// <summary>
		///   <para>Should this element clip painting to its boundaries.</para>
		/// </summary>
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
					this.Dirty(ChangeType.Repaint);
				}
			}
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

		/// <summary>
		///   <para> child elements are added to this element, usually this
		///           </para>
		/// </summary>
		public virtual VisualElement contentContainer
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		///   <para>Add an element to this element's contentContainer</para>
		/// </summary>
		/// <param name="child"></param>
		public void Add(VisualElement child)
		{
			if (this.contentContainer == this)
			{
				this.shadow.Add(child);
			}
			else
			{
				this.contentContainer.Add(child);
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
				this.contentContainer.Insert(index, element);
			}
			element.m_LogicalParent = this;
		}

		/// <summary>
		///   <para>Removes this child from the hierarchy</para>
		/// </summary>
		/// <param name="element"></param>
		public void Remove(VisualElement element)
		{
			if (this.contentContainer == this)
			{
				this.shadow.Remove(element);
			}
			else
			{
				this.contentContainer.Remove(element);
			}
		}

		/// <summary>
		///   <para>Remove the child element located at this position from this element's contentContainer</para>
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			if (this.contentContainer == this)
			{
				this.shadow.RemoveAt(index);
			}
			else
			{
				this.contentContainer.RemoveAt(index);
			}
		}

		/// <summary>
		///   <para>Remove all child elements from this element's contentContainer</para>
		/// </summary>
		public void Clear()
		{
			if (this.contentContainer == this)
			{
				this.shadow.Clear();
			}
			else
			{
				this.contentContainer.Clear();
			}
		}

		/// <summary>
		///   <para>Retrieves the child element at position</para>
		/// </summary>
		/// <param name="index"></param>
		public VisualElement ElementAt(int index)
		{
			VisualElement visualElement;
			if (this.contentContainer == this)
			{
				visualElement = this.shadow.ElementAt(index);
			}
			else
			{
				visualElement = this.contentContainer.ElementAt(index);
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

		/// <summary>
		///   <para> Number of child elements in this object's contentContainer
		///           </para>
		/// </summary>
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
					num = this.contentContainer.childCount;
				}
				return num;
			}
		}

		/// <summary>
		///   <para>Retrieves the child index of the specified VisualElement.</para>
		/// </summary>
		/// <param name="element">The child to return the index for.</param>
		/// <returns>
		///   <para>Returns the index of the child, or -1 if the child is not found.</para>
		/// </returns>
		public int IndexOf(VisualElement element)
		{
			int num;
			if (this.contentContainer == this)
			{
				num = this.shadow.IndexOf(element);
			}
			else
			{
				num = this.contentContainer.IndexOf(element);
			}
			return num;
		}

		/// <summary>
		///   <para>Returns the elements from its contentContainer</para>
		/// </summary>
		public IEnumerable<VisualElement> Children()
		{
			IEnumerable<VisualElement> enumerable;
			if (this.contentContainer == this)
			{
				enumerable = this.shadow.Children();
			}
			else
			{
				enumerable = this.contentContainer.Children();
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
				this.contentContainer.Sort(comp);
			}
		}

		/// <summary>
		///   <para>Brings this element to the end of its parent children list. The element will be visually in front of any overlapping sibling elements.</para>
		/// </summary>
		public void BringToFront()
		{
			if (this.shadow.parent != null)
			{
				this.shadow.parent.shadow.BringToFront(this);
			}
		}

		/// <summary>
		///   <para>Sends this element to the beginning of its parent children list. The element will be visually behind any overlapping sibling elements.</para>
		/// </summary>
		public void SendToBack()
		{
			if (this.shadow.parent != null)
			{
				this.shadow.parent.shadow.SendToBack(this);
			}
		}

		/// <summary>
		///   <para>Places this element right before the sibling element in their parent children list. If the element and the sibling position overlap, the element will be visually behind of its sibling.</para>
		/// </summary>
		/// <param name="sibling">The sibling element.</param>
		public void PlaceBehind(VisualElement sibling)
		{
			if (this.shadow.parent == null || sibling.shadow.parent != this.shadow.parent)
			{
				throw new ArgumentException("VisualElements are not siblings");
			}
			this.shadow.parent.shadow.PlaceBehind(this, sibling);
		}

		/// <summary>
		///   <para>Places this element right after the sibling element in their parent children list. If the element and the sibling position overlap, the element will be visually in front of its sibling.</para>
		/// </summary>
		/// <param name="sibling">The sibling element.</param>
		public void PlaceInFront(VisualElement sibling)
		{
			if (this.shadow.parent == null || sibling.shadow.parent != this.shadow.parent)
			{
				throw new ArgumentException("VisualElements are not siblings");
			}
			this.shadow.parent.shadow.PlaceInFront(this, sibling);
		}

		/// <summary>
		///   <para>Removes this element from its parent hierarchy</para>
		/// </summary>
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

		/// <summary>
		///   <para>Returns true if the element is a direct child of this VisualElement</para>
		/// </summary>
		/// <param name="child"></param>
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

		/// <summary>
		///   <para>Finds the lowest commont ancestor between two VisualElements inside the VisualTree hierarchy</para>
		/// </summary>
		/// <param name="other"></param>
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

		/// <summary>
		///   <para>Allows to iterate into this elements children</para>
		/// </summary>
		public IEnumerator<VisualElement> GetEnumerator()
		{
			IEnumerator<VisualElement> enumerator;
			if (this.contentContainer == this)
			{
				enumerator = this.shadow.Children().GetEnumerator();
			}
			else
			{
				enumerator = this.contentContainer.GetEnumerator();
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
				enumerator = ((IEnumerable)this.contentContainer).GetEnumerator();
			}
			return enumerator;
		}

		/// <summary>
		///   <para>Retrieves this VisualElement's IVisualElementScheduler</para>
		/// </summary>
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

		/// <summary>
		///   <para>Reference to the style object of this element.</para>
		/// </summary>
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
					this.yogaNode.MinHeight = value.value;
				}
			}
		}

		StyleValue<float> IStyle.flex
		{
			get
			{
				return this.effectiveStyle.flex;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.flex, value))
				{
					this.Dirty(ChangeType.Layout);
					this.yogaNode.Flex = value.value;
				}
			}
		}

		StyleValue<float> IStyle.flexBasis
		{
			get
			{
				return this.effectiveStyle.flexBasis;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.flexBasis, value))
				{
					this.Dirty(ChangeType.Layout);
					this.yogaNode.FlexBasis = value.value;
				}
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					ChangeType changeType = this.changesNeeded;
					this.Dirty(ChangeType.Layout);
					if ((changeType & ChangeType.Repaint) == (ChangeType)0)
					{
						this.ClearDirty(ChangeType.Repaint);
					}
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
					ChangeType changeType = this.changesNeeded;
					this.Dirty(ChangeType.Layout);
					if ((changeType & ChangeType.Repaint) == (ChangeType)0)
					{
						this.ClearDirty(ChangeType.Repaint);
					}
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
					this.yogaNode.MarginBottom = value.value;
				}
			}
		}

		StyleValue<float> IStyle.borderLeft
		{
			get
			{
				return this.effectiveStyle.borderLeft;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderLeft, value))
				{
					this.Dirty(ChangeType.Layout);
					this.yogaNode.BorderLeftWidth = value.value;
				}
			}
		}

		StyleValue<float> IStyle.borderTop
		{
			get
			{
				return this.effectiveStyle.borderTop;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderTop, value))
				{
					this.Dirty(ChangeType.Layout);
					this.yogaNode.BorderTopWidth = value.value;
				}
			}
		}

		StyleValue<float> IStyle.borderRight
		{
			get
			{
				return this.effectiveStyle.borderRight;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderRight, value))
				{
					this.Dirty(ChangeType.Layout);
					this.yogaNode.BorderRightWidth = value.value;
				}
			}
		}

		StyleValue<float> IStyle.borderBottom
		{
			get
			{
				return this.effectiveStyle.borderBottom;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.borderBottom, value))
				{
					this.Dirty(ChangeType.Layout);
					this.yogaNode.BorderBottomWidth = value.value;
				}
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
					this.yogaNode.AlignSelf = (YogaAlign)value.value;
				}
			}
		}

		StyleValue<TextAnchor> IStyle.textAlignment
		{
			get
			{
				return new StyleValue<TextAnchor>((TextAnchor)this.effectiveStyle.textAlignment.value, this.effectiveStyle.textAlignment.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.textAlignment, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.Dirty(ChangeType.Repaint);
				}
			}
		}

		StyleValue<FontStyle> IStyle.fontStyle
		{
			get
			{
				return new StyleValue<FontStyle>((FontStyle)this.effectiveStyle.fontStyle.value, this.effectiveStyle.fontStyle.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.fontStyle, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Repaint);
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
				if (StyleValueUtils.ApplyAndCompare<Font>(ref this.inlineStyle.font, value))
				{
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
				}
			}
		}

		StyleValue<Color> IStyle.textColor
		{
			get
			{
				return this.effectiveStyle.textColor;
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.textColor, value))
				{
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Repaint);
				}
				else if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.backgroundColor, value))
				{
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Repaint);
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
				if (StyleValueUtils.ApplyAndCompare<Texture2D>(ref this.inlineStyle.backgroundImage, value))
				{
					this.Dirty(ChangeType.Repaint);
				}
			}
		}

		StyleValue<ScaleMode> IStyle.backgroundSize
		{
			get
			{
				return new StyleValue<ScaleMode>((ScaleMode)this.effectiveStyle.backgroundSize.value, this.effectiveStyle.backgroundSize.specificity);
			}
			set
			{
				if (StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.backgroundSize, new StyleValue<int>((int)value.value, value.specificity)))
				{
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Layout);
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
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Repaint);
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
					this.Dirty(ChangeType.Repaint);
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
				StyleValueUtils.ApplyAndCompare(ref this.inlineStyle.cursor, value);
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
					this.Dirty(ChangeType.Repaint);
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

		/// <summary>
		///   <para>Adds this stylesheet file to this element list of applied styles</para>
		/// </summary>
		/// <param name="sheetPath"></param>
		public void AddStyleSheetPath(string sheetPath)
		{
			if (this.m_StyleSheetPaths == null)
			{
				this.m_StyleSheetPaths = new List<string>();
			}
			this.m_StyleSheetPaths.Add(sheetPath);
			this.m_StyleSheets = null;
			this.Dirty(ChangeType.Styles);
		}

		/// <summary>
		///   <para>Removes this stylesheet file from this element list of applied styles</para>
		/// </summary>
		/// <param name="sheetPath"></param>
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
				this.Dirty(ChangeType.Styles);
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
					this.Dirty(ChangeType.Styles);
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
						int i = 0;
						int num = styleSheet.complexSelectors.Length;
						while (i < num)
						{
							styleSheet.complexSelectors[i].CachePseudoStateMasks();
							i++;
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

		/// <summary>
		///   <para>The default focus index for newly created elements.</para>
		/// </summary>
		public static readonly int defaultFocusIndex = -1;

		private static uint s_NextId;

		private string m_Name;

		private HashSet<string> m_ClassList;

		private string m_TypeName;

		private string m_FullTypeName;

		private string m_PersistenceKey;

		private RenderData m_RenderData;

		private Vector3 m_Position = Vector3.zero;

		private Quaternion m_Rotation = Quaternion.identity;

		private Vector3 m_Scale = Vector3.one;

		private Rect m_Layout;

		internal PseudoStates triggerPseudoMask;

		internal PseudoStates dependencyPseudoMask;

		private PseudoStates m_PseudoStates;

		internal VisualElementStylesData m_SharedStyle = VisualElementStylesData.none;

		internal VisualElementStylesData m_Style = VisualElementStylesData.none;

		internal readonly uint controlid;

		private ChangeType changesNeeded;

		private bool m_Enabled;

		private bool m_RequireMeasureFunction = false;

		internal const Align DefaultAlignContent = Align.FlexStart;

		internal const Align DefaultAlignItems = Align.Stretch;

		private VisualElement.ClippingOptions m_ClippingOptions;

		private VisualElement m_PhysicalParent;

		private VisualElement m_LogicalParent;

		private static readonly VisualElement[] s_EmptyList = new VisualElement[0];

		private List<VisualElement> m_Children;

		private List<StyleSheet> m_StyleSheets;

		private List<string> m_StyleSheetPaths;

		/// <summary>
		///   <para>Instantiates a VisualElement using the data read from a UXML file.</para>
		/// </summary>
		public class VisualElementFactory : UxmlFactory<VisualElement, VisualElement.VisualElementUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the VisualElement.</para>
		/// </summary>
		public class VisualElementUxmlTraits : UxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public VisualElementUxmlTraits()
			{
				this.m_Name = new UxmlStringAttributeDescription
				{
					name = "name"
				};
				this.m_PickingMode = new UxmlEnumAttributeDescription<PickingMode>
				{
					name = "pickingMode"
				};
				this.m_FocusIndex = new UxmlIntAttributeDescription
				{
					name = "focusIndex",
					defaultValue = VisualElement.defaultFocusIndex
				};
			}

			/// <summary>
			///   <para>Returns an enumerable containing attribute descriptions for VisualElement properties that should be available in UXML.</para>
			/// </summary>
			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_Name;
					yield return this.m_PickingMode;
					yield return this.m_FocusIndex;
					yield break;
				}
			}

			/// <summary>
			///   <para>Returns an enumerable containing UxmlChildElementDescription(typeof(VisualElement)), since VisualElements can contain other VisualElements.</para>
			/// </summary>
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield return new UxmlChildElementDescription(typeof(VisualElement));
					yield break;
				}
			}

			/// <summary>
			///   <para>Initialize VisualElement properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				ve.name = this.m_Name.GetValueFromBag(bag);
				ve.pickingMode = this.m_PickingMode.GetValueFromBag(bag);
				ve.focusIndex = this.m_FocusIndex.GetValueFromBag(bag);
			}

			private UxmlStringAttributeDescription m_Name;

			private UxmlEnumAttributeDescription<PickingMode> m_PickingMode;

			protected UxmlIntAttributeDescription m_FocusIndex;
		}

		/// <summary>
		///   <para>The modes available to measure VisualElement sizes.</para>
		/// </summary>
		public enum MeasureMode
		{
			/// <summary>
			///   <para>The element should give its preferred width/height without any constraint.</para>
			/// </summary>
			Undefined,
			/// <summary>
			///   <para>The element should give the width/height that is passed in and derive the opposite site from this value (for example, calculate text size from a fixed width).</para>
			/// </summary>
			Exactly,
			/// <summary>
			///   <para>At Most. The element should give its preferred width/height but no more than the value passed.</para>
			/// </summary>
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

		/// <summary>
		///   <para>Options to select clipping strategy.</para>
		/// </summary>
		public enum ClippingOptions
		{
			/// <summary>
			///   <para>Will enable clipping. This VisualElement and its children's content will be limited to this element's bounds.</para>
			/// </summary>
			ClipContents,
			/// <summary>
			///   <para>Will disable clipping and let children VisualElements paint outside its bounds.</para>
			/// </summary>
			NoClipping,
			/// <summary>
			///   <para>Enables clipping and renders contents to a cache texture.</para>
			/// </summary>
			ClipAndCacheContents
		}

		/// <summary>
		///   <para>Hierarchy is a sctuct allowing access to the shadow hierarchy of visual elements</para>
		/// </summary>
		public struct Hierarchy
		{
			internal Hierarchy(VisualElement element)
			{
				this.m_Owner = element;
			}

			/// <summary>
			///   <para> Access the physical parent of this element in the hierarchy
			///           </para>
			/// </summary>
			public VisualElement parent
			{
				get
				{
					return this.m_Owner.m_PhysicalParent;
				}
			}

			/// <summary>
			///   <para>Add an element to this element's contentContainer</para>
			/// </summary>
			/// <param name="child"></param>
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
				child.Dirty(ChangeType.Styles);
				child.Dirty(ChangeType.Transform);
				this.m_Owner.Dirty(ChangeType.Layout);
				if (!string.IsNullOrEmpty(child.persistenceKey))
				{
					child.Dirty(ChangeType.PersistentData);
				}
			}

			/// <summary>
			///   <para>Removes this child from the hierarchy</para>
			/// </summary>
			/// <param name="child"></param>
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

			/// <summary>
			///   <para>Remove the child element located at this position from this element's contentContainer</para>
			/// </summary>
			/// <param name="index"></param>
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
				this.m_Owner.Dirty(ChangeType.Layout);
			}

			/// <summary>
			///   <para>Remove all child elements from this element's contentContainer</para>
			/// </summary>
			public void Clear()
			{
				if (this.childCount > 0)
				{
					foreach (VisualElement visualElement in this.m_Owner.m_Children)
					{
						visualElement.shadow.SetParent(null);
						visualElement.m_LogicalParent = null;
					}
					this.ReleaseChildList();
					this.m_Owner.yogaNode.Clear();
					if (this.m_Owner.requireMeasureFunction)
					{
						this.m_Owner.yogaNode.SetMeasureFunction(new MeasureFunction(this.m_Owner.Measure));
					}
					this.m_Owner.Dirty(ChangeType.Layout);
				}
			}

			internal void BringToFront(VisualElement child)
			{
				if (this.childCount > 1)
				{
					int num = this.m_Owner.m_Children.IndexOf(child);
					if (num >= 0 && num < this.childCount - 1)
					{
						this.RemoveChildAtIndex(num);
						this.PutChildAtIndex(child, this.childCount);
						this.m_Owner.Dirty(ChangeType.Layout);
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
						this.RemoveChildAtIndex(num);
						this.PutChildAtIndex(child, 0);
						this.m_Owner.Dirty(ChangeType.Layout);
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
						this.RemoveChildAtIndex(num);
						num = this.m_Owner.m_Children.IndexOf(over);
						if (num < 0)
						{
							num = 0;
						}
						this.PutChildAtIndex(child, num);
						this.m_Owner.Dirty(ChangeType.Layout);
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
						this.RemoveChildAtIndex(num);
						num = this.m_Owner.m_Children.IndexOf(under) + 1;
						this.PutChildAtIndex(child, num);
						this.m_Owner.Dirty(ChangeType.Layout);
					}
				}
			}

			/// <summary>
			///   <para> Number of child elements in this object's contentContainer
			///           </para>
			/// </summary>
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

			/// <summary>
			///   <para>Retrieves the index of the specified VisualElement in the Hierarchy.</para>
			/// </summary>
			/// <param name="element">The element to return the index for.</param>
			/// <returns>
			///   <para>Returns the index of the element, or -1 if the element is not found.</para>
			/// </returns>
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

			/// <summary>
			///   <para>Retrieves the child element at position</para>
			/// </summary>
			/// <param name="index"></param>
			public VisualElement ElementAt(int index)
			{
				if (this.m_Owner.m_Children != null)
				{
					return this.m_Owner.m_Children[index];
				}
				throw new IndexOutOfRangeException("Index out of range: " + index);
			}

			/// <summary>
			///   <para>Returns the elements from its contentContainer</para>
			/// </summary>
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
					this.m_Owner.PropagateChangesToParents();
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
					this.m_Owner.Dirty(ChangeType.Layout);
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
