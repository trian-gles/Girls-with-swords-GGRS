using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public sealed class InputContainer : IEnumerable<char[]>
{
	private readonly char[] _buffer;   // flat: capacity * 2
	private readonly int _capacity;
	private int _count;

	// Fixed-capacity constructor
	public InputContainer(int maxLength)
	{
		if (maxLength <= 0)
			throw new ArgumentOutOfRangeException(nameof(maxLength));

		_capacity = maxLength;
		_buffer = new char[maxLength * 2];
		_count = 0;
	}

	// Collection-initializer constructor
	public InputContainer(IEnumerable<char[]> initial)
	{
		if (initial == null)
			throw new ArgumentNullException(nameof(initial));

		int count = 0;
		foreach (var item in initial)
		{
			if (item == null || item.Length != 2)
				throw new ArgumentException("All elements must be char[2].", nameof(initial));
			count++;
		}

		_capacity = count;
		_buffer = new char[count * 2];

		int i = 0;
		foreach (var item in initial)
		{
			int offset = i * 2;
			_buffer[offset] = item[0];
			_buffer[offset + 1] = item[1];
			i++;
		}

		_count = count;
	}

	public int Count => _count;
	public int Capacity => _capacity;

	public void Add(char[] item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (item.Length != 2)
			throw new ArgumentException("Input arrays must have length 2.", nameof(item));
		if (_count >= _capacity)
			throw new InvalidOperationException("InputContainer capacity exceeded.");

		int offset = _count * 2;
		_buffer[offset] = item[0];
		_buffer[offset + 1] = item[1];
		_count++;
	}

	public void Clear()
	{
		_count = 0;
	}

	public bool Contains(char[] item)
	{
		if (item == null || item.Length != 2)
			return false;

		for (int i = 0; i < _count; i++)
		{
			int offset = i * 2;
			if (_buffer[offset] == item[0] &&
				_buffer[offset + 1] == item[1])
				return true;
		}

		return false;
	}

	public char[] this[int index]
	{
		get
		{
			if ((uint)index >= (uint)_count)
				throw new ArgumentOutOfRangeException(nameof(index));

			int offset = index * 2;
			return new char[]
			{
				_buffer[offset],
				_buffer[offset + 1]
			};
		}
	}

	/// <summary>
	/// Prepends the contents of another InputContainer to the front of this one.
	/// Order is preserved.
	/// </summary>
	public void Prepend(InputContainer other)
{
	if (other == null)
		throw new ArgumentNullException(nameof(other));

	if (other._count == 0)
		return;

	if (other._count > _capacity)
		throw new InvalidOperationException("Prepending InputContainer exceeding capacity");

	if (ReferenceEquals(this, other))
		throw new InvalidOperationException("InputContainer self prepend");

	int oldCount = _count;

	int keepCount = oldCount;
	if (oldCount + other._count > _capacity)
		keepCount = _capacity - other._count;

	int charsPerInput = 2;

	int existingChars = keepCount * charsPerInput;
	int prependChars = other._count * charsPerInput;

	int srcOffset = (oldCount - keepCount) * charsPerInput;

	// Shift surviving existing inputs forward
	Array.Copy(_buffer, srcOffset, _buffer, prependChars, existingChars);

	// Copy prepended inputs
	Array.Copy(other._buffer, 0, _buffer, 0, prependChars);

	_count = keepCount + other._count;
}

	public string Dump()
	{
		string s = "";
		for (int i = 0; i < _count * 2; i++)
			{
				s += _buffer[i];
			}
		return s;
	}


	public IEnumerator<char[]> GetEnumerator()
	{
		for (int i = 0; i < _count; i++)
		{
			int offset = i * 2;
			yield return new char[]
			{
				_buffer[offset],
				_buffer[offset + 1]
			};
		}
	}

	public unsafe void SetState(int count, char* buffer)
	{
		if ((uint)count > (uint)_capacity)
			throw new ArgumentOutOfRangeException(nameof(count));

		int charCount = count * 2;

		for (int i = 0; i < charCount; i++)
			_buffer[i] = buffer[i];

		_count = count;
	}

	public unsafe int GetState(char* buffer)
	{
		int charCount = _count * 2;

		for (int i = 0; i < charCount; i++)
			buffer[i] = _buffer[i];

		return _count;
	}


	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
