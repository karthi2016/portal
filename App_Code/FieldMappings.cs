using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;
using MemberSuite.SDK.Web.Controls.CascadingDropDown;

/// <summary>
/// Summary description for FieldDisplayMappings
/// </summary>
public static class FieldMappings
{
    private static Dictionary<FieldDataType, List<FieldDisplayType>> _dataMappingToDisplayCache;
    private static Dictionary<FieldDisplayType, List<FieldDataType>> _displayMappingToDataCache;

    static FieldMappings()
    {
        initializeMappingsCache();
    }

    private static void initializeMappingsCache()
    {
        _displayMappingToDataCache = new Dictionary<FieldDisplayType, List<FieldDataType>>();
        _dataMappingToDisplayCache = new Dictionary<FieldDataType, List<FieldDisplayType>>();

        // let's load up the mapping
        string resourceName = "MemberSuite.SDK.Web.Controls.FieldDataDisplayMapping.xml";
        XDocument xd = EmbeddedResource.LoadAsXmlLinq(resourceName, Assembly.GetAssembly(typeof(CascadingDropDownManager)));

        // iterate through all of the data types
        IEnumerable<XElement> displayTypes = from m in xd.Descendants("DisplayType")
                                             select m;

        // now, let's add them to a dictionary
        foreach (XElement displayType in displayTypes)
        {
            var dependencies = new List<FieldDataType>();
            XAttribute dtAttr = displayType.Attribute("Value");

            if (dtAttr == null)
                throw new ApplicationException("No value specified for " + displayType.Value);

            var displayTypeName = dtAttr.Value.ToEnum<FieldDisplayType>();

            // go through all of the dependencies tied to the current display type
            foreach (string d in (from d in displayType.Descendants("Dependency")
                                  select d.Value))
            {
                // ok, first add the dependcy, easy
                var dataType = d.ToEnum<FieldDataType>();
                dependencies.Add(dataType);

                // now - do we have a list for this data type already
                List<FieldDisplayType> relatedDisplayTypes;

                if (!_dataMappingToDisplayCache.TryGetValue(dataType, out relatedDisplayTypes))
                {
                    relatedDisplayTypes = new List<FieldDisplayType>();
                    _dataMappingToDisplayCache.Add(dataType, relatedDisplayTypes);
                }

                // now add the current display type
                relatedDisplayTypes.Add(displayTypeName);
            }

            _displayMappingToDataCache[displayTypeName] = dependencies;
        }
    }

    public static List<FieldDisplayType> GetDependentDisplayTypesFor(FieldDataType dataType)
    {
        List<FieldDisplayType> relatedDisplayTypes;

        if (!_dataMappingToDisplayCache.TryGetValue(dataType, out relatedDisplayTypes))
            return new List<FieldDisplayType>(); // emptty list

        return relatedDisplayTypes;
    }
}