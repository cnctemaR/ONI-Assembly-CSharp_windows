using System;
using System.Collections.Generic;

namespace ClipperLib
{
	internal class Clipper : ClipperBase
	{
		public Clipper(int InitOptions = 0)
		{
			this.m_Scanbeam = null;
			this.m_Maxima = null;
			this.m_ActiveEdges = null;
			this.m_SortedEdges = null;
			this.m_IntersectList = new List<IntersectNode>();
			this.m_IntersectNodeComparer = new MyIntersectNodeSort();
			this.m_ExecuteLocked = false;
			this.m_UsingPolyTree = false;
			this.m_PolyOuts = new List<OutRec>();
			this.m_Joins = new List<Join>();
			this.m_GhostJoins = new List<Join>();
			this.ReverseSolution = (1 & InitOptions) != 0;
			this.StrictlySimple = (2 & InitOptions) != 0;
			base.PreserveCollinear = (4 & InitOptions) != 0;
		}

		private void InsertMaxima(long X)
		{
			Maxima maxima = new Maxima();
			maxima.X = X;
			bool flag = this.m_Maxima == null;
			if (flag)
			{
				this.m_Maxima = maxima;
				this.m_Maxima.Next = null;
				this.m_Maxima.Prev = null;
			}
			else
			{
				bool flag2 = X < this.m_Maxima.X;
				if (flag2)
				{
					maxima.Next = this.m_Maxima;
					maxima.Prev = null;
					this.m_Maxima = maxima;
				}
				else
				{
					Maxima maxima2 = this.m_Maxima;
					while (maxima2.Next != null && X >= maxima2.Next.X)
					{
						maxima2 = maxima2.Next;
					}
					bool flag3 = X == maxima2.X;
					if (!flag3)
					{
						maxima.Next = maxima2.Next;
						maxima.Prev = maxima2;
						bool flag4 = maxima2.Next != null;
						if (flag4)
						{
							maxima2.Next.Prev = maxima;
						}
						maxima2.Next = maxima;
					}
				}
			}
		}

		public bool ReverseSolution { get; set; }

		public bool StrictlySimple { get; set; }

		public bool Execute(ClipType clipType, List<List<IntPoint>> solution, PolyFillType FillType = PolyFillType.pftEvenOdd)
		{
			return this.Execute(clipType, solution, FillType, FillType);
		}

		public bool Execute(ClipType clipType, PolyTree polytree, PolyFillType FillType = PolyFillType.pftEvenOdd)
		{
			return this.Execute(clipType, polytree, FillType, FillType);
		}

		public bool Execute(ClipType clipType, List<List<IntPoint>> solution, PolyFillType subjFillType, PolyFillType clipFillType)
		{
			bool executeLocked = this.m_ExecuteLocked;
			bool flag;
			if (executeLocked)
			{
				flag = false;
			}
			else
			{
				bool hasOpenPaths = this.m_HasOpenPaths;
				if (hasOpenPaths)
				{
					throw new ClipperException("Error: PolyTree struct is needed for open path clipping.");
				}
				this.m_ExecuteLocked = true;
				solution.Clear();
				this.m_SubjFillType = subjFillType;
				this.m_ClipFillType = clipFillType;
				this.m_ClipType = clipType;
				this.m_UsingPolyTree = false;
				bool flag2;
				try
				{
					flag2 = this.ExecuteInternal();
					bool flag3 = flag2;
					if (flag3)
					{
						this.BuildResult(solution);
					}
				}
				finally
				{
					this.DisposeAllPolyPts();
					this.m_ExecuteLocked = false;
				}
				flag = flag2;
			}
			return flag;
		}

		public bool Execute(ClipType clipType, PolyTree polytree, PolyFillType subjFillType, PolyFillType clipFillType)
		{
			bool executeLocked = this.m_ExecuteLocked;
			bool flag;
			if (executeLocked)
			{
				flag = false;
			}
			else
			{
				this.m_ExecuteLocked = true;
				this.m_SubjFillType = subjFillType;
				this.m_ClipFillType = clipFillType;
				this.m_ClipType = clipType;
				this.m_UsingPolyTree = true;
				bool flag2;
				try
				{
					flag2 = this.ExecuteInternal();
					bool flag3 = flag2;
					if (flag3)
					{
						this.BuildResult2(polytree);
					}
				}
				finally
				{
					this.DisposeAllPolyPts();
					this.m_ExecuteLocked = false;
				}
				flag = flag2;
			}
			return flag;
		}

		internal void FixHoleLinkage(OutRec outRec)
		{
			bool flag = outRec.FirstLeft == null || (outRec.IsHole != outRec.FirstLeft.IsHole && outRec.FirstLeft.Pts != null);
			if (!flag)
			{
				OutRec outRec2 = outRec.FirstLeft;
				while (outRec2 != null && (outRec2.IsHole == outRec.IsHole || outRec2.Pts == null))
				{
					outRec2 = outRec2.FirstLeft;
				}
				outRec.FirstLeft = outRec2;
			}
		}

		private bool ExecuteInternal()
		{
			bool flag2;
			try
			{
				this.Reset();
				this.m_SortedEdges = null;
				this.m_Maxima = null;
				long num;
				bool flag = !base.PopScanbeam(out num);
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					this.InsertLocalMinimaIntoAEL(num);
					long num2;
					while (base.PopScanbeam(out num2) || base.LocalMinimaPending())
					{
						this.ProcessHorizontals();
						this.m_GhostJoins.Clear();
						bool flag3 = !this.ProcessIntersections(num2);
						if (flag3)
						{
							return false;
						}
						this.ProcessEdgesAtTopOfScanbeam(num2);
						num = num2;
						this.InsertLocalMinimaIntoAEL(num);
					}
					foreach (OutRec outRec in this.m_PolyOuts)
					{
						bool flag4 = outRec.Pts == null || outRec.IsOpen;
						if (!flag4)
						{
							bool flag5 = (outRec.IsHole ^ this.ReverseSolution) == this.Area(outRec) > 0.0;
							if (flag5)
							{
								this.ReversePolyPtLinks(outRec.Pts);
							}
						}
					}
					this.JoinCommonEdges();
					foreach (OutRec outRec2 in this.m_PolyOuts)
					{
						bool flag6 = outRec2.Pts == null;
						if (!flag6)
						{
							bool isOpen = outRec2.IsOpen;
							if (isOpen)
							{
								this.FixupOutPolyline(outRec2);
							}
							else
							{
								this.FixupOutPolygon(outRec2);
							}
						}
					}
					bool strictlySimple = this.StrictlySimple;
					if (strictlySimple)
					{
						this.DoSimplePolygons();
					}
					flag2 = true;
				}
			}
			finally
			{
				this.m_Joins.Clear();
				this.m_GhostJoins.Clear();
			}
			return flag2;
		}

		private void DisposeAllPolyPts()
		{
			for (int i = 0; i < this.m_PolyOuts.Count; i++)
			{
				base.DisposeOutRec(i);
			}
			this.m_PolyOuts.Clear();
		}

		private void AddJoin(OutPt Op1, OutPt Op2, IntPoint OffPt)
		{
			Join join = new Join();
			join.OutPt1 = Op1;
			join.OutPt2 = Op2;
			join.OffPt = OffPt;
			this.m_Joins.Add(join);
		}

		private void AddGhostJoin(OutPt Op, IntPoint OffPt)
		{
			Join join = new Join();
			join.OutPt1 = Op;
			join.OffPt = OffPt;
			this.m_GhostJoins.Add(join);
		}

		private void InsertLocalMinimaIntoAEL(long botY)
		{
			LocalMinima localMinima;
			while (base.PopLocalMinima(botY, out localMinima))
			{
				TEdge leftBound = localMinima.LeftBound;
				TEdge rightBound = localMinima.RightBound;
				OutPt outPt = null;
				bool flag = leftBound == null;
				if (flag)
				{
					this.InsertEdgeIntoAEL(rightBound, null);
					this.SetWindingCount(rightBound);
					bool flag2 = this.IsContributing(rightBound);
					if (flag2)
					{
						outPt = this.AddOutPt(rightBound, rightBound.Bot);
					}
				}
				else
				{
					bool flag3 = rightBound == null;
					if (flag3)
					{
						this.InsertEdgeIntoAEL(leftBound, null);
						this.SetWindingCount(leftBound);
						bool flag4 = this.IsContributing(leftBound);
						if (flag4)
						{
							outPt = this.AddOutPt(leftBound, leftBound.Bot);
						}
						base.InsertScanbeam(leftBound.Top.Y);
					}
					else
					{
						this.InsertEdgeIntoAEL(leftBound, null);
						this.InsertEdgeIntoAEL(rightBound, leftBound);
						this.SetWindingCount(leftBound);
						rightBound.WindCnt = leftBound.WindCnt;
						rightBound.WindCnt2 = leftBound.WindCnt2;
						bool flag5 = this.IsContributing(leftBound);
						if (flag5)
						{
							outPt = this.AddLocalMinPoly(leftBound, rightBound, leftBound.Bot);
						}
						base.InsertScanbeam(leftBound.Top.Y);
					}
				}
				bool flag6 = rightBound != null;
				if (flag6)
				{
					bool flag7 = ClipperBase.IsHorizontal(rightBound);
					if (flag7)
					{
						bool flag8 = rightBound.NextInLML != null;
						if (flag8)
						{
							base.InsertScanbeam(rightBound.NextInLML.Top.Y);
						}
						this.AddEdgeToSEL(rightBound);
					}
					else
					{
						base.InsertScanbeam(rightBound.Top.Y);
					}
				}
				bool flag9 = leftBound == null || rightBound == null;
				if (!flag9)
				{
					bool flag10 = outPt != null && ClipperBase.IsHorizontal(rightBound) && this.m_GhostJoins.Count > 0 && rightBound.WindDelta != 0;
					if (flag10)
					{
						for (int i = 0; i < this.m_GhostJoins.Count; i++)
						{
							Join join = this.m_GhostJoins[i];
							bool flag11 = this.HorzSegmentsOverlap(join.OutPt1.Pt.X, join.OffPt.X, rightBound.Bot.X, rightBound.Top.X);
							if (flag11)
							{
								this.AddJoin(join.OutPt1, outPt, join.OffPt);
							}
						}
					}
					bool flag12 = leftBound.OutIdx >= 0 && leftBound.PrevInAEL != null && leftBound.PrevInAEL.Curr.X == leftBound.Bot.X && leftBound.PrevInAEL.OutIdx >= 0 && ClipperBase.SlopesEqual(leftBound.PrevInAEL.Curr, leftBound.PrevInAEL.Top, leftBound.Curr, leftBound.Top, this.m_UseFullRange) && leftBound.WindDelta != 0 && leftBound.PrevInAEL.WindDelta != 0;
					if (flag12)
					{
						OutPt outPt2 = this.AddOutPt(leftBound.PrevInAEL, leftBound.Bot);
						this.AddJoin(outPt, outPt2, leftBound.Top);
					}
					bool flag13 = leftBound.NextInAEL != rightBound;
					if (flag13)
					{
						bool flag14 = rightBound.OutIdx >= 0 && rightBound.PrevInAEL.OutIdx >= 0 && ClipperBase.SlopesEqual(rightBound.PrevInAEL.Curr, rightBound.PrevInAEL.Top, rightBound.Curr, rightBound.Top, this.m_UseFullRange) && rightBound.WindDelta != 0 && rightBound.PrevInAEL.WindDelta != 0;
						if (flag14)
						{
							OutPt outPt3 = this.AddOutPt(rightBound.PrevInAEL, rightBound.Bot);
							this.AddJoin(outPt, outPt3, rightBound.Top);
						}
						TEdge tedge = leftBound.NextInAEL;
						bool flag15 = tedge != null;
						if (flag15)
						{
							while (tedge != rightBound)
							{
								this.IntersectEdges(rightBound, tedge, leftBound.Curr);
								tedge = tedge.NextInAEL;
							}
						}
					}
				}
			}
		}

		private void InsertEdgeIntoAEL(TEdge edge, TEdge startEdge)
		{
			bool flag = this.m_ActiveEdges == null;
			if (flag)
			{
				edge.PrevInAEL = null;
				edge.NextInAEL = null;
				this.m_ActiveEdges = edge;
			}
			else
			{
				bool flag2 = startEdge == null && this.E2InsertsBeforeE1(this.m_ActiveEdges, edge);
				if (flag2)
				{
					edge.PrevInAEL = null;
					edge.NextInAEL = this.m_ActiveEdges;
					this.m_ActiveEdges.PrevInAEL = edge;
					this.m_ActiveEdges = edge;
				}
				else
				{
					bool flag3 = startEdge == null;
					if (flag3)
					{
						startEdge = this.m_ActiveEdges;
					}
					while (startEdge.NextInAEL != null && !this.E2InsertsBeforeE1(startEdge.NextInAEL, edge))
					{
						startEdge = startEdge.NextInAEL;
					}
					edge.NextInAEL = startEdge.NextInAEL;
					bool flag4 = startEdge.NextInAEL != null;
					if (flag4)
					{
						startEdge.NextInAEL.PrevInAEL = edge;
					}
					edge.PrevInAEL = startEdge;
					startEdge.NextInAEL = edge;
				}
			}
		}

		private bool E2InsertsBeforeE1(TEdge e1, TEdge e2)
		{
			bool flag = e2.Curr.X == e1.Curr.X;
			bool flag3;
			if (flag)
			{
				bool flag2 = e2.Top.Y > e1.Top.Y;
				if (flag2)
				{
					flag3 = e2.Top.X < Clipper.TopX(e1, e2.Top.Y);
				}
				else
				{
					flag3 = e1.Top.X > Clipper.TopX(e2, e1.Top.Y);
				}
			}
			else
			{
				flag3 = e2.Curr.X < e1.Curr.X;
			}
			return flag3;
		}

		private bool IsEvenOddFillType(TEdge edge)
		{
			bool flag = edge.PolyTyp == PolyType.ptSubject;
			bool flag2;
			if (flag)
			{
				flag2 = this.m_SubjFillType == PolyFillType.pftEvenOdd;
			}
			else
			{
				flag2 = this.m_ClipFillType == PolyFillType.pftEvenOdd;
			}
			return flag2;
		}

		private bool IsEvenOddAltFillType(TEdge edge)
		{
			bool flag = edge.PolyTyp == PolyType.ptSubject;
			bool flag2;
			if (flag)
			{
				flag2 = this.m_ClipFillType == PolyFillType.pftEvenOdd;
			}
			else
			{
				flag2 = this.m_SubjFillType == PolyFillType.pftEvenOdd;
			}
			return flag2;
		}

