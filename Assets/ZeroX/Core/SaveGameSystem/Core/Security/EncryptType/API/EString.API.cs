#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace ZeroX.SaveGameSystem.Security
{
	public sealed partial class EString : IEquatable<EString>, IEquatable<string>, IComparable<EString>, IComparable<string>, IComparable
	{
		public int Length => hiddenChars.Length;
		
		/// <summary>
		/// Proxy to the String API.
		/// Please consider avoiding using this in a hot path since it invokes decryption on every access call.
		/// </summary>
		public string Substring(int startIndex)
		{
			return Substring(startIndex, Length - startIndex);
		}

		/// <summary>
		/// Proxy to the String API.
		/// Please consider avoiding using this in a hot path since it invokes decryption on every access call.
		/// </summary>
		public string Substring(int startIndex, int length)
		{
			return InternalDecryptToString().Substring(startIndex, length);
		}

		/// <summary>
		/// Proxy to the String API.
		/// Please consider avoiding using this in a hot path since it invokes decryption on every access call.
		/// </summary>
		public bool StartsWith(string value, StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			return InternalDecryptToString().StartsWith(value, comparisonType);
		}

		/// <summary>
		/// Proxy to the String API.
		/// Please consider avoiding using this in a hot path since it invokes decryption on every access call.
		/// </summary>
		public bool EndsWith(string value, StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			return InternalDecryptToString().EndsWith(value, comparisonType);
		}

		public override int GetHashCode()
		{
			return InternalDecryptToString().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecryptToString();
		}
		
		public override bool Equals(object other)
		{
			return other is EString o && Equals(o) ||
				   other is string r && Equals(r);
		}
		
		public bool Equals(EString obj)
		{
			if (obj == null) return false;
			
			return cryptoKey == obj.cryptoKey ? ArraysEquals(hiddenChars, obj.hiddenChars) : 
				ArraysEquals(InternalDecrypt(), obj.InternalDecrypt());
		}

		public bool Equals(EString value, StringComparison comparisonType)
		{
			return value != null &&
				   string.Equals(InternalDecryptToString(), value.InternalDecryptToString(), comparisonType);
		}
		
		public bool Equals(string other)
		{
			return InternalDecryptToString().Equals(other);
		}
		
		public int CompareTo(EString other)
		{
			return InternalDecryptToString().CompareTo(other.InternalDecryptToString());
		}

		public int CompareTo(string other)
		{
			return InternalDecryptToString().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecryptToString().CompareTo(obj);
		}
	}
}