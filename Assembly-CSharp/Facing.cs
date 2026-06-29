using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Facing : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.log = new LoggerFS("Facing");
	}

	public void Face(float target_x)
	{
		float x = base.transform.GetLocalPosition().x;
		if (target_x < x)
		{
			this.facingLeft = true;
			this.UpdateMirror();
		}
		else if (target_x > x)
		{
			this.facingLeft = false;
			this.UpdateMirror();
		}
	}

	public void Face(Vector3 target_pos)
	{
		int num = Grid.CellColumn(Grid.PosToCell(base.transform.GetLocalPosition()));
		int num2 = Grid.CellColumn(Grid.PosToCell(target_pos));
		if (num > num2)
		{
			this.facingLeft = true;
			this.UpdateMirror();
		}
		else if (num2 > num)
		{
			this.facingLeft = false;
			this.UpdateMirror();
		}
	}

	[ContextMenu("Flip")]
	public void SwapFacing()
	{
		this.facingLeft = !this.facingLeft;
		this.UpdateMirror();
	}

	private void UpdateMirror()
	{
		if (this.kanimController != null && this.kanimController.FlipX != this.facingLeft)
		{
			this.kanimController.FlipX = this.facingLeft;
			if (this.facingLeft)
			{
			}
		}
	}

	public bool GetFacing()
	{
		return this.facingLeft;
	}

	public void SetFacing(bool mirror_x)
	{
		this.facingLeft = mirror_x;
		this.UpdateMirror();
	}

	public int GetFrontCell()
	{
		int num = Grid.PosToCell(this);
		if (this.GetFacing())
		{
			return Grid.CellLeft(num);
		}
		return Grid.CellRight(num);
	}

	[MyCmpGet]
	private KAnimControllerBase kanimController;

	private LoggerFS log;

	private bool facingLeft;
}
