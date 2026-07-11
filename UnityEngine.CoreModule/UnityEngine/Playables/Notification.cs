using System;

namespace UnityEngine.Playables
{
	public class Notification : INotification
	{
		public Notification(string name)
		{
			this.id = new PropertyName(name);
		}

		public PropertyName id { get; }
	}
}
