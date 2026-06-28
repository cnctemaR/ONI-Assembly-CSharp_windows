using System;
using UnityEngine;

public class Exploder : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		int num = Grid.PosToCell(this);
		Vector3 vector = Grid.CellToPosCCC(num, Grid.SceneLayer.Building);
		vector.z = -Grid.CellSizeInMeters * 0.5f;
		this.PlayExplosion(vector);
		SimMessages.AddRemoveSubstance(num, this.result, CellEventLogger.Instance.ExploderOnSpawn, this.mass, this.temperature, -1);
		base.gameObject.DeleteObject();
	}

	private void PlayExplosion(Vector3 pos)
	{
		GameUtil.KInstantiate(EffectPrefabs.Instance.Explosion, pos, Grid.SceneLayer.Front, Folder.FX, null, 0);
	}

	public SimHashes result = SimHashes.CarbonDioxide;

	public float mass = 1000f;

	public float temperature = 1000f;
}
