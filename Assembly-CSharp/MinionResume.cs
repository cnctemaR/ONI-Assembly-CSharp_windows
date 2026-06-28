using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionResume : KMonoBehaviour, ISaveLoadable
{
}
