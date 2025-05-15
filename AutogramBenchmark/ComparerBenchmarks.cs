using Autogram.Comparer;
using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class ComparerBenchmarks
    {
        private ByteArrayComparer BComparer = new();
        private ByteArraySpanComparer BSpanComparer = new();
        private IntArrayComparer IComparer = new();
        private IntArraySpanComparer ISpanComparer = new();

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
        public void ByteDataCompare()
        {
            var x = BComparer.Equals(ByteData, ByteDataCopy);
        }

        [Benchmark]
        public void ByteDataHashCode()
        {
            var x = BComparer.GetHashCode(ByteData);
        }

        [Benchmark]
        public void ByteSpanDataCompare()
        {
            var x = BSpanComparer.Equals(ByteData, ByteDataCopy);
        }

        [Benchmark]
        public void IntDataCompare()
        {
            var x = IComparer.Equals(IntData, IntDataCopy);
        }

        [Benchmark]
        public void IntDataHashCode()
        {
            var x = IComparer.GetHashCode(IntData);
        }

        [Benchmark]
        public void IntSpanDataCompare()
        {
            var x = ISpanComparer.Equals(IntData, IntDataCopy);
        }
    }
}
