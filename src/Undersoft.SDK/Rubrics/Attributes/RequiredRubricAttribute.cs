﻿namespace Undersoft.SDK.Rubrics.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RequiredRubricAttribute : RubricAttribute
    {
        public RequiredRubricAttribute() { }
    }
}
