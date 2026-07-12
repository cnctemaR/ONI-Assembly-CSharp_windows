using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LightShapePreview")]
public class LightShapePreview : KMonoBehaviour
{
	private void Update()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (num != this.previousCell)
		{
			this.previousCell = num;
			LightGridManager.DestroyPreview();
			LightGridManager.CreatePreview(Grid.OffsetCell(num, this.offset), this.radius, this.shape, this.lux, this.width, this.direction);
		}
	}

	protected override void OnCleanUp()
	{
		LightGridManager.DestroyPreview();
	}

	public float radius;

	public int lux;

	public int width;

	public DiscreteShadowCaster.Direction direction;

	public global::LightShape shape;

	public CellOffset offset;

	private int previousCell = -1;
}
