using Autogram;

namespace AutogramTest;

public class ByteHistoryKey32SplitHashTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(21)]
    [InlineData(32)]
    public void EqualKeysHaveEqualHashes(int length)
    {
        var bytes = Enumerable.Range(0, length).Select(i => (byte)i).ToArray();
        var key = new ByteHistoryKey32SplitHash(bytes);
        var copy = ByteHistoryKey32SplitHash.Create(bytes.ToArray());

        Assert.Equal(key, copy);
        Assert.Equal(key.GetHashCode(), copy.GetHashCode());
    }

    [Fact]
    public void LengthDistinguishesTrailingZeroBytes()
    {
        Assert.NotEqual(new ByteHistoryKey32SplitHash([1, 2, 3]),
            new ByteHistoryKey32SplitHash([1, 2, 3, 0]));
    }

    [Fact]
    public void RejectsOversizedKeys()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ByteHistoryKey32SplitHash(new byte[33]));
    }

    [Fact]
    public void StructuredCountsNoLongerCollapseThroughXorFolding()
    {
        var originalHashes = new HashSet<int>();
        var revisedHashes = new HashSet<int>();
        var keys = new HashSet<ByteHistoryKey32SplitHash>();
        for (var value = 0; value < 256; value++)
        {
            byte[] bytes = [(byte)value, 0, 0, 0, (byte)value, 0, 0, 0];
            originalHashes.Add(new ByteHistoryKey32(bytes).GetHashCode());
            var key = new ByteHistoryKey32SplitHash(bytes);
            revisedHashes.Add(key.GetHashCode());
            keys.Add(key);
        }

        Assert.Single(originalHashes);
        // Allow occasional 32-bit collisions, but reject systematic collapse.
        Assert.InRange(revisedHashes.Count, 250, 256);
        Assert.Equal(256, keys.Count);
    }
}
