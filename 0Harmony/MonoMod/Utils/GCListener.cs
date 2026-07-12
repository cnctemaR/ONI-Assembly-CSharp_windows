using System;

namespace MonoMod.Utils
{
	internal static class GCListener
	{
		public static event Action OnCollect;

		static GCListener()
		{
			new GCListener.CollectionDummy();
		}

		private static bool Unloading;

		private sealed class CollectionDummy
		{
			protected override void Finalize()
			{
				try
				{
					GCListener.Unloading |= AppDomain.CurrentDomain.IsFinalizingForUnload() || Environment.HasShutdownStarted;
					if (!GCListener.Unloading)
					{
						GC.ReRegisterForFinalize(this);
					}
					Action onCollect = GCListener.OnCollect;
					if (onCollect != null)
					{
						onCollect();
					}
				}
				finally
				{
					base.Finalize();
				}
			}
		}
	}
}
