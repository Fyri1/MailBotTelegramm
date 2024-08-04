using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegrammMailBOT
{
    internal class SendMail
    {
        public static string ReadMail()
        {
            try
            {
                string mail;
                string filePath = "Mail.txt";
                string ChangeResult;
                string resultFinal;
                string[] readtext = System.IO.File.ReadAllLines(filePath);
                if (readtext.Length <= 0)
                {
                    return "Почт больше нет ";
                }
                foreach (string line in readtext)
                {
                    string[] lineSplit = line.Split(":");
                    ChangeResult = line.Replace(":", Environment.NewLine);
                    resultFinal = "Данные почты\n" + ChangeResult + "\n" + "\n Данные от Play Station аккаунта \n" + lineSplit[0] + "\nQazwsxed12 \n" + "16.11.2000";
                    RemoveLineFromFile(filePath, line);
                    return resultFinal;

                }
            }
            catch (Exception ex)
            {
                return "Похоже Почты закончились ";
            }
            return "";

        }
        public static void RemoveLineFromFile(string filePath, string lineToRemove)
        {
            try
            {
                // Считываем все строки из файла
                List<string> lines = new List<string>(System.IO.File.ReadAllLines(filePath));

                // Удаляем указанную строку
                lines.RemoveAll(line => line.Equals(lineToRemove, StringComparison.OrdinalIgnoreCase));

                // Записываем обновленный список строк обратно в файл
                System.IO.File.WriteAllLines(filePath, lines.ToArray());

                Console.WriteLine("Строка успешно удалена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
