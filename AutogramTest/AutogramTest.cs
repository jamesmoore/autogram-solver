using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autogram;

namespace AutogramTest
{
    public class AutogramTest
    {
        [Fact]
        public void Autogram_GetCounts_Test()
        {
            var a = new Autogram.Autogram();
            var x = a.GetActualCounts();
            Assert.Equal(26, x.Length);

            Assert.All(x, p => Assert.True(p > 0));
        }
    }
}
