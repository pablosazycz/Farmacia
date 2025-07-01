# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY Farmacia/*.csproj ./Farmacia/
RUN dotnet restore

COPY . . 
WORKDIR /app/Farmacia
RUN dotnet publish -c Release -o /out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./

ENTRYPOINT ["dotnet", "Farmacia.dll"]
