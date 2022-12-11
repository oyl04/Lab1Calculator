using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1Calculator
{
    class Lab1CalculatorVisitor : Lab1CalculatorBaseVisitor<double>
    {

        private Int64 toInteger(double x)
        {
            double val = Math.Round(x, 0, MidpointRounding.ToNegativeInfinity);
            return Convert.ToInt64(val); 
        }

        public override double VisitMaxMinExpr([NotNull] Lab1CalculatorParser.MaxMinExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (context.operatorToken.Type == Lab1CalculatorLexer.MIN)
            {
                Debug.WriteLine("min({0}; {1})", left, right);
                return Math.Min(left, right);
            }
            else
            {
                Debug.WriteLine("max({0}; {1})", left, right);
                return Math.Max(left, right);
            }
        }

        public override double VisitCompileUnit(Lab1CalculatorParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitNumberExpr(Lab1CalculatorParser.NumberExprContext context)
        {
            var result = double.Parse(context.GetText());
            Debug.WriteLine(result);
            return result;
        }

        public override double VisitIdentifierExpr(Lab1CalculatorParser.IdentifierExprContext context)
        {
            var result = context.GetText();
            var cell = NumberConverter.From26System(result);
            double value = Convert.ToDouble(Calculator.Evaluate(Table.grid[cell.row][cell.column].expression));
            return value;
            
        }

        public override double VisitParenthesizedExpr(Lab1CalculatorParser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitExponentialExpr(Lab1CalculatorParser.ExponentialExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (left == 0 && right == 0) throw new Exception();
            Debug.WriteLine("{0} ^ {1}", left, right);
            return Math.Pow(left, right);
        }

        public override double VisitAdditiveExpr(Lab1CalculatorParser.AdditiveExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == Lab1CalculatorLexer.ADD)
            {
                Debug.WriteLine("{0} + {1}", left, right);
                return left + right;
            }
            else 
            {
                Debug.WriteLine("{0} - {1}", left, right);
                return left - right;
            }
        }

        public override double VisitMultiplicativeExpr(Lab1CalculatorParser.MultiplicativeExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == Lab1CalculatorLexer.MULTIPLY)
            {
                Debug.WriteLine("{0} * {1}", left, right);
                return left * right;
            }
            else 
            {
                if (right == 0) throw new DivideByZeroException();
                Debug.WriteLine("{0} / {1}", left, right);
                return left / right;
            }
        }

        private long ModValue(long num, long mod)
        {
            return ((num % mod) + mod) % mod;
        }

        public override double VisitIncDecExpr(Lab1CalculatorParser.IncDecExprContext context)
        {
            var val = toInteger(WalkLeft(context));
            if (context.operatorToken.Type == Lab1CalculatorLexer.DEC)
            {
                Debug.WriteLine("dec({0})", val);
                return Convert.ToDouble(val - 1);
            }
            else
            {
                Debug.WriteLine("inc({0})", val);
                return Convert.ToDouble(val + 1);
            }
        }

        public override double VisitModDivExpr(Lab1CalculatorParser.ModDivExprContext context)
        {
            var left = toInteger(WalkLeft(context));
            var right = toInteger(WalkRight(context));
            if (right == 0) throw new DivideByZeroException();
            if (context.operatorToken.Type == Lab1CalculatorLexer.MOD)
            {
                Debug.WriteLine("mod({0};{1})", left, right);
                return ModValue(left, right);
            }
            else 
            {
                Debug.WriteLine("div({0};{1})", left, right);
                return left / right;
            }
        }

        private double WalkLeft(Lab1CalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<Lab1CalculatorParser.ExpressionContext>(0));
        }

        private double WalkRight(Lab1CalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<Lab1CalculatorParser.ExpressionContext>(1));
        }
    }
}
