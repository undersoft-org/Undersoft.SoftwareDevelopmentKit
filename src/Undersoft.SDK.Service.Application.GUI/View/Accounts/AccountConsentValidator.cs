﻿// ********************************************************
//   Copyright (c) Undersoft. All Rights Reserved.
//   Licensed under the MIT License. 
//   author: Dariusz Hanc
//   email: dh@undersoft.pl
//   library: Undersoft.SVC.Service.Application.GUI
// ********************************************************

namespace Undersoft.SDK.Service.Application.GUI.View.Accounts;

using Undersoft.SDK.Service.Access.Licensing;

/// <summary>
/// The account validator.
/// </summary>
public class AccountConsentValidator<TModel> : ViewValidator<TModel> where TModel : Consent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountValidator"/> class.
    /// </summary>
    /// <param name="servicer">The servicer.</param>
    public AccountConsentValidator(IServicer servicer) : base(servicer)
    {
        ValidationScope(
            OperationType.Any,
            () =>
            {
                ValidateRequired(p => p.Model.TermsConsent);
                ValidateRequired(p => p.Model.PersonalDataConsent);
            }
        );
    }
}