namespace Undersoft.SDK.Instant.Math.Operation.Unsigned.Formulas
{
    using System;
    using Set;
    using Math.Formulas;
    using System.Reflection.Emit;
    using Undersoft.SDK.Instant.Math;
    using Undersoft.SDK.Instant.Math.Formulas;
    using Undersoft.SDK.Instant.Math.Set;

    [Serializable]
    public class UnsignedFormula : Formula
    {
        internal double thevalue;

        public UnsignedFormula(double vv)
        {
            thevalue = vv;
        }

        public override MathSetSize Size
        {
            get { return MathSetSize.Scalar; }
        }

        public override void Compile(ILGenerator g, InstantMathCompilerContext cc)
        {
            if (cc.IsFirstPass())
                return;
            g.Emit(OpCodes.Ldc_R8, thevalue);
        }
    }
}
