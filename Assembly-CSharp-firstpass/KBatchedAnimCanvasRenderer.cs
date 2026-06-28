using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class KBatchedAnimCanvasRenderer : MonoBehaviour, IMaskable
{
	public CanvasRenderer canvass { get; private set; }

	public CompareFunction compare
	{
		get
		{
			return this._cmp;
		}
		set
		{
			if (this._cmp != value)
			{
				if (this.uiMat != null)
				{
					this.uiMat.SetInt("_StencilComp", (int)value);
				}
				this._cmp = value;
			}
		}
	}

	public StencilOp stencilOp
	{
		get
		{
			return this._op;
		}
		set
		{
			if (this._op != value)
			{
				if (this.uiMat != null)
				{
					this.uiMat.SetInt("_StencilOp", (int)this._op);
				}
				this._op = value;
			}
		}
	}

	void IMaskable.RecalculateMasking()
	{
		Mask componentInParent = base.GetComponentInParent<Mask>();
		if (componentInParent != null && componentInParent.enabled)
		{
			this.compare = CompareFunction.Equal;
			this.stencilOp = StencilOp.Keep;
		}
		else
		{
			this.compare = CompareFunction.Disabled;
			this.stencilOp = StencilOp.Keep;
		}
		if (this.uiMat != null)
		{
			this.uiMat.SetInt("_StencilComp", (int)this.compare);
			this.uiMat.SetInt("_StencilOp", (int)this.stencilOp);
		}
	}

	public void SetBatch(KAnimConverter.IAnimConverter conv)
	{
		this.converter = conv;
		if (conv != null)
		{
			this.batch = conv.GetBatch();
		}
		else
		{
			this.batch = null;
			if (this.uiMat != null)
			{
				global::UnityEngine.Object.Destroy(this.uiMat);
				this.uiMat = null;
			}
		}
		if (this.batch != null)
		{
			this.canvass = base.GetComponent<CanvasRenderer>();
			if (this.canvass == null)
			{
				this.canvass = base.gameObject.AddComponent<CanvasRenderer>();
			}
			this.rootRectTransform = base.GetComponent<RectTransform>();
			if (this.rootRectTransform == null)
			{
				this.rootRectTransform = base.gameObject.AddComponent<RectTransform>();
			}
			if (!this.batch.group.InitOK)
			{
				return;
			}
			this.canvass.Clear();
			this.canvass.SetMesh(this.batch.group.mesh);
			this.canvass.materialCount = 1;
			if (this.uiMat != null)
			{
				global::UnityEngine.Object.Destroy(this.uiMat);
				this.uiMat = null;
			}
			Material material = this.batch.group.GetMaterial(this.batch.materialType);
			this.uiMat = new Material(material);
			this.uiMat.SetFloat(KAnimBatchGroup.ShaderProperty_SYMBOLS_PER_BUILD, material.GetFloat(KAnimBatchGroup.ShaderProperty_SYMBOLS_PER_BUILD));
			if (KBatchedAnimCanvasRenderer.texturesToCopy == null)
			{
				KBatchedAnimCanvasRenderer.texturesToCopy = new KBatchedAnimCanvasRenderer.TextureTopCopyEntry[]
				{
					new KBatchedAnimCanvasRenderer.TextureTopCopyEntry
					{
						textureId = Shader.PropertyToID("instanceTex"),
						sizeId = Shader.PropertyToID("INSTANCE_TEXTURE_SIZE")
					},
					new KBatchedAnimCanvasRenderer.TextureTopCopyEntry
					{
						textureId = Shader.PropertyToID("animTex"),
						sizeId = Shader.PropertyToID("ANIM_TEXTURE_SIZE")
					},
					new KBatchedAnimCanvasRenderer.TextureTopCopyEntry
					{
						textureId = Shader.PropertyToID("buildTex"),
						sizeId = Shader.PropertyToID("BUILD_TEXTURE_SIZE")
					},
					new KBatchedAnimCanvasRenderer.TextureTopCopyEntry
					{
						textureId = Shader.PropertyToID("symbolInstanceTex"),
						sizeId = Shader.PropertyToID("SYMBOL_INSTANCE_TEXTURE_SIZE")
					}
				};
			}
			foreach (KBatchedAnimCanvasRenderer.TextureTopCopyEntry textureTopCopyEntry in KBatchedAnimCanvasRenderer.texturesToCopy)
			{
				this.uiMat.SetTexture(textureTopCopyEntry.textureId, this.batch.matProperties.GetTexture(textureTopCopyEntry.textureId));
				this.uiMat.SetVector(textureTopCopyEntry.sizeId, this.batch.matProperties.GetVector(textureTopCopyEntry.sizeId));
			}
			for (int j = 0; j < 12; j++)
			{
				Texture texture = this.batch.matProperties.GetTexture(KBatchedAnimCanvasRenderer.atlasNames[j]);
				if (texture != null)
				{
					this.uiMat.SetTexture(KBatchedAnimCanvasRenderer.atlasNames[j], texture);
				}
			}
			((IMaskable)this).RecalculateMasking();
			this.canvass.SetMaterial(this.uiMat, 0);
		}
	}

	private void Update()
	{
		if (this.batch != null)
		{
			if (base.transform.hasChanged)
			{
				this.batch.SetDirty(this.converter);
				base.transform.hasChanged = false;
			}
			this._ClipRect.x = this.rootRectTransform.rect.xMin;
			this._ClipRect.y = this.rootRectTransform.rect.yMin;
			this._ClipRect.z = this.rootRectTransform.rect.xMax;
			this._ClipRect.w = this.rootRectTransform.rect.yMax;
			Texture texture = this.batch.matProperties.GetTexture("instanceTex");
			this.uiMat.SetTexture("instanceTex", texture);
			this.uiMat.SetTexture("symbolInstanceTex", this.batch.matProperties.GetTexture("symbolInstanceTex"));
		}
	}

	private RectTransform rootRectTransform;

	public KAnimBatch batch;

	public Material uiMat;

	public static string[] atlasNames = new string[]
	{
		"atlas0", "atlas1", "atlas2", "atlas3", "atlas4", "atlas5", "atlas6", "atlas7", "atlas8", "atlas9",
		"atlas10", "atlas11", "atlas12"
	};

	private KAnimConverter.IAnimConverter converter;

	private CompareFunction _cmp = CompareFunction.Never;

	private StencilOp _op = StencilOp.Zero;

	private static KBatchedAnimCanvasRenderer.TextureTopCopyEntry[] texturesToCopy = null;

	private Vector4 _ClipRect = new Vector4(0f, 0f, 0f, 1f);

	private struct TextureTopCopyEntry
	{
		public int textureId;

		public int sizeId;
	}
}
