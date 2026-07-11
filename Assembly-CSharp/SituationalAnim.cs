using System;
using System.Collections.Generic;

public class SituationalAnim : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		SituationalAnim.Situation situation = this.GetSituation();
		Output.Log(new object[] { "Situation is", situation });
		this.SetAnimForSituation(situation);
	}

	private void SetAnimForSituation(SituationalAnim.Situation situation)
	{
		foreach (Tuple<SituationalAnim.Situation, string> tuple in this.anims)
		{
			if ((tuple.first & situation) == tuple.first)
			{
				Output.Log(new object[] { "Chose Anim", tuple.first, tuple.second });
				this.SetAnim(tuple.second);
				break;
			}
		}
	}

	private void SetAnim(string animName)
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.Play(animName, KAnim.PlayMode.Once, 1f, 0f);
	}

	private SituationalAnim.Situation GetSituation()
	{
		SituationalAnim.Situation situation = (SituationalAnim.Situation)0;
		Building component = base.GetComponent<Building>();
		Extents extents = component.GetExtents();
		int x = extents.x;
		int num = extents.x + extents.width - 1;
		int y = extents.y;
		int num2 = extents.y + extents.height - 1;
		if (this.DoesSatisfy(this.GetSatisfactionForEdge(x, num, y - 1, y - 1), this.mustSatisfy))
		{
			situation |= SituationalAnim.Situation.Bottom;
		}
		if (this.DoesSatisfy(this.GetSatisfactionForEdge(x - 1, x - 1, y, num2), this.mustSatisfy))
		{
			situation |= SituationalAnim.Situation.Left;
		}
		if (this.DoesSatisfy(this.GetSatisfactionForEdge(x, num, num2 + 1, num2 + 1), this.mustSatisfy))
		{
			situation |= SituationalAnim.Situation.Top;
		}
		if (this.DoesSatisfy(this.GetSatisfactionForEdge(num + 1, num + 1, y, num2), this.mustSatisfy))
		{
			situation |= SituationalAnim.Situation.Right;
		}
		return situation;
	}

	private bool DoesSatisfy(SituationalAnim.MustSatisfy result, SituationalAnim.MustSatisfy requirement)
	{
		if (requirement == SituationalAnim.MustSatisfy.All)
		{
			return result == SituationalAnim.MustSatisfy.All;
		}
		if (requirement == SituationalAnim.MustSatisfy.Any)
		{
			return result != SituationalAnim.MustSatisfy.None;
		}
		return result == SituationalAnim.MustSatisfy.None;
	}

	private SituationalAnim.MustSatisfy GetSatisfactionForEdge(int minx, int maxx, int miny, int maxy)
	{
		bool flag = false;
		bool flag2 = true;
		for (int i = minx; i <= maxx; i++)
		{
			for (int j = miny; j <= maxy; j++)
			{
				int num = Grid.XYToCell(i, j);
				if (this.test(num))
				{
					flag = true;
				}
				else
				{
					flag2 = false;
				}
			}
		}
		if (flag2)
		{
			return SituationalAnim.MustSatisfy.All;
		}
		if (flag)
		{
			return SituationalAnim.MustSatisfy.Any;
		}
		return SituationalAnim.MustSatisfy.None;
	}

	public List<Tuple<SituationalAnim.Situation, string>> anims;

	public Func<int, bool> test;

	public SituationalAnim.MustSatisfy mustSatisfy;

	[Flags]
	public enum Situation
	{
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8
	}

	public enum MustSatisfy
	{
		None,
		Any,
		All
	}
}
