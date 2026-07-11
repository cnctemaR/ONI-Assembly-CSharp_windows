using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class VisualTreeRepaintUpdater : BaseVisualTreeUpdater
	{
		public override string description
		{
			get
			{
				return "Repaint";
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			if ((versionChangeType & VersionChangeType.Repaint) == VersionChangeType.Repaint)
			{
				this.m_RepaintList.Add(ve);
			}
			this.PropagateToParents(ve);
		}

		public override void Update()
		{
			this.m_WhinedOnceAboutRotatedClipSpaceThisFrame = false;
			Rect rect = ((!base.visualTree.ShouldClip()) ? GUIClip.topmostRect : base.visualTree.layout);
			this.PaintSubTree(base.visualTree, Matrix4x4.identity, false, false, rect);
			this.m_RepaintList.Clear();
		}

		private void PropagateToParents(VisualElement ve)
		{
			for (VisualElement visualElement = ve.shadow.parent; visualElement != null; visualElement = visualElement.shadow.parent)
			{
				if (!this.m_RepaintList.Add(visualElement))
				{
					break;
				}
			}
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

		internal bool ShouldUsePixelCache(VisualElement root)
		{
			return base.panel.allowPixelCaching && root.clippingOptions == VisualElement.ClippingOptions.ClipAndCacheContents && root.worldBound.size.magnitude > Mathf.Epsilon;
		}

		private void PaintSubTree(VisualElement root, Matrix4x4 offset, bool shouldClip, bool shouldCache, Rect currentGlobalClip)
		{
			if (root != null && root.panel == base.panel)
			{
				if (root.visible && root.style.opacity.GetSpecifiedValueOrDefault(1f) >= Mathf.Epsilon)
				{
					if (root.ShouldClip())
					{
						Rect rect = VisualElement.ComputeAAAlignedBound(root.rect, offset * root.worldTransform);
						if (!rect.Overlaps(currentGlobalClip))
						{
							return;
						}
						float num = Mathf.Max(rect.x, currentGlobalClip.x);
						float num2 = Mathf.Min(rect.xMax, currentGlobalClip.xMax);
						float num3 = Mathf.Max(rect.y, currentGlobalClip.y);
						float num4 = Mathf.Min(rect.yMax, currentGlobalClip.yMax);
						currentGlobalClip = new Rect(num, num3, num2 - num, num4 - num3);
						shouldClip = true;
						shouldCache = root.clippingOptions == VisualElement.ClippingOptions.ClipAndCacheContents;
					}
					if (!this.m_WhinedOnceAboutRotatedClipSpaceThisFrame && shouldClip && !shouldCache && this.DoesMatrixHaveUnsupportedRotation(root.worldTransform))
					{
						Debug.LogError("Panel.PaintSubTree - Rotated clip-spaces are only supported by the VisualElement.ClippingOptions.ClipAndCacheContents mode. First offending Panel:'" + root.name + "'.");
						this.m_WhinedOnceAboutRotatedClipSpaceThisFrame = true;
					}
					VisualElement currentElement = this.m_StylePainter.currentElement;
					this.m_StylePainter.currentElement = root;
					RepaintData repaintData = base.panel.repaintData;
					if (this.ShouldUsePixelCache(root))
					{
						Rect worldBound = root.worldBound;
						repaintData.currentWorldClip = currentGlobalClip;
						repaintData.currentOffset = offset;
						int num5;
						int num6;
						Rect rect2;
						using (new GUIClip.ParentClipScope(offset * root.worldTransform, currentGlobalClip))
						{
							rect2 = GUIUtility.AlignRectToDevice(root.rect, out num5, out num6);
						}
						num5 = Math.Max(num5, 1);
						num6 = Math.Max(num6, 1);
						RenderTexture renderTexture = root.renderData.pixelCache;
						if (renderTexture != null && (renderTexture.width != num5 || renderTexture.height != num6) && (!base.panel.keepPixelCacheOnWorldBoundChange || this.m_RepaintList.Contains(root)))
						{
							Object.DestroyImmediate(renderTexture);
							renderTexture = (root.renderData.pixelCache = null);
						}
						if (this.m_RepaintList.Contains(root) || root.renderData.pixelCache == null || !root.renderData.pixelCache.IsCreated())
						{
							if (renderTexture == null)
							{
								renderTexture = (root.renderData.pixelCache = new RenderTexture(num5, num6, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear));
							}
							bool flag = root.style.borderTopLeftRadius > 0f || root.style.borderTopRightRadius > 0f || root.style.borderBottomLeftRadius > 0f || root.style.borderBottomRightRadius > 0f;
							RenderTexture renderTexture2 = null;
							RenderTexture active = RenderTexture.active;
							try
							{
								if (flag)
								{
									renderTexture = (renderTexture2 = RenderTexture.GetTemporary(num5, num6, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear));
								}
								RenderTexture.active = renderTexture;
								GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
								Rect rect3 = root.LocalToWorld(rect2);
								Matrix4x4 matrix4x = Matrix4x4.Translate(new Vector3(-rect3.x, -rect3.y, 0f));
								Matrix4x4 matrix4x2 = matrix4x * root.worldTransform;
								Rect rect4 = new Rect(0f, 0f, rect3.width, rect3.height);
								repaintData.currentOffset = matrix4x;
								using (new GUIClip.ParentClipScope(matrix4x2, rect4))
								{
									repaintData.currentWorldClip = rect4;
									this.m_StylePainter.opacity = root.style.opacity.GetSpecifiedValueOrDefault(1f);
									root.Repaint(this.m_StylePainter);
									this.m_StylePainter.opacity = 1f;
									this.PaintSubTreeChildren(root, matrix4x, shouldClip, shouldCache, rect4);
								}
								if (flag)
								{
									RenderTexture.active = root.renderData.pixelCache;
									bool manualTex2SRGBEnabled = GUIUtility.manualTex2SRGBEnabled;
									GUIUtility.manualTex2SRGBEnabled = false;
									using (new GUIClip.ParentClipScope(Matrix4x4.identity, rect4))
									{
										GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
										TextureStylePainterParameters @default = TextureStylePainterParameters.GetDefault(root);
										@default.color = Color.white;
										@default.texture = renderTexture;
										@default.scaleMode = ScaleMode.StretchToFill;
										@default.rect = rect4;
										@default.border.SetWidth(0f);
										Vector4 vector = new Vector4(1f, 0f, 0f, 0f);
										float x = (matrix4x2 * vector).x;
										@default.border.SetRadius(@default.border.topLeftRadius * x, @default.border.topRightRadius * x, @default.border.bottomRightRadius * x, @default.border.bottomLeftRadius * x);
										@default.usePremultiplyAlpha = true;
										this.m_StylePainter.DrawTexture(@default);
									}
									using (new GUIClip.ParentClipScope(matrix4x2, rect4))
									{
										this.m_StylePainter.DrawBorder();
									}
									GUIUtility.manualTex2SRGBEnabled = manualTex2SRGBEnabled;
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
						repaintData.currentWorldClip = currentGlobalClip;
						repaintData.currentOffset = offset;
						bool manualTex2SRGBEnabled2 = GUIUtility.manualTex2SRGBEnabled;
						GUIUtility.manualTex2SRGBEnabled = false;
						using (new GUIClip.ParentClipScope(offset * root.worldTransform, currentGlobalClip))
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
							Color editorPlayModeTintColor = UIElementsUtility.editorPlayModeTintColor;
							UIElementsUtility.editorPlayModeTintColor = Color.white;
							this.m_StylePainter.DrawTexture(textureStylePainterParameters);
							UIElementsUtility.editorPlayModeTintColor = editorPlayModeTintColor;
						}
						GUIUtility.manualTex2SRGBEnabled = manualTex2SRGBEnabled2;
					}
					else
					{
						repaintData.currentOffset = offset;
						using (new GUIClip.ParentClipScope(offset * root.worldTransform, currentGlobalClip))
						{
							repaintData.currentWorldClip = currentGlobalClip;
							repaintData.mousePosition = root.worldTransform.inverse.MultiplyPoint3x4(repaintData.repaintEvent.mousePosition);
							this.m_StylePainter.opacity = root.style.opacity.GetSpecifiedValueOrDefault(1f);
							root.Repaint(this.m_StylePainter);
							this.m_StylePainter.opacity = 1f;
							this.PaintSubTreeChildren(root, offset, shouldClip, shouldCache, currentGlobalClip);
						}
					}
					this.m_StylePainter.currentElement = currentElement;
				}
			}
		}

		private void PaintSubTreeChildren(VisualElement root, Matrix4x4 offset, bool shouldClip, bool shouldCache, Rect textureClip)
		{
			int childCount = root.shadow.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = root.shadow[i];
				this.PaintSubTree(visualElement, offset, shouldClip, shouldCache, textureClip);
				if (childCount != root.shadow.childCount)
				{
					throw new NotImplementedException("Visual tree is read-only during repaint");
				}
			}
		}

		private HashSet<VisualElement> m_RepaintList = new HashSet<VisualElement>();

		private bool m_WhinedOnceAboutRotatedClipSpaceThisFrame = false;

		private ImmediateStylePainter m_StylePainter = new ImmediateStylePainter();
	}
}
