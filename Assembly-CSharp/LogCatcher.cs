using System;
using UnityEngine;

public class LogCatcher : ILogHandler
{
	public LogCatcher(ILogHandler old)
	{
		this.def = old;
	}

	void ILogHandler.LogException(Exception exception, global::UnityEngine.Object context)
	{
		string text = exception.ToString();
		string text2 = ((context != null) ? context.ToString() : null);
		if (text == "False" || text2 == "False")
		{
			global::Debug.LogError("False only message!");
		}
		this.def.LogException(exception, context);
	}

	void ILogHandler.LogFormat(LogType logType, global::UnityEngine.Object context, string format, params object[] args)
	{
		if (string.Format(format, args) == "False")
		{
			global::Debug.LogError("False only message!");
		}
		this.def.LogFormat(logType, context, format, args);
	}

	private ILogHandler def;
}
