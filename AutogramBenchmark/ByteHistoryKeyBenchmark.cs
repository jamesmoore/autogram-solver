using Autogram;
using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class ByteHistoryKeyBenchmark
    {
        private readonly byte[] values16 = Enumerable.Range(0, 16).Select(p => (byte)p).ToArray();
        private readonly byte[] values24 = Enumerable.Range(0, 24).Select(p => (byte)p).ToArray();
        private readonly byte[] values32 = Enumerable.Range(0, 32).Select(p => (byte)p).ToArray();
        private readonly byte[] values64 = Enumerable.Range(0, 64).Select(p => (byte)p).ToArray();

        private ByteHistoryKey16 key16;
        private ByteHistoryKey24 key24;
        private ByteHistoryKey32 key32;
        private ByteHistoryKey64 key64;

        [GlobalSetup]
        public void Setup()
        {
            key16 = new ByteHistoryKey16(values16);
            key24 = new ByteHistoryKey24(values24);
            key32 = new ByteHistoryKey32(values32);
            key64 = new ByteHistoryKey64(values64);
        }

        [Benchmark]
        public ByteHistoryKey16 Create16()
        {
            return new ByteHistoryKey16(values16);
        }

        [Benchmark]
        public ByteHistoryKey24 Create24()
        {
            return new ByteHistoryKey24(values24);
        }

        [Benchmark]
        public ByteHistoryKey32 Create32()
        {
            return new ByteHistoryKey32(values32);
        }

        [Benchmark]
        public ByteHistoryKey64 Create64()
        {
            return new ByteHistoryKey64(values64);
        }

        [Benchmark]
        public bool Equals16()
        {
            return key16.Equals(new ByteHistoryKey16(values16));
        }

        [Benchmark]
        public bool Equals24()
        {
            return key24.Equals(new ByteHistoryKey24(values24));
        }

        [Benchmark]
        public bool Equals32()
        {
            return key32.Equals(new ByteHistoryKey32(values32));
        }

        [Benchmark]
        public bool Equals64()
        {
            return key64.Equals(new ByteHistoryKey64(values64));
        }

        [Benchmark]
        public int GetHashCode16()
        {
            return key16.GetHashCode();
        }

        [Benchmark]
        public int GetHashCode24()
        {
            return key24.GetHashCode();
        }

        [Benchmark]
        public int GetHashCode32()
        {
            return key32.GetHashCode();
        }

        [Benchmark]
        public int GetHashCode64()
        {
            return key64.GetHashCode();
        }
    }
}
