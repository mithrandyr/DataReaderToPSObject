Public Class DataReaderToPSObject
    Private Class Map
        Public Ordinal As Integer
        Public Name As String
        Public AllowNull As Boolean
        Public DataType As String

        Public Sub New(ordinal As Integer, datatype As String, allownull As Boolean, Optional name As String = Nothing)
            If String.IsNullOrWhiteSpace(name) Then name = String.Format("Columne{0}", ordinal + 1)

            Me.Ordinal = ordinal
            Me.Name = name
            Me.AllowNull = allownull
            Me.DataType = datatype
        End Sub

    End Class

    Public Shared Iterator Function Translate(dataReader As IDataReader) As IEnumerable(Of PSObject)
        Dim MapList = dataReader.GetSchemaTable.AsEnumerable.Select(Function(x) New Map(x("ColumnOrdinal"), x("DataType").Name, x("AllowDBNull"), x("ColumnName"))).ToList

        While dataReader.Read
            Dim psObj As New PSObject
            For Each m As Map In MapList
                With psObj.Members
                    If m.AllowNull AndAlso dataReader.IsDBNull(m.Ordinal) Then
                        .Add(New PSNoteProperty(m.Name, Nothing), True)
                    Else
                        Select Case m.DataType
                            Case "Boolean"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetBoolean(m.Ordinal)), True)
                            Case "Byte"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetByte(m.Ordinal)), True)
                            Case "Char"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetChar(m.Ordinal)), True)
                            Case "DateTime"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetDateTime(m.Ordinal)), True)
                            Case "Decimal"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetDecimal(m.Ordinal)), True)
                            Case "Double"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetDouble(m.Ordinal)), True)
                            Case "Single"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetFloat(m.Ordinal)), True)
                            Case "Guid"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetGuid(m.Ordinal)), True)
                            Case "Int16"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetInt16(m.Ordinal)), True)
                            Case "Int32"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetInt32(m.Ordinal)), True)
                            Case "Int64"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetInt64(m.Ordinal)), True)
                            Case "String"
                                .Add(New PSNoteProperty(m.Name, dataReader.GetString(m.Ordinal)), True)
                            Case Else
                                .Add(New PSNoteProperty(m.Name, dataReader.GetValue(m.Ordinal)), True)
                        End Select
                    End If
                End With
            Next
            Yield psObj
        End While
    End Function
End Class
