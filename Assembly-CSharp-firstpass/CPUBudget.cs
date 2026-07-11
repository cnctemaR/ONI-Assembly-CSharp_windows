using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class CPUBudget
{
	public static int coreCount
	{
		get
		{
			int overrideCoreCount = TuningData<CPUBudget.Tuning>.Get().overrideCoreCount;
			return (0 >= overrideCoreCount || overrideCoreCount >= SystemInfo.processorCount) ? SystemInfo.processorCount : overrideCoreCount;
		}
	}

	public static float ComputeDuration(long start)
	{
		long num = (CPUBudget.stopwatch.ElapsedTicks - start) * 1000000L / Stopwatch.Frequency;
		return (float)num / 1000f;
	}

	public static void AddRoot(ICPULoad root)
	{
		CPUBudget.nodes.Add(root, new CPUBudget.Node
		{
			load = root,
			children = new List<CPUBudget.Node>(),
			frameTime = root.GetEstimatedFrameTime(),
			loadBalanceThreshold = TuningData<CPUBudget.Tuning>.Get().defaultLoadBalanceThreshold
		});
	}

	public static void AddChild(ICPULoad parent, ICPULoad child, float loadBalanceThreshold)
	{
		CPUBudget.Node node = new CPUBudget.Node
		{
			load = child,
			children = new List<CPUBudget.Node>(),
			frameTime = child.GetEstimatedFrameTime(),
			loadBalanceThreshold = loadBalanceThreshold
		};
		CPUBudget.nodes.Add(child, node);
		CPUBudget.nodes[parent].children.Add(node);
	}

	public static void AddChild(ICPULoad parent, ICPULoad child)
	{
		CPUBudget.AddChild(parent, child, TuningData<CPUBudget.Tuning>.Get().defaultLoadBalanceThreshold);
	}

	public static void FinalizeChildren(ICPULoad parent)
	{
		CPUBudget.Node node = CPUBudget.nodes[parent];
		List<CPUBudget.Node> children = CPUBudget.nodes[parent].children;
		float num = 0f;
		foreach (CPUBudget.Node node2 in children)
		{
			CPUBudget.FinalizeChildren(node2.load);
			num += node2.frameTime;
		}
		for (int num2 = 0; num2 != children.Count; num2++)
		{
			CPUBudget.Node node3 = children[num2];
			node3.frameTime = node.frameTime * (node3.frameTime / num);
			children[num2] = node3;
		}
	}

	public static void Start(ICPULoad cpuLoad)
	{
		CPUBudget.Node node = CPUBudget.nodes[cpuLoad];
		node.start = CPUBudget.stopwatch.ElapsedTicks;
		CPUBudget.nodes[cpuLoad] = node;
	}

	public static void End(ICPULoad cpuLoad)
	{
		CPUBudget.Node node = CPUBudget.nodes[cpuLoad];
		float num = node.frameTime - CPUBudget.ComputeDuration(node.start);
		if (node.loadBalanceThreshold < Math.Abs(num))
		{
			CPUBudget.Balance(cpuLoad, num);
		}
	}

	public static void Balance(ICPULoad cpuLoad, float frameTimeDelta)
	{
		CPUBudget.Node node = CPUBudget.nodes[cpuLoad];
		List<CPUBudget.Node> children = node.children;
		if (children.Count == 0)
		{
			if (node.load.AdjustLoad(node.frameTime, frameTimeDelta))
			{
				node.frameTime += frameTimeDelta;
			}
		}
		else
		{
			for (int num = 0; num != children.Count; num++)
			{
				CPUBudget.Node node2 = children[num];
				float num2 = node2.frameTime / node.frameTime;
				float num3 = frameTimeDelta * num2;
				CPUBudget.Balance(node2.load, num3);
				children[num] = node2;
			}
		}
	}

	public static Stopwatch stopwatch = Stopwatch.StartNew();

	private static Dictionary<ICPULoad, CPUBudget.Node> nodes = new Dictionary<ICPULoad, CPUBudget.Node>();

	private class Tuning : TuningData<CPUBudget.Tuning>
	{
		public int overrideCoreCount = -1;

		public float defaultLoadBalanceThreshold = 0.1f;
	}

	private struct Node
	{
		public ICPULoad load;

		public List<CPUBudget.Node> children;

		public long start;

		public float frameTime;

		public float loadBalanceThreshold;
	}
}
