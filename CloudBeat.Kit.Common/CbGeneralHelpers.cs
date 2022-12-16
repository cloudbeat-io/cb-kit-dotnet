using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CloudBeat.Kit.Common
{
	public static class CbGeneralHelpers
	{
		public static string FqnToFileName(string fqn)
		{
			// check if fqn contains "()", if yes - hash the arguments part
			var parenthesesOpenPos = fqn.IndexOf("(");
			if (parenthesesOpenPos > -1)
			{
				var funcPart = fqn.Substring(0, parenthesesOpenPos);
				var argsPart = fqn.Substring(parenthesesOpenPos);
				var argsHash = GetHashString(argsPart);
				return $"{funcPart}-{argsHash}";

			}
			var illegalChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()).ToArray();
			foreach (char c in illegalChars)
			{
				fqn = fqn.Replace(c.ToString(), "");
			}
			return fqn;
		}
		public static byte[] GetHash(string inputString)
		{
			using (HashAlgorithm algorithm = SHA256.Create())
				return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
		}

		public static string GetHashString(string inputString)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in GetHash(inputString))
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}

		public static string GenerateShortUid()
		{
			var ticks = new DateTime(2016, 1, 1).Ticks;
			var ans = DateTime.Now.Ticks - ticks;
			return ans.ToString("x");
		}
	}
}
