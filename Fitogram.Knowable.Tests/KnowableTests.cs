using Shouldly;
using Xunit;

namespace Fitogram.Knowable.Tests
{
    public class KnowableTests
    {
        private enum Enum
        {
            Foo = 1,
        }
        
        [Fact]
        public void KnowableDefaultShouldBe0()
        {
            // [Arrange]

            Knowable<Enum> myEnum = null;

            // [Act/Assert]

            myEnum.IsKnown.ShouldBe(false);
            myEnum.InnerValue.ShouldBe(0);
        }
    }
}
