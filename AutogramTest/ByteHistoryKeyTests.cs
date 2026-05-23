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
    }
}
