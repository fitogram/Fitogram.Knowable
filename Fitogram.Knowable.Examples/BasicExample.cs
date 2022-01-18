using System;

namespace Fitogram.Knowable.Examples
{
    public class BasicExample
    {
        public void Run()
        {
            Knowable<MyEnum> myKnowableEnum = 1;
            Console.WriteLine(myKnowableEnum.IsKnown); // True
            Console.WriteLine(myKnowableEnum.Value); // Foo
            myKnowableEnum = 2;
            Console.WriteLine(myKnowableEnum.IsKnown); // False
            // Console.WriteLine(myKnowableEnum.Value); // Exception!
        }
        
        private enum MyEnum
        {
            Foo = 1,
        }
    }
}
