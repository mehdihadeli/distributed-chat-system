# https://devblogs.microsoft.com/nuget/microsoft-author-signing-certificate-update/
# https://github.com/NuGet/Home/issues/10491

# the first, heavier image to build your code

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS builder

# Setup working directory for the project	 
WORKDIR /app

RUN curl -o /usr/local/share/ca-certificates/verisign.crt -SsL https://crt.sh/?d=1039083 && update-ca-certificates
COPY ./src/Chat.Core/Chat.Core.csproj ./src/Chat.Core/ 
COPY ./src/Chat.Application/Chat.Application.csproj ./src/Chat.Application/
COPY ./src/Chat.Infrastructure/Chat.Infrastructure.csproj ./src/Chat.Infrastructure/
COPY ./src/Chat.Web/Chat.Web.csproj ./src/Chat.Web/
 
# Restore nuget packages	 
RUN dotnet restore ./src/Chat.Web/Chat.Web.csproj 

# Copy project files
COPY ./src/Chat.Core ./src/Chat.Core/ 
COPY ./src/Chat.Application ./src/Chat.Application/
COPY ./src/Chat.Infrastructure ./src/Chat.Infrastructure/
COPY ./src/Chat.Web ./src/Chat.Web/

# Build project with Release configuration
# and no restore, as we did it already
RUN dotnet build -c Release --no-restore ./src/Chat.Web/Chat.Web.csproj

# Publish project to output folder	 
# and no build, as we did it already	
WORKDIR /app/src/Chat.Web
RUN dotnet publish -c Release --no-build -o out


# second, final, lighter image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine

# Setup working directory for the project  
WORKDIR /app

# Copy published in previous stage binaries	 
  
# from the `builder` image
COPY --from=builder /app/src/Chat.Web/out  .		

ENV ASPNETCORE_URLS https://*:5001;http://*:5000
ENV ASPNETCORE_ENVIRONMENT docker  

# sets entry point command to automatically	 
# run application on `docker run`	 
ENTRYPOINT ["dotnet", "Chat.Web.dll"]