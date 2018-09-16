namespace Assignment1.Data
{
    enum Operator
    {
        [ArithmeticPriority(InComingPriority = 2, InStackPriority = 3)]
        Add,
        [ArithmeticPriority(InComingPriority = 2, InStackPriority = 3)]
        Subtract,
        [ArithmeticPriority(InComingPriority = 4, InStackPriority = 5)]
        Multiply,
        [ArithmeticPriority(InComingPriority = 4, InStackPriority = 5)]
        Divide,
        [ArithmeticPriority(InComingPriority = 7, InStackPriority = 6)]
        Positive,
        [ArithmeticPriority(InComingPriority = 7, InStackPriority = 6)]
        Negtive,
        [ArithmeticPriority(InComingPriority = 8, InStackPriority = 0)]
        OpenParenthesis,
        [ArithmeticPriority(InComingPriority = 1, InStackPriority = 9)]
        CloseParenthesis,
        Default
    }
}
