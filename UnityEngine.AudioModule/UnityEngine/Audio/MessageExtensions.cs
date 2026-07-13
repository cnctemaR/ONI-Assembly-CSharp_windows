using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Audio
{
	public static class MessageExtensions
	{
		public static T Get<T>(this ProcessorInstance.Message message) where T : class
		{
			bool flag = !message.Is<T>();
			if (flag)
			{
				throw new InvalidCastException(string.Format("Message does not contain data of type {0}", typeof(T)));
			}
			return (T)((object)GCHandle.FromIntPtr(message.ManagedHandle).Target);
		}

		public static ProcessorInstance.Response SendMessage<T>(this ControlContext context, ProcessorInstance processorInstance, T message) where T : class
		{
			return context.SendManagedMessage<T>(processorInstance, message);
		}
	}
}
