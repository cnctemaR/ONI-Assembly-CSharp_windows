using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct TransitionData : IStyleDataGroup<TransitionData>, IEquatable<TransitionData>
	{
		public TransitionData Copy()
		{
			return new TransitionData
			{
				transitionDelay = new List<TimeValue>(this.transitionDelay),
				transitionDuration = new List<TimeValue>(this.transitionDuration),
				transitionProperty = new List<StylePropertyName>(this.transitionProperty),
				transitionTimingFunction = new List<EasingFunction>(this.transitionTimingFunction)
			};
		}

		public void CopyFrom(ref TransitionData other)
		{
			bool flag = this.transitionDelay != other.transitionDelay;
			if (flag)
			{
				this.transitionDelay.Clear();
				this.transitionDelay.AddRange(other.transitionDelay);
			}
			bool flag2 = this.transitionDuration != other.transitionDuration;
			if (flag2)
			{
				this.transitionDuration.Clear();
				this.transitionDuration.AddRange(other.transitionDuration);
			}
			bool flag3 = this.transitionProperty != other.transitionProperty;
			if (flag3)
			{
				this.transitionProperty.Clear();
				this.transitionProperty.AddRange(other.transitionProperty);
			}
			bool flag4 = this.transitionTimingFunction != other.transitionTimingFunction;
			if (flag4)
			{
				this.transitionTimingFunction.Clear();
				this.transitionTimingFunction.AddRange(other.transitionTimingFunction);
			}
		}

		public static bool operator ==(TransitionData lhs, TransitionData rhs)
		{
			return lhs.transitionDelay == rhs.transitionDelay && lhs.transitionDuration == rhs.transitionDuration && lhs.transitionProperty == rhs.transitionProperty && lhs.transitionTimingFunction == rhs.transitionTimingFunction;
		}

		public static bool operator !=(TransitionData lhs, TransitionData rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(TransitionData other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is TransitionData && this.Equals((TransitionData)obj);
		}

		public override int GetHashCode()
		{
			int num = this.transitionDelay.GetHashCode();
			num = (num * 397) ^ this.transitionDuration.GetHashCode();
			num = (num * 397) ^ this.transitionProperty.GetHashCode();
			return (num * 397) ^ this.transitionTimingFunction.GetHashCode();
		}

		public List<TimeValue> transitionDelay;

		public List<TimeValue> transitionDuration;

		public List<StylePropertyName> transitionProperty;

		public List<EasingFunction> transitionTimingFunction;
	}
}
