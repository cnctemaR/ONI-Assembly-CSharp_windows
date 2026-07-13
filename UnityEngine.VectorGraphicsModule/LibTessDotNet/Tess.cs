using System;

namespace LibTessDotNet
{
	internal class Tess
	{
		private Tess.ActiveRegion RegionBelow(Tess.ActiveRegion reg)
		{
			return reg._nodeUp._prev._key;
		}

		private Tess.ActiveRegion RegionAbove(Tess.ActiveRegion reg)
		{
			return reg._nodeUp._next._key;
		}

		private bool EdgeLeq(Tess.ActiveRegion reg1, Tess.ActiveRegion reg2)
		{
			MeshUtils.Edge eUp = reg1._eUp;
			MeshUtils.Edge eUp2 = reg2._eUp;
			bool flag = eUp._Dst == this._event;
			bool flag4;
			if (flag)
			{
				bool flag2 = eUp2._Dst == this._event;
				if (flag2)
				{
					bool flag3 = Geom.VertLeq(eUp._Org, eUp2._Org);
					if (flag3)
					{
						flag4 = Geom.EdgeSign(eUp2._Dst, eUp._Org, eUp2._Org) <= 0f;
					}
					else
					{
						flag4 = Geom.EdgeSign(eUp._Dst, eUp2._Org, eUp._Org) >= 0f;
					}
				}
				else
				{
					flag4 = Geom.EdgeSign(eUp2._Dst, this._event, eUp2._Org) <= 0f;
				}
			}
			else
			{
				bool flag5 = eUp2._Dst == this._event;
				if (flag5)
				{
					flag4 = Geom.EdgeSign(eUp._Dst, this._event, eUp._Org) >= 0f;
				}
				else
				{
					float num = Geom.EdgeEval(eUp._Dst, this._event, eUp._Org);
					float num2 = Geom.EdgeEval(eUp2._Dst, this._event, eUp2._Org);
					flag4 = num >= num2;
				}
			}
			return flag4;
		}

		private void DeleteRegion(Tess.ActiveRegion reg)
		{
			bool fixUpperEdge = reg._fixUpperEdge;
			if (fixUpperEdge)
			{
			}
			reg._eUp._activeRegion = null;
			this._dict.Remove(reg._nodeUp);
		}

		private void FixUpperEdge(Tess.ActiveRegion reg, MeshUtils.Edge newEdge)
		{
			this._mesh.Delete(reg._eUp);
			reg._fixUpperEdge = false;
			reg._eUp = newEdge;
			newEdge._activeRegion = reg;
		}

		private Tess.ActiveRegion TopLeftRegion(Tess.ActiveRegion reg)
		{
			MeshUtils.Vertex org = reg._eUp._Org;
			do
			{
				reg = this.RegionAbove(reg);
			}
			while (reg._eUp._Org == org);
			bool fixUpperEdge = reg._fixUpperEdge;
			if (fixUpperEdge)
			{
				MeshUtils.Edge edge = this._mesh.Connect(this.RegionBelow(reg)._eUp._Sym, reg._eUp._Lnext);
				this.FixUpperEdge(reg, edge);
				reg = this.RegionAbove(reg);
			}
			return reg;
		}

		private Tess.ActiveRegion TopRightRegion(Tess.ActiveRegion reg)
		{
			MeshUtils.Vertex dst = reg._eUp._Dst;
			do
			{
				reg = this.RegionAbove(reg);
			}
			while (reg._eUp._Dst == dst);
			return reg;
		}

		private Tess.ActiveRegion AddRegionBelow(Tess.ActiveRegion regAbove, MeshUtils.Edge eNewUp)
		{
			Tess.ActiveRegion activeRegion = new Tess.ActiveRegion();
			activeRegion._eUp = eNewUp;
			activeRegion._nodeUp = this._dict.InsertBefore(regAbove._nodeUp, activeRegion);
			activeRegion._fixUpperEdge = false;
			activeRegion._sentinel = false;
			activeRegion._dirty = false;
			eNewUp._activeRegion = activeRegion;
			return activeRegion;
		}

		private void ComputeWinding(Tess.ActiveRegion reg)
		{
			reg._windingNumber = this.RegionAbove(reg)._windingNumber + reg._eUp._winding;
			reg._inside = Geom.IsWindingInside(this._windingRule, reg._windingNumber);
		}

		private void FinishRegion(Tess.ActiveRegion reg)
		{
			MeshUtils.Edge eUp = reg._eUp;
			MeshUtils.Face lface = eUp._Lface;
			lface._inside = reg._inside;
			lface._anEdge = eUp;
			this.DeleteRegion(reg);
		}

		private MeshUtils.Edge FinishLeftRegions(Tess.ActiveRegion regFirst, Tess.ActiveRegion regLast)
		{
			Tess.ActiveRegion activeRegion = regFirst;
			MeshUtils.Edge edge = regFirst._eUp;
			while (activeRegion != regLast)
			{
				activeRegion._fixUpperEdge = false;
				Tess.ActiveRegion activeRegion2 = this.RegionBelow(activeRegion);
				MeshUtils.Edge edge2 = activeRegion2._eUp;
				bool flag = edge2._Org != edge._Org;
				if (flag)
				{
					bool flag2 = !activeRegion2._fixUpperEdge;
					if (flag2)
					{
						this.FinishRegion(activeRegion);
						break;
					}
					edge2 = this._mesh.Connect(edge._Lprev, edge2._Sym);
					this.FixUpperEdge(activeRegion2, edge2);
				}
				bool flag3 = edge._Onext != edge2;
				if (flag3)
				{
					this._mesh.Splice(edge2._Oprev, edge2);
					this._mesh.Splice(edge, edge2);
				}
				this.FinishRegion(activeRegion);
				edge = activeRegion2._eUp;
				activeRegion = activeRegion2;
			}
			return edge;
		}

