# ==========================================
# Buổi 13: Dockerfile cho ASP.NET Core Web API (.NET 10)
# ==========================================

# 1. Base Image SDK để biên dịch và publish dự án
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy file .csproj và restore dependencies trước (tận dụng cache Docker layer)
COPY ["DemoAPINET.csproj", "./"]
RUN dotnet restore "DemoAPINET.csproj"

# Copy toàn bộ mã nguồn và biên dịch ở chế độ Release
COPY . .
RUN dotnet publish "DemoAPINET.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Base Image ASP.NET Runtime nhẹ để chạy ứng dụng (không kèm SDK)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Cấu hình cổng lắng nghe mặc định
EXPOSE 8080
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:8080

# Sao chép các file đã publish từ stage build
COPY --from=build /app/publish .

# Lệnh khởi chạy ứng dụng
ENTRYPOINT ["dotnet", "DemoAPINET.dll"]
