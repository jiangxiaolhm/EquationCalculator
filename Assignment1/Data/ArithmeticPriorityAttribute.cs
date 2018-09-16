using System;

namespace Assignment1.Data
{
    [AttributeUsage(AttributeTargets.Field)]
    class ArithmeticPriorityAttribute : Attribute
    {
        public int InComingPriority { get; set; }
        public int InStackPriority { get; set; }
    }
}
