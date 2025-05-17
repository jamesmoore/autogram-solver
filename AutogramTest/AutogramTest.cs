namespace AutogramTest
{
    public class AutogramTest
    {
        [Fact]
        public void Autogram_GetCounts_Test()
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";
            var a = new Autogram.AutogramBytes(alphabet.ToCharArray());
            var x = a.GetActualCounts(a.ToString());
            Assert.Equal(alphabet.Length, x.Length);
        }
    }
}
