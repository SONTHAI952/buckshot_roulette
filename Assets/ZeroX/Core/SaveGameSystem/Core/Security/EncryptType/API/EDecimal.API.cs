#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace ZeroX.SaveGameSystem.Security
{
	public partial struct EDecimal : IFormattable, IEquatable<EDecimal>, IEquatable<decimal>, IComparable<EDecimal>, IComparable<decimal>, IComparable
	{
		private static EDecimal Increment(EDecimal input, decimal increment)
		{
			var decrypted = input.InternalDecrypt() + increment;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			return input;
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}

		public override bool Equals(object other)
		{
			return other is EDecimal o && Equals(o) ||
				   other is decimal r && Equals(r);
		}
		
		public bool Equals(EDecimal other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}
		
		public bool Equals(decimal other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(EDecimal other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(decimal other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}