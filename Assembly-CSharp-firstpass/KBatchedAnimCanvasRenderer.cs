using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class KBatchedAnimCanvasRenderer : MonoBehaviour, IMaskable
{
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
			this.uiMat = new Material(this.batch.group.material);
			Texture texture = this.batch.matProperties.GetTexture("instanceTex");
			this.uiMat.SetTexture("instanceTex", texture);
			this.uiMat.SetVector("INSTANCE_TEXEL_SIZE", texture.texelSize);
			this.uiMat.SetVector("INSTANCE_TEXTURE_SIZE", new Vector2((float)texture.width, (float)texture.height));
			Texture texture2 = this.batch.matProperties.GetTexture("animTex");
			this.uiMat.SetTexture("animTex", texture2);
			this.uiMat.SetVector("ANIM_TEXEL_SIZE", texture2.texelSize);
			this.uiMat.SetVector("ANIM_TEXTURE_SIZE", new Vector2((float)texture2.width, (float)texture2.height));
			Texture texture3 = this.batch.matProperties.GetTexture("buildTex");
			this.uiMat.SetTexture("buildTex", texture3);
			this.uiMat.SetVector("BUILD_TEXEL_SIZE", texture3.texelSize);
			this.uiMat.SetVector("BUILD_TEXTURE_SIZE", new Vector2((float)texture3.width, (float)texture3.height));
			for (int i = 0; i < 12; i++)
			{
				Texture texture4 = this.batch.matProperties.GetTexture(KBatchedAnimCanvasRenderer.atlasNames[i]);
				if (texture4 != null)
				{
					this.uiMat.SetTexture(KBatchedAnimCanvasRenderer.atlasNames[i], texture4);
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

	private Vector4 _ClipRect = new Vector4(0f, 0f, 0f, 1f);
}
