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
			if (this.uiMat != null)
			{
				global::UnityEngine.Object.Destroy(this.uiMat);
				this.uiMat = null;
			}
			Material material = this.batch.group.GetMaterial(this.batch.materialType);
			this.uiMat = new Material(material);
			((IMaskable)this).RecalculateMasking();
		}
	}

	private void UpdateCanvas()
	{
		this.canvass.Clear();
		this.canvass.SetMesh(this.batch.group.mesh);
		this.canvass.materialCount = 1;
		this.canvass.SetMaterial(this.uiMat, 0);
	}

	private void CopyPropertyBlockToMaterial()
	{
		if (KBatchedAnimCanvasRenderer.texturesToCopy == null)
		{
			KBatchedAnimCanvasRenderer.texturesToCopy = new KBatchedAnimCanvasRenderer.TextureToCopyEntry[]
			{
				new KBatchedAnimCanvasRenderer.TextureToCopyEntry
				{
					textureId = Shader.PropertyToID("instanceTex"),
					sizeId = Shader.PropertyToID("INSTANCE_TEXTURE_SIZE")
				},
				new KBatchedAnimCanvasRenderer.TextureToCopyEntry
				{
					textureId = Shader.PropertyToID("buildAndAnimTex"),
					sizeId = Shader.PropertyToID("BUILD_AND_ANIM_TEXTURE_SIZE")
				},
				new KBatchedAnimCanvasRenderer.TextureToCopyEntry
				{
					textureId = Shader.PropertyToID("symbolInstanceTex"),
					sizeId = Shader.PropertyToID("SYMBOL_INSTANCE_TEXTURE_SIZE")
				},
				new KBatchedAnimCanvasRenderer.TextureToCopyEntry
				{
					textureId = Shader.PropertyToID("symbolOverrideInfoTex"),
					sizeId = Shader.PropertyToID("SYMBOL_OVERRIDE_INFO_TEXTURE_SIZE")
				}
			};
		}
		foreach (KBatchedAnimCanvasRenderer.TextureToCopyEntry textureToCopyEntry in KBatchedAnimCanvasRenderer.texturesToCopy)
		{
			this.uiMat.SetTexture(textureToCopyEntry.textureId, this.batch.matProperties.GetTexture(textureToCopyEntry.textureId));
			this.uiMat.SetVector(textureToCopyEntry.sizeId, this.batch.matProperties.GetVector(textureToCopyEntry.sizeId));
		}
		for (int j = 0; j < KAnimBatchManager.instance.atlasNames.Length; j++)
		{
			Texture texture = this.batch.matProperties.GetTexture(KAnimBatchManager.instance.atlasNames[j]);
			if (texture != null)
			{
				this.uiMat.SetTexture(KAnimBatchManager.instance.atlasNames[j], texture);
			}
		}
		foreach (KBatchedAnimCanvasRenderer.TextureToCopyEntry textureToCopyEntry2 in KBatchedAnimCanvasRenderer.texturesToCopy)
		{
			this.uiMat.SetTexture(textureToCopyEntry2.textureId, this.batch.matProperties.GetTexture(textureToCopyEntry2.textureId));
			this.uiMat.SetVector(textureToCopyEntry2.sizeId, this.batch.matProperties.GetVector(textureToCopyEntry2.sizeId));
		}
		for (int l = 0; l < KAnimBatchManager.instance.atlasNames.Length; l++)
		{
			Texture texture2 = this.batch.matProperties.GetTexture(KAnimBatchManager.instance.atlasNames[l]);
			if (texture2 != null)
			{
				this.uiMat.SetTexture(KAnimBatchManager.instance.atlasNames[l], texture2);
			}
		}
		this.uiMat.SetFloat(KAnimBatch.ShaderProperty_SUPPORTS_SYMBOL_OVERRIDING, this.batch.matProperties.GetFloat(KAnimBatch.ShaderProperty_SUPPORTS_SYMBOL_OVERRIDING));
		this.uiMat.SetFloat(KAnimBatch.ShaderProperty_ANIM_TEXTURE_START_OFFSET, this.batch.matProperties.GetFloat(KAnimBatch.ShaderProperty_ANIM_TEXTURE_START_OFFSET));
	}

	private void LateUpdate()
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
			this.UpdateCanvas();
			this.CopyPropertyBlockToMaterial();
		}
	}

	private RectTransform rootRectTransform;

	public KAnimBatch batch;

	public Material uiMat;

	private KAnimConverter.IAnimConverter converter;

	private CompareFunction _cmp = CompareFunction.Never;

	private StencilOp _op = StencilOp.Zero;

	private static KBatchedAnimCanvasRenderer.TextureToCopyEntry[] texturesToCopy;

	private Vector4 _ClipRect = new Vector4(0f, 0f, 0f, 1f);

	private struct TextureToCopyEntry
	{
		public int textureId;

		public int sizeId;
	}
}
