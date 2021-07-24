using Godot;
using System;
using System.Collections.Generic;

public class GlobalsTests : WAT.Test
{
	public override string Title()
	{
		return "Testing various global functions";
	}

	[Test]
	public void CharArrayInList()
	{
		List<char[]> inpList = new List<char[]>();
		inpList.Add(new char[] {'k', 'p' });
		inpList.Add(new char[] {'8', 'p'});
		inpList.Add(new char[] { '8', 'r' });
		char[] testInp = new char[] { '8', 'r' } ;
		Assert.IsTrue(Globals.ArrayInList(inpList, testInp), "char array found in list containing it");
	}

	[Test]
	public void charArrayNotInList()
	{
		List<char[]> inpList = new List<char[]>();
		inpList.Add(new char[] { 'k', 'p' });
		inpList.Add(new char[] { '8', 'p' });
		inpList.Add(new char[] { '8', 'r' });
		char[] testInp = new char[] { '2', 'r' };
		Assert.IsFalse(Globals.ArrayInList(inpList, testInp), "char array not found in list not containing it");
	}

	[Test]
	public void KeyPressTest()
    {
		char[] input = new char[] { 'k', 'p' };
		Assert.IsTrue(Globals.CheckKeyPress(input, 'k'), "Input properly checks as key press");
		Assert.IsFalse(Globals.CheckKeyPress(input, 'p'), "Bad Input properly checks as not key press");

	}

}