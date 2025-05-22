FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
COPY . .
RUN dotnet restore
COPY . .
RUN dotnet build --no-restore -c Release 

FROM build AS publish
RUN dotnet publish -c Release /p:UseAppHost=false -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./
ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid "${UID}" \
    appuser
USER appuser
ENTRYPOINT [ "dotnet", "Typeneering.HostApi.dll" ]
