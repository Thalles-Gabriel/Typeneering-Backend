using Typeneering.Application.Base.Validators;
using Typeneering.Application.Sessions.Contracts.Requests;

namespace Typeneering.Application.Sessions.Validators;

public class LeaderboardValidator : PagedRequestValidator<LeaderboardRequest>
{
    public LeaderboardValidator() : base()
    {
        
    }
}
