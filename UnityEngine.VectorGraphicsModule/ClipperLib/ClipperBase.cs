using System;
using System.Collections.Generic;

namespace ClipperLib
{
	internal class ClipperBase
	{
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
			for (;;)
			{
				bool flag = outPt.Pt == pt;
				if (flag)
				{
					break;
				}
				outPt = outPt.Next;
				if (outPt == pp)
				{
					goto Block_2;
				}
			}
			return true;
			Block_2:
			return false;
		}

		internal bool PointOnLineSegment(IntPoint pt, IntPoint linePt1, IntPoint linePt2, bool UseFullRange)
		{
			bool flag;
			if (UseFullRange)
			{
				flag = (pt.X == linePt1.X && pt.Y == linePt1.Y) || (pt.X == linePt2.X && pt.Y == linePt2.Y) || (pt.X > linePt1.X == pt.X < linePt2.X && pt.Y > linePt1.Y == pt.Y < linePt2.Y && Int128.Int128Mul(pt.X - linePt1.X, linePt2.Y - linePt1.Y) == Int128.Int128Mul(linePt2.X - linePt1.X, pt.Y - linePt1.Y));
			}
			else
			{
				flag = (pt.X == linePt1.X && pt.Y == linePt1.Y) || (pt.X == linePt2.X && pt.Y == linePt2.Y) || (pt.X > linePt1.X == pt.X < linePt2.X && pt.Y > linePt1.Y == pt.Y < linePt2.Y && (pt.X - linePt1.X) * (linePt2.Y - linePt1.Y) == (linePt2.X - linePt1.X) * (pt.Y - linePt1.Y));
			}
			return flag;
		}

		internal bool PointOnPolygon(IntPoint pt, OutPt pp, bool UseFullRange)
		{
			OutPt outPt = pp;
			for (;;)
			{
				bool flag = this.PointOnLineSegment(pt, outPt.Pt, outPt.Next.Pt, UseFullRange);
				if (flag)
				{
					break;
				}
				outPt = outPt.Next;
				bool flag2 = outPt == pp;
				if (flag2)
				{
					goto Block_2;
				}
			}
			return true;
			Block_2:
			return false;
		}

		internal static bool SlopesEqual(TEdge e1, TEdge e2, bool UseFullRange)
		{
			bool flag;
			if (UseFullRange)
			{
				flag = Int128.Int128Mul(e1.Delta.Y, e2.Delta.X) == Int128.Int128Mul(e1.Delta.X, e2.Delta.Y);
			}
			else
			{
				flag = e1.Delta.Y * e2.Delta.X == e1.Delta.X * e2.Delta.Y;
			}
			return flag;
		}