		private bool IsContributing(TEdge edge)
		{
			bool flag = edge.PolyTyp == PolyType.ptSubject;
			PolyFillType polyFillType;
			PolyFillType polyFillType2;
			if (flag)
			{
				polyFillType = this.m_SubjFillType;
				polyFillType2 = this.m_ClipFillType;
			}
			else
			{
				polyFillType = this.m_ClipFillType;
				polyFillType2 = this.m_SubjFillType;
			}
			switch (polyFillType)
			{
			case PolyFillType.pftEvenOdd:
			{
				bool flag2 = edge.WindDelta == 0 && edge.WindCnt != 1;
				if (flag2)
				{
					return false;
				}
				break;
			}
			case PolyFillType.pftNonZero:
			{
				bool flag3 = Math.Abs(edge.WindCnt) != 1;
				if (flag3)
				{
					return false;
				}
				break;
			}
			case PolyFillType.pftPositive:
			{
				bool flag4 = edge.WindCnt != 1;
				if (flag4)
				{
					return false;
				}
				break;
			}
			default:
			{
				bool flag5 = edge.WindCnt != -1;
				if (flag5)
				{
					return false;
				}
				break;
			}
			}
			bool flag6;
			switch (this.m_ClipType)
			{
			case ClipType.ctIntersection:
			{
				PolyFillType polyFillType3 = polyFillType2;
				PolyFillType polyFillType4 = polyFillType3;
				if (polyFillType4 > PolyFillType.pftNonZero)
				{
					if (polyFillType4 != PolyFillType.pftPositive)
					{
						flag6 = edge.WindCnt2 < 0;
					}
					else
					{
						flag6 = edge.WindCnt2 > 0;
					}
				}
				else
				{
					flag6 = edge.WindCnt2 != 0;
				}
				break;
			}
			case ClipType.ctUnion:
			{
				PolyFillType polyFillType5 = polyFillType2;
				PolyFillType polyFillType6 = polyFillType5;
				if (polyFillType6 > PolyFillType.pftNonZero)
				{
					if (polyFillType6 != PolyFillType.pftPositive)
					{
						flag6 = edge.WindCnt2 >= 0;
					}
					else
					{
						flag6 = edge.WindCnt2 <= 0;
					}
				}
				else
				{
					flag6 = edge.WindCnt2 == 0;
				}
				break;
			}
			case ClipType.ctDifference:
			{
				bool flag7 = edge.PolyTyp == PolyType.ptSubject;
				if (flag7)
				{
					PolyFillType polyFillType7 = polyFillType2;
					PolyFillType polyFillType8 = polyFillType7;
					if (polyFillType8 > PolyFillType.pftNonZero)
					{
						if (polyFillType8 != PolyFillType.pftPositive)
						{
							flag6 = edge.WindCnt2 >= 0;
						}
						else
						{
							flag6 = edge.WindCnt2 <= 0;
						}
					}
					else
					{
						flag6 = edge.WindCnt2 == 0;
					}
				}
				else
				{
					PolyFillType polyFillType9 = polyFillType2;
					PolyFillType polyFillType10 = polyFillType9;
					if (polyFillType10 > PolyFillType.pftNonZero)
					{
						if (polyFillType10 != PolyFillType.pftPositive)
						{
							flag6 = edge.WindCnt2 < 0;
						}
						else
						{
							flag6 = edge.WindCnt2 > 0;
						}
					}
					else
					{
						flag6 = edge.WindCnt2 != 0;
					}
				}
				break;
			}
			case ClipType.ctXor:
			{
				bool flag8 = edge.WindDelta == 0;
				if (flag8)
				{
					PolyFillType polyFillType11 = polyFillType2;
					PolyFillType polyFillType12 = polyFillType11;
					if (polyFillType12 > PolyFillType.pftNonZero)
					{
						if (polyFillType12 != PolyFillType.pftPositive)
						{
							flag6 = edge.WindCnt2 >= 0;
						}
						else
						{
							flag6 = edge.WindCnt2 <= 0;
						}
					}
					else
					{
						flag6 = edge.WindCnt2 == 0;
					}
				}
				else
				{
					flag6 = true;
				}
				break;
			}
			default:
				flag6 = true;
				break;
			}
			return flag6;
		}

		private void SetWindingCount(TEdge edge)
		{
			TEdge tedge = edge.PrevInAEL;
			while (tedge != null && (tedge.PolyTyp != edge.PolyTyp || tedge.WindDelta == 0))
			{
				tedge = tedge.PrevInAEL;
			}
			bool flag = tedge == null;
			if (flag)
			{
				PolyFillType polyFillType = ((edge.PolyTyp == PolyType.ptSubject) ? this.m_SubjFillType : this.m_ClipFillType);
				bool flag2 = edge.WindDelta == 0;
				if (flag2)
				{
					edge.WindCnt = ((polyFillType == PolyFillType.pftNegative) ? (-1) : 1);
				}
				else
				{
					edge.WindCnt = edge.WindDelta;
				}
				edge.WindCnt2 = 0;
				tedge = this.m_ActiveEdges;
			}
			else
			{
				bool flag3 = edge.WindDelta == 0 && this.m_ClipType != ClipType.ctUnion;
				if (flag3)
				{
					edge.WindCnt = 1;
					edge.WindCnt2 = tedge.WindCnt2;
					tedge = tedge.NextInAEL;
				}
				else
				{
					bool flag4 = this.IsEvenOddFillType(edge);
					if (flag4)
					{
						bool flag5 = edge.WindDelta == 0;
						if (flag5)
						{
							bool flag6 = true;
							for (TEdge tedge2 = tedge.PrevInAEL; tedge2 != null; tedge2 = tedge2.PrevInAEL)
							{
								bool flag7 = tedge2.PolyTyp == tedge.PolyTyp && tedge2.WindDelta != 0;
								if (flag7)
								{
									flag6 = !flag6;
								}
							}
							edge.WindCnt = (flag6 ? 0 : 1);
						}
						else
						{
							edge.WindCnt = edge.WindDelta;
						}
						edge.WindCnt2 = tedge.WindCnt2;
						tedge = tedge.NextInAEL;
					}
					else
					{
						bool flag8 = tedge.WindCnt * tedge.WindDelta < 0;
						if (flag8)
						{
							bool flag9 = Math.Abs(tedge.WindCnt) > 1;
							if (flag9)
							{
								bool flag10 = tedge.WindDelta * edge.WindDelta < 0;
								if (flag10)
								{
									edge.WindCnt = tedge.WindCnt;
								}
								else
								{
									edge.WindCnt = tedge.WindCnt + edge.WindDelta;
								}
							}
							else
							{
								edge.WindCnt = ((edge.WindDelta == 0) ? 1 : edge.WindDelta);
							}
						}
						else
						{
							bool flag11 = edge.WindDelta == 0;
							if (flag11)
							{
								edge.WindCnt = ((tedge.WindCnt < 0) ? (tedge.WindCnt - 1) : (tedge.WindCnt + 1));
							}
							else
							{
								bool flag12 = tedge.WindDelta * edge.WindDelta < 0;
								if (flag12)
								{
									edge.WindCnt = tedge.WindCnt;
								}
								else
								{
									edge.WindCnt = tedge.WindCnt + edge.WindDelta;
								}
							}
						}
						edge.WindCnt2 = tedge.WindCnt2;
						tedge = tedge.NextInAEL;
					}
				}
			}
			bool flag13 = this.IsEvenOddAltFillType(edge);
			if (flag13)
			{
				while (tedge != edge)
				{
					bool flag14 = tedge.WindDelta != 0;
					if (flag14)
					{
						edge.WindCnt2 = ((edge.WindCnt2 == 0) ? 1 : 0);
					}
					tedge = tedge.NextInAEL;
				}
			}
			else
			{
				while (tedge != edge)
				{
					edge.WindCnt2 += tedge.WindDelta;
					tedge = tedge.NextInAEL;
				}
			}
		}

		private void AddEdgeToSEL(TEdge edge)
		{
			bool flag = this.m_SortedEdges == null;
			if (flag)
			{
				this.m_SortedEdges = edge;
				edge.PrevInSEL = null;
				edge.NextInSEL = null;
			}
			else
			{
				edge.NextInSEL = this.m_SortedEdges;
				edge.PrevInSEL = null;
				this.m_SortedEdges.PrevInSEL = edge;
				this.m_SortedEdges = edge;
			}
		}

