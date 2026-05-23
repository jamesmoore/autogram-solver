namespace Autogram
{
    public interface IByteHistoryKey<TSelf>
        where TSelf : struct, IByteHistoryKey<TSelf>
    {
        static abstract int MaxLength { get; }

        static abstract TSelf Create(ReadOnlySpan<byte> values);
    }
}
