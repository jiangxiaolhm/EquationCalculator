using Assignment1.Data;
using Assignment1.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assignment1.Main
{
    class EquationCalculator
    {
        private string _equation;

        private Stack<Variable> _variableStack;
        private Stack<Operator> _operatorStack;

        /// <summary>
        /// Starts the equation calculator.
        /// </summary>
        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                ReadNextEquation();
                if (_equation.ToLower() == "exit")
                {
                    isRunning = false;
                    continue;
                }

                if (IsEquationValid())
                {
                    TransformEquation();
                    try
                    {
                        CalculateEquation();
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Out of Integer Range");
                    }
                    catch (DivideByZeroException)
                    {
                        Console.WriteLine("Division by zero");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Input (this is not equation and invalid as well)");
                    continue;
                }
            }
        }

        /// <summary>
        /// Reads a new equation from the console and preprocesses the equation to remove white spaces and unnecessary operators.
        /// </summary>
        private void ReadNextEquation()
        {
            Console.Write("Calc ");
            _equation = Console.ReadLine();
            // Remove all white spaces.
            _equation = Regex.Replace(_equation, @"\s+", "");
        }

        /// <summary>
        /// Uses a regular expression to match the entire equation to indicate if the equation's format is correct.
        /// </summary>
        /// <returns>If true, the equation is valid.</returns>
        private bool IsEquationValid()
        {
            // This pattern is used to match an equation consisting of digits, unknowns, operators and parentheses.
            // var pattern = @"^[\+\-]*\(?[\+\-]*\d*(\d|(X(\^2)?))\)?([\+\-\*\/]?[\+\-]*\(?[\+\-]*\d*(\d|(X(\^2)?))\)?)*=[\+\-]*\(?[\+\-]*\d*(\d|(X(\^2)?))\)?([\+\-\*\/]?[\+\-]*\(?[\+\-]*\d*(\d|(X(\^2)?))\)?)*[\+\-]*$";

            // Eliminate combinations of a variable following an other variable immediately, such as XX, X^23X.
            var p1 = @"X(\^2)?(\d+|X)";
            if (Regex.Match(_equation, p1).Success) return false;

            // Match variables with or without unary operators +/- or one pair of parentheses ().
            var p2 = @"[\+\-]*\(?[\+\-]*\d*(\d|(X(\^2)?))\)?";
            // Match one or not operator between variables.
            var p3 = @"[\+\-\*\/]?";
            // Match one side of equal sign =.
            var p4 = p2 + @"(" + p3 + p2 + @")*";
            // Match ending +/-.
            var p5 = @"[\+\-]*";
            // Match a valid equation.
            var p6 = @"^" + p4 + @"=" + p4 + p5 + @"$";

            return Regex.Match(_equation, p6).Success;
        }

        /// <summary>
        /// Transforms to simplify the equation.
        /// </summary>
        private void TransformEquation()
        {
            // Remove all ending operators.
            _equation = _equation.TrimEnd('+', '-');
            // Move all right part to the left with parentheses.
            _equation = Regex.Replace(_equation, @"=", @"-(");
            _equation += ')';
            // Add multiply sign for multiplication through parenthesis
            // Eliminate )(
            _equation = Regex.Replace(_equation, @"(?<=\))(?=\()", "*");
            // Eliminate )Variable
            _equation = Regex.Replace(_equation, @"(?<=(\d|X))(?=\()", "*");
            // Eliminate Variable(
            _equation = Regex.Replace(_equation, @"(?<=\))(?=(\d|X))", "*");

            // Eliminate redundant operators
            bool isNewRedundantOperatorStr = true;
            int operatorsCount = 0;
            int negativeOperatorsCount = 0;
            bool isNegtive = false;
            for (var i = 0; i < _equation.Length; i++)
            {
                if (_equation[i] == '+' || _equation[i] == '-')
                {
                    if (isNewRedundantOperatorStr)
                    {
                        isNewRedundantOperatorStr = false;
                        operatorsCount = 0;
                        negativeOperatorsCount = 0;
                    }

                    operatorsCount++;
                    if (_equation[i] == '-') negativeOperatorsCount++;
                }
                else
                {
                    if (!isNewRedundantOperatorStr)
                    {
                        isNewRedundantOperatorStr = true;
                        if (operatorsCount > 1)
                        {
                            // Negtive sign is odd.
                            isNegtive = (negativeOperatorsCount % 2 == 1);
                            var a = _equation.Substring(0, i - operatorsCount);
                            var b = _equation.Substring(i, _equation.Length - i);
                            _equation = a + (isNegtive ? '-' : '+') + b;
                            i -= operatorsCount - 1;
                        }
                    }
                }
            }
        }

        private void CalculateEquation()
        {
            _variableStack = new Stack<Variable>();
            _operatorStack = new Stack<Operator>();

            for (var i = 0; i < _equation.Length; i++)
            {
                // Throw OverflowException
                Variable inComingVariable = GetNextVariable(_equation.Substring(i), out int stepLength);
                bool isInComingVariable = stepLength > 0;
                if (isInComingVariable)
                {
                    // The next is an variable.
                    i += stepLength - 1;
                    _variableStack.Push(inComingVariable);
                }
                else
                {
                    // The next is an operator.
                    Operator inComingOperator;

                    char[] temp = { '+', '-', '*', '/', '(' };
                    if ((_equation[i] == '+' || _equation[i] == '-') && (i == 0 || temp.Contains(_equation[i - 1])))
                    {
                        // In coming operator is unary operator.
                        inComingOperator = _equation[i] == '+' ? Operator.Positive : Operator.Negtive;
                    }
                    else
                    {
                        // In coming operator is binary oeprator.
                        switch (_equation[i])
                        {
                            case '+':
                                inComingOperator = Operator.Add;
                                break;
                            case '-':
                                inComingOperator = Operator.Subtract;
                                break;
                            case '*':
                                inComingOperator = Operator.Multiply;
                                break;
                            case '/':
                                inComingOperator = Operator.Divide;
                                break;
                            case '(':
                                inComingOperator = Operator.OpenParenthesis;
                                break;
                            case ')':
                                inComingOperator = Operator.CloseParenthesis;
                                break;
                            default:
                                inComingOperator = Operator.Default;
                                break;
                        }
                    }

                    if (_operatorStack.Count == 0)
                    {
                        _operatorStack.Push(inComingOperator);
                    }
                    else
                    {
                        var icp = inComingOperator.GetAttributeOfType<ArithmeticPriorityAttribute>().InComingPriority;
                        var isp = _operatorStack.Peek().GetAttributeOfType<ArithmeticPriorityAttribute>().InStackPriority;

                        while (_operatorStack.Count != 0 && icp < isp)
                        {
                            // Process stack top operator until the in coming priority is greater than the in stack priority.
                            Variable a;
                            Variable b;
                            switch (_operatorStack.Pop())
                            {
                                case Operator.Add:
                                    b = _variableStack.Pop();
                                    a = _variableStack.Pop();
                                    _variableStack.Push(a + b);
                                    break;
                                case Operator.Subtract:
                                    b = _variableStack.Pop();
                                    a = _variableStack.Pop();
                                    _variableStack.Push(a - b);
                                    break;
                                case Operator.Multiply:
                                    b = _variableStack.Pop();
                                    a = _variableStack.Pop();
                                    _variableStack.Push(a * b);
                                    break;
                                case Operator.Divide:
                                    b = _variableStack.Pop();
                                    a = _variableStack.Pop();
                                    _variableStack.Push(a / b);
                                    break;
                                case Operator.Positive:
                                    a = _variableStack.Pop();
                                    _variableStack.Push(+a);
                                    break;
                                case Operator.Negtive:
                                    a = _variableStack.Pop();
                                    _variableStack.Push(-a);
                                    break;
                            }
                            if (_operatorStack.Count > 0)
                            {
                                isp = _operatorStack.Peek().GetAttributeOfType<ArithmeticPriorityAttribute>().InStackPriority;
                            }
                        }
                        if (inComingOperator == Operator.CloseParenthesis)
                        {
                            // Pop open parenthesis
                            _operatorStack.Pop();
                        }
                        else
                        {
                            _operatorStack.Push(inComingOperator);
                        }
                    }
                }
            }

            while (_operatorStack.Count != 0)
            {
                // Process stack top operator until the in coming priority is greater than the in stack priority.
                Variable a;
                Variable b;
                switch (_operatorStack.Pop())
                {
                    case Operator.Add:
                        b = _variableStack.Pop();
                        a = _variableStack.Pop();
                        _variableStack.Push(a + b);
                        break;
                    case Operator.Subtract:
                        b = _variableStack.Pop();
                        a = _variableStack.Pop();
                        _variableStack.Push(a - b);
                        break;
                    case Operator.Multiply:
                        b = _variableStack.Pop();
                        a = _variableStack.Pop();
                        _variableStack.Push(a * b);
                        break;
                    case Operator.Divide:
                        b = _variableStack.Pop();
                        a = _variableStack.Pop();
                        _variableStack.Push(a / b);
                        break;
                    case Operator.Positive:
                        a = _variableStack.Pop();
                        _variableStack.Push(+a);
                        break;
                    case Operator.Negtive:
                        a = _variableStack.Pop();
                        _variableStack.Push(-a);
                        break;
                }
            }
        }

        /// <summary>
        /// Get next variable
        /// or null when next is an operator
        /// or throw OverflowException when the numerical part of the variable is overflowed.
        /// </summary>
        /// <param name="source">The source string to match next variable string</param>
        /// <param name="stepLength">The offset of the variable string in the scource string</param>
        /// <returns></returns>
        private Variable GetNextVariable(string source, out int stepLength)
        {

            var variableMatch = Regex.Match(source, @"^\d*(\d|(X(\^2)?))");
            if (!variableMatch.Success)
            {
                // Next is not a variable.
                stepLength = 0;
                return null;
            }

            var variableString = variableMatch.Value;
            stepLength = variableString.Length;

            // Match the exponent part of the variable.
            var powerPart = 0;
            if (Regex.Match(variableString, @"X\^2").Success)
            {
                powerPart = 2;
            }
            else if (Regex.Match(variableString, @"X").Success)
            {
                powerPart = 1;
            }

            // Match the numerical part of the variable.
            int numericalPart;
            var numericalPartMatch = Regex.Match(variableString, @"(?<!\^)\d+");

            if (numericalPartMatch.Success)
            {
                // Throw OverflowException
                numericalPart = Int32.Parse(numericalPartMatch.Value);
            }
            else
            {
                // Doesn't match a number, default is 1.
                numericalPart = 1;
            }

            // Create a new variable.
            Variable nextVariable = new Variable();
            switch (powerPart)
            {
                case 2:
                    nextVariable.x2 = numericalPart;
                    break;
                case 1:
                    nextVariable.x = numericalPart;
                    break;
                case 0:
                    nextVariable.c = numericalPart;
                    break;
            }

            return nextVariable;
        }
    }
}
