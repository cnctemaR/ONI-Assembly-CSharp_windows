using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	[HelpURL("UIE-Runtime-Panel-Settings")]
	public class PanelSettings : ScriptableObject
	{
		public ThemeStyleSheet themeStyleSheet
		{
			get
			{
				return this.themeUss;
			}
			set
			{
				this.themeUss = value;
				this.ApplyThemeStyleSheet(null);
			}
		}

		internal bool disableNoThemeWarning
		{
			get
			{
				return this.m_DisableNoThemeWarning;
			}
			set
			{
				this.m_DisableNoThemeWarning = value;
			}
		}

		public RenderTexture targetTexture
		{
			get
			{
				return this.m_TargetTexture;
			}
			set
			{
				this.m_TargetTexture = value;
				this.m_PanelAccess.SetTargetTexture();
			}
		}

		internal PanelRenderMode renderMode
		{
			get
			{
				return this.m_RenderMode;
			}
			set
			{
				this.m_RenderMode = value;
			}
		}

		internal ColliderUpdateMode colliderUpdateMode
		{
			get
			{
				return this.m_ColliderUpdateMode;
			}
			set
			{
				this.m_ColliderUpdateMode = value;
			}
		}

		internal bool colliderIsTrigger
		{
			get
			{
				return this.m_ColliderIsTrigger;
			}
			set
			{
				this.m_ColliderIsTrigger = value;
			}
		}

		public PanelScaleMode scaleMode
		{
			get
			{
				return this.m_ScaleMode;
			}
			set
			{
				this.m_ScaleMode = value;
			}
		}

		public float referenceSpritePixelsPerUnit
		{
			get
			{
				return this.m_ReferenceSpritePixelsPerUnit;
			}
			set
			{
				this.m_ReferenceSpritePixelsPerUnit = value;
			}
		}

		internal float pixelsPerUnit
		{
			get
			{
				return this.m_PixelsPerUnit;
			}
			set
			{
				this.m_PixelsPerUnit = value;
			}
		}

		public float scale
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				this.m_Scale = value;
			}
		}

		public float referenceDpi
		{
			get
			{
				return this.m_ReferenceDpi;
			}
			set
			{
				this.m_ReferenceDpi = ((value >= 1f) ? value : 96f);
			}
		}

		public float fallbackDpi
		{
			get
			{
				return this.m_FallbackDpi;
			}
			set
			{
				this.m_FallbackDpi = ((value >= 1f) ? value : 96f);
			}
		}

		public Vector2Int referenceResolution
		{
			get
			{
				return this.m_ReferenceResolution;
			}
			set
			{
				this.m_ReferenceResolution = value;
			}
		}

		public PanelScreenMatchMode screenMatchMode
		{
			get
			{
				return this.m_ScreenMatchMode;
			}
			set
			{
				this.m_ScreenMatchMode = value;
			}
		}

		public float match
		{
			get
			{
				return this.m_Match;
			}
			set
			{
				this.m_Match = value;
			}
		}

		public float sortingOrder
		{
			get
			{
				return this.m_SortingOrder;
			}
			set
			{
				this.m_SortingOrder = value;
				this.ApplySortingOrder();
			}
		}

		internal void ApplySortingOrder()
		{
			this.m_PanelAccess.SetSortingPriority();
		}

		public int targetDisplay
		{
			get
			{
				return this.m_TargetDisplay;
			}
			set
			{
				this.m_TargetDisplay = value;
				this.m_PanelAccess.SetTargetDisplay();
			}
		}

		public BindingLogLevel bindingLogLevel
		{
			get
			{
				return this.m_BindingLogLevel;
			}
			set
			{
				bool flag = this.m_BindingLogLevel == value;
				if (!flag)
				{
					this.m_BindingLogLevel = value;
					Binding.SetPanelLogLevel(this.panel, value);
				}
			}
		}

		public bool clearDepthStencil
		{
			get
			{
				return this.m_ClearDepthStencil;
			}
			set
			{
				this.m_ClearDepthStencil = value;
			}
		}

		public float depthClearValue
		{
			get
			{
				return 0.99f;
			}
		}

		public bool clearColor
		{
			get
			{
				return this.m_ClearColor;
			}
			set
			{
				this.m_ClearColor = value;
			}
		}

		public Color colorClearValue
		{
			get
			{
				return this.m_ColorClearValue;
			}
			set
			{
				this.m_ColorClearValue = value;
			}
		}

		public uint vertexBudget
		{
			get
			{
				return this.m_VertexBudget;
			}
			set
			{
				this.m_VertexBudget = value;
			}
		}

		public TextureSlotCount textureSlotCount
		{
			get
			{
				return this.m_TextureSlotCount;
			}
			set
			{
				this.m_TextureSlotCount = value;
			}
		}

		internal BaseRuntimePanel panel
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
			get
			{
				return this.m_PanelAccess.panel;
			}
		}

		internal bool isInitialized
		{
			get
			{
				PanelSettings.RuntimePanelAccess panelAccess = this.m_PanelAccess;
				return panelAccess != null && panelAccess.isInitialized;
			}
		}

		internal bool isTransient
		{
			get
			{
				return this.m_PanelAccess.isTransient;
			}
			set
			{
				this.m_PanelAccess.isTransient = value;
			}
		}

		internal VisualElement visualTree
		{
			get
			{
				return this.m_PanelAccess.panel.visualTree;
			}
		}

		public DynamicAtlasSettings dynamicAtlasSettings
		{
			get
			{
				return this.m_DynamicAtlasSettings;
			}
			set
			{
				this.m_DynamicAtlasSettings = value;
			}
		}

		private PanelSettings()
		{
			this.m_PanelAccess = new PanelSettings.RuntimePanelAccess(this);
		}

		private void Reset()
		{
		}

		private void OnEnable()
		{
			this.UpdateScreenDPI();
			this.InitializeShaders();
			this.AssignICUData();
		}

		private void OnDisable()
		{
			this.m_PanelAccess.DisposePanel();
		}

		internal void DisposePanel()
		{
			this.m_PanelAccess.DisposePanel();
		}

		private float ScreenDPI { get; set; }

		[Conditional("ENABLE_PROFILER")]
		public void SetPanelChangeReceiver(IDebugPanelChangeReceiver value)
		{
			this.m_PanelChangeReceiver = value;
			this.m_PanelAccess.SetPanelChangeReceiver();
		}

		internal IDebugPanelChangeReceiver GetPanelChangeReceiver()
		{
			return this.m_PanelChangeReceiver;
		}

		internal void UpdateScreenDPI()
		{
			this.ScreenDPI = Screen.dpi;
		}

		private void ApplyThemeStyleSheet(VisualElement root = null)
		{
			bool flag = !this.m_PanelAccess.isInitialized;
			if (!flag)
			{
				bool flag2 = root == null;
				if (flag2)
				{
					root = this.visualTree;
				}
				bool flag3 = this.m_OldThemeUss != this.themeUss && this.m_OldThemeUss != null;
				if (flag3)
				{
					if (root != null)
					{
						root.styleSheets.Remove(this.m_OldThemeUss);
					}
				}
				bool flag4 = this.themeUss != null;
				if (flag4)
				{
					this.themeUss.isDefaultStyleSheet = true;
					if (root != null)
					{
						root.styleSheets.Add(this.themeUss);
					}
				}
				else
				{
					bool flag5 = !this.m_DisableNoThemeWarning;
					if (flag5)
					{
						Debug.LogWarning("No Theme Style Sheet set to PanelSettings " + base.name + ", UI will not render properly", this);
					}
				}
				this.m_OldThemeUss = this.themeUss;
			}
		}

		internal bool AssignICUData()
		{
			return false;
		}

		private void InitializeShaders()
		{
			bool flag = this.m_AtlasBlitShader == null;
			if (flag)
			{
				this.m_AtlasBlitShader = Shader.Find(Shaders.k_AtlasBlit);
			}
			bool flag2 = this.m_DefaultShader == null;
			if (flag2)
			{
				this.m_DefaultShader = Shader.Find(Shaders.k_Default);
			}
			bool flag3 = this.m_RuntimeGaussianBlurShader == null;
			if (flag3)
			{
				this.m_RuntimeGaussianBlurShader = Shader.Find(Shaders.k_RuntimeGaussianBlur);
			}
			bool flag4 = this.m_RuntimeColorEffectShader == null;
			if (flag4)
			{
				this.m_RuntimeColorEffectShader = Shader.Find(Shaders.k_RuntimeColorEffect);
			}
			bool flag5 = this.m_SDFShader == null;
			if (flag5)
			{
				this.m_SDFShader = Shader.Find(TextShaderUtilities.k_SDFText);
			}
			bool flag6 = this.m_BitmapShader == null;
			if (flag6)
			{
				this.m_BitmapShader = Shader.Find(TextShaderUtilities.k_BitmapText);
			}
			bool flag7 = this.m_SpriteShader == null;
			if (flag7)
			{
				this.m_SpriteShader = Shader.Find(TextShaderUtilities.k_SpriteText);
			}
			this.m_PanelAccess.SetTargetTexture();
		}

		internal void ApplyPanelSettings()
		{
			Rect targetRect = this.m_TargetRect;
			float resolvedScale = this.m_ResolvedScale;
			this.UpdateScreenDPI();
			this.m_TargetRect = this.GetDisplayRect();
			bool flag = this.renderMode == PanelRenderMode.WorldSpace;
			if (flag)
			{
				this.m_ResolvedScale = 1f;
			}
			else
			{
				this.m_ResolvedScale = this.ResolveScale(this.m_TargetRect, this.ScreenDPI);
			}
			BaseRuntimePanel panel = this.panel;
			bool flag2 = this.renderMode != PanelRenderMode.WorldSpace;
			if (flag2)
			{
				bool flag3 = this.visualTree.style.width.value == 0f || this.m_ResolvedScale != resolvedScale || this.m_TargetRect.width != targetRect.width || this.m_TargetRect.height != targetRect.height;
				if (flag3)
				{
					panel.scale = ((this.m_ResolvedScale == 0f) ? 0f : (1f / this.m_ResolvedScale));
					this.visualTree.style.left = 0f;
					this.visualTree.style.top = 0f;
					this.visualTree.style.width = this.m_TargetRect.width * this.m_ResolvedScale;
					this.visualTree.style.height = this.m_TargetRect.height * this.m_ResolvedScale;
				}
				panel.panelRenderer.forceGammaRendering = this.targetTexture != null && this.forceGammaRendering;
			}
			panel.targetTexture = this.targetTexture;
			panel.targetDisplay = this.targetDisplay;
			panel.drawsInCameras = this.renderMode == PanelRenderMode.WorldSpace;
			panel.pixelsPerUnit = this.pixelsPerUnit;
			panel.isFlat = this.renderMode != PanelRenderMode.WorldSpace;
			panel.clearSettings = new PanelClearSettings
			{
				clearColor = this.m_ClearColor,
				clearDepthStencil = this.m_ClearDepthStencil,
				color = this.m_ColorClearValue
			};
			panel.referenceSpritePixelsPerUnit = this.referenceSpritePixelsPerUnit;
			panel.panelRenderer.vertexBudget = this.m_VertexBudget;
			panel.panelRenderer.textureSlotCount = this.m_TextureSlotCount;
			panel.dataBindingManager.logLevel = this.m_BindingLogLevel;
			DynamicAtlas dynamicAtlas = panel.atlas as DynamicAtlas;
			bool flag4 = dynamicAtlas != null;
			if (flag4)
			{
				dynamicAtlas.minAtlasSize = this.dynamicAtlasSettings.minAtlasSize;
				dynamicAtlas.maxAtlasSize = this.dynamicAtlasSettings.maxAtlasSize;
				dynamicAtlas.maxSubTextureSize = this.dynamicAtlasSettings.maxSubTextureSize;
				dynamicAtlas.activeFilters = this.dynamicAtlasSettings.activeFilters;
				dynamicAtlas.customFilter = this.dynamicAtlasSettings.customFilter;
			}
		}

		public void SetScreenToPanelSpaceFunction3D(Func<Vector2, Vector3> screenToPanelSpaceFunction)
		{
			this.m_AssignedScreenToPanel = screenToPanelSpaceFunction;
			this.panel.screenToPanelSpace = this.m_AssignedScreenToPanel;
		}

		public void SetScreenToPanelSpaceFunction(Func<Vector2, Vector2> screenToPanelSpaceFunction)
		{
			this.m_AssignedScreenToPanel = (Vector2 p) => screenToPanelSpaceFunction(p);
			this.panel.screenToPanelSpace = this.m_AssignedScreenToPanel;
		}

		internal float ResolveScale(Rect targetRect, float screenDpi)
		{
			float num = 1f;
			switch (this.scaleMode)
			{
			case PanelScaleMode.ConstantPhysicalSize:
			{
				float num2 = ((screenDpi == 0f) ? this.fallbackDpi : screenDpi);
				bool flag = num2 != 0f;
				if (flag)
				{
					num = this.referenceDpi / num2;
				}
				break;
			}
			case PanelScaleMode.ScaleWithScreenSize:
			{
				bool flag2 = this.referenceResolution.x * this.referenceResolution.y != 0;
				if (flag2)
				{
					Vector2 vector = this.referenceResolution;
					Vector2 vector2 = new Vector2(targetRect.width / vector.x, targetRect.height / vector.y);
					PanelScreenMatchMode screenMatchMode = this.screenMatchMode;
					PanelScreenMatchMode panelScreenMatchMode = screenMatchMode;
					float num4;
					if (panelScreenMatchMode != PanelScreenMatchMode.Shrink)
					{
						if (panelScreenMatchMode != PanelScreenMatchMode.Expand)
						{
							float num3 = Mathf.Clamp01(this.match);
							num4 = Mathf.Lerp(vector2.x, vector2.y, num3);
						}
						else
						{
							num4 = Mathf.Min(vector2.x, vector2.y);
						}
					}
					else
					{
						num4 = Mathf.Max(vector2.x, vector2.y);
					}
					bool flag3 = num4 != 0f;
					if (flag3)
					{
						num = 1f / num4;
					}
				}
				break;
			}
			}
			bool flag4 = this.scale > 0f;
			if (flag4)
			{
				num /= this.scale;
			}
			else
			{
				num = 0f;
			}
			return num;
		}

		internal Rect GetDisplayRect()
		{
			bool flag = this.m_TargetTexture != null;
			Rect rect;
			if (flag)
			{
				rect = new Rect(0f, 0f, (float)this.m_TargetTexture.width, (float)this.m_TargetTexture.height);
			}
			else
			{
				rect = new Rect(0f, 0f, (float)BaseRuntimePanel.getScreenRenderingWidth(this.targetDisplay), (float)BaseRuntimePanel.getScreenRenderingHeight(this.targetDisplay));
			}
			return rect;
		}

		internal void AttachAndInsertUIDocumentToVisualTree(UIDocument uiDocument)
		{
			bool flag = this.m_AttachedUIDocumentsList == null;
			if (flag)
			{
				this.m_AttachedUIDocumentsList = new UIDocumentList();
			}
			else
			{
				this.m_AttachedUIDocumentsList.RemoveFromListAndFromVisualTree(uiDocument);
			}
			this.m_AttachedUIDocumentsList.AddToListAndToVisualTree(uiDocument, this.visualTree, false, 0);
		}

		internal void DetachUIDocument(UIDocument uiDocument)
		{
			bool flag = this.m_AttachedUIDocumentsList == null;
			if (!flag)
			{
				this.m_AttachedUIDocumentsList.RemoveFromListAndFromVisualTree(uiDocument);
				bool flag2 = this.m_AttachedUIDocumentsList.m_AttachedUIDocuments.Count == 0;
				if (flag2)
				{
					this.m_PanelAccess.MarkPotentiallyEmpty();
				}
			}
		}

		private const int k_DefaultSortingOrder = 0;

		private const float k_DefaultScaleValue = 1f;

		internal const string k_DefaultStyleSheetPath = "Packages/com.unity.ui/PackageResources/StyleSheets/Generated/Default.tss.asset";

		[SerializeField]
		private ThemeStyleSheet themeUss;

		[SerializeField]
		private bool m_DisableNoThemeWarning = false;

		[SerializeField]
		private RenderTexture m_TargetTexture;

		[SerializeField]
		private PanelRenderMode m_RenderMode = PanelRenderMode.ScreenSpaceOverlay;

		[FormerlySerializedAs("m_WorldInputMode")]
		[SerializeField]
		private ColliderUpdateMode m_ColliderUpdateMode = ColliderUpdateMode.MatchBoundingBox;

		[SerializeField]
		private bool m_ColliderIsTrigger = true;

		[SerializeField]
		private PanelScaleMode m_ScaleMode = PanelScaleMode.ConstantPhysicalSize;

		[SerializeField]
		private float m_ReferenceSpritePixelsPerUnit = 100f;

		[SerializeField]
		private float m_PixelsPerUnit = 100f;

		[SerializeField]
		private float m_Scale = 1f;

		private const float DefaultDpi = 96f;

		[SerializeField]
		private float m_ReferenceDpi = 96f;

		[SerializeField]
		private float m_FallbackDpi = 96f;

		[SerializeField]
		private Vector2Int m_ReferenceResolution = new Vector2Int(1200, 800);

		[SerializeField]
		private PanelScreenMatchMode m_ScreenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_Match = 0f;

		[SerializeField]
		private float m_SortingOrder = 0f;

		[SerializeField]
		private int m_TargetDisplay = 0;

		[SerializeField]
		private BindingLogLevel m_BindingLogLevel;

		[SerializeField]
		private bool m_ClearDepthStencil = true;

		[SerializeField]
		private bool m_ClearColor;

		[SerializeField]
		private Color m_ColorClearValue = Color.clear;

		[SerializeField]
		private uint m_VertexBudget = 0U;

		[SerializeField]
		private TextureSlotCount m_TextureSlotCount = TextureSlotCount.Eight;

		private PanelSettings.RuntimePanelAccess m_PanelAccess;

		internal UIDocumentList m_AttachedUIDocumentsList;

		[HideInInspector]
		[SerializeField]
		private DynamicAtlasSettings m_DynamicAtlasSettings = DynamicAtlasSettings.defaults;

		[SerializeField]
		[HideInInspector]
		private Shader m_AtlasBlitShader;

		[HideInInspector]
		[SerializeField]
		private Shader m_DefaultShader;

		[HideInInspector]
		[SerializeField]
		private Shader m_RuntimeGaussianBlurShader;

		[SerializeField]
		[HideInInspector]
		private Shader m_RuntimeColorEffectShader;

		[SerializeField]
		[HideInInspector]
		private Shader m_SDFShader;

		[SerializeField]
		[HideInInspector]
		private Shader m_BitmapShader;

		[HideInInspector]
		[SerializeField]
		private Shader m_SpriteShader;

		[HideInInspector]
		[SerializeField]
		internal TextAsset m_ICUDataAsset;

		[SerializeField]
		public bool forceGammaRendering;

		[SerializeField]
		public PanelTextSettings textSettings;

		private Rect m_TargetRect;

		private float m_ResolvedScale;

		private StyleSheet m_OldThemeUss;

		private IDebugPanelChangeReceiver m_PanelChangeReceiver = null;

		private Func<Vector2, Vector3> m_AssignedScreenToPanel;

		private class RuntimePanelAccess
		{
			internal RuntimePanelAccess(PanelSettings settings)
			{
				this.m_Settings = settings;
			}

			internal bool isInitialized
			{
				get
				{
					return this.m_RuntimePanel != null;
				}
			}

			internal bool isTransient { get; set; }

			internal BaseRuntimePanel panel
			{
				get
				{
					bool flag = this.m_RuntimePanel == null;
					if (flag)
					{
						this.m_RuntimePanel = (this.isTransient ? RuntimePanel.Create(this.m_Settings) : this.CreateRelatedRuntimePanel());
						this.m_RuntimePanel.sortingPriority = this.m_Settings.m_SortingOrder;
						this.m_RuntimePanel.targetDisplay = this.m_Settings.m_TargetDisplay;
						this.m_RuntimePanel.panelChangeReceiver = this.m_Settings.GetPanelChangeReceiver();
						VisualElement visualTree = this.m_RuntimePanel.visualTree;
						visualTree.name = this.m_Settings.name;
						this.m_Settings.ApplyPanelSettings();
						this.m_Settings.ApplyThemeStyleSheet(visualTree);
						bool flag2 = this.m_Settings.m_TargetTexture != null;
						if (flag2)
						{
							this.m_RuntimePanel.targetTexture = this.m_Settings.m_TargetTexture;
						}
						bool flag3 = this.m_Settings.m_AssignedScreenToPanel != null;
						if (flag3)
						{
							this.m_Settings.SetScreenToPanelSpaceFunction3D(this.m_Settings.m_AssignedScreenToPanel);
						}
					}
					return this.m_RuntimePanel;
				}
			}

			internal void DisposePanel()
			{
				bool flag = this.m_RuntimePanel != null;
				if (flag)
				{
					this.DisposeRelatedPanel();
					this.m_RuntimePanel = null;
				}
			}

			internal void SetTargetTexture()
			{
				bool flag = this.m_RuntimePanel != null;
				if (flag)
				{
					this.m_RuntimePanel.targetTexture = this.m_Settings.targetTexture;
				}
			}

			internal void SetSortingPriority()
			{
				bool flag = this.m_RuntimePanel != null;
				if (flag)
				{
					this.m_RuntimePanel.sortingPriority = this.m_Settings.m_SortingOrder;
				}
			}

			internal void SetTargetDisplay()
			{
				bool flag = this.m_RuntimePanel != null;
				if (flag)
				{
					this.m_RuntimePanel.targetDisplay = this.m_Settings.m_TargetDisplay;
				}
			}

			internal void SetPanelChangeReceiver()
			{
				bool flag = this.m_RuntimePanel != null;
				if (flag)
				{
					this.m_RuntimePanel.panelChangeReceiver = this.m_Settings.m_PanelChangeReceiver;
				}
			}

			private BaseRuntimePanel CreateRelatedRuntimePanel()
			{
				return (RuntimePanel)UIElementsRuntimeUtility.FindOrCreateRuntimePanel(this.m_Settings, new UIElementsRuntimeUtility.CreateRuntimePanelDelegate(RuntimePanel.Create));
			}

			private void DisposeRelatedPanel()
			{
				UIElementsRuntimeUtility.DisposeRuntimePanel(this.m_Settings);
			}

			internal void MarkPotentiallyEmpty()
			{
				UIElementsRuntimeUtility.MarkPotentiallyEmpty(this.m_Settings);
			}

			private readonly PanelSettings m_Settings;

			private BaseRuntimePanel m_RuntimePanel;
		}
	}
}
