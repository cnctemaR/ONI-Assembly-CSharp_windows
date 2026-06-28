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
		CellOffset cellOffset;
		switch (this.direction)
		{
		case PlantRenderer.Direction.Up:
			cellOffset = new CellOffset(0, 1);
			break;
		case PlantRenderer.Direction.Down:
			cellOffset = new CellOffset(0, -1);
			break;
		case PlantRenderer.Direction.Left:
			cellOffset = new CellOffset(-1, 0);
			break;
		case PlantRenderer.Direction.Right:
			cellOffset = new CellOffset(1, 0);
			break;
		default:
			cellOffset = new CellOffset(1, 0);
			break;
		}
		return cellOffset;
	}

	private float GetRotation()
	{
		float num;
		switch (this.direction)
		{
		case PlantRenderer.Direction.Up:
			num = 0f;
			break;
		case PlantRenderer.Direction.Down:
			num = 180f;
			break;
		case PlantRenderer.Direction.Left:
			num = 90f;
			break;
		case PlantRenderer.Direction.Right:
			num = 270f;
			break;
		default:
			num = 0f;
			break;
		}
		return num;
	}

	private Vector3 GetRotationOffset()
	{
		Vector3 vector;
		switch (this.direction)
		{
		case PlantRenderer.Direction.Up:
			vector = Vector3.zero;
			break;
		case PlantRenderer.Direction.Down:
			vector = new Vector3(0f, 1f, 0f);
			break;
		case PlantRenderer.Direction.Left:
			vector = new Vector3(0.5f, 0f, 0f);
			break;
		case PlantRenderer.Direction.Right:
			vector = new Vector3(-0.5f, 0f, 0f);
			break;
		default:
			vector = Vector3.zero;
			break;
		}
		return vector;
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
		string text;
		if (this.isDead)
		{
			text = "dead";
		}
		else if (this.stage == 0)
		{
			text = "seedling_ground";
		}
		else if (this.stage == 1)
		{
			if (piece_id == 0)
			{
				text = "open";
			}
			else
			{
				text = null;
			}
		}
		else if (this.stage - 1 == piece_id)
		{
			text = "open";
		}
		else if (piece_id < this.stage)
		{
			text = "vine_idle";
		}
		else
		{
			text = null;
		}
		return text;
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
		if (this.enableRefresh)
		{
			this.Clear();
			int num = Grid.PosToCell(this);
			KAnimFile kanimFile = this.aliveAnims;
			if (this.IsDead())
			{
				kanimFile = this.deadAnims;
			}
			if (!(kanimFile == null))
			{
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
