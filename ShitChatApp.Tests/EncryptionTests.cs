using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShitChatApp.Helpers;

namespace ShitChatApp.Tests
{
	public class EncryptionTests
	{
		//ENCRYPT

		[Fact]
		public void Encrypt_ValidText_ShouldReturnEncryptedString()
		{
			//Arrange
			string text = "Hello world";

			//Act
			string encryptedText = EncryptionHelper.Encrypt(text);

			//Assert
			Assert.NotNull(encryptedText);
			Assert.NotEmpty(encryptedText);
		}

		[Fact]
		public void Encrypt_EmptyString_ShouldReturnEncryptedString()
		{
			//Arrange
			string text = string.Empty;

			//Act
			string encryptedText = EncryptionHelper.Encrypt(text);

			//Assert
			Assert.NotNull(encryptedText);
			Assert.NotEmpty(encryptedText);
		}

		[Theory]
		[InlineData("!@#$%^&*()_+<>?")] // Special characters
		[InlineData("1234567890")]       // Numbers
		[InlineData("This is a test message.")] // Normal text
		public void Encrypt_DifferentInputs_ShouldReturnEncryptedString(string text)
		{
			// Act
			string encryptedText = EncryptionHelper.Encrypt(text);

			// Assert
			Assert.NotNull(encryptedText);
			Assert.NotEmpty(encryptedText);
		}

		//DECRYPT

		[Fact]
		public void Decrypt_EncryptedText_ShouldReturnOriginalText()
		{
			//Arrange
			string originaltext = "Hello world";
			string encryptedText = EncryptionHelper.Encrypt(originaltext);

			//Act
			string decryptedText = EncryptionHelper.Decrypt(encryptedText);

			//Assert
			Assert.Equal(originaltext, decryptedText);
		}

		[Theory]
		[InlineData("!@#$%^&*()_+<>?")] // Special characters
		[InlineData("1234567890")]       // Numbers
		[InlineData("This is a test message.")] // Normal text
		public void Decrypt_EncryptedDifferentInputs_ShouldReturnOriginalText(string originalText)
		{
			//Arrange
			string encryptedText = EncryptionHelper.Encrypt(originalText);

			//Act
			string decryptedText = EncryptionHelper.Decrypt(encryptedText);

			//Assert
			Assert.Equal(originalText, decryptedText);
		}

		[Fact]
		public void Decrypt_InvalidBase64String_ShouldThrowFormatException()
		{
			//Arrange
			string invalidCipherText = "ThisIsNotValidBase64String";

			//Act & Assert
			Assert.Throws<FormatException>(() => EncryptionHelper.Decrypt(invalidCipherText));
		}
	}
}
