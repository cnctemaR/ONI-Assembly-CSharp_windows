using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UILineRenderer")]
	public class UILineRenderer : UIPrimitiveBase
	{
		public Rect uvRect
		{
			get
			{
				return this.m_UVRect;
			}
			set
			{
				if (this.m_UVRect == value)
				{
					return;
				}
				this.m_UVRect = value;
				this.SetVerticesDirty();
			}
		}

		public Vector2[] Points
		{
			get
			{
				return this.m_points;
			}
			set
			{
				if (this.m_points == value)
				{
					return;
				}
				this.m_points = value;
				this.SetAllDirty();
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (this.m_points == null)
			{
				return;
			}
			Vector2[] array = this.m_points;
			if (this.BezierMode != UILineRenderer.BezierType.None && this.m_points.Length > 3)
			{
				BezierPath bezierPath = new BezierPath();
				bezierPath.SetControlPoints(array);
				bezierPath.SegmentsPerCurve = this.BezierSegmentsPerCurve;
				UILineRenderer.BezierType bezierMode = this.BezierMode;
				List<Vector2> list;
				if (bezierMode != UILineRenderer.BezierType.Basic)
				{
					if (bezierMode != UILineRenderer.BezierType.Improved)
					{
						list = bezierPath.GetDrawingPoints2();
					}
					else
					{
						list = bezierPath.GetDrawingPoints1();
					}
				}
				else
				{
					list = bezierPath.GetDrawingPoints0();
				}
				array = list.ToArray();
			}
			float num = base.rectTransform.rect.width;
			float num2 = base.rectTransform.rect.height;
			float num3 = -base.rectTransform.pivot.x * base.rectTransform.rect.width;
			float num4 = -base.rectTransform.pivot.y * base.rectTransform.rect.height;
			if (!this.relativeSize)
			{
				num = 1f;
				num2 = 1f;
			}
			if (this.UseMargins)
			{
				num -= this.Margin.x;
				num2 -= this.Margin.y;
				num3 += this.Margin.x / 2f;
				num4 += this.Margin.y / 2f;
			}
			vh.Clear();
			List<UIVertex[]> list2 = new List<UIVertex[]>();
			if (this.LineList)
			{
				for (int i = 1; i < array.Length; i += 2)
				{
					Vector2 vector = array[i - 1];
					Vector2 vector2 = array[i];
					vector = new Vector2(vector.x * num + num3, vector.y * num2 + num4);
					vector2 = new Vector2(vector2.x * num + num3, vector2.y * num2 + num4);
					if (this.LineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRenderer.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector, vector2, UILineRenderer.SegmentType.Middle));
					if (this.LineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRenderer.SegmentType.End));
					}
				}
			}
			else
			{
				for (int j = 1; j < array.Length; j++)
				{
					Vector2 vector3 = array[j - 1];
					Vector2 vector4 = array[j];
					vector3 = new Vector2(vector3.x * num + num3, vector3.y * num2 + num4);
					vector4 = new Vector2(vector4.x * num + num3, vector4.y * num2 + num4);
					if (this.LineCaps && j == 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRenderer.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector3, vector4, UILineRenderer.SegmentType.Middle));
					if (this.LineCaps && j == array.Length - 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRenderer.SegmentType.End));
					}
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				if (!this.LineList && k < list2.Count - 1)
				{
					Vector3 vector5 = list2[k][1].position - list2[k][2].position;
					Vector3 vector6 = list2[k + 1][2].position - list2[k + 1][1].position;
					float num5 = Vector2.Angle(vector5, vector6) * 0.017453292f;
					float num6 = Mathf.Sign(Vector3.Cross(vector5.normalized, vector6.normalized).z);
					float num7 = this.LineThickness / (2f * Mathf.Tan(num5 / 2f));
					Vector3 vector7 = list2[k][2].position - vector5.normalized * num7 * num6;
					Vector3 vector8 = list2[k][3].position + vector5.normalized * num7 * num6;
					UILineRenderer.JoinType joinType = this.LineJoins;
					if (joinType == UILineRenderer.JoinType.Miter)
					{
						if (num7 < vector5.magnitude / 2f && num7 < vector6.magnitude / 2f && num5 > 0.2617994f)
						{
							list2[k][2].position = vector7;
							list2[k][3].position = vector8;
							list2[k + 1][0].position = vector8;
							list2[k + 1][1].position = vector7;
						}
						else
						{
							joinType = UILineRenderer.JoinType.Bevel;
						}
					}
					if (joinType == UILineRenderer.JoinType.Bevel)
					{
						if (num7 < vector5.magnitude / 2f && num7 < vector6.magnitude / 2f && num5 > 0.5235988f)
						{
							if (num6 < 0f)
							{
								list2[k][2].position = vector7;
								list2[k + 1][1].position = vector7;
							}
							else
							{
								list2[k][3].position = vector8;
								list2[k + 1][0].position = vector8;
							}
						}
						UIVertex[] array2 = new UIVertex[]
						{
							list2[k][2],
							list2[k][3],
							list2[k + 1][0],
							list2[k + 1][1]
						};
						vh.AddUIVertexQuad(array2);
					}
				}
				vh.AddUIVertexQuad(list2[k]);
			}
		}

		private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, UILineRenderer.SegmentType type)
		{
			if (type == UILineRenderer.SegmentType.Start)
			{
				Vector2 vector = start - (end - start).normalized * this.LineThickness / 2f;
				return this.CreateLineSegment(vector, start, UILineRenderer.SegmentType.Start);
			}
			if (type == UILineRenderer.SegmentType.End)
			{
				Vector2 vector2 = end + (end - start).normalized * this.LineThickness / 2f;
				return this.CreateLineSegment(end, vector2, UILineRenderer.SegmentType.End);
			}
			Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
			return null;
		}

		private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, UILineRenderer.SegmentType type)
		{
			Vector2[] array = UILineRenderer.middleUvs;
			if (type == UILineRenderer.SegmentType.Start)
			{
				array = UILineRenderer.startUvs;
			}
			else if (type == UILineRenderer.SegmentType.End)
			{
				array = UILineRenderer.endUvs;
			}
			Vector2 vector = new Vector2(start.y - end.y, end.x - start.x);
			Vector2 vector2 = vector.normalized * this.LineThickness / 2f;
			Vector2 vector3 = start - vector2;
			Vector2 vector4 = start + vector2;
			Vector2 vector5 = end + vector2;
			Vector2 vector6 = end - vector2;
			return base.SetVbo(new Vector2[] { vector3, vector4, vector5, vector6 }, array);
		}

		private const float MIN_MITER_JOIN = 0.2617994f;

		private const float MIN_BEVEL_NICE_JOIN = 0.5235988f;

		private static readonly Vector2 UV_TOP_LEFT = Vector2.zero;

		private static readonly Vector2 UV_BOTTOM_LEFT = new Vector2(0f, 1f);

		private static readonly Vector2 UV_TOP_CENTER = new Vector2(0.5f, 0f);

		private static readonly Vector2 UV_BOTTOM_CENTER = new Vector2(0.5f, 1f);

		private static readonly Vector2 UV_TOP_RIGHT = new Vector2(1f, 0f);

		private static readonly Vector2 UV_BOTTOM_RIGHT = new Vector2(1f, 1f);

		private static readonly Vector2[] startUvs = new Vector2[]
		{
			UILineRenderer.UV_TOP_LEFT,
			UILineRenderer.UV_BOTTOM_LEFT,
			UILineRenderer.UV_BOTTOM_CENTER,
			UILineRenderer.UV_TOP_CENTER
		};

		private static readonly Vector2[] middleUvs = new Vector2[]
		{
			UILineRenderer.UV_TOP_CENTER,
			UILineRenderer.UV_BOTTOM_CENTER,
			UILineRenderer.UV_BOTTOM_CENTER,
			UILineRenderer.UV_TOP_CENTER
		};

		private static readonly Vector2[] endUvs = new Vector2[]
		{
			UILineRenderer.UV_TOP_CENTER,
			UILineRenderer.UV_BOTTOM_CENTER,
			UILineRenderer.UV_BOTTOM_RIGHT,
			UILineRenderer.UV_TOP_RIGHT
		};

		[SerializeField]
		private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

		[SerializeField]
		private Vector2[] m_points;

		public float LineThickness = 2f;

		public bool UseMargins;

		public Vector2 Margin;

		public bool relativeSize;

		public bool LineList;

		public bool LineCaps;

		public UILineRenderer.JoinType LineJoins;

		public UILineRenderer.BezierType BezierMode;

		public int BezierSegmentsPerCurve = 10;

		private enum SegmentType
		{
			Start,
			Middle,
			End
		}

		public enum JoinType
		{
			Bevel,
			Miter
		}

		public enum BezierType
		{
			None,
			Quick,
			Basic,
			Improved
		}
	}
}
