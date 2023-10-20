using FasleSafar.Models;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace FasleSafar.Utilities
{
    public static class ToolBox
    {
        public static string ToShamsi(this string value) // use this word for use the method for all DateTime variables in project
        {
            var date = value.Split(' ')[0].Split('/');
            DateTime dateTime = new DateTime(Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(date[2]));
            PersianCalendar persianCalendar = new PersianCalendar();
            return persianCalendar.GetYear(dateTime) + "/" + persianCalendar.GetMonth(dateTime).ToString("00") + "/" + persianCalendar.GetDayOfMonth(dateTime).ToString("00") + " - " + DateTime.Now.ToString("HH:mm");
        }

        public static string FixPrice(this decimal value)
        {
            return value.ToString("#,0");
        }
        public static string ToHash(this string value)
        {
			if (string.IsNullOrEmpty(value)) return "";
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
		}

		public static string ToUnHash(this string value)
		{
            if (string.IsNullOrEmpty(value)) return "";
			return Encoding.UTF8.GetString(Convert.FromBase64String(value));
		}

		public static void SaveLog(object log)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"{log.ToString()}");
			sb.AppendLine(DateTime.Now.ToShortTimeString());
			sb.AppendLine($"--------------------------------");
			System.IO.File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(),
					  "wwwroot",
					  "log.txt"), sb.ToString());
		}

		public static string FixDateToSave(this string value)
		{
			string monthNumber = "";
			string[] arr = value.Split(' ');
			switch (arr[1])
			{
				case "فروردین":
				default:
					monthNumber = "01";
					break;
				case "اردیبهشت":
					monthNumber = "02";
					break;
				case "خرداد":
					monthNumber = "03";
					break;
				case "تیر":
					monthNumber = "04";
					break;
				case "مرداد":
					monthNumber = "05";
					break;
				case "شهریور":
					monthNumber = "06";
					break;
				case "مهر":
					monthNumber = "07";
					break;
				case "آبان":
					monthNumber = "08";
					break;
				case "آذر":
					monthNumber = "09";
					break;
				case "دی":
					monthNumber = "10";
					break;
				case "بهمن":
					monthNumber = "11";
					break;
				case "اسفند":
					monthNumber = "12";
					break;
			}
			return $"{arr[2]}/{monthNumber}/{arr[0]}";
		}

		public static bool SendEmail(string emailAddress,string subject,string body)
		{
			bool send = false;
			try
			{
				MailMessage mail = new MailMessage(
					new MailAddress("admin@faslesafar.com","فصل سفر"),
					new MailAddress(emailAddress));
				mail.Subject = subject;
				mail.Body = body;
				mail.IsBodyHtml = true;
				SmtpClient smtp = new SmtpClient("mani.r1host.com");
				smtp.UseDefaultCredentials = false;
				smtp.Credentials = new NetworkCredential("admin@faslesafar.com","7784914Z@z");
				smtp.Port = 25;
				smtp.EnableSsl = true;
				smtp.Send(mail);
				send = true;
			}
			catch (Exception ex)
			{
				ToolBox.SaveLog(ex.Message + '\n' + ex.InnerException?.Message + "\n log 12");
				send = false;
			}
			return send;
		}

		public static string GenerateToken()
		{
			byte[] randomBytes = new byte[10]; // اندازه توکن را می‌توانید تغییر دهید
			using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				rngCryptoServiceProvider.GetBytes(randomBytes);
			}
			return $"{Convert.ToBase64String(randomBytes)}{Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyy/MM/dd - HH:mm").ToShamsi()))}";
		}
	}
}
