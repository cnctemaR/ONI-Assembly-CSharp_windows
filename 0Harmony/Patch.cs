using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	[Serializable]
	public class Patch : IComparable
	{
		public Patch(MethodInfo patch, int index, string owner, int priority, string[] before, string[] after)
		{
			bool flag = patch is DynamicMethod;
			if (flag)
			{
				throw new Exception("Cannot directly reference dynamic method \"" + patch.FullDescription() + "\" in Harmony. Use a factory method instead that will return the dynamic method.");
			}
			this.index = index;
			this.owner = owner;
			this.priority = priority;
			this.before = before;
			this.after = after;
			this.patch = patch;
		}

		public MethodInfo GetMethod(MethodBase original)
		{
			bool flag = this.patch.ReturnType != typeof(DynamicMethod);
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = this.patch;
			}
			else
			{
				bool flag2 = !this.patch.IsStatic;
				if (flag2)
				{
					methodInfo = this.patch;
				}
				else
				{
					ParameterInfo[] parameters = this.patch.GetParameters();
					bool flag3 = parameters.Count<ParameterInfo>() != 1;
					if (flag3)
					{
						methodInfo = this.patch;
					}
					else
					{
						bool flag4 = parameters[0].ParameterType != typeof(MethodBase);
						if (flag4)
						{
							methodInfo = this.patch;
						}
						else
						{
							methodInfo = this.patch.Invoke(null, new object[] { original }) as DynamicMethod;
						}
					}
				}
			}
			return methodInfo;
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Patch && this.patch == ((Patch)obj).patch;
		}

		public int CompareTo(object obj)
		{
			return PatchInfoSerialization.PriorityComparer(obj, this.index, this.priority, this.before, this.after);
		}

		public override int GetHashCode()
		{
			return this.patch.GetHashCode();
		}

		public readonly int index;

		public readonly string owner;

		public readonly int priority;

		public readonly string[] before;

		public readonly string[] after;

		public readonly MethodInfo patch;
	}
}
