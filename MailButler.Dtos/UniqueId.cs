using System.Globalization;

namespace MailButler.Dtos;

/// <summary>A unique identifier.</summary>
/// <remarks>
///     Represents a unique identifier for messages in a <see cref="T:MailKit.IMailFolder" />.
/// </remarks>
public struct UniqueId : IComparable<UniqueId>, IEquatable<UniqueId>
{
	/// <summary>
	///     The invalid <see cref="T:MailKit.UniqueId" /> value.
	/// </summary>
	/// <remarks>
	///     The invalid <see cref="T:MailKit.UniqueId" /> value.
	/// </remarks>
	public static readonly UniqueId Invalid;

	/// <summary>
	///     The minimum <see cref="T:MailKit.UniqueId" /> value.
	/// </summary>
	/// <remarks>
	///     The minimum <see cref="T:MailKit.UniqueId" /> value.
	/// </remarks>
	public static readonly UniqueId MinValue = new(1U);

	/// <summary>
	///     The maximum <see cref="T:MailKit.UniqueId" /> value.
	/// </summary>
	/// <remarks>
	///     The maximum <see cref="T:MailKit.UniqueId" /> value.
	/// </remarks>
	public static readonly UniqueId MaxValue = new(uint.MaxValue);

	/// <summary>
	///     Initializes a new instance of the <see cref="T:MailKit.UniqueId" /> struct.
	/// </summary>
	/// <remarks>
	///     Creates a new <see cref="T:MailKit.UniqueId" /> with the specified validity and value.
	/// </remarks>
	/// <param name="validity">The uid validity.</param>
	/// <param name="id">The unique identifier.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///     <paramref name="id" /> is <c>0</c>.
	/// </exception>
	public UniqueId(uint validity, uint id)
	{
		if (id == 0U)
			throw new ArgumentOutOfRangeException(nameof(id));
		Validity = validity;
		Id = id;
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="T:MailKit.UniqueId" /> struct.
	/// </summary>
	/// <remarks>
	///     Creates a new <see cref="T:MailKit.UniqueId" /> with the specified value.
	/// </remarks>
	/// <param name="id">The unique identifier.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///     <paramref name="id" /> is <c>0</c>.
	/// </exception>
	public UniqueId(uint id)
	{
		if (id == 0U)
			throw new ArgumentOutOfRangeException(nameof(id));
		Validity = 0U;
		Id = id;
	}

	/// <summary>Gets the identifier.</summary>
	/// <remarks>The identifier.</remarks>
	/// <value>The identifier.</value>
	public uint Id { get; }

	/// <summary>Gets the validity, if non-zero.</summary>
	/// <remarks>Gets the UidValidity of the containing folder.</remarks>
	/// <value>The UidValidity of the containing folder.</value>
	public uint Validity { get; }

	/// <summary>Gets whether or not the unique identifier is valid.</summary>
	/// <remarks>Gets whether or not the unique identifier is valid.</remarks>
	/// <value><c>true</c> if the unique identifier is valid; otherwise, <c>false</c>.</value>
	public bool IsValid => Id > 0U;

	/// <summary>
	///     Compares two <see cref="T:MailKit.UniqueId" /> objects.
	/// </summary>
	/// <remarks>
	///     Compares two <see cref="T:MailKit.UniqueId" /> objects.
	/// </remarks>
	/// <returns>
	///     A value less than <c>0</c> if this <see cref="T:MailKit.UniqueId" /> is less than <paramref name="other" />,
	///     a value of <c>0</c> if this <see cref="T:MailKit.UniqueId" /> is equal to <paramref name="other" />, or
	///     a value greater than <c>0</c> if this <see cref="T:MailKit.UniqueId" /> is greater than <paramref name="other" />.
	/// </returns>
	/// <param name="other">The other unique identifier.</param>
	public int CompareTo(UniqueId other)
	{
		return Id.CompareTo(other.Id);
	}

	/// <summary>
	///     Determines whether the specified <see cref="T:MailKit.UniqueId" /> is equal to the current
	///     <see cref="T:MailKit.UniqueId" />.
	/// </summary>
	/// <remarks>
	///     Determines whether the specified <see cref="T:MailKit.UniqueId" /> is equal to the current
	///     <see cref="T:MailKit.UniqueId" />.
	/// </remarks>
	/// <param name="other">
	///     The <see cref="T:MailKit.UniqueId" /> to compare with the current <see cref="T:MailKit.UniqueId" />
	///     .
	/// </param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="T:MailKit.UniqueId" /> is equal to the current
	///     <see cref="T:MailKit.UniqueId" />; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals(UniqueId other)
	{
		return (int)other.Id == (int)Id;
	}

	/// <summary>Determines whether two unique identifiers are equal.</summary>
	/// <remarks>Determines whether two unique identifiers are equal.</remarks>
	/// <returns><c>true</c> if <paramref name="uid1" /> and <paramref name="uid2" /> are equal; otherwise, <c>false</c>.</returns>
	/// <param name="uid1">The first unique id to compare.</param>
	/// <param name="uid2">The second unique id to compare.</param>
	public static bool operator ==(UniqueId uid1, UniqueId uid2)
	{
		return (int)uid1.Id == (int)uid2.Id;
	}

	/// <summary>
	///     Determines whether one unique identifier is greater than another unique identifier.
	/// </summary>
	/// <remarks>
	///     Determines whether one unique identifier is greater than another unique identifier.
	/// </remarks>
	/// <returns><c>true</c> if <paramref name="uid1" /> is greater than <paramref name="uid2" />; otherwise, <c>false</c>.</returns>
	/// <param name="uid1">The first unique id to compare.</param>
	/// <param name="uid2">The second unique id to compare.</param>
	public static bool operator >(UniqueId uid1, UniqueId uid2)
	{
		return uid1.Id > uid2.Id;
	}

	/// <summary>
	///     Determines whether one unique identifier is greater than or equal to another unique identifier.
	/// </summary>
	/// <remarks>
	///     Determines whether one unique identifier is greater than or equal to another unique identifier.
	/// </remarks>
	/// <returns>
	///     <c>true</c> if <paramref name="uid1" /> is greater than or equal to <paramref name="uid2" />; otherwise,
	///     <c>false</c>.
	/// </returns>
	/// <param name="uid1">The first unique id to compare.</param>
	/// <param name="uid2">The second unique id to compare.</param>
	public static bool operator >=(UniqueId uid1, UniqueId uid2)
	{
		return uid1.Id >= uid2.Id;
	}

	/// <summary>
	///     Determines whether two unique identifiers are not equal.
	/// </summary>
	/// <remarks>
	///     Determines whether two unique identifiers are not equal.
	/// </remarks>
	/// <returns><c>true</c> if <paramref name="uid1" /> and <paramref name="uid2" /> are not equal; otherwise, <c>false</c>.</returns>
	/// <param name="uid1">The first unique id to compare.</param>
	/// <param name="uid2">The second unique id to compare.</param>
	public static bool operator !=(UniqueId uid1, UniqueId uid2)
	{
		return (int)uid1.Id != (int)uid2.Id;
	}

	/// <summary>
	///     Determines whether one unique identifier is less than another unique identifier.
	/// </summary>
	/// <remarks>
	///     Determines whether one unique identifier is less than another unique identifier.
	/// </remarks>
	/// <returns><c>true</c> if <paramref name="uid1" /> is less than <paramref name="uid2" />; otherwise, <c>false</c>.</returns>
	/// <param name="uid1">The first unique id to compare.</param>
	/// <param name="uid2">The second unique id to compare.</param>
	public static bool operator <(UniqueId uid1, UniqueId uid2)
	{
		return uid1.Id < uid2.Id;
	}

	/// <summary>
	///     Determines whether one unique identifier is less than or equal to another unique identifier.
	/// </summary>
	/// <remarks>
	///     Determines whether one unique identifier is less than or equal to another unique identifier.
	/// </remarks>
	/// <returns>
	///     <c>true</c> if <paramref name="uid1" /> is less than or equal to <paramref name="uid2" />; otherwise,
	///     <c>false</c>.
	/// </returns>
	/// <param name="uid1">The first unique id to compare.</param>
	/// <param name="uid2">The second unique id to compare.</param>
	public static bool operator <=(UniqueId uid1, UniqueId uid2)
	{
		return uid1.Id <= uid2.Id;
	}

	/// <summary>
	///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
	///     <see cref="T:MailKit.UniqueId" />.
	/// </summary>
	/// <remarks>
	///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
	///     <see cref="T:MailKit.UniqueId" />.
	/// </remarks>
	/// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:MailKit.UniqueId" />.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="T:System.Object" /> is equal to the current
	///     <see cref="T:MailKit.UniqueId" />;
	///     otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals(object obj)
	{
		return obj is UniqueId uniqueId && (int)uniqueId.Id == (int)Id;
	}

	/// <summary>
	///     Serves as a hash function for a <see cref="T:MailKit.UniqueId" /> object.
	/// </summary>
	/// <remarks>
	///     Serves as a hash function for a <see cref="T:MailKit.UniqueId" /> object.
	/// </remarks>
	/// <returns>
	///     A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
	///     hash table.
	/// </returns>
	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}

