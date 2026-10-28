#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Numerics;

namespace ZeroX.SaveGameSystem.Security
{
	public partial struct EBigInteger : IFormattable, IEquatable<EBigInteger>, IEquatable<BigInteger>, IComparable<EBigInteger>, IComparable<BigInteger>, IComparable
	{
		private static EBigInteger Increment(EBigInteger input, int increment)
		{
			var decrypted = input.InternalDecrypt() + increment;
			input.hiddenValue.value = new BigInteger(decrypted.ToByteArray());
			input.hiddenValue = input.hiddenValue.Encrypt(input.currentCryptoKey);

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
			return InternalDecrypt().ToString(format, provider) ?? string.Empty;
		}

		public override bool Equals(object other)
		{
			return other is EBigInteger o && Equals(o) ||
				   other is BigInteger r && Equals(r);
		}

		public bool Equals(BigInteger other)
		{
			return InternalDecrypt().Equals(other);
		}
		
		public bool Equals(EBigInteger obj)
		{
			return currentCryptoKey == obj.currentCryptoKey ? hiddenValue.Equals(obj.hiddenValue) : 
				InternalDecrypt().Equals(obj.InternalDecrypt());
		}
		
		public int CompareTo(EBigInteger other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(BigInteger other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}

		public int CompareTo(long other)
		{
			return InternalDecrypt().CompareTo(other);
		}		
		
		public int CompareTo(ulong other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public byte[] ToByteArray()
		{
			return InternalDecrypt().ToByteArray();
		}
		
		//! @endcond
	}
}
