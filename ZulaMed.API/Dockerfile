﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ZulaMed.API.csproj", "."]
RUN dotnet restore "ZulaMed.API.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "ZulaMed.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZulaMed.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG CONNECTION_STRING=null
ARG MUX_SIGNING_KEY
ARG MUX_SECRET
ARG FIREBASE_API_KEY
ENV DATABASE_CONNECTION_STRING=$CONNECTION_STRING
ENV MUX_SIGNING_KEY=$MUX_SIGNING_KEY
ENV MUX_SECRET=$MUX_SECRET
ENV FIREBASE_API_KEY=$FIREBASE_API_KEY
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZulaMed.API.dll"]
