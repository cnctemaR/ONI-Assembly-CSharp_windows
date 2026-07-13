using System;
using System.Collections.Generic;
using System.Reflection;

namespace HarmonyLib
{
	internal class Infix
	{
		internal Infix(Patch patch)
		{
			this.patch = patch;
		}

		internal MethodInfo OuterMethod
		{
			get
			{
				return this.patch.PatchMethod;
			}
		}

		internal MethodBase InnerMethod
		{
			get
			{
				return this.patch.innerMethod.Method;
			}
		}

		internal int[] Positions
		{
			get
			{
				return this.patch.innerMethod.positions;
			}
		}

		internal bool Matches(MethodBase method, int index, int total)
		{
			if (method != this.InnerMethod)
			{
				return false;
			}
			if (this.Positions.Length == 0)
			{
				return true;
			}
			foreach (int num in this.Positions)
			{
				if (num > 0 && num == index)
				{
					return true;
				}
				if (num < 0 && index == total + num + 1)
				{
					return true;
				}
			}
			return false;
		}

		internal IEnumerable<CodeInstruction> Apply(MethodCreatorConfig config, bool isPrefix)
		{
			Infix.<Apply>d__9 <Apply>d__ = new Infix.<Apply>d__9(-2);
			<Apply>d__.<>3__config = config;
			<Apply>d__.<>3__isPrefix = isPrefix;
			return <Apply>d__;
		}

		internal Patch patch;
	}
}
