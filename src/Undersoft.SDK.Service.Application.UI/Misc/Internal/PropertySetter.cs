// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET6_0_OR_GREATER
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Undersoft.SDK.Instant.Proxies;

namespace Microsoft.AspNetCore.Components.Reflection;

[ExcludeFromCodeCoverage]
internal sealed class PropertySetter
{
    private int _index;

    public PropertySetter(Type targetType, PropertyInfo property)
    {
        if (property.SetMethod == null)
        {
            throw new InvalidOperationException("Cannot provide a value for property " +
                $"'{property.Name}' on type '{targetType.FullName}' because the property " +
                "has no setter.");
        }

        _index = targetType.ToProxy().Rubrics[property.Name].RubricId;
    }

    public bool Cascading { get; init; }

    public void SetValue(object target, object value) => target.ValueOf(_index, value);
    
}
#endif
