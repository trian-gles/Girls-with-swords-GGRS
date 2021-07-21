
using Godot;
using System;
using System.Collections.Generic;

public class GlobalsTests : WAT.Test
{
	public override String Title()
	{
		return "Testing various global functions";
	}

	[Test]
	public void StringArrayInList()
	{
		List<string[]> inpList = new List<string[]>();
		inpList.Add(new string[] {"k", "press" });
		inpList.Add(new string[] { "up", "press" });
		inpList.Add(new string[] { "up", "release" });
		string[] testInp = new string[] { "up", "press" };
		Assert.IsTrue(Globals.ArrayInList(inpList, testInp), "String array found in list containing it");
	}

	[Test]
	public void StringArrayNotInList()
	{
		List<string[]> inpList = new List<string[]>();
		inpList.Add(new string[] { "k", "press" });
		inpList.Add(new string[] { "up", "press" });
		inpList.Add(new string[] { "up", "release" });
		string[] testInp = new string[] { "down", "press" };
		Assert.IsFalse(Globals.ArrayInList(inpList, testInp), "String array not found in list not containing it");
	}

}