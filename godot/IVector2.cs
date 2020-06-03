using Godot;
using Nova;
using System;
using System.Collections.Generic;

namespace Nova {

	/// <summary>
	/// Represents a two dimensional integer vector.
	/// </summary>
	public struct IVector2 {

		public static IVector2 Zero => new IVector2(0, 0);
		public static IVector2 One => new IVector2(1, 1);

		public int x;
		public int y;

		public IVector2(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public IVector2(IVector2 copy) : this(copy.x, copy.y) { }

		public static IVector2 operator -(IVector2 vec) => new IVector2(-vec.x, -vec.y);

		public static IVector2 operator +(IVector2 vec, int amount) => new IVector2(vec.x + amount, vec.y + amount);
		public static IVector2 operator +(int amount, IVector2 vec) => vec + amount;

		public static IVector2 operator -(IVector2 vec, int amount) => new IVector2(vec.x - amount, vec.y - amount);

		public static IVector2 operator *(IVector2 vec, int scale) => new IVector2(vec.x * scale, vec.y * scale);
		public static IVector2 operator *(int scale, IVector2 vec) => vec * scale;

		public static IVector2 operator /(IVector2 vec, int div) => new IVector2(vec.x / div, vec.y / div);

		public static IVector2 operator %(IVector2 vec, int mod) => new IVector2(vec.x % mod, vec.y % mod);

		public static implicit operator Vector2(IVector2 vec) => new Vector2(vec.x, vec.y);

	}

}