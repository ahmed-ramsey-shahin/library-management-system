# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build

# Copy project files
COPY ["src/Lms.Api/Lms.Api.csproj", "src/Lms.Api/"]
COPY ["src/Lms.Application/Lms.Application.csproj", "src/Lms.Application/"]
COPY ["src/Lms.Domain/Lms.Domain.csproj", "src/Lms.Domain/"]
COPY ["src/Lms.Infrastructure/Lms.Infrastructure.csproj", "src/Lms.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Lms.Api/Lms.Api.csproj"

# Copy all source code
COPY . .

# Build and publish
RUN dotnet publish "src/Lms.Api/Lms.Api.csproj" -c Release -o /app

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

RUN apt-get update && apt-get install -y tzdata && \
    ln -fs /usr/share/zoneinfo/America/Montreal /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata && \
    rm -rf /var/lib/apt/lists/*

ENV TZ=America/Montreal

WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT [ "dotnet", "Lms.Api.dll" ]
