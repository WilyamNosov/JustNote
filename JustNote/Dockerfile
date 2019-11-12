#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk-nanoserver-sac2016 AS build
WORKDIR /src
COPY ["JustNote/JustNote.csproj", "JustNote/"]
RUN dotnet restore "JustNote/JustNote.csproj"
COPY . .
WORKDIR "/src/JustNote"
RUN dotnet build "JustNote.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JustNote.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JustNote.dll"]