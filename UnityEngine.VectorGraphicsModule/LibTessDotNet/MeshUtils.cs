using System;
using System.Collections.Generic;

namespace LibTessDotNet
{
	internal static class MeshUtils
	{
		public static MeshUtils.Edge MakeEdge(MeshUtils.Edge eNext)
		{
			MeshUtils.EdgePair edgePair = MeshUtils.EdgePair.Create();
			MeshUtils.Edge e = edgePair._e;
			MeshUtils.Edge eSym = edgePair._eSym;
			MeshUtils.Edge.EnsureFirst(ref eNext);
			MeshUtils.Edge next = eNext._Sym._next;
			eSym._next = next;
			next._Sym._next = e;
			e._next = eNext;
			eNext._Sym._next = eSym;
			e._Sym = eSym;
			e._Onext = e;
			e._Lnext = eSym;
			e._Org = null;
			e._Lface = null;
			e._winding = 0;
			e._activeRegion = null;
			eSym._Sym = e;
			eSym._Onext = eSym;
			eSym._Lnext = e;
			eSym._Org = null;
			eSym._Lface = null;
			eSym._winding = 0;
			eSym._activeRegion = null;
			return e;
		}

		public static void Splice(MeshUtils.Edge a, MeshUtils.Edge b)
		{
			MeshUtils.Edge onext = a._Onext;
			MeshUtils.Edge onext2 = b._Onext;
			onext._Sym._Lnext = b;
			onext2._Sym._Lnext = a;
			a._Onext = onext2;
			b._Onext = onext;
		}

		public static void MakeVertex(MeshUtils.Edge eOrig, MeshUtils.Vertex vNext)
		{
			MeshUtils.Vertex vertex = MeshUtils.Pooled<MeshUtils.Vertex>.Create();
			MeshUtils.Vertex prev = vNext._prev;
			vertex._prev = prev;
			prev._next = vertex;
			vertex._next = vNext;
			vNext._prev = vertex;
			vertex._anEdge = eOrig;
			MeshUtils.Edge edge = eOrig;
			do
			{
				edge._Org = vertex;
				edge = edge._Onext;
			}
			while (edge != eOrig);
		}

		public static void MakeFace(MeshUtils.Edge eOrig, MeshUtils.Face fNext)
		{
			MeshUtils.Face face = MeshUtils.Pooled<MeshUtils.Face>.Create();
			MeshUtils.Face prev = fNext._prev;
			face._prev = prev;
			prev._next = face;
			face._next = fNext;
			fNext._prev = face;
			face._anEdge = eOrig;
			face._trail = null;
			face._marked = false;
			face._inside = fNext._inside;
			MeshUtils.Edge edge = eOrig;
			do
			{
				edge._Lface = face;
				edge = edge._Lnext;
			}
			while (edge != eOrig);
		}

		public static void KillEdge(MeshUtils.Edge eDel)
		{
			MeshUtils.Edge.EnsureFirst(ref eDel);
			MeshUtils.Edge next = eDel._next;
			MeshUtils.Edge next2 = eDel._Sym._next;
			next._Sym._next = next2;
			next2._Sym._next = next;
			eDel.Free();
		}

		public static void KillVertex(MeshUtils.Vertex vDel, MeshUtils.Vertex newOrg)
		{
			MeshUtils.Edge anEdge = vDel._anEdge;
			MeshUtils.Edge edge = anEdge;
			do
			{
				edge._Org = newOrg;
				edge = edge._Onext;
			}
			while (edge != anEdge);
			MeshUtils.Vertex prev = vDel._prev;
			MeshUtils.Vertex next = vDel._next;
			next._prev = prev;
			prev._next = next;
			vDel.Free();
		}

		public static void KillFace(MeshUtils.Face fDel, MeshUtils.Face newLFace)
		{
			MeshUtils.Edge anEdge = fDel._anEdge;
			MeshUtils.Edge edge = anEdge;
			do
			{
				edge._Lface = newLFace;
				edge = edge._Lnext;
			}
			while (edge != anEdge);
			MeshUtils.Face prev = fDel._prev;
			MeshUtils.Face next = fDel._next;
			next._prev = prev;
			prev._next = next;
			fDel.Free();
		}

		public static float FaceArea(MeshUtils.Face f)
		{
			float num = 0f;
			MeshUtils.Edge edge = f._anEdge;
			do
			{
				num += (edge._Org._s - edge._Dst._s) * (edge._Org._t + edge._Dst._t);
				edge = edge._Lnext;
			}
			while (edge != f._anEdge);
			return num;
		}

		public const int Undef = -1;

		public abstract class Pooled<T> where T : MeshUtils.Pooled<T>, new()
		{
			public abstract void Reset();

			public virtual void OnFree()
			{
			}

			public static T Create()
			{
				bool flag = MeshUtils.Pooled<T>._stack != null && MeshUtils.Pooled<T>._stack.Count > 0;
				T t;
				if (flag)
				{
					t = MeshUtils.Pooled<T>._stack.Pop();
				}
				else
				{
					t = new T();
				}
				return t;
			}

