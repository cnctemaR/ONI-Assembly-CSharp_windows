using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{Id}")]
public class ScheduleGroup : Resource
{
	public ScheduleGroup(string id, ResourceSet parent, int defaultSegments, string name, string description, List<ScheduleBlockType> allowedTypes, bool flexible = false, bool alarm = false)
		: base(id, parent, name)
	{
		this.defaultSegments = defaultSegments;
		this.description = description;
		this.allowedTypes = allowedTypes;
		this.flexible = flexible;
		this.alarm = alarm;
	}

	public int defaultSegments { get; private set; }

	public string description { get; private set; }

	public List<ScheduleBlockType> allowedTypes { get; private set; }

	public bool flexible { get; private set; }

	public bool alarm { get; private set; }

	public bool Allowed(ScheduleBlockType type)
	{
		return this.allowedTypes.Contains(type);
	}
}
