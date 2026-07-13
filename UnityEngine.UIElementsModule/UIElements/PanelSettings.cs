using System;
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

		internal BaseRuntimePanel panel
		{
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
			bool flag = this.themeUss == null;
			if (flag)
			{
				Debug.LogWarning("No Theme Style Sheet set to PanelSettings " + base.name + ", UI will not render properly", this);
			}
			this.UpdateScreenDPI();
			this.InitializeShaders();
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
				this.m_OldThemeUss = this.themeUss;
			}
		}

		private void InitializeShaders()
		{
			bool flag = this.m_AtlasBlitShader == null;
			if (flag)
			{
				this.m_AtlasBlitShader = Shader.Find(Shaders.k_AtlasBlit);
			}
			bool flag2 = this.m_RuntimeShader == null;
			if (flag2)
			{
				this.m_RuntimeShader = Shader.Find(Shaders.k_Runtime);
			}
			bool flag3 = this.m_RuntimeWorldShader == null;
			if (flag3)
			{
				this.m_RuntimeWorldShader = Shader.Find(Shaders.k_RuntimeWorld);
			}
			this.m_PanelAccess.SetTargetTexture();
		}

		internal void ApplyPanelSettings()
		{
			Rect targetRect = this.m_TargetRect;
			float resolvedScale = this.m_ResolvedScale;
			this.UpdateScreenDPI();
			this.m_TargetRect = this.GetDisplayRect();
			this.m_ResolvedScale = this.ResolveScale(this.m_TargetRect, this.ScreenDPI);
			bool flag = this.visualTree.style.width.value == 0f || this.m_ResolvedScale != resolvedScale || this.m_TargetRect.width != targetRect.width || this.m_TargetRect.height != targetRect.height;
			if (flag)
			{
				this.panel.scale = ((this.m_ResolvedScale == 0f) ? 0f : (1f / this.m_ResolvedScale));
				this.visualTree.style.left = 0f;
				this.visualTree.style.top = 0f;
				this.visualTree.style.width = this.m_TargetRect.width * this.m_ResolvedScale;
				this.visualTree.style.height = this.m_TargetRect.height * this.m_ResolvedScale;
			}
			this.panel.targetTexture = this.targetTexture;
			this.panel.targetDisplay = this.targetDisplay;
			this.panel.drawToCameras = false;
			this.panel.clearSettings = new PanelClearSettings
			{
				clearColor = this.m_ClearColor,
				clearDepthStencil = this.m_ClearDepthStencil,
				color = this.m_ColorClearValue
			};
			this.panel.referenceSpritePixelsPerUnit = this.referenceSpritePixelsPerUnit;
			DynamicAtlas dynamicAtlas = this.panel.atlas as DynamicAtlas;
			bool flag2 = dynamicAtlas != null;
			if (flag2)
			{
				dynamicAtlas.minAtlasSize = this.dynamicAtlasSettings.minAtlasSize;
				dynamicAtlas.maxAtlasSize = this.dynamicAtlasSettings.maxAtlasSize;
				dynamicAtlas.maxSubTextureSize = this.dynamicAtlasSettings.maxSubTextureSize;
				dynamicAtlas.activeFilters = this.dynamicAtlasSettings.activeFilters;
				dynamicAtlas.customFilter = this.dynamicAtlasSettings.customFilter;
			}
		}

		public void SetScreenToPanelSpaceFunction(Func<Vector2, Vector2> screentoPanelSpaceFunction)
		{
			this.m_AssignedScreenToPanel = screentoPanelSpaceFunction;
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
			this.m_AttachedUIDocumentsList.AddToListAndToVisualTree(uiDocument, this.visualTree, 0);
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
		private RenderTexture m_TargetTexture;

		[SerializeField]
		private PanelScaleMode m_ScaleMode = PanelScaleMode.ConstantPhysicalSize;

		[SerializeField]
		private float m_ReferenceSpritePixelsPerUnit = 100f;

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
		private bool m_ClearDepthStencil = true;

		[SerializeField]
		private bool m_ClearColor;

		[SerializeField]
		private Color m_ColorClearValue = Color.clear;

		private PanelSettings.RuntimePanelAccess m_PanelAccess;

		internal UIDocumentList m_AttachedUIDocumentsList;

		[SerializeField]
		[HideInInspector]
		private DynamicAtlasSettings m_DynamicAtlasSettings = DynamicAtlasSettings.defaults;

		[SerializeField]
		[HideInInspector]
		private Shader m_AtlasBlitShader;

		[SerializeField]
		[HideInInspector]
		private Shader m_RuntimeShader;

		[HideInInspector]
		[SerializeField]
		private Shader m_RuntimeWorldShader;

		[SerializeField]
		public PanelTextSettings textSettings;

		private Rect m_TargetRect;

		private float m_ResolvedScale;

		private StyleSheet m_OldThemeUss;

		private Func<Vector2, Vector2> m_AssignedScreenToPanel;

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

			internal BaseRuntimePanel panel
			{
				get
				{
					bool flag = this.m_RuntimePanel == null;
					if (flag)
					{
						this.m_RuntimePanel = this.CreateRelatedRuntimePanel();
						this.m_RuntimePanel.sortingPriority = this.m_Settings.m_SortingOrder;
						this.m_RuntimePanel.targetDisplay = this.m_Settings.m_TargetDisplay;
						VisualElement visualTree = this.m_RuntimePanel.visualTree;
						visualTree.name = this.m_Settings.name;
						this.m_Settings.ApplyThemeStyleSheet(visualTree);
						bool flag2 = this.m_Settings.m_TargetTexture != null;
						if (flag2)
						{
							this.m_RuntimePanel.targetTexture = this.m_Settings.m_TargetTexture;
						}
						bool flag3 = this.m_Settings.m_AssignedScreenToPanel != null;
						if (flag3)
						{
							this.m_Settings.SetScreenToPanelSpaceFunction(this.m_Settings.m_AssignedScreenToPanel);
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
