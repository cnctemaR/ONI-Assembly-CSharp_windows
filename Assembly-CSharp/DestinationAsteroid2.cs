using System;
using ProcGen;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/DestinationAsteroid2")]
public class DestinationAsteroid2 : KMonoBehaviour
{
	public event Action<ColonyDestinationAsteroidBeltData> OnClicked;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.button.onClick += this.OnClickInternal;
	}

	public void SetAsteroid(ColonyDestinationAsteroidBeltData newAsteroidData)
	{
		if (this.asteroidData == null || newAsteroidData.beltPath != this.asteroidData.beltPath)
		{
			this.asteroidData = newAsteroidData;
			global::ProcGen.World getStartWorld = newAsteroidData.GetStartWorld;
			KAnimFile kanimFile;
			Assets.TryGetAnim(getStartWorld.asteroidIcon.IsNullOrWhiteSpace() ? AsteroidGridEntity.DEFAULT_ASTEROID_ICON_ANIM : getStartWorld.asteroidIcon, out kanimFile);
			if (kanimFile != null)
			{
				this.asteroidImage.gameObject.SetActive(false);
				this.animController.AnimFiles = new KAnimFile[] { kanimFile };
				this.animController.initialMode = KAnim.PlayMode.Loop;
				this.animController.initialAnim = "idle_loop";
				this.animController.gameObject.SetActive(true);
				if (this.animController.HasAnimation(this.animController.initialAnim))
				{
					this.animController.Play(this.animController.initialAnim, KAnim.PlayMode.Loop, 1f, 0f);
					return;
				}
			}
			else
			{
				this.animController.gameObject.SetActive(false);
				this.asteroidImage.gameObject.SetActive(true);
				this.asteroidImage.sprite = this.asteroidData.sprite;
			}
		}
	}

	private void OnClickInternal()
	{
		DebugUtil.LogArgs(new object[]
		{
			"Clicked asteroid belt",
			this.asteroidData.beltPath
		});
		this.OnClicked(this.asteroidData);
	}

	[SerializeField]
	private Image asteroidImage;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private KBatchedAnimController animController;

	private ColonyDestinationAsteroidBeltData asteroidData;
}
