using System;

public class LightShapePreview : KMonoBehaviour
{
	private void Update()
	{
		int num = Grid.PosToCell(this.transform.position);
		if (num != this.previousCell)
		{
			this.previousCell = num;
			LightGridManager.DestroyPreview();
			LightGridManager.CreatePreview(Grid.OffsetCell(num, this.offset), this.radius, this.shape);
		}
	}

	protected override void OnCleanUp()
	{
		LightGridManager.DestroyPreview();
	}

	public float radius;

	public LightShape shape;

	public CellOffset offset;

	private int previousCell = -1;
}
