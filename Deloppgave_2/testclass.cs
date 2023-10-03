using System.Reflection;
using Xunit;
using UtilityLibraries;


public class testclass
{
    [fact]
    public void PassingAddTest()
    {
        Assert.Equal(5, 2 + 2);
    }
}