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
                ID INTEGER PRIMARY KEY AUTOINCREMENT, NUMERO INTEGER, ENDERECO TEXT)";
                cmdCreateTableAgencia.ExecuteNonQuery();

                var cmdCreateTableUsuario = conn.CreateCommand();
                cmdCreateTableUsuario.CommandText = @"
                CREATE TABLE IF NOT EXISTS USUARIO (
                ID INTEGER PRIMARY KEY AUTOINCREMENT, NOME TEXT, LOGIN TEXT UNIQUE, CPF TEXT UNIQUE, AGENCIAID INTEGER, 
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

                var cmdCreateTableTipoTransacao = conn.CreateCommand();
                cmdCreateTableTipoTransacao.CommandText = @"CREATE TABLE IF NOT EXISTS TIPOTRANSACAO (
                ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                DESCRICAO TEXT)";
                cmdCreateTableTipoTransacao.ExecuteNonQuery();

                var cmdCreateTableTransacao = conn.CreateCommand();
                cmdCreateTableTransacao.CommandText = @"CREATE TABLE IF NOT EXISTS TRANSACAO (
                ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                DESCRICAO TEXT,
                VALOR REAL, 
                USUARIOID INTEGER, 
                USUARIORECEBEDORID INTEGER,
                DATATRANSACAO DATETIME,
                TIPOTRANSACAOID INTEGER,
                FOREIGN KEY (USUARIOID) REFERENCES USUARIO(ID)
                ON DELETE SET NULL,
                FOREIGN KEY (USUARIORECEBEDORID) REFERENCES USUARIO(ID)
                ON DELETE SET NULL)";
                cmdCreateTableTransacao.ExecuteNonQuery();

                var cmdInsertTableTipoTransacao = conn.CreateCommand();
                cmdInsertTableTipoTransacao.CommandText = @"
                INSERT INTO TIPOTRANSACAO (DESCRICAO) VALUES ('DEBITO');
                INSERT INTO TIPOTRANSACAO (DESCRICAO) VALUES ('CREDITO');
                INSERT INTO TIPOTRANSACAO (DESCRICAO) VALUES ('PIX');
                INSERT INTO TIPOTRANSACAO (DESCRICAO) VALUES ('TRANFERENCIA');";
                cmdInsertTableTipoTransacao.ExecuteNonQuery();

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
        public ResultadoRetornoUsuarioId VerificarUsuario(UsuarioLoginDTC usuario)
        {
            ResultadoRetornoUsuarioId resultadoRetorno = new ResultadoRetornoUsuarioId();
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
                        resultadoRetorno.UsuarioId = reader.GetInt32(0);
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
        public int VerificarUsuarioCnpj(string usuarioCpf)
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
                WHERE CPF = @cpf";
                cmdVerificarUsuario.Parameters.AddWithValue("@cpf", usuarioCpf);

                using var reader = cmdVerificarUsuario.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    if(id != 0)
                    {
                        return id;
                    }
                }
                return 0;
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
            return 0;
        }
        public ResultadoRetorno VerificarUsuarioId(int usuarioId)
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
                WHERE ID = @id";
                cmdVerificarUsuario.Parameters.AddWithValue("@id", usuarioId);

                using var reader = cmdVerificarUsuario.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetString(0);
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        isValido = true;
                    }
                    ;
                }
                ;
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
                resultadoRetorno.Mensagem = $"Erro de Banco de Dados: {ex.Message}";
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
        public ResultadoRetorno EfetuarTransacao(TransacaoDTO transacao, int usuarioRecebedorId)
        {
            ResultadoRetorno retorno = new ResultadoRetorno();
            retorno.Sucesso = false;
            try
            {
                using var conn = new SqliteConnection(connectionString);
                conn.Open();
                using var transaction = conn.BeginTransaction();
                using var cmdInsertTableTransacao = conn.CreateCommand();
                cmdInsertTableTransacao.CommandText = @"
                INSERT INTO TRANSACAO (DESCRICAO, VALOR, USUARIOID, USUARIORECEBEDORID, TIPOTRANSACAOID, DATATRANSACAO)
                VALUES (@descricao, @valor, @usuarioid, @usuariorecebedorcnpj, @tipotransacaoid, @datatransacao)";
                cmdInsertTableTransacao.Transaction = transaction;
                cmdInsertTableTransacao.Parameters.AddWithValue("@descricao", transacao.Descricao);
                cmdInsertTableTransacao.Parameters.AddWithValue("@valor", transacao.Valor);
                cmdInsertTableTransacao.Parameters.AddWithValue("@usuarioid", transacao.UsuarioId);
                cmdInsertTableTransacao.Parameters.AddWithValue("@usuariorecebedorcnpj", usuarioRecebedorId);
                cmdInsertTableTransacao.Parameters.AddWithValue("@tipotransacaoid", transacao.TipoTransacao);
                cmdInsertTableTransacao.Parameters.AddWithValue("@datatransacao", transacao.DataTransacao);
                cmdInsertTableTransacao.ExecuteNonQuery();

                using var cmdUpdateTableUsuarioSaldo = conn.CreateCommand();
                cmdUpdateTableUsuarioSaldo.CommandText = $@"
                UPDATE USUARIO_SALDO
                SET SALDO = SALDO - @valor, DATAULTIMATRANSACAO = DATETIME('now', '-3h')
                WHERE USUARIOID = @usuarioid";
                cmdUpdateTableUsuarioSaldo.Transaction = transaction;
                cmdUpdateTableUsuarioSaldo.Parameters.AddWithValue("@valor", transacao.Valor);
                cmdUpdateTableUsuarioSaldo.Parameters.AddWithValue("@usuarioid", transacao.UsuarioId);
                cmdUpdateTableUsuarioSaldo.ExecuteNonQuery();

                using var cmdUpdateTableUsuarioRecebedorSaldo = conn.CreateCommand();
                cmdUpdateTableUsuarioRecebedorSaldo.CommandText = $@"
                UPDATE USUARIO_SALDO
                SET SALDO = SALDO + @valor, DATAULTIMATRANSACAO = DATETIME('now', '-3h')
                WHERE USUARIOID = @usuarioid";
                cmdUpdateTableUsuarioRecebedorSaldo.Transaction = transaction;
                cmdUpdateTableUsuarioRecebedorSaldo.Parameters.AddWithValue("@valor", transacao.Valor);
                cmdUpdateTableUsuarioRecebedorSaldo.Parameters.AddWithValue("@usuarioid", usuarioRecebedorId);
                cmdUpdateTableUsuarioRecebedorSaldo.ExecuteNonQuery();

                transaction.Commit();

                retorno.Mensagem = "Sucesso ao efetuar transacao";
                retorno.Sucesso = true;
                return retorno;
            }
            catch(SqliteException ex)
            {
                retorno.Mensagem = $"Erro de banco de dados: {ex.Message}";
            }
            catch(Exception ex)
            {
                retorno.Mensagem = $"Erro: {ex.Message}";
            }
            return retorno;
        }
        public int VerificarSaldo(int usuarioId)
        {
            try
            {
                using var conn = new SqliteConnection(connectionString);
                conn.Open();
                using var cmdSelectVerificarSaldo = conn.CreateCommand();
                cmdSelectVerificarSaldo.CommandText = @"
                SELECT SALDO
                FROM USUARIO_SALDO
                WHERE USUARIOID = @usuarioid";
                cmdSelectVerificarSaldo.Parameters.AddWithValue("@usuarioid", usuarioId);
                using var reader = cmdSelectVerificarSaldo.ExecuteReader();
                int saldo = 0;
                while (reader.Read()) {
                    saldo = reader.GetInt32(0);
                }
                return saldo;
            }
            catch
            {
                return 0;
            }
        }
        public DadosUsuario DetalhesUsuario(int usuarioId)
        {
            DadosUsuario dadosUsuario = new DadosUsuario();
            try 
            {
                using var conn = new SqliteConnection(connectionString);
                conn.Open();
                var transaction = conn.BeginTransaction();
                using var cmdSelectDetalhesUsuario = conn.CreateCommand();
                cmdSelectDetalhesUsuario.CommandText = @"
                SELECT U.NOME, U.CPF , US.SALDO, U.AGENCIAID
                FROM USUARIO U
                JOIN USUARIO_SALDO US ON US.USUARIOID = U.ID
                WHERE U.ID = @id";
                cmdSelectDetalhesUsuario.Transaction = transaction;
                cmdSelectDetalhesUsuario.Parameters.AddWithValue("@id", usuarioId);
                using var reader = cmdSelectDetalhesUsuario.ExecuteReader();
                while (reader.Read()) {
                    dadosUsuario.Nome = reader["NOME"].ToString();
                    dadosUsuario.CPF = reader["CPF"].ToString();
                    dadosUsuario.Saldo = reader.GetInt32(2);
                    dadosUsuario.AgenciaId = reader.GetInt32(3);
                };

                using var cmdSelectDetalhesTransacaoUsuario = conn.CreateCommand();
                cmdSelectDetalhesTransacaoUsuario.CommandText = @"
                SELECT DESCRICAO, VALOR, USUARIOID, USUARIORECEBEDORID, TIPOTRANSACAOID, DATATRANSACAO
                FROM TRANSACAO
                WHERE USUARIOID = @id";
                cmdSelectDetalhesTransacaoUsuario.Transaction = transaction;
                cmdSelectDetalhesTransacaoUsuario.Parameters.AddWithValue("@id", usuarioId);
                using var reader2 = cmdSelectDetalhesTransacaoUsuario.ExecuteReader();
                List<TransacaoDTO> transacoes = new List<TransacaoDTO>();
                while (reader2.Read()) {
                    transacoes.Add(new TransacaoDTO()
                    {
                        Descricao = reader2.GetString(0),
                        Valor = reader2.GetInt32(1),
                        UsuarioId = reader2.GetInt32(2),
                        UsuarioRecebedorId = reader2.GetInt32(3),
                        TipoTransacao = (EnumTipoTransacao)reader2.GetInt32(4),
                        DataTransacao = reader2.GetDateTime(5)
                    });
                }
                dadosUsuario.Transacao = transacoes;

                transaction.Commit();

                return dadosUsuario;

            }
            catch(SqliteException ex)
            {
                dadosUsuario.Nome = $"Erro de Banco de dados: {ex.Message}";
                dadosUsuario.CPF = "Erro";
            }
            catch(Exception ex)
            {
                dadosUsuario.Nome = $"Erro: {ex.Message}";
                dadosUsuario.CPF = "Erro";
            }
            return dadosUsuario;
        }
        public string RetornarNomeUsuario(int usuarioId)
        {
            try
            {
                using var conn = new SqliteConnection(connectionString);
                conn.Open();

                using var cmdSelectNomeUsuario = conn.CreateCommand();
                cmdSelectNomeUsuario.CommandText = @"
                SELECT NOME
                FROM USUARIO
                WHERE ID = @id";
                cmdSelectNomeUsuario.Parameters.AddWithValue("@id", usuarioId);
                string? nomeUsuario = Convert.ToString(cmdSelectNomeUsuario.ExecuteScalar());

                return nomeUsuario;
            }
            catch
            {
                throw new Exception();
            }
        }
        public string RetornarCpfUsuario(int usuarioId)
        {
            try
            {
                using var conn = new SqliteConnection(connectionString);
                conn.Open();

                using var cmdSelectCpfUsuario = conn.CreateCommand();
                cmdSelectCpfUsuario.CommandText = @"
                SELECT CPF
                FROM USUARIO
                WHERE ID = @id";
                cmdSelectCpfUsuario.Parameters.AddWithValue("@id", usuarioId);
                string? cpfUsuario = Convert.ToString(cmdSelectCpfUsuario.ExecuteScalar());

                return cpfUsuario;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
