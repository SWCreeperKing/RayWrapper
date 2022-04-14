using System;

namespace RayWrapperTester.Example_Setup;

[AttributeUsage(AttributeTargets.Class)]
public class ExampleAttribute : Attribute
{
    public string tabName;
    public ExampleAttribute(string tabName) => this.tabName = tabName;
}