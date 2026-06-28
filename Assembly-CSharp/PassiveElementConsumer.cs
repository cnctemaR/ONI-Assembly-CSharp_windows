using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class PassiveElementConsumer : ElementConsumer, ISaveLoadableJson, IEffectDescriptor
{
	protected override bool IsActive()
	{
		return true;
	}
}