		private void AddRightEdges(Tess.ActiveRegion regUp, MeshUtils.Edge eFirst, MeshUtils.Edge eLast, MeshUtils.Edge eTopLeft, bool cleanUp)
		{
			bool flag = true;
			MeshUtils.Edge edge = eFirst;
			do
			{
				this.AddRegionBelow(regUp, edge._Sym);
				edge = edge._Onext;
			}
			while (edge != eLast);
			bool flag2 = eTopLeft == null;
			if (flag2)
			{
				eTopLeft = this.RegionBelow(regUp)._eUp._Rprev;
			}
			Tess.ActiveRegion activeRegion = regUp;
			MeshUtils.Edge edge2 = eTopLeft;
			for (;;)
			{
				Tess.ActiveRegion activeRegion2 = this.RegionBelow(activeRegion);
				edge = activeRegion2._eUp._Sym;
				bool flag3 = edge._Org != edge2._Org;
				if (flag3)
				{
					break;
				}
				bool flag4 = edge._Onext != edge2;
				if (flag4)
				{
					this._mesh.Splice(edge._Oprev, edge);
					this._mesh.Splice(edge2._Oprev, edge);
				}
				activeRegion2._windingNumber = activeRegion._windingNumber - edge._winding;
				activeRegion2._inside = Geom.IsWindingInside(this._windingRule, activeRegion2._windingNumber);
				activeRegion._dirty = true;
				bool flag5 = !flag && this.CheckForRightSplice(activeRegion);
				if (flag5)
				{
					Geom.AddWinding(edge, edge2);
					this.DeleteRegion(activeRegion);
					this._mesh.Delete(edge2);
				}
				flag = false;
				activeRegion = activeRegion2;
				edge2 = edge;
			}
			activeRegion._dirty = true;
			if (cleanUp)
			{
				this.WalkDirtyRegions(activeRegion);
			}
		}

		private void SpliceMergeVertices(MeshUtils.Edge e1, MeshUtils.Edge e2)
		{
			this._mesh.Splice(e1, e2);
		}

		private void VertexWeights(MeshUtils.Vertex isect, MeshUtils.Vertex org, MeshUtils.Vertex dst, out float w0, out float w1)
		{
			float num = Geom.VertL1dist(org, isect);
			float num2 = Geom.VertL1dist(dst, isect);
			w0 = num2 / (num + num2) / 2f;
			w1 = num / (num + num2) / 2f;
			isect._coords.X = isect._coords.X + (w0 * org._coords.X + w1 * dst._coords.X);
			isect._coords.Y = isect._coords.Y + (w0 * org._coords.Y + w1 * dst._coords.Y);
			isect._coords.Z = isect._coords.Z + (w0 * org._coords.Z + w1 * dst._coords.Z);
		}

		private void GetIntersectData(MeshUtils.Vertex isect, MeshUtils.Vertex orgUp, MeshUtils.Vertex dstUp, MeshUtils.Vertex orgLo, MeshUtils.Vertex dstLo)
		{
			isect._coords = Vec3.Zero;
			float num;
			float num2;
			this.VertexWeights(isect, orgUp, dstUp, out num, out num2);
			float num3;
			float num4;
			this.VertexWeights(isect, orgLo, dstLo, out num3, out num4);
			bool flag = this._combineCallback != null;
			if (flag)
			{
				isect._data = this._combineCallback(isect._coords, new object[] { orgUp._data, dstUp._data, orgLo._data, dstLo._data }, new float[] { num, num2, num3, num4 });
			}
		}

		private bool CheckForRightSplice(Tess.ActiveRegion regUp)
		{
			Tess.ActiveRegion activeRegion = this.RegionBelow(regUp);
			MeshUtils.Edge eUp = regUp._eUp;
			MeshUtils.Edge eUp2 = activeRegion._eUp;
			bool flag = Geom.VertLeq(eUp._Org, eUp2._Org);
			if (flag)
			{
				bool flag2 = Geom.EdgeSign(eUp2._Dst, eUp._Org, eUp2._Org) > 0f;
				if (flag2)
				{
					return false;
				}
				bool flag3 = !Geom.VertEq(eUp._Org, eUp2._Org);
				if (flag3)
				{
					this._mesh.SplitEdge(eUp2._Sym);
					this._mesh.Splice(eUp, eUp2._Oprev);
					regUp._dirty = (activeRegion._dirty = true);
				}
				else
				{
					bool flag4 = eUp._Org != eUp2._Org;
					if (flag4)
					{
						this._pq.Remove(eUp._Org._pqHandle);
						this.SpliceMergeVertices(eUp2._Oprev, eUp);
					}
				}
			}
			else
			{
				bool flag5 = Geom.EdgeSign(eUp._Dst, eUp2._Org, eUp._Org) < 0f;
				if (flag5)
				{
					return false;
				}
				this.RegionAbove(regUp)._dirty = (regUp._dirty = true);
				this._mesh.SplitEdge(eUp._Sym);
				this._mesh.Splice(eUp2._Oprev, eUp);
			}
			return true;
		}

		private bool CheckForLeftSplice(Tess.ActiveRegion regUp)
		{
			Tess.ActiveRegion activeRegion = this.RegionBelow(regUp);
			MeshUtils.Edge eUp = regUp._eUp;
			MeshUtils.Edge eUp2 = activeRegion._eUp;
			bool flag = Geom.VertLeq(eUp._Dst, eUp2._Dst);
			if (flag)
			{
				bool flag2 = Geom.EdgeSign(eUp._Dst, eUp2._Dst, eUp._Org) < 0f;
				if (flag2)
				{
					return false;
				}
				this.RegionAbove(regUp)._dirty = (regUp._dirty = true);
				MeshUtils.Edge edge = this._mesh.SplitEdge(eUp);
				this._mesh.Splice(eUp2._Sym, edge);
				edge._Lface._inside = regUp._inside;
			}
			else
			{
				bool flag3 = Geom.EdgeSign(eUp2._Dst, eUp._Dst, eUp2._Org) > 0f;
				if (flag3)
				{
					return false;
				}
				regUp._dirty = (activeRegion._dirty = true);
				MeshUtils.Edge edge2 = this._mesh.SplitEdge(eUp2);
				this._mesh.Splice(eUp._Lnext, eUp2._Sym);
				edge2._Rface._inside = regUp._inside;
			}
			return true;
		}

