FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
WORKDIR /app

RUN apt-get update && \
    apt-get install -y wget unzip curl gnupg software-properties-common && \
    wget https://releases.hashicorp.com/terraform/1.7.5/terraform_1.7.5_linux_amd64.zip && \
    unzip terraform_1.7.5_linux_amd64.zip && \
    mv terraform /usr/local/bin/ && \
    terraform -version

FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src
COPY MCPServer/ ./MCPServer/
RUN dotnet publish ./MCPServer -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MCPServer.dll"]
