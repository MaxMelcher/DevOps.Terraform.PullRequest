FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 56926
EXPOSE 44377

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["DevOps.Terraform.PullRequest/DevOps.Terraform.PullRequest.csproj", "DevOps.Terraform.PullRequest/"]
RUN dotnet restore "DevOps.Terraform.PullRequest/DevOps.Terraform.PullRequest.csproj"
COPY . .
WORKDIR "/src/DevOps.Terraform.PullRequest"
RUN dotnet build "DevOps.Terraform.PullRequest.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DevOps.Terraform.PullRequest.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DevOps.Terraform.PullRequest.dll"]