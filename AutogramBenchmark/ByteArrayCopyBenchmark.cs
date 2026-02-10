using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class ByteArrayCopyBenchmark
    {
        private byte[] sourceArray = null!;
        private byte[] destinationArray = null!;

        [Params(10, 20, 50)]
        public int ArrayLength { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            sourceArray = new byte[ArrayLength];
            destinationArray = new byte[ArrayLength];
            
            // Fill source array with some data
            var random = new Random(42);
            random.NextBytes(sourceArray);
        }

        [Benchmark]
        public void CopyToAsSpan()
        {
            sourceArray.CopyTo(destinationArray.AsSpan());
        }

        [Benchmark]
        public void BufferBlockCopy()
        {
            Buffer.BlockCopy(sourceArray, 0, destinationArray, 0, sourceArray.Length);
        }

        [Benchmark]
        public void ManualForLoop()
        {
            for (int i = 0; i < sourceArray.Length; i++)
            {
                destinationArray[i] = sourceArray[i];
            }
        }

        [Benchmark]
        public void ArrayCopy()
        {
            Array.Copy(sourceArray, destinationArray, sourceArray.Length);
        }

        [Benchmark]
        public void SpanCopyTo()
        {
            sourceArray.AsSpan().CopyTo(destinationArray.AsSpan());
        }
    }
}