		private bool CheckForIntersect(Tess.ActiveRegion regUp)
		{
			Tess.ActiveRegion activeRegion = this.RegionBelow(regUp);
			MeshUtils.Edge edge = regUp._eUp;
			MeshUtils.Edge edge2 = activeRegion._eUp;
			MeshUtils.Vertex org = edge._Org;
			MeshUtils.Vertex org2 = edge2._Org;
			MeshUtils.Vertex dst = edge._Dst;
			MeshUtils.Vertex dst2 = edge2._Dst;
			bool flag = org == org2;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				float num = Math.Min(org._t, dst._t);
				float num2 = Math.Max(org2._t, dst2._t);
				bool flag3 = num > num2;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					bool flag4 = Geom.VertLeq(org, org2);
					if (flag4)
					{
						bool flag5 = Geom.EdgeSign(dst2, org, org2) > 0f;
						if (flag5)
						{
							return false;
						}
					}
					else
					{
						bool flag6 = Geom.EdgeSign(dst, org2, org) < 0f;
						if (flag6)
						{
							return false;
						}
					}
					MeshUtils.Vertex vertex = MeshUtils.Pooled<MeshUtils.Vertex>.Create();
					Geom.EdgeIntersect(dst, org, dst2, org2, vertex);
					bool flag7 = Geom.VertLeq(vertex, this._event);
					if (flag7)
					{
						vertex._s = this._event._s;
						vertex._t = this._event._t;
					}
					MeshUtils.Vertex vertex2 = (Geom.VertLeq(org, org2) ? org : org2);
					bool flag8 = Geom.VertLeq(vertex2, vertex);
					if (flag8)
					{
						vertex._s = vertex2._s;
						vertex._t = vertex2._t;
					}
					bool flag9 = Geom.VertEq(vertex, org) || Geom.VertEq(vertex, org2);
					if (flag9)
					{
						this.CheckForRightSplice(regUp);
						flag2 = false;
					}
					else
					{
						bool flag10 = (!Geom.VertEq(dst, this._event) && Geom.EdgeSign(dst, this._event, vertex) >= 0f) || (!Geom.VertEq(dst2, this._event) && Geom.EdgeSign(dst2, this._event, vertex) <= 0f);
						if (flag10)
						{
							bool flag11 = dst2 == this._event;
							if (flag11)
							{
								this._mesh.SplitEdge(edge._Sym);
								this._mesh.Splice(edge2._Sym, edge);
								regUp = this.TopLeftRegion(regUp);
								edge = this.RegionBelow(regUp)._eUp;
								this.FinishLeftRegions(this.RegionBelow(regUp), activeRegion);
								this.AddRightEdges(regUp, edge._Oprev, edge, edge, true);
								flag2 = true;
							}
							else
							{
								bool flag12 = dst == this._event;
								if (flag12)
								{
									this._mesh.SplitEdge(edge2._Sym);
									this._mesh.Splice(edge._Lnext, edge2._Oprev);
									activeRegion = regUp;
									regUp = this.TopRightRegion(regUp);
									MeshUtils.Edge rprev = this.RegionBelow(regUp)._eUp._Rprev;
									activeRegion._eUp = edge2._Oprev;
									edge2 = this.FinishLeftRegions(activeRegion, null);
									this.AddRightEdges(regUp, edge2._Onext, edge._Rprev, rprev, true);
									flag2 = true;
								}
								else
								{
									bool flag13 = Geom.EdgeSign(dst, this._event, vertex) >= 0f;
									if (flag13)
									{
										this.RegionAbove(regUp)._dirty = (regUp._dirty = true);
										this._mesh.SplitEdge(edge._Sym);
										edge._Org._s = this._event._s;
										edge._Org._t = this._event._t;
									}
									bool flag14 = Geom.EdgeSign(dst2, this._event, vertex) <= 0f;
									if (flag14)
									{
										regUp._dirty = (activeRegion._dirty = true);
										this._mesh.SplitEdge(edge2._Sym);
										edge2._Org._s = this._event._s;
										edge2._Org._t = this._event._t;
									}
									flag2 = false;
								}
							}
						}
						else
						{
							this._mesh.SplitEdge(edge._Sym);
							this._mesh.SplitEdge(edge2._Sym);
							this._mesh.Splice(edge2._Oprev, edge);
							edge._Org._s = vertex._s;
							edge._Org._t = vertex._t;
							edge._Org._pqHandle = this._pq.Insert(edge._Org);
							bool flag15 = edge._Org._pqHandle._handle == PQHandle.Invalid;
							if (flag15)
							{
								throw new InvalidOperationException("PQHandle should not be invalid");
							}
							this.GetIntersectData(edge._Org, org, dst, org2, dst2);
							this.RegionAbove(regUp)._dirty = (regUp._dirty = (activeRegion._dirty = true));
							flag2 = false;
						}
					}
				}
			}
			return flag2;
		}

		private void WalkDirtyRegions(Tess.ActiveRegion regUp)
		{
			Tess.ActiveRegion activeRegion = this.RegionBelow(regUp);
			for (;;)
			{
				while (activeRegion._dirty)
				{
					regUp = activeRegion;
					activeRegion = this.RegionBelow(activeRegion);
				}
				bool flag = !regUp._dirty;
				if (flag)
				{
					activeRegion = regUp;
					regUp = this.RegionAbove(regUp);
					bool flag2 = regUp == null || !regUp._dirty;
					if (flag2)
					{
						break;
					}
				}
				regUp._dirty = false;
				MeshUtils.Edge edge = regUp._eUp;
				MeshUtils.Edge edge2 = activeRegion._eUp;
				bool flag3 = edge._Dst != edge2._Dst;
				if (flag3)
				{
					bool flag4 = this.CheckForLeftSplice(regUp);
					if (flag4)
					{
						bool fixUpperEdge = activeRegion._fixUpperEdge;
						if (fixUpperEdge)
						{
							this.DeleteRegion(activeRegion);
							this._mesh.Delete(edge2);
							activeRegion = this.RegionBelow(regUp);
							edge2 = activeRegion._eUp;
						}
						else
						{
							bool fixUpperEdge2 = regUp._fixUpperEdge;
							if (fixUpperEdge2)
							{
								this.DeleteRegion(regUp);
								this._mesh.Delete(edge);
								regUp = this.RegionAbove(activeRegion);
								edge = regUp._eUp;
							}
						}
					}
				}
				bool flag5 = edge._Org != edge2._Org;
				if (flag5)
				{
					bool flag6 = edge._Dst != edge2._Dst && !regUp._fixUpperEdge && !activeRegion._fixUpperEdge && (edge._Dst == this._event || edge2._Dst == this._event);
					if (flag6)
					{
						bool flag7 = this.CheckForIntersect(regUp);
						if (flag7)
						{
							break;
						}
					}
					else
					{
						this.CheckForRightSplice(regUp);
					}
				}
				bool flag8 = edge._Org == edge2._Org && edge._Dst == edge2._Dst;
				if (flag8)
				{
					Geom.AddWinding(edge2, edge);
					this.DeleteRegion(regUp);
					this._mesh.Delete(edge);
					regUp = this.RegionAbove(activeRegion);
				}
			}
		}

		private void ConnectRightVertex(Tess.ActiveRegion regUp, MeshUtils.Edge eBottomLeft)
		{
			MeshUtils.Edge edge = eBottomLeft._Onext;
			Tess.ActiveRegion activeRegion = this.RegionBelow(regUp);
			MeshUtils.Edge eUp = regUp._eUp;
			MeshUtils.Edge eUp2 = activeRegion._eUp;
			bool flag = false;
			bool flag2 = eUp._Dst != eUp2._Dst;
			if (flag2)
			{
				this.CheckForIntersect(regUp);
			}
			bool flag3 = Geom.VertEq(eUp._Org, this._event);
			if (flag3)
			{
				this._mesh.Splice(edge._Oprev, eUp);
				regUp = this.TopLeftRegion(regUp);
				edge = this.RegionBelow(regUp)._eUp;
				this.FinishLeftRegions(this.RegionBelow(regUp), activeRegion);
				flag = true;
			}
			bool flag4 = Geom.VertEq(eUp2._Org, this._event);
			if (flag4)
			{
				this._mesh.Splice(eBottomLeft, eUp2._Oprev);
				eBottomLeft = this.FinishLeftRegions(activeRegion, null);
				flag = true;
			}
			bool flag5 = flag;
			if (flag5)
			{
				this.AddRightEdges(regUp, eBottomLeft._Onext, edge, edge, true);
			}
			else
			{
				bool flag6 = Geom.VertLeq(eUp2._Org, eUp._Org);
				MeshUtils.Edge edge2;
				if (flag6)
				{
					edge2 = eUp2._Oprev;
				}
				else
				{
					edge2 = eUp;
				}
				edge2 = this._mesh.Connect(eBottomLeft._Lprev, edge2);
				this.AddRightEdges(regUp, edge2, edge2._Onext, edge2._Onext, false);
				edge2._Sym._activeRegion._fixUpperEdge = true;
				this.WalkDirtyRegions(regUp);
			}
		}

		private void ConnectLeftDegenerate(Tess.ActiveRegion regUp, MeshUtils.Vertex vEvent)
		{
			MeshUtils.Edge eUp = regUp._eUp;
			bool flag = Geom.VertEq(eUp._Org, vEvent);
			if (flag)
			{
				throw new InvalidOperationException("Vertices should have been merged before");
			}
			bool flag2 = !Geom.VertEq(eUp._Dst, vEvent);
			if (flag2)
			{
				this._mesh.SplitEdge(eUp._Sym);
				bool fixUpperEdge = regUp._fixUpperEdge;
				if (fixUpperEdge)
				{
					this._mesh.Delete(eUp._Onext);
					regUp._fixUpperEdge = false;
				}
				this._mesh.Splice(vEvent._anEdge, eUp);
				this.SweepEvent(vEvent);
				return;
			}
			throw new InvalidOperationException("Vertices should have been merged before");
		}

		private void ConnectLeftVertex(MeshUtils.Vertex vEvent)
		{
			Tess.ActiveRegion activeRegion = new Tess.ActiveRegion();
			activeRegion._eUp = vEvent._anEdge._Sym;
			Tess.ActiveRegion key = this._dict.Find(activeRegion).Key;
			Tess.ActiveRegion activeRegion2 = this.RegionBelow(key);
			bool flag = activeRegion2 == null;
			if (!flag)
			{
				MeshUtils.Edge eUp = key._eUp;
				MeshUtils.Edge eUp2 = activeRegion2._eUp;
				bool flag2 = Geom.EdgeSign(eUp._Dst, vEvent, eUp._Org) == 0f;
				if (flag2)
				{
					this.ConnectLeftDegenerate(key, vEvent);
				}
				else
				{
					Tess.ActiveRegion activeRegion3 = (Geom.VertLeq(eUp2._Dst, eUp._Dst) ? key : activeRegion2);
					bool flag3 = key._inside || activeRegion3._fixUpperEdge;
					if (flag3)
					{
						bool flag4 = activeRegion3 == key;
						MeshUtils.Edge edge;
						if (flag4)
						{
							edge = this._mesh.Connect(vEvent._anEdge._Sym, eUp._Lnext);
						}
						else
						{
							edge = this._mesh.Connect(eUp2._Dnext, vEvent._anEdge)._Sym;
						}
						bool fixUpperEdge = activeRegion3._fixUpperEdge;
						if (fixUpperEdge)
						{
							this.FixUpperEdge(activeRegion3, edge);
						}
						else
						{
							this.ComputeWinding(this.AddRegionBelow(key, edge));
						}
						this.SweepEvent(vEvent);
					}
					else
					{
						this.AddRightEdges(key, vEvent._anEdge, vEvent._anEdge, null, true);
					}
				}
			}
		}

		private void SweepEvent(MeshUtils.Vertex vEvent)
		{
			this._event = vEvent;
			MeshUtils.Edge edge = vEvent._anEdge;
			while (edge._activeRegion == null)
			{
				edge = edge._Onext;
				bool flag = edge == vEvent._anEdge;
				if (flag)
				{
					this.ConnectLeftVertex(vEvent);
					return;
				}
			}
			Tess.ActiveRegion activeRegion = this.TopLeftRegion(edge._activeRegion);
			Tess.ActiveRegion activeRegion2 = this.RegionBelow(activeRegion);
			MeshUtils.Edge eUp = activeRegion2._eUp;
			MeshUtils.Edge edge2 = this.FinishLeftRegions(activeRegion2, null);
			bool flag2 = edge2._Onext == eUp;
			if (flag2)
			{
				this.ConnectRightVertex(activeRegion, edge2);
				return;
			}
			this.AddRightEdges(activeRegion, edge2._Onext, eUp, eUp, true);
		}

		private void AddSentinel(float smin, float smax, float t)
		{
			MeshUtils.Edge edge = this._mesh.MakeEdge();
			edge._Org._s = smax;
			edge._Org._t = t;
			edge._Dst._s = smin;
			edge._Dst._t = t;
			this._event = edge._Dst;
			Tess.ActiveRegion activeRegion = new Tess.ActiveRegion();
			activeRegion._eUp = edge;
			activeRegion._windingNumber = 0;
			activeRegion._inside = false;
			activeRegion._fixUpperEdge = false;
			activeRegion._sentinel = true;
			activeRegion._dirty = false;
			activeRegion._nodeUp = this._dict.Insert(activeRegion);
		}

		private void InitEdgeDict()
		{
			this._dict = new Dict<Tess.ActiveRegion>(new Dict<Tess.ActiveRegion>.LessOrEqual(this.EdgeLeq));
			this.AddSentinel(-this.SentinelCoord, this.SentinelCoord, -this.SentinelCoord);
			this.AddSentinel(-this.SentinelCoord, this.SentinelCoord, this.SentinelCoord);
		}

		private void DoneEdgeDict()
		{
			Tess.ActiveRegion key;
			while ((key = this._dict.Min().Key) != null)
			{
				bool flag = !key._sentinel;
				if (flag)
				{
				}
				this.DeleteRegion(key);
			}
			this._dict = null;
		}

		private void RemoveDegenerateEdges()
		{
			MeshUtils.Edge eHead = this._mesh._eHead;
			MeshUtils.Edge edge2;
			for (MeshUtils.Edge edge = eHead._next; edge != eHead; edge = edge2)
			{
				edge2 = edge._next;
				MeshUtils.Edge edge3 = edge._Lnext;
				bool flag = Geom.VertEq(edge._Org, edge._Dst) && edge._Lnext._Lnext != edge;
				if (flag)
				{
					this.SpliceMergeVertices(edge3, edge);
					this._mesh.Delete(edge);
					edge = edge3;
					edge3 = edge._Lnext;
				}
				bool flag2 = edge3._Lnext == edge;
				if (flag2)
				{
					bool flag3 = edge3 != edge;
					if (flag3)
					{
						bool flag4 = edge3 == edge2 || edge3 == edge2._Sym;
						if (flag4)
						{
							edge2 = edge2._next;
						}
						this._mesh.Delete(edge3);
					}
					bool flag5 = edge == edge2 || edge == edge2._Sym;
					if (flag5)
					{
						edge2 = edge2._next;
					}
					this._mesh.Delete(edge);
				}
			}
		}

		private void InitPriorityQ()
		{
			MeshUtils.Vertex vertex = this._mesh._vHead;
			int num = 0;
			for (MeshUtils.Vertex vertex2 = vertex._next; vertex2 != vertex; vertex2 = vertex2._next)
			{
				num++;
			}
			num += 8;
			this._pq = new PriorityQueue<MeshUtils.Vertex>(num, new PriorityHeap<MeshUtils.Vertex>.LessOrEqual(Geom.VertLeq));
			vertex = this._mesh._vHead;
			for (MeshUtils.Vertex vertex2 = vertex._next; vertex2 != vertex; vertex2 = vertex2._next)
			{
				vertex2._pqHandle = this._pq.Insert(vertex2);
				bool flag = vertex2._pqHandle._handle == PQHandle.Invalid;
				if (flag)
				{
					throw new InvalidOperationException("PQHandle should not be invalid");
				}
			}
			this._pq.Init();
		}

		private void DonePriorityQ()
		{
			this._pq = null;
		}

		private void RemoveDegenerateFaces()
		{
			MeshUtils.Face next;
			for (MeshUtils.Face face = this._mesh._fHead._next; face != this._mesh._fHead; face = next)
			{
				next = face._next;
				MeshUtils.Edge anEdge = face._anEdge;
				bool flag = anEdge._Lnext._Lnext == anEdge;
				if (flag)
				{
					Geom.AddWinding(anEdge._Onext, anEdge);
					this._mesh.Delete(anEdge);
				}
			}
		}

		protected void ComputeInterior()
		{
			this.RemoveDegenerateEdges();
			this.InitPriorityQ();
			this.RemoveDegenerateFaces();
			this.InitEdgeDict();
			MeshUtils.Vertex vertex;
			while ((vertex = this._pq.ExtractMin()) != null)
			{
				for (;;)
				{
					MeshUtils.Vertex vertex2 = this._pq.Minimum();
					bool flag = vertex2 == null || !Geom.VertEq(vertex2, vertex);
					if (flag)
					{
						break;
					}
					vertex2 = this._pq.ExtractMin();
					this.SpliceMergeVertices(vertex._anEdge, vertex2._anEdge);
				}
				this.SweepEvent(vertex);
			}
			this.DoneEdgeDict();
			this.DonePriorityQ();
			this.RemoveDegenerateFaces();
		}

		public Vec3 Normal
		{
			get
			{
				return this._normal;
			}
			set
			{
				this._normal = value;
			}
		}

		public ContourVertex[] Vertices
		{
			get
			{
				return this._vertices;
			}
		}

		public int VertexCount
		{
			get
			{
				return this._vertexCount;
			}
		}

		public int[] Elements
		{
			get
			{
				return this._elements;
			}
		}

		public int ElementCount
		{
			get
			{
				return this._elementCount;
			}
		}

		public Tess()
		{
			this._normal = Vec3.Zero;
			this._bminX = (this._bminY = (this._bmaxX = (this._bmaxY = 0f)));
			this._windingRule = WindingRule.EvenOdd;
			this._mesh = null;
			this._vertices = null;
			this._vertexCount = 0;
			this._elements = null;
			this._elementCount = 0;
		}

		private void ComputeNormal(ref Vec3 norm)
		{
			MeshUtils.Vertex vertex = this._mesh._vHead._next;
			float[] array = new float[]
			{
				vertex._coords.X,
				vertex._coords.Y,
				vertex._coords.Z
			};
			MeshUtils.Vertex[] array2 = new MeshUtils.Vertex[] { vertex, vertex, vertex };
			float[] array3 = new float[]
			{
				vertex._coords.X,
				vertex._coords.Y,
				vertex._coords.Z
			};
			MeshUtils.Vertex[] array4 = new MeshUtils.Vertex[] { vertex, vertex, vertex };
			while (vertex != this._mesh._vHead)
			{
				bool flag = vertex._coords.X < array[0];
				if (flag)
				{
					array[0] = vertex._coords.X;
					array2[0] = vertex;
				}
				bool flag2 = vertex._coords.Y < array[1];
				if (flag2)
				{
					array[1] = vertex._coords.Y;
					array2[1] = vertex;
				}
				bool flag3 = vertex._coords.Z < array[2];
				if (flag3)
				{
					array[2] = vertex._coords.Z;
					array2[2] = vertex;
				}
				bool flag4 = vertex._coords.X > array3[0];
				if (flag4)
				{
					array3[0] = vertex._coords.X;
					array4[0] = vertex;
				}
				bool flag5 = vertex._coords.Y > array3[1];
				if (flag5)
				{
					array3[1] = vertex._coords.Y;
					array4[1] = vertex;
				}
				bool flag6 = vertex._coords.Z > array3[2];
				if (flag6)
				{
					array3[2] = vertex._coords.Z;
					array4[2] = vertex;
				}
				vertex = vertex._next;
			}
			int num = 0;
			bool flag7 = array3[1] - array[1] > array3[0] - array[0];
			if (flag7)
			{
				num = 1;
			}
			bool flag8 = array3[2] - array[2] > array3[num] - array[num];
			if (flag8)
			{
				num = 2;
			}
			bool flag9 = array[num] >= array3[num];
			if (flag9)
			{
				norm = new Vec3
				{
					X = 0f,
					Y = 0f,
					Z = 1f
				};
			}
			else
			{
				float num2 = 0f;
				MeshUtils.Vertex vertex2 = array2[num];
				MeshUtils.Vertex vertex3 = array4[num];
				Vec3 vec;
				Vec3.Sub(ref vertex2._coords, ref vertex3._coords, out vec);
				for (vertex = this._mesh._vHead._next; vertex != this._mesh._vHead; vertex = vertex._next)
				{
					Vec3 vec2;
					Vec3.Sub(ref vertex._coords, ref vertex3._coords, out vec2);
					Vec3 vec3;
					vec3.X = vec.Y * vec2.Z - vec.Z * vec2.Y;
					vec3.Y = vec.Z * vec2.X - vec.X * vec2.Z;
					vec3.Z = vec.X * vec2.Y - vec.Y * vec2.X;
					float num3 = vec3.X * vec3.X + vec3.Y * vec3.Y + vec3.Z * vec3.Z;
					bool flag10 = num3 > num2;
					if (flag10)
					{
						num2 = num3;
						norm = vec3;
					}
				}
				bool flag11 = num2 <= 0f;
				if (flag11)
				{
					norm = Vec3.Zero;
					num = Vec3.LongAxis(ref vec);
					norm[num] = 1f;
				}
			}
		}

		private void CheckOrientation()
		{
			float num = 0f;
			for (MeshUtils.Face face = this._mesh._fHead._next; face != this._mesh._fHead; face = face._next)
			{
				bool flag = face._anEdge._winding <= 0;
				if (!flag)
				{
					num += MeshUtils.FaceArea(face);
				}
			}
			bool flag2 = num < 0f;
			if (flag2)
			{
				for (MeshUtils.Vertex vertex = this._mesh._vHead._next; vertex != this._mesh._vHead; vertex = vertex._next)
				{
					vertex._t = -vertex._t;
				}
				Vec3.Neg(ref this._tUnit);
			}
		}

		private void ProjectPolygon()
		{
			Vec3 normal = this._normal;
			bool flag = false;
			bool flag2 = normal.X == 0f && normal.Y == 0f && normal.Z == 0f;
			if (flag2)
			{
				this.ComputeNormal(ref normal);
				this._normal = normal;
				flag = true;
			}
			int num = Vec3.LongAxis(ref normal);
			this._sUnit[num] = 0f;
			this._sUnit[(num + 1) % 3] = this.SUnitX;
			this._sUnit[(num + 2) % 3] = this.SUnitY;
			this._tUnit[num] = 0f;
			this._tUnit[(num + 1) % 3] = ((normal[num] > 0f) ? (-this.SUnitY) : this.SUnitY);
			this._tUnit[(num + 2) % 3] = ((normal[num] > 0f) ? this.SUnitX : (-this.SUnitX));
			for (MeshUtils.Vertex vertex = this._mesh._vHead._next; vertex != this._mesh._vHead; vertex = vertex._next)
			{
				Vec3.Dot(ref vertex._coords, ref this._sUnit, out vertex._s);
				Vec3.Dot(ref vertex._coords, ref this._tUnit, out vertex._t);
			}
			bool flag3 = flag;
			if (flag3)
			{
				this.CheckOrientation();
			}
			bool flag4 = true;
			for (MeshUtils.Vertex vertex2 = this._mesh._vHead._next; vertex2 != this._mesh._vHead; vertex2 = vertex2._next)
			{
				bool flag5 = flag4;
				if (flag5)
				{
					this._bminX = (this._bmaxX = vertex2._s);
					this._bminY = (this._bmaxY = vertex2._t);
					flag4 = false;
				}
				else
				{
					bool flag6 = vertex2._s < this._bminX;
					if (flag6)
					{
						this._bminX = vertex2._s;
					}
					bool flag7 = vertex2._s > this._bmaxX;
					if (flag7)
					{
						this._bmaxX = vertex2._s;
					}
					bool flag8 = vertex2._t < this._bminY;
					if (flag8)
					{
						this._bminY = vertex2._t;
					}
					bool flag9 = vertex2._t > this._bmaxY;
					if (flag9)
					{
						this._bmaxY = vertex2._t;
					}
				}
			}
		}

		private void TessellateMonoRegion(MeshUtils.Face face)
		{
			MeshUtils.Edge edge = face._anEdge;
			while (Geom.VertLeq(edge._Dst, edge._Org))
			{
				edge = edge._Lprev;
			}
			while (Geom.VertLeq(edge._Org, edge._Dst))
			{
				edge = edge._Lnext;
			}
			MeshUtils.Edge edge2 = edge._Lprev;
			while (edge._Lnext != edge2)
			{
				bool flag = Geom.VertLeq(edge._Dst, edge2._Org);
				if (flag)
				{
					while (edge2._Lnext != edge && (Geom.EdgeGoesLeft(edge2._Lnext) || Geom.EdgeSign(edge2._Org, edge2._Dst, edge2._Lnext._Dst) <= 0f))
					{
						edge2 = this._mesh.Connect(edge2._Lnext, edge2)._Sym;
					}
					edge2 = edge2._Lprev;
				}
				else
				{
					while (edge2._Lnext != edge && (Geom.EdgeGoesRight(edge._Lprev) || Geom.EdgeSign(edge._Dst, edge._Org, edge._Lprev._Org) >= 0f))
					{
						edge = this._mesh.Connect(edge, edge._Lprev)._Sym;
					}
					edge = edge._Lnext;
				}
			}
			while (edge2._Lnext._Lnext != edge)
			{
				edge2 = this._mesh.Connect(edge2._Lnext, edge2)._Sym;
			}
		}

		private void TessellateInterior()
		{
			MeshUtils.Face next;
			for (MeshUtils.Face face = this._mesh._fHead._next; face != this._mesh._fHead; face = next)
			{
				next = face._next;
				bool inside = face._inside;
				if (inside)
				{
					this.TessellateMonoRegion(face);
				}
			}
		}

		private void DiscardExterior()
		{
			MeshUtils.Face next;
			for (MeshUtils.Face face = this._mesh._fHead._next; face != this._mesh._fHead; face = next)
			{
				next = face._next;
				bool flag = !face._inside;
				if (flag)
				{
					this._mesh.ZapFace(face);
				}
			}
		}

		private void SetWindingNumber(int value, bool keepOnlyBoundary)
		{
			MeshUtils.Edge next;
			for (MeshUtils.Edge edge = this._mesh._eHead._next; edge != this._mesh._eHead; edge = next)
			{
				next = edge._next;
				bool flag = edge._Rface._inside != edge._Lface._inside;
				if (flag)
				{
					edge._winding = (edge._Lface._inside ? value : (-value));
				}
				else
				{
					bool flag2 = !keepOnlyBoundary;
					if (flag2)
					{
						edge._winding = 0;
					}
					else
					{
						this._mesh.Delete(edge);
					}
				}
			}
		}

		private int GetNeighbourFace(MeshUtils.Edge edge)
		{
			bool flag = edge._Rface == null;
			int num;
			if (flag)
			{
				num = -1;
			}
			else
			{
				bool flag2 = !edge._Rface._inside;
				if (flag2)
				{
					num = -1;
				}
				else
				{
					num = edge._Rface._n;
				}
			}
			return num;
		}

		private void OutputPolymesh(ElementType elementType, int polySize)
		{
			int num = 0;
			int num2 = 0;
			bool flag = polySize < 3;
			if (flag)
			{
				polySize = 3;
			}
			bool flag2 = polySize > 3;
			if (flag2)
			{
				this._mesh.MergeConvexFaces(polySize);
			}
			for (MeshUtils.Vertex vertex = this._mesh._vHead._next; vertex != this._mesh._vHead; vertex = vertex._next)
			{
				vertex._n = -1;
			}
			for (MeshUtils.Face face = this._mesh._fHead._next; face != this._mesh._fHead; face = face._next)
			{
				face._n = -1;
				bool flag3 = !face._inside;
				if (!flag3)
				{
					bool noEmptyPolygons = this.NoEmptyPolygons;
					if (noEmptyPolygons)
					{
						float num3 = MeshUtils.FaceArea(face);
						bool flag4 = Math.Abs(num3) < float.Epsilon;
						if (flag4)
						{
							goto IL_0122;
						}
					}
					MeshUtils.Edge edge = face._anEdge;
					int num4 = 0;
					do
					{
						MeshUtils.Vertex vertex = edge._Org;
						bool flag5 = vertex._n == -1;
						if (flag5)
						{
							vertex._n = num2;
							num2++;
						}
						num4++;
						edge = edge._Lnext;
					}
					while (edge != face._anEdge);
					face._n = num;
					num++;
				}
				IL_0122:;
			}
			this._elementCount = num;
			bool flag6 = elementType == ElementType.ConnectedPolygons;
			if (flag6)
			{
				num *= 2;
			}
			this._elements = new int[num * polySize];
			this._vertexCount = num2;
			this._vertices = new ContourVertex[this._vertexCount];
			for (MeshUtils.Vertex vertex = this._mesh._vHead._next; vertex != this._mesh._vHead; vertex = vertex._next)
			{
				bool flag7 = vertex._n != -1;
				if (flag7)
				{
					this._vertices[vertex._n].Position = vertex._coords;
					this._vertices[vertex._n].Data = vertex._data;
				}
			}
			int num5 = 0;
			for (MeshUtils.Face face = this._mesh._fHead._next; face != this._mesh._fHead; face = face._next)
			{
				bool flag8 = !face._inside;
				if (!flag8)
				{
					bool noEmptyPolygons2 = this.NoEmptyPolygons;
					if (noEmptyPolygons2)
					{
						float num6 = MeshUtils.FaceArea(face);
						bool flag9 = Math.Abs(num6) < float.Epsilon;
						if (flag9)
						{
							goto IL_0336;
						}
					}
					MeshUtils.Edge edge = face._anEdge;
					int num4 = 0;
					do
					{
						MeshUtils.Vertex vertex = edge._Org;
						this._elements[num5++] = vertex._n;
						num4++;
						edge = edge._Lnext;
					}
					while (edge != face._anEdge);
					for (int i = num4; i < polySize; i++)
					{
						this._elements[num5++] = -1;
					}
					bool flag10 = elementType == ElementType.ConnectedPolygons;
					if (flag10)
					{
						edge = face._anEdge;
						do
						{
							this._elements[num5++] = this.GetNeighbourFace(edge);
							edge = edge._Lnext;
						}
						while (edge != face._anEdge);
						for (int i = num4; i < polySize; i++)
						{
							this._elements[num5++] = -1;
						}
					}
				}
				IL_0336:;
			}
		}

		private void OutputContours()
		{
			this._vertexCount = 0;
			this._elementCount = 0;
			for (MeshUtils.Face face = this._mesh._fHead._next; face != this._mesh._fHead; face = face._next)
			{
				bool flag = !face._inside;
				if (!flag)
				{
					MeshUtils.Edge edge2;
					MeshUtils.Edge edge = (edge2 = face._anEdge);
					do
					{
						this._vertexCount++;
						edge = edge._Lnext;
					}
					while (edge != edge2);
					this._elementCount++;
				}
			}
			this._elements = new int[this._elementCount * 2];
			this._vertices = new ContourVertex[this._vertexCount];
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (MeshUtils.Face face = this._mesh._fHead._next; face != this._mesh._fHead; face = face._next)
			{
				bool flag2 = !face._inside;
				if (!flag2)
				{
					int num4 = 0;
					MeshUtils.Edge edge2;
					MeshUtils.Edge edge = (edge2 = face._anEdge);
					do
					{
						this._vertices[num].Position = edge._Org._coords;
						this._vertices[num].Data = edge._Org._data;
						num++;
						num4++;
						edge = edge._Lnext;
					}
					while (edge != edge2);
					this._elements[num2++] = num3;
					this._elements[num2++] = num4;
					num3 += num4;
				}
			}
		}

		private float SignedArea(ContourVertex[] vertices)
		{
			float num = 0f;
			for (int i = 0; i < vertices.Length; i++)
			{
				ContourVertex contourVertex = vertices[i];
				ContourVertex contourVertex2 = vertices[(i + 1) % vertices.Length];
				num += contourVertex.Position.X * contourVertex2.Position.Y;
				num -= contourVertex.Position.Y * contourVertex2.Position.X;
			}
			return 0.5f * num;
		}

		public void AddContour(ContourVertex[] vertices)
		{
			this.AddContour(vertices, ContourOrientation.Original);
		}

		public void AddContour(ContourVertex[] vertices, ContourOrientation forceOrientation)
		{
			bool flag = this._mesh == null;
			if (flag)
			{
				this._mesh = new Mesh();
			}
			bool flag2 = false;
			bool flag3 = forceOrientation > ContourOrientation.Original;
			if (flag3)
			{
				float num = this.SignedArea(vertices);
				flag2 = (forceOrientation == ContourOrientation.Clockwise && num < 0f) || (forceOrientation == ContourOrientation.CounterClockwise && num > 0f);
			}
			MeshUtils.Edge edge = null;
			for (int i = 0; i < vertices.Length; i++)
			{
				bool flag4 = edge == null;
				if (flag4)
				{
					edge = this._mesh.MakeEdge();
					this._mesh.Splice(edge, edge._Sym);
				}
				else
				{
					this._mesh.SplitEdge(edge);
					edge = edge._Lnext;
				}
				int num2 = (flag2 ? (vertices.Length - 1 - i) : i);
				edge._Org._coords = vertices[num2].Position;
				edge._Org._data = vertices[num2].Data;
				edge._winding = 1;
				edge._Sym._winding = -1;
			}
		}

		public void Tessellate(WindingRule windingRule, ElementType elementType, int polySize)
		{
			this.Tessellate(windingRule, elementType, polySize, null);
		}

		public void Tessellate(WindingRule windingRule, ElementType elementType, int polySize, CombineCallback combineCallback)
		{
			this._normal = Vec3.Zero;
			this._vertices = null;
			this._elements = null;
			this._windingRule = windingRule;
			this._combineCallback = combineCallback;
			bool flag = this._mesh == null;
			if (!flag)
			{
				this.ProjectPolygon();
				this.ComputeInterior();
				bool flag2 = elementType == ElementType.BoundaryContours;
				if (flag2)
				{
					this.SetWindingNumber(1, true);
				}
				else
				{
					this.TessellateInterior();
				}
				bool flag3 = elementType == ElementType.BoundaryContours;
				if (flag3)
				{
					this.OutputContours();
				}
				else
				{
					this.OutputPolymesh(elementType, polySize);
				}
				bool usePooling = this.UsePooling;
				if (usePooling)
				{
					this._mesh.Free();
				}
				this._mesh = null;
			}
		}

		private Mesh _mesh;

		private Vec3 _normal;

		private Vec3 _sUnit;

		private Vec3 _tUnit;

		private float _bminX;

		private float _bminY;

		private float _bmaxX;

		private float _bmaxY;

		private WindingRule _windingRule;

		private Dict<Tess.ActiveRegion> _dict;

		private PriorityQueue<MeshUtils.Vertex> _pq;

		private MeshUtils.Vertex _event;

		private CombineCallback _combineCallback;

		private ContourVertex[] _vertices;

		private int _vertexCount;

		private int[] _elements;

		private int _elementCount;

		public float SUnitX = 1f;

		public float SUnitY = 0f;

		public float SentinelCoord = 4E+30f;

		public bool NoEmptyPolygons = false;

		public bool UsePooling = false;

		internal class ActiveRegion
		{
			internal MeshUtils.Edge _eUp;

			internal Dict<Tess.ActiveRegion>.Node _nodeUp;

			internal int _windingNumber;

			internal bool _inside;

			internal bool _sentinel;

			internal bool _dirty;

			internal bool _fixUpperEdge;
		}
	}
}
