﻿namespace KRFTemplateApi.App.CQRS.Sample.Command
{
    using System.Net;

    using FluentValidation;

    using KRFCommon.CQRS.Validator;

    using KRFTemplateApi.Domain.CQRS.Sample.Command;

    public class PostSampleDataValidator : KRFValidator<SampleCommandInput>, IKRFValidator<SampleCommandInput>
    {
        public PostSampleDataValidator() : base()
        {
            this.RuleFor( r => r.Min )
                .LessThan( r => r.Max )
                .WithErrorCode( HttpStatusCode.BadRequest.ToString() )
                .WithMessage( "Min Value must be smaller than Max Value" );


            this.RuleFor( r => r.Code )
                .NotNull()
                .WithErrorCode( HttpStatusCode.BadRequest.ToString() )
                .WithMessage( "You must add a Code" )
                .NotEmpty()
                .WithErrorCode( HttpStatusCode.OK.ToString() )
                .WithMessage( "Code Value cannot be empty" );

            this.RuleFor( r => r.Description )
                .NotNull()
                .WithErrorCode( HttpStatusCode.BadRequest.ToString() )
                .WithMessage( "You must add a Description" )
                .NotEmpty()
                .WithErrorCode( HttpStatusCode.OK.ToString() )
                .WithMessage( "Description Value cannot be empty" );
        }
    }
}
