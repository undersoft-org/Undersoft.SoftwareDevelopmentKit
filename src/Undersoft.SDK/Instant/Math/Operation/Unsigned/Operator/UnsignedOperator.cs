namespace Undersoft.SDK.Instant.Math.Operation.Unsigned.Operator
{
    using System;
    using Set;
    using Math.Formulas;
    using System.Reflection.Emit;
    using Undersoft.SDK.Instant.Math;
    using Undersoft.SDK.Instant.Math.Formulas;
    using Undersoft.SDK.Instant.Math.Set;

    [Serializable]
    public class UnsignedOperator : Formula
    {
        protected Formula e;

        public UnsignedOperator(Formula ee)
        {
            e = ee;
        }

        public override MathSetSize Size
        {
            get { return e.Size; }
        }

        public override void Compile(ILGenerator g, InstantMathCompilerContext cc)
        {
            e.Compile(g, cc);
        }
    }
}
