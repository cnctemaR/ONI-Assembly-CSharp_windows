using System;

namespace UnityEngine.Android
{
	public interface IApplicationExitInfo
	{
		string description { get; }

		int describeContents { get; }

		int definingUid { get; }

		ProcessImportance importance { get; }

		int packageUid { get; }

		int pid { get; }

		string processName { get; }

		sbyte[] processStateSummary { get; }

		long pss { get; }

		int realUid { get; }

		ExitReason reason { get; }

		long rss { get; }

		int status { get; }

		long timestamp { get; }

		byte[] trace { get; }

		string traceAsString { get; }
	}
}
