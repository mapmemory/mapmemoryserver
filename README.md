# 🗺 Mapa de Memórias 

Respositório com a API back end do app Mapa de memórias.

## Requisitos da aplicação

- Docker;
- Docker Compose.

[-> Instalação Docker](https://docs.google.com/document/d/1ktNOhnK6ty9N4Ejly5cqk1Q5YBMh8TWpdxZ0HmTmMcc/edit?usp=sharing).

## Ambiente de desenvolvimento

1. Clone o repositório:

    ```sh
    git clone https://github.com/mapmemory/mapmemoryserver.git

    cd mapmemoryserver/
    ```

2. Compile e execute os containers do Docker
    ```sh
    docker-compose up --build
    ```

    Esse comando irá configurar a:    
    - API rodando na porta 5000;
    - O banco de dados MySQL rodando na porta 3306.

3. Para parar os containers:
    ```sh
    docker-compose down
    ```

## Documentação da API

A documentação via Swagger UI se tornará disponível em [`http://localhost:5000/swagger`](http://localhost:5000/swagger) após os containers serem executados.

## Autenticação

Para autenticar, use a rota `/api/auth/login`, para testes, têm-se as seguintes credenciais:
- Usuário: `testuser`;
- Senha: `password`;

A resposta da API irá incluir um token JWT que poderá ser utilizado para acessar as rotas protegidas do usuário.

## Banco de dados

A API se conecta a um banco de dados MySQL. As credenciais padrão são:

- Nome do banco de dados: `mapmemorydb`;
- Usuário: `apiuser`;
- Senha: `apipassword`.

Para mudar as configurações do banco de dados, atualize o arquivo `docker-compose.yml`.

## Colaboradores

- Ezequiel Dannus;
- Felipe Estevo Gomes;
- Luis Felipe Assmann;
- Mathias Gabriel Schneider Steffen;
- Victor Emanuel Mello.

# Licença

- MIT License.