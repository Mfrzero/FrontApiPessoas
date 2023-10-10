using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepSafeFront
{
    public class Pessoa
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Sexo { get; set; }
        public int Idade { get; set; }
        public string Cidade { get; set; }
    }

    public enum TipoSexo
    {
        MASCULINO,
        FEMININO
    }
}
