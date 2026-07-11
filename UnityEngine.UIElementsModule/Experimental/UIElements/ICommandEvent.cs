using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface ICommandEvent
	{
		/// <summary>
		///   <para>Name of the command.</para>
		/// </summary>
		string commandName { get; }
	}
}
