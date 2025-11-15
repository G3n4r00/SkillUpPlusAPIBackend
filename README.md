# Backend da API SkillUpPlus 2030+

Este repositório contém o código-fonte do backend da plataforma móvel de microlearning e requalificação profissional SkillUpPlus 2030+.

A plataforma oferece trilhas de aprendizado personalizadas e gamificação (badges, XP) para capacitar profissionais em habilidades emergentes, em linha com os Objetivos de Desenvolvimento Sustentável (ODS) 4 (Educação de Qualidade) e 8 (Trabalho Decente e Crescimento Econômico) da ONU.

## 🚀 Tecnologias Principais

Este backend é um monólito modular construído com uma arquitetura moderna e pronta para a nuvem:

* Framework: C# / .NET 8 (ASP.NET Core Web API)

* Banco de Dados: PostgreSQL 16

* Autenticação: ASP.NET Core Identity

* Autorização: JWT

* Arquitetura de Deploy: Docker

* Infraestrutura: Nginx (como Proxy Reverso para terminação SSL)

## 🏛️ Arquitetura de Implantação (Deploy)

A aplicação é projetada para rodar inteiramente em containers Docker orquestrados pelo Docker Compose.

A arquitetura de produção consiste em 3 containers principais que se comunicam através de uma rede Docker privada:

1. skillup_proxy (Nginx):

    * É o único container exposto para a internet (Entrypoint).

    * Ouve nas portas 80 (para renovação de certificado) e 8443 (HTTPS).

    * Realiza a terminação SSL, usando os certificados Let's Encrypt montados a partir do host da VM.

    * Atua como Proxy Reverso, encaminhando o tráfego descriptografado (HTTP) para o container da API.

2. skillup_api (ASP.NET 8):

    * Não é exposto publicamente. Ouve apenas na porta interna 8080.

    * Executa toda a lógica de negócios da aplicação.

    * Comunica-se com o banco de dados através do nome do serviço db na rede interna.

3. skillup_db (PostgreSQL):

    * Não é exposto publicamente. Ouve apenas na porta interna 5432.

    * Persiste todos os dados reais da aplicação no volume /opt/skillup_db_data do host, garantindo que os dados sobrevivam a reinicializações.

### Diagrama da Arquitetura 

<img width="1849" height="635" alt="image" src="https://github.com/user-attachments/assets/4040d665-d2f5-4ca1-9753-03c04d4b8ac3" />

### Versioning

Esta API implementa o Versionamento de API para garantir que futuras atualizações não quebrem os aplicativos móveis existentes.

#### Estratégia

* Controle via URL: A versão é definida no segmento da URL (ex: /api/v1/..., /api/v2/...).

* Suporte Lado a Lado: Um único Controller (ex: ProfileController) pode servir múltiplos endpoints para múltiplas versões (ex: GET /api/v1/profile/me e GET /api/v2/profile/me), permitindo a evolução da API sem duplicação de código.

* Documentação: O Swagger detecta automaticamente as versões disponíveis e fornece um dropdown para selecionar e testar cada versão da API.

### Versões Atuais

v1.0 (Base)

A v1 representa a funcionalidade central do MVP:

    * Registro e Login (/api/v1/auth).

    * Catálogo e Módulos de Trilhas (/api/v1/tracks).

    * Sistema de Progresso e Badges (/api/v1/profile).

    * Onboarding de Interesses (/api/v1/onboarding).

v2.0 (Gamificação Avançada)

A v2 introduz a "Gamificação Avançada" (Req. 11), adicionando Pontos de Experiência (XP) e Leaderboards:

    * [v2] POST /api/v2/profile/progress: Agora retorna o xpGained (XP ganho) ao completar um módulo.

    * [v2] GET /api/v2/profile/me: Agora retorna o totalXp (XP total) do usuário.

    * [v2] GET /api/v2/leaderboard: Um novo endpoint que retorna o Top 10 de usuários da plataforma, ordenados por XP.

