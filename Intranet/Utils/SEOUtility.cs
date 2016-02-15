using System;
using System.Text;

namespace PubSystem.SEO
{
	public class Utility
	{

		public static char[] GetAsciiChars(string input)
		{
			return System.Text.ASCIIEncoding.ASCII.GetChars((Encoding.GetEncoding(1253)).GetBytes(input));
		}

		public static string GetSafeString(string input)
		{
			if ((input == null) || (input.Length == 0))
			{
				return "";
			}
			char[] chars = GetAsciiChars(input.Trim());
			char[] charso = (new String(' ', chars.Length)).ToCharArray();
			int len = chars.Length;
			int okCount = 0;
			for (int i = 0; i < len; i++)
			{
				if (IsSafe(chars[i]))
				{
					charso[okCount] = chars[i];
					okCount++;
				}
				else if (chars[i] == ' ')
				{
					charso[okCount] = '-';
					okCount++;
				}
			}
			return new String(charso, 0, okCount).ToLower();
		}

		private static bool IsSafe(char ch)
		{
			if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9')))
			{
				return true;
			}
			switch (ch)
			{
				//case '\'':
				//case '(':
				//case ')':
				//case '*':
				case '-':
				//case '.':
				case '_':
                //case '/':
				//case '!':
					return true;
			}
			return false;
		}

	}
}
