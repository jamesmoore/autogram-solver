using Autogram;
using System.Numerics;

namespace AutogramTest
{
    public class AutogramConfigTest
    {
        private const string PluralSuffix = "'s";

        [Fact]
        public void GetAutogramSnapshot_UsesByteCounts()
        {
            AssertSnapshot<byte>([7, 0, 3]);
        }

        [Fact]
        public void GetAutogramSnapshot_UsesIntCounts()
        {
            AssertSnapshot<int>([7, -1, 3]);
        }

        private static void AssertSnapshot<T>(T[] proposedCounts)
            where T : IBinaryInteger<T>
        {
            var config = new AutogramConfig
            {
                Input = new AutogramInput {
                    Template = "{0}",
                    Conjunction = " and ",
                    SeparatorString = ", ",
                    Forced = string.Empty,
                    Alphabet = "abcenrz",
                    PluralSuffix = PluralSuffix,
                },
                AllChars =
                [
                    MakeCharacter(0, 'a', false, 2),
                    MakeCharacter(1, 'e', true, 1),
                    MakeCharacter(2, 'b', false, 4),
                    MakeCharacter(3, 'n', true, 1),
                    MakeCharacter(4, 'r', true, 1),
                    MakeCharacter(5, 'z', false, 0),
                ],
            };

            var snapshot = config.GetAutogramSnapshot(proposedCounts);
            var expected = new AutogramSnapshot(config.Input, [('a', 2), ('e', 7), ('b', 4), ('r', 3)])
                .ToString();

            Assert.Equal(expected, snapshot.ToString());

            Array.Fill(proposedCounts, T.Zero);
            Assert.Equal(expected, snapshot.ToString());
        }

        [Fact]
        public void Snapshot_RendersUsingCapturedInput()
        {
            var input = new AutogramInput
            {
                Alphabet = "abc",
                Template = "Captured: {0}.",
                Conjunction = " plus ",
                SeparatorString = " / ",
                PluralSuffix = "s",
                Forced = string.Empty,
            };
            var snapshot = new AutogramSnapshot(input, [('a', 1), ('b', 2), ('c', 3)]);

            Assert.Equal("Captured: one a / two bs plus three cs.", snapshot.ToString());
        }

        private static CharacterConfig MakeCharacter(int index, char character, bool isVariable, int minimumCount)
        {
            return new CharacterConfig(", ", PluralSuffix)
            {
                Index = index,
                Char = character,
                UnadjustedBaselineCount = 0,
                InvariantMinimumContribution = minimumCount,
                IsVariable = isVariable,
                Forced = false,
            };
        }
    }
}
