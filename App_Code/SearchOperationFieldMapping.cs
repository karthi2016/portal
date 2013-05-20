using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// The class lists search operation attributes for each type of field data type.
/// </summary>
/// <remarks>Notice that this class INVERTS what's in the ConsoleSearchOperationsMapping.data.xml
/// file - in that file, the XML is grouped by search operation, but here, we're grouping 
/// by FieldDataType. This is intentional.</remarks>
public class SearchOperationFieldMapping
{
    public SearchOperationFieldMapping()
    {
        ApplicableOperations = new List<string>();
        PredicateMapping = new Dictionary<string, string>();
    }

    /// <summary>
    /// Gets or sets the applicable operations for a given field data type
    /// </summary>
    /// <value>The applicable operations.</value>
    public List<string> ApplicableOperations { get; set; }

    /// <summary>
    /// Gets or sets the predicate mapping for a 
    /// </summary>
    /// <value>The predicate mapping.</value>
    /// <remarks>This mapping keys off of the operation - so for a given field mapping and operation,
    /// you should be able to find out which predicate to show. This information is used by SearchView.cs</remarks>
    public Dictionary<string, string> PredicateMapping { get; set; }
}