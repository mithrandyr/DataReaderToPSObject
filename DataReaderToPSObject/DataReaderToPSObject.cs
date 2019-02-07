using System;
using System.Collections.Generic;
using System.Data;
using System.Management.Automation;

public class DataReaderToPSObject
{
    private class Map
    {
        public int Ordinal;
        public string Name;
        // Public AllowNull As Boolean
        public string DataType;

        public Map(int ordinal, string datatype, string name = null) // allownull As Boolean, 
        {
            if (string.IsNullOrWhiteSpace(name))
                name = string.Format("Column{0}", ordinal + 1);

            this.Ordinal = ordinal;
            this.Name = name;
            // Me.AllowNull = allownull
            this.DataType = datatype;
        }
    }

    public static IEnumerable<PSObject> Translate(System.Data.Common.DbDataReader dataReader, Boolean ProviderTypes)
    {
        List<Map> MapList = new List<Map>();
        int Ord = 0;
        foreach (var x in dataReader.GetSchemaTable().Select("", "ColumnOrdinal"))
        {
            MapList.Add(new Map(Ord, x["DataType"].ToString(), x["ColumnName"].ToString())); // x("AllowDBNull"),
            Ord += 1;
        }

        PSObject responseObject = new PSObject();
        while (dataReader.Read())
        {
            PSObject psObj = new PSObject();
            foreach (Map m in MapList)
            {
                {
                    var withBlock = psObj.Members;
                    if (dataReader.IsDBNull(m.Ordinal))
                        withBlock.Add(new PSNoteProperty(m.Name, null), true);
                    else
                        try
                        {
                            if (ProviderTypes)
                                withBlock.Add(new PSNoteProperty(m.Name, dataReader.GetProviderSpecificValue(m.Ordinal)), true);
                            else
                                withBlock.Add(new PSNoteProperty(m.Name, dataReader.GetValue(m.Ordinal)), true);
                        }
                        catch (Exception ex)
                        {
                            string msg = string.Format("Failed to translate, ColumnName = {0} | ColumnOrdinal = {1} | ColumnType = {2} | ToStringValue = '{3}' | See InnerException for details", m.Name, m.Ordinal, m.DataType, dataReader.GetValue(m.Ordinal).ToString());
                            throw new Exception(msg, ex);
                        }
                }
            }
            yield return psObj;
        }
    }
}