			public void Free()
			{
				this.OnFree();
				this.Reset();
				bool flag = MeshUtils.Pooled<T>._stack == null;
				if (flag)
				{
					MeshUtils.Pooled<T>._stack = new Stack<T>();
				}
				MeshUtils.Pooled<T>._stack.Push((T)((object)this));
			}

			private static Stack<T> _stack;
		}

		public class Vertex : MeshUtils.Pooled<MeshUtils.Vertex>
		{
			public override void Reset()
			{
				this._prev = (this._next = null);
				this._anEdge = null;
				this._coords = Vec3.Zero;
				this._s = 0f;
				this._t = 0f;
				this._pqHandle = default(PQHandle);
				this._n = 0;
				this._data = null;
			}

			internal MeshUtils.Vertex _prev;

			internal MeshUtils.Vertex _next;

			internal MeshUtils.Edge _anEdge;

			internal Vec3 _coords;

			internal float _s;

			internal float _t;

			internal PQHandle _pqHandle;

			internal int _n;

			internal object _data;
		}

		public class Face : MeshUtils.Pooled<MeshUtils.Face>
		{
			internal int VertsCount
			{
				get
				{
					int num = 0;
					MeshUtils.Edge edge = this._anEdge;
					do
					{
						num++;
						edge = edge._Lnext;
					}
					while (edge != this._anEdge);
					return num;
				}
			}

			public override void Reset()
			{
				this._prev = (this._next = null);
				this._anEdge = null;
				this._trail = null;
				this._n = 0;
				this._marked = false;
				this._inside = false;
			}

			internal MeshUtils.Face _prev;

			internal MeshUtils.Face _next;

			internal MeshUtils.Edge _anEdge;

			internal MeshUtils.Face _trail;

			internal int _n;

			internal bool _marked;

			internal bool _inside;
		}

		public struct EdgePair
		{
			public static MeshUtils.EdgePair Create()
			{
				MeshUtils.EdgePair edgePair = default(MeshUtils.EdgePair);
				edgePair._e = MeshUtils.Pooled<MeshUtils.Edge>.Create();
				edgePair._e._pair = edgePair;
				edgePair._eSym = MeshUtils.Pooled<MeshUtils.Edge>.Create();
				edgePair._eSym._pair = edgePair;
				return edgePair;
			}

			public void Reset()
			{
				this._e = (this._eSym = null);
			}

			internal MeshUtils.Edge _e;

			internal MeshUtils.Edge _eSym;
		}

		public class Edge : MeshUtils.Pooled<MeshUtils.Edge>
		{
			internal MeshUtils.Face _Rface
			{
				get
				{
					return this._Sym._Lface;
				}
				set
				{
					this._Sym._Lface = value;
				}
			}

			internal MeshUtils.Vertex _Dst
			{
				get
				{
					return this._Sym._Org;
				}
				set
				{
					this._Sym._Org = value;
				}
			}

			internal MeshUtils.Edge _Oprev
			{
				get
				{
					return this._Sym._Lnext;
				}
				set
				{
					this._Sym._Lnext = value;
				}
			}

			internal MeshUtils.Edge _Lprev
			{
				get
				{
					return this._Onext._Sym;
				}
				set
				{
					this._Onext._Sym = value;
				}
			}

			internal MeshUtils.Edge _Dprev
			{
				get
				{
					return this._Lnext._Sym;
				}
				set
				{
					this._Lnext._Sym = value;
				}
			}

			internal MeshUtils.Edge _Rprev
			{
				get
				{
					return this._Sym._Onext;
				}
				set
				{
					this._Sym._Onext = value;
				}
			}

			internal MeshUtils.Edge _Dnext
			{
				get
				{
					return this._Rprev._Sym;
				}
				set
				{
					this._Rprev._Sym = value;
				}
			}

			internal MeshUtils.Edge _Rnext
			{
				get
				{
					return this._Oprev._Sym;
				}
				set
				{
					this._Oprev._Sym = value;
				}
			}

			internal static void EnsureFirst(ref MeshUtils.Edge e)
			{
				bool flag = e == e._pair._eSym;
				if (flag)
				{
					e = e._Sym;
				}
			}

			public override void Reset()
			{
				this._pair.Reset();
				this._next = (this._Sym = (this._Onext = (this._Lnext = null)));
				this._Org = null;
				this._Lface = null;
				this._activeRegion = null;
				this._winding = 0;
			}

			internal MeshUtils.EdgePair _pair;

			internal MeshUtils.Edge _next;

			internal MeshUtils.Edge _Sym;

			internal MeshUtils.Edge _Onext;

			internal MeshUtils.Edge _Lnext;

			internal MeshUtils.Vertex _Org;

			internal MeshUtils.Face _Lface;

			internal Tess.ActiveRegion _activeRegion;

			internal int _winding;
		}
	}
}
