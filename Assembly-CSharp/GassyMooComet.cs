using System;
using UnityEngine;

public class GassyMooComet : Comet
{
	public void SetCustomInitialFlip(bool state)
	{
		this.initialFlipState = new bool?(state);
	}

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
		if (this.initialFlipState != null)
		{
			flag = this.initialFlipState.Value;
		}
		float num2 = (flag ? (-75f) : (-105f)) * 3.1415927f / 180f;
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
				float num = 0f;
				int num2 = Grid.PosToCell(this.transform.GetPosition());
				int num3 = Grid.OffsetCell(num2, 0, 1);
				if (Grid.IsValidCell(num3))
				{
					num2 = num3;
				}
				if (Grid.Solid[num2])
				{
					bool flipX = animController.FlipX;
					int num4 = Grid.OffsetCell(num2, -1, 0);
					int num5 = Grid.OffsetCell(num2, 2, 0);
					if (!flipX && Grid.IsValidCell(num4) && !Grid.Solid[num4])
					{
						num2 = num4;
					}
					else if (flipX && Grid.IsValidCell(num5) && !Grid.Solid[num5])
					{
						num2 = num5;
					}
				}
				else
				{
					num = this.gameObject.transform.position.x - Mathf.Floor(this.gameObject.transform.position.x);
				}
				Vector3 vector = Grid.CellToPos(num2) + new Vector3(num, 0f, Grid.GetLayerZ(Grid.SceneLayer.Creatures));
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(this.craterPrefabs[global::UnityEngine.Random.Range(0, this.craterPrefabs.Length)]), vector);
				Vector3 vector2 = gameObject.transform.position + this.mooSpawnImpactOffset;
				if (!Grid.Solid[Grid.PosToCell(vector2)])
				{
					gameObject.transform.position = vector2;
				}
				gameObject.GetComponent<KBatchedAnimController>().FlipX = animController.FlipX;
				gameObject.SetActive(true);
			}
			Util.KDestroyGameObject(this.gameObject);
		};
	}

	public const float MOO_ANGLE = 15f;

	public Vector3 mooSpawnImpactOffset = new Vector3(-0.5f, 0f, 0f);

	private bool? initialFlipState;
}
