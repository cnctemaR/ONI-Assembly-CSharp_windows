using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FrontEndManager")]
public class FrontEndManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		FrontEndManager.Instance = this;
		string highestActiveDlcId = DlcManager.GetHighestActiveDlcId();
		if (highestActiveDlcId == null || (highestActiveDlcId != null && highestActiveDlcId.Length == 0) || !(highestActiveDlcId == "EXPANSION1_ID"))
		{
			Util.KInstantiateUI(this.mainMenuVanilla, base.gameObject, true);
		}
		else
		{
			Util.KInstantiateUI(this.mainMenuExpansion1, base.gameObject, true);
		}
		if (this.SpawnOnLoadScreens != null && this.SpawnOnLoadScreens.Length != 0)
		{
			foreach (GameObject gameObject in this.SpawnOnLoadScreens)
			{
				if (gameObject != null)
				{
					Util.KInstantiateUI(gameObject, base.gameObject, true);
				}
			}
		}
		if (FrontEndManager.firstInit)
		{
			FrontEndManager.firstInit = false;
			if (this.SpawnOnLaunchScreens != null && this.SpawnOnLaunchScreens.Length != 0)
			{
				foreach (GameObject gameObject2 in this.SpawnOnLaunchScreens)
				{
					if (gameObject2 != null)
					{
						Util.KInstantiateUI(gameObject2, base.gameObject, true);
					}
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (global::Debug.developerConsoleVisible)
		{
			global::Debug.developerConsoleVisible = false;
		}
		KAnimBatchManager.Instance().UpdateActiveArea(new Vector2I(0, 0), new Vector2I(9999, 9999));
		KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
		KAnimBatchManager.Instance().Render();
	}

	public static FrontEndManager Instance;

	public static bool firstInit = true;

	public GameObject mainMenuVanilla;

	public GameObject mainMenuExpansion1;

	public GameObject[] SpawnOnLoadScreens;

	public GameObject[] SpawnOnLaunchScreens;
}
