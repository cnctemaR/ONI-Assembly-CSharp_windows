using System;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.UIElements
{
	internal class Panel : BaseVisualElementPanel
	{
		public Panel(ScriptableObject ownerObject, ContextType contextType, IDataWatchService dataWatch = null, IEventDispatcher dispatcher = null)
		{
			this.ownerObject = ownerObject;
			this.contextType = contextType;
			this.m_DataWatch = dataWatch;
			this.dispatcher = dispatcher;
			this.stylePainter = new StylePainter();
			this.cursorManager = new CursorManager();
			this.contextualMenuManager = null;
			this.m_RootContainer = new VisualElement();
			this.m_RootContainer.name = VisualElementUtils.GetUniqueName("PanelContainer");
			this.m_RootContainer.persistenceKey = "PanelContainer";
			this.visualTree.SetPanel(this);
			this.focusController = new FocusController(new VisualElementFocusRing(this.visualTree, VisualElementFocusRing.DefaultFocusOrder.ChildOrder));
			this.m_StyleContext = new StyleContext(this.m_RootContainer);
			this.allowPixelCaching = true;
		}

		public override VisualElement visualTree
		{
			get
			{
				return this.m_RootContainer;
			}
		}

		public override IEventDispatcher dispatcher { get; protected set; }

		internal override IDataWatchService dataWatch
		{
			get
			{
				return this.m_DataWatch;
			}
		}

		public TimerEventScheduler timerEventScheduler
		{
			get
			{
				TimerEventScheduler timerEventScheduler;
				if ((timerEventScheduler = this.m_Scheduler) == null)
				{
					timerEventScheduler = (this.m_Scheduler = new TimerEventScheduler());
				}
				return timerEventScheduler;
			}
		}

		internal override IScheduler scheduler
		{
			get
			{
				return this.timerEventScheduler;
			}
		}

		internal StyleContext styleContext
		{
			get
			{
				return this.m_StyleContext;
			}
		}

		public override ScriptableObject ownerObject { get; protected set; }

		public bool allowPixelCaching { get; set; }

		public override ContextType contextType { get; protected set; }

		public override SavePersistentViewData savePersistentViewData { get; set; }

		public override GetViewDataDictionary getViewDataDictionary { get; set; }

		public override FocusController focusController { get; set; }

		public override EventInterests IMGUIEventInterests { get; set; }

		internal static TimeMsFunction TimeSinceStartup
		{
			get
			{
				return Panel.s_TimeSinceStartup;
			}
			set
			{
				if (value == null)
				{
					value = new TimeMsFunction(Panel.DefaultTimeSinceStartupMs);
				}
				Panel.s_TimeSinceStartup = value;
			}
		}

		public override bool keepPixelCacheOnWorldBoundChange
		{
			get
			{
				return this.m_KeepPixelCacheOnWorldBoundChange;
			}
			set
			{
				if (this.m_KeepPixelCacheOnWorldBoundChange != value)
				{
					this.m_KeepPixelCacheOnWorldBoundChange = value;
					if (!value)
					{
						this.m_RootContainer.Dirty(ChangeType.Transform | ChangeType.Repaint);
					}
				}
			}
		}

		public override int IMGUIContainersCount { get; set; }

		public static long TimeSinceStartupMs()
		{
			return (Panel.s_TimeSinceStartup != null) ? Panel.s_TimeSinceStartup() : Panel.DefaultTimeSinceStartupMs();
		}

		internal static long DefaultTimeSinceStartupMs()
		{
			return (long)(Time.realtimeSinceStartup * 1000f);
		}

		private VisualElement PickAll(VisualElement root, Vector2 point, List<VisualElement> picked = null)
		{
			return this.PerformPick(root, point, picked);
		}

		private VisualElement PerformPick(VisualElement root, Vector2 point, List<VisualElement> picked = null)
		{
			VisualElement visualElement;
			if (!root.visible)
			{
				visualElement = null;
			}
			else if (root.pickingMode == PickingMode.Ignore && root.shadow.childCount == 0)
			{
				visualElement = null;
			}
			else
			{
				Vector3 vector = root.WorldToLocal(point);
				bool flag = root.ContainsPoint(vector);
				if (!flag && root.clippingOptions != VisualElement.ClippingOptions.NoClipping)
				{
					visualElement = null;
				}
				else
				{
					VisualElement visualElement2 = null;
					for (int i = root.shadow.childCount - 1; i >= 0; i--)
					{
						VisualElement visualElement3 = root.shadow[i];
						VisualElement visualElement4 = this.PerformPick(visualElement3, point, picked);
						if (visualElement2 == null && visualElement4 != null)
						{
							visualElement2 = visualElement4;
						}
					}
					if (picked != null && root.enabledInHierarchy && root.pickingMode == PickingMode.Position && flag)
					{
						picked.Add(root);
					}
					if (visualElement2 != null)
					{
						visualElement = visualElement2;
					}
					else
					{
						PickingMode pickingMode = root.pickingMode;
						if (pickingMode != PickingMode.Position)
						{
							if (pickingMode != PickingMode.Ignore)
							{
							}
						}
						else if (flag && root.enabledInHierarchy)
						{
							return root;
						}
						visualElement = null;
					}
				}
			}
			return visualElement;
		}

		public override VisualElement LoadTemplate(string path, Dictionary<string, VisualElement> slots = null)
		{
			VisualTreeAsset visualTreeAsset = Panel.loadResourceFunc(path, typeof(VisualTreeAsset)) as VisualTreeAsset;
			VisualElement visualElement;
			if (visualTreeAsset == null)
			{
				visualElement = null;
			}
			else
			{
				visualElement = visualTreeAsset.CloneTree(slots);
			}
			return visualElement;
		}

		public override VisualElement PickAll(Vector2 point, List<VisualElement> picked)
		{
			this.ValidateLayout();
			if (picked != null)
			{
				picked.Clear();
			}
			return this.PickAll(this.visualTree, point, picked);
		}

		public override VisualElement Pick(Vector2 point)
		{
			return this.PickAll(this.visualTree, point, null);
		}

		private void ValidatePersistentData()
		{
			int num = 0;
			while (this.visualTree.AnyDirty(ChangeType.PersistentData | ChangeType.PersistentDataPath))
			{
				this.ValidatePersistentDataOnSubTree(this.visualTree, true);
				num++;
				if (num > 5)
				{
					Debug.LogError("UIElements: Too many children recursively added that rely on persistent data: " + this.visualTree);
					break;
				}
			}
		}

		private void ValidatePersistentDataOnSubTree(VisualElement root, bool enablePersistence)
		{
			if (!root.IsPersitenceSupportedOnChildren())
			{
				enablePersistence = false;
			}
			if (root.IsDirty(ChangeType.PersistentData))
			{
				root.OnPersistentDataReady(enablePersistence);
				root.ClearDirty(ChangeType.PersistentData);
			}
			if (root.IsDirty(ChangeType.PersistentDataPath))
			{
				for (int i = 0; i < root.shadow.childCount; i++)
				{
					this.ValidatePersistentDataOnSubTree(root.shadow[i], enablePersistence);
				}
				root.ClearDirty(ChangeType.PersistentDataPath);
			}
		}

		private void ValidateStyling()
		{
			if (this.m_RootContainer.AnyDirty(ChangeType.Styles | ChangeType.StylesPath))
			{
				this.m_StyleContext.ApplyStyles();
			}
		}

		public override void ValidateLayout()
		{
			this.ValidateStyling();
			int num = 0;
			while (this.visualTree.yogaNode.IsDirty)
			{
				this.visualTree.yogaNode.CalculateLayout(float.NaN, float.NaN);
				this.ValidateSubTree(this.visualTree);
				if (num++ >= 5)
				{
					Debug.LogError("ValidateLayout is struggling to process current layout (consider simplifying to avoid recursive layout): " + this.visualTree);
					break;
				}
			}
			if (this.hasDirtyTransform)
			{
				this.hasDirtyTransform = false;
				EventDispatcher eventDispatcher = UIElementsUtility.eventDispatcher as EventDispatcher;
				if (eventDispatcher != null)
				{
					eventDispatcher.UpdateElementUnderMouse(this);
				}
			}
		}

		private void ValidateSubTree(VisualElement root)
		{
			Rect rect = new Rect(root.yogaNode.LayoutX, root.yogaNode.LayoutY, root.yogaNode.LayoutWidth, root.yogaNode.LayoutHeight);
			Rect lastLayout = root.renderData.lastLayout;
			bool flag = lastLayout != rect;
			if (flag)
			{
				if (rect.position != lastLayout.position)
				{
					root.Dirty(ChangeType.Transform);
				}
				root.renderData.lastLayout = rect;
			}
			bool hasNewLayout = root.yogaNode.HasNewLayout;
			if (hasNewLayout)
			{
				for (int i = 0; i < root.shadow.childCount; i++)
				{
					this.ValidateSubTree(root.shadow[i]);
				}
			}
			if (flag)
			{
				using (GeometryChangedEvent pooled = GeometryChangedEvent.GetPooled(lastLayout, rect))
				{
					pooled.target = root;
					UIElementsUtility.eventDispatcher.DispatchEvent(pooled, this);
				}
			}
			root.ClearDirty(ChangeType.Layout);
			if (hasNewLayout)
			{
				root.yogaNode.MarkLayoutSeen();
			}
		}

		private Rect ComputeAAAlignedBound(Rect position, Matrix4x4 mat)
		{
			Rect rect = position;
			Vector3 vector = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y, 0f));
			Vector3 vector2 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y, 0f));
			Vector3 vector3 = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y + rect.height, 0f));
			Vector3 vector4 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y + rect.height, 0f));
			return Rect.MinMaxRect(Mathf.Min(vector.x, Mathf.Min(vector2.x, Mathf.Min(vector3.x, vector4.x))), Mathf.Min(vector.y, Mathf.Min(vector2.y, Mathf.Min(vector3.y, vector4.y))), Mathf.Max(vector.x, Mathf.Max(vector2.x, Mathf.Max(vector3.x, vector4.x))), Mathf.Max(vector.y, Mathf.Max(vector2.y, Mathf.Max(vector3.y, vector4.y))));
		}

		private bool ShouldUsePixelCache(VisualElement root)
		{
			return this.allowPixelCaching && root.clippingOptions == VisualElement.ClippingOptions.ClipAndCacheContents && root.worldBound.size.magnitude > Mathf.Epsilon;
		}

		private bool DoesMatrixHaveUnsupportedRotation(Matrix4x4 m)
		{
			Func<float, bool> func = (float f) => Math.Abs(f) < 0.0001f;
			for (int i = 0; i < 3; i++)
			{
				int num = 0;
				num += ((!func(m[0, i])) ? 0 : 1);
				num += ((!func(m[1, i])) ? 0 : 1);
				num += ((!func(m[2, i])) ? 0 : 1);
				if (num < 2)
				{
					return true;
				}
			}
			return false;
		}

		private void PaintSubTree(Event e, VisualElement root, Matrix4x4 offset, VisualElement.ClippingOptions clippingOption, Rect currentGlobalClip)
		{
			if (root != null && root.panel == this)
			{
				if (root.visible && root.style.opacity.GetSpecifiedValueOrDefault(1f) >= Mathf.Epsilon)
				{
					if (root.clippingOptions != VisualElement.ClippingOptions.NoClipping)
					{
						Rect rect = this.ComputeAAAlignedBound(root.rect, offset * root.worldTransform);
						if (!rect.Overlaps(currentGlobalClip))
						{
							return;
						}
						float num = Mathf.Max(rect.x, currentGlobalClip.x);
						float num2 = Mathf.Min(rect.xMax, currentGlobalClip.xMax);
						float num3 = Mathf.Max(rect.y, currentGlobalClip.y);
						float num4 = Mathf.Min(rect.yMax, currentGlobalClip.yMax);
						currentGlobalClip = new Rect(num, num3, num2 - num, num4 - num3);
						clippingOption = root.clippingOptions;
					}
					if (!this.whinedOnceAboutRotatedClipSpaceThisFrame && clippingOption == VisualElement.ClippingOptions.ClipContents && this.DoesMatrixHaveUnsupportedRotation(root.worldTransform))
					{
						Debug.LogError("Panel.PaintSubTree - Rotated clip-spaces are only supported by the VisualElement.ClippingOptions.ClipAndCacheContents mode. First offending Panel:'" + root.name + "'.");
						this.whinedOnceAboutRotatedClipSpaceThisFrame = true;
					}
					if (this.ShouldUsePixelCache(root))
					{
						IStylePainter stylePainter = this.stylePainter;
						Rect worldBound = root.worldBound;
						stylePainter.currentWorldClip = currentGlobalClip;
						stylePainter.currentTransform = offset * root.worldTransform;
						int num5;
						int num6;
						Rect rect2;
						using (new GUIClip.ParentClipScope(stylePainter.currentTransform, currentGlobalClip))
						{
							rect2 = GUIUtility.AlignRectToDevice(root.rect, out num5, out num6);
						}
						num5 = Math.Max(num5, 1);
						num6 = Math.Max(num6, 1);
						RenderTexture renderTexture = root.renderData.pixelCache;
						if (renderTexture != null && (renderTexture.width != num5 || renderTexture.height != num6) && (!this.keepPixelCacheOnWorldBoundChange || root.IsDirty(ChangeType.Repaint)))
						{
							Object.DestroyImmediate(renderTexture);
							renderTexture = (root.renderData.pixelCache = null);
						}
						if (root.IsDirty(ChangeType.Repaint) || root.renderData.pixelCache == null || !root.renderData.pixelCache.IsCreated())
						{
							if (renderTexture == null)
							{
								renderTexture = (root.renderData.pixelCache = new RenderTexture(num5, num6, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB));
							}
							bool flag = root.style.borderTopLeftRadius > 0f || root.style.borderTopRightRadius > 0f || root.style.borderBottomLeftRadius > 0f || root.style.borderBottomRightRadius > 0f;
							RenderTexture renderTexture2 = null;
							RenderTexture active = RenderTexture.active;
							try
							{
								if (flag)
								{
									renderTexture = (renderTexture2 = RenderTexture.GetTemporary(num5, num6, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB));
								}
								RenderTexture.active = renderTexture;
								GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
								Rect rect3 = root.LocalToWorld(rect2);
								Matrix4x4 matrix4x = Matrix4x4.Translate(new Vector3(-rect3.x, -rect3.y, 0f));
								Matrix4x4 matrix4x2 = matrix4x * root.worldTransform;
								Rect rect4 = new Rect(0f, 0f, rect3.width, rect3.height);
								stylePainter.currentTransform = matrix4x2;
								bool flag2 = SystemInfo.graphicsDeviceType != GraphicsDeviceType.Metal;
								using (new GUIUtility.ManualTex2SRGBScope(flag2))
								{
									using (new GUIClip.ParentClipScope(stylePainter.currentTransform, rect4))
									{
										stylePainter.currentWorldClip = rect4;
										this.stylePainter.opacity = root.style.opacity.GetSpecifiedValueOrDefault(1f);
										root.DoRepaint(stylePainter);
										this.stylePainter.opacity = 1f;
										root.ClearDirty(ChangeType.Repaint);
										this.PaintSubTreeChildren(e, root, matrix4x, clippingOption, rect4);
									}
								}
								if (flag)
								{
									RenderTexture.active = root.renderData.pixelCache;
									stylePainter.currentTransform = Matrix4x4.identity;
									using (new GUIUtility.ManualTex2SRGBScope(flag2))
									{
										using (new GUIClip.ParentClipScope(stylePainter.currentTransform, rect4))
										{
											GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
											TextureStylePainterParameters defaultTextureParameters = stylePainter.GetDefaultTextureParameters(root);
											defaultTextureParameters.texture = renderTexture;
											defaultTextureParameters.scaleMode = ScaleMode.StretchToFill;
											defaultTextureParameters.rect = rect4;
											defaultTextureParameters.border.SetWidth(0f);
											Vector4 vector = new Vector4(1f, 0f, 0f, 0f);
											float x = (matrix4x2 * vector).x;
											defaultTextureParameters.border.SetRadius(defaultTextureParameters.border.topLeftRadius * x, defaultTextureParameters.border.topRightRadius * x, defaultTextureParameters.border.bottomRightRadius * x, defaultTextureParameters.border.bottomLeftRadius * x);
											defaultTextureParameters.usePremultiplyAlpha = true;
											stylePainter.DrawTexture(defaultTextureParameters);
										}
									}
									stylePainter.currentTransform = matrix4x2;
									using (new GUIUtility.ManualTex2SRGBScope(flag2))
									{
										using (new GUIClip.ParentClipScope(stylePainter.currentTransform, rect4))
										{
											stylePainter.DrawBorder(root);
										}
									}
								}
							}
							finally
							{
								renderTexture = null;
								if (renderTexture2 != null)
								{
									RenderTexture.ReleaseTemporary(renderTexture2);
								}
								RenderTexture.active = active;
							}
						}
						stylePainter.currentWorldClip = currentGlobalClip;
						stylePainter.currentTransform = offset * root.worldTransform;
						using (new GUIClip.ParentClipScope(stylePainter.currentTransform, currentGlobalClip))
						{
							TextureStylePainterParameters textureStylePainterParameters = new TextureStylePainterParameters
							{
								rect = GUIUtility.AlignRectToDevice(root.rect),
								uv = new Rect(0f, 0f, 1f, 1f),
								texture = root.renderData.pixelCache,
								color = Color.white,
								scaleMode = ScaleMode.StretchToFill,
								usePremultiplyAlpha = true
							};
							stylePainter.DrawTexture(textureStylePainterParameters);
						}
					}
					else
					{
						this.stylePainter.currentTransform = offset * root.worldTransform;
						using (new GUIClip.ParentClipScope(this.stylePainter.currentTransform, currentGlobalClip))
						{
							this.stylePainter.currentWorldClip = currentGlobalClip;
							this.stylePainter.mousePosition = root.worldTransform.inverse.MultiplyPoint3x4(e.mousePosition);
							this.stylePainter.opacity = root.style.opacity.GetSpecifiedValueOrDefault(1f);
							root.DoRepaint(this.stylePainter);
							this.stylePainter.opacity = 1f;
							root.ClearDirty(ChangeType.Repaint);
							this.PaintSubTreeChildren(e, root, offset, clippingOption, currentGlobalClip);
						}
					}
				}
			}
		}

		private void PaintSubTreeChildren(Event e, VisualElement root, Matrix4x4 offset, VisualElement.ClippingOptions clippingOption, Rect textureClip)
		{
			int childCount = root.shadow.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = root.shadow[i];
				this.PaintSubTree(e, visualElement, offset, clippingOption, textureClip);
				if (childCount != root.shadow.childCount)
				{
					throw new NotImplementedException("Visual tree is read-only during repaint");
				}
			}
		}

		public override void Repaint(Event e)
		{
			Debug.Assert(GUIClip.Internal_GetCount() == 0, "UIElement is not compatible with IMGUI GUIClips, only GUIClip.ParentClipScope");
			if (!Mathf.Approximately(this.m_StyleContext.currentPixelsPerPoint, GUIUtility.pixelsPerPoint))
			{
				this.m_RootContainer.Dirty(ChangeType.Styles);
				this.m_StyleContext.currentPixelsPerPoint = GUIUtility.pixelsPerPoint;
			}
			this.ValidatePersistentData();
			this.ValidateLayout();
			this.stylePainter.repaintEvent = e;
			Rect rect = ((this.visualTree.clippingOptions == VisualElement.ClippingOptions.NoClipping) ? GUIClip.topmostRect : this.visualTree.layout);
			this.whinedOnceAboutRotatedClipSpaceThisFrame = false;
			this.PaintSubTree(e, this.visualTree, Matrix4x4.identity, VisualElement.ClippingOptions.NoClipping, rect);
		}

		private StyleContext m_StyleContext;

		private VisualElement m_RootContainer;

		private IDataWatchService m_DataWatch;

		private TimerEventScheduler m_Scheduler;

		internal static LoadResourceFunction loadResourceFunc = null;

		private static TimeMsFunction s_TimeSinceStartup;

		private bool m_KeepPixelCacheOnWorldBoundChange;

		private const int kMaxValidatePersistentDataCount = 5;

		private const int kMaxValidateLayoutCount = 5;

		private bool whinedOnceAboutRotatedClipSpaceThisFrame = false;
	}
}
