namespace Typeneering.Application.Base.Contracts.Requests;

public abstract record PagedResultRequest(int Take = 20, int Skip = 0);
