using System;
using UnityEngine;

public class Vista : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.transform.SetPosition(new Vector3(base.transform.position.x, base.transform.position.y, Grid.GetLayerZ(this.sceneLayer)));
		this.visualizer = global::UnityEngine.Object.Instantiate<GameObject>(Assets.instance.vistasPrefabs.Find((GameObject p) => p.name == this.prefabName));
		Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y + (float)this.height / 2f, base.transform.position.z);
		this.visualizer.transform.position = vector;
		this.visualizer.transform.SetParent(base.transform, true);
		this.visualizer.gameObject.SetActive(true);
		if (!string.IsNullOrEmpty(this.audioName))
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StartSound(GlobalAssets.GetSound(this.audioName, false));
			}
		}
	}

	public string prefabName;

	public string audioName;

	public Grid.SceneLayer sceneLayer;

	public GameObject visualizer;

	public int width;

	public int height;
}
