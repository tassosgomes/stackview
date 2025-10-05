namespace StackShare.Domain.Exceptions;

public class TechnologyAlreadyExistsException : Exception
{
    public string TechnologyName { get; }

    public TechnologyAlreadyExistsException(string technologyName) 
        : base($"Já existe uma tecnologia com o nome '{technologyName}'")
    {
        TechnologyName = technologyName;
    }

    public TechnologyAlreadyExistsException(string technologyName, Exception innerException) 
        : base($"Já existe uma tecnologia com o nome '{technologyName}'", innerException)
    {
        TechnologyName = technologyName;
    }
}