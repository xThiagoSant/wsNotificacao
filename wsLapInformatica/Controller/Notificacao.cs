namespace wsLapInformatica.Controller
{
    public class Notificacao
    {
        /*
         Documentação:
         L8priori -> S/N - Se é importante ou não.
         L8tipovd -> N,A...etc - Se notificação é destinada a:
         N = Notbook
         A = Android(Celular)             
        */
        public string L8numero { get; set; }
        public string L8codemp { get; set; }
        public string L8nomint { get; set; }
        public string L8usumen { get; set; }
        public string L8usuari { get; set; }
        public string L8datmen { get; set; }
        public string L8titulo { get; set; }
        public string L8notifi { get; set; }
        public string L8priori { get; set; }
        public string L8tipovd { get; set; }
        public string Retorno { get; set; }
    }
}