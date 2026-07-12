using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class DevToolEntityTarget
{
	public abstract string GetTag();

	[return: TupleElementNames(new string[] { "cornerA", "cornerB" })]
	public abstract Option<ValueTuple<Vector2, Vector2>> GetScreenRect();

	public string GetDebugName()
	{
		return "[" + this.GetTag() + "] " + this.ToString();
	}

	public class ForUIGameObject : DevToolEntityTarget
	{
		public ForUIGameObject(GameObject gameObject)
		{
			this.gameObject = gameObject;
		}

		[return: TupleElementNames(new string[] { "cornerA", "cornerB" })]
		public override Option<ValueTuple<Vector2, Vector2>> GetScreenRect()
		{
			if (this.gameObject.IsNullOrDestroyed())
			{
				return Option.None;
			}
			RectTransform component = this.gameObject.GetComponent<RectTransform>();
			if (component.IsNullOrDestroyed())
			{
				return Option.None;
			}
			Canvas componentInParent = this.gameObject.GetComponentInParent<Canvas>();
			if (component.IsNullOrDestroyed())
			{
				return Option.None;
			}
			if (!componentInParent.worldCamera.IsNullOrDestroyed())
			{
				DevToolEntityTarget.ForUIGameObject.<>c__DisplayClass2_0 CS$<>8__locals1;
				CS$<>8__locals1.camera = componentInParent.worldCamera;
				Vector3[] array = new Vector3[4];
				component.GetWorldCorners(array);
				return new ValueTuple<Vector2, Vector2>(DevToolEntityTarget.ForUIGameObject.<GetScreenRect>g__ScreenPointToScreenPosition|2_0(CS$<>8__locals1.camera.WorldToScreenPoint(array[0]), ref CS$<>8__locals1), DevToolEntityTarget.ForUIGameObject.<GetScreenRect>g__ScreenPointToScreenPosition|2_0(CS$<>8__locals1.camera.WorldToScreenPoint(array[2]), ref CS$<>8__locals1));
			}
			if (componentInParent.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				Vector3[] array2 = new Vector3[4];
				component.GetWorldCorners(array2);
				return new ValueTuple<Vector2, Vector2>(DevToolEntityTarget.ForUIGameObject.<GetScreenRect>g__ScreenPointToScreenPosition|2_1(array2[0]), DevToolEntityTarget.ForUIGameObject.<GetScreenRect>g__ScreenPointToScreenPosition|2_1(array2[2]));
			}
			return Option.None;
		}

		public override string GetTag()
		{
			return "UI";
		}

		public override string ToString()
		{
			return DevToolEntity.GetNameFor(this.gameObject);
		}

		[CompilerGenerated]
		internal static Vector2 <GetScreenRect>g__ScreenPointToScreenPosition|2_0(Vector2 coord, ref DevToolEntityTarget.ForUIGameObject.<>c__DisplayClass2_0 A_1)
		{
			return new Vector2(coord.x, (float)A_1.camera.pixelHeight - coord.y);
		}

		[CompilerGenerated]
		internal static Vector2 <GetScreenRect>g__ScreenPointToScreenPosition|2_1(Vector2 coord)
		{
			return new Vector2(coord.x, (float)Screen.height - coord.y);
		}

		public GameObject gameObject;
	}

	public class ForWorldGameObject : DevToolEntityTarget
	{
		public ForWorldGameObject(GameObject gameObject)
		{
			this.gameObject = gameObject;
		}

		[return: TupleElementNames(new string[] { "cornerA", "cornerB" })]
		public override Option<ValueTuple<Vector2, Vector2>> GetScreenRect()
		{
			if (this.gameObject.IsNullOrDestroyed())
			{
				return Option.None;
			}
			DevToolEntityTarget.ForWorldGameObject.<>c__DisplayClass2_0 CS$<>8__locals1;
			CS$<>8__locals1.camera = Camera.main;
			if (CS$<>8__locals1.camera.IsNullOrDestroyed())
			{
				return Option.None;
			}
			KCollider2D component = this.gameObject.GetComponent<KCollider2D>();
			if (component.IsNullOrDestroyed())
			{
				return Option.None;
			}
			return new ValueTuple<Vector2, Vector2>(DevToolEntityTarget.ForWorldGameObject.<GetScreenRect>g__ScreenPointToScreenPosition|2_0(CS$<>8__locals1.camera.WorldToScreenPoint(component.bounds.min), ref CS$<>8__locals1), DevToolEntityTarget.ForWorldGameObject.<GetScreenRect>g__ScreenPointToScreenPosition|2_0(CS$<>8__locals1.camera.WorldToScreenPoint(component.bounds.max), ref CS$<>8__locals1));
		}

		public override string GetTag()
		{
			return "World";
		}

		public override string ToString()
		{
			return DevToolEntity.GetNameFor(this.gameObject);
		}

		[CompilerGenerated]
		internal static Vector2 <GetScreenRect>g__ScreenPointToScreenPosition|2_0(Vector2 coord, ref DevToolEntityTarget.ForWorldGameObject.<>c__DisplayClass2_0 A_1)
		{
			return new Vector2(coord.x, (float)A_1.camera.pixelHeight - coord.y);
		}

		public GameObject gameObject;
	}

	public class ForSimCell : DevToolEntityTarget
	{
		public ForSimCell(int cellIndex)
		{
			this.cellIndex = cellIndex;
		}

		[return: TupleElementNames(new string[] { "cornerA", "cornerB" })]
		public override Option<ValueTuple<Vector2, Vector2>> GetScreenRect()
		{
			DevToolEntityTarget.ForSimCell.<>c__DisplayClass2_0 CS$<>8__locals1;
			CS$<>8__locals1.camera = Camera.main;
			if (CS$<>8__locals1.camera.IsNullOrDestroyed())
			{
				return Option.None;
			}
			Vector2 vector = Grid.CellToPosCCC(this.cellIndex, Grid.SceneLayer.Background);
			Vector2 vector2 = Grid.HalfCellSizeInMeters * Vector2.one;
			Vector2 vector3 = vector - vector2;
			Vector2 vector4 = vector + vector2;
			return new ValueTuple<Vector2, Vector2>(DevToolEntityTarget.ForSimCell.<GetScreenRect>g__ScreenPointToScreenPosition|2_0(CS$<>8__locals1.camera.WorldToScreenPoint(vector3), ref CS$<>8__locals1), DevToolEntityTarget.ForSimCell.<GetScreenRect>g__ScreenPointToScreenPosition|2_0(CS$<>8__locals1.camera.WorldToScreenPoint(vector4), ref CS$<>8__locals1));
		}

		public override string GetTag()
		{
			return "Sim Cell";
		}

		public override string ToString()
		{
			return this.cellIndex.ToString();
		}

		[CompilerGenerated]
		internal static Vector2 <GetScreenRect>g__ScreenPointToScreenPosition|2_0(Vector2 coord, ref DevToolEntityTarget.ForSimCell.<>c__DisplayClass2_0 A_1)
		{
			return new Vector2(coord.x, (float)A_1.camera.pixelHeight - coord.y);
		}

		public int cellIndex;
	}
}
