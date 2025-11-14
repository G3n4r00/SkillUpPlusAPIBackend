# --- Estágio 1: Compilação (Build) ---
# Usamos a imagem completa do SDK do .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto (.csproj) primeiro para otimizar o cache do Docker
# Isso evita baixar dependências novamente se apenas o código-fonte mudar
COPY *.sln .
COPY SkillUpPlus.API/*.csproj ./SkillUpPlus.API/

# Restaura todas as dependências
RUN dotnet restore

# Copia todo o resto do código-fonte
COPY . .
WORKDIR /src/SkillUpPlus.API

# Publica o aplicativo em modo Release, otimizado para produção
RUN dotnet publish -c Release -o /app/publish

# --- Estágio 2: Imagem Final (Final) ---
# Usamos a imagem leve do runtime do ASP.NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expõe a porta 8080 que você solicitou
EXPOSE 8080

# Define a variável de ambiente para que o Kestrel (servidor .NET)
# ouça na porta 8080 em todas as interfaces de rede dentro do container
ENV ASPNETCORE_URLS=http://+:8080

# Ponto de entrada: como iniciar o aplicativo
ENTRYPOINT ["dotnet", "SkillUpPlus.API.dll"]