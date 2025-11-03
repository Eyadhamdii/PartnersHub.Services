using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace PartnersHub.InnovationHub.Application.Challenge.Queries.ChallengeRequest
{
    public class ChallengeRequestListValidator : AbstractValidator<ChallengeRequestListQuery>
    {
        public ChallengeRequestListValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

            //as Per US only search if Chars at least 3
            When(x => !string.IsNullOrWhiteSpace(x.Search), () =>
            {
                RuleFor(x => x.Search!).MinimumLength(3);
            });
        }
    }
}
