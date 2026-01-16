using System;
using System.Collections;
using System.Collections.Generic;

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

		if (_count + other._count > _capacity)
			throw new InvalidOperationException("InputContainer capacity exceeded.");

		// Special case: self-prepend
		if (ReferenceEquals(this, other))
		{
			int originalCount = _count;
			int charsToMove = originalCount * 2;

			Array.Copy(_buffer, 0, _buffer, originalCount * 2, charsToMove);
			Array.Copy(_buffer, originalCount * 2, _buffer, 0, charsToMove);

			_count = originalCount * 2 > _capacity * 2
				? throw new InvalidOperationException("InputContainer capacity exceeded.")
				: originalCount * 2;

			return;
		}

		int existingChars = _count * 2;
		int prependChars = other._count * 2;

		// Shift existing contents forward
		Array.Copy(_buffer, 0, _buffer, prependChars, existingChars);

		// Copy other contents to front
		Array.Copy(other._buffer, 0, _buffer, 0, prependChars);

		_count += other._count;
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

	private char[] temp = new char[] {' ', ' '};
	public void SetState(int count, char[] buffer){
		Clear();
		for (int i = 0; i < count; i++){
			temp[0] = buffer[i * 2];
			temp[1] = buffer[i * 2 + 1];
			Add(temp);
		}
	}

	public int GetState(char[] buffer){
		for (int i = 0; i < _count; i++){
			buffer[i] = _buffer[i];
		}
		return _count;
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
