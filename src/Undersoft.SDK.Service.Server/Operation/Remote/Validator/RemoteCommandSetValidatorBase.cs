using FluentValidation;
using Undersoft.SDK.Service.Server.Operation.Command;
using System.Globalization;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Validator;

using Instant.Proxies;
using Undersoft.SDK.Service.Server.Operation.Remote.Command;

using Undersoft.SDK.Service.Data.Query;
using Undersoft.SDK.Service.Data.Store;

public abstract class RemoteCommandSetValidatorBase<TCommand> : AbstractValidator<TCommand>
    where TCommand : IRemoteCommandSet
{
    protected static readonly string[] SupportedLanguages;

    protected readonly IServicer _servicer;

    static RemoteCommandSetValidatorBase()
    {
        SupportedLanguages = CultureInfo
            .GetCultures(CultureTypes.NeutralCultures)
            .Select(c => c.TwoLetterISOLanguageName)
            .Distinct()
            .ToArray();
    }

    public RemoteCommandSetValidatorBase(IServicer servicer)
    {
        _servicer = servicer;
    }

    protected virtual void ValidateLimit(int min, int max)
    {
        RuleFor(a => a)
            .Must(a => a.Commands.Count() >= min)
            .WithMessage($"Items count below minimum quantity")
            .Must(a => a.Commands.Count() <= max)
            .WithMessage($"Items count above maximum quantity");
    }

    protected void ValidateRequired(params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            RuleForEach(a => a.Commands)
                .ChildRules(
                    c =>
                        c.RuleFor(r => r.Model.ValueOf(propertyName))
                            .NotEmpty()
                            .WithMessage(a => $"{propertyName} is required!")
                );
        }
    }

    protected void ValidateLanguage(params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            RuleForEach(a => a.Commands)
                .ChildRules(
                    c =>
                        c.RuleFor(r => r.Model.ValueOf(propertyName))
                            .Must(SupportedLanguages.Contains)
                            .WithMessage("Agreement language must conform to ISO 639-1.")
                );
        }
    }

    protected void ValidateEqual(object item, params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            RuleForEach(a => a.Commands)
                .ChildRules(
                    c =>
                        c.RuleFor(r => r.Model.ValueOf(propertyName))
                            .Equal(item)
                            .WithMessage($"{propertyName} is equal: {item}")
                );
        }
    }

    protected void ValidateNotEqual(object item, params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            RuleForEach(a => a.Commands)
                .ChildRules(
                    c =>
                        c.RuleFor(r => r.Model.ValueOf(propertyName))
                            .NotEqual(item)
                            .WithMessage($"{propertyName} is not equal: {item}")
                );
        }
    }

    protected void ValidateLength(int min, int max, params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            RuleForEach(a => a.Commands)
                .ChildRules(
                    c =>
                        c.RuleFor(r => r.Model.ValueOf(propertyName).ToString())
                            .MinimumLength(min)
                            .WithMessage($"{propertyName} minimum text rubricCount: {max} characters")
                            .MaximumLength(max)
                            .WithMessage($"{propertyName} maximum text rubricCount: {max} characters")
                );
        }
    }

    protected void ValidateEnum(params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            RuleForEach(a => a.Commands)
                .ChildRules(
                    c =>
                        c.RuleFor(r => r.Model.ValueOf(propertyName))
                            .IsInEnum()
                            .WithMessage($"Incorrect {propertyName} number")
                );
        }
    }

    protected void ValidateEmail(params string[] emailPropertyNames)
    {
        foreach (string emailPropertyName in emailPropertyNames)
        {
            RuleForEach(a => a.Commands)
                .ChildRules(
                    c =>
                        c.RuleFor(r => r.Model.ValueOf(emailPropertyName).ToString())
                            .EmailAddress()
                            .WithMessage($"Invalid {emailPropertyName} address.")
                );
        }
    }

    protected void ValidateExist<TStore, TEntity>(
        LogicOperand operand,
        params string[] propertyNames
    )
        where TEntity : class, IOrigin, IInnerProxy
        where TStore : IDataServerStore
    {
        var _repository = _servicer.Use<TStore, TEntity>();

        RuleForEach(a => a.Commands)
            .ChildRules(
                c =>
                    c.RuleFor(r => r.Model)
                        .MustAsync(
                            async (cmd, cancel) =>
                            {
                                return await _repository.Exist(
                                    buildPredicate<TEntity>(
                                        (IInnerProxy)cmd,
                                        operand,
                                        propertyNames
                                    )
                                );
                            }
                        )
                        .WithMessage($"{typeof(TEntity).Name} already exists")
            );
    }

    protected void ValidateNotExist<TStore, TEntity>(
        LogicOperand operand,
        params string[] propertyNames
    )
        where TEntity : class, IOrigin, IInnerProxy
        where TStore : IDataServerStore
    {
        var _repository = _servicer.Use<TStore, TEntity>();

        RuleForEach(a => a.Commands)
            .ChildRules(
                c =>
                    c.RuleFor(r => r.Model)
                        .MustAsync(
                            async (cmd, cancel) =>
                            {
                                return await _repository.NotExist(
                                    buildPredicate<TEntity>(
                                        (IInnerProxy)cmd,
                                        operand,
                                        propertyNames
                                    )
                                );
                            }
                        )
                        .WithMessage($"{typeof(TEntity).Name} does not exists")
            );
    }

    private Expression<Func<TEntity, bool>> buildPredicate<TEntity>(
        IInnerProxy dataInput,
        LogicOperand operand,
        params string[] propertyNames
    ) where TEntity : IInnerProxy
    {
        Expression<Func<TEntity, bool>> predicate =
            operand == LogicOperand.And ? predicate = e => true : predicate = e => false;
        foreach (var item in propertyNames)
        {
            predicate =
                operand == LogicOperand.And
                    ? predicate.And(e => e.Proxy[item] == dataInput.Proxy[item])
                    : predicate.Or(e => e.Proxy[item] == dataInput.Proxy[item]);
        }
        return predicate;
    }
}
