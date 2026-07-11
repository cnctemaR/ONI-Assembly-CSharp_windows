using System;

namespace System
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	internal class MonoDocumentationNoteAttribute : global::System.MonoTODOAttribute
	{
		public MonoDocumentationNoteAttribute(string comment)
			: base(comment)
		{
		}
	}
}
