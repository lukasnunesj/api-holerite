# Usar a imagem base do .NET SDK para construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Definir o diretório de trabalho
WORKDIR /app

# Copiar os arquivos de solução e de projeto para o contêiner
COPY HoleriteAPI.sln ./
COPY HoleriteAPI.Core/HoleriteAPI.Core.Application/HoleriteAPI.Core.Application.csproj ./HoleriteAPI.Core/HoleriteAPI.Core.Application/
COPY HoleriteAPI.Core/HoleriteAPI.Core.Domain/HoleriteAPI.Core.Domain.csproj ./HoleriteAPI.Core/HoleriteAPI.Core.Domain/
COPY HoleriteAPI.Data/HoleriteAPI.Data.csproj ./HoleriteAPI.Data/
COPY HoleriteAPI.Consumer/HoleriteAPI.Consumer.csproj ./HoleriteAPI.Consumer/

# Restaurar as dependências
RUN dotnet restore

# Copiar todos os arquivos para o contêiner
COPY . .

# Publicar a aplicação
RUN dotnet publish HoleriteAPI.Consumer/HoleriteAPI.Consumer.csproj -c Release -o /out

# Usar uma imagem base do runtime do .NET para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Definir o diretório de trabalho
WORKDIR /app

# Copiar os arquivos publicados do estágio de build
COPY --from=build /out .

# Expor a porta que a aplicação vai rodar
EXPOSE 8080

# Definir o comando de entrada
ENTRYPOINT ["dotnet", "HoleriteAPI.Consumer.dll"]
