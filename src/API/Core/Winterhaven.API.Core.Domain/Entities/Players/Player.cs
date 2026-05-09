namespace Winterhaven.API.Core.Domain.Entities.Players;

/// <summary>
/// Represents a player.
/// </summary>
/// <seealso cref="EntityBase"/>
public class Player : AuditableEntityBase
{
    /// <summary>
    /// Gets or sets a <see cref="string"/> that represents the name of the <see cref="Player"/>.
    /// </summary>
    /// <value>The <see cref="string"/> that represents the name of the <see cref="Player"/>.</value>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets an <see cref="double"/> that represents the X-coordinate of the <see cref="Player"/>.
    /// </summary>
    /// <value>The <see cref="double"/> that represents the X-coordinate of the <see cref="Player"/>.</value>
    public double X { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="double"/> that represents the Y-coordinate of the <see cref="Player"/>.
    /// </summary>
    /// <value>The <see cref="double"/> that represents the Y-coordinate of the <see cref="Player"/>.</value>
    public double Y { get; set; }
}