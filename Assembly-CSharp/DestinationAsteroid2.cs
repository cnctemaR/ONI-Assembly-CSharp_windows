using System;
using UnityEngine;
using UnityEngine.UI;

public class DestinationAsteroid2 : KMonoBehaviour
{
	public event Action<ColonyDestinationAsteroidData> OnClicked;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.button.onClick += this.OnClickInternal;
	}

	public void SetAsteroid(ColonyDestinationAsteroidData newAsteroidData)
	{
		if (newAsteroidData != this.asteroidData)
		{
			this.asteroidData = newAsteroidData;
			this.asteroidImage.sprite = Assets.GetSprite(this.asteroidData.sprite);
		}
	}

	private void OnClickInternal()
	{
		DebugUtil.LogArgs(new object[]
		{
			"Clicked asteroid",
			this.asteroidData.worldPath
		});
		this.OnClicked(this.asteroidData);
	}

	[SerializeField]
	private Image asteroidImage;

	[SerializeField]
	private KButton button;

	private ColonyDestinationAsteroidData asteroidData;
}
