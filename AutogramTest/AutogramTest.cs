using Autogram;
using Autogram.Extensions;
using System.Reflection;
using System.Runtime.Intrinsics;

namespace AutogramTest
{
    public class AutogramTest
    {
        private const string defaultTemplate = "This sentence is an autogram and it contains {0}."; // from https://en.wikipedia.org/wiki/Autogram
        private const int ExpectedIterations = 1710690;
        private const int RandomSeed = 2021428396; // cherry picked for fast resolve
        private const string Conjunction = " and lastly ";
        private const string SeparatorString = ", ";

        private static void RunAutogramTest<TAutogram>(
            Func<AutogramConfig, int, TAutogram> factory,
            int expectedIterations)
            where TAutogram : IAutogramFinder
        {
            var autogramConfig = GetConfig();
            var sut = factory(autogramConfig, RandomSeed);
            int i = 0;
            while (true)
            {
                var status = sut.Iterate();
                i++;
                if (status.Success)
                {
                    var result = sut.GetAutogramSnapshot().ToString(defaultTemplate, Conjunction, SeparatorString);
                    Assert.True(result.IsAutogram());
                    Assert.Equal(expectedIterations, i);
                    break;
                }
            }
        }

        private static AutogramConfig GetConfig()
        {
            return new AutogramConfigFactory().MakeAutogramConfig(
                "abcdefghijklmnopqrstuvwxyz",
                defaultTemplate,
                Conjunction,
                SeparatorString,
                "'s",
                "");
        }

        [Fact]
        public void Test()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV4(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestV5()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestV5a()
        {
            const int Expected = 1042770;
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5a(config, seed),
                Expected);
        }

        [Fact]
        public void TestV5b()
        {
            const int Expected = 661049;
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5b(config, seed),
                Expected);
        }

        [Fact]
        public void TestV5c()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5c(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestV5d()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5d(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestV5e()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5e(config, seed),
                1769732);
        }

        [Fact]
        public void TestV5g()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5g(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestV5h32()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5h32(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestV5h64()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5h64(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestV5i()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5i(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestVector256()
        {
            RunAutogramTest(
                (config, seed) => new AutogramVector256(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestVector512()
        {
            RunAutogramTest(
                (config, seed) => new AutogramVector512(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestVector256V2()
        {
            RunAutogramTest(
                (config, seed) => new AutogramVector256V2(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestVector256V3()
        {
            RunAutogramTest(
                (config, seed) => new AutogramVector256V3(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestV5i_FlattensVariableNumericCounts()
        {
            var config = GetConfig();
            var sut = new AutogramBytesNoStringsV5i(config, RandomSeed);

            Assert.Null(typeof(AutogramBytesNoStringsV5i).GetField("variableNumericCounts", BindingFlags.Instance | BindingFlags.NonPublic));

            var field = typeof(AutogramBytesNoStringsV5i).GetField("variableNumericCountsFlattened", BindingFlags.Instance | BindingFlags.NonPublic);
            var actual = Assert.IsType<byte[][]>(field?.GetValue(sut));
            var expected = config.GetVariableNumericCounts().SelectMany(p => p).ToArray();

            Assert.Equal(expected.Length, actual.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.True(expected[i].SequenceEqual(actual[i]));
            }
        }

        [Fact]
        public void TestVector256V2_ZeroesZeroCountVectors()
        {
            var config = GetConfig();
            var sut = new AutogramVector256V2(config, RandomSeed);

            var vectorsField = typeof(AutogramVector256V2).GetField("variableNumericCountsVectors", BindingFlags.Instance | BindingFlags.NonPublic);
            var strideField = typeof(AutogramVector256V2).GetField("variableNumericCountStride", BindingFlags.Instance | BindingFlags.NonPublic);
            var actual = Assert.IsType<Vector256<byte>[]>(vectorsField?.GetValue(sut));
            var stride = Assert.IsType<int>(strideField?.GetValue(sut));
            var zeroBuffer = new byte[Vector256<byte>.Count];

            for (var i = 0; i < config.VariableChars.Count(); i++)
            {
                actual[i * stride].CopyTo(zeroBuffer);
                Assert.True(zeroBuffer.All(b => b == 0));
            }
        }

        [Fact]
        public void TestV5h16ThrowsWhenAlphabetIsTooLarge()
        {
            var autogramConfig = new AutogramConfig
            {
                AllChars = Enumerable.Range(0, ByteHistoryKey16.MaxLength + 1)
                    .Select(i => new CharacterConfig(", ")
                    {
                        Index = i,
                        Char = (char)('a' + i),
                        BaselineCount = 0,
                        MinimumCount = 0,
                        IsVariable = true,
                        VariableIndex = i,
                        VariableBaselineCount = 0,
                    })
                    .ToList(),
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => new AutogramBytesNoStringsV5h16(autogramConfig, RandomSeed));
        }

        [Fact]
        public void TestVector256ThrowsWhenAlphabetIsTooLarge()
        {
            var autogramConfig = new AutogramConfig
            {
                AllChars = Enumerable.Range(0, ByteHistoryKey32.MaxLength + 1)
                    .Select(i => new CharacterConfig(", ")
                    {
                        Index = i,
                        Char = (char)('a' + i),
                        BaselineCount = 0,
                        MinimumCount = 0,
                        IsVariable = true,
                        VariableIndex = i,
                        VariableBaselineCount = 0,
                    })
                    .ToList(),
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => new AutogramVector256(autogramConfig, RandomSeed));
        }

        [Fact]
        public void TestVector512ThrowsWhenAlphabetIsTooLarge()
        {
            var autogramConfig = new AutogramConfig
            {
                AllChars = Enumerable.Range(0, ByteHistoryKey64.MaxLength + 1)
                    .Select(i => new CharacterConfig(", ")
                    {
                        Index = i,
                        Char = (char)('a' + i),
                        BaselineCount = 0,
                        MinimumCount = 0,
                        IsVariable = true,
                        VariableIndex = i,
                        VariableBaselineCount = 0,
                    })
                    .ToList(),
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => new AutogramVector512(autogramConfig, RandomSeed));
        }

        [Fact]
        public void TestV6()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV6(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestV7()
        {
            RunAutogramTest(
                (config, seed) => new AutogramIntsNoStringsV7(config, seed),
                ExpectedIterations);
        }

        [Fact]
        public void TestV8()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV8(config, seed),
                ExpectedIterations);
        }
    }
}
