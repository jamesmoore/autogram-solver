using Autogram;

namespace AutogramTest
{
    public class ByteHistoryKeyTests
    {
        [Fact]
        public void ByteHistoryKey16_EqualKeysAreEqual()
        {
            byte[] bytes = [1, 2, 3, 4, 5];
            var key1 = new ByteHistoryKey16(bytes);
            var key2 = new ByteHistoryKey16(bytes);
            Assert.Equal(key1, key2);
            Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
        }

        [Fact]
        public void ByteHistoryKey16_DifferentBytesAreNotEqual()
        {
            var key1 = new ByteHistoryKey16([1, 2, 3]);
            var key2 = new ByteHistoryKey16([1, 2, 4]);
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void ByteHistoryKey16_DifferentLengthsAreNotEqual()
        {
            var key1 = new ByteHistoryKey16([1, 2, 3]);
            var key2 = new ByteHistoryKey16([1, 2, 3, 0]);
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void ByteHistoryKey24_EqualKeysAreEqual()
        {
            byte[] bytes = [1, 2, 3, 4, 5];
            var key1 = new ByteHistoryKey24(bytes);
            var key2 = new ByteHistoryKey24(bytes);
            Assert.Equal(key1, key2);
            Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
        }

        [Fact]
        public void ByteHistoryKey24_DifferentBytesAreNotEqual()
        {
            var key1 = new ByteHistoryKey24([1, 2, 3]);
            var key2 = new ByteHistoryKey24([1, 2, 4]);
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void ByteHistoryKey24_DifferentLengthsAreNotEqual()
        {
            var key1 = new ByteHistoryKey24([1, 2, 3]);
            var key2 = new ByteHistoryKey24([1, 2, 3, 0]);
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void ByteHistoryKey32_EqualKeysAreEqual()
        {
            byte[] bytes = [1, 2, 3, 4, 5];
            var key1 = new ByteHistoryKey32(bytes);
            var key2 = new ByteHistoryKey32(bytes);
            Assert.Equal(key1, key2);
            Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
        }

        [Fact]
        public void ByteHistoryKey32_DifferentBytesAreNotEqual()
        {
            var key1 = new ByteHistoryKey32([1, 2, 3]);
            var key2 = new ByteHistoryKey32([1, 2, 4]);
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void ByteHistoryKey32_DifferentLengthsAreNotEqual()
        {
            var key1 = new ByteHistoryKey32([1, 2, 3]);
            var key2 = new ByteHistoryKey32([1, 2, 3, 0]);
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void ByteHistoryKey64_EqualKeysAreEqual()
        {
            byte[] bytes = [1, 2, 3, 4, 5];
            var key1 = new ByteHistoryKey64(bytes);
            var key2 = new ByteHistoryKey64(bytes);
            Assert.Equal(key1, key2);
            Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
        }

        [Fact]
        public void ByteHistoryKey64_DifferentBytesAreNotEqual()
        {
            var key1 = new ByteHistoryKey64([1, 2, 3]);
            var key2 = new ByteHistoryKey64([1, 2, 4]);
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void ByteHistoryKey64_DifferentLengthsAreNotEqual()
        {
            var key1 = new ByteHistoryKey64([1, 2, 3]);
            var key2 = new ByteHistoryKey64([1, 2, 3, 0]);
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void ByteHistoryKey64_DecodeReturnsOriginalBytes()
        {
            byte[][] cases =
            [
                [],
                [1, 2, 3, 4, 5],
                [1, 2, 3, 0],
                [1, 2, 3, 4, 5, 6, 7, 8],
                [1, 2, 3, 4, 5, 6, 7, 8, 9],
                [
                    1, 2, 3, 4, 5, 6, 7, 8,
                    9, 10, 11, 12, 13, 14, 15, 16,
                    17, 18, 19, 20, 21, 22, 23, 24,
                    25, 26, 27, 28, 29, 30, 31, 32,
                    33, 34, 35, 36, 37, 38, 39, 40,
                    41, 42, 43, 44, 45, 46, 47, 48,
                    49, 50, 51, 52, 53, 54, 55, 56,
                    57, 58, 59, 60, 61, 62, 63, 64
                ]
            ];

            foreach (var bytes in cases)
            {
                var key = new ByteHistoryKey64(bytes);

                Assert.Equal(bytes, key.Decode());
            }
        }
    }
}
