using System.Security.Cryptography;
using System.Text;

namespace ShitChatApp.Helpers
{
	public class EncryptionHelper
	{
		private static readonly string _key = "U11QMIcy9vibJCJ8Cu82bG9HsVyoJXs3";

        public static string Encrypt(string text)
		{
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Encoding.UTF8.GetBytes(_key);
				aesAlg.GenerateIV();

				var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
				using (var msEncrypt = new MemoryStream())
				{
					msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
					using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					using (var swEncrypt = new StreamWriter(csEncrypt))
					{
						swEncrypt.Write(text);
					}
					return Convert.ToBase64String(msEncrypt.ToArray());
				}
			}
		}

		public static string Decrypt(string cipherText)
		{
			var fullCipher = Convert.FromBase64String(cipherText);

			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Encoding.UTF8.GetBytes(_key);
				aesAlg.IV = fullCipher.Take(aesAlg.BlockSize / 8).ToArray();

				var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
				using (var msDecrypt = new MemoryStream(fullCipher.Skip(aesAlg.BlockSize / 8).ToArray()))
				using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
				using (var srDecrypt = new StreamReader(csDecrypt))
				{
					return srDecrypt.ReadToEnd();
				}
			}
		}
	}
}
