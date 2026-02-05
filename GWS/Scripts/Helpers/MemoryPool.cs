public sealed class MemoryPool
{
    private byte[][] _pool;
    private int _poolLength;
    private int _head = 0;

    public MemoryPool(int bufferSize, int poolLength)
    {
        _pool = new byte[poolLength][];
        _poolLength = poolLength;

        for (int i = 0; i < poolLength; i++)
        {
            _pool[i] = new byte[bufferSize];
        }
    }

    public byte[] Get()
    {
        byte[] returnPool = _pool[_head];
        _head++;
        if (_head == _poolLength)
            _head = 0;
        
        return returnPool;
        
    }
}