using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TaApostado.Utils
{
    public class CPFUtils : ValidationAttribute
    {

        public override bool IsValid(object obj)
        {
            string cpf = obj as string;
            if(Regex.IsMatch(cpf, @"(?!(\d)\1{2}.\1{3}.\1{3}-\1{2})\d{3}\.\d{3}\.\d{3}\-\d{2}") && cpf.Length == 14){
                cpf = cpf.Replace(".", "");
                cpf = cpf.Replace("-", "");

                char[] arrayCpf = cpf.ToCharArray();

                // Calculo do 1o. Digito Verificador
                var sm = 0;
                var peso = 10;

                int num = 0, r = 0;
                char dig10, dig11;

                for (int i = 0; i < 9; i++)
                {
                        // converte o i-esimo caractere do CPF em um numero:
                        // por exemplo, transforma o caractere '0' no inteiro 0
                        // (48 eh a posicao de '0' na tabela ASCII)
                        
                    num = (int)(arrayCpf[i] - 48);
                    sm = sm + (num * peso);
                    peso = peso - 1;
                }

                    r = 11 - (sm % 11);
                    if ((r == 10) || (r == 11))
                        dig10 = '0';
                    else dig10 = (char)(r + 48); // converte no respectivo caractere numerico

                    // Calculo do 2o. Digito Verificador
                    sm = 0;
                    peso = 11;
                    for (int i = 0; i < 10; i++)
                    {
                        num = (int)(arrayCpf[i] - 48);
                        sm = sm + (num * peso);
                        peso = peso - 1;
                    }

                    r = 11 - (sm % 11);
                    if ((r == 10) || (r == 11))
                        dig11 = '0';
                    else dig11 = (char)(r + 48);

                    // Verifica se os digitos calculados conferem com os digitos informados.
                    if ((dig10 == arrayCpf[9]) && (dig11 == arrayCpf[10]))
                        return (true);
                    else return (false);
            }
            return false;
        }
    }
}
