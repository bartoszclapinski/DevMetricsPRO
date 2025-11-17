using DevMetricsPro.Application.DTOs.GitHub;
using FluentValidation;

namespace DevMetricsPro.Application.Validators;

public class GitHubCallbackRequestValidator : AbstractValidator<GitHubCallbackRequest>
{
    public GitHubCallbackRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.State).NotEmpty();
    }
}