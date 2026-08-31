using Autogram;
using System.Numerics;

namespace AutogramTest
{
    public class AutogramConfigTest
    {
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
            var expected = new AutogramSnapshot([('a', 2), ('e', 7), ('b', 4), ('r', 3)])
                .ToString("{0}", " and ", ", ");

            Assert.Equal(expected, snapshot.ToString("{0}", " and ", ", "));

            Array.Fill(proposedCounts, T.Zero);
            Assert.Equal(expected, snapshot.ToString("{0}", " and ", ", "));
        }

        private static CharacterConfig MakeCharacter(int index, char character, bool isVariable, int minimumCount)
        {
            return new CharacterConfig(", ")
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
