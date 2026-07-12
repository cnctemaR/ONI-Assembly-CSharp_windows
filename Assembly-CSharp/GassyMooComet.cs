using System;
using UnityEngine;

public class GassyMooComet : Comet
{
	public override void RandomizeVelocity()
	{
		bool flag = false;
		byte b = Grid.WorldIdx[Grid.PosToCell(base.gameObject.transform.position)];
		WorldContainer world = ClusterManager.Instance.GetWorld((int)b);
		if (world == null)
		{
			return;
		}
		int num = world.WorldOffset.x + world.Width / 2;
		if (Grid.PosToXY(base.gameObject.transform.position).x > num)
		{
			flag = true;
		}
		float num2 = (float)(flag ? (-75) : 255) * 3.1415927f / 180f;
		float num3 = global::UnityEngine.Random.Range(this.spawnVelocity.x, this.spawnVelocity.y);
		this.velocity = new Vector2(-Mathf.Cos(num2) * num3, Mathf.Sin(num2) * num3);
		base.GetComponent<KBatchedAnimController>().FlipX = flag;
	}

	protected override void SpawnCraterPrefabs()
	{
		KBatchedAnimController animController = base.GetComponent<KBatchedAnimController>();
		animController.Play("landing", KAnim.PlayMode.Once, 1f, 0f);
		animController.onAnimComplete += delegate(HashedString obj)
		{
			if (this.craterPrefabs != null && this.craterPrefabs.Length != 0)
			{
				int num = Grid.PosToCell(this);
				if (Grid.IsValidCell(Grid.CellAbove(num)))
				{
					num = Grid.CellAbove(num);
				}
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(this.craterPrefabs[global::UnityEngine.Random.Range(0, this.craterPrefabs.Length)]), Grid.CellToPos(num));
				gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
				gameObject.transform.position += this.mooSpawnImpactOffset;
				gameObject.GetComponent<KBatchedAnimController>().FlipX = animController.FlipX;
				gameObject.SetActive(true);
			}
			Util.KDestroyGameObject(this.gameObject);
		};
	}

	public Vector3 mooSpawnImpactOffset = new Vector3(-0.5f, 0f, 0f);
}
