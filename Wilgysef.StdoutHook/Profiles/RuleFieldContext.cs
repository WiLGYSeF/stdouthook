using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles;

/// <summary>
/// Context for field rule.
/// </summary>
internal class RuleFieldContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RuleFieldContext"/> class.
    /// </summary>
    /// <param name="fieldsWithSeparators">Fields with separators in between.</param>
    public RuleFieldContext(IReadOnlyList<string> fieldsWithSeparators)
    {
        var fieldCount = fieldsWithSeparators.Count / 2 + 1;
        var fields = new string[fieldCount];
        var fieldSeparators = new string[fieldsWithSeparators.Count - fieldCount];

        Fields = fields;
        FieldSeparators = fieldSeparators;

        var fieldIndex = 0;
        var fieldSeparatorIndex = 0;

        for (var i = 0; i < fieldsWithSeparators.Count; i++)
        {
            if ((i & 1) == 0)
            {
                fields[fieldIndex++] = fieldsWithSeparators[i];
            }
            else
            {
                fieldSeparators[fieldSeparatorIndex++] = fieldsWithSeparators[i];
            }
        }
    }

    /// <summary>
    /// Fields.
    /// </summary>
    public IReadOnlyList<string> Fields { get; }

    /// <summary>
    /// Field separators.
    /// </summary>
    public IReadOnlyList<string> FieldSeparators { get; }

    /// <summary>
    /// Current field number.
    /// </summary>
    public int CurrentFieldNumber { get; set; } = 1;

    /// <summary>
    /// Whether to increment <see cref="CurrentFieldNumber"/> when calling <see cref="GetCurrentFieldNumber"/>.
    /// </summary>
    public bool IncrementFieldNumberOnGet { get; set; }

    /// <summary>
    /// Gets the current field number.
    /// </summary>
    /// <remarks>
    /// Increments <see cref="CurrentFieldNumber"/> if <see cref="IncrementFieldNumberOnGet"/> is <see langword="true"/>.
    /// </remarks>
    /// <returns>Current field number.</returns>
    public int GetCurrentFieldNumber()
    {
#pragma warning disable SA1003 // Symbols should be spaced correctly
        return IncrementFieldNumberOnGet
            ? CurrentFieldNumber++
            : CurrentFieldNumber;
#pragma warning restore SA1003 // Symbols should be spaced correctly
    }
}
