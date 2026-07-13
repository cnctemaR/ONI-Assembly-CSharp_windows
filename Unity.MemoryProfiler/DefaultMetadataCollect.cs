using System;
using Unity.Profiling.Memory;
using UnityEngine;

namespace Unity.MemoryProfiler
{
	internal class DefaultMetadataCollect : MetadataCollect
	{
		public DefaultMetadataCollect()
		{
			MetadataInjector.DefaultCollectorInjected = 1;
		}

		public override void CollectMetadata(MemorySnapshotMetadata data)
		{
			data.Description = string.Concat(new string[]
			{
				data.Description,
				"Project name: ",
				Application.productName,
				"\n",
				string.Format("This Memory Snapshot capture started at {0} (UTC)\n", DateTime.UtcNow),
				string.Format("Time.frameCount: {0}\n", Time.frameCount),
				"Time.realtimeSinceStartup: ",
				this.FormatSecondsToTime(Time.realtimeSinceStartupAsDouble),
				"\n"
			});
		}

		private string FormatSecondsToTime(double timeInSeconds)
		{
			int num = (int)timeInSeconds;
			int num2 = (int)((timeInSeconds - (double)num) * 1000.0);
			int num3 = num / 60;
			num %= 60;
			int num4 = num3 / 60;
			num3 %= 60;
			return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", new object[] { num4, num3, num, num2 });
		}
	}
}
