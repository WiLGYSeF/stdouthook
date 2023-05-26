using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles.Loaders
{
    public class InvalidPropertyTypeException : ProfileLoaderException
    {
        public string PropertyName { get; }

        public string InvalidType { get; }

        public IReadOnlyList<string> ExpectedTypes { get; }

        public InvalidPropertyTypeException(string propertyName, string invalidType, IReadOnlyList<string> expectedTypes)
            : base($"{propertyName} is type of {invalidType}, but expects: {string.Join(", ", expectedTypes)}")
        {
            PropertyName = propertyName;
            InvalidType = invalidType;
            ExpectedTypes = expectedTypes;
        }
    }
}
