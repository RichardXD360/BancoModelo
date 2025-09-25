using Application;
using Microsoft.Data.Sqlite;
using Shared.Models;
using System.Data;

namespace Data.Domain
{
    public class DataAccess: IData
    {
        string connectionString = "Data Source=banco.db";
        public void AbrirConexao()
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                var cmdCreateTableAgencia = conn.CreateCommand();
                cmdCreateTableAgencia.CommandText = @"
                CREATE TABLE IF NOT EXISTS AGENCIA (
                ID INTEGER PRIMARY KEY AUTOINCREMENT, NUMERO INTEGER, ENDERECO VARCHAR(3200))";
                cmdCreateTableAgencia.ExecuteNonQuery();

                var cmdCreateTableUsuario = conn.CreateCommand();
                cmdCreateTableUsuario.CommandText = @"
                CREATE TABLE IF NOT EXISTS USUARIO (
                ID INTEGER PRIMARY KEY AUTOINCREMENT, NOME VARCHAR(3200), LOGIN VARCHAR(3200) UNIQUE, CPF VARCHAR(3200) UNIQUE, AGENCIAID INTEGER, 
                FOREIGN KEY (AGENCIAID) REFERENCES AGENCIA(ID)
                    ON UPDATE CASCADE)";
                cmdCreateTableUsuario.ExecuteNonQuery();

                var cmdCreateTableUsuarioSaldo = conn.CreateCommand();
                cmdCreateTableUsuarioSaldo.CommandText = @"CREATE TABLE IF NOT EXISTS USUARIO_SALDO (
                ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                SALDO REAL, 
                USUARIOID INTEGER, 
                DATAULTIMATRANSACAO DATETIME,
                FOREIGN KEY (USUARIOID) REFERENCES USUARIO(ID)
                ON DELETE SET NULL)";
                cmdCreateTableUsuarioSaldo.ExecuteNonQuery();

                /*
                var cmdInsertAgenciaPadrao = conn.CreateCommand();
                cmdInsertAgenciaPadrao.CommandText = @"
                INSERT INTO AGENCIA (NUMERO, ENDERECO) VALUES (0001, 'AV. ANTONIO RUETA DE NOBREGA, 87 - SP')";
                cmdInsertAgenciaPadrao.ExecuteNonQuery();

                var cmdInsertUsuarioPadrao = conn.CreateCommand();
                cmdInsertUsuarioPadrao.CommandText = @"
                INSERT INTO USUARIO (NOME, CPF, LOGIN, AGENCIAID) VALUES ('USUARIO PADRAO', '123.456.789-10', 'USUARIO.PADRAO', 1)";
                cmdInsertUsuarioPadrao.ExecuteNonQuery();
                */
            }
        }
        public ResultadoRetorno VerificarUsuario(UsuarioDTC usuario)
        {
            ResultadoRetorno resultadoRetorno = new ResultadoRetorno();
            try
            {
                bool isValido = false;

                using var conn = new SqliteConnection(connectionString);
                conn.Open();

                using var cmdVerificarUsuario = conn.CreateCommand();
                cmdVerificarUsuario.CommandText = @"
                SELECT ID
                FROM USUARIO
                WHERE LOGIN = @login";
                cmdVerificarUsuario.Parameters.AddWithValue("@login", usuario.Login);

                using var reader = cmdVerificarUsuario.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetString(0);
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        isValido = true;
                    };
                };
                if (isValido)
                {
                    resultadoRetorno.Mensagem = "Usuário Válido";
                }
                else
                {
                    resultadoRetorno.Mensagem = "Usuário Inválido";
                }
                resultadoRetorno.Sucesso = isValido;
                return resultadoRetorno;
            }
            catch (SqliteException ex)
            {
                resultadoRetorno.Sucesso = false;
                resultadoRetorno.Mensagem = $"Erro de Banco de Dados: {ex}";
            }
            catch (ArgumentNullException ex)
            {
                resultadoRetorno.Sucesso = false;
                resultadoRetorno.Mensagem = $"Erro: {ex.ParamName} - {ex.Message}";
            }
            catch (Exception ex)
            {
                resultadoRetorno.Sucesso = false;
                resultadoRetorno.Mensagem = $"Erro: {ex.Message}";
            }
            return resultadoRetorno;
        }

        public ResultadoRetorno CriarUsuario(UsuarioDTC usuario)
        {
            ResultadoRetorno retorno = new ResultadoRetorno();
            retorno.Sucesso = false;
            try
            {
                using var conn = new SqliteConnection(connectionString);
                conn.Open();
                using var transaction = conn.BeginTransaction();

                using var cmdCriarUsuario = conn.CreateCommand();
                cmdCriarUsuario.Transaction = transaction;

                using var cmdCriarUsuarioSaldo = conn.CreateCommand();
                cmdCriarUsuarioSaldo.Transaction = transaction;

                using var cmdIdUsuario = conn.CreateCommand();
                cmdIdUsuario.Transaction = transaction;

                cmdCriarUsuario.CommandText = @"INSERT INTO USUARIO
                (NOME, LOGIN, CPF, AGENCIAID)
                VALUES (@nome, @login, @cpf, @agenciaid)";
                cmdCriarUsuario.Parameters.AddWithValue("@cpf", usuario.Cpf);
                cmdCriarUsuario.Parameters.AddWithValue("@nome", usuario.Nome);
                cmdCriarUsuario.Parameters.AddWithValue("@login", usuario.Login);
                cmdCriarUsuario.Parameters.AddWithValue("@agenciaid", usuario.AgenciaId);
                cmdCriarUsuario.ExecuteNonQuery();

                cmdIdUsuario.CommandText = @"
                SELECT ID
                FROM USUARIO
                WHERE CPF = @cpf";
                cmdIdUsuario.Parameters.AddWithValue("@cpf", usuario.Cpf);
                int usuarioId = Convert.ToInt32(cmdIdUsuario.ExecuteScalar()); //ExecuteScalar faz a consulta e já retorna o resultado

                cmdCriarUsuarioSaldo.CommandText = @"INSERT INTO USUARIO_SALDO
                (SALDO, USUARIOID, DATAULTIMATRANSACAO)
                VALUES (0, @usuarioid, NULL)";
                cmdCriarUsuarioSaldo.Parameters.AddWithValue("@usuarioid", usuarioId);

                cmdCriarUsuarioSaldo.ExecuteNonQuery();

                transaction.Commit();

                retorno.Mensagem = "Sucesso ao criar usuário";
                retorno.Sucesso = true;

                return retorno;
            }
            catch (SqliteException ex)
            {
                retorno.Mensagem = $"Erro de banco de dados: {ex}";
            }
            catch(ArgumentException ex)
            {
                retorno.Mensagem = $"Erro: {ex.ParamName} - {ex.Message}";
            }
            catch(Exception ex)
            {
                retorno.Mensagem = $"Erro: {ex.Message}";
            }
            
            return retorno;
        }
    }
}
