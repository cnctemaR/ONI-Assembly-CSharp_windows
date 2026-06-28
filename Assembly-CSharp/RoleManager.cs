using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class RoleManager : KMonoBehaviour, ISaveLoadable
{
}
