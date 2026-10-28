#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace ZeroX.SaveGameSystem.Security
{
	public partial struct EBool : IEquatable<EBool>, IEquatable<bool>, IComparable<EBool>, IComparable<bool>, IComparable
	{
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public override bool Equals(object other)
		{
			return other is EBool o && Equals(o) ||
				   other is bool r && Equals(r);
		}
		
		public bool Equals(EBool obj)
		{
			return currentCryptoKey == obj.currentCryptoKey ? hiddenValue.Equals(obj.hiddenValue) : 
				InternalDecrypt().Equals(obj.InternalDecrypt());
		}
		
		public bool Equals(bool other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(EBool other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(bool other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}