using System;

public static unsafe class UnsafeSerializer
{
	// Serialize a struct into a preallocated byte[] buffer
	public static void Serialize<T>(ref T data, byte[] buffer) where T : struct
	{
		if (buffer.Length < sizeof(T))
			throw new ArgumentException("Buffer too small");

		fixed (byte* dst = buffer)
		{
			//Buffer.MemoryCopy(&data, dst, buffer.Length, sizeof(T));
		}
	}

	// Deserialize from a preallocated byte[] buffer back into a struct
	public static void Deserialize<T>(byte[] buffer, out T data) where T : struct
	{
		if (buffer.Length < sizeof(T))
			throw new ArgumentException("Buffer too small");

		data = default;

		fixed (byte* src = buffer)
		{
			//Buffer.MemoryCopy(src, &data, sizeof(T), sizeof(T));
		}
	}
}
