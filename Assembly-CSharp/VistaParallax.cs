using System;

public class VistaParallax : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.maskKanim.Stop();
		this.maskKanim.AnimFiles = new KAnimFile[] { Assets.GetAnim("beachbg_parallax_kanim") };
		this.maskKanim.Play(VistaParallax.maskAnimationName, KAnim.PlayMode.Paused, 1f, 0f);
		base.transform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.transform, true);
		this.SetLayer("beachbg_parallax_kanim", 0);
	}

	public void SetLayer(string animation, int layer)
	{
		VistaParallax.BgLayer bgLayer = this.layers[layer];
		bgLayer.kbac.Stop();
		bgLayer.kbac.AnimFiles = new KAnimFile[] { Assets.GetAnim("beachbg_parallax_kanim") };
		bgLayer.kbac.Play("layer" + layer.ToString(), KAnim.PlayMode.Once, 1f, 0f);
	}

	public VistaParallax.BgLayer[] layers;

	public KBatchedAnimController maskKanim;

	private static readonly string maskAnimationName = "mask";

	[Serializable]
	public class BgLayer
	{
		public KBatchedAnimController kbac;

		public float distance;
	}
}
