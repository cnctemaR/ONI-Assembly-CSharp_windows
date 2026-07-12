using System;
using UnityEngine.UI.Collections;

namespace UnityEngine.UI
{
	public class ClipperRegistry
	{
		protected ClipperRegistry()
		{
		}

		public static ClipperRegistry instance
		{
			get
			{
				if (ClipperRegistry.s_Instance == null)
				{
					ClipperRegistry.s_Instance = new ClipperRegistry();
				}
				return ClipperRegistry.s_Instance;
			}
		}

		public void Cull()
		{
			int count = this.m_Clippers.Count;
			for (int i = 0; i < count; i++)
			{
				this.m_Clippers[i].PerformClipping();
			}
		}

		public static void Register(IClipper c)
		{
			if (c == null)
			{
				return;
			}
			ClipperRegistry.instance.m_Clippers.AddUnique(c, true);
		}

		public static void Unregister(IClipper c)
		{
			ClipperRegistry.instance.m_Clippers.Remove(c);
		}

		public static void Disable(IClipper c)
		{
			ClipperRegistry.instance.m_Clippers.DisableItem(c);
		}

		private static ClipperRegistry s_Instance;

		private readonly IndexedSet<IClipper> m_Clippers = new IndexedSet<IClipper>();
	}
}
