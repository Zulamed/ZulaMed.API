FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base

# install ffmpeg
RUN apk update
RUN apk upgrade
RUN apk add --no-cache ffmpeg

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["ZulaMed.VideoConversion.csproj", "."]
RUN dotnet restore "ZulaMed.VideoConversion.csproj"
COPY . .
WORKDIR /src 
RUN dotnet build "ZulaMed.VideoConversion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZulaMed.VideoConversion.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZulaMed.VideoConversion.dll"]
