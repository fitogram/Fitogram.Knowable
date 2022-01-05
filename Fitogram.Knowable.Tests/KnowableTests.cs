using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Fitogram.Knowable.Tests
{
    public class KnowableTests
    {
        public enum Enum
        {
            Foo = 1,
        }
        
        [Fact]
        public void KnowableShouldSupportNull()
        {
            // [Arrange]

            Knowable<Enum> myEnum = null;

            // [Act/Assert]

            myEnum.IsKnown.ShouldBe(false);
            myEnum.InnerValue.ShouldBe(null);
        }
    }
}