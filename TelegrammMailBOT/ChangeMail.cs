using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegrammMailBOT
{
    internal class ChangeMail
    {

        public static string DeliteMail()
        {
            try
            {
                string filePath = "Mail.txt";
                // Открываем файл для записи (это автоматически очистит его содержимое)
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    // Здесь не нужно ничего записывать
                }
                return "Удалил все почты 💠👍🏽";

            }
            catch (Exception ex)
            {
                return $"Произошла ошибка при удалении: {ex.Message}";
            }
        }
        public static string AddMail(string message)
        {
            try
            {
                string filePath = "Mail.txt";
                // Добавляем новую строку в конец файла
                System.IO.File.AppendAllText(filePath, message + Environment.NewLine);

                return "Почтa успешно добавлена. ✅💠";

            }
            catch (Exception ex)
            {
                return $"Произошла ошибка: {ex.Message}";
            }
        }
    }
}
