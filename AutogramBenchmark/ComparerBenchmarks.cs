using Autogram.Comparer;
using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class ComparerBenchmarks
    {
        private ByteArrayComparer BComparer = new();
        private ByteArraySpanComparer BSpanComparer = new();
        private ByteArraySpanWithHashCodeComparer BSpanWithHashCodeComparer = new();
        private ByteArraySpanWithChunkedHashCodeComparer BSpanWithChunkedHashCodeComparer = new();
        private readonly IntArraySpanComparer IntArraySpanComparer = new();

        private readonly byte[] ByteData;
        private readonly byte[] ByteDataCopy;

        private readonly int[] IntData;
        private readonly int[] IntDataCopy;

        public ComparerBenchmarks()
        {
            ByteData = Enumerable.Range(0, 26).Select(p => (byte)(p + 97)).ToArray();
            ByteDataCopy = ByteData.ToArray();
            IntData = Enumerable.Range(0, 26).Select(p => p + 97).ToArray();
            IntDataCopy = IntData.ToArray();
        }

        [Benchmark]
        public void ByteArrayComparer_Equals()
        {
            var x = BComparer.Equals(ByteData, ByteDataCopy);
        }

        [Benchmark]
        public void ByteArrayComparer_HashCode()
        {
            var x = BComparer.GetHashCode(ByteData);
        }

        [Benchmark]
        public void ByteArraySpanComparer_Equals()
        {
            var x = BSpanComparer.Equals(ByteData, ByteDataCopy);
        }

        [Benchmark]
        public void ByteArraySpanComparer_HashCode()
        {
            var x = BSpanComparer.GetHashCode(ByteData);
        }

        [Benchmark]
        public void ByteArraySpanWithHashCodeComparer_HashCode()
        {
            var x = BSpanWithHashCodeComparer.GetHashCode(ByteData);
        }


        [Benchmark]
        public void ByteArraySpanWithChunkedHashCodeComparer_HashCode()
        {
            var x = BSpanWithChunkedHashCodeComparer.GetHashCode(ByteData);
        }

        [Benchmark]
        public void IntArraySpanComparer_HashCode()
        {
            var x = IntArraySpanComparer.GetHashCode(IntData);
        }

        [Benchmark]
        public void IntArraySpanComparer_Equals()
        {
            var x = IntArraySpanComparer.Equals(IntData, IntDataCopy);
        }
    }
}
