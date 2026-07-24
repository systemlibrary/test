
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace SystemLibrary.Common.Framework;

internal class ExtendedActionConstraint : IActionConstraint
{
    public int Order => -899;

    const string ActionWinnerKey = "slcf__ActionWinner";
    const string ActionWinnerKeyDistance = "slcf__ActionWinner_Distance";

    public bool Accept(ActionConstraintContext context)
    {
        if (context.Candidates.Count <= 1) return true;

        var httpContext = context.RouteContext.HttpContext;
        var queryCount = httpContext.Request.Query.Count;
        var currentCandidate = context.CurrentCandidate;

        if (queryCount == 0)
        {
            var parameterlessCandidate = context.Candidates.FirstOrDefault(c => c.Action.Parameters.Count == 0);

            return currentCandidate.Action == parameterlessCandidate.Action;
        }

        if (httpContext.Items.TryGetValue(ActionWinnerKey, out object bestAction))
        {
            return ReferenceEquals((ActionDescriptor)bestAction, currentCandidate.Action);
        }

        int bestDistance = 9999;

        foreach (var candidate in context.Candidates)
        {
            var parameters = candidate.Action.Parameters;

            var parameterCount = parameters.Count;

            if (parameterCount == 0) continue;

            if (parameterCount < queryCount) continue;

            var distance = parameterCount - queryCount;

            if (distance < bestDistance)
            {
                bool hasMatch = false;

                foreach (var parameter in parameters)
                {
                    if (httpContext.Request.Query.ContainsKey(parameter.Name))
                    {
                        hasMatch = true;
                        break;
                    }
                }

                if (!hasMatch) continue;

                bestDistance = distance;
                bestAction = candidate.Action;
            }
        }

        httpContext.Items[ActionWinnerKey] = bestAction;

        return ReferenceEquals(currentCandidate.Action, (ActionDescriptor)bestAction);
    }
}