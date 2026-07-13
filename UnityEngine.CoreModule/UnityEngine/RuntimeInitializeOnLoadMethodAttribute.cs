using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class RuntimeInitializeOnLoadMethodAttribute : PreserveAttribute
	{
		public RuntimeInitializeOnLoadMethodAttribute()
		{
			this.loadType = RuntimeInitializeLoadType.AfterSceneLoad;
		}

		public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType loadType)
		{
			this.loadType = loadType;
		}

		public RuntimeInitializeLoadType loadType
		{
			get
			{
				return this.m_LoadType;
			}
			private set
			{
				this.m_LoadType = value;
			}
		}

		private RuntimeInitializeLoadType m_LoadType;
	}
}
