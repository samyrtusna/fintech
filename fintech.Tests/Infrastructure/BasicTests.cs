using System;
using System.Collections.Generic;
using System.Text;

namespace fintech.Tests.Infrastructure
{
    public class BasicTests
    {
        [Fact]
        public void One_Plus_One_Should_Equal_Two()
        {
            var result = 1 + 1;

            Assert.Equal(2, result);
        }
    }
}
