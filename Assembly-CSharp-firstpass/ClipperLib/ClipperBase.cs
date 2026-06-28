using System;
using System.Collections.Generic;

namespace ClipperLib
{
	public class ClipperBase
	{
		internal ClipperBase()
		{
			this.m_MinimaList = null;
			this.m_CurrentLM = null;
			this.m_UseFullRange = false;
			this.m_HasOpenPaths = false;
		}

		internal static bool near_zero(double val)
		{
			return val > -1E-20 && val < 1E-20;
		}

		public bool PreserveCollinear { get; set; }

		public void Swap(ref long val1, ref long val2)
		{
			long num = val1;
			val1 = val2;
			val2 = num;
		}

		internal static bool IsHorizontal(TEdge e)
		{
			return e.Delta.Y == 0L;
		}

		internal bool PointIsVertex(IntPoint pt, OutPt pp)
		{
			OutPt outPt = pp;
			while (!(outPt.Pt == pt))
			{
				outPt = outPt.Next;
				if (outPt == pp)
				{
					return false;
				}
			}
			return true;
		}

		internal bool PointOnLineSegment(IntPoint pt, IntPoint linePt1, IntPoint linePt2, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return (pt.X == linePt1.X && pt.Y == linePt1.Y) || (pt.X == linePt2.X && pt.Y == linePt2.Y) || (pt.X > linePt1.X == pt.X < linePt2.X && pt.Y > linePt1.Y == pt.Y < linePt2.Y && Int128.Int128Mul(pt.X - linePt1.X, linePt2.Y - linePt1.Y) == Int128.Int128Mul(linePt2.X - linePt1.X, pt.Y - linePt1.Y));
			}
			return (pt.X == linePt1.X && pt.Y == linePt1.Y) || (pt.X == linePt2.X && pt.Y == linePt2.Y) || (pt.X > linePt1.X == pt.X < linePt2.X && pt.Y > linePt1.Y == pt.Y < linePt2.Y && (pt.X - linePt1.X) * (linePt2.Y - linePt1.Y) == (linePt2.X - linePt1.X) * (pt.Y - linePt1.Y));
		}

		internal bool PointOnPolygon(IntPoint pt, OutPt pp, bool UseFullRange)
		{
			OutPt outPt = pp;
			while (!this.PointOnLineSegment(pt, outPt.Pt, outPt.Next.Pt, UseFullRange))
			{
				outPt = outPt.Next;
				if (outPt == pp)
				{
					return false;
				}
			}
			return true;
		}

