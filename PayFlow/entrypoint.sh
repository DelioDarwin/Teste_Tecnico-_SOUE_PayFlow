#!/bin/bash
set -e

# Aguarda o SQL Server ficar disponível e aplica as migrations
until dotnet ef database update; do
  echo "Aguardando o banco de dados ficar disponível..."
  sleep 3
done

# Inicia a aplicação
dotnet payflow.dll