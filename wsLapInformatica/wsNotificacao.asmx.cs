using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using wsLapInformatica.Model;
using wsLapInformatica.Controller;
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;
using System.Text;

namespace wsLapInformatica
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class wsNotificacao : System.Web.Services.WebService
    {

        [WebMethod]
        public void Manual_Func(string nomeFuncao)
        {
            List<Tutorial> listTutorial = new List<Tutorial>();

            if (nomeFuncao == "GetNotificacoes")
            {
                Tutorial tutorial = new Tutorial();
                tutorial.NomeMetodo = nomeFuncao;
                tutorial.DescricaoMetodo = "Retorna uma lista de notificações de uma determinada empresa.";
                tutorial.Parametros = "lapToken, codEmp";
                listTutorial.Add(tutorial);
            }
            else if (nomeFuncao == "GetNotificacoesData")
            {
                Tutorial tutorial = new Tutorial();
                tutorial.NomeMetodo = nomeFuncao;
                tutorial.DescricaoMetodo = "Retorna uma lista de notificações de uma determinada empresa por data.";
                tutorial.Parametros = "lapToken,dataInicial, dataFinal, codEmp";
                listTutorial.Add(tutorial);
            }
            else if (nomeFuncao == "GetSql")
            {
                Tutorial tutorial = new Tutorial();
                tutorial.NomeMetodo = nomeFuncao;
                tutorial.DescricaoMetodo = "Informe um SQL - ANSI e obtenha o retorno.";
                tutorial.Parametros = "lapToken, strSql";
                listTutorial.Add(tutorial);
            }
            else if (nomeFuncao == "PostNotificacao")
            {
                Tutorial tutorial = new Tutorial();
                tutorial.NomeMetodo = nomeFuncao;
                tutorial.DescricaoMetodo = "Grava uma Notificação.";
                tutorial.Parametros = "lapToken, l8codemp, l8nomint, l8usumen, l8usuari, l8datmen, l8titulo, l8notifi, l8priori";
                listTutorial.Add(tutorial);
            }
            else if (nomeFuncao == "PutNotificacao")
            {
                Tutorial tutorial = new Tutorial();
                tutorial.NomeMetodo = nomeFuncao;
                tutorial.DescricaoMetodo = "Altera uma Notificação.";
                tutorial.Parametros = "lapToken, l8numero, l8datmen, l8titulo, l8notifi";
                listTutorial.Add(tutorial);
            }
            else if (nomeFuncao == "DelNotificacao")
            {
                Tutorial tutorial = new Tutorial();
                tutorial.NomeMetodo = nomeFuncao;
                tutorial.DescricaoMetodo = "Altera uma Notificação.";
                tutorial.Parametros = "lapToken, l8numero";
                listTutorial.Add(tutorial);
            }
            else
            {
                Tutorial t1 = new Tutorial();
                t1.NomeMetodo = nomeFuncao;
                t1.DescricaoMetodo = "Retorna uma lista de notificações de uma determinada empresa.";
                t1.Parametros = "lapToken, codEmp";
                listTutorial.Add(t1);

                Tutorial t2 = new Tutorial();
                t2.NomeMetodo = nomeFuncao;
                t2.DescricaoMetodo = "Retorna uma lista de notificações de uma determinada empresa por data.";
                t2.Parametros = "lapToken,dataInicial, dataFinal, codEmp";
                listTutorial.Add(t2);

                Tutorial t3 = new Tutorial();
                t3.NomeMetodo = nomeFuncao;
                t3.DescricaoMetodo = "Informe um SQL - ANSI e obtenha o retorno.";
                t3.Parametros = "lapToken, strSql";
                listTutorial.Add(t3);

                Tutorial t4 = new Tutorial();
                t4.NomeMetodo = nomeFuncao;
                t4.DescricaoMetodo = "Grava uma Notificação.";
                t4.Parametros = "lapToken, l8codemp, l8nomint, l8usumen, l8usuari, l8datmen, l8titulo, l8notifi, l8priori, l8tipovd";
                listTutorial.Add(t4);

                Tutorial t5 = new Tutorial();
                t5.NomeMetodo = nomeFuncao;
                t5.DescricaoMetodo = "Altera uma Notificação.";
                t5.Parametros = "lapToken, l8numero, l8datmen, l8titulo, l8notifi";
                listTutorial.Add(t5);

                Tutorial t6 = new Tutorial();
                t6.NomeMetodo = nomeFuncao;
                t6.DescricaoMetodo = "Altera uma Notificação.";
                t6.Parametros = "lapToken, l8numero";
                listTutorial.Add(t6);
            }

            JavaScriptSerializer json = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.Write(json.Serialize(listTutorial));
            Context.Response.Flush();
            Context.Response.End();
        }

        //Gets
        [WebMethod]
        public void GetNotifData(string laptoken, string dataInicial, string dataFinal, string codEmp, string tipoVd)
        {
            ConectDB myConn = new ConectDB() { Host = Util.Host ,User = Util.User, Password = Util.Password, Database = Util.Database };
            //List<TabNotificacao> listNotificacao = new List<TabNotificacao>();
            List<Notificacao> listNotificacao = new List<Notificacao>();

            laptoken = HttpUtility.UrlDecode(laptoken);
            dataInicial = HttpUtility.UrlDecode(dataInicial);
            dataFinal = HttpUtility.UrlDecode(dataFinal);

            if (laptoken != "lap@info@7532159")
            {
                Notificacao errorNotificacao = new Notificacao()
                {
                    Retorno = Server.HtmlEncode(Util.RemoverAcentuacao("Token Inválido")),
                    L8numero = Server.HtmlEncode(""),
                    L8codemp = Server.HtmlEncode(""),
                    L8nomint = Server.HtmlEncode(""),
                    L8usumen = Server.HtmlEncode(""),
                    L8usuari = Server.HtmlEncode(""),
                    L8datmen = Server.HtmlEncode(String.Format("{0:dd/MM/yyyy}", DateTime.Now)),
                    L8titulo = Server.HtmlEncode(""),
                    L8notifi = Server.HtmlEncode(""),
                    L8priori = Server.HtmlEncode(""),
                    L8tipovd = Server.HtmlEncode("")
                };

                listNotificacao.Add(errorNotificacao);
            }
            else
            {
                try
                {
                    myConn.Conectar();
                    MySqlCommand cmd = new MySqlCommand("", myConn.Conexao());

                    DateTime dtInicial = DateTime.Parse(dataInicial);
                    DateTime dtFinal = DateTime.Parse(dataFinal);

                    string sqlTipoVd = "";

                    if (!String.IsNullOrEmpty(tipoVd))
                    {
                        sqlTipoVd = $" and l8tipovd = '{tipoVd}'";
                    }

                    string strSql = $"select * from lapa08 where l8datmen between '{String.Format("{0:yyyy/MM/dd}", dtInicial)}' and '{String.Format("{0:yyyy/MM/dd}", dtFinal)}' and l8codEmp='{codEmp}' {sqlTipoVd} limit 0, 100;";
                    cmd.CommandText = strSql;

                    MySqlDataReader dr;
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Notificacao notificacao = new Notificacao()
                        {
                            Retorno = Server.HtmlEncode("OK"),
                            L8numero = Server.HtmlEncode((string)dr["l8numero"].ToString()),
                            L8codemp = Server.HtmlEncode((string)dr["l8codemp"]),
                            L8nomint = Server.HtmlEncode((string)dr["l8nomint"]),
                            L8usumen = Server.HtmlEncode((string)dr["l8usumen"]),
                            L8usuari = Server.HtmlEncode((string)dr["l8usuari"]),
                            L8datmen = Server.HtmlEncode(String.Format("{0:dd/MM/yyyy}", dr["l8datmen"])),
                            L8titulo = Server.HtmlEncode((string)dr["l8titulo"]),
                            L8notifi = Server.HtmlEncode((string)dr["l8notifi"]),
                            L8priori = Server.HtmlEncode((string)dr["l8priori"]),
                            L8tipovd = Server.HtmlEncode((string)dr["l8tipovd"])
                        };
                        listNotificacao.Add(notificacao);
                    }
                    cmd = null;

                }
                catch (Exception e)
                {
                    Notificacao errorNotificacao = new Notificacao()
                    {
                        Retorno = Server.HtmlEncode("wsError: " + Util.RemoverAcentuacao(e.Message)),
                        L8numero = Server.HtmlEncode(""),
                        L8codemp = Server.HtmlEncode(""),
                        L8nomint = Server.HtmlEncode(""),
                        L8usumen = Server.HtmlEncode(""),
                        L8usuari = Server.HtmlEncode(""),
                        L8datmen = Server.HtmlEncode(String.Format("{0:dd/MM/YYYY}", DateTime.Now)),
                        L8titulo = Server.HtmlEncode(""),
                        L8notifi = Server.HtmlEncode(""),
                        L8priori = Server.HtmlEncode(""),
                        L8tipovd = Server.HtmlEncode("")
                    };

                    listNotificacao.Add(errorNotificacao);
                }

            }

            JavaScriptSerializer json = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.Write(json.Serialize(listNotificacao));
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void GetNotif(string lapToken, string codEmp, string tipoVd)
        {
            ConectDB myConn = new ConectDB() { Host = Util.Host, User = Util.User, Password = Util.Password, Database = Util.Database };
            List<Notificacao> listNotificacao = new List<Notificacao>();

            lapToken = HttpUtility.UrlDecode(lapToken);
            codEmp = HttpUtility.UrlDecode(codEmp);
            tipoVd = HttpUtility.UrlDecode(tipoVd);

            //offSet = HttpUtility.UrlDecode(offSet);
            //limit = HttpUtility.UrlDecode(limit);

            if (lapToken != "lap@info@7532159")
            {
                Notificacao errorNotificacao = new Notificacao()
                {
                    Retorno = Server.HtmlEncode(Util.RemoverAcentuacao("Token Inválido")),
                    L8numero = Server.HtmlEncode(""),
                    L8codemp = Server.HtmlEncode(""),
                    L8nomint = Server.HtmlEncode(""),
                    L8usumen = Server.HtmlEncode(""),
                    L8usuari = Server.HtmlEncode(""),
                    L8datmen = Server.HtmlEncode(String.Format("{0:dd/MM/yyyy}", DateTime.Now)),
                    L8titulo = Server.HtmlEncode(""),
                    L8notifi = Server.HtmlEncode(""),
                    L8priori = Server.HtmlEncode(""),
                    L8tipovd = Server.HtmlEncode("")
                };
                listNotificacao.Add(errorNotificacao);
            }
            else
            {
                try
                {
                    myConn.Conectar();
                    /*
                    if(String.IsNullOrEmpty(offSet) && (String.IsNullOrEmpty(limit)))
                    {
                        offSet = "1";
                        limit = "100";
                    }
                    string strSql = $"select * from lapa08 where l8codemp ='{codEmp}' limit {offSet},{limit}";
                    */
                    string sql2 = "";

                    if (! String.IsNullOrEmpty(tipoVd))
                    {
                        sql2 = $" and l8tipovd = '{tipoVd}'"; 
                    }                    

                    string strSql = $"select * from lapa08 where l8codemp = '{codEmp}'{sql2} limit 0, 100";

                    MySqlCommand cmd = new MySqlCommand(strSql, myConn.Conexao());
                    MySqlDataReader dr;
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Notificacao notificacao = new Notificacao()
                        {
                            Retorno = Server.HtmlEncode("OK"),
                            L8numero = Server.HtmlEncode((string)dr["l8numero"].ToString()),
                            L8codemp = Server.HtmlEncode((string)dr["l8codemp"]),
                            L8nomint = Server.HtmlEncode((string)dr["l8nomint"]),
                            L8usumen = Server.HtmlEncode((string)dr["l8usumen"]),
                            L8usuari = Server.HtmlEncode((string)dr["l8usuari"]),
                            L8datmen = Server.HtmlEncode(String.Format("{0:dd/MM/yyyy}", dr["l8datmen"])),
                            L8titulo = Server.HtmlEncode((string)dr["l8titulo"]),
                            L8notifi = Server.HtmlEncode((string)dr["l8notifi"]),
                            L8priori = Server.HtmlEncode((string)dr["l8priori"]),
                            L8tipovd = Server.HtmlEncode((string)dr["l8tipovd"])
                        };
                        listNotificacao.Add(notificacao);
                    }
                    cmd = null;

                }
                catch (Exception e)
                {
                    Notificacao errorNotificacao = new Notificacao()
                    {
                        Retorno = Server.HtmlEncode("wsError: " + Util.RemoverAcentuacao(e.Message)),
                        L8numero = Server.HtmlEncode(""),
                        L8codemp = Server.HtmlEncode(""),
                        L8nomint = Server.HtmlEncode(""),
                        L8usumen = Server.HtmlEncode(""),
                        L8usuari = Server.HtmlEncode(""),
                        L8datmen = Server.HtmlEncode(String.Format("{0:dd/MM/YYYY}", DateTime.Now)),
                        L8titulo = Server.HtmlEncode(""),
                        L8notifi = Server.HtmlEncode(""),
                        L8priori = Server.HtmlEncode("")
                    };

                    listNotificacao.Add(errorNotificacao);
                }

            }

            JavaScriptSerializer json = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.Write(json.Serialize(listNotificacao));
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void GetSql(string lapToken, string strSql)
        {
            ConectDB myConn = new ConectDB() { Host = Util.Host, User = Util.User, Password = Util.Password, Database = Util.Database };
            List<Notificacao> listNotificacao = new List<Notificacao>();

            lapToken = HttpUtility.UrlDecode(lapToken);
            strSql = HttpUtility.UrlDecode(strSql);


            if (lapToken != "lap@info@7532159")
            {
                Notificacao errorNotificacao = new Notificacao()
                {
                    Retorno = Server.HtmlEncode(Util.RemoverAcentuacao("Token Inválido")),
                    L8numero = Server.HtmlEncode(""),
                    L8codemp = Server.HtmlEncode(""),
                    L8nomint = Server.HtmlEncode(""),
                    L8usumen = Server.HtmlEncode(""),
                    L8usuari = Server.HtmlEncode(""),
                    L8datmen = Server.HtmlEncode(String.Format("{0:dd/MM/yyyy}", DateTime.Now)),
                    L8titulo = Server.HtmlEncode(""),
                    L8notifi = Server.HtmlEncode(""),
                    L8priori = Server.HtmlEncode(""),
                    L8tipovd = Server.HtmlEncode("")
                };
                listNotificacao.Add(errorNotificacao);
            }
            else
            {
                try
                {
                    myConn.Conectar();
                    MySqlCommand cmd = new MySqlCommand(strSql, myConn.Conexao());
                    MySqlDataReader dr;
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Notificacao notificacao = new Notificacao()
                        {
                            Retorno = Server.HtmlEncode("OK"),
                            L8numero = Server.HtmlEncode((string)dr["l8numero"].ToString()),
                            L8codemp = Server.HtmlEncode((string)dr["l8codemp"]),
                            L8nomint = Server.HtmlEncode((string)dr["l8nomint"]),
                            L8usumen = Server.HtmlEncode((string)dr["l8usumen"]),
                            L8usuari = Server.HtmlEncode((string)dr["l8usuari"]),
                            L8datmen = Server.HtmlEncode(String.Format("{0:dd/MM/yyyy}", dr["l8datmen"])),
                            L8titulo = Server.HtmlEncode((string)dr["l8titulo"]),
                            L8notifi = Server.HtmlEncode((string)dr["l8notifi"]),
                            L8priori = Server.HtmlEncode((string)dr["l8priori"]),
                            L8tipovd = Server.HtmlEncode((string)dr["l8tipovd"])
                        };

                        listNotificacao.Add(notificacao);
                    }
                    cmd = null;

                }
                catch (Exception e)
                {
                    Notificacao errorNotificacao = new Notificacao()
                    {
                        Retorno = Server.HtmlEncode("wsError: " + Util.RemoverAcentuacao(e.Message)),
                        L8numero = Server.HtmlEncode(""),
                        L8codemp = Server.HtmlEncode(""),
                        L8nomint = Server.HtmlEncode(""),
                        L8usumen = Server.HtmlEncode(""),
                        L8usuari = Server.HtmlEncode(""),
                        L8datmen = Server.HtmlEncode(String.Format("{0:dd/MM/YYYY}", DateTime.Now)),
                        L8titulo = Server.HtmlEncode(""),
                        L8notifi = Server.HtmlEncode(""),
                        L8priori = Server.HtmlEncode(""),
                        L8tipovd = Server.HtmlEncode("")
                    };

                    listNotificacao.Add(errorNotificacao);
                }

            }

            JavaScriptSerializer json = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.Write(json.Serialize(listNotificacao));
            Context.Response.Flush();
            Context.Response.End();
        }

        //Posts

        [WebMethod]
        public void PostNotif(string lapToken, string l8codemp, string l8nomint, string l8usumen, string l8usuari,
            string l8datmen, string l8titulo, string l8notifi, string l8priori, string l8tipovd)

        {
            ConectDB myConn = new ConectDB() { Host = Util.Host, User = Util.User, Password = Util.Password, Database = Util.Database };
            List<Retorno> ret = new List<Retorno>();

            lapToken = HttpUtility.UrlDecode(lapToken);
            l8codemp = HttpUtility.UrlDecode(l8codemp);
            l8nomint = HttpUtility.UrlDecode(l8nomint);
            l8usumen = HttpUtility.UrlDecode(l8usumen);
            l8usuari = HttpUtility.UrlDecode(l8usuari);
            l8datmen = HttpUtility.UrlDecode(l8datmen);
            l8titulo = HttpUtility.UrlDecode(l8titulo);
            l8notifi = HttpUtility.UrlDecode(l8notifi, Encoding.UTF8);
            l8priori = HttpUtility.UrlDecode(l8priori, Encoding.UTF8);
            l8tipovd = HttpUtility.UrlDecode(l8tipovd);

            if (lapToken != "lap@info@7532159")
            {
                Retorno erro = new Retorno()
                {
                    Codigo = "ERRO",
                    Descricao = Server.HtmlEncode(Util.RemoverAcentuacao("Token Inválido"))
                };

                ret.Add(erro);
            }
            else
            {
                try
                {
                    myConn.Conectar();

                    string strSql = "Insert into notificacao.lapa08 (l8codemp, l8nomint, l8usumen, l8usuari, l8datmen, l8titulo, l8notifi, l8priori, l8tipovd)" +
                                    "Values (@l8codemp, @l8nomint, @l8usumen, @l8usuari, @l8datmen, @l8titulo, @l8notifi, @l8priori, @l8tipovd);";

                    MySqlCommand cmd = new MySqlCommand(strSql, myConn.Conexao());

                    cmd.Parameters.AddWithValue("@l8codemp", l8codemp);
                    cmd.Parameters.AddWithValue("@l8nomint", l8nomint);
                    cmd.Parameters.AddWithValue("@l8usumen", l8usumen);
                    cmd.Parameters.AddWithValue("@l8usuari", l8usuari);
                    cmd.Parameters.AddWithValue("@l8datmen", Convert.ToDateTime(Util.ConvertFormData(l8datmen)));
                    cmd.Parameters.AddWithValue("@l8titulo", l8titulo);
                    cmd.Parameters.AddWithValue("@l8notifi", l8notifi);
                    cmd.Parameters.AddWithValue("@l8priori", l8priori);
                    cmd.Parameters.AddWithValue("@l8tipovd", l8tipovd);
                    cmd.ExecuteNonQuery();

                    if(cmd.LastInsertedId != 0)
                        cmd.Parameters.Add(new MySqlParameter("ultimoId", cmd.LastInsertedId));

                    int idNotificacao = Convert.ToInt32(cmd.Parameters["@ultimoId"].Value);

                    Retorno retorno = new Retorno()
                    {
                        Codigo = "OK",
                        Descricao = Server.HtmlEncode(Util.RemoverAcentuacao($"Cadastro Concluído: - ID:{idNotificacao}"))
                    };

                    ret.Add(retorno);

                    cmd = null;

                }
                catch (Exception e)
                {
                    Retorno erro = new Retorno()
                    {
                        Codigo = "ERRO",
                        Descricao = Server.HtmlEncode("wsError: " + Util.RemoverAcentuacao(e.Message))
                    };

                    ret.Add(erro);
                }

            }

            JavaScriptSerializer json = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.Write(json.Serialize(ret));
            Context.Response.Flush();
            Context.Response.End();
        }

        //Puts
        [WebMethod]
        public void PutNotif(string lapToken, string l8numero, string l8datmen, string l8titulo, string l8notifi, string l8tipovd)
        {
            lapToken = HttpUtility.UrlDecode(lapToken);
            l8numero = HttpUtility.UrlDecode(l8numero);

            ConectDB myConn = new ConectDB() { Host = Util.Host, User = Util.User, Password = Util.Password, Database = Util.Database };
            List<Retorno> ret = new List<Retorno>();

            if (lapToken != "lap@info@7532159")
            {
                Retorno erro = new Retorno()
                {
                    Codigo = "ERRO",
                    Descricao = Server.HtmlEncode(Util.RemoverAcentuacao("Token Inválido"))
                };

                ret.Add(erro);
            }
            else if (String.IsNullOrEmpty(l8titulo) && String.IsNullOrEmpty(l8notifi) && String.IsNullOrEmpty(l8datmen))
            {
                Retorno erro = new Retorno()
                {
                    Codigo = "ERRO",
                    Descricao = Server.HtmlEncode(Util.RemoverAcentuacao("Informe os dados para serem alterados."))
                };

                ret.Add(erro);
            }
            else if (String.IsNullOrEmpty(l8numero))
            {
                Retorno erro = new Retorno()
                {
                    Codigo = "ERRO",
                    Descricao = Server.HtmlEncode(Util.RemoverAcentuacao("Informe um ID para sere alterads."))
                };

                ret.Add(erro);
            }
            else
            {
                try
                {
                    myConn.Conectar();

                    string fd1 = String.IsNullOrEmpty(l8datmen) ? "" : "l8datmen = @l8datmen,";
                    string fd2 = String.IsNullOrEmpty(l8titulo) ? "" : "l8titulo = @l8titulo,";
                    string fd3 = String.IsNullOrEmpty(l8notifi) ? "" : "l8notifi = @l8notifi";
                    string fd4 = String.IsNullOrEmpty(l8tipovd) ? "" : "l8tipovd = @l8tipovd";
                    string parSQL = fd1 + fd2 + fd3 + fd4;
                    
                    if(parSQL.Substring(parSQL.Length-1, 1) == ",")
                        parSQL = parSQL.Remove(parSQL.Length-1, 1);                    

                    string strSql = $"UPDATE lapa08 set {parSQL} WHERE l8numero = @l8numero";

                    MySqlCommand cmd = new MySqlCommand(strSql, myConn.Conexao());

                    cmd.Parameters.AddWithValue("@l8numero", l8numero);

                    if (!(String.IsNullOrEmpty(l8datmen)))
                        cmd.Parameters.AddWithValue("@l8datmen", Convert.ToDateTime(Util.ConvertFormData(l8datmen)));

                    if (!(String.IsNullOrEmpty(l8titulo)))
                        cmd.Parameters.AddWithValue("@l8titulo", l8titulo);

                    if (!(String.IsNullOrEmpty(l8notifi)))
                        cmd.Parameters.AddWithValue("@l8notifi", l8notifi);

                    if (!(String.IsNullOrEmpty(l8tipovd)))
                        cmd.Parameters.AddWithValue("l8tipovd", l8tipovd);

                    cmd.ExecuteNonQuery();

                    Retorno retorno = new Retorno()
                    {
                        Codigo = "OK",
                        Descricao = Server.HtmlEncode(Util.RemoverAcentuacao($"ID: {l8numero}, alterado com sucesso!"))
                    };

                    ret.Add(retorno);

                    cmd = null;
                }
                catch (Exception e)
                {
                    Retorno erro = new Retorno()
                    {
                        Codigo = "ERRO",
                        Descricao = Server.HtmlEncode("wsError: " + Util.RemoverAcentuacao(e.Message))
                    };

                    ret.Add(erro);
                }
            }

            JavaScriptSerializer json = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.Write(json.Serialize(ret));
            Context.Response.Flush();
            Context.Response.End();
        }
        //Deletes
        [WebMethod]
        public void DelNotif(string lapToken, string l8numero)
        {
            lapToken = HttpUtility.UrlDecode(lapToken);
            l8numero = HttpUtility.UrlDecode(l8numero);

            ConectDB myConn = new ConectDB() { Host = Util.Host, User = Util.User, Password = Util.Password, Database = Util.Database };
            List<Retorno> ret = new List<Retorno>();

            if (lapToken != "lap@info@7532159")
            {
                Retorno erro = new Retorno()
                {
                    Codigo = "ERRO",
                    Descricao = Server.HtmlEncode(Util.RemoverAcentuacao("Token Inválido"))
                };
                ret.Add(erro);
            }
            else
            {
                try
                {
                    myConn.Conectar();

                    MySqlCommand cmd = new MySqlCommand($"DELETE from lapa08 WHERE l8numero = {l8numero}", myConn.Conexao());
                    cmd.ExecuteNonQuery();

                    Retorno notificacao = new Retorno()
                    {
                        Codigo = "OK",
                        Descricao = Server.HtmlEncode(Util.RemoverAcentuacao($"ID: {l8numero}, Apagado com sucesso!"))
                    };
                    ret.Add(notificacao);

                    cmd = null;
                }
                catch (Exception e)
                {
                    Retorno erro = new Retorno()
                    {
                        Codigo = "ERRO",
                        Descricao = Server.HtmlEncode("wsError: " + Util.RemoverAcentuacao(e.Message))
                    };

                    ret.Add(erro);
                }

                JavaScriptSerializer json = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.Write(json.Serialize(ret));
                Context.Response.Flush();
                Context.Response.End();
            }
        }

    }
}
