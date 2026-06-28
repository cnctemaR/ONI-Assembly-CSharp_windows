using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantRenderer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.Refresh();
	}

	public PlantRenderer.Direction GetDirection()
	{
		return this.direction;
	}

	public void SetDirection(PlantRenderer.Direction direction)
	{
		if (direction != this.direction)
		{
			this.direction = direction;
			this.Refresh();
		}
	}

	public KAnimFile GetAliveAnims()
	{
		return this.aliveAnims;
	}

	public void SetAliveAnims(KAnimFile anims)
	{
		if (anims != this.aliveAnims)
		{
			this.aliveAnims = anims;
			this.Refresh();
		}
	}

	public KAnimFile GetDeadAnims()
	{
		return this.deadAnims;
	}

	public void SetDeadAnims(KAnimFile anims)
	{
		if (anims != this.deadAnims)
		{
			this.deadAnims = anims;
			this.Refresh();
		}
	}

	public bool IsDead()
	{
		return this.isDead;
	}

	public void SetDead(bool is_dead)
	{
		if (is_dead != this.isDead)
		{
			this.isDead = is_dead;
			this.Refresh();
		}
	}

	public int GetStage()
	{
		return this.stage;
	}

	public void SetStage(int stage)
	{
		if (this.stage != stage)
		{
			this.stage = stage;
			this.Refresh();
		}
	}

	protected override void OnCleanUp()
	{
		this.Clear();
	}

	private void Clear()
	{
		foreach (KBatchedAnimController kbatchedAnimController in this.controllers)
		{
			kbatchedAnimController.gameObject.DeleteObject();
		}
		this.controllers.Clear();
	}

	private CellOffset GetOffset()
	{
		switch (this.direction)
		{
		case PlantRenderer.Direction.Up:
			return new CellOffset(0, 1);
		case PlantRenderer.Direction.Down:
			return new CellOffset(0, -1);
		case PlantRenderer.Direction.Left:
			return new CellOffset(-1, 0);
		case PlantRenderer.Direction.Right:
			return new CellOffset(1, 0);
		default:
			return new CellOffset(1, 0);
		}
	}

	private float GetRotation()
	{
		switch (this.direction)
		{
		case PlantRenderer.Direction.Up:
			return 0f;
		case PlantRenderer.Direction.Down:
			return 180f;
		case PlantRenderer.Direction.Left:
			return 90f;
		case PlantRenderer.Direction.Right:
			return 270f;
		default:
			return 0f;
		}
	}

	private Vector3 GetRotationOffset()
	{
		switch (this.direction)
		{
		case PlantRenderer.Direction.Up:
			return Vector3.zero;
		case PlantRenderer.Direction.Down:
			return new Vector3(0f, 1f, 0f);
		case PlantRenderer.Direction.Left:
			return new Vector3(0.5f, 0f, 0f);
		case PlantRenderer.Direction.Right:
			return new Vector3(-0.5f, 0f, 0f);
		default:
			return Vector3.zero;
		}
	}

	public void SetEnableRefresh(bool enable_refresh)
	{
		if (this.enableRefresh != enable_refresh)
		{
			this.enableRefresh = enable_refresh;
			if (this.enableRefresh)
			{
				this.Refresh();
			}
		}
	}

	private string GetAnim(int piece_id)
	{
		if (this.isDead)
		{
			return "dead";
		}
		if (this.stage == 0)
		{
			return "seedling_ground";
		}
		if (this.stage == 1)
		{
			if (piece_id == 0)
			{
				return "open";
			}
			return null;
		}
		else
		{
			if (this.stage - 1 == piece_id)
			{
				return "open";
			}
			if (piece_id < this.stage)
			{
				return "vine_idle";
			}
			return null;
		}
	}

	private void CreateController(KAnimFile anims, int cell, float rotation, string anim)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = base.name + "Controller";
		gameObject.transform.parent = base.transform;
		gameObject.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move) + this.GetRotationOffset());
		gameObject.AddComponent<KPrefabID>().PrefabTag = GameTags.PlantRenderer;
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.AddAnims(new KAnimFile[] { anims });
		kbatchedAnimController.Play(anim, KAnim.PlayMode.Once, 1f, 0f);
		gameObject.transform.eulerAngles = new Vector3(0f, 0f, rotation);
		this.controllers.Add(kbatchedAnimController);
	}

	private void Refresh()
	{
		if (!this.enableRefresh)
		{
			return;
		}
		this.Clear();
		int num = Grid.PosToCell(this);
		KAnimFile kanimFile = this.aliveAnims;
		if (this.IsDead())
		{
			kanimFile = this.deadAnims;
		}
		if (kanimFile == null)
		{
			return;
		}
		CellOffset offset = this.GetOffset();
		float rotation = this.GetRotation();
		for (int i = 0; i <= this.stage; i++)
		{
			string anim = this.GetAnim(i);
			if (anim != null)
			{
				int num2 = Grid.OffsetCell(num, i * offset);
				this.CreateController(kanimFile, num2, rotation, anim);
			}
		}
	}

	[SerializeField]
	private int stage;

	[SerializeField]
	private bool isDead;

	[SerializeField]
	private KAnimFile aliveAnims;

	[SerializeField]
	private KAnimFile deadAnims;

	[SerializeField]
	private PlantRenderer.Direction direction;

	private bool enableRefresh = true;

	private List<KBatchedAnimController> controllers = new List<KBatchedAnimController>();

	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}
}
