using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-100)]
	[AddComponentMenu("UI Toolkit/UI Document")]
	[HelpURL("UIE-get-started-with-runtime-ui")]
	public sealed class UIDocument : MonoBehaviour
	{
		public PanelSettings panelSettings
		{
			get
			{
				return this.m_PanelSettings;
			}
			set
			{
				bool flag = this.parentUI == null;
				if (flag)
				{
					bool flag2 = this.m_PanelSettings == value;
					if (flag2)
					{
						this.m_PreviousPanelSettings = this.m_PanelSettings;
						return;
					}
					bool flag3 = this.m_PanelSettings != null;
					if (flag3)
					{
						this.m_PanelSettings.DetachUIDocument(this);
					}
					this.m_PanelSettings = value;
					bool flag4 = this.m_PanelSettings != null;
					if (flag4)
					{
						this.m_PanelSettings.AttachAndInsertUIDocumentToVisualTree(this);
					}
				}
				else
				{
					Assert.AreEqual<PanelSettings>(this.parentUI.m_PanelSettings, value);
					this.m_PanelSettings = this.parentUI.m_PanelSettings;
				}
				bool flag5 = this.m_ChildrenContent != null;
				if (flag5)
				{
					foreach (UIDocument uidocument in this.m_ChildrenContent.m_AttachedUIDocuments)
					{
						uidocument.panelSettings = this.m_PanelSettings;
					}
				}
				this.m_PreviousPanelSettings = this.m_PanelSettings;
			}
		}

		public UIDocument parentUI
		{
			get
			{
				return this.m_ParentUI;
			}
			private set
			{
				this.m_ParentUI = value;
			}
		}

		public VisualTreeAsset visualTreeAsset
		{
			get
			{
				return this.sourceAsset;
			}
			set
			{
				this.sourceAsset = value;
				this.RecreateUI();
			}
		}

		public VisualElement rootVisualElement
		{
			get
			{
				return this.m_RootVisualElement;
			}
			private set
			{
				this.m_RootVisualElement = (UIDocumentRootElement)value;
				this.focusRing = ((value != null) ? new VisualElementFocusRing(value, VisualElementFocusRing.DefaultFocusOrder.ChildOrder) : null);
			}
		}

		internal VisualElementFocusRing focusRing { get; private set; } = null;

		internal int firstChildInserIndex
		{
			get
			{
				return this.m_FirstChildInsertIndex;
			}
		}

		public Position position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				bool flag = this.m_Position == value;
				if (!flag)
				{
					this.m_Position = value;
					this.SetupPosition();
				}
			}
		}

		public UIDocument.WorldSpaceSizeMode worldSpaceSizeMode
		{
			get
			{
				return this.m_WorldSpaceSizeMode;
			}
			set
			{
				bool flag = this.m_WorldSpaceSizeMode == value;
				if (!flag)
				{
					this.m_WorldSpaceSizeMode = value;
					this.SetupWorldSpaceSize();
				}
			}
		}

		public Vector2 worldSpaceSize
		{
			get
			{
				return new Vector2(this.m_WorldSpaceWidth, this.m_WorldSpaceHeight);
			}
			set
			{
				bool flag = this.m_WorldSpaceWidth == value.x && this.m_WorldSpaceHeight == value.y;
				if (!flag)
				{
					this.m_WorldSpaceWidth = value.x;
					this.m_WorldSpaceHeight = value.y;
					this.SetupWorldSpaceSize();
				}
			}
		}

		private bool isWorldSpace
		{
			get
			{
				return this.m_PanelSettings != null && this.m_PanelSettings.renderMode == PanelRenderMode.WorldSpace;
			}
		}

		internal bool isTransformControlledByGameObject
		{
			get
			{
				return this.isWorldSpace && (this.m_ParentUI == null || this.m_Position == Position.Absolute);
			}
		}

		public PivotReferenceSize pivotReferenceSize
		{
			get
			{
				return this.m_PivotReferenceSize;
			}
			set
			{
				this.m_PivotReferenceSize = value;
			}
		}

		public Pivot pivot
		{
			get
			{
				return this.m_Pivot;
			}
			set
			{
				this.m_Pivot = value;
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
				bool flag = this.m_SortingOrder == value;
				if (!flag)
				{
					this.m_SortingOrder = value;
					this.ApplySortingOrder();
				}
			}
		}

		internal void ApplySortingOrder()
		{
			this.AddRootVisualElementToTree();
		}

		internal static UIDocument FindRootUIDocument(VisualElement element)
		{
			UIDocumentRootElement firstOfType = element.GetFirstOfType<UIDocumentRootElement>();
			UIDocument uidocument = ((firstOfType != null) ? firstOfType.document : null);
			while (((uidocument != null) ? uidocument.parentUI : null) != null)
			{
				uidocument = uidocument.parentUI;
			}
			return uidocument;
		}

		private UIDocument()
		{
			this.m_UIDocumentCreationIndex = UIDocument.s_CurrentUIDocumentCounter++;
		}

		private void Awake()
		{
			this.SetupFromHierarchy();
		}

		private void OnEnable()
		{
			this._Enable();
			UIDocument.EnabledDocumentCount++;
		}

		private void _Enable()
		{
			bool flag = this.parentUI != null && this.m_PanelSettings == null;
			if (flag)
			{
				this.m_PanelSettings = this.parentUI.m_PanelSettings;
			}
			bool flag2 = this.m_RootVisualElement == null;
			if (flag2)
			{
				this.RecreateUI();
			}
			else
			{
				this.AddRootVisualElementToTree();
			}
			UIRenderer uirenderer;
			bool flag3 = base.TryGetComponent<UIRenderer>(out uirenderer);
			if (flag3)
			{
				uirenderer.enabled = true;
			}
		}

		public IRuntimePanel runtimePanel
		{
			get
			{
				return this.containerPanel;
			}
		}

		internal RuntimePanel containerPanel
		{
			get
			{
				VisualElement rootVisualElement = this.rootVisualElement;
				return (RuntimePanel)((rootVisualElement != null) ? rootVisualElement.elementPanel : null);
			}
		}

		private void LateUpdate()
		{
			this.DoUpdate();
		}

		internal void DoUpdate()
		{
			bool flag = this.m_RootVisualElement == null || this.panelSettings == null || this.panelSettings.panel == null;
			if (flag)
			{
				this.RemoveWorldSpaceCollider();
			}
			else
			{
				this.AddOrRemoveRendererComponent();
				bool isWorldSpace = this.isWorldSpace;
				if (isWorldSpace)
				{
					bool isTransformControlledByGameObject = this.isTransformControlledByGameObject;
					if (isTransformControlledByGameObject)
					{
						this.SetTransform();
					}
					else
					{
						this.ClearTransform();
					}
					this.UpdateRenderer();
					bool flag2 = this.panelSettings.colliderUpdateMode != ColliderUpdateMode.Keep;
					if (flag2)
					{
						this.UpdateWorldSpaceCollider(this.panelSettings.colliderUpdateMode);
					}
				}
				else
				{
					this.RemoveWorldSpaceCollider();
					bool rootHasWorldTransform = this.m_RootHasWorldTransform;
					if (rootHasWorldTransform)
					{
						this.ClearTransform();
					}
				}
				this.UpdateIsWorldSpaceRootFlag();
			}
		}

		private void UpdateRenderer()
		{
			UIRenderer uirenderer;
			bool flag = !base.TryGetComponent<UIRenderer>(out uirenderer);
			if (flag)
			{
				this.m_RootVisualElement.uiRenderer = null;
			}
			else
			{
				this.m_RootVisualElement.uiRenderer = uirenderer;
				uirenderer.skipRendering = this.parentUI != null || this.pixelsPerUnit < Mathf.Epsilon;
				BaseRuntimePanel baseRuntimePanel = (BaseRuntimePanel)this.m_RootVisualElement.panel;
				bool flag2 = baseRuntimePanel == null;
				if (!flag2)
				{
					Bounds bounds = this.SanitizeRendererBounds(this.rootVisualElement.localBounds3D);
					Matrix4x4 matrix4x = this.TransformToGameObjectMatrix();
					VisualElement.TransformAlignedBounds(ref matrix4x, ref bounds);
					uirenderer.localBounds = bounds;
					this.UpdateIsWorldSpaceRootFlag();
				}
			}
		}

		private Bounds SanitizeRendererBounds(Bounds b)
		{
			bool flag = float.IsNaN(b.size.x) || float.IsNaN(b.size.y) || float.IsNaN(b.size.z);
			if (flag)
			{
				b = new Bounds(Vector3.zero, Vector3.zero);
			}
			bool flag2 = b.size.x < 0f || b.size.y < 0f;
			if (flag2)
			{
				b.size = Vector3.zero;
			}
			return b;
		}

		private void AddOrRemoveRendererComponent()
		{
			UIRenderer uirenderer;
			base.TryGetComponent<UIRenderer>(out uirenderer);
			bool isWorldSpace = this.isWorldSpace;
			if (isWorldSpace)
			{
				bool flag = uirenderer == null;
				if (flag)
				{
					base.gameObject.AddComponent<UIRenderer>();
				}
			}
			else
			{
				UIRUtility.Destroy(uirenderer);
			}
		}

		internal void UpdateWorldSpaceCollider(ColliderUpdateMode mode)
		{
			bool flag = this.parentUI != null;
			if (!flag)
			{
				bool flag2 = this.containerPanel == null;
				if (flag2)
				{
					this.RemoveWorldSpaceCollider();
				}
				else
				{
					bool flag3 = mode == ColliderUpdateMode.MatchBoundingBox;
					Bounds picking3DWorldBounds;
					if (flag3)
					{
						picking3DWorldBounds = WorldSpaceInput.GetPicking3DWorldBounds(this.rootVisualElement);
					}
					else
					{
						Rect worldBound = this.rootVisualElement.worldBound;
						picking3DWorldBounds = new Bounds(worldBound.center, worldBound.size);
					}
					bool flag4 = !UIDocument.IsValidBounds(in picking3DWorldBounds);
					if (flag4)
					{
						this.RemoveWorldSpaceCollider();
					}
					else
					{
						bool flag5 = this.m_WorldSpaceCollider == null;
						if (flag5)
						{
							this.m_WorldSpaceCollider = base.gameObject.AddComponent<BoxCollider>();
							this.m_WorldSpaceCollider.isTrigger = this.panelSettings.colliderIsTrigger;
						}
						bool flag6 = picking3DWorldBounds.center != this.m_WorldSpaceCollider.center || picking3DWorldBounds.size != this.m_WorldSpaceCollider.size;
						if (flag6)
						{
							this.m_WorldSpaceCollider.center = picking3DWorldBounds.center;
							this.m_WorldSpaceCollider.size = picking3DWorldBounds.size;
						}
					}
				}
			}
		}

		internal void RemoveWorldSpaceCollider()
		{
			UIRUtility.Destroy(this.m_WorldSpaceCollider);
			this.m_WorldSpaceCollider = null;
		}

		private static bool IsValidBounds(in Bounds b)
		{
			Vector3 extents = b.extents;
			int num = ((extents.x > 0f) ? 1 : 0) + ((extents.y > 0f) ? 1 : 0) + ((extents.z > 0f) ? 1 : 0);
			return num >= 2;
		}

		private void UpdateIsWorldSpaceRootFlag()
		{
			bool flag = !this.panelSettings.panel.isFlat;
			bool flag2 = flag;
			bool flag3 = flag2 && this.parentUI == null;
			bool flag4 = this.m_RootVisualElement.isWorldSpaceRootUIDocument != flag3;
			if (flag4)
			{
				this.m_RootVisualElement.isWorldSpaceRootUIDocument = flag3;
				this.m_RootVisualElement.MarkDirtyRepaint();
			}
		}

		private void SetTransform()
		{
			Matrix4x4 matrix4x;
			this.ComputeTransform(base.transform, out matrix4x);
			this.m_RootVisualElement.style.transformOrigin = new TransformOrigin(Vector3.zero);
			this.m_RootVisualElement.style.translate = new Translate(matrix4x.GetPosition());
			this.m_RootVisualElement.style.rotate = new Rotate(matrix4x.rotation);
			this.m_RootVisualElement.style.scale = new Scale(matrix4x.lossyScale);
			this.m_RootHasWorldTransform = true;
		}

		private void ClearTransform()
		{
			this.m_RootVisualElement.style.transformOrigin = StyleKeyword.Null;
			this.m_RootVisualElement.style.translate = StyleKeyword.Null;
			this.m_RootVisualElement.style.rotate = StyleKeyword.Null;
			this.m_RootVisualElement.style.scale = StyleKeyword.Null;
			this.m_RootHasWorldTransform = false;
		}

		private float pixelsPerUnit
		{
			get
			{
				RuntimePanel containerPanel = this.containerPanel;
				return (containerPanel != null) ? containerPanel.pixelsPerUnit : 1f;
			}
		}

		private Matrix4x4 ScaleAndFlipMatrix()
		{
			float pixelsPerUnit = this.pixelsPerUnit;
			bool flag = pixelsPerUnit < Mathf.Epsilon;
			Matrix4x4 matrix4x;
			if (flag)
			{
				matrix4x = Matrix4x4.identity;
			}
			else
			{
				float num = 1f / pixelsPerUnit;
				Vector3 vector = Vector3.one * num;
				Quaternion quaternion = Quaternion.AngleAxis(180f, Vector3.right);
				matrix4x = Matrix4x4.TRS(Vector3.zero, quaternion, vector);
			}
			return matrix4x;
		}

		private Bounds LocalBoundsFromPivotSource()
		{
			Bounds localBounds3DWithoutNested3D = this.m_RootVisualElement.localBounds3DWithoutNested3D;
			bool flag = this.m_PivotReferenceSize == PivotReferenceSize.BoundingBox;
			Bounds bounds;
			if (flag)
			{
				bounds = localBounds3DWithoutNested3D;
			}
			else
			{
				Rect layout = this.m_RootVisualElement.layout;
				Vector2 center = layout.center;
				Vector2 size = layout.size;
				float z = localBounds3DWithoutNested3D.size.z;
				bounds = new Bounds(new Vector3(center.x, center.y, localBounds3DWithoutNested3D.min.z + z * 0.5f), new Vector3(size.x, size.y, z));
			}
			return this.SanitizeRendererBounds(bounds);
		}

		private Vector2 PivotOffset()
		{
			Vector2 pivotAsPercent = UIDocument.GetPivotAsPercent(this.m_Pivot);
			Bounds bounds = this.LocalBoundsFromPivotSource();
			return -bounds.min + new Vector2(-bounds.size.x * pivotAsPercent.x, -bounds.size.y * pivotAsPercent.y);
		}

		private Matrix4x4 TransformToGameObjectMatrix()
		{
			Matrix4x4 matrix4x = this.ScaleAndFlipMatrix();
			MathUtils.PostApply2DOffset(ref matrix4x, this.PivotOffset());
			return matrix4x;
		}

		private void ComputeTransform(Transform transform, out Matrix4x4 matrix)
		{
			bool flag = this.parentUI == null;
			if (flag)
			{
				matrix = this.TransformToGameObjectMatrix();
			}
			else
			{
				Matrix4x4 matrix4x = this.parentUI.ScaleAndFlipMatrix();
				Matrix4x4 inverse = matrix4x.inverse;
				Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
				Matrix4x4 worldToLocalMatrix = this.parentUI.transform.worldToLocalMatrix;
				matrix = inverse * worldToLocalMatrix * localToWorldMatrix * matrix4x;
				MathUtils.PreApply2DOffset(ref matrix, -this.parentUI.PivotOffset());
				MathUtils.PostApply2DOffset(ref matrix, this.PivotOffset());
			}
		}

		private static Vector2 GetPivotAsPercent(Pivot origin)
		{
			Vector2 vector;
			switch (origin)
			{
			case Pivot.Center:
				vector = new Vector2(0.5f, 0.5f);
				break;
			case Pivot.TopLeft:
				vector = new Vector2(0f, 0f);
				break;
			case Pivot.TopCenter:
				vector = new Vector2(0.5f, 0f);
				break;
			case Pivot.TopRight:
				vector = new Vector2(1f, 0f);
				break;
			case Pivot.LeftCenter:
				vector = new Vector2(0f, 0.5f);
				break;
			case Pivot.RightCenter:
				vector = new Vector2(1f, 0.5f);
				break;
			case Pivot.BottomLeft:
				vector = new Vector2(0f, 1f);
				break;
			case Pivot.BottomCenter:
				vector = new Vector2(0.5f, 1f);
				break;
			case Pivot.BottomRight:
				vector = new Vector2(1f, 1f);
				break;
			default:
				vector = new Vector2(0.5f, 0.5f);
				break;
			}
			return vector;
		}

		private void SetupFromHierarchy()
		{
			bool flag = this.parentUI != null;
			if (flag)
			{
				this.parentUI.RemoveChild(this);
			}
			this.parentUI = this.FindUIDocumentParent();
		}

		private UIDocument FindUIDocumentParent()
		{
			Transform transform = base.transform;
			Transform parent = transform.parent;
			bool flag = parent != null;
			if (flag)
			{
				UIDocument[] componentsInParent = parent.GetComponentsInParent<UIDocument>(true);
				bool flag2 = componentsInParent != null && componentsInParent.Length != 0;
				if (flag2)
				{
					return componentsInParent[0];
				}
			}
			return null;
		}

		internal void Reset()
		{
			bool flag = this.parentUI == null;
			if (flag)
			{
				PanelSettings previousPanelSettings = this.m_PreviousPanelSettings;
				if (previousPanelSettings != null)
				{
					previousPanelSettings.DetachUIDocument(this);
				}
				this.panelSettings = null;
			}
			this.SetupFromHierarchy();
			bool flag2 = this.parentUI != null;
			if (flag2)
			{
				this.m_PanelSettings = this.parentUI.m_PanelSettings;
				this.AddRootVisualElementToTree();
			}
			else
			{
				bool flag3 = this.m_PanelSettings != null;
				if (flag3)
				{
					this.AddRootVisualElementToTree();
				}
			}
		}

		internal void AddChildAndInsertContentToVisualTree(UIDocument child)
		{
			bool flag = this.m_ChildrenContent == null;
			if (flag)
			{
				this.m_ChildrenContent = new UIDocumentList();
			}
			else
			{
				this.m_ChildrenContent.RemoveFromListAndFromVisualTree(child);
			}
			bool flag2 = child.position == Position.Absolute;
			this.m_ChildrenContent.AddToListAndToVisualTree(child, this.m_RootVisualElement, flag2, this.m_FirstChildInsertIndex);
		}

		private void RemoveChild(UIDocument child)
		{
			UIDocumentList childrenContent = this.m_ChildrenContent;
			if (childrenContent != null)
			{
				childrenContent.RemoveFromListAndFromVisualTree(child);
			}
		}

		private void RecreateUI()
		{
			bool flag = this.m_RootVisualElement != null;
			if (flag)
			{
				this.RemoveFromHierarchy();
				this.rootVisualElement = null;
			}
			bool flag2 = this.sourceAsset != null;
			if (flag2)
			{
				this.rootVisualElement = new UIDocumentRootElement(this, this.sourceAsset);
				try
				{
					bool hasEditorElements = this.sourceAsset.hasEditorElements;
					if (hasEditorElements)
					{
						Debug.LogWarning("The VisualTreeAsset contains editor-only elements that are incompatible at runtime.\nTo fix this, remove the editor elements from the VisualTreeAsset.", this);
					}
					this.sourceAsset.CloneTree(this.m_RootVisualElement);
				}
				catch (Exception ex)
				{
					Debug.LogError("The UXML file set for the UIDocument could not be cloned.");
					Debug.LogException(ex);
				}
			}
			bool flag3 = this.m_RootVisualElement == null;
			if (flag3)
			{
				this.rootVisualElement = new UIDocumentRootElement(this, null)
				{
					name = base.gameObject.name + "-container"
				};
			}
			else
			{
				this.m_RootVisualElement.name = base.gameObject.name + "-container";
			}
			this.m_RootVisualElement.pickingMode = PickingMode.Ignore;
			bool isActiveAndEnabled = base.isActiveAndEnabled;
			if (isActiveAndEnabled)
			{
				this.AddRootVisualElementToTree();
			}
			this.m_FirstChildInsertIndex = this.m_RootVisualElement.childCount;
			bool flag4 = this.m_ChildrenContent != null;
			if (flag4)
			{
				bool flag5 = this.m_ChildrenContentCopy == null;
				if (flag5)
				{
					this.m_ChildrenContentCopy = new List<UIDocument>(this.m_ChildrenContent.m_AttachedUIDocuments);
				}
				else
				{
					this.m_ChildrenContentCopy.AddRange(this.m_ChildrenContent.m_AttachedUIDocuments);
				}
				foreach (UIDocument uidocument in this.m_ChildrenContentCopy)
				{
					bool isActiveAndEnabled2 = uidocument.isActiveAndEnabled;
					if (isActiveAndEnabled2)
					{
						bool flag6 = uidocument.m_RootVisualElement == null;
						if (flag6)
						{
							uidocument.RecreateUI();
						}
						else
						{
							this.AddChildAndInsertContentToVisualTree(uidocument);
						}
					}
				}
				this.m_ChildrenContentCopy.Clear();
			}
			this.SetupRootClassList();
		}

		internal void SetupPosition()
		{
			bool flag = this.m_RootVisualElement == null || this.m_ParentUI == null;
			if (!flag)
			{
				bool isTransformControlledByGameObject = this.isTransformControlledByGameObject;
				if (isTransformControlledByGameObject)
				{
					this.m_RootVisualElement.style.position = Position.Absolute;
				}
				else
				{
					this.m_RootVisualElement.style.position = this.m_Position;
				}
				this.m_ParentUI.AddChildAndInsertContentToVisualTree(this);
			}
		}

		private void SetupRootClassList()
		{
			bool flag = this.m_RootVisualElement == null;
			if (!flag)
			{
				bool flag2 = !this.isWorldSpace;
				if (flag2)
				{
					this.m_RootVisualElement.EnableInClassList("unity-ui-document__root", this.parentUI == null);
					this.m_RootVisualElement.style.position = StyleKeyword.Null;
					this.m_RootVisualElement.style.width = StyleKeyword.Null;
					this.m_RootVisualElement.style.height = StyleKeyword.Null;
				}
				else
				{
					this.SetupWorldSpaceSize();
				}
				this.SetupPosition();
			}
		}

		private void SetupWorldSpaceSize()
		{
			bool flag = this.m_RootVisualElement == null;
			if (!flag)
			{
				bool flag2 = !this.isTransformControlledByGameObject;
				if (flag2)
				{
					this.m_RootVisualElement.style.width = StyleKeyword.Null;
					this.m_RootVisualElement.style.height = StyleKeyword.Null;
				}
				else
				{
					bool flag3 = this.m_WorldSpaceSizeMode == UIDocument.WorldSpaceSizeMode.Fixed;
					if (flag3)
					{
						this.m_RootVisualElement.style.position = Position.Absolute;
						this.m_RootVisualElement.style.width = this.m_WorldSpaceWidth;
						this.m_RootVisualElement.style.height = this.m_WorldSpaceHeight;
					}
					else
					{
						this.m_RootVisualElement.style.position = Position.Absolute;
						this.m_RootVisualElement.style.width = StyleKeyword.Null;
						this.m_RootVisualElement.style.height = StyleKeyword.Null;
					}
				}
			}
		}

		private void AddRootVisualElementToTree()
		{
			bool flag = !base.enabled;
			if (!flag)
			{
				bool flag2 = this.parentUI != null;
				if (flag2)
				{
					this.parentUI.AddChildAndInsertContentToVisualTree(this);
				}
				else
				{
					bool flag3 = this.m_PanelSettings != null;
					if (flag3)
					{
						this.m_PanelSettings.AttachAndInsertUIDocumentToVisualTree(this);
					}
				}
			}
		}

		private void RemoveFromHierarchy()
		{
			bool flag = this.parentUI != null;
			if (flag)
			{
				this.parentUI.RemoveChild(this);
			}
			else
			{
				bool flag2 = this.m_PanelSettings != null;
				if (flag2)
				{
					this.m_PanelSettings.DetachUIDocument(this);
				}
			}
		}

		private void OnDisable()
		{
			UIDocument.EnabledDocumentCount--;
			PointerDeviceState.RemoveDocumentData(this);
			this.RemoveWorldSpaceCollider();
			bool flag = this.m_RootVisualElement != null;
			if (flag)
			{
				this.RemoveFromHierarchy();
				this.rootVisualElement = null;
			}
			UIRenderer uirenderer;
			bool flag2 = base.TryGetComponent<UIRenderer>(out uirenderer);
			if (flag2)
			{
				uirenderer.enabled = false;
			}
		}

		private void OnTransformChildrenChanged()
		{
			bool flag = this.m_ChildrenContent != null;
			if (flag)
			{
				bool flag2 = this.m_ChildrenContentCopy == null;
				if (flag2)
				{
					this.m_ChildrenContentCopy = new List<UIDocument>(this.m_ChildrenContent.m_AttachedUIDocuments);
				}
				else
				{
					this.m_ChildrenContentCopy.AddRange(this.m_ChildrenContent.m_AttachedUIDocuments);
				}
				foreach (UIDocument uidocument in this.m_ChildrenContentCopy)
				{
					uidocument.ReactToHierarchyChanged();
				}
				this.m_ChildrenContentCopy.Clear();
			}
		}

		private void OnTransformParentChanged()
		{
			this.ReactToHierarchyChanged();
		}

		internal void ReactToHierarchyChanged()
		{
			this.SetupFromHierarchy();
			bool flag = this.parentUI != null;
			if (flag)
			{
				this.panelSettings = this.parentUI.m_PanelSettings;
			}
			UIDocumentRootElement rootVisualElement = this.m_RootVisualElement;
			if (rootVisualElement != null)
			{
				rootVisualElement.RemoveFromHierarchy();
			}
			this.AddRootVisualElementToTree();
			this.SetupRootClassList();
		}

		internal const string k_RootStyleClassName = "unity-ui-document__root";

		internal const string k_VisualElementNameSuffix = "-container";

		internal const string k_EditorElementsWarningMessage = "The VisualTreeAsset contains editor-only elements that are incompatible at runtime.\nTo fix this, remove the editor elements from the VisualTreeAsset.";

		private const int k_DefaultSortingOrder = 0;

		private static int s_CurrentUIDocumentCounter;

		internal readonly int m_UIDocumentCreationIndex;

		internal static int EnabledDocumentCount;

		[SerializeField]
		private PanelSettings m_PanelSettings;

		private PanelSettings m_PreviousPanelSettings = null;

		[SerializeField]
		private UIDocument m_ParentUI;

		private UIDocumentList m_ChildrenContent = null;

		private List<UIDocument> m_ChildrenContentCopy = null;

		[SerializeField]
		private VisualTreeAsset sourceAsset;

		private UIDocumentRootElement m_RootVisualElement;

		internal int softPointerCaptures = 0;

		private int m_FirstChildInsertIndex;

		[SerializeField]
		private float m_SortingOrder = 0f;

		[SerializeField]
		private Position m_Position = Position.Relative;

		[SerializeField]
		private UIDocument.WorldSpaceSizeMode m_WorldSpaceSizeMode = UIDocument.WorldSpaceSizeMode.Fixed;

		[SerializeField]
		private float m_WorldSpaceWidth = 1920f;

		[SerializeField]
		private float m_WorldSpaceHeight = 1080f;

		[SerializeField]
		private PivotReferenceSize m_PivotReferenceSize;

		[SerializeField]
		private Pivot m_Pivot;

		[SerializeField]
		[HideInInspector]
		private BoxCollider m_WorldSpaceCollider;

		private bool m_RootHasWorldTransform;

		public enum WorldSpaceSizeMode
		{
			Dynamic,
			Fixed
		}
	}
}