		internal bool PopEdgeFromSEL(out TEdge e)
		{
			e = this.m_SortedEdges;
			bool flag = e == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				TEdge tedge = e;
				this.m_SortedEdges = e.NextInSEL;
				bool flag3 = this.m_SortedEdges != null;
				if (flag3)
				{
					this.m_SortedEdges.PrevInSEL = null;
				}
				tedge.NextInSEL = null;
				tedge.PrevInSEL = null;
				flag2 = true;
			}
			return flag2;
		}

		private void CopyAELToSEL()
		{
			TEdge tedge = this.m_ActiveEdges;
			this.m_SortedEdges = tedge;
			while (tedge != null)
			{
				tedge.PrevInSEL = tedge.PrevInAEL;
				tedge.NextInSEL = tedge.NextInAEL;
				tedge = tedge.NextInAEL;
			}
		}

		private void SwapPositionsInSEL(TEdge edge1, TEdge edge2)
		{
			bool flag = edge1.NextInSEL == null && edge1.PrevInSEL == null;
			if (!flag)
			{
				bool flag2 = edge2.NextInSEL == null && edge2.PrevInSEL == null;
				if (!flag2)
				{
					bool flag3 = edge1.NextInSEL == edge2;
					if (flag3)
					{
						TEdge nextInSEL = edge2.NextInSEL;
						bool flag4 = nextInSEL != null;
						if (flag4)
						{
							nextInSEL.PrevInSEL = edge1;
						}
						TEdge prevInSEL = edge1.PrevInSEL;
						bool flag5 = prevInSEL != null;
						if (flag5)
						{
							prevInSEL.NextInSEL = edge2;
						}
						edge2.PrevInSEL = prevInSEL;
						edge2.NextInSEL = edge1;
						edge1.PrevInSEL = edge2;
						edge1.NextInSEL = nextInSEL;
					}
					else
					{
						bool flag6 = edge2.NextInSEL == edge1;
						if (flag6)
						{
							TEdge nextInSEL2 = edge1.NextInSEL;
							bool flag7 = nextInSEL2 != null;
							if (flag7)
							{
								nextInSEL2.PrevInSEL = edge2;
							}
							TEdge prevInSEL2 = edge2.PrevInSEL;
							bool flag8 = prevInSEL2 != null;
							if (flag8)
							{
								prevInSEL2.NextInSEL = edge1;
							}
							edge1.PrevInSEL = prevInSEL2;
							edge1.NextInSEL = edge2;
							edge2.PrevInSEL = edge1;
							edge2.NextInSEL = nextInSEL2;
						}
						else
						{
							TEdge nextInSEL3 = edge1.NextInSEL;
							TEdge prevInSEL3 = edge1.PrevInSEL;
							edge1.NextInSEL = edge2.NextInSEL;
							bool flag9 = edge1.NextInSEL != null;
							if (flag9)
							{
								edge1.NextInSEL.PrevInSEL = edge1;
							}
							edge1.PrevInSEL = edge2.PrevInSEL;
							bool flag10 = edge1.PrevInSEL != null;
							if (flag10)
							{
								edge1.PrevInSEL.NextInSEL = edge1;
							}
							edge2.NextInSEL = nextInSEL3;
							bool flag11 = edge2.NextInSEL != null;
							if (flag11)
							{
								edge2.NextInSEL.PrevInSEL = edge2;
							}
							edge2.PrevInSEL = prevInSEL3;
							bool flag12 = edge2.PrevInSEL != null;
							if (flag12)
							{
								edge2.PrevInSEL.NextInSEL = edge2;
							}
						}
					}
					bool flag13 = edge1.PrevInSEL == null;
					if (flag13)
					{
						this.m_SortedEdges = edge1;
					}
					else
					{
						bool flag14 = edge2.PrevInSEL == null;
						if (flag14)
						{
							this.m_SortedEdges = edge2;
						}
					}
				}
			}
		}

		private void AddLocalMaxPoly(TEdge e1, TEdge e2, IntPoint pt)
		{
			this.AddOutPt(e1, pt);
			bool flag = e2.WindDelta == 0;
			if (flag)
			{
				this.AddOutPt(e2, pt);
			}
			bool flag2 = e1.OutIdx == e2.OutIdx;
			if (flag2)
			{
				e1.OutIdx = -1;
				e2.OutIdx = -1;
			}
			else
			{
				bool flag3 = e1.OutIdx < e2.OutIdx;
				if (flag3)
				{
					this.AppendPolygon(e1, e2);
				}
				else
				{
					this.AppendPolygon(e2, e1);
				}
			}
		}

		private OutPt AddLocalMinPoly(TEdge e1, TEdge e2, IntPoint pt)
		{
			bool flag = ClipperBase.IsHorizontal(e2) || e1.Dx > e2.Dx;
			OutPt outPt;
			TEdge tedge;
			TEdge tedge2;
			if (flag)
			{
				outPt = this.AddOutPt(e1, pt);
				e2.OutIdx = e1.OutIdx;
				e1.Side = EdgeSide.esLeft;
				e2.Side = EdgeSide.esRight;
				tedge = e1;
				bool flag2 = tedge.PrevInAEL == e2;
				if (flag2)
				{
					tedge2 = e2.PrevInAEL;
				}
				else
				{
					tedge2 = tedge.PrevInAEL;
				}
			}
			else
			{
				outPt = this.AddOutPt(e2, pt);
				e1.OutIdx = e2.OutIdx;
				e1.Side = EdgeSide.esRight;
				e2.Side = EdgeSide.esLeft;
				tedge = e2;
				bool flag3 = tedge.PrevInAEL == e1;
				if (flag3)
				{
					tedge2 = e1.PrevInAEL;
				}
				else
				{
					tedge2 = tedge.PrevInAEL;
				}
			}
			bool flag4 = tedge2 != null && tedge2.OutIdx >= 0 && tedge2.Top.Y < pt.Y && tedge.Top.Y < pt.Y;
			if (flag4)
			{
				long num = Clipper.TopX(tedge2, pt.Y);
				long num2 = Clipper.TopX(tedge, pt.Y);
				bool flag5 = num == num2 && tedge.WindDelta != 0 && tedge2.WindDelta != 0 && ClipperBase.SlopesEqual(new IntPoint(num, pt.Y), tedge2.Top, new IntPoint(num2, pt.Y), tedge.Top, this.m_UseFullRange);
				if (flag5)
				{
					OutPt outPt2 = this.AddOutPt(tedge2, pt);
					this.AddJoin(outPt, outPt2, tedge.Top);
				}
			}
			return outPt;
		}

		private OutPt AddOutPt(TEdge e, IntPoint pt)
		{
			bool flag = e.OutIdx < 0;
			OutPt outPt2;
			if (flag)
			{
				OutRec outRec = base.CreateOutRec();
				outRec.IsOpen = e.WindDelta == 0;
				OutPt outPt = new OutPt();
				outRec.Pts = outPt;
				outPt.Idx = outRec.Idx;
				outPt.Pt = pt;
				outPt.Next = outPt;
				outPt.Prev = outPt;
				bool flag2 = !outRec.IsOpen;
				if (flag2)
				{
					this.SetHoleState(e, outRec);
				}
				e.OutIdx = outRec.Idx;
				outPt2 = outPt;
			}
			else
			{
				OutRec outRec2 = this.m_PolyOuts[e.OutIdx];
				OutPt pts = outRec2.Pts;
				bool flag3 = e.Side == EdgeSide.esLeft;
				bool flag4 = flag3 && pt == pts.Pt;
				if (flag4)
				{
					outPt2 = pts;
				}
				else
				{
					bool flag5 = !flag3 && pt == pts.Prev.Pt;
					if (flag5)
					{
						outPt2 = pts.Prev;
					}
					else
					{
						OutPt outPt3 = new OutPt();
						outPt3.Idx = outRec2.Idx;
						outPt3.Pt = pt;
						outPt3.Next = pts;
						outPt3.Prev = pts.Prev;
						outPt3.Prev.Next = outPt3;
						pts.Prev = outPt3;
						bool flag6 = flag3;
						if (flag6)
						{
							outRec2.Pts = outPt3;
						}
						outPt2 = outPt3;
					}
				}
			}
			return outPt2;
		}

		private OutPt GetLastOutPt(TEdge e)
		{
			OutRec outRec = this.m_PolyOuts[e.OutIdx];
			bool flag = e.Side == EdgeSide.esLeft;
			OutPt outPt;
			if (flag)
			{
				outPt = outRec.Pts;
			}
			else
			{
				outPt = outRec.Pts.Prev;
			}
			return outPt;
		}

		internal void SwapPoints(ref IntPoint pt1, ref IntPoint pt2)
		{
			IntPoint intPoint = new IntPoint(pt1);
			pt1 = pt2;
			pt2 = intPoint;
		}

		private bool HorzSegmentsOverlap(long seg1a, long seg1b, long seg2a, long seg2b)
		{
			bool flag = seg1a > seg1b;
			if (flag)
			{
				base.Swap(ref seg1a, ref seg1b);
			}
			bool flag2 = seg2a > seg2b;
			if (flag2)
			{
				base.Swap(ref seg2a, ref seg2b);
			}
			return seg1a < seg2b && seg2a < seg1b;
		}

		private void SetHoleState(TEdge e, OutRec outRec)
		{
			TEdge tedge = e.PrevInAEL;
			TEdge tedge2 = null;
			while (tedge != null)
			{
				bool flag = tedge.OutIdx >= 0 && tedge.WindDelta != 0;
				if (flag)
				{
					bool flag2 = tedge2 == null;
					if (flag2)
					{
						tedge2 = tedge;
					}
					else
					{
						bool flag3 = tedge2.OutIdx == tedge.OutIdx;
						if (flag3)
						{
							tedge2 = null;
						}
					}
				}
				tedge = tedge.PrevInAEL;
			}
			bool flag4 = tedge2 == null;
			if (flag4)
			{
				outRec.FirstLeft = null;
				outRec.IsHole = false;
			}
			else
			{
				outRec.FirstLeft = this.m_PolyOuts[tedge2.OutIdx];
				outRec.IsHole = !outRec.FirstLeft.IsHole;
			}
		}

		private double GetDx(IntPoint pt1, IntPoint pt2)
		{
			bool flag = pt1.Y == pt2.Y;
			double num;
			if (flag)
			{
				num = -3.4E+38;
			}
			else
			{
				num = (double)(pt2.X - pt1.X) / (double)(pt2.Y - pt1.Y);
			}
			return num;
		}

		private bool FirstIsBottomPt(OutPt btmPt1, OutPt btmPt2)
		{
			OutPt outPt = btmPt1.Prev;
			while (outPt.Pt == btmPt1.Pt && outPt != btmPt1)
			{
				outPt = outPt.Prev;
			}
			double num = Math.Abs(this.GetDx(btmPt1.Pt, outPt.Pt));
			outPt = btmPt1.Next;
			while (outPt.Pt == btmPt1.Pt && outPt != btmPt1)
			{
				outPt = outPt.Next;
			}
			double num2 = Math.Abs(this.GetDx(btmPt1.Pt, outPt.Pt));
			outPt = btmPt2.Prev;
			while (outPt.Pt == btmPt2.Pt && outPt != btmPt2)
			{
				outPt = outPt.Prev;
			}
			double num3 = Math.Abs(this.GetDx(btmPt2.Pt, outPt.Pt));
			outPt = btmPt2.Next;
			while (outPt.Pt == btmPt2.Pt && outPt != btmPt2)
			{
				outPt = outPt.Next;
			}
			double num4 = Math.Abs(this.GetDx(btmPt2.Pt, outPt.Pt));
			bool flag = Math.Max(num, num2) == Math.Max(num3, num4) && Math.Min(num, num2) == Math.Min(num3, num4);
			bool flag2;
			if (flag)
			{
				flag2 = this.Area(btmPt1) > 0.0;
			}
			else
			{
				flag2 = (num >= num3 && num >= num4) || (num2 >= num3 && num2 >= num4);
			}
			return flag2;
		}

		private OutPt GetBottomPt(OutPt pp)
		{
			OutPt outPt = null;
			OutPt outPt2;
			for (outPt2 = pp.Next; outPt2 != pp; outPt2 = outPt2.Next)
			{
				bool flag = outPt2.Pt.Y > pp.Pt.Y;
				if (flag)
				{
					pp = outPt2;
					outPt = null;
				}
				else
				{
					bool flag2 = outPt2.Pt.Y == pp.Pt.Y && outPt2.Pt.X <= pp.Pt.X;
					if (flag2)
					{
						bool flag3 = outPt2.Pt.X < pp.Pt.X;
						if (flag3)
						{
							outPt = null;
							pp = outPt2;
						}
						else
						{
							bool flag4 = outPt2.Next != pp && outPt2.Prev != pp;
							if (flag4)
							{
								outPt = outPt2;
							}
						}
					}
				}
			}
			bool flag5 = outPt != null;
			if (flag5)
			{
				while (outPt != outPt2)
				{
					bool flag6 = !this.FirstIsBottomPt(outPt2, outPt);
					if (flag6)
					{
						pp = outPt;
					}
					outPt = outPt.Next;
					while (outPt.Pt != pp.Pt)
					{
						outPt = outPt.Next;
					}
				}
			}
			return pp;
		}

		private OutRec GetLowermostRec(OutRec outRec1, OutRec outRec2)
		{
			bool flag = outRec1.BottomPt == null;
			if (flag)
			{
				outRec1.BottomPt = this.GetBottomPt(outRec1.Pts);
			}
			bool flag2 = outRec2.BottomPt == null;
			if (flag2)
			{
				outRec2.BottomPt = this.GetBottomPt(outRec2.Pts);
			}
			OutPt bottomPt = outRec1.BottomPt;
			OutPt bottomPt2 = outRec2.BottomPt;
			bool flag3 = bottomPt.Pt.Y > bottomPt2.Pt.Y;
			OutRec outRec3;
			if (flag3)
			{
				outRec3 = outRec1;
			}
			else
			{
				bool flag4 = bottomPt.Pt.Y < bottomPt2.Pt.Y;
				if (flag4)
				{
					outRec3 = outRec2;
				}
				else
				{
					bool flag5 = bottomPt.Pt.X < bottomPt2.Pt.X;
					if (flag5)
					{
						outRec3 = outRec1;
					}
					else
					{
						bool flag6 = bottomPt.Pt.X > bottomPt2.Pt.X;
						if (flag6)
						{
							outRec3 = outRec2;
						}
						else
						{
							bool flag7 = bottomPt.Next == bottomPt;
							if (flag7)
							{
								outRec3 = outRec2;
							}
							else
							{
								bool flag8 = bottomPt2.Next == bottomPt2;
								if (flag8)
								{
									outRec3 = outRec1;
								}
								else
								{
									bool flag9 = this.FirstIsBottomPt(bottomPt, bottomPt2);
									if (flag9)
									{
										outRec3 = outRec1;
									}
									else
									{
										outRec3 = outRec2;
									}
								}
							}
						}
					}
				}
			}
			return outRec3;
		}

		private bool OutRec1RightOfOutRec2(OutRec outRec1, OutRec outRec2)
		{
			for (;;)
			{
				outRec1 = outRec1.FirstLeft;
				bool flag = outRec1 == outRec2;
				if (flag)
				{
					break;
				}
				if (outRec1 == null)
				{
					goto Block_1;
				}
			}
			return true;
			Block_1:
			return false;
		}

		private OutRec GetOutRec(int idx)
		{
			OutRec outRec;
			for (outRec = this.m_PolyOuts[idx]; outRec != this.m_PolyOuts[outRec.Idx]; outRec = this.m_PolyOuts[outRec.Idx])
			{
			}
			return outRec;
		}

		private void AppendPolygon(TEdge e1, TEdge e2)
		{
			OutRec outRec = this.m_PolyOuts[e1.OutIdx];
			OutRec outRec2 = this.m_PolyOuts[e2.OutIdx];
			bool flag = this.OutRec1RightOfOutRec2(outRec, outRec2);
			OutRec outRec3;
			if (flag)
			{
				outRec3 = outRec2;
			}
			else
			{
				bool flag2 = this.OutRec1RightOfOutRec2(outRec2, outRec);
				if (flag2)
				{
					outRec3 = outRec;
				}
				else
				{
					outRec3 = this.GetLowermostRec(outRec, outRec2);
				}
			}
			OutPt pts = outRec.Pts;
			OutPt prev = pts.Prev;
			OutPt pts2 = outRec2.Pts;
			OutPt prev2 = pts2.Prev;
			bool flag3 = e1.Side == EdgeSide.esLeft;
			if (flag3)
			{
				bool flag4 = e2.Side == EdgeSide.esLeft;
				if (flag4)
				{
					this.ReversePolyPtLinks(pts2);
					pts2.Next = pts;
					pts.Prev = pts2;
					prev.Next = prev2;
					prev2.Prev = prev;
					outRec.Pts = prev2;
				}
				else
				{
					prev2.Next = pts;
					pts.Prev = prev2;
					pts2.Prev = prev;
					prev.Next = pts2;
					outRec.Pts = pts2;
				}
			}
			else
			{
				bool flag5 = e2.Side == EdgeSide.esRight;
				if (flag5)
				{
					this.ReversePolyPtLinks(pts2);
					prev.Next = prev2;
					prev2.Prev = prev;
					pts2.Next = pts;
					pts.Prev = pts2;
				}
				else
				{
					prev.Next = pts2;
					pts2.Prev = prev;
					pts.Prev = prev2;
					prev2.Next = pts;
				}
			}
			outRec.BottomPt = null;
			bool flag6 = outRec3 == outRec2;
			if (flag6)
			{
				bool flag7 = outRec2.FirstLeft != outRec;
				if (flag7)
				{
					outRec.FirstLeft = outRec2.FirstLeft;
				}
				outRec.IsHole = outRec2.IsHole;
			}
			outRec2.Pts = null;
			outRec2.BottomPt = null;
			outRec2.FirstLeft = outRec;
			int outIdx = e1.OutIdx;
			int outIdx2 = e2.OutIdx;
			e1.OutIdx = -1;
			e2.OutIdx = -1;
			for (TEdge tedge = this.m_ActiveEdges; tedge != null; tedge = tedge.NextInAEL)
			{
				bool flag8 = tedge.OutIdx == outIdx2;
				if (flag8)
				{
					tedge.OutIdx = outIdx;
					tedge.Side = e1.Side;
					break;
				}
			}
			outRec2.Idx = outRec.Idx;
		}

		private void ReversePolyPtLinks(OutPt pp)
		{
			bool flag = pp == null;
			if (!flag)
			{
				OutPt outPt = pp;
				do
				{
					OutPt next = outPt.Next;
					outPt.Next = outPt.Prev;
					outPt.Prev = next;
					outPt = next;
				}
				while (outPt != pp);
			}
		}

		private static void SwapSides(TEdge edge1, TEdge edge2)
		{
			EdgeSide side = edge1.Side;
			edge1.Side = edge2.Side;
			edge2.Side = side;
		}

		private static void SwapPolyIndexes(TEdge edge1, TEdge edge2)
		{
			int outIdx = edge1.OutIdx;
			edge1.OutIdx = edge2.OutIdx;
			edge2.OutIdx = outIdx;
		}

		private void IntersectEdges(TEdge e1, TEdge e2, IntPoint pt)
		{
			bool flag = e1.OutIdx >= 0;
			bool flag2 = e2.OutIdx >= 0;
			bool flag3 = e1.WindDelta == 0 || e2.WindDelta == 0;
			if (flag3)
			{
				bool flag4 = e1.WindDelta == 0 && e2.WindDelta == 0;
				if (!flag4)
				{
					bool flag5 = e1.PolyTyp == e2.PolyTyp && e1.WindDelta != e2.WindDelta && this.m_ClipType == ClipType.ctUnion;
					if (flag5)
					{
						bool flag6 = e1.WindDelta == 0;
						if (flag6)
						{
							bool flag7 = flag2;
							if (flag7)
							{
								this.AddOutPt(e1, pt);
								bool flag8 = flag;
								if (flag8)
								{
									e1.OutIdx = -1;
								}
							}
						}
						else
						{
							bool flag9 = flag;
							if (flag9)
							{
								this.AddOutPt(e2, pt);
								bool flag10 = flag2;
								if (flag10)
								{
									e2.OutIdx = -1;
								}
							}
						}
					}
					else
					{
						bool flag11 = e1.PolyTyp != e2.PolyTyp;
						if (flag11)
						{
							bool flag12 = e1.WindDelta == 0 && Math.Abs(e2.WindCnt) == 1 && (this.m_ClipType != ClipType.ctUnion || e2.WindCnt2 == 0);
							if (flag12)
							{
								this.AddOutPt(e1, pt);
								bool flag13 = flag;
								if (flag13)
								{
									e1.OutIdx = -1;
								}
							}
							else
							{
								bool flag14 = e2.WindDelta == 0 && Math.Abs(e1.WindCnt) == 1 && (this.m_ClipType != ClipType.ctUnion || e1.WindCnt2 == 0);
								if (flag14)
								{
									this.AddOutPt(e2, pt);
									bool flag15 = flag2;
									if (flag15)
									{
										e2.OutIdx = -1;
									}
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag16 = e1.PolyTyp == e2.PolyTyp;
				if (flag16)
				{
					bool flag17 = this.IsEvenOddFillType(e1);
					if (flag17)
					{
						int windCnt = e1.WindCnt;
						e1.WindCnt = e2.WindCnt;
						e2.WindCnt = windCnt;
					}
					else
					{
						bool flag18 = e1.WindCnt + e2.WindDelta == 0;
						if (flag18)
						{
							e1.WindCnt = -e1.WindCnt;
						}
						else
						{
							e1.WindCnt += e2.WindDelta;
						}
						bool flag19 = e2.WindCnt - e1.WindDelta == 0;
						if (flag19)
						{
							e2.WindCnt = -e2.WindCnt;
						}
						else
						{
							e2.WindCnt -= e1.WindDelta;
						}
					}
				}
				else
				{
					bool flag20 = !this.IsEvenOddFillType(e2);
					if (flag20)
					{
						e1.WindCnt2 += e2.WindDelta;
					}
					else
					{
						e1.WindCnt2 = ((e1.WindCnt2 == 0) ? 1 : 0);
					}
					bool flag21 = !this.IsEvenOddFillType(e1);
					if (flag21)
					{
						e2.WindCnt2 -= e1.WindDelta;
					}
					else
					{
						e2.WindCnt2 = ((e2.WindCnt2 == 0) ? 1 : 0);
					}
				}
				bool flag22 = e1.PolyTyp == PolyType.ptSubject;
				PolyFillType polyFillType;
				PolyFillType polyFillType2;
				if (flag22)
				{
					polyFillType = this.m_SubjFillType;
					polyFillType2 = this.m_ClipFillType;
				}
				else
				{
					polyFillType = this.m_ClipFillType;
					polyFillType2 = this.m_SubjFillType;
				}
				bool flag23 = e2.PolyTyp == PolyType.ptSubject;
				PolyFillType polyFillType3;
				PolyFillType polyFillType4;
				if (flag23)
				{
					polyFillType3 = this.m_SubjFillType;
					polyFillType4 = this.m_ClipFillType;
				}
				else
				{
					polyFillType3 = this.m_ClipFillType;
					polyFillType4 = this.m_SubjFillType;
				}
				PolyFillType polyFillType5 = polyFillType;
				PolyFillType polyFillType6 = polyFillType5;
				int num;
				if (polyFillType6 != PolyFillType.pftPositive)
				{
					if (polyFillType6 != PolyFillType.pftNegative)
					{
						num = Math.Abs(e1.WindCnt);
					}
					else
					{
						num = -e1.WindCnt;
					}
				}
				else
				{
					num = e1.WindCnt;
				}
				PolyFillType polyFillType7 = polyFillType3;
				PolyFillType polyFillType8 = polyFillType7;
				int num2;
				if (polyFillType8 != PolyFillType.pftPositive)
				{
					if (polyFillType8 != PolyFillType.pftNegative)
					{
						num2 = Math.Abs(e2.WindCnt);
					}
					else
					{
						num2 = -e2.WindCnt;
					}
				}
				else
				{
					num2 = e2.WindCnt;
				}
				bool flag24 = flag && flag2;
				if (flag24)
				{
					bool flag25 = (num != 0 && num != 1) || (num2 != 0 && num2 != 1) || (e1.PolyTyp != e2.PolyTyp && this.m_ClipType != ClipType.ctXor);
					if (flag25)
					{
						this.AddLocalMaxPoly(e1, e2, pt);
					}
					else
					{
						this.AddOutPt(e1, pt);
						this.AddOutPt(e2, pt);
						Clipper.SwapSides(e1, e2);
						Clipper.SwapPolyIndexes(e1, e2);
					}
				}
				else
				{
					bool flag26 = flag;
					if (flag26)
					{
						bool flag27 = num2 == 0 || num2 == 1;
						if (flag27)
						{
							this.AddOutPt(e1, pt);
							Clipper.SwapSides(e1, e2);
							Clipper.SwapPolyIndexes(e1, e2);
						}
					}
					else
					{
						bool flag28 = flag2;
						if (flag28)
						{
							bool flag29 = num == 0 || num == 1;
							if (flag29)
							{
								this.AddOutPt(e2, pt);
								Clipper.SwapSides(e1, e2);
								Clipper.SwapPolyIndexes(e1, e2);
							}
						}
						else
						{
							bool flag30 = (num == 0 || num == 1) && (num2 == 0 || num2 == 1);
							if (flag30)
							{
								PolyFillType polyFillType9 = polyFillType2;
								PolyFillType polyFillType10 = polyFillType9;
								long num3;
								if (polyFillType10 != PolyFillType.pftPositive)
								{
									if (polyFillType10 != PolyFillType.pftNegative)
									{
										num3 = (long)Math.Abs(e1.WindCnt2);
									}
									else
									{
										num3 = (long)(-(long)e1.WindCnt2);
									}
								}
								else
								{
									num3 = (long)e1.WindCnt2;
								}
								PolyFillType polyFillType11 = polyFillType4;
								PolyFillType polyFillType12 = polyFillType11;
								long num4;
								if (polyFillType12 != PolyFillType.pftPositive)
								{
									if (polyFillType12 != PolyFillType.pftNegative)
									{
										num4 = (long)Math.Abs(e2.WindCnt2);
									}
									else
									{
										num4 = (long)(-(long)e2.WindCnt2);
									}
								}
								else
								{
									num4 = (long)e2.WindCnt2;
								}
								bool flag31 = e1.PolyTyp != e2.PolyTyp;
								if (flag31)
								{
									this.AddLocalMinPoly(e1, e2, pt);
								}
								else
								{
									bool flag32 = num == 1 && num2 == 1;
									if (flag32)
									{
										switch (this.m_ClipType)
										{
										case ClipType.ctIntersection:
										{
											bool flag33 = num3 > 0L && num4 > 0L;
											if (flag33)
											{
												this.AddLocalMinPoly(e1, e2, pt);
											}
											break;
										}
										case ClipType.ctUnion:
										{
											bool flag34 = num3 <= 0L && num4 <= 0L;
											if (flag34)
											{
												this.AddLocalMinPoly(e1, e2, pt);
											}
											break;
										}
										case ClipType.ctDifference:
										{
											bool flag35 = (e1.PolyTyp == PolyType.ptClip && num3 > 0L && num4 > 0L) || (e1.PolyTyp == PolyType.ptSubject && num3 <= 0L && num4 <= 0L);
											if (flag35)
											{
												this.AddLocalMinPoly(e1, e2, pt);
											}
											break;
										}
										case ClipType.ctXor:
											this.AddLocalMinPoly(e1, e2, pt);
											break;
										}
									}
									else
									{
										Clipper.SwapSides(e1, e2);
									}
								}
							}
						}
					}
				}
			}
		}

		private void DeleteFromSEL(TEdge e)
		{
			TEdge prevInSEL = e.PrevInSEL;
			TEdge nextInSEL = e.NextInSEL;
			bool flag = prevInSEL == null && nextInSEL == null && e != this.m_SortedEdges;
			if (!flag)
			{
				bool flag2 = prevInSEL != null;
				if (flag2)
				{
					prevInSEL.NextInSEL = nextInSEL;
				}
				else
				{
					this.m_SortedEdges = nextInSEL;
				}
				bool flag3 = nextInSEL != null;
				if (flag3)
				{
					nextInSEL.PrevInSEL = prevInSEL;
				}
				e.NextInSEL = null;
				e.PrevInSEL = null;
			}
		}

		private void ProcessHorizontals()
		{
			TEdge tedge;
			while (this.PopEdgeFromSEL(out tedge))
			{
				this.ProcessHorizontal(tedge);
			}
		}

		private void GetHorzDirection(TEdge HorzEdge, out Direction Dir, out long Left, out long Right)
		{
			bool flag = HorzEdge.Bot.X < HorzEdge.Top.X;
			if (flag)
			{
				Left = HorzEdge.Bot.X;
				Right = HorzEdge.Top.X;
				Dir = Direction.dLeftToRight;
			}
			else
			{
				Left = HorzEdge.Top.X;
				Right = HorzEdge.Bot.X;
				Dir = Direction.dRightToLeft;
			}
		}

		private void ProcessHorizontal(TEdge horzEdge)
		{
			bool flag = horzEdge.WindDelta == 0;
			Direction direction;
			long num;
			long num2;
			this.GetHorzDirection(horzEdge, out direction, out num, out num2);
			TEdge tedge = horzEdge;
			TEdge tedge2 = null;
			while (tedge.NextInLML != null && ClipperBase.IsHorizontal(tedge.NextInLML))
			{
				tedge = tedge.NextInLML;
			}
			bool flag2 = tedge.NextInLML == null;
			if (flag2)
			{
				tedge2 = this.GetMaximaPair(tedge);
			}
			Maxima maxima = this.m_Maxima;
			bool flag3 = maxima != null;
			if (flag3)
			{
				bool flag4 = direction == Direction.dLeftToRight;
				if (flag4)
				{
					while (maxima != null && maxima.X <= horzEdge.Bot.X)
					{
						maxima = maxima.Next;
					}
					bool flag5 = maxima != null && maxima.X >= tedge.Top.X;
					if (flag5)
					{
						maxima = null;
					}
				}
				else
				{
					while (maxima.Next != null && maxima.Next.X < horzEdge.Bot.X)
					{
						maxima = maxima.Next;
					}
					bool flag6 = maxima.X <= tedge.Top.X;
					if (flag6)
					{
						maxima = null;
					}
				}
			}
			OutPt outPt = null;
			for (;;)
			{
				bool flag7 = horzEdge == tedge;
				TEdge nextInAEL;
				for (TEdge tedge3 = this.GetNextInAEL(horzEdge, direction); tedge3 != null; tedge3 = nextInAEL)
				{
					bool flag8 = maxima != null;
					if (flag8)
					{
						bool flag9 = direction == Direction.dLeftToRight;
						if (flag9)
						{
							while (maxima != null && maxima.X < tedge3.Curr.X)
							{
								bool flag10 = horzEdge.OutIdx >= 0 && !flag;
								if (flag10)
								{
									this.AddOutPt(horzEdge, new IntPoint(maxima.X, horzEdge.Bot.Y));
								}
								maxima = maxima.Next;
							}
						}
						else
						{
							while (maxima != null && maxima.X > tedge3.Curr.X)
							{
								bool flag11 = horzEdge.OutIdx >= 0 && !flag;
								if (flag11)
								{
									this.AddOutPt(horzEdge, new IntPoint(maxima.X, horzEdge.Bot.Y));
								}
								maxima = maxima.Prev;
							}
						}
					}
					bool flag12 = (direction == Direction.dLeftToRight && tedge3.Curr.X > num2) || (direction == Direction.dRightToLeft && tedge3.Curr.X < num);
					if (flag12)
					{
						break;
					}
					bool flag13 = tedge3.Curr.X == horzEdge.Top.X && horzEdge.NextInLML != null && tedge3.Dx < horzEdge.NextInLML.Dx;
					if (flag13)
					{
						break;
					}
					bool flag14 = horzEdge.OutIdx >= 0 && !flag;
					if (flag14)
					{
						outPt = this.AddOutPt(horzEdge, tedge3.Curr);
						for (TEdge tedge4 = this.m_SortedEdges; tedge4 != null; tedge4 = tedge4.NextInSEL)
						{
							bool flag15 = tedge4.OutIdx >= 0 && this.HorzSegmentsOverlap(horzEdge.Bot.X, horzEdge.Top.X, tedge4.Bot.X, tedge4.Top.X);
							if (flag15)
							{
								OutPt lastOutPt = this.GetLastOutPt(tedge4);
								this.AddJoin(lastOutPt, outPt, tedge4.Top);
							}
						}
						this.AddGhostJoin(outPt, horzEdge.Bot);
					}
					bool flag16 = tedge3 == tedge2 && flag7;
					if (flag16)
					{
						goto Block_33;
					}
					bool flag17 = direction == Direction.dLeftToRight;
					if (flag17)
					{
						IntPoint intPoint = new IntPoint(tedge3.Curr.X, horzEdge.Curr.Y);
						this.IntersectEdges(horzEdge, tedge3, intPoint);
					}
					else
					{
						IntPoint intPoint2 = new IntPoint(tedge3.Curr.X, horzEdge.Curr.Y);
						this.IntersectEdges(tedge3, horzEdge, intPoint2);
					}
					nextInAEL = this.GetNextInAEL(tedge3, direction);
					base.SwapPositionsInAEL(horzEdge, tedge3);
				}
				bool flag18 = horzEdge.NextInLML == null || !ClipperBase.IsHorizontal(horzEdge.NextInLML);
				if (flag18)
				{
					goto Block_37;
				}
				base.UpdateEdgeIntoAEL(ref horzEdge);
				bool flag19 = horzEdge.OutIdx >= 0;
				if (flag19)
				{
					this.AddOutPt(horzEdge, horzEdge.Bot);
				}
				this.GetHorzDirection(horzEdge, out direction, out num, out num2);
			}
			Block_33:
			bool flag20 = horzEdge.OutIdx >= 0;
			if (flag20)
			{
				this.AddLocalMaxPoly(horzEdge, tedge2, horzEdge.Top);
			}
			base.DeleteFromAEL(horzEdge);
			base.DeleteFromAEL(tedge2);
			return;
			Block_37:
			bool flag21 = horzEdge.OutIdx >= 0 && outPt == null;
			if (flag21)
			{
				outPt = this.GetLastOutPt(horzEdge);
				for (TEdge tedge5 = this.m_SortedEdges; tedge5 != null; tedge5 = tedge5.NextInSEL)
				{
					bool flag22 = tedge5.OutIdx >= 0 && this.HorzSegmentsOverlap(horzEdge.Bot.X, horzEdge.Top.X, tedge5.Bot.X, tedge5.Top.X);
					if (flag22)
					{
						OutPt lastOutPt2 = this.GetLastOutPt(tedge5);
						this.AddJoin(lastOutPt2, outPt, tedge5.Top);
					}
				}
				this.AddGhostJoin(outPt, horzEdge.Top);
			}
			bool flag23 = horzEdge.NextInLML != null;
			if (flag23)
			{
				bool flag24 = horzEdge.OutIdx >= 0;
				if (flag24)
				{
					outPt = this.AddOutPt(horzEdge, horzEdge.Top);
					base.UpdateEdgeIntoAEL(ref horzEdge);
					bool flag25 = horzEdge.WindDelta == 0;
					if (!flag25)
					{
						TEdge prevInAEL = horzEdge.PrevInAEL;
						TEdge nextInAEL2 = horzEdge.NextInAEL;
						bool flag26 = prevInAEL != null && prevInAEL.Curr.X == horzEdge.Bot.X && prevInAEL.Curr.Y == horzEdge.Bot.Y && prevInAEL.WindDelta != 0 && (prevInAEL.OutIdx >= 0 && prevInAEL.Curr.Y > prevInAEL.Top.Y) && ClipperBase.SlopesEqual(horzEdge, prevInAEL, this.m_UseFullRange);
						if (flag26)
						{
							OutPt outPt2 = this.AddOutPt(prevInAEL, horzEdge.Bot);
							this.AddJoin(outPt, outPt2, horzEdge.Top);
						}
						else
						{
							bool flag27 = nextInAEL2 != null && nextInAEL2.Curr.X == horzEdge.Bot.X && nextInAEL2.Curr.Y == horzEdge.Bot.Y && nextInAEL2.WindDelta != 0 && nextInAEL2.OutIdx >= 0 && nextInAEL2.Curr.Y > nextInAEL2.Top.Y && ClipperBase.SlopesEqual(horzEdge, nextInAEL2, this.m_UseFullRange);
							if (flag27)
							{
								OutPt outPt3 = this.AddOutPt(nextInAEL2, horzEdge.Bot);
								this.AddJoin(outPt, outPt3, horzEdge.Top);
							}
						}
					}
				}
				else
				{
					base.UpdateEdgeIntoAEL(ref horzEdge);
				}
			}
			else
			{
				bool flag28 = horzEdge.OutIdx >= 0;
				if (flag28)
				{
					this.AddOutPt(horzEdge, horzEdge.Top);
				}
				base.DeleteFromAEL(horzEdge);
			}
		}

		private TEdge GetNextInAEL(TEdge e, Direction Direction)
		{
			return (Direction == Direction.dLeftToRight) ? e.NextInAEL : e.PrevInAEL;
		}

		private bool IsMinima(TEdge e)
		{
			return e != null && e.Prev.NextInLML != e && e.Next.NextInLML != e;
		}

		private bool IsMaxima(TEdge e, double Y)
		{
			return e != null && (double)e.Top.Y == Y && e.NextInLML == null;
		}

		private bool IsIntermediate(TEdge e, double Y)
		{
			return (double)e.Top.Y == Y && e.NextInLML != null;
		}

		internal TEdge GetMaximaPair(TEdge e)
		{
			bool flag = e.Next.Top == e.Top && e.Next.NextInLML == null;
			TEdge tedge;
			if (flag)
			{
				tedge = e.Next;
			}
			else
			{
				bool flag2 = e.Prev.Top == e.Top && e.Prev.NextInLML == null;
				if (flag2)
				{
					tedge = e.Prev;
				}
				else
				{
					tedge = null;
				}
			}
			return tedge;
		}

		internal TEdge GetMaximaPairEx(TEdge e)
		{
			TEdge maximaPair = this.GetMaximaPair(e);
			bool flag = maximaPair == null || maximaPair.OutIdx == -2 || (maximaPair.NextInAEL == maximaPair.PrevInAEL && !ClipperBase.IsHorizontal(maximaPair));
			TEdge tedge;
			if (flag)
			{
				tedge = null;
			}
			else
			{
				tedge = maximaPair;
			}
			return tedge;
		}

		private bool ProcessIntersections(long topY)
		{
			bool flag = this.m_ActiveEdges == null;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				try
				{
					this.BuildIntersectList(topY);
					bool flag3 = this.m_IntersectList.Count == 0;
					if (flag3)
					{
						return true;
					}
					bool flag4 = this.m_IntersectList.Count == 1 || this.FixupIntersectionOrder();
					if (!flag4)
					{
						return false;
					}
					this.ProcessIntersectList();
				}
				catch
				{
					this.m_SortedEdges = null;
					this.m_IntersectList.Clear();
					throw new ClipperException("ProcessIntersections error");
				}
				this.m_SortedEdges = null;
				flag2 = true;
			}
			return flag2;
		}

		private void BuildIntersectList(long topY)
		{
			bool flag = this.m_ActiveEdges == null;
			if (!flag)
			{
				TEdge tedge = this.m_ActiveEdges;
				this.m_SortedEdges = tedge;
				while (tedge != null)
				{
					tedge.PrevInSEL = tedge.PrevInAEL;
					tedge.NextInSEL = tedge.NextInAEL;
					tedge.Curr.X = Clipper.TopX(tedge, topY);
					tedge = tedge.NextInAEL;
				}
				bool flag2 = true;
				while (flag2 && this.m_SortedEdges != null)
				{
					flag2 = false;
					tedge = this.m_SortedEdges;
					while (tedge.NextInSEL != null)
					{
						TEdge nextInSEL = tedge.NextInSEL;
						bool flag3 = tedge.Curr.X > nextInSEL.Curr.X;
						if (flag3)
						{
							IntPoint intPoint;
							this.IntersectPoint(tedge, nextInSEL, out intPoint);
							bool flag4 = intPoint.Y < topY;
							if (flag4)
							{
								intPoint = new IntPoint(Clipper.TopX(tedge, topY), topY);
							}
							IntersectNode intersectNode = new IntersectNode();
							intersectNode.Edge1 = tedge;
							intersectNode.Edge2 = nextInSEL;
							intersectNode.Pt = intPoint;
							this.m_IntersectList.Add(intersectNode);
							this.SwapPositionsInSEL(tedge, nextInSEL);
							flag2 = true;
						}
						else
						{
							tedge = nextInSEL;
						}
					}
					bool flag5 = tedge.PrevInSEL != null;
					if (!flag5)
					{
						break;
					}
					tedge.PrevInSEL.NextInSEL = null;
				}
				this.m_SortedEdges = null;
			}
		}

		private bool EdgesAdjacent(IntersectNode inode)
		{
			return inode.Edge1.NextInSEL == inode.Edge2 || inode.Edge1.PrevInSEL == inode.Edge2;
		}

		private static int IntersectNodeSort(IntersectNode node1, IntersectNode node2)
		{
			return (int)(node2.Pt.Y - node1.Pt.Y);
		}

		private bool FixupIntersectionOrder()
		{
			this.m_IntersectList.Sort(this.m_IntersectNodeComparer);
			this.CopyAELToSEL();
			int count = this.m_IntersectList.Count;
			for (int i = 0; i < count; i++)
			{
				bool flag = !this.EdgesAdjacent(this.m_IntersectList[i]);
				if (flag)
				{
					int num = i + 1;
					while (num < count && !this.EdgesAdjacent(this.m_IntersectList[num]))
					{
						num++;
					}
					bool flag2 = num == count;
					if (flag2)
					{
						return false;
					}
					IntersectNode intersectNode = this.m_IntersectList[i];
					this.m_IntersectList[i] = this.m_IntersectList[num];
					this.m_IntersectList[num] = intersectNode;
				}
				this.SwapPositionsInSEL(this.m_IntersectList[i].Edge1, this.m_IntersectList[i].Edge2);
			}
			return true;
		}

		private void ProcessIntersectList()
		{
			for (int i = 0; i < this.m_IntersectList.Count; i++)
			{
				IntersectNode intersectNode = this.m_IntersectList[i];
				this.IntersectEdges(intersectNode.Edge1, intersectNode.Edge2, intersectNode.Pt);
				base.SwapPositionsInAEL(intersectNode.Edge1, intersectNode.Edge2);
			}
			this.m_IntersectList.Clear();
		}

		internal static long Round(double value)
		{
			return (value < 0.0) ? ((long)(value - 0.5)) : ((long)(value + 0.5));
		}

		private static long TopX(TEdge edge, long currentY)
		{
			bool flag = currentY == edge.Top.Y;
			long num;
			if (flag)
			{
				num = edge.Top.X;
			}
			else
			{
				num = edge.Bot.X + Clipper.Round(edge.Dx * (double)(currentY - edge.Bot.Y));
			}
			return num;
		}

		private void IntersectPoint(TEdge edge1, TEdge edge2, out IntPoint ip)
		{
			ip = default(IntPoint);
			bool flag = edge1.Dx == edge2.Dx;
			if (flag)
			{
				ip.Y = edge1.Curr.Y;
				ip.X = Clipper.TopX(edge1, ip.Y);
			}
			else
			{
				bool flag2 = edge1.Delta.X == 0L;
				if (flag2)
				{
					ip.X = edge1.Bot.X;
					bool flag3 = ClipperBase.IsHorizontal(edge2);
					if (flag3)
					{
						ip.Y = edge2.Bot.Y;
					}
					else
					{
						double num = (double)edge2.Bot.Y - (double)edge2.Bot.X / edge2.Dx;
						ip.Y = Clipper.Round((double)ip.X / edge2.Dx + num);
					}
				}
				else
				{
					bool flag4 = edge2.Delta.X == 0L;
					if (flag4)
					{
						ip.X = edge2.Bot.X;
						bool flag5 = ClipperBase.IsHorizontal(edge1);
						if (flag5)
						{
							ip.Y = edge1.Bot.Y;
						}
						else
						{
							double num2 = (double)edge1.Bot.Y - (double)edge1.Bot.X / edge1.Dx;
							ip.Y = Clipper.Round((double)ip.X / edge1.Dx + num2);
						}
					}
					else
					{
						double num2 = (double)edge1.Bot.X - (double)edge1.Bot.Y * edge1.Dx;
						double num = (double)edge2.Bot.X - (double)edge2.Bot.Y * edge2.Dx;
						double num3 = (num - num2) / (edge1.Dx - edge2.Dx);
						ip.Y = Clipper.Round(num3);
						bool flag6 = Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx);
						if (flag6)
						{
							ip.X = Clipper.Round(edge1.Dx * num3 + num2);
						}
						else
						{
							ip.X = Clipper.Round(edge2.Dx * num3 + num);
						}
					}
				}
				bool flag7 = ip.Y < edge1.Top.Y || ip.Y < edge2.Top.Y;
				if (flag7)
				{
					bool flag8 = edge1.Top.Y > edge2.Top.Y;
					if (flag8)
					{
						ip.Y = edge1.Top.Y;
					}
					else
					{
						ip.Y = edge2.Top.Y;
					}
					bool flag9 = Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx);
					if (flag9)
					{
						ip.X = Clipper.TopX(edge1, ip.Y);
					}
					else
					{
						ip.X = Clipper.TopX(edge2, ip.Y);
					}
				}
				bool flag10 = ip.Y > edge1.Curr.Y;
				if (flag10)
				{
					ip.Y = edge1.Curr.Y;
					bool flag11 = Math.Abs(edge1.Dx) > Math.Abs(edge2.Dx);
					if (flag11)
					{
						ip.X = Clipper.TopX(edge2, ip.Y);
					}
					else
					{
						ip.X = Clipper.TopX(edge1, ip.Y);
					}
				}
			}
		}

		private void ProcessEdgesAtTopOfScanbeam(long topY)
		{
			TEdge tedge = this.m_ActiveEdges;
			while (tedge != null)
			{
				bool flag = this.IsMaxima(tedge, (double)topY);
				bool flag2 = flag;
				if (flag2)
				{
					TEdge maximaPairEx = this.GetMaximaPairEx(tedge);
					flag = maximaPairEx == null || !ClipperBase.IsHorizontal(maximaPairEx);
				}
				bool flag3 = flag;
				if (flag3)
				{
					bool strictlySimple = this.StrictlySimple;
					if (strictlySimple)
					{
						this.InsertMaxima(tedge.Top.X);
					}
					TEdge prevInAEL = tedge.PrevInAEL;
					this.DoMaxima(tedge);
					bool flag4 = prevInAEL == null;
					if (flag4)
					{
						tedge = this.m_ActiveEdges;
					}
					else
					{
						tedge = prevInAEL.NextInAEL;
					}
				}
				else
				{
					bool flag5 = this.IsIntermediate(tedge, (double)topY) && ClipperBase.IsHorizontal(tedge.NextInLML);
					if (flag5)
					{
						base.UpdateEdgeIntoAEL(ref tedge);
						bool flag6 = tedge.OutIdx >= 0;
						if (flag6)
						{
							this.AddOutPt(tedge, tedge.Bot);
						}
						this.AddEdgeToSEL(tedge);
					}
					else
					{
						tedge.Curr.X = Clipper.TopX(tedge, topY);
						tedge.Curr.Y = topY;
					}
					bool strictlySimple2 = this.StrictlySimple;
					if (strictlySimple2)
					{
						TEdge prevInAEL2 = tedge.PrevInAEL;
						bool flag7 = tedge.OutIdx >= 0 && tedge.WindDelta != 0 && prevInAEL2 != null && prevInAEL2.OutIdx >= 0 && prevInAEL2.Curr.X == tedge.Curr.X && prevInAEL2.WindDelta != 0;
						if (flag7)
						{
							IntPoint intPoint = new IntPoint(tedge.Curr);
							OutPt outPt = this.AddOutPt(prevInAEL2, intPoint);
							OutPt outPt2 = this.AddOutPt(tedge, intPoint);
							this.AddJoin(outPt, outPt2, intPoint);
						}
					}
					tedge = tedge.NextInAEL;
				}
			}
			this.ProcessHorizontals();
			this.m_Maxima = null;
			for (tedge = this.m_ActiveEdges; tedge != null; tedge = tedge.NextInAEL)
			{
				bool flag8 = this.IsIntermediate(tedge, (double)topY);
				if (flag8)
				{
					OutPt outPt3 = null;
					bool flag9 = tedge.OutIdx >= 0;
					if (flag9)
					{
						outPt3 = this.AddOutPt(tedge, tedge.Top);
					}
					base.UpdateEdgeIntoAEL(ref tedge);
					TEdge prevInAEL3 = tedge.PrevInAEL;
					TEdge nextInAEL = tedge.NextInAEL;
					bool flag10 = prevInAEL3 != null && prevInAEL3.Curr.X == tedge.Bot.X && prevInAEL3.Curr.Y == tedge.Bot.Y && outPt3 != null && prevInAEL3.OutIdx >= 0 && prevInAEL3.Curr.Y > prevInAEL3.Top.Y && ClipperBase.SlopesEqual(tedge.Curr, tedge.Top, prevInAEL3.Curr, prevInAEL3.Top, this.m_UseFullRange) && tedge.WindDelta != 0 && prevInAEL3.WindDelta != 0;
					if (flag10)
					{
						OutPt outPt4 = this.AddOutPt(prevInAEL3, tedge.Bot);
						this.AddJoin(outPt3, outPt4, tedge.Top);
					}
					else
					{
						bool flag11 = nextInAEL != null && nextInAEL.Curr.X == tedge.Bot.X && nextInAEL.Curr.Y == tedge.Bot.Y && outPt3 != null && nextInAEL.OutIdx >= 0 && nextInAEL.Curr.Y > nextInAEL.Top.Y && ClipperBase.SlopesEqual(tedge.Curr, tedge.Top, nextInAEL.Curr, nextInAEL.Top, this.m_UseFullRange) && tedge.WindDelta != 0 && nextInAEL.WindDelta != 0;
						if (flag11)
						{
							OutPt outPt5 = this.AddOutPt(nextInAEL, tedge.Bot);
							this.AddJoin(outPt3, outPt5, tedge.Top);
						}
					}
				}
			}
		}

		private void DoMaxima(TEdge e)
		{
			TEdge maximaPairEx = this.GetMaximaPairEx(e);
			bool flag = maximaPairEx == null;
			if (flag)
			{
				bool flag2 = e.OutIdx >= 0;
				if (flag2)
				{
					this.AddOutPt(e, e.Top);
				}
				base.DeleteFromAEL(e);
			}
			else
			{
				TEdge tedge = e.NextInAEL;
				while (tedge != null && tedge != maximaPairEx)
				{
					this.IntersectEdges(e, tedge, e.Top);
					base.SwapPositionsInAEL(e, tedge);
					tedge = e.NextInAEL;
				}
				bool flag3 = e.OutIdx == -1 && maximaPairEx.OutIdx == -1;
				if (flag3)
				{
					base.DeleteFromAEL(e);
					base.DeleteFromAEL(maximaPairEx);
				}
				else
				{
					bool flag4 = e.OutIdx >= 0 && maximaPairEx.OutIdx >= 0;
					if (flag4)
					{
						bool flag5 = e.OutIdx >= 0;
						if (flag5)
						{
							this.AddLocalMaxPoly(e, maximaPairEx, e.Top);
						}
						base.DeleteFromAEL(e);
						base.DeleteFromAEL(maximaPairEx);
					}
					else
					{
						bool flag6 = e.WindDelta == 0;
						if (!flag6)
						{
							throw new ClipperException("DoMaxima error");
						}
						bool flag7 = e.OutIdx >= 0;
						if (flag7)
						{
							this.AddOutPt(e, e.Top);
							e.OutIdx = -1;
						}
						base.DeleteFromAEL(e);
						bool flag8 = maximaPairEx.OutIdx >= 0;
						if (flag8)
						{
							this.AddOutPt(maximaPairEx, e.Top);
							maximaPairEx.OutIdx = -1;
						}
						base.DeleteFromAEL(maximaPairEx);
					}
				}
			}
		}

		public static void ReversePaths(List<List<IntPoint>> polys)
		{
			foreach (List<IntPoint> list in polys)
			{
				list.Reverse();
			}
		}

		public static bool Orientation(List<IntPoint> poly)
		{
			return Clipper.Area(poly) >= 0.0;
		}

		private int PointCount(OutPt pts)
		{
			bool flag = pts == null;
			int num;
			if (flag)
			{
				num = 0;
			}
			else
			{
				int num2 = 0;
				OutPt outPt = pts;
				do
				{
					num2++;
					outPt = outPt.Next;
				}
				while (outPt != pts);
				num = num2;
			}
			return num;
		}

		private void BuildResult(List<List<IntPoint>> polyg)
		{
			polyg.Clear();
			polyg.Capacity = this.m_PolyOuts.Count;
			for (int i = 0; i < this.m_PolyOuts.Count; i++)
			{
				OutRec outRec = this.m_PolyOuts[i];
				bool flag = outRec.Pts == null;
				if (!flag)
				{
					OutPt outPt = outRec.Pts.Prev;
					int num = this.PointCount(outPt);
					bool flag2 = num < 2;
					if (!flag2)
					{
						List<IntPoint> list = new List<IntPoint>(num);
						for (int j = 0; j < num; j++)
						{
							list.Add(outPt.Pt);
							outPt = outPt.Prev;
						}
						polyg.Add(list);
					}
				}
			}
		}

		private void BuildResult2(PolyTree polytree)
		{
			polytree.Clear();
			polytree.m_AllPolys.Capacity = this.m_PolyOuts.Count;
			for (int i = 0; i < this.m_PolyOuts.Count; i++)
			{
				OutRec outRec = this.m_PolyOuts[i];
				int num = this.PointCount(outRec.Pts);
				bool flag = (outRec.IsOpen && num < 2) || (!outRec.IsOpen && num < 3);
				if (!flag)
				{
					this.FixHoleLinkage(outRec);
					PolyNode polyNode = new PolyNode();
					polytree.m_AllPolys.Add(polyNode);
					outRec.PolyNode = polyNode;
					polyNode.m_polygon.Capacity = num;
					OutPt outPt = outRec.Pts.Prev;
					for (int j = 0; j < num; j++)
					{
						polyNode.m_polygon.Add(outPt.Pt);
						outPt = outPt.Prev;
					}
				}
			}
			polytree.m_Childs.Capacity = this.m_PolyOuts.Count;
			for (int k = 0; k < this.m_PolyOuts.Count; k++)
			{
				OutRec outRec2 = this.m_PolyOuts[k];
				bool flag2 = outRec2.PolyNode == null;
				if (!flag2)
				{
					bool isOpen = outRec2.IsOpen;
					if (isOpen)
					{
						outRec2.PolyNode.IsOpen = true;
						polytree.AddChild(outRec2.PolyNode);
					}
					else
					{
						bool flag3 = outRec2.FirstLeft != null && outRec2.FirstLeft.PolyNode != null;
						if (flag3)
						{
							outRec2.FirstLeft.PolyNode.AddChild(outRec2.PolyNode);
						}
						else
						{
							polytree.AddChild(outRec2.PolyNode);
						}
					}
				}
			}
		}

		private void FixupOutPolyline(OutRec outrec)
		{
			OutPt outPt = outrec.Pts;
			OutPt outPt2 = outPt.Prev;
			while (outPt != outPt2)
			{
				outPt = outPt.Next;
				bool flag = outPt.Pt == outPt.Prev.Pt;
				if (flag)
				{
					bool flag2 = outPt == outPt2;
					if (flag2)
					{
						outPt2 = outPt.Prev;
					}
					OutPt prev = outPt.Prev;
					prev.Next = outPt.Next;
					outPt.Next.Prev = prev;
					outPt = prev;
				}
			}
			bool flag3 = outPt == outPt.Prev;
			if (flag3)
			{
				outrec.Pts = null;
			}
		}

		private void FixupOutPolygon(OutRec outRec)
		{
			OutPt outPt = null;
			outRec.BottomPt = null;
			OutPt outPt2 = outRec.Pts;
			bool flag = base.PreserveCollinear || this.StrictlySimple;
			for (;;)
			{
				bool flag2 = outPt2.Prev == outPt2 || outPt2.Prev == outPt2.Next;
				if (flag2)
				{
					break;
				}
				bool flag3 = outPt2.Pt == outPt2.Next.Pt || outPt2.Pt == outPt2.Prev.Pt || (ClipperBase.SlopesEqual(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt, this.m_UseFullRange) && (!flag || !base.Pt2IsBetweenPt1AndPt3(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt)));
				if (flag3)
				{
					outPt = null;
					outPt2.Prev.Next = outPt2.Next;
					outPt2.Next.Prev = outPt2.Prev;
					outPt2 = outPt2.Prev;
				}
				else
				{
					bool flag4 = outPt2 == outPt;
					if (flag4)
					{
						goto Block_9;
					}
					bool flag5 = outPt == null;
					if (flag5)
					{
						outPt = outPt2;
					}
					outPt2 = outPt2.Next;
				}
			}
			outRec.Pts = null;
			return;
			Block_9:
			outRec.Pts = outPt2;
		}

		private OutPt DupOutPt(OutPt outPt, bool InsertAfter)
		{
			OutPt outPt2 = new OutPt();
			outPt2.Pt = outPt.Pt;
			outPt2.Idx = outPt.Idx;
			if (InsertAfter)
			{
				outPt2.Next = outPt.Next;
				outPt2.Prev = outPt;
				outPt.Next.Prev = outPt2;
				outPt.Next = outPt2;
			}
			else
			{
				outPt2.Prev = outPt.Prev;
				outPt2.Next = outPt;
				outPt.Prev.Next = outPt2;
				outPt.Prev = outPt2;
			}
			return outPt2;
		}

		private bool GetOverlap(long a1, long a2, long b1, long b2, out long Left, out long Right)
		{
			bool flag = a1 < a2;
			if (flag)
			{
				bool flag2 = b1 < b2;
				if (flag2)
				{
					Left = Math.Max(a1, b1);
					Right = Math.Min(a2, b2);
				}
				else
				{
					Left = Math.Max(a1, b2);
					Right = Math.Min(a2, b1);
				}
			}
			else
			{
				bool flag3 = b1 < b2;
				if (flag3)
				{
					Left = Math.Max(a2, b1);
					Right = Math.Min(a1, b2);
				}
				else
				{
					Left = Math.Max(a2, b2);
					Right = Math.Min(a1, b1);
				}
			}
			return Left < Right;
		}

		private bool JoinHorz(OutPt op1, OutPt op1b, OutPt op2, OutPt op2b, IntPoint Pt, bool DiscardLeft)
		{
			Direction direction = ((op1.Pt.X > op1b.Pt.X) ? Direction.dRightToLeft : Direction.dLeftToRight);
			Direction direction2 = ((op2.Pt.X > op2b.Pt.X) ? Direction.dRightToLeft : Direction.dLeftToRight);
			bool flag = direction == direction2;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = direction == Direction.dLeftToRight;
				if (flag3)
				{
					while (op1.Next.Pt.X <= Pt.X && op1.Next.Pt.X >= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
					{
						op1 = op1.Next;
					}
					bool flag4 = DiscardLeft && op1.Pt.X != Pt.X;
					if (flag4)
					{
						op1 = op1.Next;
					}
					op1b = this.DupOutPt(op1, !DiscardLeft);
					bool flag5 = op1b.Pt != Pt;
					if (flag5)
					{
						op1 = op1b;
						op1.Pt = Pt;
						op1b = this.DupOutPt(op1, !DiscardLeft);
					}
				}
				else
				{
					while (op1.Next.Pt.X >= Pt.X && op1.Next.Pt.X <= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
					{
						op1 = op1.Next;
					}
					bool flag6 = !DiscardLeft && op1.Pt.X != Pt.X;
					if (flag6)
					{
						op1 = op1.Next;
					}
					op1b = this.DupOutPt(op1, DiscardLeft);
					bool flag7 = op1b.Pt != Pt;
					if (flag7)
					{
						op1 = op1b;
						op1.Pt = Pt;
						op1b = this.DupOutPt(op1, DiscardLeft);
					}
				}
				bool flag8 = direction2 == Direction.dLeftToRight;
				if (flag8)
				{
					while (op2.Next.Pt.X <= Pt.X && op2.Next.Pt.X >= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
					{
						op2 = op2.Next;
					}
					bool flag9 = DiscardLeft && op2.Pt.X != Pt.X;
					if (flag9)
					{
						op2 = op2.Next;
					}
					op2b = this.DupOutPt(op2, !DiscardLeft);
					bool flag10 = op2b.Pt != Pt;
					if (flag10)
					{
						op2 = op2b;
						op2.Pt = Pt;
						op2b = this.DupOutPt(op2, !DiscardLeft);
					}
				}
				else
				{
					while (op2.Next.Pt.X >= Pt.X && op2.Next.Pt.X <= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
					{
						op2 = op2.Next;
					}
					bool flag11 = !DiscardLeft && op2.Pt.X != Pt.X;
					if (flag11)
					{
						op2 = op2.Next;
					}
					op2b = this.DupOutPt(op2, DiscardLeft);
					bool flag12 = op2b.Pt != Pt;
					if (flag12)
					{
						op2 = op2b;
						op2.Pt = Pt;
						op2b = this.DupOutPt(op2, DiscardLeft);
					}
				}
				bool flag13 = direction == Direction.dLeftToRight == DiscardLeft;
				if (flag13)
				{
					op1.Prev = op2;
					op2.Next = op1;
					op1b.Next = op2b;
					op2b.Prev = op1b;
				}
				else
				{
					op1.Next = op2;
					op2.Prev = op1;
					op1b.Prev = op2b;
					op2b.Next = op1b;
				}
				flag2 = true;
			}
			return flag2;
		}

		private bool JoinPoints(Join j, OutRec outRec1, OutRec outRec2)
		{
			OutPt outPt = j.OutPt1;
			OutPt outPt2 = j.OutPt2;
			bool flag = j.OutPt1.Pt.Y == j.OffPt.Y;
			bool flag2 = flag && j.OffPt == j.OutPt1.Pt && j.OffPt == j.OutPt2.Pt;
			bool flag4;
			if (flag2)
			{
				bool flag3 = outRec1 != outRec2;
				if (flag3)
				{
					flag4 = false;
				}
				else
				{
					OutPt outPt3 = j.OutPt1.Next;
					while (outPt3 != outPt && outPt3.Pt == j.OffPt)
					{
						outPt3 = outPt3.Next;
					}
					bool flag5 = outPt3.Pt.Y > j.OffPt.Y;
					OutPt outPt4 = j.OutPt2.Next;
					while (outPt4 != outPt2 && outPt4.Pt == j.OffPt)
					{
						outPt4 = outPt4.Next;
					}
					bool flag6 = outPt4.Pt.Y > j.OffPt.Y;
					bool flag7 = flag5 == flag6;
					if (flag7)
					{
						flag4 = false;
					}
					else
					{
						bool flag8 = flag5;
						if (flag8)
						{
							outPt3 = this.DupOutPt(outPt, false);
							outPt4 = this.DupOutPt(outPt2, true);
							outPt.Prev = outPt2;
							outPt2.Next = outPt;
							outPt3.Next = outPt4;
							outPt4.Prev = outPt3;
							j.OutPt1 = outPt;
							j.OutPt2 = outPt3;
							flag4 = true;
						}
						else
						{
							outPt3 = this.DupOutPt(outPt, true);
							outPt4 = this.DupOutPt(outPt2, false);
							outPt.Next = outPt2;
							outPt2.Prev = outPt;
							outPt3.Prev = outPt4;
							outPt4.Next = outPt3;
							j.OutPt1 = outPt;
							j.OutPt2 = outPt3;
							flag4 = true;
						}
					}
				}
			}
			else
			{
				bool flag9 = flag;
				if (flag9)
				{
					OutPt outPt3 = outPt;
					while (outPt.Prev.Pt.Y == outPt.Pt.Y && outPt.Prev != outPt3 && outPt.Prev != outPt2)
					{
						outPt = outPt.Prev;
					}
					while (outPt3.Next.Pt.Y == outPt3.Pt.Y && outPt3.Next != outPt && outPt3.Next != outPt2)
					{
						outPt3 = outPt3.Next;
					}
					bool flag10 = outPt3.Next == outPt || outPt3.Next == outPt2;
					if (flag10)
					{
						flag4 = false;
					}
					else
					{
						OutPt outPt4 = outPt2;
						while (outPt2.Prev.Pt.Y == outPt2.Pt.Y && outPt2.Prev != outPt4 && outPt2.Prev != outPt3)
						{
							outPt2 = outPt2.Prev;
						}
						while (outPt4.Next.Pt.Y == outPt4.Pt.Y && outPt4.Next != outPt2 && outPt4.Next != outPt)
						{
							outPt4 = outPt4.Next;
						}
						bool flag11 = outPt4.Next == outPt2 || outPt4.Next == outPt;
						if (flag11)
						{
							flag4 = false;
						}
						else
						{
							long num;
							long num2;
							bool flag12 = !this.GetOverlap(outPt.Pt.X, outPt3.Pt.X, outPt2.Pt.X, outPt4.Pt.X, out num, out num2);
							if (flag12)
							{
								flag4 = false;
							}
							else
							{
								bool flag13 = outPt.Pt.X >= num && outPt.Pt.X <= num2;
								IntPoint intPoint;
								bool flag14;
								if (flag13)
								{
									intPoint = outPt.Pt;
									flag14 = outPt.Pt.X > outPt3.Pt.X;
								}
								else
								{
									bool flag15 = outPt2.Pt.X >= num && outPt2.Pt.X <= num2;
									if (flag15)
									{
										intPoint = outPt2.Pt;
										flag14 = outPt2.Pt.X > outPt4.Pt.X;
									}
									else
									{
										bool flag16 = outPt3.Pt.X >= num && outPt3.Pt.X <= num2;
										if (flag16)
										{
											intPoint = outPt3.Pt;
											flag14 = outPt3.Pt.X > outPt.Pt.X;
										}
										else
										{
											intPoint = outPt4.Pt;
											flag14 = outPt4.Pt.X > outPt2.Pt.X;
										}
									}
								}
								j.OutPt1 = outPt;
								j.OutPt2 = outPt2;
								flag4 = this.JoinHorz(outPt, outPt3, outPt2, outPt4, intPoint, flag14);
							}
						}
					}
				}
				else
				{
					OutPt outPt3 = outPt.Next;
					while (outPt3.Pt == outPt.Pt && outPt3 != outPt)
					{
						outPt3 = outPt3.Next;
					}
					bool flag17 = outPt3.Pt.Y > outPt.Pt.Y || !ClipperBase.SlopesEqual(outPt.Pt, outPt3.Pt, j.OffPt, this.m_UseFullRange);
					bool flag18 = flag17;
					if (flag18)
					{
						outPt3 = outPt.Prev;
						while (outPt3.Pt == outPt.Pt && outPt3 != outPt)
						{
							outPt3 = outPt3.Prev;
						}
						bool flag19 = outPt3.Pt.Y > outPt.Pt.Y || !ClipperBase.SlopesEqual(outPt.Pt, outPt3.Pt, j.OffPt, this.m_UseFullRange);
						if (flag19)
						{
							return false;
						}
					}
					OutPt outPt4 = outPt2.Next;
					while (outPt4.Pt == outPt2.Pt && outPt4 != outPt2)
					{
						outPt4 = outPt4.Next;
					}
					bool flag20 = outPt4.Pt.Y > outPt2.Pt.Y || !ClipperBase.SlopesEqual(outPt2.Pt, outPt4.Pt, j.OffPt, this.m_UseFullRange);
					bool flag21 = flag20;
					if (flag21)
					{
						outPt4 = outPt2.Prev;
						while (outPt4.Pt == outPt2.Pt && outPt4 != outPt2)
						{
							outPt4 = outPt4.Prev;
						}
						bool flag22 = outPt4.Pt.Y > outPt2.Pt.Y || !ClipperBase.SlopesEqual(outPt2.Pt, outPt4.Pt, j.OffPt, this.m_UseFullRange);
						if (flag22)
						{
							return false;
						}
					}
					bool flag23 = outPt3 == outPt || outPt4 == outPt2 || outPt3 == outPt4 || (outRec1 == outRec2 && flag17 == flag20);
					if (flag23)
					{
						flag4 = false;
					}
					else
					{
						bool flag24 = flag17;
						if (flag24)
						{
							outPt3 = this.DupOutPt(outPt, false);
							outPt4 = this.DupOutPt(outPt2, true);
							outPt.Prev = outPt2;
							outPt2.Next = outPt;
							outPt3.Next = outPt4;
							outPt4.Prev = outPt3;
							j.OutPt1 = outPt;
							j.OutPt2 = outPt3;
							flag4 = true;
						}
						else
						{
							outPt3 = this.DupOutPt(outPt, true);
							outPt4 = this.DupOutPt(outPt2, false);
							outPt.Next = outPt2;
							outPt2.Prev = outPt;
							outPt3.Prev = outPt4;
							outPt4.Next = outPt3;
							j.OutPt1 = outPt;
							j.OutPt2 = outPt3;
							flag4 = true;
						}
					}
				}
			}
			return flag4;
		}

		public static int PointInPolygon(IntPoint pt, List<IntPoint> path)
		{
			int num = 0;
			int count = path.Count;
			bool flag = count < 3;
			int num2;
			if (flag)
			{
				num2 = 0;
			}
			else
			{
				IntPoint intPoint = path[0];
				for (int i = 1; i <= count; i++)
				{
					IntPoint intPoint2 = ((i == count) ? path[0] : path[i]);
					bool flag2 = intPoint2.Y == pt.Y;
					if (flag2)
					{
						bool flag3 = intPoint2.X == pt.X || (intPoint.Y == pt.Y && intPoint2.X > pt.X == intPoint.X < pt.X);
						if (flag3)
						{
							return -1;
						}
					}
					bool flag4 = intPoint.Y < pt.Y != intPoint2.Y < pt.Y;
					if (flag4)
					{
						bool flag5 = intPoint.X >= pt.X;
						if (flag5)
						{
							bool flag6 = intPoint2.X > pt.X;
							if (flag6)
							{
								num = 1 - num;
							}
							else
							{
								double num3 = (double)(intPoint.X - pt.X) * (double)(intPoint2.Y - pt.Y) - (double)(intPoint2.X - pt.X) * (double)(intPoint.Y - pt.Y);
								bool flag7 = num3 == 0.0;
								if (flag7)
								{
									return -1;
								}
								bool flag8 = num3 > 0.0 == intPoint2.Y > intPoint.Y;
								if (flag8)
								{
									num = 1 - num;
								}
							}
						}
						else
						{
							bool flag9 = intPoint2.X > pt.X;
							if (flag9)
							{
								double num4 = (double)(intPoint.X - pt.X) * (double)(intPoint2.Y - pt.Y) - (double)(intPoint2.X - pt.X) * (double)(intPoint.Y - pt.Y);
								bool flag10 = num4 == 0.0;
								if (flag10)
								{
									return -1;
								}
								bool flag11 = num4 > 0.0 == intPoint2.Y > intPoint.Y;
								if (flag11)
								{
									num = 1 - num;
								}
							}
						}
					}
					intPoint = intPoint2;
				}
				num2 = num;
			}
			return num2;
		}

		private static int PointInPolygon(IntPoint pt, OutPt op)
		{
			int num = 0;
			OutPt outPt = op;
			long x = pt.X;
			long y = pt.Y;
			long num2 = op.Pt.X;
			long num3 = op.Pt.Y;
			for (;;)
			{
				op = op.Next;
				long x2 = op.Pt.X;
				long y2 = op.Pt.Y;
				bool flag = y2 == y;
				if (flag)
				{
					bool flag2 = x2 == x || (num3 == y && x2 > x == num2 < x);
					if (flag2)
					{
						break;
					}
				}
				bool flag3 = num3 < y != y2 < y;
				if (flag3)
				{
					bool flag4 = num2 >= x;
					if (flag4)
					{
						bool flag5 = x2 > x;
						if (flag5)
						{
							num = 1 - num;
						}
						else
						{
							double num4 = (double)(num2 - x) * (double)(y2 - y) - (double)(x2 - x) * (double)(num3 - y);
							bool flag6 = num4 == 0.0;
							if (flag6)
							{
								goto Block_8;
							}
							bool flag7 = num4 > 0.0 == y2 > num3;
							if (flag7)
							{
								num = 1 - num;
							}
						}
					}
					else
					{
						bool flag8 = x2 > x;
						if (flag8)
						{
							double num5 = (double)(num2 - x) * (double)(y2 - y) - (double)(x2 - x) * (double)(num3 - y);
							bool flag9 = num5 == 0.0;
							if (flag9)
							{
								goto Block_11;
							}
							bool flag10 = num5 > 0.0 == y2 > num3;
							if (flag10)
							{
								num = 1 - num;
							}
						}
					}
				}
				num2 = x2;
				num3 = y2;
				if (outPt == op)
				{
					goto Block_13;
				}
			}
			return -1;
			Block_8:
			return -1;
			Block_11:
			return -1;
			Block_13:
			return num;
		}

		private static bool Poly2ContainsPoly1(OutPt outPt1, OutPt outPt2)
		{
			OutPt outPt3 = outPt1;
			int num;
			for (;;)
			{
				num = Clipper.PointInPolygon(outPt3.Pt, outPt2);
				bool flag = num >= 0;
				if (flag)
				{
					break;
				}
				outPt3 = outPt3.Next;
				if (outPt3 == outPt1)
				{
					goto Block_2;
				}
			}
			return num > 0;
			Block_2:
			return true;
		}

		private void FixupFirstLefts1(OutRec OldOutRec, OutRec NewOutRec)
		{
			foreach (OutRec outRec in this.m_PolyOuts)
			{
				OutRec outRec2 = Clipper.ParseFirstLeft(outRec.FirstLeft);
				bool flag = outRec.Pts != null && outRec2 == OldOutRec;
				if (flag)
				{
					bool flag2 = Clipper.Poly2ContainsPoly1(outRec.Pts, NewOutRec.Pts);
					if (flag2)
					{
						outRec.FirstLeft = NewOutRec;
					}
				}
			}
		}

		private void FixupFirstLefts2(OutRec innerOutRec, OutRec outerOutRec)
		{
			OutRec firstLeft = outerOutRec.FirstLeft;
			foreach (OutRec outRec in this.m_PolyOuts)
			{
				bool flag = outRec.Pts == null || outRec == outerOutRec || outRec == innerOutRec;
				if (!flag)
				{
					OutRec outRec2 = Clipper.ParseFirstLeft(outRec.FirstLeft);
					bool flag2 = outRec2 != firstLeft && outRec2 != innerOutRec && outRec2 != outerOutRec;
					if (!flag2)
					{
						bool flag3 = Clipper.Poly2ContainsPoly1(outRec.Pts, innerOutRec.Pts);
						if (flag3)
						{
							outRec.FirstLeft = innerOutRec;
						}
						else
						{
							bool flag4 = Clipper.Poly2ContainsPoly1(outRec.Pts, outerOutRec.Pts);
							if (flag4)
							{
								outRec.FirstLeft = outerOutRec;
							}
							else
							{
								bool flag5 = outRec.FirstLeft == innerOutRec || outRec.FirstLeft == outerOutRec;
								if (flag5)
								{
									outRec.FirstLeft = firstLeft;
								}
							}
						}
					}
				}
			}
		}

		private void FixupFirstLefts3(OutRec OldOutRec, OutRec NewOutRec)
		{
			foreach (OutRec outRec in this.m_PolyOuts)
			{
				OutRec outRec2 = Clipper.ParseFirstLeft(outRec.FirstLeft);
				bool flag = outRec.Pts != null && outRec2 == OldOutRec;
				if (flag)
				{
					outRec.FirstLeft = NewOutRec;
				}
			}
		}

		private static OutRec ParseFirstLeft(OutRec FirstLeft)
		{
			while (FirstLeft != null && FirstLeft.Pts == null)
			{
				FirstLeft = FirstLeft.FirstLeft;
			}
			return FirstLeft;
		}

		private void JoinCommonEdges()
		{
			for (int i = 0; i < this.m_Joins.Count; i++)
			{
				Join join = this.m_Joins[i];
				OutRec outRec = this.GetOutRec(join.OutPt1.Idx);
				OutRec outRec2 = this.GetOutRec(join.OutPt2.Idx);
				bool flag = outRec.Pts == null || outRec2.Pts == null;
				if (!flag)
				{
					bool flag2 = outRec.IsOpen || outRec2.IsOpen;
					if (!flag2)
					{
						bool flag3 = outRec == outRec2;
						OutRec outRec3;
						if (flag3)
						{
							outRec3 = outRec;
						}
						else
						{
							bool flag4 = this.OutRec1RightOfOutRec2(outRec, outRec2);
							if (flag4)
							{
								outRec3 = outRec2;
							}
							else
							{
								bool flag5 = this.OutRec1RightOfOutRec2(outRec2, outRec);
								if (flag5)
								{
									outRec3 = outRec;
								}
								else
								{
									outRec3 = this.GetLowermostRec(outRec, outRec2);
								}
							}
						}
						bool flag6 = !this.JoinPoints(join, outRec, outRec2);
						if (!flag6)
						{
							bool flag7 = outRec == outRec2;
							if (flag7)
							{
								outRec.Pts = join.OutPt1;
								outRec.BottomPt = null;
								outRec2 = base.CreateOutRec();
								outRec2.Pts = join.OutPt2;
								this.UpdateOutPtIdxs(outRec2);
								bool flag8 = Clipper.Poly2ContainsPoly1(outRec2.Pts, outRec.Pts);
								if (flag8)
								{
									outRec2.IsHole = !outRec.IsHole;
									outRec2.FirstLeft = outRec;
									bool usingPolyTree = this.m_UsingPolyTree;
									if (usingPolyTree)
									{
										this.FixupFirstLefts2(outRec2, outRec);
									}
									bool flag9 = (outRec2.IsHole ^ this.ReverseSolution) == this.Area(outRec2) > 0.0;
									if (flag9)
									{
										this.ReversePolyPtLinks(outRec2.Pts);
									}
								}
								else
								{
									bool flag10 = Clipper.Poly2ContainsPoly1(outRec.Pts, outRec2.Pts);
									if (flag10)
									{
										outRec2.IsHole = outRec.IsHole;
										outRec.IsHole = !outRec2.IsHole;
										outRec2.FirstLeft = outRec.FirstLeft;
										outRec.FirstLeft = outRec2;
										bool usingPolyTree2 = this.m_UsingPolyTree;
										if (usingPolyTree2)
										{
											this.FixupFirstLefts2(outRec, outRec2);
										}
										bool flag11 = (outRec.IsHole ^ this.ReverseSolution) == this.Area(outRec) > 0.0;
										if (flag11)
										{
											this.ReversePolyPtLinks(outRec.Pts);
										}
									}
									else
									{
										outRec2.IsHole = outRec.IsHole;
										outRec2.FirstLeft = outRec.FirstLeft;
										bool usingPolyTree3 = this.m_UsingPolyTree;
										if (usingPolyTree3)
										{
											this.FixupFirstLefts1(outRec, outRec2);
										}
									}
								}
							}
							else
							{
								outRec2.Pts = null;
								outRec2.BottomPt = null;
								outRec2.Idx = outRec.Idx;
								outRec.IsHole = outRec3.IsHole;
								bool flag12 = outRec3 == outRec2;
								if (flag12)
								{
									outRec.FirstLeft = outRec2.FirstLeft;
								}
								outRec2.FirstLeft = outRec;
								bool usingPolyTree4 = this.m_UsingPolyTree;
								if (usingPolyTree4)
								{
									this.FixupFirstLefts3(outRec2, outRec);
								}
							}
						}
					}
				}
			}
		}

		private void UpdateOutPtIdxs(OutRec outrec)
		{
			OutPt outPt = outrec.Pts;
			do
			{
				outPt.Idx = outrec.Idx;
				outPt = outPt.Prev;
			}
			while (outPt != outrec.Pts);
		}

		private void DoSimplePolygons()
		{
			int i = 0;
			while (i < this.m_PolyOuts.Count)
			{
				OutRec outRec = this.m_PolyOuts[i++];
				OutPt outPt = outRec.Pts;
				bool flag = outPt == null || outRec.IsOpen;
				if (!flag)
				{
					do
					{
						for (OutPt outPt2 = outPt.Next; outPt2 != outRec.Pts; outPt2 = outPt2.Next)
						{
							bool flag2 = outPt.Pt == outPt2.Pt && outPt2.Next != outPt && outPt2.Prev != outPt;
							if (flag2)
							{
								OutPt prev = outPt.Prev;
								OutPt prev2 = outPt2.Prev;
								outPt.Prev = prev2;
								prev2.Next = outPt;
								outPt2.Prev = prev;
								prev.Next = outPt2;
								outRec.Pts = outPt;
								OutRec outRec2 = base.CreateOutRec();
								outRec2.Pts = outPt2;
								this.UpdateOutPtIdxs(outRec2);
								bool flag3 = Clipper.Poly2ContainsPoly1(outRec2.Pts, outRec.Pts);
								if (flag3)
								{
									outRec2.IsHole = !outRec.IsHole;
									outRec2.FirstLeft = outRec;
									bool usingPolyTree = this.m_UsingPolyTree;
									if (usingPolyTree)
									{
										this.FixupFirstLefts2(outRec2, outRec);
									}
								}
								else
								{
									bool flag4 = Clipper.Poly2ContainsPoly1(outRec.Pts, outRec2.Pts);
									if (flag4)
									{
										outRec2.IsHole = outRec.IsHole;
										outRec.IsHole = !outRec2.IsHole;
										outRec2.FirstLeft = outRec.FirstLeft;
										outRec.FirstLeft = outRec2;
										bool usingPolyTree2 = this.m_UsingPolyTree;
										if (usingPolyTree2)
										{
											this.FixupFirstLefts2(outRec, outRec2);
										}
									}
									else
									{
										outRec2.IsHole = outRec.IsHole;
										outRec2.FirstLeft = outRec.FirstLeft;
										bool usingPolyTree3 = this.m_UsingPolyTree;
										if (usingPolyTree3)
										{
											this.FixupFirstLefts1(outRec, outRec2);
										}
									}
								}
								outPt2 = outPt;
							}
						}
						outPt = outPt.Next;
					}
					while (outPt != outRec.Pts);
				}
			}
		}

		public static double Area(List<IntPoint> poly)
		{
			int count = poly.Count;
			bool flag = count < 3;
			double num;
			if (flag)
			{
				num = 0.0;
			}
			else
			{
				double num2 = 0.0;
				int i = 0;
				int num3 = count - 1;
				while (i < count)
				{
					num2 += ((double)poly[num3].X + (double)poly[i].X) * ((double)poly[num3].Y - (double)poly[i].Y);
					num3 = i;
					i++;
				}
				num = -num2 * 0.5;
			}
			return num;
		}

		internal double Area(OutRec outRec)
		{
			return this.Area(outRec.Pts);
		}

		internal double Area(OutPt op)
		{
			OutPt outPt = op;
			bool flag = op == null;
			double num;
			if (flag)
			{
				num = 0.0;
			}
			else
			{
				double num2 = 0.0;
				do
				{
					num2 += (double)(op.Prev.Pt.X + op.Pt.X) * (double)(op.Prev.Pt.Y - op.Pt.Y);
					op = op.Next;
				}
				while (op != outPt);
				num = num2 * 0.5;
			}
			return num;
		}

		public static List<List<IntPoint>> SimplifyPolygon(List<IntPoint> poly, PolyFillType fillType = PolyFillType.pftEvenOdd)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			Clipper clipper = new Clipper(0);
			clipper.StrictlySimple = true;
			clipper.AddPath(poly, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, fillType, fillType);
			return list;
		}

		public static List<List<IntPoint>> SimplifyPolygons(List<List<IntPoint>> polys, PolyFillType fillType = PolyFillType.pftEvenOdd)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			Clipper clipper = new Clipper(0);
			clipper.StrictlySimple = true;
			clipper.AddPaths(polys, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, fillType, fillType);
			return list;
		}

		private static double DistanceSqrd(IntPoint pt1, IntPoint pt2)
		{
			double num = (double)pt1.X - (double)pt2.X;
			double num2 = (double)pt1.Y - (double)pt2.Y;
			return num * num + num2 * num2;
		}

		private static double DistanceFromLineSqrd(IntPoint pt, IntPoint ln1, IntPoint ln2)
		{
			double num = (double)(ln1.Y - ln2.Y);
			double num2 = (double)(ln2.X - ln1.X);
			double num3 = num * (double)ln1.X + num2 * (double)ln1.Y;
			num3 = num * (double)pt.X + num2 * (double)pt.Y - num3;
			return num3 * num3 / (num * num + num2 * num2);
		}

		private static bool SlopesNearCollinear(IntPoint pt1, IntPoint pt2, IntPoint pt3, double distSqrd)
		{
			bool flag = Math.Abs(pt1.X - pt2.X) > Math.Abs(pt1.Y - pt2.Y);
			bool flag3;
			if (flag)
			{
				bool flag2 = pt1.X > pt2.X == pt1.X < pt3.X;
				if (flag2)
				{
					flag3 = Clipper.DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
				}
				else
				{
					bool flag4 = pt2.X > pt1.X == pt2.X < pt3.X;
					if (flag4)
					{
						flag3 = Clipper.DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd;
					}
					else
					{
						flag3 = Clipper.DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
					}
				}
			}
			else
			{
				bool flag5 = pt1.Y > pt2.Y == pt1.Y < pt3.Y;
				if (flag5)
				{
					flag3 = Clipper.DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
				}
				else
				{
					bool flag6 = pt2.Y > pt1.Y == pt2.Y < pt3.Y;
					if (flag6)
					{
						flag3 = Clipper.DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd;
					}
					else
					{
						flag3 = Clipper.DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
					}
				}
			}
			return flag3;
		}

		private static bool PointsAreClose(IntPoint pt1, IntPoint pt2, double distSqrd)
		{
			double num = (double)pt1.X - (double)pt2.X;
			double num2 = (double)pt1.Y - (double)pt2.Y;
			return num * num + num2 * num2 <= distSqrd;
		}

		private static OutPt ExcludeOp(OutPt op)
		{
			OutPt prev = op.Prev;
			prev.Next = op.Next;
			op.Next.Prev = prev;
			prev.Idx = 0;
			return prev;
		}

		public static List<IntPoint> CleanPolygon(List<IntPoint> path, double distance = 1.415)
		{
			int num = path.Count;
			bool flag = num == 0;
			List<IntPoint> list;
			if (flag)
			{
				list = new List<IntPoint>();
			}
			else
			{
				OutPt[] array = new OutPt[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = new OutPt();
				}
				for (int j = 0; j < num; j++)
				{
					array[j].Pt = path[j];
					array[j].Next = array[(j + 1) % num];
					array[j].Next.Prev = array[j];
					array[j].Idx = 0;
				}
				double num2 = distance * distance;
				OutPt outPt = array[0];
				while (outPt.Idx == 0 && outPt.Next != outPt.Prev)
				{
					bool flag2 = Clipper.PointsAreClose(outPt.Pt, outPt.Prev.Pt, num2);
					if (flag2)
					{
						outPt = Clipper.ExcludeOp(outPt);
						num--;
					}
					else
					{
						bool flag3 = Clipper.PointsAreClose(outPt.Prev.Pt, outPt.Next.Pt, num2);
						if (flag3)
						{
							Clipper.ExcludeOp(outPt.Next);
							outPt = Clipper.ExcludeOp(outPt);
							num -= 2;
						}
						else
						{
							bool flag4 = Clipper.SlopesNearCollinear(outPt.Prev.Pt, outPt.Pt, outPt.Next.Pt, num2);
							if (flag4)
							{
								outPt = Clipper.ExcludeOp(outPt);
								num--;
							}
							else
							{
								outPt.Idx = 1;
								outPt = outPt.Next;
							}
						}
					}
				}
				bool flag5 = num < 3;
				if (flag5)
				{
					num = 0;
				}
				List<IntPoint> list2 = new List<IntPoint>(num);
				for (int k = 0; k < num; k++)
				{
					list2.Add(outPt.Pt);
					outPt = outPt.Next;
				}
				list = list2;
			}
			return list;
		}

		public static List<List<IntPoint>> CleanPolygons(List<List<IntPoint>> polys, double distance = 1.415)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>(polys.Count);
			for (int i = 0; i < polys.Count; i++)
			{
				list.Add(Clipper.CleanPolygon(polys[i], distance));
			}
			return list;
		}

		internal static List<List<IntPoint>> Minkowski(List<IntPoint> pattern, List<IntPoint> path, bool IsSum, bool IsClosed)
		{
			int num = (IsClosed ? 1 : 0);
			int count = pattern.Count;
			int count2 = path.Count;
			List<List<IntPoint>> list = new List<List<IntPoint>>(count2);
			if (IsSum)
			{
				for (int i = 0; i < count2; i++)
				{
					List<IntPoint> list2 = new List<IntPoint>(count);
					foreach (IntPoint intPoint in pattern)
					{
						list2.Add(new IntPoint(path[i].X + intPoint.X, path[i].Y + intPoint.Y));
					}
					list.Add(list2);
				}
			}
			else
			{
				for (int j = 0; j < count2; j++)
				{
					List<IntPoint> list3 = new List<IntPoint>(count);
					foreach (IntPoint intPoint2 in pattern)
					{
						list3.Add(new IntPoint(path[j].X - intPoint2.X, path[j].Y - intPoint2.Y));
					}
					list.Add(list3);
				}
			}
			List<List<IntPoint>> list4 = new List<List<IntPoint>>((count2 + num) * (count + 1));
			for (int k = 0; k < count2 - 1 + num; k++)
			{
				for (int l = 0; l < count; l++)
				{
					List<IntPoint> list5 = new List<IntPoint>(4);
					list5.Add(list[k % count2][l % count]);
					list5.Add(list[(k + 1) % count2][l % count]);
					list5.Add(list[(k + 1) % count2][(l + 1) % count]);
					list5.Add(list[k % count2][(l + 1) % count]);
					bool flag = !Clipper.Orientation(list5);
					if (flag)
					{
						list5.Reverse();
					}
					list4.Add(list5);
				}
			}
			return list4;
		}

		public static List<List<IntPoint>> MinkowskiSum(List<IntPoint> pattern, List<IntPoint> path, bool pathIsClosed)
		{
			List<List<IntPoint>> list = Clipper.Minkowski(pattern, path, true, pathIsClosed);
			Clipper clipper = new Clipper(0);
			clipper.AddPaths(list, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
			return list;
		}

		private static List<IntPoint> TranslatePath(List<IntPoint> path, IntPoint delta)
		{
			List<IntPoint> list = new List<IntPoint>(path.Count);
			for (int i = 0; i < path.Count; i++)
			{
				list.Add(new IntPoint(path[i].X + delta.X, path[i].Y + delta.Y));
			}
			return list;
		}

		public static List<List<IntPoint>> MinkowskiSum(List<IntPoint> pattern, List<List<IntPoint>> paths, bool pathIsClosed)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			Clipper clipper = new Clipper(0);
			for (int i = 0; i < paths.Count; i++)
			{
				List<List<IntPoint>> list2 = Clipper.Minkowski(pattern, paths[i], true, pathIsClosed);
				clipper.AddPaths(list2, PolyType.ptSubject, true);
				if (pathIsClosed)
				{
					List<IntPoint> list3 = Clipper.TranslatePath(paths[i], pattern[0]);
					clipper.AddPath(list3, PolyType.ptClip, true);
				}
			}
			clipper.Execute(ClipType.ctUnion, list, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
			return list;
		}

		public static List<List<IntPoint>> MinkowskiDiff(List<IntPoint> poly1, List<IntPoint> poly2)
		{
			List<List<IntPoint>> list = Clipper.Minkowski(poly1, poly2, false, true);
			Clipper clipper = new Clipper(0);
			clipper.AddPaths(list, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
			return list;
		}

		public static List<List<IntPoint>> PolyTreeToPaths(PolyTree polytree)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Capacity = polytree.Total;
			Clipper.AddPolyNodeToPaths(polytree, Clipper.NodeType.ntAny, list);
			return list;
		}

		internal static void AddPolyNodeToPaths(PolyNode polynode, Clipper.NodeType nt, List<List<IntPoint>> paths)
		{
			bool flag = true;
			if (nt != Clipper.NodeType.ntOpen)
			{
				if (nt == Clipper.NodeType.ntClosed)
				{
					flag = !polynode.IsOpen;
				}
				bool flag2 = polynode.m_polygon.Count > 0 && flag;
				if (flag2)
				{
					paths.Add(polynode.m_polygon);
				}
				foreach (PolyNode polyNode in polynode.Childs)
				{
					Clipper.AddPolyNodeToPaths(polyNode, nt, paths);
				}
			}
		}

		public static List<List<IntPoint>> OpenPathsFromPolyTree(PolyTree polytree)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Capacity = polytree.ChildCount;
			for (int i = 0; i < polytree.ChildCount; i++)
			{
				bool isOpen = polytree.Childs[i].IsOpen;
				if (isOpen)
				{
					list.Add(polytree.Childs[i].m_polygon);
				}
			}
			return list;
		}

		public static List<List<IntPoint>> ClosedPathsFromPolyTree(PolyTree polytree)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Capacity = polytree.Total;
			Clipper.AddPolyNodeToPaths(polytree, Clipper.NodeType.ntClosed, list);
			return list;
		}

		public const int ioReverseSolution = 1;

		public const int ioStrictlySimple = 2;

		public const int ioPreserveCollinear = 4;

		private ClipType m_ClipType;

		private Maxima m_Maxima;

		private TEdge m_SortedEdges;

		private List<IntersectNode> m_IntersectList;

		private IComparer<IntersectNode> m_IntersectNodeComparer;

		private bool m_ExecuteLocked;

		private PolyFillType m_ClipFillType;

		private PolyFillType m_SubjFillType;

		private List<Join> m_Joins;

		private List<Join> m_GhostJoins;

		private bool m_UsingPolyTree;

		internal enum NodeType
		{
			ntAny,
			ntOpen,
			ntClosed
		}
	}
}
