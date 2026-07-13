using System;
using System.Linq;
using System.Reflection;

namespace HarmonyLib
{
	[Serializable]
	public class InnerMethod
	{
		public InnerMethod(MethodInfo method, params int[] positions)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (positions.Any<int>((int p) => p == 0))
			{
				throw new ArgumentException("positions cannot contain zeros");
			}
			this.Method = method;
			this.positions = positions;
		}

		internal InnerMethod(int methodToken, string moduleGUID, int[] positions)
		{
			this.methodToken = methodToken;
			this.moduleGUID = moduleGUID;
			this.positions = positions;
		}

		public MethodInfo Method
		{
			get
			{
				if (this.method == null)
				{
					this.method = AccessTools.GetMethodByModuleAndToken(this.moduleGUID, this.methodToken);
				}
				return this.method;
			}
			set
			{
				this.method = value;
				this.methodToken = this.method.MetadataToken;
				this.moduleGUID = this.method.Module.ModuleVersionId.ToString();
			}
		}

		[NonSerialized]
		private MethodInfo method;

		private int methodToken;

		private string moduleGUID;

		public int[] positions;
	}
}
