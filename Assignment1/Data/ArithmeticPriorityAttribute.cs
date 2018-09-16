using System;

namespace Assignment1.Data
{
    /// <summary>
    /// Add priorities for the enum of Operators.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    class ArithmeticPriorityAttribute : Attribute
    {
        public int InComingPriority { get; set; }
        public int InStackPriority { get; set; }
    }
}