## 📚 Documentação da API (Endpoints)

A documentação interativa completa, com todos os schemas e a capacidade de testar os endpoints, está disponível no Swagger da aplicação.

<a href="rm551986.admninistradorlinux.com.br">URL de acesso: rm551986.admninistradorlinux.com.br</a> 

#### Autenticação (v1)

* POST /api/v1/auth/register: Registra um novo usuário (Nome, E-mail, Senha).

* POST /api/v1/auth/login: Autentica um usuário e retorna um Token JWT válido por 7 dias.

#### Testando Endpoints Protegidos

Todos os endpoints (exceto Login/Registro) são protegidos ([Authorize]). Para testá-los via Swagger:

Use POST /api/v1/auth/login (ou /register) para obter um token.

Clique no botão "Authorize" 🔒 no topo da página.

Na caixa de texto, digite Bearer (com espaço) e cole seu token.
Exemplo:
```bash
Bearer numeros123456eLetrasDoToken
```

Clique em "Authorize". Todos os seus testes subsequentes agora incluirão o token.

### Endpoints Principais (v1 & v2)

* GET /api/v1/tracks: Retorna o catálogo de trilhas (JSON).

* GET /api/v1/tracks/{id}: Retorna os detalhes e módulos de uma trilha (JSON).

* POST /api/v1/onboarding: Salva as tags de interesse do usuário (JSON).

* POST /api/v2/profile/progress: Marca um módulo como concluído e retorna o XP ganho (JSON).

* GET /api/v2/profile/me: Retorna o dashboard completo do usuário, incluindo XP e badges (JSON).

* GET /api/v2/leaderboard: Retorna o Top 10 do ranking (JSON).

## ⚙️ Guia de Implantação (Deploy na VM Azure)

Este foi o processo que seguimos utilizando uma VM Debian com Docker e docker-compose instalados assim como um domínio apontando para o IP da VM.

1. Pré-requisitos (No Host da VM)

```bash
# 1. Crie a rede Docker
sudo docker network create --subnet=10.11.200.0/24 skillup_net

# 2. Crie os diretórios de volumes persistentes
sudo mkdir -p /opt/skillup_db_data
sudo mkdir -p /var/lib/docker/gs     
sudo mkdir -p /var/www/certbot       
```

2. Gerar Certificado SSL (Let's Encrypt)

```bash
# 1. Instale o Certbot
sudo apt update && sudo apt install certbot -y

# 2. Gere o certificado
sudo certbot certonly --standalone -d [SEU_DOMINIO.COM]
```

3. Publicar a Imagem (Na Máquina de DEV)

```bash
# 1. Construa a imagem final (ex: v3.0)
docker build -t [SEU_USUARIO_DOCKERHUB]/skillup-api:3.0 .

# 2. Publique no Docker Hub (Req 06)
docker login
docker push [SEU_USUARIO_DOCKERHUB]/skillup-api:3.0
```

4. Criar os Arquivos de Orquestração (No Host da VM)

Crie uma pasta de deploy (ex: ~/skillup-deploy) e coloque nela os dois arquivos a seguir:

* Arquivo 1: nginx/nginx.conf (Crie este arquivo. Ele deve conter a configuração do proxy reverso para api:8080, os caminhos para os certificados SSL e o bloco listen 80 para renovação do Certbot).

* Arquivo 2: docker-compose.ssl.yml (Este é o arquivo principal que orquestra os 3 containers: proxy, api e db, com os healthchecks e volumes corretos).

5. Iniciar a Aplicação

Na VM, dentro da pasta ~/skillup-deploy:

```bash
# 1. Baixe a imagem mais recente da API
sudo docker-compose -f docker-compose.ssl.yml pull api

# 2. Suba a pilha completa
sudo docker-compose -f docker-compose.ssl.yml up -d
```

Aplicação então estará no ar e acessível em https://[SEU_DOMINIO.COM]:8443/swagger.

## O time


