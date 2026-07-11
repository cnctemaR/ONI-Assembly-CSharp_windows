using System;
using System.Collections.Generic;

namespace ClipperLib
{
	public class Clipper : ClipperBase
	{
		public Clipper(int InitOptions = 0)
		{
			this.m_Scanbeam = null;
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

		private void DisposeScanbeamList()
		{
			while (this.m_Scanbeam != null)
			{
				Scanbeam next = this.m_Scanbeam.Next;
				this.m_Scanbeam = null;
				this.m_Scanbeam = next;
			}
		}

		protected override void Reset()
		{
			base.Reset();
			this.m_Scanbeam = null;
			this.m_ActiveEdges = null;
			this.m_SortedEdges = null;
			for (LocalMinima localMinima = this.m_MinimaList; localMinima != null; localMinima = localMinima.Next)
			{
				this.InsertScanbeam(localMinima.Y);
			}
		}

		public bool ReverseSolution { get; set; }

		public bool StrictlySimple { get; set; }

		private void InsertScanbeam(long Y)
		{
			if (this.m_Scanbeam == null)
			{
				this.m_Scanbeam = new Scanbeam();
				this.m_Scanbeam.Next = null;
				this.m_Scanbeam.Y = Y;
				return;
			}
			if (Y > this.m_Scanbeam.Y)
			{
				this.m_Scanbeam = new Scanbeam
				{
					Y = Y,
					Next = this.m_Scanbeam
				};
				return;
			}
			Scanbeam scanbeam = this.m_Scanbeam;
			while (scanbeam.Next != null && Y <= scanbeam.Next.Y)
			{
				scanbeam = scanbeam.Next;
			}
			if (Y == scanbeam.Y)
			{
				return;
			}
			scanbeam.Next = new Scanbeam
			{
				Y = Y,
				Next = scanbeam.Next
			};
		}

		public bool Execute(ClipType clipType, List<List<IntPoint>> solution, PolyFillType subjFillType, PolyFillType clipFillType)
		{
			if (this.m_ExecuteLocked)
			{
				return false;
			}
			if (this.m_HasOpenPaths)
			{
				throw new ClipperException("Error: PolyTree struct is need for open path clipping.");
			}
			this.m_ExecuteLocked = true;
			solution.Clear();
			this.m_SubjFillType = subjFillType;
			this.m_ClipFillType = clipFillType;
			this.m_ClipType = clipType;
			this.m_UsingPolyTree = false;
			bool flag;
			try
			{
				flag = this.ExecuteInternal();
				if (flag)
				{
					this.BuildResult(solution);
				}
			}
			finally
			{
				this.DisposeAllPolyPts();
				this.m_ExecuteLocked = false;
			}
			return flag;
		}

		public bool Execute(ClipType clipType, PolyTree polytree, PolyFillType subjFillType, PolyFillType clipFillType)
		{
			if (this.m_ExecuteLocked)
			{
				return false;
			}
			this.m_ExecuteLocked = true;
			this.m_SubjFillType = subjFillType;
			this.m_ClipFillType = clipFillType;
			this.m_ClipType = clipType;
			this.m_UsingPolyTree = true;
			bool flag;
			try
			{
				flag = this.ExecuteInternal();
				if (flag)
				{
					this.BuildResult2(polytree);
				}
			}
			finally
			{
				this.DisposeAllPolyPts();
				this.m_ExecuteLocked = false;
			}
			return flag;
		}

		public bool Execute(ClipType clipType, List<List<IntPoint>> solution)
		{
			return this.Execute(clipType, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
		}

		public bool Execute(ClipType clipType, PolyTree polytree)
		{
			return this.Execute(clipType, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
		}

		internal void FixHoleLinkage(OutRec outRec)
		{
			if (outRec.FirstLeft == null || (outRec.IsHole != outRec.FirstLeft.IsHole && outRec.FirstLeft.Pts != null))
			{
				return;
			}
			OutRec outRec2 = outRec.FirstLeft;
			while (outRec2 != null && (outRec2.IsHole == outRec.IsHole || outRec2.Pts == null))
			{
				outRec2 = outRec2.FirstLeft;
			}
			outRec.FirstLeft = outRec2;
		}

		private bool ExecuteInternal()
		{
			bool flag;
			try
			{
				this.Reset();
				if (this.m_CurrentLM == null)
				{
					flag = false;
				}
				else
				{
					long num = this.PopScanbeam();
					for (;;)
					{
						this.InsertLocalMinimaIntoAEL(num);
						this.m_GhostJoins.Clear();
						this.ProcessHorizontals(false);
						if (this.m_Scanbeam == null)
						{
							goto IL_006D;
						}
						long num2 = this.PopScanbeam();
						if (!this.ProcessIntersections(num2))
						{
							break;
						}
						this.ProcessEdgesAtTopOfScanbeam(num2);
						num = num2;
						if (this.m_Scanbeam == null && this.m_CurrentLM == null)
						{
							goto IL_006D;
						}
					}
					return false;
					IL_006D:
					for (int i = 0; i < this.m_PolyOuts.Count; i++)
					{
						OutRec outRec = this.m_PolyOuts[i];
						if (outRec.Pts != null && !outRec.IsOpen && (outRec.IsHole ^ this.ReverseSolution) == this.Area(outRec) > 0.0)
						{
							this.ReversePolyPtLinks(outRec.Pts);
						}
					}
					this.JoinCommonEdges();
					for (int j = 0; j < this.m_PolyOuts.Count; j++)
					{
						OutRec outRec2 = this.m_PolyOuts[j];
						if (outRec2.Pts != null && !outRec2.IsOpen)
						{
							this.FixupOutPolygon(outRec2);
						}
					}
					if (this.StrictlySimple)
					{
						this.DoSimplePolygons();
					}
					flag = true;
				}
			}
			finally
			{
				this.m_Joins.Clear();
				this.m_GhostJoins.Clear();
			}
			return flag;
		}

		private long PopScanbeam()
		{
			long y = this.m_Scanbeam.Y;
			this.m_Scanbeam = this.m_Scanbeam.Next;
			return y;
		}

		private void DisposeAllPolyPts()
		{
			for (int i = 0; i < this.m_PolyOuts.Count; i++)
			{
				this.DisposeOutRec(i);
			}
			this.m_PolyOuts.Clear();
		}

		private void DisposeOutRec(int index)
		{
			this.m_PolyOuts[index].Pts = null;
			this.m_PolyOuts[index] = null;
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
			while (this.m_CurrentLM != null && this.m_CurrentLM.Y == botY)
			{
				TEdge leftBound = this.m_CurrentLM.LeftBound;
				TEdge rightBound = this.m_CurrentLM.RightBound;
				base.PopLocalMinima();
				OutPt outPt = null;
				if (leftBound == null)
				{
					this.InsertEdgeIntoAEL(rightBound, null);
					this.SetWindingCount(rightBound);
					if (this.IsContributing(rightBound))
					{
						outPt = this.AddOutPt(rightBound, rightBound.Bot);
					}
				}
				else if (rightBound == null)
				{
					this.InsertEdgeIntoAEL(leftBound, null);
					this.SetWindingCount(leftBound);
					if (this.IsContributing(leftBound))
					{
						outPt = this.AddOutPt(leftBound, leftBound.Bot);
					}
					this.InsertScanbeam(leftBound.Top.Y);
				}
				else
				{
					this.InsertEdgeIntoAEL(leftBound, null);
					this.InsertEdgeIntoAEL(rightBound, leftBound);
					this.SetWindingCount(leftBound);
					rightBound.WindCnt = leftBound.WindCnt;
					rightBound.WindCnt2 = leftBound.WindCnt2;
					if (this.IsContributing(leftBound))
					{
						outPt = this.AddLocalMinPoly(leftBound, rightBound, leftBound.Bot);
					}
					this.InsertScanbeam(leftBound.Top.Y);
				}
				if (rightBound != null)
				{
					if (ClipperBase.IsHorizontal(rightBound))
					{
						this.AddEdgeToSEL(rightBound);
					}
					else
					{
						this.InsertScanbeam(rightBound.Top.Y);
					}
				}
				if (leftBound != null && rightBound != null)
				{
					if (outPt != null && ClipperBase.IsHorizontal(rightBound) && this.m_GhostJoins.Count > 0 && rightBound.WindDelta != 0)
					{
						for (int i = 0; i < this.m_GhostJoins.Count; i++)
						{
							Join join = this.m_GhostJoins[i];
							if (this.HorzSegmentsOverlap(join.OutPt1.Pt.X, join.OffPt.X, rightBound.Bot.X, rightBound.Top.X))
							{
								this.AddJoin(join.OutPt1, outPt, join.OffPt);
							}
						}
					}
					if (leftBound.OutIdx >= 0 && leftBound.PrevInAEL != null && leftBound.PrevInAEL.Curr.X == leftBound.Bot.X && leftBound.PrevInAEL.OutIdx >= 0 && ClipperBase.SlopesEqual(leftBound.PrevInAEL, leftBound, this.m_UseFullRange) && leftBound.WindDelta != 0 && leftBound.PrevInAEL.WindDelta != 0)
					{
						OutPt outPt2 = this.AddOutPt(leftBound.PrevInAEL, leftBound.Bot);
						this.AddJoin(outPt, outPt2, leftBound.Top);
					}
					if (leftBound.NextInAEL != rightBound)
					{
						if (rightBound.OutIdx >= 0 && rightBound.PrevInAEL.OutIdx >= 0 && ClipperBase.SlopesEqual(rightBound.PrevInAEL, rightBound, this.m_UseFullRange) && rightBound.WindDelta != 0 && rightBound.PrevInAEL.WindDelta != 0)
						{
							OutPt outPt3 = this.AddOutPt(rightBound.PrevInAEL, rightBound.Bot);
							this.AddJoin(outPt, outPt3, rightBound.Top);
						}
						TEdge tedge = leftBound.NextInAEL;
						if (tedge != null)
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
			if (this.m_ActiveEdges == null)
			{
				edge.PrevInAEL = null;
				edge.NextInAEL = null;
				this.m_ActiveEdges = edge;
				return;
			}
			if (startEdge == null && this.E2InsertsBeforeE1(this.m_ActiveEdges, edge))
			{
				edge.PrevInAEL = null;
				edge.NextInAEL = this.m_ActiveEdges;
				this.m_ActiveEdges.PrevInAEL = edge;
				this.m_ActiveEdges = edge;
				return;
			}
			if (startEdge == null)
			{
				startEdge = this.m_ActiveEdges;
			}
			while (startEdge.NextInAEL != null && !this.E2InsertsBeforeE1(startEdge.NextInAEL, edge))
			{
				startEdge = startEdge.NextInAEL;
			}
			edge.NextInAEL = startEdge.NextInAEL;
			if (startEdge.NextInAEL != null)
			{
				startEdge.NextInAEL.PrevInAEL = edge;
			}
			edge.PrevInAEL = startEdge;
			startEdge.NextInAEL = edge;
		}

		private bool E2InsertsBeforeE1(TEdge e1, TEdge e2)
		{
			if (e2.Curr.X != e1.Curr.X)
			{
				return e2.Curr.X < e1.Curr.X;
			}
			if (e2.Top.Y > e1.Top.Y)
			{
				return e2.Top.X < Clipper.TopX(e1, e2.Top.Y);
			}
			return e1.Top.X > Clipper.TopX(e2, e1.Top.Y);
		}

		private bool IsEvenOddFillType(TEdge edge)
		{
			if (edge.PolyTyp == PolyType.ptSubject)
			{
				return this.m_SubjFillType == PolyFillType.pftEvenOdd;
			}
			return this.m_ClipFillType == PolyFillType.pftEvenOdd;
		}

		private bool IsEvenOddAltFillType(TEdge edge)
		{
			if (edge.PolyTyp == PolyType.ptSubject)
			{
				return this.m_ClipFillType == PolyFillType.pftEvenOdd;
			}
			return this.m_SubjFillType == PolyFillType.pftEvenOdd;
		}

		private bool IsContributing(TEdge edge)
		{
			PolyFillType polyFillType;
			PolyFillType polyFillType2;
			if (edge.PolyTyp == PolyType.ptSubject)
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
				if (edge.WindDelta == 0 && edge.WindCnt != 1)
				{
					return false;
				}
				break;
			case PolyFillType.pftNonZero:
				if (Math.Abs(edge.WindCnt) != 1)
				{
					return false;
				}
				break;
			case PolyFillType.pftPositive:
				if (edge.WindCnt != 1)
				{
					return false;
				}
				break;
			default:
				if (edge.WindCnt != -1)
				{
					return false;
				}
				break;
			}
			switch (this.m_ClipType)
			{
			case ClipType.ctIntersection:
				if (polyFillType2 <= PolyFillType.pftNonZero)
				{
					return edge.WindCnt2 != 0;
				}
				if (polyFillType2 != PolyFillType.pftPositive)
				{
					return edge.WindCnt2 < 0;
				}
				return edge.WindCnt2 > 0;
			case ClipType.ctUnion:
				if (polyFillType2 <= PolyFillType.pftNonZero)
				{
					return edge.WindCnt2 == 0;
				}
				if (polyFillType2 != PolyFillType.pftPositive)
				{
					return edge.WindCnt2 >= 0;
				}
				return edge.WindCnt2 <= 0;
			case ClipType.ctDifference:
				if (edge.PolyTyp == PolyType.ptSubject)
				{
					if (polyFillType2 <= PolyFillType.pftNonZero)
					{
						return edge.WindCnt2 == 0;
					}
					if (polyFillType2 != PolyFillType.pftPositive)
					{
						return edge.WindCnt2 >= 0;
					}
					return edge.WindCnt2 <= 0;
				}
				else
				{
					if (polyFillType2 <= PolyFillType.pftNonZero)
					{
						return edge.WindCnt2 != 0;
					}
					if (polyFillType2 != PolyFillType.pftPositive)
					{
						return edge.WindCnt2 < 0;
					}
					return edge.WindCnt2 > 0;
				}
				break;
			case ClipType.ctXor:
				if (edge.WindDelta != 0)
				{
					return true;
				}
				if (polyFillType2 <= PolyFillType.pftNonZero)
				{
					return edge.WindCnt2 == 0;
				}
				if (polyFillType2 != PolyFillType.pftPositive)
				{
					return edge.WindCnt2 >= 0;
				}
				return edge.WindCnt2 <= 0;
			default:
				return true;
			}
		}

		private void SetWindingCount(TEdge edge)
		{
			TEdge tedge = edge.PrevInAEL;
			while (tedge != null && (tedge.PolyTyp != edge.PolyTyp || tedge.WindDelta == 0))
			{
				tedge = tedge.PrevInAEL;
			}
			if (tedge == null)
			{
				edge.WindCnt = ((edge.WindDelta == 0) ? 1 : edge.WindDelta);
				edge.WindCnt2 = 0;
				tedge = this.m_ActiveEdges;
			}
			else if (edge.WindDelta == 0 && this.m_ClipType != ClipType.ctUnion)
			{
				edge.WindCnt = 1;
				edge.WindCnt2 = tedge.WindCnt2;
				tedge = tedge.NextInAEL;
			}
			else if (this.IsEvenOddFillType(edge))
			{
				if (edge.WindDelta == 0)
				{
					bool flag = true;
					for (TEdge tedge2 = tedge.PrevInAEL; tedge2 != null; tedge2 = tedge2.PrevInAEL)
					{
						if (tedge2.PolyTyp == tedge.PolyTyp && tedge2.WindDelta != 0)
						{
							flag = !flag;
						}
					}
					edge.WindCnt = (flag ? 0 : 1);
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
				if (tedge.WindCnt * tedge.WindDelta < 0)
				{
					if (Math.Abs(tedge.WindCnt) > 1)
					{
						if (tedge.WindDelta * edge.WindDelta < 0)
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
				else if (edge.WindDelta == 0)
				{
					edge.WindCnt = ((tedge.WindCnt < 0) ? (tedge.WindCnt - 1) : (tedge.WindCnt + 1));
				}
				else if (tedge.WindDelta * edge.WindDelta < 0)
				{
					edge.WindCnt = tedge.WindCnt;
				}
				else
				{
					edge.WindCnt = tedge.WindCnt + edge.WindDelta;
				}
				edge.WindCnt2 = tedge.WindCnt2;
				tedge = tedge.NextInAEL;
			}
			if (this.IsEvenOddAltFillType(edge))
			{
				while (tedge != edge)
				{
					if (tedge.WindDelta != 0)
					{
						edge.WindCnt2 = ((edge.WindCnt2 == 0) ? 1 : 0);
					}
					tedge = tedge.NextInAEL;
				}
				return;
			}
			while (tedge != edge)
			{
				edge.WindCnt2 += tedge.WindDelta;
				tedge = tedge.NextInAEL;
			}
		}

		private void AddEdgeToSEL(TEdge edge)
		{
			if (this.m_SortedEdges == null)
			{
				this.m_SortedEdges = edge;
				edge.PrevInSEL = null;
				edge.NextInSEL = null;
				return;
			}
			edge.NextInSEL = this.m_SortedEdges;
			edge.PrevInSEL = null;
			this.m_SortedEdges.PrevInSEL = edge;
			this.m_SortedEdges = edge;
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

		private void SwapPositionsInAEL(TEdge edge1, TEdge edge2)
		{
			if (edge1.NextInAEL == edge1.PrevInAEL || edge2.NextInAEL == edge2.PrevInAEL)
			{
				return;
			}
			if (edge1.NextInAEL == edge2)
			{
				TEdge nextInAEL = edge2.NextInAEL;
				if (nextInAEL != null)
				{
					nextInAEL.PrevInAEL = edge1;
				}
				TEdge prevInAEL = edge1.PrevInAEL;
				if (prevInAEL != null)
				{
					prevInAEL.NextInAEL = edge2;
				}
				edge2.PrevInAEL = prevInAEL;
				edge2.NextInAEL = edge1;
				edge1.PrevInAEL = edge2;
				edge1.NextInAEL = nextInAEL;
			}
			else if (edge2.NextInAEL == edge1)
			{
				TEdge nextInAEL2 = edge1.NextInAEL;
				if (nextInAEL2 != null)
				{
					nextInAEL2.PrevInAEL = edge2;
				}
				TEdge prevInAEL2 = edge2.PrevInAEL;
				if (prevInAEL2 != null)
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
				if (edge1.NextInAEL != null)
				{
					edge1.NextInAEL.PrevInAEL = edge1;
				}
				edge1.PrevInAEL = edge2.PrevInAEL;
				if (edge1.PrevInAEL != null)
				{
					edge1.PrevInAEL.NextInAEL = edge1;
				}
				edge2.NextInAEL = nextInAEL3;
				if (edge2.NextInAEL != null)
				{
					edge2.NextInAEL.PrevInAEL = edge2;
				}
				edge2.PrevInAEL = prevInAEL3;
				if (edge2.PrevInAEL != null)
				{
					edge2.PrevInAEL.NextInAEL = edge2;
				}
			}
			if (edge1.PrevInAEL == null)
			{
				this.m_ActiveEdges = edge1;
				return;
			}
			if (edge2.PrevInAEL == null)
			{
				this.m_ActiveEdges = edge2;
			}
		}

		private void SwapPositionsInSEL(TEdge edge1, TEdge edge2)
		{
			if (edge1.NextInSEL == null && edge1.PrevInSEL == null)
			{
				return;
			}
			if (edge2.NextInSEL == null && edge2.PrevInSEL == null)
			{
				return;
			}
			if (edge1.NextInSEL == edge2)
			{
				TEdge nextInSEL = edge2.NextInSEL;
				if (nextInSEL != null)
				{
					nextInSEL.PrevInSEL = edge1;
				}
				TEdge prevInSEL = edge1.PrevInSEL;
				if (prevInSEL != null)
				{
					prevInSEL.NextInSEL = edge2;
				}
				edge2.PrevInSEL = prevInSEL;
				edge2.NextInSEL = edge1;
				edge1.PrevInSEL = edge2;
				edge1.NextInSEL = nextInSEL;
			}
			else if (edge2.NextInSEL == edge1)
			{
				TEdge nextInSEL2 = edge1.NextInSEL;
				if (nextInSEL2 != null)
				{
					nextInSEL2.PrevInSEL = edge2;
				}
				TEdge prevInSEL2 = edge2.PrevInSEL;
				if (prevInSEL2 != null)
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
				if (edge1.NextInSEL != null)
				{
					edge1.NextInSEL.PrevInSEL = edge1;
				}
				edge1.PrevInSEL = edge2.PrevInSEL;
				if (edge1.PrevInSEL != null)
				{
					edge1.PrevInSEL.NextInSEL = edge1;
				}
				edge2.NextInSEL = nextInSEL3;
				if (edge2.NextInSEL != null)
				{
					edge2.NextInSEL.PrevInSEL = edge2;
				}
				edge2.PrevInSEL = prevInSEL3;
				if (edge2.PrevInSEL != null)
				{
					edge2.PrevInSEL.NextInSEL = edge2;
				}
			}
			if (edge1.PrevInSEL == null)
			{
				this.m_SortedEdges = edge1;
				return;
			}
			if (edge2.PrevInSEL == null)
			{
				this.m_SortedEdges = edge2;
			}
		}

		private void AddLocalMaxPoly(TEdge e1, TEdge e2, IntPoint pt)
		{
			this.AddOutPt(e1, pt);
			if (e2.WindDelta == 0)
			{
				this.AddOutPt(e2, pt);
			}
			if (e1.OutIdx == e2.OutIdx)
			{
				e1.OutIdx = -1;
				e2.OutIdx = -1;
				return;
			}
			if (e1.OutIdx < e2.OutIdx)
			{
				this.AppendPolygon(e1, e2);
				return;
			}
			this.AppendPolygon(e2, e1);
		}

		private OutPt AddLocalMinPoly(TEdge e1, TEdge e2, IntPoint pt)
		{
			OutPt outPt;
			TEdge tedge;
			TEdge tedge2;
			if (ClipperBase.IsHorizontal(e2) || e1.Dx > e2.Dx)
			{
				outPt = this.AddOutPt(e1, pt);
				e2.OutIdx = e1.OutIdx;
				e1.Side = EdgeSide.esLeft;
				e2.Side = EdgeSide.esRight;
				tedge = e1;
				if (tedge.PrevInAEL == e2)
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
				if (tedge.PrevInAEL == e1)
				{
					tedge2 = e1.PrevInAEL;
				}
				else
				{
					tedge2 = tedge.PrevInAEL;
				}
			}
			if (tedge2 != null && tedge2.OutIdx >= 0 && Clipper.TopX(tedge2, pt.Y) == Clipper.TopX(tedge, pt.Y) && ClipperBase.SlopesEqual(tedge, tedge2, this.m_UseFullRange) && tedge.WindDelta != 0 && tedge2.WindDelta != 0)
			{
				OutPt outPt2 = this.AddOutPt(tedge2, pt);
				this.AddJoin(outPt, outPt2, tedge.Top);
			}
			return outPt;
		}

		private OutRec CreateOutRec()
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

		private OutPt AddOutPt(TEdge e, IntPoint pt)
		{
			bool flag = e.Side == EdgeSide.esLeft;
			if (e.OutIdx < 0)
			{
				OutRec outRec = this.CreateOutRec();
				outRec.IsOpen = e.WindDelta == 0;
				OutPt outPt = new OutPt();
				outRec.Pts = outPt;
				outPt.Idx = outRec.Idx;
				outPt.Pt = pt;
				outPt.Next = outPt;
				outPt.Prev = outPt;
				if (!outRec.IsOpen)
				{
					this.SetHoleState(e, outRec);
				}
				e.OutIdx = outRec.Idx;
				return outPt;
			}
			OutRec outRec2 = this.m_PolyOuts[e.OutIdx];
			OutPt pts = outRec2.Pts;
			if (flag && pt == pts.Pt)
			{
				return pts;
			}
			if (!flag && pt == pts.Prev.Pt)
			{
				return pts.Prev;
			}
			OutPt outPt2 = new OutPt();
			outPt2.Idx = outRec2.Idx;
			outPt2.Pt = pt;
			outPt2.Next = pts;
			outPt2.Prev = pts.Prev;
			outPt2.Prev.Next = outPt2;
			pts.Prev = outPt2;
			if (flag)
			{
				outRec2.Pts = outPt2;
			}
			return outPt2;
		}

		internal void SwapPoints(ref IntPoint pt1, ref IntPoint pt2)
		{
			IntPoint intPoint = new IntPoint(pt1);
			pt1 = pt2;
			pt2 = intPoint;
		}

		private bool HorzSegmentsOverlap(long seg1a, long seg1b, long seg2a, long seg2b)
		{
			if (seg1a > seg1b)
			{
				base.Swap(ref seg1a, ref seg1b);
			}
			if (seg2a > seg2b)
			{
				base.Swap(ref seg2a, ref seg2b);
			}
			return seg1a < seg2b && seg2a < seg1b;
		}

		private void SetHoleState(TEdge e, OutRec outRec)
		{
			bool flag = false;
			for (TEdge tedge = e.PrevInAEL; tedge != null; tedge = tedge.PrevInAEL)
			{
				if (tedge.OutIdx >= 0 && tedge.WindDelta != 0)
				{
					flag = !flag;
					if (outRec.FirstLeft == null)
					{
						outRec.FirstLeft = this.m_PolyOuts[tedge.OutIdx];
					}
				}
			}
			if (flag)
			{
				outRec.IsHole = true;
			}
		}

		private double GetDx(IntPoint pt1, IntPoint pt2)
		{
			if (pt1.Y == pt2.Y)
			{
				return -3.4E+38;
			}
			return (double)(pt2.X - pt1.X) / (double)(pt2.Y - pt1.Y);
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
			return (num >= num3 && num >= num4) || (num2 >= num3 && num2 >= num4);
		}

		private OutPt GetBottomPt(OutPt pp)
		{
			OutPt outPt = null;
			OutPt outPt2;
			for (outPt2 = pp.Next; outPt2 != pp; outPt2 = outPt2.Next)
			{
				if (outPt2.Pt.Y > pp.Pt.Y)
				{
					pp = outPt2;
					outPt = null;
				}
				else if (outPt2.Pt.Y == pp.Pt.Y && outPt2.Pt.X <= pp.Pt.X)
				{
					if (outPt2.Pt.X < pp.Pt.X)
					{
						outPt = null;
						pp = outPt2;
					}
					else if (outPt2.Next != pp && outPt2.Prev != pp)
					{
						outPt = outPt2;
					}
				}
			}
			if (outPt != null)
			{
				while (outPt != outPt2)
				{
					if (!this.FirstIsBottomPt(outPt2, outPt))
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
			if (outRec1.BottomPt == null)
			{
				outRec1.BottomPt = this.GetBottomPt(outRec1.Pts);
			}
			if (outRec2.BottomPt == null)
			{
				outRec2.BottomPt = this.GetBottomPt(outRec2.Pts);
			}
			OutPt bottomPt = outRec1.BottomPt;
			OutPt bottomPt2 = outRec2.BottomPt;
			if (bottomPt.Pt.Y > bottomPt2.Pt.Y)
			{
				return outRec1;
			}
			if (bottomPt.Pt.Y < bottomPt2.Pt.Y)
			{
				return outRec2;
			}
			if (bottomPt.Pt.X < bottomPt2.Pt.X)
			{
				return outRec1;
			}
			if (bottomPt.Pt.X > bottomPt2.Pt.X)
			{
				return outRec2;
			}
			if (bottomPt.Next == bottomPt)
			{
				return outRec2;
			}
			if (bottomPt2.Next == bottomPt2)
			{
				return outRec1;
			}
			if (this.FirstIsBottomPt(bottomPt, bottomPt2))
			{
				return outRec1;
			}
			return outRec2;
		}

		private bool Param1RightOfParam2(OutRec outRec1, OutRec outRec2)
		{
			for (;;)
			{
				outRec1 = outRec1.FirstLeft;
				if (outRec1 == outRec2)
				{
					break;
				}
				if (outRec1 == null)
				{
					return false;
				}
			}
			return true;
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
			OutRec outRec3;
			if (this.Param1RightOfParam2(outRec, outRec2))
			{
				outRec3 = outRec2;
			}
			else if (this.Param1RightOfParam2(outRec2, outRec))
			{
				outRec3 = outRec;
			}
			else
			{
				outRec3 = this.GetLowermostRec(outRec, outRec2);
			}
			OutPt pts = outRec.Pts;
			OutPt prev = pts.Prev;
			OutPt pts2 = outRec2.Pts;
			OutPt prev2 = pts2.Prev;
			EdgeSide edgeSide;
			if (e1.Side == EdgeSide.esLeft)
			{
				if (e2.Side == EdgeSide.esLeft)
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
				edgeSide = EdgeSide.esLeft;
			}
			else
			{
				if (e2.Side == EdgeSide.esRight)
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
				edgeSide = EdgeSide.esRight;
			}
			outRec.BottomPt = null;
			if (outRec3 == outRec2)
			{
				if (outRec2.FirstLeft != outRec)
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
				if (tedge.OutIdx == outIdx2)
				{
					tedge.OutIdx = outIdx;
					tedge.Side = edgeSide;
					break;
				}
			}
			outRec2.Idx = outRec.Idx;
		}

		private void ReversePolyPtLinks(OutPt pp)
		{
			if (pp == null)
			{
				return;
			}
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
			if (e1.PolyTyp == e2.PolyTyp)
			{
				if (this.IsEvenOddFillType(e1))
				{
					int windCnt = e1.WindCnt;
					e1.WindCnt = e2.WindCnt;
					e2.WindCnt = windCnt;
				}
				else
				{
					if (e1.WindCnt + e2.WindDelta == 0)
					{
						e1.WindCnt = -e1.WindCnt;
					}
					else
					{
						e1.WindCnt += e2.WindDelta;
					}
					if (e2.WindCnt - e1.WindDelta == 0)
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
				if (!this.IsEvenOddFillType(e2))
				{
					e1.WindCnt2 += e2.WindDelta;
				}
				else
				{
					e1.WindCnt2 = ((e1.WindCnt2 == 0) ? 1 : 0);
				}
				if (!this.IsEvenOddFillType(e1))
				{
					e2.WindCnt2 -= e1.WindDelta;
				}
				else
				{
					e2.WindCnt2 = ((e2.WindCnt2 == 0) ? 1 : 0);
				}
			}
			PolyFillType polyFillType;
			PolyFillType polyFillType2;
			if (e1.PolyTyp == PolyType.ptSubject)
			{
				polyFillType = this.m_SubjFillType;
				polyFillType2 = this.m_ClipFillType;
			}
			else
			{
				polyFillType = this.m_ClipFillType;
				polyFillType2 = this.m_SubjFillType;
			}
			PolyFillType polyFillType3;
			PolyFillType polyFillType4;
			if (e2.PolyTyp == PolyType.ptSubject)
			{
				polyFillType3 = this.m_SubjFillType;
				polyFillType4 = this.m_ClipFillType;
			}
			else
			{
				polyFillType3 = this.m_ClipFillType;
				polyFillType4 = this.m_SubjFillType;
			}
			int num;
			if (polyFillType != PolyFillType.pftPositive)
			{
				if (polyFillType != PolyFillType.pftNegative)
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
			int num2;
			if (polyFillType3 != PolyFillType.pftPositive)
			{
				if (polyFillType3 != PolyFillType.pftNegative)
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
			if (!flag || !flag2)
			{
				if (flag)
				{
					if (num2 == 0 || num2 == 1)
					{
						this.AddOutPt(e1, pt);
						Clipper.SwapSides(e1, e2);
						Clipper.SwapPolyIndexes(e1, e2);
						return;
					}
				}
				else if (flag2)
				{
					if (num == 0 || num == 1)
					{
						this.AddOutPt(e2, pt);
						Clipper.SwapSides(e1, e2);
						Clipper.SwapPolyIndexes(e1, e2);
						return;
					}
				}
				else if ((num == 0 || num == 1) && (num2 == 0 || num2 == 1))
				{
					long num3;
					if (polyFillType2 != PolyFillType.pftPositive)
					{
						if (polyFillType2 != PolyFillType.pftNegative)
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
					long num4;
					if (polyFillType4 != PolyFillType.pftPositive)
					{
						if (polyFillType4 != PolyFillType.pftNegative)
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
					if (e1.PolyTyp != e2.PolyTyp)
					{
						this.AddLocalMinPoly(e1, e2, pt);
						return;
					}
					if (num == 1 && num2 == 1)
					{
						switch (this.m_ClipType)
						{
						case ClipType.ctIntersection:
							if (num3 > 0L && num4 > 0L)
							{
								this.AddLocalMinPoly(e1, e2, pt);
								return;
							}
							break;
						case ClipType.ctUnion:
							if (num3 <= 0L && num4 <= 0L)
							{
								this.AddLocalMinPoly(e1, e2, pt);
								return;
							}
							break;
						case ClipType.ctDifference:
							if ((e1.PolyTyp == PolyType.ptClip && num3 > 0L && num4 > 0L) || (e1.PolyTyp == PolyType.ptSubject && num3 <= 0L && num4 <= 0L))
							{
								this.AddLocalMinPoly(e1, e2, pt);
								return;
							}
							break;
						case ClipType.ctXor:
							this.AddLocalMinPoly(e1, e2, pt);
							return;
						default:
							return;
						}
					}
					else
					{
						Clipper.SwapSides(e1, e2);
					}
				}
				return;
			}
			if ((num != 0 && num != 1) || (num2 != 0 && num2 != 1) || (e1.PolyTyp != e2.PolyTyp && this.m_ClipType != ClipType.ctXor))
			{
				this.AddLocalMaxPoly(e1, e2, pt);
				return;
			}
			this.AddOutPt(e1, pt);
			this.AddOutPt(e2, pt);
			Clipper.SwapSides(e1, e2);
			Clipper.SwapPolyIndexes(e1, e2);
		}

		private void DeleteFromAEL(TEdge e)
		{
			TEdge prevInAEL = e.PrevInAEL;
			TEdge nextInAEL = e.NextInAEL;
			if (prevInAEL == null && nextInAEL == null && e != this.m_ActiveEdges)
			{
				return;
			}
			if (prevInAEL != null)
			{
				prevInAEL.NextInAEL = nextInAEL;
			}
			else
			{
				this.m_ActiveEdges = nextInAEL;
			}
			if (nextInAEL != null)
			{
				nextInAEL.PrevInAEL = prevInAEL;
			}
			e.NextInAEL = null;
			e.PrevInAEL = null;
		}

		private void DeleteFromSEL(TEdge e)
		{
			TEdge prevInSEL = e.PrevInSEL;
			TEdge nextInSEL = e.NextInSEL;
			if (prevInSEL == null && nextInSEL == null && e != this.m_SortedEdges)
			{
				return;
			}
			if (prevInSEL != null)
			{
				prevInSEL.NextInSEL = nextInSEL;
			}
			else
			{
				this.m_SortedEdges = nextInSEL;
			}
			if (nextInSEL != null)
			{
				nextInSEL.PrevInSEL = prevInSEL;
			}
			e.NextInSEL = null;
			e.PrevInSEL = null;
		}

		private void UpdateEdgeIntoAEL(ref TEdge e)
		{
			if (e.NextInLML == null)
			{
				throw new ClipperException("UpdateEdgeIntoAEL: invalid call");
			}
			TEdge prevInAEL = e.PrevInAEL;
			TEdge nextInAEL = e.NextInAEL;
			e.NextInLML.OutIdx = e.OutIdx;
			if (prevInAEL != null)
			{
				prevInAEL.NextInAEL = e.NextInLML;
			}
			else
			{
				this.m_ActiveEdges = e.NextInLML;
			}
			if (nextInAEL != null)
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
			if (!ClipperBase.IsHorizontal(e))
			{
				this.InsertScanbeam(e.Top.Y);
			}
		}

		private void ProcessHorizontals(bool isTopOfScanbeam)
		{
			for (TEdge tedge = this.m_SortedEdges; tedge != null; tedge = this.m_SortedEdges)
			{
				this.DeleteFromSEL(tedge);
				this.ProcessHorizontal(tedge, isTopOfScanbeam);
			}
		}

		private void GetHorzDirection(TEdge HorzEdge, out Direction Dir, out long Left, out long Right)
		{
			if (HorzEdge.Bot.X < HorzEdge.Top.X)
			{
				Left = HorzEdge.Bot.X;
				Right = HorzEdge.Top.X;
				Dir = Direction.dLeftToRight;
				return;
			}
			Left = HorzEdge.Top.X;
			Right = HorzEdge.Bot.X;
			Dir = Direction.dRightToLeft;
		}

		private void ProcessHorizontal(TEdge horzEdge, bool isTopOfScanbeam)
		{
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
			if (tedge.NextInLML == null)
			{
				tedge2 = this.GetMaximaPair(tedge);
			}
			for (;;)
			{
				bool flag = horzEdge == tedge;
				TEdge tedge3 = this.GetNextInAEL(horzEdge, direction);
				while (tedge3 != null && (tedge3.Curr.X != horzEdge.Top.X || horzEdge.NextInLML == null || tedge3.Dx >= horzEdge.NextInLML.Dx))
				{
					TEdge nextInAEL = this.GetNextInAEL(tedge3, direction);
					if ((direction == Direction.dLeftToRight && tedge3.Curr.X <= num2) || (direction == Direction.dRightToLeft && tedge3.Curr.X >= num))
					{
						if (tedge3 == tedge2 && flag)
						{
							goto Block_7;
						}
						if (direction == Direction.dLeftToRight)
						{
							IntPoint intPoint = new IntPoint(tedge3.Curr.X, horzEdge.Curr.Y);
							this.IntersectEdges(horzEdge, tedge3, intPoint);
						}
						else
						{
							IntPoint intPoint2 = new IntPoint(tedge3.Curr.X, horzEdge.Curr.Y);
							this.IntersectEdges(tedge3, horzEdge, intPoint2);
						}
						this.SwapPositionsInAEL(horzEdge, tedge3);
					}
					else if ((direction == Direction.dLeftToRight && tedge3.Curr.X >= num2) || (direction == Direction.dRightToLeft && tedge3.Curr.X <= num))
					{
						break;
					}
					tedge3 = nextInAEL;
				}
				if (horzEdge.NextInLML == null || !ClipperBase.IsHorizontal(horzEdge.NextInLML))
				{
					goto IL_026C;
				}
				this.UpdateEdgeIntoAEL(ref horzEdge);
				if (horzEdge.OutIdx >= 0)
				{
					this.AddOutPt(horzEdge, horzEdge.Bot);
				}
				this.GetHorzDirection(horzEdge, out direction, out num, out num2);
			}
			Block_7:
			if (horzEdge.OutIdx >= 0)
			{
				OutPt outPt = this.AddOutPt(horzEdge, horzEdge.Top);
				for (TEdge tedge4 = this.m_SortedEdges; tedge4 != null; tedge4 = tedge4.NextInSEL)
				{
					if (tedge4.OutIdx >= 0 && this.HorzSegmentsOverlap(horzEdge.Bot.X, horzEdge.Top.X, tedge4.Bot.X, tedge4.Top.X))
					{
						OutPt outPt2 = this.AddOutPt(tedge4, tedge4.Bot);
						this.AddJoin(outPt2, outPt, tedge4.Top);
					}
				}
				this.AddGhostJoin(outPt, horzEdge.Bot);
				this.AddLocalMaxPoly(horzEdge, tedge2, horzEdge.Top);
			}
			this.DeleteFromAEL(horzEdge);
			this.DeleteFromAEL(tedge2);
			return;
			IL_026C:
			if (horzEdge.NextInLML != null)
			{
				if (horzEdge.OutIdx < 0)
				{
					this.UpdateEdgeIntoAEL(ref horzEdge);
					return;
				}
				OutPt outPt3 = this.AddOutPt(horzEdge, horzEdge.Top);
				if (isTopOfScanbeam)
				{
					this.AddGhostJoin(outPt3, horzEdge.Bot);
				}
				this.UpdateEdgeIntoAEL(ref horzEdge);
				if (horzEdge.WindDelta == 0)
				{
					return;
				}
				TEdge prevInAEL = horzEdge.PrevInAEL;
				TEdge nextInAEL2 = horzEdge.NextInAEL;
				if (prevInAEL != null && prevInAEL.Curr.X == horzEdge.Bot.X && prevInAEL.Curr.Y == horzEdge.Bot.Y && prevInAEL.WindDelta != 0 && prevInAEL.OutIdx >= 0 && prevInAEL.Curr.Y > prevInAEL.Top.Y && ClipperBase.SlopesEqual(horzEdge, prevInAEL, this.m_UseFullRange))
				{
					OutPt outPt4 = this.AddOutPt(prevInAEL, horzEdge.Bot);
					this.AddJoin(outPt3, outPt4, horzEdge.Top);
					return;
				}
				if (nextInAEL2 != null && nextInAEL2.Curr.X == horzEdge.Bot.X && nextInAEL2.Curr.Y == horzEdge.Bot.Y && nextInAEL2.WindDelta != 0 && nextInAEL2.OutIdx >= 0 && nextInAEL2.Curr.Y > nextInAEL2.Top.Y && ClipperBase.SlopesEqual(horzEdge, nextInAEL2, this.m_UseFullRange))
				{
					OutPt outPt5 = this.AddOutPt(nextInAEL2, horzEdge.Bot);
					this.AddJoin(outPt3, outPt5, horzEdge.Top);
					return;
				}
			}
			else
			{
				if (horzEdge.OutIdx >= 0)
				{
					this.AddOutPt(horzEdge, horzEdge.Top);
				}
				this.DeleteFromAEL(horzEdge);
			}
		}

		private TEdge GetNextInAEL(TEdge e, Direction Direction)
		{
			if (Direction != Direction.dLeftToRight)
			{
				return e.PrevInAEL;
			}
			return e.NextInAEL;
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

		private TEdge GetMaximaPair(TEdge e)
		{
			TEdge tedge = null;
			if (e.Next.Top == e.Top && e.Next.NextInLML == null)
			{
				tedge = e.Next;
			}
			else if (e.Prev.Top == e.Top && e.Prev.NextInLML == null)
			{
				tedge = e.Prev;
			}
			if (tedge != null && (tedge.OutIdx == -2 || (tedge.NextInAEL == tedge.PrevInAEL && !ClipperBase.IsHorizontal(tedge))))
			{
				return null;
			}
			return tedge;
		}

		private bool ProcessIntersections(long topY)
		{
			if (this.m_ActiveEdges == null)
			{
				return true;
			}
			try
			{
				this.BuildIntersectList(topY);
				if (this.m_IntersectList.Count == 0)
				{
					return true;
				}
				if (this.m_IntersectList.Count != 1 && !this.FixupIntersectionOrder())
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
			return true;
		}

		private void BuildIntersectList(long topY)
		{
			if (this.m_ActiveEdges == null)
			{
				return;
			}
			TEdge tedge = this.m_ActiveEdges;
			this.m_SortedEdges = tedge;
			while (tedge != null)
			{
				tedge.PrevInSEL = tedge.PrevInAEL;
				tedge.NextInSEL = tedge.NextInAEL;
				tedge.Curr.X = Clipper.TopX(tedge, topY);
				tedge = tedge.NextInAEL;
			}
			bool flag = true;
			while (flag && this.m_SortedEdges != null)
			{
				flag = false;
				tedge = this.m_SortedEdges;
				while (tedge.NextInSEL != null)
				{
					TEdge nextInSEL = tedge.NextInSEL;
					if (tedge.Curr.X > nextInSEL.Curr.X)
					{
						IntPoint intPoint;
						this.IntersectPoint(tedge, nextInSEL, out intPoint);
						IntersectNode intersectNode = new IntersectNode();
						intersectNode.Edge1 = tedge;
						intersectNode.Edge2 = nextInSEL;
						intersectNode.Pt = intPoint;
						this.m_IntersectList.Add(intersectNode);
						this.SwapPositionsInSEL(tedge, nextInSEL);
						flag = true;
					}
					else
					{
						tedge = nextInSEL;
					}
				}
				if (tedge.PrevInSEL == null)
				{
					break;
				}
				tedge.PrevInSEL.NextInSEL = null;
			}
			this.m_SortedEdges = null;
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
				if (!this.EdgesAdjacent(this.m_IntersectList[i]))
				{
					int num = i + 1;
					while (num < count && !this.EdgesAdjacent(this.m_IntersectList[num]))
					{
						num++;
					}
					if (num == count)
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
				this.SwapPositionsInAEL(intersectNode.Edge1, intersectNode.Edge2);
			}
			this.m_IntersectList.Clear();
		}

		internal static long Round(double value)
		{
			if (value >= 0.0)
			{
				return (long)(value + 0.5);
			}
			return (long)(value - 0.5);
		}

		private static long TopX(TEdge edge, long currentY)
		{
			if (currentY == edge.Top.Y)
			{
				return edge.Top.X;
			}
			return edge.Bot.X + Clipper.Round(edge.Dx * (double)(currentY - edge.Bot.Y));
		}

		private void IntersectPoint(TEdge edge1, TEdge edge2, out IntPoint ip)
		{
			ip = default(IntPoint);
			if (edge1.Dx == edge2.Dx)
			{
				ip.Y = edge1.Curr.Y;
				ip.X = Clipper.TopX(edge1, ip.Y);
				return;
			}
			if (edge1.Delta.X == 0L)
			{
				ip.X = edge1.Bot.X;
				if (ClipperBase.IsHorizontal(edge2))
				{
					ip.Y = edge2.Bot.Y;
				}
				else
				{
					double num = (double)edge2.Bot.Y - (double)edge2.Bot.X / edge2.Dx;
					ip.Y = Clipper.Round((double)ip.X / edge2.Dx + num);
				}
			}
			else if (edge2.Delta.X == 0L)
			{
				ip.X = edge2.Bot.X;
				if (ClipperBase.IsHorizontal(edge1))
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
				if (Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx))
				{
					ip.X = Clipper.Round(edge1.Dx * num3 + num2);
				}
				else
				{
					ip.X = Clipper.Round(edge2.Dx * num3 + num);
				}
			}
			if (ip.Y < edge1.Top.Y || ip.Y < edge2.Top.Y)
			{
				if (edge1.Top.Y > edge2.Top.Y)
				{
					ip.Y = edge1.Top.Y;
				}
				else
				{
					ip.Y = edge2.Top.Y;
				}
				if (Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx))
				{
					ip.X = Clipper.TopX(edge1, ip.Y);
				}
				else
				{
					ip.X = Clipper.TopX(edge2, ip.Y);
				}
			}
			if (ip.Y > edge1.Curr.Y)
			{
				ip.Y = edge1.Curr.Y;
				if (Math.Abs(edge1.Dx) > Math.Abs(edge2.Dx))
				{
					ip.X = Clipper.TopX(edge2, ip.Y);
					return;
				}
				ip.X = Clipper.TopX(edge1, ip.Y);
			}
		}

		private void ProcessEdgesAtTopOfScanbeam(long topY)
		{
			TEdge tedge = this.m_ActiveEdges;
			while (tedge != null)
			{
				bool flag = this.IsMaxima(tedge, (double)topY);
				if (flag)
				{
					TEdge maximaPair = this.GetMaximaPair(tedge);
					flag = maximaPair == null || !ClipperBase.IsHorizontal(maximaPair);
				}
				if (flag)
				{
					TEdge prevInAEL = tedge.PrevInAEL;
					this.DoMaxima(tedge);
					if (prevInAEL == null)
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
					if (this.IsIntermediate(tedge, (double)topY) && ClipperBase.IsHorizontal(tedge.NextInLML))
					{
						this.UpdateEdgeIntoAEL(ref tedge);
						if (tedge.OutIdx >= 0)
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
					if (this.StrictlySimple)
					{
						TEdge prevInAEL2 = tedge.PrevInAEL;
						if (tedge.OutIdx >= 0 && tedge.WindDelta != 0 && prevInAEL2 != null && prevInAEL2.OutIdx >= 0 && prevInAEL2.Curr.X == tedge.Curr.X && prevInAEL2.WindDelta != 0)
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
			this.ProcessHorizontals(true);
			for (tedge = this.m_ActiveEdges; tedge != null; tedge = tedge.NextInAEL)
			{
				if (this.IsIntermediate(tedge, (double)topY))
				{
					OutPt outPt3 = null;
					if (tedge.OutIdx >= 0)
					{
						outPt3 = this.AddOutPt(tedge, tedge.Top);
					}
					this.UpdateEdgeIntoAEL(ref tedge);
					TEdge prevInAEL3 = tedge.PrevInAEL;
					TEdge nextInAEL = tedge.NextInAEL;
					if (prevInAEL3 != null && prevInAEL3.Curr.X == tedge.Bot.X && prevInAEL3.Curr.Y == tedge.Bot.Y && outPt3 != null && prevInAEL3.OutIdx >= 0 && prevInAEL3.Curr.Y > prevInAEL3.Top.Y && ClipperBase.SlopesEqual(tedge, prevInAEL3, this.m_UseFullRange) && tedge.WindDelta != 0 && prevInAEL3.WindDelta != 0)
					{
						OutPt outPt4 = this.AddOutPt(prevInAEL3, tedge.Bot);
						this.AddJoin(outPt3, outPt4, tedge.Top);
					}
					else if (nextInAEL != null && nextInAEL.Curr.X == tedge.Bot.X && nextInAEL.Curr.Y == tedge.Bot.Y && outPt3 != null && nextInAEL.OutIdx >= 0 && nextInAEL.Curr.Y > nextInAEL.Top.Y && ClipperBase.SlopesEqual(tedge, nextInAEL, this.m_UseFullRange) && tedge.WindDelta != 0 && nextInAEL.WindDelta != 0)
					{
						OutPt outPt5 = this.AddOutPt(nextInAEL, tedge.Bot);
						this.AddJoin(outPt3, outPt5, tedge.Top);
					}
				}
			}
		}

		private void DoMaxima(TEdge e)
		{
			TEdge maximaPair = this.GetMaximaPair(e);
			if (maximaPair == null)
			{
				if (e.OutIdx >= 0)
				{
					this.AddOutPt(e, e.Top);
				}
				this.DeleteFromAEL(e);
				return;
			}
			TEdge tedge = e.NextInAEL;
			while (tedge != null && tedge != maximaPair)
			{
				this.IntersectEdges(e, tedge, e.Top);
				this.SwapPositionsInAEL(e, tedge);
				tedge = e.NextInAEL;
			}
			if (e.OutIdx == -1 && maximaPair.OutIdx == -1)
			{
				this.DeleteFromAEL(e);
				this.DeleteFromAEL(maximaPair);
				return;
			}
			if (e.OutIdx >= 0 && maximaPair.OutIdx >= 0)
			{
				if (e.OutIdx >= 0)
				{
					this.AddLocalMaxPoly(e, maximaPair, e.Top);
				}
				this.DeleteFromAEL(e);
				this.DeleteFromAEL(maximaPair);
				return;
			}
			throw new ClipperException("DoMaxima error");
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
			if (pts == null)
			{
				return 0;
			}
			int num = 0;
			OutPt outPt = pts;
			do
			{
				num++;
				outPt = outPt.Next;
			}
			while (outPt != pts);
			return num;
		}

		private void BuildResult(List<List<IntPoint>> polyg)
		{
			polyg.Clear();
			polyg.Capacity = this.m_PolyOuts.Count;
			for (int i = 0; i < this.m_PolyOuts.Count; i++)
			{
				OutRec outRec = this.m_PolyOuts[i];
				if (outRec.Pts != null)
				{
					OutPt outPt = outRec.Pts.Prev;
					int num = this.PointCount(outPt);
					if (num >= 2)
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
				if ((!outRec.IsOpen || num >= 2) && (outRec.IsOpen || num >= 3))
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
				if (outRec2.PolyNode != null)
				{
					if (outRec2.IsOpen)
					{
						outRec2.PolyNode.IsOpen = true;
						polytree.AddChild(outRec2.PolyNode);
					}
					else if (outRec2.FirstLeft != null && outRec2.FirstLeft.PolyNode != null)
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

		private void FixupOutPolygon(OutRec outRec)
		{
			OutPt outPt = null;
			outRec.BottomPt = null;
			OutPt outPt2 = outRec.Pts;
			while (outPt2.Prev != outPt2 && outPt2.Prev != outPt2.Next)
			{
				if (outPt2.Pt == outPt2.Next.Pt || outPt2.Pt == outPt2.Prev.Pt || (ClipperBase.SlopesEqual(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt, this.m_UseFullRange) && (!base.PreserveCollinear || !base.Pt2IsBetweenPt1AndPt3(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt))))
				{
					outPt = null;
					outPt2.Prev.Next = outPt2.Next;
					outPt2.Next.Prev = outPt2.Prev;
					outPt2 = outPt2.Prev;
				}
				else
				{
					if (outPt2 == outPt)
					{
						outRec.Pts = outPt2;
						return;
					}
					if (outPt == null)
					{
						outPt = outPt2;
					}
					outPt2 = outPt2.Next;
				}
			}
			outRec.Pts = null;
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
			if (a1 < a2)
			{
				if (b1 < b2)
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
			else if (b1 < b2)
			{
				Left = Math.Max(a2, b1);
				Right = Math.Min(a1, b2);
			}
			else
			{
				Left = Math.Max(a2, b2);
				Right = Math.Min(a1, b1);
			}
			return Left < Right;
		}

		private bool JoinHorz(OutPt op1, OutPt op1b, OutPt op2, OutPt op2b, IntPoint Pt, bool DiscardLeft)
		{
			Direction direction = ((op1.Pt.X > op1b.Pt.X) ? Direction.dRightToLeft : Direction.dLeftToRight);
			Direction direction2 = ((op2.Pt.X > op2b.Pt.X) ? Direction.dRightToLeft : Direction.dLeftToRight);
			if (direction == direction2)
			{
				return false;
			}
			if (direction == Direction.dLeftToRight)
			{
				while (op1.Next.Pt.X <= Pt.X && op1.Next.Pt.X >= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
				{
					op1 = op1.Next;
				}
				if (DiscardLeft && op1.Pt.X != Pt.X)
				{
					op1 = op1.Next;
				}
				op1b = this.DupOutPt(op1, !DiscardLeft);
				if (op1b.Pt != Pt)
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
				if (!DiscardLeft && op1.Pt.X != Pt.X)
				{
					op1 = op1.Next;
				}
				op1b = this.DupOutPt(op1, DiscardLeft);
				if (op1b.Pt != Pt)
				{
					op1 = op1b;
					op1.Pt = Pt;
					op1b = this.DupOutPt(op1, DiscardLeft);
				}
			}
			if (direction2 == Direction.dLeftToRight)
			{
				while (op2.Next.Pt.X <= Pt.X && op2.Next.Pt.X >= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
				{
					op2 = op2.Next;
				}
				if (DiscardLeft && op2.Pt.X != Pt.X)
				{
					op2 = op2.Next;
				}
				op2b = this.DupOutPt(op2, !DiscardLeft);
				if (op2b.Pt != Pt)
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
				if (!DiscardLeft && op2.Pt.X != Pt.X)
				{
					op2 = op2.Next;
				}
				op2b = this.DupOutPt(op2, DiscardLeft);
				if (op2b.Pt != Pt)
				{
					op2 = op2b;
					op2.Pt = Pt;
					op2b = this.DupOutPt(op2, DiscardLeft);
				}
			}
			if (direction == Direction.dLeftToRight == DiscardLeft)
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
			return true;
		}

		private bool JoinPoints(Join j, OutRec outRec1, OutRec outRec2)
		{
			OutPt outPt = j.OutPt1;
			OutPt outPt2 = j.OutPt2;
			bool flag = j.OutPt1.Pt.Y == j.OffPt.Y;
			if (flag && j.OffPt == j.OutPt1.Pt && j.OffPt == j.OutPt2.Pt)
			{
				if (outRec1 != outRec2)
				{
					return false;
				}
				OutPt outPt3 = j.OutPt1.Next;
				while (outPt3 != outPt && outPt3.Pt == j.OffPt)
				{
					outPt3 = outPt3.Next;
				}
				bool flag2 = outPt3.Pt.Y > j.OffPt.Y;
				OutPt outPt4 = j.OutPt2.Next;
				while (outPt4 != outPt2 && outPt4.Pt == j.OffPt)
				{
					outPt4 = outPt4.Next;
				}
				bool flag3 = outPt4.Pt.Y > j.OffPt.Y;
				if (flag2 == flag3)
				{
					return false;
				}
				if (flag2)
				{
					outPt3 = this.DupOutPt(outPt, false);
					outPt4 = this.DupOutPt(outPt2, true);
					outPt.Prev = outPt2;
					outPt2.Next = outPt;
					outPt3.Next = outPt4;
					outPt4.Prev = outPt3;
					j.OutPt1 = outPt;
					j.OutPt2 = outPt3;
					return true;
				}
				outPt3 = this.DupOutPt(outPt, true);
				outPt4 = this.DupOutPt(outPt2, false);
				outPt.Next = outPt2;
				outPt2.Prev = outPt;
				outPt3.Prev = outPt4;
				outPt4.Next = outPt3;
				j.OutPt1 = outPt;
				j.OutPt2 = outPt3;
				return true;
			}
			else if (flag)
			{
				OutPt outPt3 = outPt;
				while (outPt.Prev.Pt.Y == outPt.Pt.Y && outPt.Prev != outPt3)
				{
					if (outPt.Prev == outPt2)
					{
						break;
					}
					outPt = outPt.Prev;
				}
				while (outPt3.Next.Pt.Y == outPt3.Pt.Y && outPt3.Next != outPt && outPt3.Next != outPt2)
				{
					outPt3 = outPt3.Next;
				}
				if (outPt3.Next == outPt || outPt3.Next == outPt2)
				{
					return false;
				}
				OutPt outPt4 = outPt2;
				while (outPt2.Prev.Pt.Y == outPt2.Pt.Y && outPt2.Prev != outPt4)
				{
					if (outPt2.Prev == outPt3)
					{
						break;
					}
					outPt2 = outPt2.Prev;
				}
				while (outPt4.Next.Pt.Y == outPt4.Pt.Y && outPt4.Next != outPt2 && outPt4.Next != outPt)
				{
					outPt4 = outPt4.Next;
				}
				if (outPt4.Next == outPt2 || outPt4.Next == outPt)
				{
					return false;
				}
				long num;
				long num2;
				if (!this.GetOverlap(outPt.Pt.X, outPt3.Pt.X, outPt2.Pt.X, outPt4.Pt.X, out num, out num2))
				{
					return false;
				}
				IntPoint intPoint;
				bool flag4;
				if (outPt.Pt.X >= num && outPt.Pt.X <= num2)
				{
					intPoint = outPt.Pt;
					flag4 = outPt.Pt.X > outPt3.Pt.X;
				}
				else if (outPt2.Pt.X >= num && outPt2.Pt.X <= num2)
				{
					intPoint = outPt2.Pt;
					flag4 = outPt2.Pt.X > outPt4.Pt.X;
				}
				else if (outPt3.Pt.X >= num && outPt3.Pt.X <= num2)
				{
					intPoint = outPt3.Pt;
					flag4 = outPt3.Pt.X > outPt.Pt.X;
				}
				else
				{
					intPoint = outPt4.Pt;
					flag4 = outPt4.Pt.X > outPt2.Pt.X;
				}
				j.OutPt1 = outPt;
				j.OutPt2 = outPt2;
				return this.JoinHorz(outPt, outPt3, outPt2, outPt4, intPoint, flag4);
			}
			else
			{
				OutPt outPt3 = outPt.Next;
				while (outPt3.Pt == outPt.Pt && outPt3 != outPt)
				{
					outPt3 = outPt3.Next;
				}
				bool flag5 = outPt3.Pt.Y > outPt.Pt.Y || !ClipperBase.SlopesEqual(outPt.Pt, outPt3.Pt, j.OffPt, this.m_UseFullRange);
				if (flag5)
				{
					outPt3 = outPt.Prev;
					while (outPt3.Pt == outPt.Pt && outPt3 != outPt)
					{
						outPt3 = outPt3.Prev;
					}
					if (outPt3.Pt.Y > outPt.Pt.Y || !ClipperBase.SlopesEqual(outPt.Pt, outPt3.Pt, j.OffPt, this.m_UseFullRange))
					{
						return false;
					}
				}
				OutPt outPt4 = outPt2.Next;
				while (outPt4.Pt == outPt2.Pt && outPt4 != outPt2)
				{
					outPt4 = outPt4.Next;
				}
				bool flag6 = outPt4.Pt.Y > outPt2.Pt.Y || !ClipperBase.SlopesEqual(outPt2.Pt, outPt4.Pt, j.OffPt, this.m_UseFullRange);
				if (flag6)
				{
					outPt4 = outPt2.Prev;
					while (outPt4.Pt == outPt2.Pt && outPt4 != outPt2)
					{
						outPt4 = outPt4.Prev;
					}
					if (outPt4.Pt.Y > outPt2.Pt.Y || !ClipperBase.SlopesEqual(outPt2.Pt, outPt4.Pt, j.OffPt, this.m_UseFullRange))
					{
						return false;
					}
				}
				if (outPt3 == outPt || outPt4 == outPt2 || outPt3 == outPt4 || (outRec1 == outRec2 && flag5 == flag6))
				{
					return false;
				}
				if (flag5)
				{
					outPt3 = this.DupOutPt(outPt, false);
					outPt4 = this.DupOutPt(outPt2, true);
					outPt.Prev = outPt2;
					outPt2.Next = outPt;
					outPt3.Next = outPt4;
					outPt4.Prev = outPt3;
					j.OutPt1 = outPt;
					j.OutPt2 = outPt3;
					return true;
				}
				outPt3 = this.DupOutPt(outPt, true);
				outPt4 = this.DupOutPt(outPt2, false);
				outPt.Next = outPt2;
				outPt2.Prev = outPt;
				outPt3.Prev = outPt4;
				outPt4.Next = outPt3;
				j.OutPt1 = outPt;
				j.OutPt2 = outPt3;
				return true;
			}
		}

		public static int PointInPolygon(IntPoint pt, List<IntPoint> path)
		{
			int num = 0;
			int count = path.Count;
			if (count < 3)
			{
				return 0;
			}
			IntPoint intPoint = path[0];
			for (int i = 1; i <= count; i++)
			{
				IntPoint intPoint2 = ((i == count) ? path[0] : path[i]);
				if (intPoint2.Y == pt.Y && (intPoint2.X == pt.X || (intPoint.Y == pt.Y && intPoint2.X > pt.X == intPoint.X < pt.X)))
				{
					return -1;
				}
				if (intPoint.Y < pt.Y != intPoint2.Y < pt.Y)
				{
					if (intPoint.X >= pt.X)
					{
						if (intPoint2.X > pt.X)
						{
							num = 1 - num;
						}
						else
						{
							double num2 = (double)(intPoint.X - pt.X) * (double)(intPoint2.Y - pt.Y) - (double)(intPoint2.X - pt.X) * (double)(intPoint.Y - pt.Y);
							if (num2 == 0.0)
							{
								return -1;
							}
							if (num2 > 0.0 == intPoint2.Y > intPoint.Y)
							{
								num = 1 - num;
							}
						}
					}
					else if (intPoint2.X > pt.X)
					{
						double num3 = (double)(intPoint.X - pt.X) * (double)(intPoint2.Y - pt.Y) - (double)(intPoint2.X - pt.X) * (double)(intPoint.Y - pt.Y);
						if (num3 == 0.0)
						{
							return -1;
						}
						if (num3 > 0.0 == intPoint2.Y > intPoint.Y)
						{
							num = 1 - num;
						}
					}
				}
				intPoint = intPoint2;
			}
			return num;
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
				if (y2 == y && (x2 == x || (num3 == y && x2 > x == num2 < x)))
				{
					break;
				}
				if (num3 < y != y2 < y)
				{
					if (num2 >= x)
					{
						if (x2 > x)
						{
							num = 1 - num;
						}
						else
						{
							double num4 = (double)(num2 - x) * (double)(y2 - y) - (double)(x2 - x) * (double)(num3 - y);
							if (num4 == 0.0)
							{
								return -1;
							}
							if (num4 > 0.0 == y2 > num3)
							{
								num = 1 - num;
							}
						}
					}
					else if (x2 > x)
					{
						double num5 = (double)(num2 - x) * (double)(y2 - y) - (double)(x2 - x) * (double)(num3 - y);
						if (num5 == 0.0)
						{
							return -1;
						}
						if (num5 > 0.0 == y2 > num3)
						{
							num = 1 - num;
						}
					}
				}
				num2 = x2;
				num3 = y2;
				if (outPt == op)
				{
					return num;
				}
			}
			return -1;
		}

		private static bool Poly2ContainsPoly1(OutPt outPt1, OutPt outPt2)
		{
			OutPt outPt3 = outPt1;
			int num;
			for (;;)
			{
				num = Clipper.PointInPolygon(outPt3.Pt, outPt2);
				if (num >= 0)
				{
					break;
				}
				outPt3 = outPt3.Next;
				if (outPt3 == outPt1)
				{
					return true;
				}
			}
			return num > 0;
		}

		private void FixupFirstLefts1(OutRec OldOutRec, OutRec NewOutRec)
		{
			for (int i = 0; i < this.m_PolyOuts.Count; i++)
			{
				OutRec outRec = this.m_PolyOuts[i];
				if (outRec.Pts != null && outRec.FirstLeft != null && Clipper.ParseFirstLeft(outRec.FirstLeft) == OldOutRec && Clipper.Poly2ContainsPoly1(outRec.Pts, NewOutRec.Pts))
				{
					outRec.FirstLeft = NewOutRec;
				}
			}
		}

		private void FixupFirstLefts2(OutRec OldOutRec, OutRec NewOutRec)
		{
			foreach (OutRec outRec in this.m_PolyOuts)
			{
				if (outRec.FirstLeft == OldOutRec)
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
				if (outRec.Pts != null && outRec2.Pts != null)
				{
					OutRec outRec3;
					if (outRec == outRec2)
					{
						outRec3 = outRec;
					}
					else if (this.Param1RightOfParam2(outRec, outRec2))
					{
						outRec3 = outRec2;
					}
					else if (this.Param1RightOfParam2(outRec2, outRec))
					{
						outRec3 = outRec;
					}
					else
					{
						outRec3 = this.GetLowermostRec(outRec, outRec2);
					}
					if (this.JoinPoints(join, outRec, outRec2))
					{
						if (outRec == outRec2)
						{
							outRec.Pts = join.OutPt1;
							outRec.BottomPt = null;
							outRec2 = this.CreateOutRec();
							outRec2.Pts = join.OutPt2;
							this.UpdateOutPtIdxs(outRec2);
							if (this.m_UsingPolyTree)
							{
								for (int j = 0; j < this.m_PolyOuts.Count - 1; j++)
								{
									OutRec outRec4 = this.m_PolyOuts[j];
									if (outRec4.Pts != null && Clipper.ParseFirstLeft(outRec4.FirstLeft) == outRec && outRec4.IsHole != outRec.IsHole && Clipper.Poly2ContainsPoly1(outRec4.Pts, join.OutPt2))
									{
										outRec4.FirstLeft = outRec2;
									}
								}
							}
							if (Clipper.Poly2ContainsPoly1(outRec2.Pts, outRec.Pts))
							{
								outRec2.IsHole = !outRec.IsHole;
								outRec2.FirstLeft = outRec;
								if (this.m_UsingPolyTree)
								{
									this.FixupFirstLefts2(outRec2, outRec);
								}
								if ((outRec2.IsHole ^ this.ReverseSolution) == this.Area(outRec2) > 0.0)
								{
									this.ReversePolyPtLinks(outRec2.Pts);
								}
							}
							else if (Clipper.Poly2ContainsPoly1(outRec.Pts, outRec2.Pts))
							{
								outRec2.IsHole = outRec.IsHole;
								outRec.IsHole = !outRec2.IsHole;
								outRec2.FirstLeft = outRec.FirstLeft;
								outRec.FirstLeft = outRec2;
								if (this.m_UsingPolyTree)
								{
									this.FixupFirstLefts2(outRec, outRec2);
								}
								if ((outRec.IsHole ^ this.ReverseSolution) == this.Area(outRec) > 0.0)
								{
									this.ReversePolyPtLinks(outRec.Pts);
								}
							}
							else
							{
								outRec2.IsHole = outRec.IsHole;
								outRec2.FirstLeft = outRec.FirstLeft;
								if (this.m_UsingPolyTree)
								{
									this.FixupFirstLefts1(outRec, outRec2);
								}
							}
						}
						else
						{
							outRec2.Pts = null;
							outRec2.BottomPt = null;
							outRec2.Idx = outRec.Idx;
							outRec.IsHole = outRec3.IsHole;
							if (outRec3 == outRec2)
							{
								outRec.FirstLeft = outRec2.FirstLeft;
							}
							outRec2.FirstLeft = outRec;
							if (this.m_UsingPolyTree)
							{
								this.FixupFirstLefts2(outRec2, outRec);
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
				if (outPt != null && !outRec.IsOpen)
				{
					do
					{
						for (OutPt outPt2 = outPt.Next; outPt2 != outRec.Pts; outPt2 = outPt2.Next)
						{
							if (outPt.Pt == outPt2.Pt && outPt2.Next != outPt && outPt2.Prev != outPt)
							{
								OutPt prev = outPt.Prev;
								OutPt prev2 = outPt2.Prev;
								outPt.Prev = prev2;
								prev2.Next = outPt;
								outPt2.Prev = prev;
								prev.Next = outPt2;
								outRec.Pts = outPt;
								OutRec outRec2 = this.CreateOutRec();
								outRec2.Pts = outPt2;
								this.UpdateOutPtIdxs(outRec2);
								if (Clipper.Poly2ContainsPoly1(outRec2.Pts, outRec.Pts))
								{
									outRec2.IsHole = !outRec.IsHole;
									outRec2.FirstLeft = outRec;
									if (this.m_UsingPolyTree)
									{
										this.FixupFirstLefts2(outRec2, outRec);
									}
								}
								else if (Clipper.Poly2ContainsPoly1(outRec.Pts, outRec2.Pts))
								{
									outRec2.IsHole = outRec.IsHole;
									outRec.IsHole = !outRec2.IsHole;
									outRec2.FirstLeft = outRec.FirstLeft;
									outRec.FirstLeft = outRec2;
									if (this.m_UsingPolyTree)
									{
										this.FixupFirstLefts2(outRec, outRec2);
									}
								}
								else
								{
									outRec2.IsHole = outRec.IsHole;
									outRec2.FirstLeft = outRec.FirstLeft;
									if (this.m_UsingPolyTree)
									{
										this.FixupFirstLefts1(outRec, outRec2);
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
			if (count < 3)
			{
				return 0.0;
			}
			double num = 0.0;
			int i = 0;
			int num2 = count - 1;
			while (i < count)
			{
				num += ((double)poly[num2].X + (double)poly[i].X) * ((double)poly[num2].Y - (double)poly[i].Y);
				num2 = i;
				i++;
			}
			return -num * 0.5;
		}

		private double Area(OutRec outRec)
		{
			OutPt outPt = outRec.Pts;
			if (outPt == null)
			{
				return 0.0;
			}
			double num = 0.0;
			do
			{
				num += (double)(outPt.Prev.Pt.X + outPt.Pt.X) * (double)(outPt.Prev.Pt.Y - outPt.Pt.Y);
				outPt = outPt.Next;
			}
			while (outPt != outRec.Pts);
			return num * 0.5;
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
			if (Math.Abs(pt1.X - pt2.X) > Math.Abs(pt1.Y - pt2.Y))
			{
				if (pt1.X > pt2.X == pt1.X < pt3.X)
				{
					return Clipper.DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
				}
				if (pt2.X > pt1.X == pt2.X < pt3.X)
				{
					return Clipper.DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd;
				}
				return Clipper.DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
			}
			else
			{
				if (pt1.Y > pt2.Y == pt1.Y < pt3.Y)
				{
					return Clipper.DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
				}
				if (pt2.Y > pt1.Y == pt2.Y < pt3.Y)
				{
					return Clipper.DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd;
				}
				return Clipper.DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
			}
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
			if (num == 0)
			{
				return new List<IntPoint>();
			}
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
				if (Clipper.PointsAreClose(outPt.Pt, outPt.Prev.Pt, num2))
				{
					outPt = Clipper.ExcludeOp(outPt);
					num--;
				}
				else if (Clipper.PointsAreClose(outPt.Prev.Pt, outPt.Next.Pt, num2))
				{
					Clipper.ExcludeOp(outPt.Next);
					outPt = Clipper.ExcludeOp(outPt);
					num -= 2;
				}
				else if (Clipper.SlopesNearCollinear(outPt.Prev.Pt, outPt.Pt, outPt.Next.Pt, num2))
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
			if (num < 3)
			{
				num = 0;
			}
			List<IntPoint> list = new List<IntPoint>(num);
			for (int k = 0; k < num; k++)
			{
				list.Add(outPt.Pt);
				outPt = outPt.Next;
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
					if (!Clipper.Orientation(list5))
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
				if (polynode.m_polygon.Count > 0 && flag)
				{
					paths.Add(polynode.m_polygon);
				}
				foreach (PolyNode polyNode in polynode.Childs)
				{
					Clipper.AddPolyNodeToPaths(polyNode, nt, paths);
				}
				return;
			}
		}

		public static List<List<IntPoint>> OpenPathsFromPolyTree(PolyTree polytree)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Capacity = polytree.ChildCount;
			for (int i = 0; i < polytree.ChildCount; i++)
			{
				if (polytree.Childs[i].IsOpen)
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

		private List<OutRec> m_PolyOuts;

		private ClipType m_ClipType;

		private Scanbeam m_Scanbeam;

		private TEdge m_ActiveEdges;

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