	/// <summary>
	///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:MailKit.UniqueId" />.
	/// </summary>
	/// <remarks>
	///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:MailKit.UniqueId" />.
	/// </remarks>
	/// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:MailKit.UniqueId" />.</returns>
	public override string ToString()
	{
		return Id.ToString(CultureInfo.InvariantCulture);
	}

	/// <summary>Attempt to parse a unique identifier.</summary>
	/// <remarks>Attempts to parse a unique identifier.</remarks>
	/// <returns><c>true</c> if the unique identifier was successfully parsed; otherwise, <c>false.</c>.</returns>
	/// <param name="token">The token to parse.</param>
	/// <param name="index">The index to start parsing.</param>
	/// <param name="uid">The unique identifier.</param>
	internal static bool TryParse(string token, ref int index, out uint uid)
	{
		uint num1 = 0;
		while (index < token.Length)
		{
			var ch = token[index];
			switch (ch)
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					var num2 = ch - 48U;
					if (num1 > 429496729U || (num1 == 429496729U && num2 > 5U))
					{
						uid = 0U;
						return false;
					}

					num1 = num1 * 10U + num2;
					++index;
					continue;
				default:
					goto label_6;
			}
		}

		label_6:
		uid = num1;
		return uid > 0U;
	}

	/// <summary>Attempt to parse a unique identifier.</summary>
	/// <remarks>Attempts to parse a unique identifier.</remarks>
	/// <returns><c>true</c> if the unique identifier was successfully parsed; otherwise, <c>false.</c>.</returns>
	/// <param name="token">The token to parse.</param>
	/// <param name="validity">The UIDVALIDITY value.</param>
	/// <param name="uid">The unique identifier.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///     <paramref name="token" /> is <c>null</c>.
	/// </exception>
	public static bool TryParse(string token, uint validity, out UniqueId uid)
	{
		if (token == null)
			throw new ArgumentNullException(nameof(token));
		uint result;
		if (!uint.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out result) ||
		    result == 0U)
		{
			uid = Invalid;
			return false;
		}

		uid = new UniqueId(validity, result);
		return true;
	}

	/// <summary>Attempt to parse a unique identifier.</summary>
	/// <remarks>Attempts to parse a unique identifier.</remarks>
	/// <returns><c>true</c> if the unique identifier was successfully parsed; otherwise, <c>false.</c>.</returns>
	/// <param name="token">The token to parse.</param>
	/// <param name="uid">The unique identifier.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///     <paramref name="token" /> is <c>null</c>.
	/// </exception>
	public static bool TryParse(string token, out UniqueId uid)
	{
		return TryParse(token, 0U, out uid);
	}

	/// <summary>Parse a unique identifier.</summary>
	/// <remarks>Parses a unique identifier.</remarks>
	/// <returns>The unique identifier.</returns>
	/// <param name="token">A string containing the unique identifier.</param>
	/// <param name="validity">The UIDVALIDITY.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///     <paramref name="token" /> is <c>null</c>.
	/// </exception>
	/// <exception cref="T:System.FormatException">
	///     <paramref name="token" /> is not in the correct format.
	/// </exception>
	/// <exception cref="T:System.OverflowException">
	///     The unique identifier is greater than <see cref="F:MailKit.UniqueId.MaxValue" />.
	/// </exception>
	public static UniqueId Parse(string token, uint validity)
	{
		return new UniqueId(validity,
			uint.Parse(token, NumberStyles.None, CultureInfo.InvariantCulture));
	}

	/// <summary>Parse a unique identifier.</summary>
	/// <remarks>Parses a unique identifier.</remarks>
	/// <returns>The unique identifier.</returns>
	/// <param name="token">A string containing the unique identifier.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///     <paramref name="token" /> is <c>null</c>.
	/// </exception>
	/// <exception cref="T:System.FormatException">
	///     <paramref name="token" /> is not in the correct format.
	/// </exception>
	/// <exception cref="T:System.OverflowException">
	///     The unique identifier is greater than <see cref="F:MailKit.UniqueId.MaxValue" />.
	/// </exception>
	public static UniqueId Parse(string token)
	{
		return new UniqueId(uint.Parse(token, NumberStyles.None, CultureInfo.InvariantCulture));
	}
}