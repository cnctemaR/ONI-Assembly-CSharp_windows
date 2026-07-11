using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	internal static class NotificationUtilities
	{
		public static ScriptPlayable<TimeNotificationBehaviour> CreateNotificationsPlayable(PlayableGraph graph, IEnumerable<IMarker> markers, GameObject go)
		{
			ScriptPlayable<TimeNotificationBehaviour> scriptPlayable = ScriptPlayable<TimeNotificationBehaviour>.Null;
			PlayableDirector component = go.GetComponent<PlayableDirector>();
			foreach (IMarker marker in markers)
			{
				INotification notification = marker as INotification;
				if (notification != null)
				{
					if (scriptPlayable.Equals(ScriptPlayable<TimeNotificationBehaviour>.Null))
					{
						scriptPlayable = TimeNotificationBehaviour.Create(graph, component.playableAsset.duration, component.extrapolationMode);
					}
					DiscreteTime discreteTime = (DiscreteTime)marker.time;
					DiscreteTime discreteTime2 = (DiscreteTime)component.playableAsset.duration;
					if (discreteTime >= discreteTime2 && discreteTime <= discreteTime2.OneTickAfter() && discreteTime2 != 0)
					{
						discreteTime = discreteTime2.OneTickBefore();
					}
					INotificationOptionProvider notificationOptionProvider = marker as INotificationOptionProvider;
					if (notificationOptionProvider != null)
					{
						scriptPlayable.GetBehaviour().AddNotification((double)discreteTime, notification, notificationOptionProvider.flags);
					}
					else
					{
						scriptPlayable.GetBehaviour().AddNotification((double)discreteTime, notification, NotificationFlags.Retroactive);
					}
				}
			}
			return scriptPlayable;
		}

		public static bool TrackTypeSupportsNotifications(Type type)
		{
			TrackBindingTypeAttribute trackBindingTypeAttribute = (TrackBindingTypeAttribute)Attribute.GetCustomAttribute(type, typeof(TrackBindingTypeAttribute));
			return trackBindingTypeAttribute != null && (typeof(Component).IsAssignableFrom(trackBindingTypeAttribute.type) || typeof(GameObject).IsAssignableFrom(trackBindingTypeAttribute.type));
		}
	}
}