		internal static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, bool UseFullRange)
		{
			bool flag;
			if (UseFullRange)
			{
				flag = Int128.Int128Mul(pt1.Y - pt2.Y, pt2.X - pt3.X) == Int128.Int128Mul(pt1.X - pt2.X, pt2.Y - pt3.Y);
			}
			else
			{
				flag = (pt1.Y - pt2.Y) * (pt2.X - pt3.X) - (pt1.X - pt2.X) * (pt2.Y - pt3.Y) == 0L;
			}
			return flag;
		}

		internal static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, IntPoint pt4, bool UseFullRange)
		{
			bool flag;
			if (UseFullRange)
			{
				flag = Int128.Int128Mul(pt1.Y - pt2.Y, pt3.X - pt4.X) == Int128.Int128Mul(pt1.X - pt2.X, pt3.Y - pt4.Y);
			}
			else
			{
				flag = (pt1.Y - pt2.Y) * (pt3.X - pt4.X) - (pt1.X - pt2.X) * (pt3.Y - pt4.Y) == 0L;
			}
			return flag;
		}

		internal ClipperBase()
		{
			this.m_MinimaList = null;
			this.m_CurrentLM = null;
			this.m_UseFullRange = false;
			this.m_HasOpenPaths = false;
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
			bool flag = useFullRange;
			if (flag)
			{
				bool flag2 = Pt.X > 4611686018427387903L || Pt.Y > 4611686018427387903L || -Pt.X > 4611686018427387903L || -Pt.Y > 4611686018427387903L;
				if (flag2)
				{
					throw new ClipperException("Coordinate outside allowed range");
				}
			}
			else
			{
				bool flag3 = Pt.X > 1073741823L || Pt.Y > 1073741823L || -Pt.X > 1073741823L || -Pt.Y > 1073741823L;
				if (flag3)
				{
					useFullRange = true;
					this.RangeTest(Pt, ref useFullRange);
				}
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
			bool flag = e.Curr.Y >= e.Next.Curr.Y;
			if (flag)
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
				bool flag = E.Dx != -3.4E+38 && E.Prev.Dx != -3.4E+38;
				if (flag)
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
				bool flag2 = E.Top.Y == E.Prev.Bot.Y;
				if (!flag2)
				{
					goto IL_00E3;
				}
			}
			return E;
			IL_00E3:
			bool flag3 = tedge.Prev.Bot.X < E.Bot.X;
			if (flag3)
			{
				E = tedge;
			}
			return E;
		}

		private TEdge ProcessBound(TEdge E, bool LeftBoundIsForward)
		{
			TEdge tedge = E;
			bool flag = tedge.OutIdx == -2;
			TEdge tedge2;
			if (flag)
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
				bool flag2 = E == tedge;
				if (flag2)
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
				tedge2 = tedge;
			}
			else
			{
				bool flag3 = E.Dx == -3.4E+38;
				TEdge tedge3;
				if (flag3)
				{
					if (LeftBoundIsForward)
					{
						tedge3 = E.Prev;
					}
					else
					{
						tedge3 = E.Next;
					}
					bool flag4 = tedge3.Dx == -3.4E+38;
					if (flag4)
					{
						bool flag5 = tedge3.Bot.X != E.Bot.X && tedge3.Top.X != E.Bot.X;
						if (flag5)
						{
							this.ReverseHorizontal(E);
						}
					}
					else
					{
						bool flag6 = tedge3.Bot.X != E.Bot.X;
						if (flag6)
						{
							this.ReverseHorizontal(E);
						}
					}
				}
				tedge3 = E;
				if (LeftBoundIsForward)
				{
					while (tedge.Top.Y == tedge.Next.Bot.Y && tedge.Next.OutIdx != -2)
					{
						tedge = tedge.Next;
					}
					bool flag7 = tedge.Dx == -3.4E+38 && tedge.Next.OutIdx != -2;
					if (flag7)
					{
						TEdge tedge4 = tedge;
						while (tedge4.Prev.Dx == -3.4E+38)
						{
							tedge4 = tedge4.Prev;
						}
						bool flag8 = tedge4.Prev.Top.X > tedge.Next.Top.X;
						if (flag8)
						{
							tedge = tedge4.Prev;
						}
					}
					while (E != tedge)
					{
						E.NextInLML = E.Next;
						bool flag9 = E.Dx == -3.4E+38 && E != tedge3 && E.Bot.X != E.Prev.Top.X;
						if (flag9)
						{
							this.ReverseHorizontal(E);
						}
						E = E.Next;
					}
					bool flag10 = E.Dx == -3.4E+38 && E != tedge3 && E.Bot.X != E.Prev.Top.X;
					if (flag10)
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
					bool flag11 = tedge.Dx == -3.4E+38 && tedge.Prev.OutIdx != -2;
					if (flag11)
					{
						TEdge tedge4 = tedge;
						while (tedge4.Next.Dx == -3.4E+38)
						{
							tedge4 = tedge4.Next;
						}
						bool flag12 = tedge4.Next.Top.X == tedge.Prev.Top.X || tedge4.Next.Top.X > tedge.Prev.Top.X;
						if (flag12)
						{
							tedge = tedge4.Next;
						}
					}
					while (E != tedge)
					{
						E.NextInLML = E.Prev;
						bool flag13 = E.Dx == -3.4E+38 && E != tedge3 && E.Bot.X != E.Next.Top.X;
						if (flag13)
						{
							this.ReverseHorizontal(E);
						}
						E = E.Prev;
					}
					bool flag14 = E.Dx == -3.4E+38 && E != tedge3 && E.Bot.X != E.Next.Top.X;
					if (flag14)
					{
						this.ReverseHorizontal(E);
					}
					tedge = tedge.Prev;
				}
				tedge2 = tedge;
			}
			return tedge2;
		}

		public bool AddPath(List<IntPoint> pg, PolyType polyType, bool Closed)
		{
			bool flag = !Closed && polyType == PolyType.ptClip;
			if (flag)
			{
				throw new ClipperException("AddPath: Open paths must be subject.");
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
			bool flag2 = (Closed && num < 2) || (!Closed && num < 1);
			bool flag3;
			if (flag2)
			{
				flag3 = false;
			}
			else
			{
				List<TEdge> list = new List<TEdge>(num + 1);
				for (int i = 0; i <= num; i++)
				{
					list.Add(new TEdge());
				}
				bool flag4 = true;
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
					bool flag5 = tedge2.Curr == tedge2.Next.Curr && (Closed || tedge2.Next != tedge);
					if (flag5)
					{
						bool flag6 = tedge2 == tedge2.Next;
						if (flag6)
						{
							break;
						}
						bool flag7 = tedge2 == tedge;
						if (flag7)
						{
							tedge = tedge2.Next;
						}
						tedge2 = this.RemoveEdge(tedge2);
						tedge3 = tedge2;
					}
					else
					{
						bool flag8 = tedge2.Prev == tedge2.Next;
						if (flag8)
						{
							break;
						}
						bool flag9 = Closed && ClipperBase.SlopesEqual(tedge2.Prev.Curr, tedge2.Curr, tedge2.Next.Curr, this.m_UseFullRange) && (!this.PreserveCollinear || !this.Pt2IsBetweenPt1AndPt3(tedge2.Prev.Curr, tedge2.Curr, tedge2.Next.Curr));
						if (flag9)
						{
							bool flag10 = tedge2 == tedge;
							if (flag10)
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
							bool flag11 = tedge2 == tedge3 || (!Closed && tedge2.Next == tedge);
							if (flag11)
							{
								break;
							}
						}
					}
				}
				bool flag12 = (!Closed && tedge2 == tedge2.Next) || (Closed && tedge2.Prev == tedge2.Next);
				if (flag12)
				{
					flag3 = false;
				}
				else
				{
					bool flag13 = !Closed;
					if (flag13)
					{
						this.m_HasOpenPaths = true;
						tedge.Prev.OutIdx = -2;
					}
					tedge2 = tedge;
					do
					{
						this.InitEdge2(tedge2, polyType);
						tedge2 = tedge2.Next;
						bool flag14 = flag4 && tedge2.Curr.Y != tedge.Curr.Y;
						if (flag14)
						{
							flag4 = false;
						}
					}
					while (tedge2 != tedge);
					bool flag15 = flag4;
					if (flag15)
					{
						if (Closed)
						{
							flag3 = false;
						}
						else
						{
							tedge2.Prev.OutIdx = -2;
							LocalMinima localMinima = new LocalMinima();
							localMinima.Next = null;
							localMinima.Y = tedge2.Bot.Y;
							localMinima.LeftBound = null;
							localMinima.RightBound = tedge2;
							localMinima.RightBound.Side = EdgeSide.esRight;
							localMinima.RightBound.WindDelta = 0;
							for (;;)
							{
								bool flag16 = tedge2.Bot.X != tedge2.Prev.Top.X;
								if (flag16)
								{
									this.ReverseHorizontal(tedge2);
								}
								bool flag17 = tedge2.Next.OutIdx == -2;
								if (flag17)
								{
									break;
								}
								tedge2.NextInLML = tedge2.Next;
								tedge2 = tedge2.Next;
							}
							this.InsertLocalMinima(localMinima);
							this.m_edges.Add(list);
							flag3 = true;
						}
					}
					else
					{
						this.m_edges.Add(list);
						TEdge tedge4 = null;
						bool flag18 = tedge2.Prev.Bot == tedge2.Prev.Top;
						if (flag18)
						{
							tedge2 = tedge2.Next;
						}
						for (;;)
						{
							tedge2 = this.FindNextLocMin(tedge2);
							bool flag19 = tedge2 == tedge4;
							if (flag19)
							{
								break;
							}
							bool flag20 = tedge4 == null;
							if (flag20)
							{
								tedge4 = tedge2;
							}
							LocalMinima localMinima2 = new LocalMinima();
							localMinima2.Next = null;
							localMinima2.Y = tedge2.Bot.Y;
							bool flag21 = tedge2.Dx < tedge2.Prev.Dx;
							bool flag22;
							if (flag21)
							{
								localMinima2.LeftBound = tedge2.Prev;
								localMinima2.RightBound = tedge2;
								flag22 = false;
							}
							else
							{
								localMinima2.LeftBound = tedge2;
								localMinima2.RightBound = tedge2.Prev;
								flag22 = true;
							}
							localMinima2.LeftBound.Side = EdgeSide.esLeft;
							localMinima2.RightBound.Side = EdgeSide.esRight;
							bool flag23 = !Closed;
							if (flag23)
							{
								localMinima2.LeftBound.WindDelta = 0;
							}
							else
							{
								bool flag24 = localMinima2.LeftBound.Next == localMinima2.RightBound;
								if (flag24)
								{
									localMinima2.LeftBound.WindDelta = -1;
								}
								else
								{
									localMinima2.LeftBound.WindDelta = 1;
								}
							}
							localMinima2.RightBound.WindDelta = -localMinima2.LeftBound.WindDelta;
							tedge2 = this.ProcessBound(localMinima2.LeftBound, flag22);
							bool flag25 = tedge2.OutIdx == -2;
							if (flag25)
							{
								tedge2 = this.ProcessBound(tedge2, flag22);
							}
							TEdge tedge5 = this.ProcessBound(localMinima2.RightBound, !flag22);
							bool flag26 = tedge5.OutIdx == -2;
							if (flag26)
							{
								tedge5 = this.ProcessBound(tedge5, !flag22);
							}
							bool flag27 = localMinima2.LeftBound.OutIdx == -2;
							if (flag27)
							{
								localMinima2.LeftBound = null;
							}
							else
							{
								bool flag28 = localMinima2.RightBound.OutIdx == -2;
								if (flag28)
								{
									localMinima2.RightBound = null;
								}
							}
							this.InsertLocalMinima(localMinima2);
							bool flag29 = !flag22;
							if (flag29)
							{
								tedge2 = tedge5;
							}
						}
						flag3 = true;
					}
				}
			}
			return flag3;
		}

		public bool AddPaths(List<List<IntPoint>> ppg, PolyType polyType, bool closed)
		{
			bool flag = false;
			for (int i = 0; i < ppg.Count; i++)
			{
				bool flag2 = this.AddPath(ppg[i], polyType, closed);
				if (flag2)
				{
					flag = true;
				}
			}
			return flag;
		}

		internal bool Pt2IsBetweenPt1AndPt3(IntPoint pt1, IntPoint pt2, IntPoint pt3)
		{
			bool flag = pt1 == pt3 || pt1 == pt2 || pt3 == pt2;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = pt1.X != pt3.X;
				if (flag3)
				{
					flag2 = pt2.X > pt1.X == pt2.X < pt3.X;
				}
				else
				{
					flag2 = pt2.Y > pt1.Y == pt2.Y < pt3.Y;
				}
			}
			return flag2;
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
			bool flag = e.Delta.Y == 0L;
			if (flag)
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
			bool flag = this.m_MinimaList == null;
			if (flag)
			{
				this.m_MinimaList = newLm;
			}
			else
			{
				bool flag2 = newLm.Y >= this.m_MinimaList.Y;
				if (flag2)
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
		}

		internal bool PopLocalMinima(long Y, out LocalMinima current)
		{
			current = this.m_CurrentLM;
			bool flag = this.m_CurrentLM != null && this.m_CurrentLM.Y == Y;
			bool flag2;
			if (flag)
			{
				this.m_CurrentLM = this.m_CurrentLM.Next;
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		private void ReverseHorizontal(TEdge e)
		{
			this.Swap(ref e.Top.X, ref e.Bot.X);
		}

		internal virtual void Reset()
		{
			this.m_CurrentLM = this.m_MinimaList;
			bool flag = this.m_CurrentLM == null;
			if (!flag)
			{
				this.m_Scanbeam = null;
				for (LocalMinima localMinima = this.m_MinimaList; localMinima != null; localMinima = localMinima.Next)
				{
					this.InsertScanbeam(localMinima.Y);
					TEdge tedge = localMinima.LeftBound;
					bool flag2 = tedge != null;
					if (flag2)
					{
						tedge.Curr = tedge.Bot;
						tedge.OutIdx = -1;
					}
					tedge = localMinima.RightBound;
					bool flag3 = tedge != null;
					if (flag3)
					{
						tedge.Curr = tedge.Bot;
						tedge.OutIdx = -1;
					}
				}
				this.m_ActiveEdges = null;
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
			bool flag = i == count;
			IntRect intRect;
			if (flag)
			{
				intRect = new IntRect(0L, 0L, 0L, 0L);
			}
			else
			{
				IntRect intRect2 = default(IntRect);
				intRect2.left = paths[i][0].X;
				intRect2.right = intRect2.left;
				intRect2.top = paths[i][0].Y;
				intRect2.bottom = intRect2.top;
				while (i < count)
				{
					for (int j = 0; j < paths[i].Count; j++)
					{
						bool flag2 = paths[i][j].X < intRect2.left;
						if (flag2)
						{
							intRect2.left = paths[i][j].X;
						}
						else
						{
							bool flag3 = paths[i][j].X > intRect2.right;
							if (flag3)
							{
								intRect2.right = paths[i][j].X;
							}
						}
						bool flag4 = paths[i][j].Y < intRect2.top;
						if (flag4)
						{
							intRect2.top = paths[i][j].Y;
						}
						else
						{
							bool flag5 = paths[i][j].Y > intRect2.bottom;
							if (flag5)
							{
								intRect2.bottom = paths[i][j].Y;
							}
						}
					}
					i++;
				}
				intRect = intRect2;
			}
			return intRect;
		}

		internal void InsertScanbeam(long Y)
		{
			bool flag = this.m_Scanbeam == null;
			if (flag)
			{
				this.m_Scanbeam = new Scanbeam();
				this.m_Scanbeam.Next = null;
				this.m_Scanbeam.Y = Y;
			}
			else
			{
				bool flag2 = Y > this.m_Scanbeam.Y;
				if (flag2)
				{
					this.m_Scanbeam = new Scanbeam
					{
						Y = Y,
						Next = this.m_Scanbeam
					};
				}
				else
				{
					Scanbeam scanbeam = this.m_Scanbeam;
					while (scanbeam.Next != null && Y <= scanbeam.Next.Y)
					{
						scanbeam = scanbeam.Next;
					}
					bool flag3 = Y == scanbeam.Y;
					if (!flag3)
					{
						scanbeam.Next = new Scanbeam
						{
							Y = Y,
							Next = scanbeam.Next
						};
					}
				}
			}
		}

		internal bool PopScanbeam(out long Y)
		{
			bool flag = this.m_Scanbeam == null;
			bool flag2;
			if (flag)
			{
				Y = 0L;
				flag2 = false;
			}
			else
			{
				Y = this.m_Scanbeam.Y;
				this.m_Scanbeam = this.m_Scanbeam.Next;
				flag2 = true;
			}
			return flag2;
		}

		internal bool LocalMinimaPending()
		{
			return this.m_CurrentLM != null;
		}

		internal OutRec CreateOutRec()
		{
			OutRec outRec = new OutRec();
			outRec.Idx = -1;
			outRec.IsHole = false;
			outRec.IsOpen = false;
			outRec.FirstLeft = null;
			outRec.Pts = null;
			outRec.BottomPt = null;
			outRec.PolyNode = null;
			this.m_PolyOuts.Add(outRec);
			outRec.Idx = this.m_PolyOuts.Count - 1;
			return outRec;
		}

		internal void DisposeOutRec(int index)
		{
			OutRec outRec = this.m_PolyOuts[index];
			outRec.Pts = null;
			this.m_PolyOuts[index] = null;
		}

		internal void UpdateEdgeIntoAEL(ref TEdge e)
		{
			bool flag = e.NextInLML == null;
			if (flag)
			{
				throw new ClipperException("UpdateEdgeIntoAEL: invalid call");
			}
			TEdge prevInAEL = e.PrevInAEL;
			TEdge nextInAEL = e.NextInAEL;
			e.NextInLML.OutIdx = e.OutIdx;
			bool flag2 = prevInAEL != null;
			if (flag2)
			{
				prevInAEL.NextInAEL = e.NextInLML;
			}
			else
			{
				this.m_ActiveEdges = e.NextInLML;
			}
			bool flag3 = nextInAEL != null;
			if (flag3)
			{
				nextInAEL.PrevInAEL = e.NextInLML;
			}
			e.NextInLML.Side = e.Side;
			e.NextInLML.WindDelta = e.WindDelta;
			e.NextInLML.WindCnt = e.WindCnt;
			e.NextInLML.WindCnt2 = e.WindCnt2;
			e = e.NextInLML;
			e.Curr = e.Bot;
			e.PrevInAEL = prevInAEL;
			e.NextInAEL = nextInAEL;
			bool flag4 = !ClipperBase.IsHorizontal(e);
			if (flag4)
			{
				this.InsertScanbeam(e.Top.Y);
			}
		}

		internal void SwapPositionsInAEL(TEdge edge1, TEdge edge2)
		{
			bool flag = edge1.NextInAEL == edge1.PrevInAEL || edge2.NextInAEL == edge2.PrevInAEL;
			if (!flag)
			{
				bool flag2 = edge1.NextInAEL == edge2;
				if (flag2)
				{
					TEdge nextInAEL = edge2.NextInAEL;
					bool flag3 = nextInAEL != null;
					if (flag3)
					{
						nextInAEL.PrevInAEL = edge1;
					}
					TEdge prevInAEL = edge1.PrevInAEL;
					bool flag4 = prevInAEL != null;
					if (flag4)
					{
						prevInAEL.NextInAEL = edge2;
					}
					edge2.PrevInAEL = prevInAEL;
					edge2.NextInAEL = edge1;
					edge1.PrevInAEL = edge2;
					edge1.NextInAEL = nextInAEL;
				}
				else
				{
					bool flag5 = edge2.NextInAEL == edge1;
					if (flag5)
					{
						TEdge nextInAEL2 = edge1.NextInAEL;
						bool flag6 = nextInAEL2 != null;
						if (flag6)
						{
							nextInAEL2.PrevInAEL = edge2;
						}
						TEdge prevInAEL2 = edge2.PrevInAEL;
						bool flag7 = prevInAEL2 != null;
						if (flag7)
						{
							prevInAEL2.NextInAEL = edge1;
						}
						edge1.PrevInAEL = prevInAEL2;
						edge1.NextInAEL = edge2;
						edge2.PrevInAEL = edge1;
						edge2.NextInAEL = nextInAEL2;
					}
					else
					{
						TEdge nextInAEL3 = edge1.NextInAEL;
						TEdge prevInAEL3 = edge1.PrevInAEL;
						edge1.NextInAEL = edge2.NextInAEL;
						bool flag8 = edge1.NextInAEL != null;
						if (flag8)
						{
							edge1.NextInAEL.PrevInAEL = edge1;
						}
						edge1.PrevInAEL = edge2.PrevInAEL;
						bool flag9 = edge1.PrevInAEL != null;
						if (flag9)
						{
							edge1.PrevInAEL.NextInAEL = edge1;
						}
						edge2.NextInAEL = nextInAEL3;
						bool flag10 = edge2.NextInAEL != null;
						if (flag10)
						{
							edge2.NextInAEL.PrevInAEL = edge2;
						}
						edge2.PrevInAEL = prevInAEL3;
						bool flag11 = edge2.PrevInAEL != null;
						if (flag11)
						{
							edge2.PrevInAEL.NextInAEL = edge2;
						}
					}
				}
				bool flag12 = edge1.PrevInAEL == null;
				if (flag12)
				{
					this.m_ActiveEdges = edge1;
				}
				else
				{
					bool flag13 = edge2.PrevInAEL == null;
					if (flag13)
					{
						this.m_ActiveEdges = edge2;
					}
				}
			}
		}

		internal void DeleteFromAEL(TEdge e)
		{
			TEdge prevInAEL = e.PrevInAEL;
			TEdge nextInAEL = e.NextInAEL;
			bool flag = prevInAEL == null && nextInAEL == null && e != this.m_ActiveEdges;
			if (!flag)
			{
				bool flag2 = prevInAEL != null;
				if (flag2)
				{
					prevInAEL.NextInAEL = nextInAEL;
				}
				else
				{
					this.m_ActiveEdges = nextInAEL;
				}
				bool flag3 = nextInAEL != null;
				if (flag3)
				{
					nextInAEL.PrevInAEL = prevInAEL;
				}
				e.NextInAEL = null;
				e.PrevInAEL = null;
			}
		}

		internal const double horizontal = -3.4E+38;

		internal const int Skip = -2;

		internal const int Unassigned = -1;

		internal const double tolerance = 1E-20;

		public const long loRange = 1073741823L;

		public const long hiRange = 4611686018427387903L;

		internal LocalMinima m_MinimaList;

		internal LocalMinima m_CurrentLM;

		internal List<List<TEdge>> m_edges = new List<List<TEdge>>();

		internal Scanbeam m_Scanbeam;

		internal List<OutRec> m_PolyOuts;

		internal TEdge m_ActiveEdges;

		internal bool m_UseFullRange;

		internal bool m_HasOpenPaths;
	}
}
