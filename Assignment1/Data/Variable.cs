using System;

namespace Assignment1.Data
{
    /// <summary>
    /// Data model of a quadratic variable with quadratic part, linear part and constant part. 
    /// </summary>
    class Variable
    {
        public double x2;
        public double x;
        public double c;
        public Variable divisor;

        public Variable(double x2, double x, double c, Variable divisor)
        {
            this.x2 = x2;
            this.x = x;
            this.c = c;
            this.divisor = divisor;
        }

        public Variable() : this(0, 0, 0) { }
        public Variable(double x2, double x, double c) : this(x2, x, c, null) { }
        public Variable(Variable variable) : this(variable.x2, variable.x, variable.c) { }
        public Variable(Variable variable, Variable divisor) : this(variable.x2, variable.x, variable.c, divisor) { }

        override
        public string ToString()
        {
            if (divisor == null)
            {
                return $"{x2}X^2 + {x}X + {c}";
            }

            return $"({x2}X^2 + {x}X + {c}) / (" + divisor + ")";
        }

        public static Variable operator +(Variable a, Variable b)
        {
            return new Variable(a.x2 + b.x2, a.x + b.x, a.c + b.c);
        }

        public static Variable operator -(Variable a, Variable b)
        {
            return new Variable(a.x2 - b.x2, a.x - b.x, a.c - b.c);
        }

        public static Variable operator *(Variable a, Variable b)
        {
            if ((a.x2 != 0 && (b.x2 != 0 || b.x != 0)) || (b.x2 != 0 && (a.x2 != 0 || a.x != 0)))
            {
                // The greatest power of the equation is greater than two.
                throw new PowerGreaterThanTwoException();
            }
            return new Variable(
                a.x2 * b.c + a.x * b.x + a.c * b.x2,
                a.x * b.c + a.c * b.x,
                a.c * b.c);
        }

        public static Variable operator /(Variable a, Variable b)
        {
            var ratioX2 = (a.x2 != 0 && b.x2 != 0) ? a.x2 / b.x2 : 0;
            var ratioX = (a.x != 0 && b.x != 0) ? a.x / b.x : 0;
            var ratioC = (a.c != 0 && b.c != 0) ? a.c / b.c : 0;

            if (b.x2 != 0)
            {
                if (b.x != 0)
                {
                    if (b.c != 0)
                    {
                        if (ratioX2 == ratioX && ratioX == ratioC)
                        {
                            return new Variable(0, 0, ratioX2);
                        }
                        else
                        {
                            // Cannot be divided at the point.
                            // Should move to the divisor part of the variable.
                        }
                    }
                    else
                    {
                        if (a.c == 0 && ratioX2 == ratioX)
                        {
                            return new Variable(0, 0, ratioX2);
                        }
                        else
                        {
                            // Cannot be divided at the point.
                            // Should move to the divisor part of the variable.
                        }
                    }
                }
                else if (b.c != 0)
                {
                    if (a.x == 0 && ratioX2 == ratioC)
                    {
                        return new Variable(0, 0, ratioX2);
                    }
                    else
                    {
                        // Cannot be divided at the point.
                        // Should move to the divisor part of the variable.
                    }
                }
                else
                {
                    if (a.x == 0 && a.c == 0)
                    {
                        return new Variable(0, 0, ratioX2);
                    }
                    else
                    {
                        // Cannot be divided at the point.
                        // Should move to the divisor part of the variable.
                    }
                }
            }
            else if (b.x != 0)
            {
                if (b.c != 0)
                {
                    if (a.x2 == 0 && ratioX == ratioC)
                    {
                        return new Variable(0, 0, ratioX);
                    }
                    else
                    {
                        // Cannot be divided at the point.
                        // Should move to the divisor part of the variable.
                    }
                }
                else
                {
                    if (a.x2 == 0 && a.c == 0)
                    {
                        return new Variable(0, 0, ratioX);
                    }
                    else if (a.x2 != 0 && a.c == 0)
                    {
                        return new Variable(0, a.x2 / b.x, a.x / b.x);
                    }
                    else
                    {
                        // Cannot be divided at the point.
                        // Should move to the divisor part of the variable.
                    }
                }
            }
            else if (b.c != 0)
            {
                return new Variable(a.x2 / b.c, a.x / b.c, a.c / b.c);
            }
            else
            {
                throw new DivideByZeroException();
            }
            return null;
        }

        public static Variable operator +(Variable a)
        {
            return new Variable(a);
        }

        public static Variable operator -(Variable a)
        {
            return new Variable(-a.x2, -a.x, -a.c);
        }
    }
}
