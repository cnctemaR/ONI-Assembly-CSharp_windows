using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[NativeHeader("Runtime/Animation/Constraints/ScaleConstraint.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Animation/Constraints/Constraint.bindings.h")]
	[RequireComponent(typeof(Transform))]
	public sealed class ScaleConstraint : Behaviour, IConstraint, IConstraintInternal
	{
		private ScaleConstraint()
		{
			ScaleConstraint.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] ScaleConstraint self);

		public extern float weight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 scaleAtRest
		{
			get
			{
				Vector3 vector;
				this.get_scaleAtRest_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_scaleAtRest_Injected(ref value);
			}
		}

		public Vector3 scaleOffset
		{
			get
			{
				Vector3 vector;
				this.get_scaleOffset_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_scaleOffset_Injected(ref value);
			}
		}

		public extern Axis scalingAxis
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool constraintActive
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool locked
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public int sourceCount
		{
			get
			{
				return ScaleConstraint.GetSourceCountInternal(this);
			}
		}

		[FreeFunction("ConstraintBindings::GetSourceCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSourceCountInternal([NotNull] ScaleConstraint self);

		[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetSources([NotNull] List<ConstraintSource> sources);

		public void SetSources(List<ConstraintSource> sources)
		{
			if (sources == null)
			{
				throw new ArgumentNullException("sources");
			}
			ScaleConstraint.SetSourcesInternal(this, sources);
		}

		[FreeFunction("ConstraintBindings::SetSources")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSourcesInternal([NotNull] ScaleConstraint self, List<ConstraintSource> sources);

		public int AddSource(ConstraintSource source)
		{
			return this.AddSource_Injected(ref source);
		}

		public void RemoveSource(int index)
		{
			this.ValidateSourceIndex(index);
			this.RemoveSourceInternal(index);
		}

		[NativeName("RemoveSource")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveSourceInternal(int index);

		public ConstraintSource GetSource(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetSourceInternal(index);
		}

		[NativeName("GetSource")]
		private ConstraintSource GetSourceInternal(int index)
		{
			ConstraintSource constraintSource;
			this.GetSourceInternal_Injected(index, out constraintSource);
			return constraintSource;
		}

		public void SetSource(int index, ConstraintSource source)
		{
			this.ValidateSourceIndex(index);
			this.SetSourceInternal(index, source);
		}

		[NativeName("SetSource")]
		private void SetSourceInternal(int index, ConstraintSource source)
		{
			this.SetSourceInternal_Injected(index, ref source);
		}

		private void ValidateSourceIndex(int index)
		{
			if (this.sourceCount == 0)
			{
				throw new InvalidOperationException("The ScaleConstraint component has no sources.");
			}
			if (index < 0 || index >= this.sourceCount)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Constraint source index {0} is out of bounds (0-{1}).", index, this.sourceCount));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_scaleAtRest_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_scaleAtRest_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_scaleOffset_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_scaleOffset_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int AddSource_Injected(ref ConstraintSource source);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);
	}
}
