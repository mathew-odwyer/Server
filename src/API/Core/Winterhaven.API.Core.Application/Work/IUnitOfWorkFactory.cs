namespace Winterhaven.API.Core.Application.Work;

/// <summary>
///   Defines an interface that represents a factory for creating units of work.
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    ///   Creates a new unit of work.
    /// </summary>
    /// <returns>
    ///   Returns an <see cref="IUnitOfWork"/> that represents the newly created unit of work.
    /// </returns>
    public IUnitOfWork CreateUnitOfWork();
}