using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public sealed class HeldKeys : IEnumerable<char>
{
    private readonly char[] _keys;
    private readonly bool[] _occupied;
    private readonly int _capacity;
    private int _count;

    /// <summary>
    /// Creates a fixed-capacity HeldKeys set.
    /// All heap memory is allocated here.
    /// </summary>
    public HeldKeys(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
        _keys = new char[capacity];
        _occupied = new bool[capacity];
        _count = 0;
    }

    public int Count => _count;

    public int Capacity => _capacity;

    /// <summary>
    /// Adds a key if not already present.
    /// Returns true if added, false if already present.
    /// </summary>
    public bool Add(char key)
    {
        // Check for existing key
        for (int i = 0; i < _capacity; i++)
        {
            if (_occupied[i] && _keys[i] == key)
                return false;
        }

        // Insert into first free slot
        for (int i = 0; i < _capacity; i++)
        {
            if (!_occupied[i])
            {
                _keys[i] = key;
                _occupied[i] = true;
                _count++;
                return true;
            }
        }

        throw new InvalidOperationException("HeldKeys capacity exceeded.");
    }

    /// <summary>
    /// Removes a key if present.
    /// Returns true if removed.
    /// </summary>
    public bool Remove(char key)
    {
        for (int i = 0; i < _capacity; i++)
        {
            if (_occupied[i] && _keys[i] == key)
            {
                _occupied[i] = false;
                _count--;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns true if the key exists in the set.
    /// </summary>
    public bool Contains(char key)
    {
        for (int i = 0; i < _capacity; i++)
        {
            if (_occupied[i] && _keys[i] == key)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Removes all keys.
    /// Capacity is unchanged.
    /// </summary>
    public void Clear()
    {
        Array.Clear(_occupied, 0, _capacity);
        _count = 0;
    }

    public IEnumerator<char> GetEnumerator()
    {
        for (int i = 0; i < _capacity; i++)
        {
            if (_occupied[i])
                yield return _keys[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public unsafe int GetState(char* buffer){
        int count = 0;
        foreach (char key in this){
            buffer[count] = key;
            count++;
        }
        return count;
    }

    public unsafe void SetState(int count, char* buffer){
        Clear();
        for (int i = 0; i < count; i++)
            Add(buffer[i]);
    }

    public void DumpTest()
    {
        
        foreach (char key in this)
            GD.Print(key);
    }
}
