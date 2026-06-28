using System;
using System.Reflection;

public class GameComps : KComponents
{
	public GameComps()
	{
		FieldInfo[] fields = typeof(GameComps).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			object obj = Activator.CreateInstance(fieldInfo.FieldType);
			fieldInfo.SetValue(null, obj);
			base.Add<IComponentManager>(obj as IComponentManager);
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		FieldInfo[] fields = typeof(GameComps).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			fieldInfo.SetValue(null, null);
		}
	}

	public static GravityComponents Gravities;

	public static LoopingSoundUpdaterComponents LoopingSoundUpdaterComponents;

	public static FallerComponents Fallers;

	public static InfraredVisualizerComponents InfraredVisualizers;

	public static ElementSplitterComponents ElementSplitters;

	public static OreSizeVisualizerComponents OreSizeVisualizers;

	public static StructureTemperatureComponents StructureTemperatures;
}
