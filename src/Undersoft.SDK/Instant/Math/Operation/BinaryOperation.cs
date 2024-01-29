namespace Undersoft.SDK.Instant.Math.Operation
{
    using System;
    using Binary.Formulas;
    using Formulas;
    using Binary.Operator;
    using Set;
    using System.Reflection.Emit;
    using Undersoft.SDK.Instant.Math;
    using Undersoft.SDK.Instant.Math.Formulas;

    [Serializable]
    public class BinaryOperation : BinaryFormula
    {
        internal BinaryOperator oper;

        public BinaryOperation(Formula e1, Formula e2, BinaryOperator op) : base(e1, e2)
        {
            oper = op;
        }

        public override MathSetSize Size
        {
            get { return expr1.Size == MathSetSize.Scalar ? expr2.Size : expr1.Size; }
        }

        public override void Compile(ILGenerator g, InstantMathCompilerContext cc)
        {
            expr1.Compile(g, cc);
            expr2.Compile(g, cc);
            if (cc.IsFirstPass())
                return;
            oper.Compile(g);
        }
    }
}