		internal static bool SlopesEqual(TEdge e1, TEdge e2, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return Int128.Int128Mul(e1.Delta.Y, e2.Delta.X) == Int128.Int128Mul(e1.Delta.X, e2.Delta.Y);
			}
			return e1.Delta.Y * e2.Delta.X == e1.Delta.X * e2.Delta.Y;
		}

		protected static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return Int128.Int128Mul(pt1.Y - pt2.Y, pt2.X - pt3.X) == Int128.Int128Mul(pt1.X - pt2.X, pt2.Y - pt3.Y);
			}
			return (pt1.Y - pt2.Y) * (pt2.X - pt3.X) - (pt1.X - pt2.X) * (pt2.Y - pt3.Y) == 0L;
		}

		protected static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, IntPoint pt4, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return Int128.Int128Mul(pt1.Y - pt2.Y, pt3.X - pt4.X) == Int128.Int128Mul(pt1.X - pt2.X, pt3.Y - pt4.Y);
			}
			return (pt1.Y - pt2.Y) * (pt3.X - pt4.X) - (pt1.X - pt2.X) * (pt3.Y - pt4.Y) == 0L;
		}

		public virtual void Clear()
		{
			this.DisposeLocalMinimaList();
			for (int i = 0; i < this.m_edges.Count; i++)
			{
				for (int j = 0; j < this.m_edges[i].Count; j++)
				{
					this.m_edges[i][j] = null;
				}
				this.m_edges[i].Clear();
			}
			this.m_edges.Clear();
			this.m_UseFullRange = false;
			this.m_HasOpenPaths = false;
		}

		private void DisposeLocalMinimaList()
		{
			while (this.m_MinimaList != null)
			{
				LocalMinima next = this.m_MinimaList.Next;
				this.m_MinimaList = null;
				this.m_MinimaList = next;
			}
			this.m_CurrentLM = null;
		}

		private void RangeTest(IntPoint Pt, ref bool useFullRange)
		{
			if (useFullRange)
			{
				if (Pt.X > 4611686018427387903L || Pt.Y > 4611686018427387903L || -Pt.X > 4611686018427387903L || -Pt.Y > 4611686018427387903L)
				{
					throw new ClipperException("Coordinate outside allowed range");
				}
			}
			else if (Pt.X > 1073741823L || Pt.Y > 1073741823L || -Pt.X > 1073741823L || -Pt.Y > 1073741823L)
			{
				useFullRange = true;
				this.RangeTest(Pt, ref useFullRange);
			}
		}

		private void InitEdge(TEdge e, TEdge eNext, TEdge ePrev, IntPoint pt)
		{
			e.Next = eNext;
			e.Prev = ePrev;
			e.Curr = pt;
			e.OutIdx = -1;
		}

		private void InitEdge2(TEdge e, PolyType polyType)
		{
			if (e.Curr.Y >= e.Next.Curr.Y)
			{
				e.Bot = e.Curr;
				e.Top = e.Next.Curr;
			}
			else
			{
				e.Top = e.Curr;
				e.Bot = e.Next.Curr;
			}
			this.SetDx(e);
			e.PolyTyp = polyType;
		}

		private TEdge FindNextLocMin(TEdge E)
		{
			TEdge tedge;
			for (;;)
			{
				while (E.Bot != E.Prev.Bot || E.Curr == E.Top)
				{
					E = E.Next;
				}
				if (E.Dx != -3.4E+38 && E.Prev.Dx != -3.4E+38)
				{
					break;
				}
				while (E.Prev.Dx == -3.4E+38)
				{
					E = E.Prev;
				}
				tedge = E;
				while (E.Dx == -3.4E+38)
				{
					E = E.Next;
				}
				if (E.Top.Y != E.Prev.Bot.Y)
				{
					goto IL_00E3;
				}
			}
			return E;
			IL_00E3:
			if (tedge.Prev.Bot.X < E.Bot.X)
			{
				E = tedge;
			}
			return E;
		}

		private TEdge ProcessBound(TEdge E, bool LeftBoundIsForward)
		{
			TEdge tedge = E;
			if (tedge.OutIdx == -2)
			{
				E = tedge;
				if (LeftBoundIsForward)
				{
					while (E.Top.Y == E.Next.Bot.Y)
					{
						E = E.Next;
					}
					while (E != tedge && E.Dx == -3.4E+38)
					{
						E = E.Prev;
					}
				}
				else
				{
					while (E.Top.Y == E.Prev.Bot.Y)
					{
						E = E.Prev;
					}
					while (E != tedge && E.Dx == -3.4E+38)
					{
						E = E.Next;
					}
				}
				if (E == tedge)
				{
					if (LeftBoundIsForward)
					{
						tedge = E.Next;
					}
					else
					{
						tedge = E.Prev;
					}
				}
				else
				{
					if (LeftBoundIsForward)
					{
						E = tedge.Next;
					}
					else
					{
						E = tedge.Prev;
					}
					LocalMinima localMinima = new LocalMinima();
					localMinima.Next = null;
					localMinima.Y = E.Bot.Y;
					localMinima.LeftBound = null;
					localMinima.RightBound = E;
					E.WindDelta = 0;
					tedge = this.ProcessBound(E, LeftBoundIsForward);
					this.InsertLocalMinima(localMinima);
				}
				return tedge;
			}
			TEdge tedge2;
			if (E.Dx == -3.4E+38)
			{
				if (LeftBoundIsForward)
				{
					tedge2 = E.Prev;
				}
				else
				{
					tedge2 = E.Next;
				}
				if (tedge2.OutIdx != -2)
				{
					if (tedge2.Dx == -3.4E+38)
					{
						if (tedge2.Bot.X != E.Bot.X && tedge2.Top.X != E.Bot.X)
						{
							this.ReverseHorizontal(E);
						}
					}
					else if (tedge2.Bot.X != E.Bot.X)
					{
						this.ReverseHorizontal(E);
					}
				}
			}
			tedge2 = E;
			if (LeftBoundIsForward)
			{
				while (tedge.Top.Y == tedge.Next.Bot.Y && tedge.Next.OutIdx != -2)
				{
					tedge = tedge.Next;
				}
				if (tedge.Dx == -3.4E+38 && tedge.Next.OutIdx != -2)
				{
					TEdge tedge3 = tedge;
					while (tedge3.Prev.Dx == -3.4E+38)
					{
						tedge3 = tedge3.Prev;
					}
					if (tedge3.Prev.Top.X == tedge.Next.Top.X)
					{
						if (!LeftBoundIsForward)
						{
							tedge = tedge3.Prev;
						}
					}
					else if (tedge3.Prev.Top.X > tedge.Next.Top.X)
					{
						tedge = tedge3.Prev;
					}
				}
				while (E != tedge)
				{
					E.NextInLML = E.Next;
					if (E.Dx == -3.4E+38 && E != tedge2 && E.Bot.X != E.Prev.Top.X)
					{
						this.ReverseHorizontal(E);
					}
					E = E.Next;
				}
				if (E.Dx == -3.4E+38 && E != tedge2 && E.Bot.X != E.Prev.Top.X)
				{
					this.ReverseHorizontal(E);
				}
				tedge = tedge.Next;
			}
			else
			{
				while (tedge.Top.Y == tedge.Prev.Bot.Y && tedge.Prev.OutIdx != -2)
				{
					tedge = tedge.Prev;
				}
				if (tedge.Dx == -3.4E+38 && tedge.Prev.OutIdx != -2)
				{
					TEdge tedge3 = tedge;
					while (tedge3.Next.Dx == -3.4E+38)
					{
						tedge3 = tedge3.Next;
					}
					if (tedge3.Next.Top.X == tedge.Prev.Top.X)
					{
						if (!LeftBoundIsForward)
						{
							tedge = tedge3.Next;
						}
					}
					else if (tedge3.Next.Top.X > tedge.Prev.Top.X)
					{
						tedge = tedge3.Next;
					}
				}
				while (E != tedge)
				{
					E.NextInLML = E.Prev;
					if (E.Dx == -3.4E+38 && E != tedge2 && E.Bot.X != E.Next.Top.X)
					{
						this.ReverseHorizontal(E);
					}
					E = E.Prev;
				}
				if (E.Dx == -3.4E+38 && E != tedge2 && E.Bot.X != E.Next.Top.X)
				{
					this.ReverseHorizontal(E);
				}
				tedge = tedge.Prev;
			}
			return tedge;
		}

		public bool AddPath(List<IntPoint> pg, PolyType polyType, bool Closed)
		{
			if (!Closed)
			{
				throw new ClipperException("AddPath: Open paths have been disabled.");
			}
			int num = pg.Count - 1;
			if (Closed)
			{
				while (num > 0 && pg[num] == pg[0])
				{
					num--;
				}
			}
			while (num > 0 && pg[num] == pg[num - 1])
			{
				num--;
			}
			if ((Closed && num < 2) || (!Closed && num < 1))
			{
				return false;
			}
			List<TEdge> list = new List<TEdge>(num + 1);
			for (int i = 0; i <= num; i++)
			{
				list.Add(new TEdge());
			}
			bool flag = true;
			list[1].Curr = pg[1];
			this.RangeTest(pg[0], ref this.m_UseFullRange);
			this.RangeTest(pg[num], ref this.m_UseFullRange);
			this.InitEdge(list[0], list[1], list[num], pg[0]);
			this.InitEdge(list[num], list[0], list[num - 1], pg[num]);
			for (int j = num - 1; j >= 1; j--)
			{
				this.RangeTest(pg[j], ref this.m_UseFullRange);
				this.InitEdge(list[j], list[j + 1], list[j - 1], pg[j]);
			}
			TEdge tedge = list[0];
			TEdge tedge2 = tedge;
			TEdge tedge3 = tedge;
			for (;;)
			{
				if (tedge2.Curr == tedge2.Next.Curr && (Closed || tedge2.Next != tedge))
				{
					if (tedge2 == tedge2.Next)
					{
						break;
					}
					if (tedge2 == tedge)
					{
						tedge = tedge2.Next;
					}
					tedge2 = this.RemoveEdge(tedge2);
					tedge3 = tedge2;
				}
				else
				{
					if (tedge2.Prev == tedge2.Next)
					{
						break;
					}
					if (Closed && ClipperBase.SlopesEqual(tedge2.Prev.Curr, tedge2.Curr, tedge2.Next.Curr, this.m_UseFullRange) && (!this.PreserveCollinear || !this.Pt2IsBetweenPt1AndPt3(tedge2.Prev.Curr, tedge2.Curr, tedge2.Next.Curr)))
					{
						if (tedge2 == tedge)
						{
							tedge = tedge2.Next;
						}
						tedge2 = this.RemoveEdge(tedge2);
						tedge2 = tedge2.Prev;
						tedge3 = tedge2;
					}
					else
					{
						tedge2 = tedge2.Next;
						if (tedge2 == tedge3 || (!Closed && tedge2.Next == tedge))
						{
							break;
						}
					}
				}
			}
			if ((!Closed && tedge2 == tedge2.Next) || (Closed && tedge2.Prev == tedge2.Next))
			{
				return false;
			}
			if (!Closed)
			{
				this.m_HasOpenPaths = true;
				tedge.Prev.OutIdx = -2;
			}
			tedge2 = tedge;
			do
			{
				this.InitEdge2(tedge2, polyType);
				tedge2 = tedge2.Next;
				if (flag && tedge2.Curr.Y != tedge.Curr.Y)
				{
					flag = false;
				}
			}
			while (tedge2 != tedge);
			if (!flag)
			{
				this.m_edges.Add(list);
				TEdge tedge4 = null;
				if (tedge2.Prev.Bot == tedge2.Prev.Top)
				{
					tedge2 = tedge2.Next;
				}
				for (;;)
				{
					tedge2 = this.FindNextLocMin(tedge2);
					if (tedge2 == tedge4)
					{
						break;
					}
					if (tedge4 == null)
					{
						tedge4 = tedge2;
					}
					LocalMinima localMinima = new LocalMinima();
					localMinima.Next = null;
					localMinima.Y = tedge2.Bot.Y;
					bool flag2;
					if (tedge2.Dx < tedge2.Prev.Dx)
					{
						localMinima.LeftBound = tedge2.Prev;
						localMinima.RightBound = tedge2;
						flag2 = false;
					}
					else
					{
						localMinima.LeftBound = tedge2;
						localMinima.RightBound = tedge2.Prev;
						flag2 = true;
					}
					localMinima.LeftBound.Side = EdgeSide.esLeft;
					localMinima.RightBound.Side = EdgeSide.esRight;
					if (!Closed)
					{
						localMinima.LeftBound.WindDelta = 0;
					}
					else if (localMinima.LeftBound.Next == localMinima.RightBound)
					{
						localMinima.LeftBound.WindDelta = -1;
					}
					else
					{
						localMinima.LeftBound.WindDelta = 1;
					}
					localMinima.RightBound.WindDelta = -localMinima.LeftBound.WindDelta;
					tedge2 = this.ProcessBound(localMinima.LeftBound, flag2);
					if (tedge2.OutIdx == -2)
					{
						tedge2 = this.ProcessBound(tedge2, flag2);
					}
					TEdge tedge5 = this.ProcessBound(localMinima.RightBound, !flag2);
					if (tedge5.OutIdx == -2)
					{
						tedge5 = this.ProcessBound(tedge5, !flag2);
					}
					if (localMinima.LeftBound.OutIdx == -2)
					{
						localMinima.LeftBound = null;
					}
					else if (localMinima.RightBound.OutIdx == -2)
					{
						localMinima.RightBound = null;
					}
					this.InsertLocalMinima(localMinima);
					if (!flag2)
					{
						tedge2 = tedge5;
					}
				}
				return true;
			}
			if (Closed)
			{
				return false;
			}
			tedge2.Prev.OutIdx = -2;
			if (tedge2.Prev.Bot.X < tedge2.Prev.Top.X)
			{
				this.ReverseHorizontal(tedge2.Prev);
			}
			LocalMinima localMinima2 = new LocalMinima();
			localMinima2.Next = null;
			localMinima2.Y = tedge2.Bot.Y;
			localMinima2.LeftBound = null;
			localMinima2.RightBound = tedge2;
			localMinima2.RightBound.Side = EdgeSide.esRight;
			localMinima2.RightBound.WindDelta = 0;
			while (tedge2.Next.OutIdx != -2)
			{
				tedge2.NextInLML = tedge2.Next;
				if (tedge2.Bot.X != tedge2.Prev.Top.X)
				{
					this.ReverseHorizontal(tedge2);
				}
				tedge2 = tedge2.Next;
			}
			this.InsertLocalMinima(localMinima2);
			this.m_edges.Add(list);
			return true;
		}

		public bool AddPaths(List<List<IntPoint>> ppg, PolyType polyType, bool closed)
		{
			bool flag = false;
			for (int i = 0; i < ppg.Count; i++)
			{
				if (this.AddPath(ppg[i], polyType, closed))
				{
					flag = true;
				}
			}
			return flag;
		}

		internal bool Pt2IsBetweenPt1AndPt3(IntPoint pt1, IntPoint pt2, IntPoint pt3)
		{
			if (pt1 == pt3 || pt1 == pt2 || pt3 == pt2)
			{
				return false;
			}
			if (pt1.X != pt3.X)
			{
				return pt2.X > pt1.X == pt2.X < pt3.X;
			}
			return pt2.Y > pt1.Y == pt2.Y < pt3.Y;
		}

		private TEdge RemoveEdge(TEdge e)
		{
			e.Prev.Next = e.Next;
			e.Next.Prev = e.Prev;
			TEdge next = e.Next;
			e.Prev = null;
			return next;
		}

		private void SetDx(TEdge e)
		{
			e.Delta.X = e.Top.X - e.Bot.X;
			e.Delta.Y = e.Top.Y - e.Bot.Y;
			if (e.Delta.Y == 0L)
			{
				e.Dx = -3.4E+38;
			}
			else
			{
				e.Dx = (double)e.Delta.X / (double)e.Delta.Y;
			}
		}

		private void InsertLocalMinima(LocalMinima newLm)
		{
			if (this.m_MinimaList == null)
			{
				this.m_MinimaList = newLm;
			}
			else if (newLm.Y >= this.m_MinimaList.Y)
			{
				newLm.Next = this.m_MinimaList;
				this.m_MinimaList = newLm;
			}
			else
			{
				LocalMinima localMinima = this.m_MinimaList;
				while (localMinima.Next != null && newLm.Y < localMinima.Next.Y)
				{
					localMinima = localMinima.Next;
				}
				newLm.Next = localMinima.Next;
				localMinima.Next = newLm;
			}
		}

		protected void PopLocalMinima()
		{
			if (this.m_CurrentLM == null)
			{
				return;
			}
			this.m_CurrentLM = this.m_CurrentLM.Next;
		}

		private void ReverseHorizontal(TEdge e)
		{
			this.Swap(ref e.Top.X, ref e.Bot.X);
		}

		protected virtual void Reset()
		{
			this.m_CurrentLM = this.m_MinimaList;
			if (this.m_CurrentLM == null)
			{
				return;
			}
			for (LocalMinima localMinima = this.m_MinimaList; localMinima != null; localMinima = localMinima.Next)
			{
				TEdge tedge = localMinima.LeftBound;
				if (tedge != null)
				{
					tedge.Curr = tedge.Bot;
					tedge.Side = EdgeSide.esLeft;
					tedge.OutIdx = -1;
				}
				tedge = localMinima.RightBound;
				if (tedge != null)
				{
					tedge.Curr = tedge.Bot;
					tedge.Side = EdgeSide.esRight;
					tedge.OutIdx = -1;
				}
			}
		}

		public static IntRect GetBounds(List<List<IntPoint>> paths)
		{
			int i = 0;
			int count = paths.Count;
			while (i < count && paths[i].Count == 0)
			{
				i++;
			}
			if (i == count)
			{
				return new IntRect(0L, 0L, 0L, 0L);
			}
			IntRect intRect = default(IntRect);
			intRect.left = paths[i][0].X;
			intRect.right = intRect.left;
			intRect.top = paths[i][0].Y;
			intRect.bottom = intRect.top;
			while (i < count)
			{
				for (int j = 0; j < paths[i].Count; j++)
				{
					if (paths[i][j].X < intRect.left)
					{
						intRect.left = paths[i][j].X;
					}
					else if (paths[i][j].X > intRect.right)
					{
						intRect.right = paths[i][j].X;
					}
					if (paths[i][j].Y < intRect.top)
					{
						intRect.top = paths[i][j].Y;
					}
					else if (paths[i][j].Y > intRect.bottom)
					{
						intRect.bottom = paths[i][j].Y;
					}
				}
				i++;
			}
			return intRect;
		}

		protected const double horizontal = -3.4E+38;

		protected const int Skip = -2;

		protected const int Unassigned = -1;

		protected const double tolerance = 1E-20;

		public const long loRange = 1073741823L;

		public const long hiRange = 4611686018427387903L;

		internal LocalMinima m_MinimaList;

		internal LocalMinima m_CurrentLM;

		internal List<List<TEdge>> m_edges = new List<List<TEdge>>();

		internal bool m_UseFullRange;

		internal bool m_HasOpenPaths;
	}
}
