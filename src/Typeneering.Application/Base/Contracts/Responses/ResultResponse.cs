using System.Collections.ObjectModel;

namespace Typeneering.Application.Base.Contracts.Responses;

public sealed record ResultResponse(string Title, string Message);
public sealed record ResultResponse<T>(string Title, string Message, IReadOnlyList<T> Data, int TotalCount);
