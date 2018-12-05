FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update && apt-get install -y \
    wget \
    unzip \
  && rm -rf /var/lib/apt/lists/*

RUN wget --quiet https://releases.hashicorp.com/terraform/0.11.10/terraform_0.11.10_linux_amd64.zip \
  && unzip terraform_0.11.10_linux_amd64.zip \
  && mv terraform /usr/bin \
  && rm terraform_0.11.10_linux_amd64.zip

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


ENV PAT=ejyfdwegf6hpxlthjflgrtg2fokvq3qf3kvreasdc4c7ir7trlia
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_HTTPS_PORT=44377
ENTRYPOINT ["dotnet", "DevOps.Terraform.PullRequest.dll"]

