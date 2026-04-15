using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public sealed class InputContainer
{
	private readonly char[] _buffer;   // flat: capacity * 2
	private readonly int _capacity;
	private int _count;

	public readonly struct CharPair
	{
		public readonly char A;
		public readonly char B;

		public CharPair(char a, char b)
		{
			A = a;
			B = b;
		}

		public static bool operator ==(CharPair left, CharPair right)
		{
			return left.A == right.A && left.B == right.B;
		}

		public static bool operator !=(CharPair left, CharPair right)
		{
			return !(left == right);
		}

		public bool Equals(CharPair other)
		{
			return A == other.A && B == other.B;
		}

		public override bool Equals(object obj)
		{
			return obj is CharPair other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (A * 397) ^ B;
			}
		}
	}

	// Fixed-capacity constructor
	public InputContainer(int maxLength)
	{
		if (maxLength <= 0)
			throw new ArgumentOutOfRangeException(nameof(maxLength));

		_capacity = maxLength;
		_buffer = new char[maxLength * 2];
		_count = 0;
	}

	// Constructor from char[][]
	public InputContainer(char[][] initial)
	{
		if (initial == null)
			throw new ArgumentNullException(nameof(initial));

		_capacity = initial.Length;
		_buffer = new char[_capacity * 2];
		_count = 0;

		for (int i = 0; i < initial.Length; i++)
		{
			var item = initial[i];

			if (item == null || item.Length != 2)
				throw new ArgumentException("All elements must be char[2].", nameof(initial));

			int offset = _count * 2;
			_buffer[offset] = item[0];
			_buffer[offset + 1] = item[1];
			_count++;
		}
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public struct Enumerator
	{
		private readonly InputContainer _container;
		private int _index;

		public Enumerator(InputContainer container)
		{
			_container = container;
			_index = -1;
		}

		public bool MoveNext()
		{
			_index++;
			return _index < _container._count;
		}

		public CharPair Current
		{
			get
			{
				int offset = _index * 2;
				return new CharPair(
					_container._buffer[offset],
					_container._buffer[offset + 1]
				);
			}
		}
	}

	public int Count => _count;
	public int Capacity => _capacity;

	public void Add(CharPair item)
	{

		int offset = _count * 2;
		_buffer[offset] = item.A;
		_buffer[offset + 1] = item.B;
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

	public CharPair Get(int index)
	{
		if ((uint)index >= (uint)_count)
			throw new ArgumentOutOfRangeException(nameof(index));

		int offset = index * 2;
		return new CharPair(_buffer[offset], _buffer[offset + 1]);
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
}
